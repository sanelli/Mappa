// <copyright file="IdentityMapDeepCopyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for the identity map deep copy sample mappers.
/// </summary>
internal static class IdentityMapDeepCopyMapperRunner
{
    /// <summary>
    /// Runs all identity map deep copy sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var person = new IdentityMapDeepCopyPerson
        {
            Child = new IdentityMapDeepCopyChild { Name = "nested" },
        };
        var personStruct = new IdentityMapDeepCopyStruct
        {
            Child = new IdentityMapDeepCopyChild { Name = "nested" },
        };

        report.BeginMapper(nameof(IdentityMapDeepCopyShallowMapper));
        var shallowMapper = new IdentityMapDeepCopyShallowMapper();
        var shallowResult = shallowMapper.Map(person);
        report.RecordInvocation(
            nameof(IdentityMapDeepCopyShallowMapper.Map),
            nameof(IdentityMapDeepCopyPerson),
            nameof(IdentityMapDeepCopyPerson),
            person,
            $"rootSame={ReferenceEquals(person, shallowResult)};nestedSame={ReferenceEquals(person.Child, shallowResult.Child)}");

        report.BeginMapper(nameof(IdentityMapDeepCopyDeepMapper));
        var deepMapper = new IdentityMapDeepCopyDeepMapper();
        var deepResult = deepMapper.Map(person);
        report.RecordInvocation(
            nameof(IdentityMapDeepCopyDeepMapper.Map),
            nameof(IdentityMapDeepCopyPerson),
            nameof(IdentityMapDeepCopyPerson),
            person,
            $"rootSame={ReferenceEquals(person, deepResult)};nestedSame={ReferenceEquals(person.Child, deepResult.Child)}");

        report.BeginMapper(nameof(IdentityMapDeepCopyNestedMapper));
        var nestedMapper = new IdentityMapDeepCopyNestedMapper();
        var nestedResult = nestedMapper.Map(person);
        report.RecordInvocation(
            nameof(IdentityMapDeepCopyNestedMapper.Map),
            nameof(IdentityMapDeepCopyPerson),
            nameof(IdentityMapDeepCopyPerson),
            person,
            $"rootSame={ReferenceEquals(person, nestedResult)};nestedSame={ReferenceEquals(person.Child, nestedResult.Child)}");

        report.BeginMapper(nameof(IdentityMapDeepCopyNestedStructMapper));
        var nestedStructMapper = new IdentityMapDeepCopyNestedStructMapper();
        var nestedStructResult = nestedStructMapper.Map(personStruct);
        report.RecordInvocation(
            nameof(IdentityMapDeepCopyNestedStructMapper.Map),
            nameof(IdentityMapDeepCopyStruct),
            nameof(IdentityMapDeepCopyStruct),
            personStruct,
            $"nestedSame={ReferenceEquals(personStruct.Child, nestedStructResult.Child)};name={nestedStructResult.Child.Name}");
    }
}