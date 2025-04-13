// <copyright file="MappaMethodBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

namespace Mappa.Generator.Builders;

/// <summary>
/// Build a method.
/// </summary>
internal sealed class MappaMethodBuilder
    : IMappaBuilder
{
    private readonly MapMethod mapMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodBuilder"/> class.
    /// </summary>
    /// <param name="mapMethod">THe method to be generated.</param>
    public MappaMethodBuilder(MapMethod mapMethod)
    {
        this.mapMethod = mapMethod;
    }

    /// <inheritdoc/>
    public string BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var isNullableEnabled = this.mapMethod.NullableEnabled;

        var pragmaWarningDisable = this.mapMethod.PragmaWarning is PragmaWarningSetting.Disable
            ? builder.PragmaWarningDirective()
            : null;

        try
        {
            using (builder.NullableDirective(isNullableEnabled))
            {
                builder
                    .AppendLine(new MappaGeneratedCodeAttributeBuilder().BuildSource(context, mappaGlobalOptions))
                    .AppendLine(this.GetSignature());
                using (builder.CurlyBracesBlock())
                {
                    var (strategySource, header) = this.mapMethod.Strategy.GetBuilder().BuildSource(this.mapMethod.SourceParameterName, context, mappaGlobalOptions);
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        builder.AppendLine(header);
                        builder.AppendEmptyLine();
                    }

                    builder.AppendLine(strategySource);
                }
            }
        }
        finally
        {
            pragmaWarningDisable?.Dispose();
        }

        return builder.ToString();
    }

    private string GetSignature()
    {
        var modifiersWithReturnType = string.Join(
            " ",
            this.mapMethod.MethodSymbol.GetSymbolModifiers(),
            "partial",
            this.mapMethod.TargetType.ToDisplayString())
            .Trim();

        var extensionMethod = this.mapMethod.MethodSymbol.IsExtensionMethod ? "this " : string.Empty;
        var sourceParameter = $"{extensionMethod}{this.mapMethod.SourceType.ToDisplayString()} {this.mapMethod.SourceParameterName}";

        var contextParameter = string.Empty;
        if (this.mapMethod.RequireMappaContextWhenInvoked())
        {
            contextParameter = $", {typeof(MappaContext).FullName} {this.mapMethod.GetMappaContextParameterName()}";
        }

        var signature = $"{modifiersWithReturnType} {this.mapMethod.MethodName}({sourceParameter}{contextParameter})";

        return signature;
    }
}