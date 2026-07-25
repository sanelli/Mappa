// <copyright file="EnumMapSwitchCodeHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Emits the body of the <c>switch</c> statement used by the enum mapping strategies.
/// </summary>
internal static class EnumMapSwitchCodeHelper
{
    /// <summary>
    /// Appends the explicit <c>case</c> arms and the <c>default</c> arm described by
    /// <paramref name="enumMapConfiguration"/>.
    /// </summary>
    /// <param name="builder">The code builder positioned inside the switch block.</param>
    /// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
    /// <param name="temporary">The name of the temporary variable receiving the mapped value.</param>
    /// <param name="source">The name of the expression being mapped.</param>
    internal static void AppendSwitchArms(
        PrettyCode.StringBuilder builder,
        EnumMapConfiguration enumMapConfiguration,
        string temporary,
        string source)
    {
        foreach (var enumMapCase in enumMapConfiguration.Cases)
        {
            builder.AppendLine($"case {enumMapCase.CaseExpression}:");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine($"{temporary} = {enumMapCase.AssignmentExpression};");
                builder.AppendLine("break;");
            }
        }

        builder.AppendLine("default:");
        using (builder.CurlyBracesBlock())
        {
            if (enumMapConfiguration.DefaultBehavior is MappaMapEnumDefaultBehavior.UseDefaultValue
                && enumMapConfiguration.DefaultAssignmentExpression is { } defaultAssignmentExpression)
            {
                builder.AppendLine($"{temporary} = {defaultAssignmentExpression};");
                builder.AppendLine("break;");
            }
            else
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }
    }
}