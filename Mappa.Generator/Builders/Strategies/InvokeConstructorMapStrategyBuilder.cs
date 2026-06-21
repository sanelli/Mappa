// <copyright file="InvokeConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

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

            // Handle initializer properties
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

            resultTemporary = context.NextTemporary();
            var initializerCodeLine = $"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = new {this.strategy.TargetType.ToDisplayNameWithoutNullableAnnotation()}({string.Join(", ", parametersVariableNames)}){(hasPropertyInitializers ? string.Empty : ";")}";
            builder.AppendLine(initializerCodeLine);
            if (hasPropertyInitializers)
            {
                using (builder.CurlyBracesBlock(trailingSemicolon: true))
                {
                    foreach (var propertyInitializersMapping in propertyInitializersMappings)
                    {
                        builder.AppendLine($"{propertyInitializersMapping.TargetPropertyName} = {propertyInitializersMapping.TemporaryName},");
                    }
                }
            }

            // Initialise properties after the constructor has been created.
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
                    builder.AppendLine($"{this.strategy.ContextParameterName}[{CSharpLiteralHelper.ToStringLiteral(assignToContextEntry.ContextKey)}] = {resultTemporary}.{assignToContextEntry.MemberName};");
                }
            }
        }

        return (resultTemporary, builder.ToString());
    }
}