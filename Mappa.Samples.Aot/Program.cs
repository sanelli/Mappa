// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Aot.Runners;

namespace Mappa.Samples.Aot;

/// <summary>
/// Program class.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Entrypoint.
    /// </summary>
    public static void Main()
    {
        var report = new AotReport();
        CollectionToCollectionMapperRunner.Run(report);
        CaseInsensitiveEnumMapperRunner.Run(report);
        CaseInsensitiveEnumToEnumMapperRunner.Run(report);
        ContainersWithCapacityConstructorMapperRunner.Run(report);
        DateAndTimeMapperRunner.Run(report);
        DescriptionEnumToEnumMapperRunner.Run(report);
        DescriptionEnumToStringMapperRunner.Run(report);
        DescriptionStringToEnumMapperRunner.Run(report);
        DictionaryToDictionaryMapperRunner.Run(report);
        DictionaryAssignmentMapperRunner.Run(report);
        EnumToEnumMapperRunner.Run(report);
        EnumToIntegralMapperRunner.Run(report);
        EnumToStringMapperRunner.Run(report);
        EnumerableConcreteTypeMapperRunner.Run(report);
        ExtensionMethodMapperRunner.Run(report);
        FastCollectionToCollectionMapperRunner.Run(report);
        GuidStrategyMapperRunner.Run(report);
        IdentityMapDeepCopyMapperRunner.Run(report);
        IdentityStrategyMapperDupRunner.Run(report);
        IdentityStrategyMapperRunner.Run(report);
        IntegralToEnumMapperRunner.Run(report);
        InvokeConstructorStrategyMapperRunner.Run(report);
        InvokeEmptyConstructorOnPropertyMapperRunner.Run(report);
        InvokeEmptyConstructorStrategyMapperRunner.Run(report);
        InvokeMappingConstructorStrategyMapperRunner.Run(report);
        InvokeParseMapperRunner.Run(report);
        InvokeToStringMapperRunner.Run(report);
        MapMethodStrategyMapperRunner.Run(report);
        MapMethodStrategyWithDependencyMapperRunner.Run(report);
        MapMethodStrategyWithInheritedMapMethodMapperRunner.Run(report);
        MapMethodStrategyWithUserCustomInstanceMethodMapperRunner.Run(report);
        MapMethodStrategyWithUserCustomStaticMethodMapperRunner.Run(report);
        MapWithPropertiesOnBaseClassesMapperRunner.Run(report);
        MappaAssignFromConstantAttributeMapperRunner.Run(report);
        MappaAssignFromContextAttributeMapperRunner.Run(report);
        MappaAssignToContextAttributeMapperRunner.Run(report);
        MappaDependencyProtobufMapperRunner.Run(report);
        MappaIgnoreMappersRunner.Run(report);
        MappaIgnoreTargetPropertyAttributeMapperRunner.Run(report);
        MappaInvokeMethodAttributeMappersRunner.Run(report);
        MappaUsePropertyAttributeMapperRunner.Run(report);
        NestedPropertyPathAttributeMapperRunner.Run(report);
        NullableToNullableMapperRunner.Run(report);
        NumericValueEnumToEnumMapperRunner.Run(report);
        ParamsAndInMapperRunner.Run(report);
        PolymorphicMethodMapMapperRunner.Run(report);
        PolymorphismMappersRunner.Run(report);
        PropertyMapNameSettingsMapperRunner.Run(report);
        PragmaWarningSettingMapperRunner.Run(report);
        ProtobufOptionalMapperRunner.Run(report);
        ReadOnlyTargetCollectionMapperRunner.Run(report);
        ReferenceNullableToReferenceNullableMapperRunner.Run(report);
        RelaxedNullabilityMethodMapMapperRunner.Run(report);
        ReferenceToReferenceWithNullableDisabledMapperRunner.Run(report);
        StringToEnumMapperRunner.Run(report);
        StringToSystemEntitiesMapperRunner.Run(report);
        TupleToTupleMapperRunner.Run(report);
        report.WriteTo(Console.Out);
    }
}