// <copyright file="InvokeObjectFactoryMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeObjectFactoryMapStrategy"/>.
/// </summary>
internal sealed class InvokeObjectFactoryMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeObjectFactoryMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeObjectFactoryMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeObjectFactoryMapStrategyBuilder(InvokeObjectFactoryMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        string resultTemporary;

        using (context.PushCurrentCompositeTypeSourceName(source))
        {
            var parametersVariableNames = new List<string>();
            foreach (var parameterMapStrategy in this.strategy.ParametersMapStrategies)
            {
                var (parameterTargetTemporary, parameterCode) = parameterMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(parameterCode))
                {
                    builder.AppendLine(parameterCode);
                }

                parametersVariableNames.Add(parameterTargetTemporary);
            }

            var propertyInitializersMappings = new List<(string TargetPropertyName, string TemporaryName)>();
            foreach (var propertyMapStrategy in this.strategy.InitializerStrategies.Where(propertyMapStrategy => !propertyMapStrategy.PostConstructorInitializer))
            {
                var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                propertyInitializersMappings.Add((propertyMapStrategy.TargetProperty.Name, initializerPropertyTargetTemporary));
                if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
                {
                    builder.AppendLine(initializerPropertyCode);
                }
            }

            var hasPropertyInitializers = propertyInitializersMappings.Count > 0;
            var accessor = GetAccessor(this.strategy.ObjectFactory);
            var invocationArguments = this.GetInvocationArguments(source, context, parametersVariableNames);
            resultTemporary = context.NextTemporary();

            // Object initializers are only valid on object-creation expressions. Factory
            // invocations therefore assign properties with post-call statements instead.
            builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = {accessor}{this.strategy.ObjectFactory.Method.Name}({invocationArguments});");
            if (hasPropertyInitializers)
            {
                foreach (var propertyInitializersMapping in propertyInitializersMappings)
                {
                    builder.AppendLine($"{resultTemporary}.{propertyInitializersMapping.TargetPropertyName} = {propertyInitializersMapping.TemporaryName};");
                }
            }

            using (context.PushCurrentCompositeTypeTargetName(resultTemporary))
            {
                foreach (var propertyMapStrategy in this.strategy.InitializerStrategies.Where(propertyMapStrategy => propertyMapStrategy.PostConstructorInitializer))
                {
                    var (_, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                    builder.AppendLine(initializerPropertyCode);
                }
            }

            if (this.strategy.AssignToContextEntries.Length > 0 && this.strategy.ContextParameterName is not null)
            {
                foreach (var assignToContextEntry in this.strategy.AssignToContextEntries)
                {
                    builder.AppendLine($"{this.strategy.ContextParameterName}[{CSharpLiteralHelper.ToStringLiteral(assignToContextEntry.ContextKey)}] = {PropertyPathExpressionBuilder.BuildTargetMemberAccessExpression(resultTemporary, assignToContextEntry.MemberName)};");
                }
            }
        }

        return (resultTemporary, builder.ToString());
    }

    private static string GetAccessor(ObjectFactory objectFactory)
    {
        if (objectFactory.ExplicitType is not null)
        {
            return $"{objectFactory.ExplicitType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
        }

        if (objectFactory.FieldOrProperty is not null)
        {
            if (objectFactory.Method.IsStatic)
            {
                var fieldOrPropertyType = GetFieldOrPropertyType(objectFactory.FieldOrProperty);
                return $"{fieldOrPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
            }

            var fieldOrPropertyAccessor = objectFactory.FieldOrProperty.IsStatic ? string.Empty : "this.";
            return $"{fieldOrPropertyAccessor}{objectFactory.FieldOrProperty.Name}.";
        }

        return objectFactory.Method.IsStatic ? string.Empty : "this.";
    }

    private static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
        };

    private string GetInvocationArguments(
        string source,
        MappaBuilderContext context,
        List<string> parametersVariableNames)
    {
        return this.strategy.ObjectFactory.InvocationKind switch
        {
            ObjectFactoryInvocationKind.FullyProduced => this.GetFullyProducedArguments(source, context),
            ObjectFactoryInvocationKind.EmptyCtorLike => this.GetEmptyCtorLikeArguments(context),
            ObjectFactoryInvocationKind.ParameterizedLike => string.Join(", ", parametersVariableNames),
            _ => throw new MappaGeneratorException($"Unexpected object factory invocation kind '{this.strategy.ObjectFactory.InvocationKind}'."),
        };
    }

    private string GetFullyProducedArguments(string source, MappaBuilderContext context)
    {
        var method = this.strategy.ObjectFactory.Method;
        return method.Parameters.Length switch
        {
            1 => source,
            2 => $"{source}, {context.GetMapMethod().GetMappaContextParameterName()}",
            _ => throw new MappaGeneratorException($"Unexpected fully-produced factory signature '{method.ToDisplayString()}'."),
        };
    }

    private string GetEmptyCtorLikeArguments(MappaBuilderContext context)
    {
        var method = this.strategy.ObjectFactory.Method;
        if (method.Parameters.Length == 0)
        {
            return string.Empty;
        }

        if (method.Parameters.Length == 1 && method.ParameterIsMappaContext(context.Compilation, 0))
        {
            return context.GetMapMethod().GetMappaContextParameterName();
        }

        throw new MappaGeneratorException($"Unexpected empty-ctor-like factory signature '{method.ToDisplayString()}'.");
    }
}