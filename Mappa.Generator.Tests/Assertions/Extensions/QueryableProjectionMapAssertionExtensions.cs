// <copyright file="QueryableProjectionMapAssertionExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions.Extensions;

/// <summary>
/// Assertion extensions for queryable projection map methods.
/// </summary>
internal static class QueryableProjectionMapAssertionExtensions
{
    /// <summary>
    /// The namespace used by queryable projection integration test source code.
    /// </summary>
    internal const string TestNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// The message emitted on generated queryable projection methods.
    /// </summary>
    internal const string RequiresDynamicCodeMessage =
        "\"Queryable projection uses expression trees that require dynamic code generation and are not compatible with Native AOT.\"";

    /// <summary>
    /// Builds the <see cref="System.Linq.IQueryable{T}"/> type display string for an element type.
    /// </summary>
    /// <param name="elementType">The element type display string.</param>
    /// <returns>The queryable type display string.</returns>
    internal static string QueryableOf(string elementType)
        => $"System.Linq.IQueryable<{elementType}>";

    /// <summary>
    /// Assert that the compilation unit contains a queryable projection map method and its element method.
    /// </summary>
    /// <param name="this">The compilation unit syntax assertions.</param>
    /// <param name="className">The mapper class name.</param>
    /// <param name="classModifiers">The class modifiers.</param>
    /// <param name="methodName">The projection method name.</param>
    /// <param name="methodModifiers">The projection method modifiers.</param>
    /// <param name="isExtensionMethod">Whether the projection method is an extension method.</param>
    /// <param name="isStaticMapper">Whether the mapper class is static.</param>
    /// <param name="parameterName">The query parameter name.</param>
    /// <param name="sourceElementType">The source element type display string.</param>
    /// <param name="targetElementType">The target element type display string.</param>
    /// <param name="lambdaParameterName">The generated lambda parameter name.</param>
    /// <param name="elementExpressionAssertions">Assertions on the projection element expression.</param>
    /// <returns>The input compilation unit syntax assertions.</returns>
    internal static CompilationUnitSyntaxAssertions HaveQueryableProjectionMapMethod(
        this CompilationUnitSyntaxAssertions @this,
        string className,
        SyntaxKind[] classModifiers,
        string methodName,
        SyntaxKind[] methodModifiers,
        bool isExtensionMethod,
        bool isStaticMapper,
        string parameterName,
        string sourceElementType,
        string targetElementType,
        string lambdaParameterName,
        Action<ExpressionSyntaxAssertions> elementExpressionAssertions)
    {
        ArgumentNullException.ThrowIfNull(@this);
        ArgumentNullException.ThrowIfNull(elementExpressionAssertions);

        var queryableSourceType = QueryableOf(sourceElementType);
        var queryableTargetType = QueryableOf(targetElementType);
        var elementMethodModifiers = isStaticMapper
            ? new[] { SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword }
            : new[] { SyntaxKind.PrivateKeyword };

        return @this.HaveCommentHeader()
            .HaveFileScopedNamespace(fileScopedNamespaceDeclarationSyntaxAssertions =>
            {
                fileScopedNamespaceDeclarationSyntaxAssertions
                    .HaveClasses(1)
                    .HaveClass(
                        className,
                        classDeclarationSyntaxAssertions =>
                        {
                            classDeclarationSyntaxAssertions
                                .HaveModifiers(classModifiers)
                                .HaveMethods(2)
                                .HaveMethod(
                                    queryableTargetType,
                                    NullableAnnotation.NotAnnotated,
                                    methodName,
                                    isExtensionMethod,
                                    [(queryableSourceType, NullableAnnotation.NotAnnotated, parameterName, RefKind.None, false)],
                                    methodDeclarationSyntaxAssertions =>
                                    {
                                        methodDeclarationSyntaxAssertions
                                            .HaveNullabilityAnnotation(NullableSetup.Enable)
                                            .HavePragmaWarningDisableAnnotation(PragmaWarning.NoBlock)
                                            .HaveRequiresDynamicCodeAttribute(RequiresDynamicCodeMessage)
                                            .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                                            .HaveDebuggerNonUserCodeAttribute()
                                            .HaveModifiers(methodModifiers)
                                            .HaveBody(blockSyntaxAssertions =>
                                            {
                                                blockSyntaxAssertions
                                                    .HasSyntaxNodesCount(1)
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeReturnStatement(returnExpressionAssertions =>
                                                        {
                                                            returnExpressionAssertions.BeInvocationExpressionSyntax(
                                                                "global::System.Linq.Queryable.Select",
                                                                queryArgumentAssertions => queryArgumentAssertions.BeIdentifierNameSyntax(parameterName),
                                                                lambdaArgumentAssertions => lambdaArgumentAssertions.BeSimpleLambdaExpressionSyntax(
                                                                    lambdaParameterName,
                                                                    elementExpressionAssertions));
                                                        });
                                                    });
                                            });
                                    })
                                .HaveMethod(
                                    targetElementType,
                                    NullableAnnotation.NotAnnotated,
                                    $"{methodName}Element",
                                    false,
                                    [(sourceElementType, NullableAnnotation.NotAnnotated, lambdaParameterName, RefKind.None, false)],
                                    methodDeclarationSyntaxAssertions =>
                                    {
                                        methodDeclarationSyntaxAssertions
                                            .HaveNullabilityAnnotation(NullableSetup.Enable)
                                            .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                                            .HaveDebuggerNonUserCodeAttribute()
                                            .HaveModifiers(elementMethodModifiers)
                                            .HaveBody(blockSyntaxAssertions =>
                                            {
                                                blockSyntaxAssertions
                                                    .HasSyntaxNodesCount(1)
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeReturnStatement(elementExpressionAssertions);
                                                    });
                                            });
                                    });
                        });
            });
    }
}