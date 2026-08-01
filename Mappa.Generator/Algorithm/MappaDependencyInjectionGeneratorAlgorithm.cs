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
    public MappaDependencyInjectionGeneratorAlgorithm(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes)
    {
        this.Context = context;
        this.Compilation = compilation;
        this.ClassDeclarationSyntaxes = classDeclarationSyntaxes;
    }

    private SourceProductionContext Context { get; }

    private Compilation Compilation { get; }

    private ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes { get; }

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
        var semanticModel = this.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol classSymbol)
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

        var mappers = this.DiscoverMappers(classSymbol, attributeData, classDeclarationSyntax);
        var builder = new MappaDependencyInjectionFileBuilder(classDeclarationSyntax, classSymbol, attributeData, mappers);
        this.Context.AddSource(builder.HintName, builder.BuildSource());
    }

    private ImmutableArray<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)> DiscoverMappers(
        INamedTypeSymbol registrarClass,
        MappaDependencyInjectionAttributeData attributeData,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var results = new List<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)>();
        foreach (var type in this.Compilation.Assembly.GetAllNamedTypes())
        {
            if (SymbolEqualityComparer.Default.Equals(type, registrarClass))
            {
                continue;
            }

            if (attributeData.IsIgnored(type))
            {
                continue;
            }

            if (!type.GetAttributes().HasMappaAttribute(this.Compilation))
            {
                continue;
            }

            var interfaces = GetEligibleInterfaces(type, attributeData);
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
                        continue;
                    }

                    break;
            }

            results.Add((type, interfaces));
        }

        return [.. results];
    }
}