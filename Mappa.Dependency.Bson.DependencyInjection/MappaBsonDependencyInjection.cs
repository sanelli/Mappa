// <copyright file="MappaBsonDependencyInjection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Mappa.Dependency.Bson.DependencyInjection;

/// <summary>
/// Dependency injection helpers for <see cref="MappaBsonMapper"/>.
/// </summary>
public static class MappaBsonDependencyInjection
{
    /// <summary>
    /// Register the service mappa bson mapper as singleton.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The input <paramref name="serviceCollection"/>.</returns>
    public static IServiceCollection RegisterMappaBson(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MappaBsonMapper>();
        serviceCollection.AddSingleton<IMappaBsonMapper, MappaBsonMapper>(serviceProvider => serviceProvider.GetRequiredService<MappaBsonMapper>());
        return serviceCollection;
    }
}