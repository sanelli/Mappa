// <copyright file="MappaInterceptorCallSiteAnalyzer.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Discovers interceptor <c>MappaMap</c> call sites, collates trailing <c>With</c> configuration,
/// and reports related diagnostics.
/// </summary>
internal static class MappaInterceptorCallSiteAnalyzer
{
    private const string WithMethodName = "With";
    private const string SettingsParameterName = "mappaSettings";
    private const string StaticDependenciesParameterName = "mappaStaticDependencies";
    private const string MethodAttributesParameterName = "mappaMethodAttributes";

    private static readonly HashSet<string> UnsupportedAttributeMetadataNames = new(StringComparer.Ordinal)
    {
        "Mappa.Attributes.MappaProjectionAttribute",
    };

    /// <summary>
    /// Analyzes the interceptor class for mapping call sites.
    /// </summary>
    /// <param name="classDeclarationSyntax">The interceptor class declaration.</param>
    /// <param name="classSymbol">The interceptor class symbol.</param>
    /// <param name="methodName">The interceptable method name.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="reportDiagnostic">Callback used to report diagnostics.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The discovered call sites.</returns>
    internal static ImmutableArray<MappaInterceptorCallSite> Analyze(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        string methodName,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        var classStaticDependencies = classSymbol.GetAttributes().GetMappaStaticDependencies(compilation);
        var results = ImmutableArray.CreateBuilder<MappaInterceptorCallSite>();

        foreach (var invocation in classDeclarationSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryMatchMapInvocation(
                    invocation,
                    methodName,
                    semanticModel,
                    cancellationToken,
                    out var targetType,
                    out var sourceType,
                    out var hasContext))
            {
                continue;
            }

            var interceptableLocation = semanticModel.GetInterceptableLocation(invocation, cancellationToken);
            var withInvocations = CollectTrailingWithInvocations(invocation);
            var parseSucceeded = TryParseWithChain(
                withInvocations,
                semanticModel,
                compilation,
                reportDiagnostic,
                out var settings,
                out var withStaticDependencies,
                out var methodAttributes,
                out var methodAttributeTypes,
                cancellationToken);

            var hasErrors = !parseSucceeded;
            hasErrors |= CollateAndValidate(
                invocation,
                targetType,
                sourceType,
                methodName,
                settings,
                classStaticDependencies,
                withStaticDependencies,
                methodAttributeTypes,
                compilation,
                reportDiagnostic,
                out var dedupedStaticDependencies);

            results.Add(new MappaInterceptorCallSite(
                invocation,
                interceptableLocation,
                targetType,
                sourceType,
                hasContext,
                settings.ToImmutableArray(),
                dedupedStaticDependencies,
                methodAttributes.ToImmutableArray(),
                hasErrors));
        }

        return results.ToImmutable();
    }

    private static bool TryMatchMapInvocation(
        InvocationExpressionSyntax invocation,
        string methodName,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        out ITypeSymbol targetType,
        out ITypeSymbol sourceType,
        out bool hasContext)
    {
        targetType = null!;
        sourceType = null!;
        hasContext = false;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name is not GenericNameSyntax genericName
            || genericName.TypeArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        if (!genericName.Identifier.Text.Equals(methodName, StringComparison.Ordinal))
        {
            return false;
        }

        var argumentCount = invocation.ArgumentList.Arguments.Count;
        if (argumentCount is not (1 or 2))
        {
            return false;
        }

        var typeArgument = genericName.TypeArgumentList.Arguments[0];
        var resolvedTarget = semanticModel.GetTypeInfo(typeArgument, cancellationToken).Type;
        var sourceArgument = invocation.ArgumentList.Arguments[0].Expression;
        var resolvedSource = semanticModel.GetTypeInfo(sourceArgument, cancellationToken).Type;
        if (resolvedTarget is null || resolvedSource is null)
        {
            return false;
        }

        targetType = resolvedTarget;
        sourceType = resolvedSource;
        hasContext = argumentCount == 2;
        return true;
    }

    private static ImmutableArray<InvocationExpressionSyntax> CollectTrailingWithInvocations(
        InvocationExpressionSyntax mapInvocation)
    {
        var builder = ImmutableArray.CreateBuilder<InvocationExpressionSyntax>();
        ExpressionSyntax current = mapInvocation;

        while (true)
        {
            if (current.Parent is not MemberAccessExpressionSyntax memberAccess
                || memberAccess.Name is not IdentifierNameSyntax identifier
                || !identifier.Identifier.Text.Equals(WithMethodName, StringComparison.Ordinal)
                || !ReferenceEquals(memberAccess.Expression, current)
                || memberAccess.Parent is not InvocationExpressionSyntax withInvocation)
            {
                break;
            }

            builder.Add(withInvocation);
            current = withInvocation;
        }

        return builder.ToImmutable();
    }

    private static bool TryParseWithChain(
        ImmutableArray<InvocationExpressionSyntax> withInvocations,
        SemanticModel semanticModel,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        out List<MappaSettingsAttribute> settings,
        out List<(INamedTypeSymbol Type, Location Location)> withStaticDependencies,
        out List<Attribute> methodAttributes,
        out List<(INamedTypeSymbol Type, Location Location)> methodAttributeTypes,
        CancellationToken cancellationToken)
    {
        settings = [];
        withStaticDependencies = [];
        methodAttributes = [];
        methodAttributeTypes = [];
        var success = true;

        for (var withIndex = 0; withIndex < withInvocations.Length; withIndex++)
        {
            var arguments = withInvocations[withIndex].ArgumentList.Arguments;
            for (var argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
            {
                var argument = arguments[argumentIndex];
                var parameterName = argument.NameColon?.Name.Identifier.Text
                    ?? GetPositionalParameterName(argumentIndex);
                if (parameterName is null)
                {
                    continue;
                }

                if (argument.Expression.IsKind(SyntaxKind.NullLiteralExpression)
                    || argument.Expression.IsKind(SyntaxKind.DefaultLiteralExpression))
                {
                    continue;
                }

                switch (parameterName)
                {
                    case SettingsParameterName:
                        success &= TryParseSettingsArgument(
                            argument.Expression,
                            semanticModel,
                            reportDiagnostic,
                            settings,
                            cancellationToken);
                        break;
                    case StaticDependenciesParameterName:
                        success &= TryParseStaticDependenciesArgument(
                            argument.Expression,
                            semanticModel,
                            reportDiagnostic,
                            withStaticDependencies,
                            cancellationToken);
                        break;
                    case MethodAttributesParameterName:
                        success &= TryParseMethodAttributesArgument(
                            argument.Expression,
                            semanticModel,
                            compilation,
                            reportDiagnostic,
                            methodAttributes,
                            methodAttributeTypes,
                            cancellationToken);
                        break;
                }
            }
        }

        return success;
    }

    private static string? GetPositionalParameterName(int argumentIndex)
        => argumentIndex switch
        {
            0 => SettingsParameterName,
            1 => StaticDependenciesParameterName,
            2 => MethodAttributesParameterName,
            _ => null,
        };

    private static bool TryParseSettingsArgument(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        Action<Diagnostic> reportDiagnostic,
        List<MappaSettingsAttribute> settings,
        CancellationToken cancellationToken)
    {
        if (expression is not ObjectCreationExpressionSyntax creation)
        {
            reportDiagnostic(MappaDiagnostics.MappaInterceptorWithArgumentMustBeObjectCreation(
                expression.GetLocation(),
                expression.ToString()));
            return false;
        }

        settings.Add(MappaInterceptorAttributeSyntaxMaterializer.MaterializeSettings(
            creation,
            semanticModel,
            cancellationToken));
        return true;
    }

    private static bool TryParseStaticDependenciesArgument(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        Action<Diagnostic> reportDiagnostic,
        List<(INamedTypeSymbol Type, Location Location)> withStaticDependencies,
        CancellationToken cancellationToken)
    {
        if (!TryGetObjectCreations(expression, reportDiagnostic, out var creations))
        {
            return false;
        }

        var success = true;
        foreach (var creation in creations)
        {
            var dependencyType = MappaInterceptorAttributeSyntaxMaterializer.TryMaterializeStaticDependencyType(
                creation,
                semanticModel,
                cancellationToken);
            if (dependencyType is null)
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorWithArgumentMustBeObjectCreation(
                    creation.GetLocation(),
                    creation.ToString()));
                success = false;
                continue;
            }

            withStaticDependencies.Add((dependencyType, creation.GetLocation()));
        }

        return success;
    }

    private static bool TryParseMethodAttributesArgument(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        List<Attribute> methodAttributes,
        List<(INamedTypeSymbol Type, Location Location)> methodAttributeTypes,
        CancellationToken cancellationToken)
    {
        if (!TryGetObjectCreations(expression, reportDiagnostic, out var creations))
        {
            return false;
        }

        var success = true;
        foreach (var creation in creations)
        {
            var attributeType = semanticModel.GetTypeInfo(creation, cancellationToken).Type as INamedTypeSymbol
                ?? semanticModel.GetSymbolInfo(creation.Type, cancellationToken).Symbol as INamedTypeSymbol;
            if (attributeType is null)
            {
                success = false;
                continue;
            }

            if (!MappaInterceptorAttributeSyntaxMaterializer.ImplementsIMappaMethodAttribute(attributeType, compilation))
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorInvalidMethodAttributeType(
                    creation.GetLocation(),
                    attributeType.ToDisplayString()));
                success = false;
                continue;
            }

            methodAttributeTypes.Add((attributeType, creation.GetLocation()));
            var materialized = MappaInterceptorAttributeSyntaxMaterializer.TryMaterializeMethodAttribute(
                creation,
                attributeType,
                semanticModel,
                cancellationToken);
            if (materialized is not null)
            {
                methodAttributes.Add(materialized);
            }
        }

        return success;
    }

    private static bool TryGetObjectCreations(
        ExpressionSyntax expression,
        Action<Diagnostic> reportDiagnostic,
        out List<ObjectCreationExpressionSyntax> creations)
    {
        creations = [];
        switch (expression)
        {
            case ArrayCreationExpressionSyntax { Initializer: { } arrayInitializer }:
                return TryAddCreationsFromInitializer(arrayInitializer.Expressions, reportDiagnostic, creations);
            case ImplicitArrayCreationExpressionSyntax { Initializer: { } implicitInitializer }:
                return TryAddCreationsFromInitializer(implicitInitializer.Expressions, reportDiagnostic, creations);
            case CollectionExpressionSyntax collectionExpression:
                return TryAddCreationsFromCollectionExpression(collectionExpression, reportDiagnostic, creations);
            default:
                reportDiagnostic(MappaDiagnostics.MappaInterceptorWithArgumentMustBeObjectCreation(
                    expression.GetLocation(),
                    expression.ToString()));
                return false;
        }
    }

    private static bool TryAddCreationsFromInitializer(
        SeparatedSyntaxList<ExpressionSyntax> expressions,
        Action<Diagnostic> reportDiagnostic,
        List<ObjectCreationExpressionSyntax> creations)
    {
        var success = true;
        foreach (var expression in expressions)
        {
            if (expression is ObjectCreationExpressionSyntax creation)
            {
                creations.Add(creation);
            }
            else
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorWithArgumentMustBeObjectCreation(
                    expression.GetLocation(),
                    expression.ToString()));
                success = false;
            }
        }

        return success;
    }

    private static bool TryAddCreationsFromCollectionExpression(
        CollectionExpressionSyntax collectionExpression,
        Action<Diagnostic> reportDiagnostic,
        List<ObjectCreationExpressionSyntax> creations)
    {
        var success = true;
        foreach (var element in collectionExpression.Elements)
        {
            if (element is ExpressionElementSyntax { Expression: ObjectCreationExpressionSyntax creation })
            {
                creations.Add(creation);
            }
            else
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorWithArgumentMustBeObjectCreation(
                    element.GetLocation(),
                    element.ToString()));
                success = false;
            }
        }

        return success;
    }

    private static bool CollateAndValidate(
        InvocationExpressionSyntax invocation,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string methodName,
        List<MappaSettingsAttribute> settings,
        INamedTypeSymbol[] classStaticDependencies,
        List<(INamedTypeSymbol Type, Location Location)> withStaticDependencies,
        List<(INamedTypeSymbol Type, Location Location)> methodAttributeTypes,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        out ImmutableArray<INamedTypeSymbol> dedupedStaticDependencies)
    {
        var hasErrors = false;
        var location = invocation.GetLocation();

        if (settings.Count > 1)
        {
            reportDiagnostic(MappaDiagnostics.MappaInterceptorMultipleSettings(location));
        }

        var seenDependencies = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var deduped = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
        foreach (var dependency in classStaticDependencies)
        {
            if (seenDependencies.Add(dependency))
            {
                deduped.Add(dependency);
            }
            else
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorDuplicateStaticDependencyType(
                    location,
                    dependency.ToDisplayString()));
            }
        }

        foreach (var (dependency, dependencyLocation) in withStaticDependencies)
        {
            if (seenDependencies.Add(dependency))
            {
                deduped.Add(dependency);
            }
            else
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorDuplicateStaticDependencyType(
                    dependencyLocation,
                    dependency.ToDisplayString()));
            }
        }

        dedupedStaticDependencies = deduped.ToImmutable();

        foreach (var group in methodAttributeTypes.GroupBy(
                     item => item.Type,
                     SymbolEqualityComparer.Default))
        {
            var attributeType = (INamedTypeSymbol)group.Key!;
            if (!MappaInterceptorAttributeSyntaxMaterializer.AllowsMultiple(attributeType)
                && group.Count() > 1)
            {
                foreach (var (_, attributeLocation) in group)
                {
                    reportDiagnostic(MappaDiagnostics.MappaInterceptorDuplicateNonMultipleAttribute(
                        attributeLocation,
                        attributeType.ToDisplayString()));
                    hasErrors = true;
                }
            }

            var metadataName = MappaInterceptorAttributeSyntaxMaterializer.StripGlobalPrefix(
                attributeType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            if (UnsupportedAttributeMetadataNames.Contains(metadataName))
            {
                reportDiagnostic(MappaDiagnostics.MappaInterceptorUnsupportedFeature(
                    group.First().Location,
                    attributeType.Name));
                hasErrors = true;
            }
        }

        if (targetType.IsAnyTaskType(compilation))
        {
            reportDiagnostic(MappaDiagnostics.MethodReturnsTaskType(location, methodName));
            hasErrors = true;
        }

        if (sourceType.IsOrImplementIQueryable(compilation)
            || targetType.IsOrImplementIQueryable(compilation))
        {
            reportDiagnostic(MappaDiagnostics.MappaInterceptorQueryableNotSupported(location));
            hasErrors = true;
        }

        return hasErrors;
    }
}