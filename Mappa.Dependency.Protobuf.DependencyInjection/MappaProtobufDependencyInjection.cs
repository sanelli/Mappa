// <copyright file="MappaProtobufDependencyInjection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Mappa.Dependency.Protobuf.DependencyInjection;

/// <summary>
/// Register the protobuf service collection.
/// </summary>
public static class MappaProtobufDependencyInjection
{
    /// <summary>
    /// Register the service mappa protobuf mapper as singleton.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The input <paramref name="serviceCollection"/>.</returns>
    public static IServiceCollection RegisterMappaProtobuf(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MappaProtobufMapper>();
        serviceCollection.AddSingleton<IMappaProtobufMapper, MappaProtobufMapper>(serviceProvider => serviceProvider.GetRequiredService<MappaProtobufMapper>());
        return serviceCollection;
    }
}