// <copyright file="MappaMethodBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

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
                if (this.mapMethod.Strategy.Strategy is QueryableProjectionMapStrategy)
                {
                    builder.AppendLine(
                        "[global::System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute(\"Queryable projection uses expression trees that require dynamic code generation and are not compatible with Native AOT.\")]");
                }

                builder
                    .AppendLine($"[global::{typeof(DebuggerNonUserCodeAttribute).FullName}]")
                    .AppendLine($"[global::{typeof(GeneratedCodeAttribute).FullName}(\"Mappa\", \"{typeof(MappaGenerator).Assembly.GetName().Version}\")]")
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
        var paramsModifier = this.mapMethod.MethodSymbol.Parameters[0].IsParams ? "params " : string.Empty;
        var refModifiers = this.mapMethod.MethodSymbol.Parameters[0].RefKind == RefKind.In ? "in " : string.Empty;
        var sourceParameter = $"{extensionMethod}{refModifiers}{paramsModifier}{this.mapMethod.SourceType.ToDisplayString()} {this.mapMethod.SourceParameterName}";

        var contextParameter = string.Empty;
        if (this.mapMethod.RequireMappaContextWhenInvoked())
        {
            var contextRefModifier = this.mapMethod.MethodSymbol.Parameters[1].RefKind == RefKind.In ? "in " : string.Empty;
            contextParameter = $", {contextRefModifier}{typeof(MappaContext).FullName} {this.mapMethod.GetMappaContextParameterName()}";
        }

        var signature = $"{modifiersWithReturnType} {this.mapMethod.MethodName}({sourceParameter}{contextParameter})";

        return signature;
    }
}