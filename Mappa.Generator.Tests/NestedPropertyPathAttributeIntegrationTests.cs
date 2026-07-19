// <copyright file="NestedPropertyPathAttributeIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for dot-separated nested property paths on mapping attributes.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string SourceTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
}