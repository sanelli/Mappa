// <copyright file="InvokeConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeConstructorMapStrategy"/>.
/// </summary>
internal sealed class InvokeConstructorMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeConstructorMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeConstructorMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeConstructorMapStrategyBuilder(InvokeConstructorMapStrategy strategy)
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
            // Handle arguments mappings
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

            // Object initializers cannot target inaccessible setters; those are assigned after construction.
            var objectInitializerStrategies = this.strategy.InitializerStrategies
                .Where(propertyMapStrategy => !propertyMapStrategy.PostConstructorInitializer
                                              && !propertyMapStrategy.RequiresUnsafeAccessorOnTarget)
                .ToArray();
            var postConstructorStrategies = this.strategy.InitializerStrategies
                .Where(propertyMapStrategy => propertyMapStrategy.PostConstructorInitializer
                                              || propertyMapStrategy.RequiresUnsafeAccessorOnTarget)
                .ToArray();

            var propertyInitializersMappings = new List<(IPropertySymbol TargetProperty, string TemporaryName, bool RequiresUnsafeAccessorOnTarget)>();
            foreach (var propertyMapStrategy in objectInitializerStrategies)
            {
                var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                propertyInitializersMappings.Add((
                    propertyMapStrategy.TargetProperty,
                    initializerPropertyTargetTemporary,
                    propertyMapStrategy.RequiresUnsafeAccessorOnTarget));
                if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
                {
                    builder.AppendLine(initializerPropertyCode);
                }
            }

            var hasPropertyInitializers = propertyInitializersMappings.Count > 0;
            var constructionExpression = InaccessibleMemberAccessHelper.BuildConstructorInvocationExpression(
                this.strategy.Constructor,
                parametersVariableNames,
                this.strategy.RequiresUnsafeAccessorOnConstructor,
                context);

            resultTemporary = context.NextTemporary();
            if (hasPropertyInitializers)
            {
                builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = {constructionExpression}");
                using (builder.CurlyBracesBlock(trailingSemicolon: true))
                {
                    foreach (var propertyInitializersMapping in propertyInitializersMappings)
                    {
                        builder.AppendLine($"{propertyInitializersMapping.TargetProperty.Name} = {propertyInitializersMapping.TemporaryName},");
                    }
                }
            }
            else
            {
                builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = {constructionExpression};");
            }

            // Initialise properties after the constructor has been created.
            using (context.PushCurrentCompositeTypeTargetName(resultTemporary))
            {
                foreach (var propertyMapStrategy in postConstructorStrategies)
                {
                    var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                    if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
                    {
                        builder.AppendLine(initializerPropertyCode);
                    }

                    // Normal setter mappings moved out of the object initializer need an explicit assignment.
                    if (!propertyMapStrategy.PostConstructorInitializer
                        && propertyMapStrategy.RequiresUnsafeAccessorOnTarget)
                    {
                        builder.AppendLine(InaccessibleMemberAccessHelper.BuildPropertyAssignmentStatement(
                            resultTemporary,
                            propertyMapStrategy.TargetProperty,
                            initializerPropertyTargetTemporary,
                            requiresUnsafeAccessor: true,
                            context));
                    }
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
}