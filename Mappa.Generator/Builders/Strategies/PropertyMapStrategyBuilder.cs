// <copyright file="PropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="PropertyMapStrategy"/>.
/// </summary>
internal sealed class PropertyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly PropertyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy builder.</param>
    public PropertyMapStrategyBuilder(PropertyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var sourcePropertyTemporary = this.BuildSourcePropertyTemporary(builder, source, context);

        string targetTemporary;
        string code;
        using (context.PushCurrentTargetPropertyUnsafeAccess(
                   this.strategy.TargetProperty,
                   this.strategy.RequiresUnsafeAccessorOnTarget))
        {
            (targetTemporary, code) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                this.strategy.PropertyStrategy,
                sourcePropertyTemporary,
                context,
                mappaGlobalOptions);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            builder.AppendLine(code);
            builder.AppendEmptyLine();
        }

        return (targetTemporary, builder.ToString());
    }

    private static ITypeSymbol ResolveInnermostChainedSourceType(
        ChainedSourcePropertyPathInfo chainedSourcePropertyPath,
        string chainSource,
        string receiverPathPrefix,
        IPropertySymbol[] resolvedProperties)
    {
        if (resolvedProperties.Length > 0)
        {
            return resolvedProperties[resolvedProperties.Length - 1].Type;
        }

        if (PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                chainedSourcePropertyPath.StartingSourceType,
                chainSource,
                string.IsNullOrWhiteSpace(receiverPathPrefix) ? chainSource : receiverPathPrefix,
                out var receiverType))
        {
            return receiverType;
        }

        return chainedSourcePropertyPath.StartingSourceType;
    }

    private string BuildSourcePropertyTemporary(PrettyCode.StringBuilder builder, string source, MappaBuilderContext context)
    {
        if (this.strategy.ChainedSourcePropertyPath is not null)
        {
            return this.AppendChainedSourcePropertyRead(builder, source, context);
        }

        if (this.strategy.SourceProperty is not null)
        {
            return this.AppendDirectSourcePropertyRead(builder, source, context);
        }

        return string.Empty;
    }

    private string AppendChainedSourcePropertyRead(PrettyCode.StringBuilder builder, string source, MappaBuilderContext context)
    {
        var chainedSourcePropertyPath = this.strategy.ChainedSourcePropertyPath;
        if (chainedSourcePropertyPath is null)
        {
            return string.Empty;
        }

        var chainSource = source;
        var rootParameterName = context.GetMapMethod().SourceParameterName;
        var receiverPathPrefix = chainedSourcePropertyPath.ReceiverPathPrefix;

        if (!string.IsNullOrWhiteSpace(receiverPathPrefix)
            && (receiverPathPrefix.Equals(rootParameterName, StringComparison.Ordinal)
                || receiverPathPrefix.StartsWith($"{rootParameterName}.", StringComparison.Ordinal)))
        {
            chainSource = rootParameterName;
        }

        var accessExpression = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            chainSource,
            receiverPathPrefix,
            chainedSourcePropertyPath.RemainingSourceSegments,
            chainedSourcePropertyPath.StartingSourceType,
            context.GetMapMethod().NullableEnabled,
            this.strategy.TargetProperty.Type,
            out var resolvedProperties,
            chainedSourcePropertyPath.OriginalSourcePath);

        var innermostSourceType = ResolveInnermostChainedSourceType(
            chainedSourcePropertyPath,
            chainSource,
            receiverPathPrefix,
            resolvedProperties);

        var sourcePropertyTemporary = context.NextTemporary();
        builder.AppendLine($"{innermostSourceType.ToDisplayString()} {sourcePropertyTemporary} = {accessExpression};");
        return sourcePropertyTemporary;
    }

    private string AppendDirectSourcePropertyRead(PrettyCode.StringBuilder builder, string source, MappaBuilderContext context)
    {
        if (this.strategy.SourceProperty is not { } sourceProperty)
        {
            return string.Empty;
        }

        var sourcePropertyTemporary = context.NextTemporary();
        var sourceReadExpression = InaccessibleMemberAccessHelper.BuildPropertyReadExpression(
            source,
            sourceProperty,
            this.strategy.RequiresUnsafeAccessorOnSource,
            context);
        builder.AppendLine($"{sourceProperty.Type.ToDisplayString()} {sourcePropertyTemporary} = {sourceReadExpression};");
        return sourcePropertyTemporary;
    }
}