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

        var sourcePropertyTemporary = string.Empty;
        if (this.strategy.ChainedSourcePropertyPath is not null)
        {
            var chainedSourcePropertyPath = this.strategy.ChainedSourcePropertyPath;
            var chainSource = source;
            var rootParameterName = context.GetMapMethod().MethodSymbol.Parameters[0].Name;
            var receiverPathPrefix = chainedSourcePropertyPath.ReceiverPathPrefix;

            // Only rewrite to the root parameter when the chain is intentionally rooted there.
            // Empty prefix means read remaining segments from the current nested source receiver.
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

            ITypeSymbol innermostSourceType;
            if (resolvedProperties.Length > 0)
            {
                innermostSourceType = resolvedProperties[resolvedProperties.Length - 1].Type;
            }
            else if (PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                         chainedSourcePropertyPath.StartingSourceType,
                         chainSource,
                         string.IsNullOrWhiteSpace(receiverPathPrefix) ? chainSource : receiverPathPrefix,
                         out var receiverType))
            {
                innermostSourceType = receiverType;
            }
            else
            {
                innermostSourceType = chainedSourcePropertyPath.StartingSourceType;
            }

            sourcePropertyTemporary = context.NextTemporary();
            builder.AppendLine($"{innermostSourceType.ToDisplayString()} {sourcePropertyTemporary} = {accessExpression};");
        }
        else if (this.strategy.SourceProperty is not null)
        {
            sourcePropertyTemporary = context.NextTemporary();
            var sourceReadExpression = InaccessibleMemberAccessHelper.BuildPropertyReadExpression(
                source,
                this.strategy.SourceProperty,
                this.strategy.RequiresUnsafeAccessorOnSource,
                context);
            builder.AppendLine($"{this.strategy.SourceProperty.Type.ToDisplayString()} {sourcePropertyTemporary} = {sourceReadExpression};");
        }

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
}