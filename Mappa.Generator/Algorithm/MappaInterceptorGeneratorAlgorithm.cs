// <copyright file="MappaInterceptorGeneratorAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Builders;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm that generates interceptable mapping stubs for classes marked with
/// <see cref="Mappa.Attributes.MappaInterceptorAttribute"/>.
/// </summary>
internal sealed class MappaInterceptorGeneratorAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInterceptorGeneratorAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntaxes">The candidate interceptor class declarations.</param>
    /// <param name="classSymbolResolver">
    /// Optional resolver used by unit tests to simulate <c>GetDeclaredSymbol</c> failures.
    /// </param>
    public MappaInterceptorGeneratorAlgorithm(
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

    private void ExecuteForSingleClass(ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
    {
        var classSymbol = this.ResolveClassSymbol(classDeclarationSyntax, cancellationToken);
        if (classSymbol is null)
        {
            return;
        }

        var className = classDeclarationSyntax.Identifier.Text;
        var attributes = classSymbol.GetAttributes();
        if (!attributes.HasMappaInterceptorAttribute(this.Compilation))
        {
            return;
        }

        if (!classDeclarationSyntax.IsPartial())
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaInterceptorClassIsNotPartial(classDeclarationSyntax, className));
            return;
        }

        if (classSymbol.IsStatic)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaInterceptorClassIsStatic(classDeclarationSyntax, className));
            return;
        }

        var methodName = attributes.GetMappaInterceptorMethodName(this.Compilation);
        var builder = new MappaInterceptorFileBuilder(classDeclarationSyntax, classSymbol, methodName);
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
}