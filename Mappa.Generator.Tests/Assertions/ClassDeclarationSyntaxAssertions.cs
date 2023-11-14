// <copyright file="ClassDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="ClassDeclarationSyntax"/>.
/// </summary>
public sealed class ClassDeclarationSyntaxAssertions
    : ObjectAssertions<ClassDeclarationSyntax, ClassDeclarationSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    public ClassDeclarationSyntaxAssertions(
        ClassDeclarationSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value)
    {
        this.SemanticModel = semanticModel;
        this.Compilation = compilation;
    }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    private SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Assert that the class have all the expected modifiers.
    /// </summary>
    /// <param name="modifiers">The expected modifier.</param>
    /// <returns>The assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveModifiers(params SyntaxKind[] modifiers)
    {
        var expectedModifiers = new HashSet<SyntaxKind>(modifiers);
        this.Subject.Modifiers.Should().HaveCount(expectedModifiers.Count);
        this.Subject.Modifiers.Should()
            .Contain(syntaxToken => expectedModifiers.Contains(syntaxToken.Kind()));

        return this;
    }

    /// <summary>
    /// Assert that the class have all the expected modifiers.
    /// </summary>
    /// <returns>The assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveGeneratedCodeAttribute()
    {
        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var generatedCodeAttributes = attributes.Where(x =>
                x.Name.ToString().Equals(typeof(GeneratedCodeAttribute).FullName, StringComparison.Ordinal))
            .ToArray();
        generatedCodeAttributes.Should().HaveCount(1);
        var generatedCodeAttribute = generatedCodeAttributes.Single();
        generatedCodeAttribute.Should().BeGeneratedCodeAttribute();

        return this;
    }

    /// <summary>
    /// Assert that the class has <paramref name="count"/> methods.
    /// </summary>
    /// <param name="count">Expected number of methods.</param>
    /// <returns>The assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveMethods(int count)
    {
        var methodDeclarationSyntaxes = this.Subject.ChildNodes().OfType<MethodDeclarationSyntax>().ToArray();
        methodDeclarationSyntaxes.Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert that the class has a method with a specific signature.
    /// </summary>
    /// <param name="returnType">The return type of the method.</param>
    /// <param name="returnTypeNullableAnnotation"><c>true</c> if the method has been annotated with nullability.</param>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The expected parameters of the method.</param>
    /// <param name="assert">The assertion on the method.</param>
    /// <returns>The method declaration syntax assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveMethod(
        Type returnType,
        NullableAnnotation returnTypeNullableAnnotation,
        string name,
        (Type Type, NullableAnnotation NullableAnnotation, string Name)[] parameters,
        Action<MethodDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var methods = this.Subject.ChildNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(methodDeclarationSyntax =>
            {
                if (!methodDeclarationSyntax.Identifier.ToString().Equals(name, StringComparison.Ordinal))
                {
                    return false;
                }

                var methodSymbol = this.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax)
                                   ?? throw new MappaGeneratorException(
                                       $"Cannot obtain symbol from method \"{methodDeclarationSyntax.Identifier}\".");
                var expectedReturnType = this.GetTypeSymbol(returnType);

                if (!SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, expectedReturnType))
                {
                    return false;
                }

                if (methodSymbol.ReturnType.NullableAnnotation != returnTypeNullableAnnotation)
                {
                    return false;
                }

                var hasCorrectNumberOfParameters = methodSymbol.Parameters.Length == parameters.Length;
                if (!hasCorrectNumberOfParameters)
                {
                    return false;
                }

                for (int parameterIndex = 0; parameterIndex < parameters.Length; ++parameterIndex)
                {
                    if (!parameters[parameterIndex].Name.Equals(
                            methodSymbol.Parameters[parameterIndex].Name,
                            StringComparison.Ordinal))
                    {
                        return false;
                    }

                    var expectedType = this.GetTypeSymbol(parameters[parameterIndex].Type);
                    if (!SymbolEqualityComparer.Default.Equals(
                            expectedType,
                            methodSymbol.Parameters[parameterIndex].Type))
                    {
                        return false;
                    }

                    if (parameters[parameterIndex].NullableAnnotation !=
                        methodSymbol.Parameters[parameterIndex].Type.NullableAnnotation)
                    {
                        return false;
                    }
                }

                return true;
            })
            .ToArray();
        methods.Should().HaveCount(1);

        assert(new MethodDeclarationSyntaxAssertions(methods.Single(), this.SemanticModel, this.Compilation));
        return this;
    }

    private ITypeSymbol GetTypeSymbol(Type type)
    {
        var typeParts = type.ToString().Split("[");
        var namedTypeSymbol = this.Compilation.GetTypeByMetadataName(typeParts[0])!;
        if (typeParts.Length > 1)
        {
            var typeArguments = typeParts[^1]
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Split(",")
                .Select(s => this.Compilation.GetTypeByMetadataName(s))
                .Where(t => t is not null)
                .OfType<ITypeSymbol>()
                .ToArray();
            var constructedGenericType = namedTypeSymbol.Construct(typeArguments);
            return constructedGenericType;
        }

        return namedTypeSymbol;
    }
}