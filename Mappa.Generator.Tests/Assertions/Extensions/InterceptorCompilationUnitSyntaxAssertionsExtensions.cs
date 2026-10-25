// <copyright file="InterceptorCompilationUnitSyntaxAssertionsExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions.Extensions;

/// <summary>
/// Compilation-unit assertions for <c>[MappaInterceptor]</c> generated sources.
/// </summary>
internal static class InterceptorCompilationUnitSyntaxAssertionsExtensions
{
    /// <summary>
    /// Asserts the generated interceptor class contains the default <c>MappaMap</c> stubs
    /// and a concrete interceptor method.
    /// </summary>
    /// <param name="this">Assertions targeting the generated compilation unit.</param>
    /// <param name="className">The interceptor class name.</param>
    /// <param name="interceptorMethodName">The generated interceptor method name.</param>
    /// <param name="targetType">The concrete interceptor return type.</param>
    /// <param name="hasContext"><c>true</c> when the intercepted call used a context argument.</param>
    /// <param name="expectedMethodCount">Expected total methods on the generated partial class.</param>
    /// <param name="interceptorBodyAssertion">Assertions on the interceptor method body.</param>
    /// <returns>The same assertions instance for chaining.</returns>
    public static CompilationUnitSyntaxAssertions HaveInterceptorStubsAndMethod(
        this CompilationUnitSyntaxAssertions @this,
        string className,
        string interceptorMethodName,
        string targetType,
        bool hasContext,
        int expectedMethodCount,
        Action<BlockSyntaxAssertions> interceptorBodyAssertion)
    {
        ArgumentNullException.ThrowIfNull(@this);
        ArgumentNullException.ThrowIfNull(interceptorBodyAssertion);

        return @this
            .HaveCommentHeader()
            .HaveClass(className, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .HaveMethods(expectedMethodCount);

                AssertStubMethod(classAssertions, methodName: "MappaMap", hasContext: false);
                AssertStubMethod(classAssertions, methodName: "MappaMap", hasContext: true);

                var parameters = new List<(string Type, NullableAnnotation NullableAnnotation, string Name, RefKind RefKind, bool IsParams)>
                {
                    ("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false),
                };
                if (hasContext)
                {
                    parameters.Add((
                        typeof(MappaContext).FullName ?? "Mappa.MappaContext",
                        NullableAnnotation.NotAnnotated,
                        "context",
                        RefKind.None,
                        false));
                }

                classAssertions.HaveMethod(
                    targetType,
                    NullableAnnotation.NotAnnotated,
                    interceptorMethodName,
                    false,
                    parameters.ToArray(),
                    methodAssertions =>
                    {
                        methodAssertions
                            .HaveModifiers(SyntaxKind.PrivateKeyword)
                            .HaveInterceptsLocationAttribute()
                            .HaveDebuggerNonUserCodeAttribute()
                            .HaveGeneratedCodeAttribute(_ => { /* presence only */ })
                            .HaveBody(interceptorBodyAssertion);
                    });
            });
    }

    /// <summary>
    /// Asserts only the two interceptable stub methods are present (no interceptor method yet).
    /// </summary>
    /// <param name="this">Assertions targeting the generated compilation unit.</param>
    /// <param name="className">The interceptor class name.</param>
    /// <param name="methodName">The stub method name (<c>MappaMap</c> or renamed).</param>
    /// <returns>The same assertions instance for chaining.</returns>
    public static CompilationUnitSyntaxAssertions HaveInterceptorStubMethodsOnly(
        this CompilationUnitSyntaxAssertions @this,
        string className,
        string methodName = "MappaMap")
    {
        ArgumentNullException.ThrowIfNull(@this);

        return @this
            .HaveCommentHeader()
            .HaveClass(className, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .HaveMethods(2);
                AssertStubMethod(classAssertions, methodName, hasContext: false);
                AssertStubMethod(classAssertions, methodName, hasContext: true);
            });
    }

    private static void AssertStubMethod(
        ClassDeclarationSyntaxAssertions classAssertions,
        string methodName,
        bool hasContext)
    {
        var parameters = new List<(string Type, NullableAnnotation NullableAnnotation, string Name, RefKind RefKind, bool IsParams)>
        {
            ("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false),
        };
        if (hasContext)
        {
            parameters.Add((
                typeof(MappaContext).FullName ?? "Mappa.MappaContext",
                NullableAnnotation.NotAnnotated,
                "context",
                RefKind.None,
                false));
        }

        classAssertions.HaveMethod(
            "TTarget",
            NullableAnnotation.NotAnnotated,
            methodName,
            false,
            parameters.ToArray(),
            methodAssertions =>
            {
                methodAssertions
                    .HaveModifiers(SyntaxKind.PublicKeyword)
                    .HaveDebuggerNonUserCodeAttribute()
                    .HaveGeneratedCodeAttribute(_ => { /* presence only */ })
                    .HaveBody(blockAssertions =>
                    {
                        var expectedNodes = hasContext ? 3 : 2;
                        blockAssertions.HasSyntaxNodesCount(expectedNodes);
                        for (var index = 0; index < expectedNodes - 1; index++)
                        {
                            blockAssertions.HasNextSyntaxNode(_ => { /* discard assignments */ });
                        }

                        blockAssertions.HasNextSyntaxNode(node =>
                        {
                            node.BeThrowStatementSyntax<InvalidOperationException>(
                                message => message.BeLiteralExpressionSyntax(
                                    "This method should have been intercepted by the Mappa source generator."));
                        });
                    });
            });
    }
}