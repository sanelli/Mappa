// <copyright file="MappaDependencyInjectionGeneratorAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Builders;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm that generates dependency injection registration methods for
/// classes marked with <see cref="Mappa.Attributes.MappaDependencyInjectionAttribute"/>.
/// </summary>
internal sealed class MappaDependencyInjectionGeneratorAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaDependencyInjectionGeneratorAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntaxes">The candidate registrar class declarations.</param>
    /// <param name="classSymbolResolver">
    /// Optional resolver used by unit tests to simulate <c>GetDeclaredSymbol</c> failures.
    /// </param>
    public MappaDependencyInjectionGeneratorAlgorithm(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes,
        Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
    {
        this.Context = context;
        this.Compilation = compilation;
        this.ClassDeclarationSyntaxes = classDeclarationSyntaxes;
        this.ClassSymbolResolver = classSymbolResolver;
    }

    private SourceProductionContext Context { get; }

    private Compilation Compilation { get; }

    private ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes { get; }

    private Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? ClassSymbolResolver { get; }

    /// <summary>
    /// Execute the algorithm and produce the sources.
    /// </summary>
    internal void Execute()
    {
        var cancellationToken = this.Context.CancellationToken;
        foreach (var classDeclarationSyntax in this.ClassDeclarationSyntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (classDeclarationSyntax is null)
            {
                continue;
            }

            this.ExecuteForSingleClass(classDeclarationSyntax, cancellationToken);
        }
    }

    private static ImmutableArray<INamedTypeSymbol> GetEligibleInterfaces(
        INamedTypeSymbol mapper,
        MappaDependencyInjectionAttributeData attributeData)
    {
        return
        [
            .. mapper.AllInterfaces
                .Where(interfaceType => !attributeData.IsIgnored(interfaceType))
                .OrderBy(interfaceType => interfaceType.ToDisplayString(), StringComparer.Ordinal),
        ];
    }

    private void ExecuteForSingleClass(ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
    {
        var classSymbol = this.ResolveClassSymbol(classDeclarationSyntax, cancellationToken);
        if (classSymbol is null)
        {
            return;
        }

        var className = classDeclarationSyntax.Identifier.Text;
        var attributes = classSymbol.GetAttributes();

        if (attributes.HasMappaAttribute(this.Compilation))
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaAndMappaDependencyInjectionBothApplied(classDeclarationSyntax, className));
            return;
        }

        if (!classDeclarationSyntax.IsPartial())
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaDependencyInjectionClassIsNotPartial(classDeclarationSyntax, className));
            return;
        }

        var attributeData = attributes.GetMappaDependencyInjectionAttributeData(this.Compilation);
        if (attributeData is null)
        {
            return;
        }

        var mappers = this.DiscoverMappers(classSymbol, attributeData, classDeclarationSyntax, cancellationToken);
        var builder = new MappaDependencyInjectionFileBuilder(classDeclarationSyntax, classSymbol, attributeData, mappers);
        this.Context.AddSource(builder.HintName, builder.BuildSource());
    }

    private INamedTypeSymbol? ResolveClassSymbol(ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
    {
        if (this.ClassSymbolResolver is not null)
        {
            return this.ClassSymbolResolver(this.Compilation, classDeclarationSyntax, cancellationToken);
        }

        var semanticModel = this.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        return semanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) as INamedTypeSymbol;
    }

    private ImmutableArray<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)> DiscoverMappers(
        INamedTypeSymbol registrarClass,
        MappaDependencyInjectionAttributeData attributeData,
        ClassDeclarationSyntax classDeclarationSyntax,
        CancellationToken cancellationToken)
    {
        var assemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default)
        {
            this.Compilation.Assembly,
        };

        foreach (var markerType in attributeData.InjectFromAssemblies)
        {
            assemblies.Add(markerType.ContainingAssembly);
        }

        var results = new List<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)>();
        foreach (var assembly in assemblies
                     .OrderBy(candidate => candidate.Identity.GetDisplayName(), StringComparer.Ordinal))
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var type in assembly.GetAllNamedTypes())
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.TryAddDiscoveredMapper(type, registrarClass, attributeData, classDeclarationSyntax, results);
            }
        }

        return [.. results];
    }

    private void TryAddDiscoveredMapper(
        INamedTypeSymbol type,
        INamedTypeSymbol registrarClass,
        MappaDependencyInjectionAttributeData attributeData,
        ClassDeclarationSyntax classDeclarationSyntax,
        List<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)> results)
    {
        if (SymbolEqualityComparer.Default.Equals(type, registrarClass))
        {
            return;
        }

        if (attributeData.IsIgnored(type))
        {
            return;
        }

        if (!type.GetAttributes().HasMappaAttribute(this.Compilation))
        {
            return;
        }

        // Static mapper classes cannot be registered with Microsoft.Extensions.DependencyInjection.
        if (type.IsStatic)
        {
            this.Context.ReportDiagnostic(
                MappaDiagnostics.MappaDependencyInjectionStaticMapperSkipped(
                    classDeclarationSyntax,
                    type.ToDisplayString()));
            return;
        }

        var interfaces = GetEligibleInterfaces(type, attributeData);
        if (!this.HasRequiredEligibleInterfaces(attributeData, type, interfaces, classDeclarationSyntax))
        {
            return;
        }

        results.Add((type, interfaces));
    }

    private bool HasRequiredEligibleInterfaces(
        MappaDependencyInjectionAttributeData attributeData,
        INamedTypeSymbol type,
        ImmutableArray<INamedTypeSymbol> interfaces,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        switch (attributeData.InjectInterfaces)
        {
            case MappaDependencyInjectionInjectInterfaces.InterfaceOnly:
            case MappaDependencyInjectionInjectInterfaces.InterfaceAndClass:
                if (interfaces.IsDefaultOrEmpty)
                {
                    this.Context.ReportDiagnostic(
                        MappaDiagnostics.MappaDependencyInjectionMapperHasNoEligibleInterfaces(
                            classDeclarationSyntax,
                            type.ToDisplayString()));
                    return false;
                }

                break;
        }

        return true;
    }
}