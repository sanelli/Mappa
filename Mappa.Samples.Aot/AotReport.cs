// <copyright file="AotReport.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text;

namespace Mappa.Samples.Aot;

/// <summary>
/// In-memory AOT smoke-test report serialized as a single JSON document.
/// </summary>
internal sealed class AotReport
{
    private readonly List<MapperSection> mappers = [];
    private MapperSection? currentSection;

    /// <summary>
    /// Starts a new mapper section in the report.
    /// </summary>
    /// <param name="mapperType">The mapper type name.</param>
    public void BeginMapper(string mapperType)
    {
        this.currentSection = new MapperSection(mapperType);
        this.mappers.Add(this.currentSection);
    }

    /// <summary>
    /// Records one map invocation in the current mapper section.
    /// </summary>
    /// <param name="methodName">The map method name.</param>
    /// <param name="parameterType">The parameter type name.</param>
    /// <param name="returnType">The return type name.</param>
    /// <param name="input">The input value.</param>
    /// <param name="output">The output value.</param>
    public void RecordInvocation(string methodName, string parameterType, string returnType, object? input, object? output)
    {
        ArgumentNullException.ThrowIfNull(this.currentSection);
        this.currentSection.AddInvocation(
            AotJson.FormatMethod(methodName, parameterType, returnType),
            AotJson.ToDisplayString(input),
            AotJson.ToDisplayString(output));
    }

    /// <summary>
    /// Serializes the report to JSON.
    /// </summary>
    /// <returns>The JSON document.</returns>
    public string ToJson()
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        builder.AppendLine("  \"mappers\": [");
        for (var mapperIndex = 0; mapperIndex < this.mappers.Count; mapperIndex++)
        {
            this.mappers[mapperIndex].AppendJson(builder, mapperIndex == this.mappers.Count - 1);
        }

        builder.AppendLine("  ]");
        builder.Append('}');
        return builder.ToString();
    }

    /// <summary>
    /// Writes the JSON report to the given text writer.
    /// </summary>
    /// <param name="writer">The destination writer.</param>
    public void WriteTo(TextWriter writer)
        => writer.WriteLine(this.ToJson());

    private sealed class MapperSection(string mapperType)
    {
        private readonly List<Invocation> invocations = [];

        public void AddInvocation(string method, string input, string output)
            => this.invocations.Add(new Invocation(method, input, output));

        public void AppendJson(StringBuilder builder, bool isLastMapper)
        {
            builder.AppendLine("    {");
            builder.Append("      \"mapperType\": ");
            AotJson.AppendJsonString(builder, mapperType);
            builder.Append(',');
            builder.AppendLine();
            builder.AppendLine("      \"invocations\": [");
            for (var index = 0; index < this.invocations.Count; index++)
            {
                this.invocations[index].AppendJson(builder, index == this.invocations.Count - 1);
            }

            builder.AppendLine("      ]");
            builder.Append("    }");
            if (!isLastMapper)
            {
                builder.Append(',');
            }

            builder.AppendLine();
        }
    }

    private sealed class Invocation(string methodSignature, string inputDisplay, string outputDisplay)
    {
        public void AppendJson(StringBuilder builder, bool isLastInvocation)
        {
            builder.AppendLine("        {");
            builder.Append("          \"method\": ");
            AotJson.AppendJsonString(builder, methodSignature);
            builder.Append(',');
            builder.AppendLine();
            builder.Append("          \"input\": ");
            AotJson.AppendJsonString(builder, inputDisplay);
            builder.Append(',');
            builder.AppendLine();
            builder.Append("          \"output\": ");
            AotJson.AppendJsonString(builder, outputDisplay);
            builder.AppendLine();
            builder.Append("        }");
            if (!isLastInvocation)
            {
                builder.Append(',');
            }

            builder.AppendLine();
        }
    }
}