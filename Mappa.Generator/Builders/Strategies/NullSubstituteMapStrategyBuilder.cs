// <copyright file="NullSubstituteMapStrategyBuilder.cs" company="Stefano Anelli">
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
/// Builder for <see cref="NullSubstituteMapStrategy"/>.
/// </summary>
internal sealed class NullSubstituteMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullSubstituteMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullSubstituteMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullSubstituteMapStrategyBuilder(NullSubstituteMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var substitutedTemporary = context.NextTemporary();
        var temporaryTypeName = GetNonNullSourceType(this.strategy.SourceType).ToDisplayString();
        var substituteExpression = this.strategy.IsAssignValueMode
            ? this.BuildAssignValueExpression(source)
            : this.BuildInvokeMethodExpression(source);

        stringBuilder.AppendLine($"{temporaryTypeName} {substitutedTemporary} = {substituteExpression};");

        var (elementTemporary, elementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            this.strategy.ElementStrategy,
            substitutedTemporary,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrEmpty(elementCode))
        {
            stringBuilder.AppendEmptyLine();
            stringBuilder.AppendLine(elementCode);
        }

        return (elementTemporary, stringBuilder.ToString());
    }

    private static ITypeSymbol GetNonNullSourceType(ITypeSymbol sourceType)
    {
        if (sourceType.IsValueTypeNullable())
        {
            return sourceType.GetTypeInsideNullable();
        }

        if (sourceType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
        {
            return sourceType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        return sourceType;
    }

    private string BuildAssignValueExpression(string source)
    {
        var literal = AttributeConstantValueHelper.ToCode(this.strategy.AssignValue);
        return $"{source} ?? {literal}";
    }

    private string BuildInvokeMethodExpression(string source)
    {
        var method = this.strategy.Method
                     ?? throw new MappaGeneratorException("NullSubstitute InvokeMethod mode requires a resolved method.");
        var accessor = this.GetAccessor();
        var arguments = this.GetInvokeArguments(source);
        var invokeExpression = $"{accessor}{method.Name}({arguments})";
        var nullCheck = this.BuildNullCheckExpression(source);

        // Cast the non-null branch so the conditional type matches the non-null temporary.
        var nonNullTypeName = GetNonNullSourceType(this.strategy.SourceType).ToDisplayString();
        return $"{nullCheck} ? {invokeExpression} : ({nonNullTypeName}){source}";
    }

    private string BuildNullCheckExpression(string source)
    {
        if (this.strategy.SourceType.IsValueType)
        {
            return $"{source} == null";
        }

        return this.strategy.IsNullableEnabled
            ? $"{source} is null"
            : $"{source} == null";
    }

    private string GetAccessor()
    {
        if (this.strategy.InvokeMethodType is not null)
        {
            var classTypeFullName = this.strategy.InvokeMethodType.FullName
                ?? throw new MappaGeneratorException($"Cannot detect name for type {this.strategy.InvokeMethodType}");
            return $"{classTypeFullName}.";
        }

        if (this.strategy.FieldOrProperty is not null)
        {
            var fieldAccessor = this.strategy.FieldOrProperty.IsStatic ? string.Empty : "this.";
            return $"{fieldAccessor}{this.strategy.FieldOrProperty.Name}.";
        }

        var method = this.strategy.Method
                     ?? throw new MappaGeneratorException("NullSubstitute InvokeMethod mode requires a resolved method.");
        return method.IsStatic ? string.Empty : "this.";
    }

    private string GetInvokeArguments(string source)
    {
        var method = this.strategy.Method
                     ?? throw new MappaGeneratorException("NullSubstitute InvokeMethod mode requires a resolved method.");
        if (method.Parameters.Length == 1)
        {
            return source;
        }

        if (method.Parameters.Length == 2)
        {
            var contextParameterName = this.strategy.ContextParameterName
                ?? throw new MappaGeneratorException("Invoked null-substitute method requires MappaContext but the root map method does not provide one.");
            return $"{source}, {contextParameterName}";
        }

        throw new MappaGeneratorException("Unexpected null-substitute invoke method parameter count.");
    }
}