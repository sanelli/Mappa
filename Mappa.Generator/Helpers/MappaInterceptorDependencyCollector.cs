// <copyright file="MappaInterceptorDependencyCollector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Collects mapping dependency methods for the interceptor generation path.
/// </summary>
internal static class MappaInterceptorDependencyCollector
{
    /// <summary>
    /// Collects dependencies eligible for interceptor mapping.
    /// </summary>
    /// <param name="classContext">The interceptor class context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="hasMappaAttribute"><c>true</c> when the class also has <c>[Mappa]</c>.</param>
    /// <param name="staticDependencyTypes">Union of class and call-site static dependency types.</param>
    /// <param name="reportDiagnostic">Callback used to report diagnostics.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal static void Collect(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        bool hasMappaAttribute,
        IEnumerable<INamedTypeSymbol> staticDependencyTypes,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        CollectFromMappaDependencies(classContext, compilation, reportDiagnostic, cancellationToken);
        CollectFromStaticDependencies(
            classContext,
            compilation,
            staticDependencyTypes,
            reportDiagnostic,
            cancellationToken);
        CollectFromInterceptorMethodDependencies(classContext, compilation, cancellationToken);

        if (hasMappaAttribute)
        {
            CollectFromMapperMethods(classContext, compilation, cancellationToken);
            CollectFromTypeHierarchy(classContext, compilation, cancellationToken);
        }
    }

    private static void CollectFromMappaDependencies(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        var processedNames = new HashSet<string>(StringComparer.Ordinal);
        CollectDeclaredDependencyProperties(classContext, compilation, reportDiagnostic, processedNames, cancellationToken);
        CollectBaseDependencyProperties(classContext, compilation, reportDiagnostic, processedNames, cancellationToken);
        CollectDeclaredDependencyFields(classContext, compilation, reportDiagnostic, processedNames, cancellationToken);
        CollectBaseDependencyFields(classContext, compilation, reportDiagnostic, processedNames, cancellationToken);
    }

    private static void CollectDeclaredDependencyProperties(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        HashSet<string> processedNames,
        CancellationToken cancellationToken)
    {
        foreach (var propertyDeclaration in classContext.ClassDeclarationSyntax.ChildNodes().OfType<PropertyDeclarationSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (propertyDeclaration.AccessorList is null
                || !propertyDeclaration.AccessorList.Accessors.Any(static accessor => accessor.Kind() == SyntaxKind.GetAccessorDeclaration))
            {
                continue;
            }

            if (propertyDeclaration.AttributeLists.GetMappaDependencyAttributeSyntax(classContext.SemanticModel, cancellationToken) is null)
            {
                continue;
            }

            var propertySymbol = classContext.SemanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken) as IPropertySymbol;
            if (propertySymbol is null)
            {
                continue;
            }

            var propertyIdentifier = propertyDeclaration.Identifier.ToString();
            processedNames.Add(propertyIdentifier);
            ProcessDependencyType(
                propertyDeclaration,
                propertySymbol.Type,
                BuildDependencyAccessor(propertySymbol.IsStatic, propertySymbol.Type, propertyIdentifier),
                propertySymbol.IsStatic,
                propertyIdentifier,
                classContext,
                compilation,
                reportDiagnostic);
        }
    }

    private static void CollectBaseDependencyProperties(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        HashSet<string> processedNames,
        CancellationToken cancellationToken)
    {
        foreach (var propertySymbol in compilation.GetMappaDependencyPropertiesInMapperBaseTypeHierarchy(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!processedNames.Add(propertySymbol.Name))
            {
                continue;
            }

            ProcessDependencyType(
                classContext.ClassDeclarationSyntax,
                propertySymbol.Type,
                BuildDependencyAccessor(propertySymbol.IsStatic, propertySymbol.Type, propertySymbol.Name),
                propertySymbol.IsStatic,
                propertySymbol.Name,
                classContext,
                compilation,
                reportDiagnostic);
        }
    }

    private static void CollectDeclaredDependencyFields(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        HashSet<string> processedNames,
        CancellationToken cancellationToken)
    {
        foreach (var fieldDeclaration in classContext.ClassDeclarationSyntax.ChildNodes().OfType<FieldDeclarationSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (fieldDeclaration.AttributeLists.GetMappaDependencyAttributeSyntax(classContext.SemanticModel, cancellationToken) is null)
            {
                continue;
            }

            foreach (var variable in fieldDeclaration.Declaration.Variables.Take(1))
            {
                if (classContext.SemanticModel.GetDeclaredSymbol(variable, cancellationToken) is not IFieldSymbol fieldSymbol)
                {
                    continue;
                }

                var fieldIdentifier = variable.Identifier.ToString();
                processedNames.Add(fieldIdentifier);
                ProcessDependencyType(
                    fieldDeclaration,
                    fieldSymbol.Type,
                    BuildDependencyAccessor(fieldSymbol.IsStatic, fieldSymbol.Type, fieldIdentifier),
                    fieldSymbol.IsStatic,
                    fieldIdentifier,
                    classContext,
                    compilation,
                    reportDiagnostic);
            }
        }
    }

    private static void CollectBaseDependencyFields(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        HashSet<string> processedNames,
        CancellationToken cancellationToken)
    {
        foreach (var fieldSymbol in compilation.GetMappaDependencyFieldsInMapperBaseTypeHierarchy(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!processedNames.Add(fieldSymbol.Name))
            {
                continue;
            }

            ProcessDependencyType(
                classContext.ClassDeclarationSyntax,
                fieldSymbol.Type,
                BuildDependencyAccessor(fieldSymbol.IsStatic, fieldSymbol.Type, fieldSymbol.Name),
                fieldSymbol.IsStatic,
                fieldSymbol.Name,
                classContext,
                compilation,
                reportDiagnostic);
        }
    }

    private static string BuildDependencyAccessor(bool isStatic, ITypeSymbol dependencyType, string memberName)
        => isStatic
            ? $"global::{dependencyType.ToDisplayString()}"
            : $"this.{memberName}";

    private static void CollectFromStaticDependencies(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        IEnumerable<INamedTypeSymbol> staticDependencyTypes,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var dependencyType in staticDependencyTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!seen.Add(dependencyType))
            {
                continue;
            }

            var accessFieldName = $"global::{dependencyType.ToDisplayString()}";
            var anyMethodCanBeUsed = false;
            foreach (var method in dependencyType.GetMembers().OfType<IMethodSymbol>())
            {
                if (!method.IsStatic)
                {
                    continue;
                }

                anyMethodCanBeUsed |= TryAcceptDependencyMethod(
                    classContext.ClassDeclarationSyntax,
                    method,
                    accessFieldName,
                    canBeInvokedByStaticMethod: true,
                    classContext,
                    compilation);
            }

            if (!anyMethodCanBeUsed)
            {
                reportDiagnostic(MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(
                    classContext.ClassDeclarationSyntax,
                    dependencyType.ToDisplayString()));
            }
        }
    }

    private static void CollectFromInterceptorMethodDependencies(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        foreach (var method in classContext.ClassSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!method.GetAttributes().HasMappaInterceptorMethodDependencyAttribute(compilation))
            {
                continue;
            }

            TryAcceptDependencyMethod(
                classContext.ClassDeclarationSyntax,
                method,
                method.IsStatic ? $"global::{method.ContainingType.ToDisplayString()}" : "this",
                canBeInvokedByStaticMethod: method.IsStatic,
                classContext,
                compilation);
        }
    }

    private static void CollectFromMapperMethods(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        foreach (var methodDeclaration in classContext.ClassDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (methodDeclaration.AttributeLists.GetMappaIgnoreAttributeSyntax(classContext.SemanticModel, cancellationToken) is not null)
            {
                continue;
            }

            if (!methodDeclaration.HasArity(1, 2))
            {
                continue;
            }

            var methodSymbol = classContext.SemanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
            if (methodSymbol is null)
            {
                continue;
            }

            // Partial [Mappa] maps are generated separately; use implemented methods as deps.
            if (methodDeclaration.IsPartial())
            {
                continue;
            }

            TryAcceptDependencyMethod(
                methodDeclaration,
                methodSymbol,
                methodSymbol.IsStatic ? $"global::{methodSymbol.ContainingType.ToDisplayString()}" : "this",
                canBeInvokedByStaticMethod: methodSymbol.IsStatic,
                classContext,
                compilation);
        }
    }

    private static void CollectFromTypeHierarchy(
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        foreach (var method in compilation.GetMethodsInTypeHierarchyFromMetadata(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();
            TryAcceptDependencyMethod(
                classContext.ClassDeclarationSyntax,
                method,
                method.IsStatic ? $"global::{method.ContainingType.ToDisplayString()}" : "this",
                canBeInvokedByStaticMethod: method.IsStatic,
                classContext,
                compilation);
        }
    }

    private static void ProcessDependencyType(
        SyntaxNode referenceSyntaxNode,
        ITypeSymbol dependencyType,
        string accessFieldName,
        bool dependencyMemberIsStatic,
        string dependencyDisplayName,
        MappaClassGeneratorContext classContext,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        var staticAccessor = $"global::{dependencyType.ToDisplayString()}";
        var anyMethodCanBeUsed = false;
        foreach (var method in dependencyType.GetMethodsInTypeHierarchy())
        {
            anyMethodCanBeUsed |= TryAcceptDependencyMethod(
                referenceSyntaxNode,
                method,
                method.IsStatic ? staticAccessor : accessFieldName,
                canBeInvokedByStaticMethod: method.IsStatic || dependencyMemberIsStatic,
                classContext,
                compilation);
        }

        if (!anyMethodCanBeUsed)
        {
            reportDiagnostic(MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(
                referenceSyntaxNode,
                dependencyDisplayName));
        }
    }

    private static bool TryAcceptDependencyMethod(
        SyntaxNode referenceSyntaxNode,
        IMethodSymbol method,
        string accessFieldName,
        bool canBeInvokedByStaticMethod,
        MappaClassGeneratorContext classContext,
        Compilation compilation)
    {
        _ = referenceSyntaxNode;
        if (!IsViableDependencyMapMethod(method, classContext, compilation))
        {
            return false;
        }

        var mappaAttributes = method.GetMethodMappaAttributes(compilation);
        var mapMethod = new MapMethod(
            method,
            accessFieldName,
            classContext.IsNullableEnabled(referenceSyntaxNode),
            canBeInvokedByStaticMethod,
            mappaAttributes);
        return classContext.TryAddMethod(mapMethod);
    }

    private static bool IsViableDependencyMapMethod(
        IMethodSymbol method,
        MappaClassGeneratorContext classContext,
        Compilation compilation)
    {
        if (method.GetAttributes().GetMappaIgnoreAttribute(compilation) is not null)
        {
            return false;
        }

        if (!compilation.IsSymbolAccessibleWithin(method, classContext.ClassSymbol))
        {
            return false;
        }

        if (method.Parameters.Length is not (1 or 2))
        {
            return false;
        }

        if (method.Parameters.Length == 2
            && !method.SecondParameterIsMappaContext(compilation))
        {
            return false;
        }

        if (!method.AreParametersRefModifiersValid())
        {
            return false;
        }

        if (method.IsVoid())
        {
            return false;
        }

        return !method.ReturnsAnyTaskType(compilation);
    }
}