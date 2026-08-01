// <copyright file="ClassDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="ClassDeclarationSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
internal sealed class ClassDeclarationSyntaxAssertions
    : ObjectAssertions<ClassDeclarationSyntax, ClassDeclarationSyntaxAssertions>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    internal ClassDeclarationSyntaxAssertions(
        ClassDeclarationSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

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
        IEnumerable<(Type Type, NullableAnnotation NullableAnnotation, string Name, RefKind RefKind, bool IsParams)> parameters,
        Action<MethodDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(returnType);
        ArgumentNullException.ThrowIfNull(name);

        return this.HaveMethod(
            returnType.ToString(),
            returnTypeNullableAnnotation,
            name,
            parameters.Select(parameter => (parameter.Type.ToString(), parameter.NullableAnnotation, parameter.Name, parameter.RefKind, parameter.IsParams)).ToArray(),
            assert);
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
        string returnType,
        NullableAnnotation returnTypeNullableAnnotation,
        string name,
        (string Type, NullableAnnotation NullableAnnotation, string Name, RefKind RefKind, bool IsParams)[] parameters,
        Action<MethodDeclarationSyntaxAssertions> assert)
        => this.HaveMethod(
            returnType,
            returnTypeNullableAnnotation,
            name,
            false,
            parameters,
            assert);

    /// <summary>
    /// Assert that the class has a method with a specific signature.
    /// </summary>
    /// <param name="returnType">The return type of the method.</param>
    /// <param name="returnTypeNullableAnnotation"><c>true</c> if the method has been annotated with nullability.</param>
    /// <param name="name">The method name.</param>
    /// <param name="isExtensionMethod">The method is an extension method.</param>
    /// <param name="parameters">The expected parameters of the method.</param>
    /// <param name="assert">The assertion on the method.</param>
    /// <returns>The method declaration syntax assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveMethod(
        string returnType,
        NullableAnnotation returnTypeNullableAnnotation,
        string name,
        bool isExtensionMethod,
        (string Type, NullableAnnotation NullableAnnotation, string Name, RefKind RefKind, bool IsParams)[] parameters,
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

                var methodSymbol = this.semanticModel.GetDeclaredSymbol(methodDeclarationSyntax)
                                   ?? throw new MappaGeneratorException(
                                       $"Cannot obtain symbol from method \"{methodDeclarationSyntax.Identifier}\".");
                ITypeSymbol? expectedReturnType = this.compilation.GetTypeSymbol(returnType);

                if (methodSymbol.IsExtensionMethod != isExtensionMethod)
                {
                    return false;
                }

                var correctReturnType = (expectedReturnType != null && SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, expectedReturnType))
                                        || (expectedReturnType == null && methodSymbol.ReturnType.ToDisplayString().Equals(returnType, StringComparison.Ordinal));

                if (!correctReturnType)
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
                    var expectedParameter = parameters[parameterIndex];
                    var actualParameter = methodSymbol.Parameters[parameterIndex];

                    if (!expectedParameter.Name.Equals(actualParameter.Name, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    var expectedType = this.compilation.GetTypeSymbol(expectedParameter.Type);
                    var correctParameterType = (expectedType is not null && SymbolEqualityComparer.Default.Equals(expectedType, actualParameter.Type))
                                               || (expectedType is null && actualParameter.Type.ToDisplayString().Equals(expectedParameter.Type, StringComparison.Ordinal));
                    if (!correctParameterType)
                    {
                        return false;
                    }

                    if (expectedParameter.NullableAnnotation !=
                        actualParameter.Type.NullableAnnotation)
                    {
                        return false;
                    }

                    if (actualParameter.RefKind != expectedParameter.RefKind)
                    {
                        return false;
                    }

                    if (actualParameter.IsParams != expectedParameter.IsParams)
                    {
                        return false;
                    }
                }

                return true;
            })
            .ToArray();
        methods.Should().HaveCount(1);

        assert(new MethodDeclarationSyntaxAssertions(methods.Single(), this.semanticModel, this.compilation));
        return this;
    }

    /// <summary>
    /// Assert that the class has an <c>extern static</c> method with a
    /// <c>UnsafeAccessor</c> attribute, identified by method name only (syntax-based).
    /// </summary>
    /// <param name="methodName">The generated method identifier.</param>
    /// <param name="unsafeAccessorKind">The expected <c>UnsafeAccessorKind</c> name.</param>
    /// <param name="runtimeName">The expected <c>Name</c> attribute argument.</param>
    /// <returns>The assertions.</returns>
    public ClassDeclarationSyntaxAssertions HaveExternUnsafeAccessorMethod(
        string methodName,
        string unsafeAccessorKind,
        string runtimeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        var methods = this.Subject.ChildNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(methodDeclarationSyntax =>
                methodDeclarationSyntax.Identifier.ToString().Equals(methodName, StringComparison.Ordinal))
            .ToArray();
        methods.Should().HaveCount(1);

        new MethodDeclarationSyntaxAssertions(methods.Single(), this.semanticModel, this.compilation)
            .HaveModifiers(SyntaxKind.ExternKeyword, SyntaxKind.StaticKeyword)
            .HaveUnsafeAccessorAttribute(unsafeAccessorKind, runtimeName)
            .HaveNoBody();

        return this;
    }
}