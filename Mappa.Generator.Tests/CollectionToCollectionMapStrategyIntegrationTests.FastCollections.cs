// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.FastCollections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
public sealed partial class CollectionToCollectionMapStrategyIntegrationTests
{
    // TODO [#24] Test T[] -> S[] : Use span on both source and target (feature enabled on method).
    // TODO [#24] Test T[] -> List<S> : Use span on both source and target (feature enabled on method).
    // TODO [#24] Test List<T> -> S[] : Use span on both source and target (feature enabled on method).
    // TODO [#24] Test List<S> -> S[] : Use span on both source and target (feature enabled on method).
    // TODO [#24] Test IEnumerable<S> -> S[] : No span (feature enabled on method).
    // TODO [#24] Test IEnumerable<S> -> List<S> : No span (feature enabled on method).
    // TODO [#24] Test T[] -> IEnumerable<S> : Use span on source only (feature enabled on method).
    // TODO [#24] Test List<T> -> IEnumerable<S> : Use span on source only (feature enabled on method).
    // TODO [#24] Test T[] -> IEnumerable<S> : Use span on source (feature enabled on class).
    // TODO [#24] Test T[] -> IEnumerable<S> : Use span on source (feature disabled on class but enabled on method).
    // TODO [#24] Test T[] -> IEnumerable<S> : No span (feature enabled on class but disabled on method).
}