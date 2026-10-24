// <copyright file="MappaSettingsAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaSettingsAttribute"/>.
/// </summary>
public sealed class MappaSettingsAttributeTests
{
    /// <summary>
    /// Tests static sentinel values for unset style and depth settings.
    /// </summary>
    [Fact]
    [UnitTest]
    public void StaticSentinelValuesMatchDocumentedDefaults()
    {
        // Assert
        MappaSettingsAttribute.UndefinedDateTimeStyle.Should().Be((DateTimeStyles)(-1));
        MappaSettingsAttribute.UndefinedNumberStyle.Should().Be((NumberStyles)(-1));
        MappaSettingsAttribute.UndefinedDepth.Should().Be((short)(-1));
    }

    /// <summary>
    /// Tests the attribute targets methods and classes, and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodAndClassWithoutMultiple()
    {
        // Act
        var attributeUsage = typeof(MappaSettingsAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method | AttributeTargets.Class);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }

    /// <summary>
    /// Tests that a newly constructed attribute exposes the documented default for every property.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorDefaultsAllProperties()
    {
        // Act
        var attribute = new MappaSettingsAttribute();

        // Assert — string formats
        attribute.DateTimeFormat.Should().BeNull();
        attribute.DateTimeOffsetFormat.Should().BeNull();
        attribute.DateOnlyFormat.Should().BeNull();
        attribute.TimeOnlyFormat.Should().BeNull();
        attribute.TimeSpanFormat.Should().BeNull();
        attribute.GuidFormat.Should().BeNull();
        attribute.ByteFormat.Should().BeNull();
        attribute.SByteFormat.Should().BeNull();
        attribute.ShortFormat.Should().BeNull();
        attribute.UShortFormat.Should().BeNull();
        attribute.IntFormat.Should().BeNull();
        attribute.UIntFormat.Should().BeNull();
        attribute.LongFormat.Should().BeNull();
        attribute.ULongFormat.Should().BeNull();
        attribute.DecimalFormat.Should().BeNull();
        attribute.FloatFormat.Should().BeNull();
        attribute.DoubleFormat.Should().BeNull();
        attribute.CultureName.Should().BeNull();
        attribute.ProjectionGuidFormat.Should().BeNull();
        attribute.ProjectionCultureName.Should().BeNull();

        // Assert — date/time styles
        attribute.DateTimeStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);
        attribute.DateTimeOffsetStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);
        attribute.DateOnlyStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);
        attribute.TimeOnlyStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);
        attribute.GlobalDateTimeStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);

        // Assert — number styles
        attribute.ByteStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.SByteStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.ShortStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.UShortStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.IntStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.UIntStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.LongStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.ULongStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.DecimalStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.FloatStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.DoubleStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
        attribute.GlobalNumberStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);

        // Assert — culture / pragma / enum settings
        attribute.CultureInfoSetting.Should().Be(CultureInfoSetting.Undefined);
        attribute.PragmaWarning.Should().Be(PragmaWarningSetting.Undefined);
        attribute.EnumerableConcreteType.Should().Be(EnumerableConcreteTypeSetting.Undefined);
        attribute.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Undefined);
        attribute.EnumStringMapSetting.Should().Be(EnumStringMapSetting.Undefined);
        attribute.EnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.Undefined);
        attribute.IdentityMapDeepCopy.Should().Be(IdentityMapDeepCopySetting.Undefined);
        attribute.ProjectionEnumStringMapSetting.Should().Be(EnumStringMapSetting.Undefined);
        attribute.ProjectionEnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.Undefined);
        attribute.ProjectionCultureInfoSetting.Should().Be(CultureInfoSetting.Undefined);

        // Assert — boolean settings
        attribute.ProtobufOptional.Should().Be(BooleanSetting.Undefined);
        attribute.BsonWellKnownTypes.Should().Be(BooleanSetting.Undefined);
        attribute.ProtobufWellKnownTypes.Should().Be(BooleanSetting.Undefined);
        attribute.OneOfWellKnownTypes.Should().Be(BooleanSetting.Undefined);
        attribute.FastCollections.Should().Be(BooleanSetting.Undefined);
        attribute.ContainerCapacityConstructors.Should().Be(BooleanSetting.Undefined);
        attribute.PreventEnumerableCount.Should().Be(BooleanSetting.Undefined);
        attribute.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Undefined);
        attribute.PolymorphicMapMethodWithMatchingDefaultAttribute.Should().Be(BooleanSetting.Undefined);
        attribute.CompatibleMapMethod.Should().Be(BooleanSetting.Undefined);
        attribute.MappaTransformerFromDependency.Should().Be(BooleanSetting.Undefined);
        attribute.CaseInsensitivePropertyMap.Should().Be(BooleanSetting.Undefined);
        attribute.IgnoreUnderscoreForPropertyMap.Should().Be(BooleanSetting.Undefined);
        attribute.MapSourceExplicitInterfaceProperties.Should().Be(BooleanSetting.Undefined);
        attribute.MapTargetExplicitInterfaceProperties.Should().Be(BooleanSetting.Undefined);
        attribute.MapSourceFields.Should().Be(BooleanSetting.Undefined);
        attribute.MapTargetFields.Should().Be(BooleanSetting.Undefined);
        attribute.CaseInsensitiveEnumMap.Should().Be(BooleanSetting.Undefined);
        attribute.ReferenceReusing.Should().Be(BooleanSetting.Undefined);
        attribute.BreakCompileTimeCycles.Should().Be(BooleanSetting.Undefined);
        attribute.ProjectionCaseInsensitiveEnumMap.Should().Be(BooleanSetting.Undefined);

        // Assert — depths
        attribute.MaxRuntimeDepth.Should().Be(MappaSettingsAttribute.UndefinedDepth);
        attribute.MaxCompileTimeDepth.Should().Be(MappaSettingsAttribute.UndefinedDepth);
    }

    /// <summary>
    /// Tests that every instance property can be assigned via an object initializer.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllPropertiesCanBeSetViaObjectInitializer()
    {
        // Act
        var attribute = new MappaSettingsAttribute
        {
            DateTimeFormat = "O",
            DateTimeOffsetFormat = "o",
            DateOnlyFormat = "yyyy-MM-dd",
            TimeOnlyFormat = "HH:mm",
            DateTimeStyle = DateTimeStyles.AssumeUniversal,
            DateTimeOffsetStyle = DateTimeStyles.AdjustToUniversal,
            DateOnlyStyle = DateTimeStyles.AllowWhiteSpaces,
            TimeOnlyStyle = DateTimeStyles.NoCurrentDateDefault,
            GlobalDateTimeStyle = DateTimeStyles.RoundtripKind,
            TimeSpanFormat = "c",
            GuidFormat = "D",
            ByteFormat = "X2",
            SByteFormat = "G",
            ShortFormat = "N0",
            UShortFormat = "N0",
            IntFormat = "N0",
            UIntFormat = "N0",
            LongFormat = "N0",
            ULongFormat = "N0",
            DecimalFormat = "F2",
            FloatFormat = "F2",
            DoubleFormat = "F2",
            ByteStyle = NumberStyles.HexNumber,
            SByteStyle = NumberStyles.Integer,
            ShortStyle = NumberStyles.Integer,
            UShortStyle = NumberStyles.Integer,
            IntStyle = NumberStyles.Integer,
            UIntStyle = NumberStyles.Integer,
            LongStyle = NumberStyles.Integer,
            ULongStyle = NumberStyles.Integer,
            DecimalStyle = NumberStyles.Number,
            FloatStyle = NumberStyles.Float,
            DoubleStyle = NumberStyles.Float,
            GlobalNumberStyle = NumberStyles.Any,
            CultureInfoSetting = CultureInfoSetting.InvariantCulture,
            CultureName = "en-GB",
            ProtobufOptional = BooleanSetting.Enable,
            BsonWellKnownTypes = BooleanSetting.Enable,
            ProtobufWellKnownTypes = BooleanSetting.Enable,
            OneOfWellKnownTypes = BooleanSetting.Enable,
            PragmaWarning = PragmaWarningSetting.Disable,
            FastCollections = BooleanSetting.Enable,
            ContainerCapacityConstructors = BooleanSetting.Enable,
            PreventEnumerableCount = BooleanSetting.Enable,
            ReplaceNullWithEmptyContainer = BooleanSetting.Enable,
            EnumerableConcreteType = EnumerableConcreteTypeSetting.Array,
            DictionaryAssignment = DictionaryAssignmentSetting.Add,
            PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable,
            CompatibleMapMethod = BooleanSetting.Enable,
            MappaTransformerFromDependency = BooleanSetting.Enable,
            CaseInsensitivePropertyMap = BooleanSetting.Enable,
            IgnoreUnderscoreForPropertyMap = BooleanSetting.Enable,
            MapSourceExplicitInterfaceProperties = BooleanSetting.Enable,
            MapTargetExplicitInterfaceProperties = BooleanSetting.Enable,
            MapSourceFields = BooleanSetting.Enable,
            MapTargetFields = BooleanSetting.Enable,
            CaseInsensitiveEnumMap = BooleanSetting.Enable,
            EnumStringMapSetting = EnumStringMapSetting.Description,
            EnumToEnumMapSetting = EnumToEnumMapSetting.MemberName,
            IdentityMapDeepCopy = IdentityMapDeepCopySetting.ShallowCopy,
            ReferenceReusing = BooleanSetting.Enable,
            MaxRuntimeDepth = 7,
            MaxCompileTimeDepth = 42,
            BreakCompileTimeCycles = BooleanSetting.Enable,
            ProjectionEnumStringMapSetting = EnumStringMapSetting.MemberName,
            ProjectionEnumToEnumMapSetting = EnumToEnumMapSetting.MemberName,
            ProjectionCaseInsensitiveEnumMap = BooleanSetting.Enable,
            ProjectionGuidFormat = "N",
            ProjectionCultureInfoSetting = CultureInfoSetting.CurrentCulture,
            ProjectionCultureName = "it-IT",
        };

        // Assert — string formats
        attribute.DateTimeFormat.Should().Be("O");
        attribute.DateTimeOffsetFormat.Should().Be("o");
        attribute.DateOnlyFormat.Should().Be("yyyy-MM-dd");
        attribute.TimeOnlyFormat.Should().Be("HH:mm");
        attribute.TimeSpanFormat.Should().Be("c");
        attribute.GuidFormat.Should().Be("D");
        attribute.ByteFormat.Should().Be("X2");
        attribute.SByteFormat.Should().Be("G");
        attribute.ShortFormat.Should().Be("N0");
        attribute.UShortFormat.Should().Be("N0");
        attribute.IntFormat.Should().Be("N0");
        attribute.UIntFormat.Should().Be("N0");
        attribute.LongFormat.Should().Be("N0");
        attribute.ULongFormat.Should().Be("N0");
        attribute.DecimalFormat.Should().Be("F2");
        attribute.FloatFormat.Should().Be("F2");
        attribute.DoubleFormat.Should().Be("F2");
        attribute.CultureName.Should().Be("en-GB");
        attribute.ProjectionGuidFormat.Should().Be("N");
        attribute.ProjectionCultureName.Should().Be("it-IT");

        // Assert — date/time styles
        attribute.DateTimeStyle.Should().Be(DateTimeStyles.AssumeUniversal);
        attribute.DateTimeOffsetStyle.Should().Be(DateTimeStyles.AdjustToUniversal);
        attribute.DateOnlyStyle.Should().Be(DateTimeStyles.AllowWhiteSpaces);
        attribute.TimeOnlyStyle.Should().Be(DateTimeStyles.NoCurrentDateDefault);
        attribute.GlobalDateTimeStyle.Should().Be(DateTimeStyles.RoundtripKind);

        // Assert — number styles
        attribute.ByteStyle.Should().Be(NumberStyles.HexNumber);
        attribute.SByteStyle.Should().Be(NumberStyles.Integer);
        attribute.ShortStyle.Should().Be(NumberStyles.Integer);
        attribute.UShortStyle.Should().Be(NumberStyles.Integer);
        attribute.IntStyle.Should().Be(NumberStyles.Integer);
        attribute.UIntStyle.Should().Be(NumberStyles.Integer);
        attribute.LongStyle.Should().Be(NumberStyles.Integer);
        attribute.ULongStyle.Should().Be(NumberStyles.Integer);
        attribute.DecimalStyle.Should().Be(NumberStyles.Number);
        attribute.FloatStyle.Should().Be(NumberStyles.Float);
        attribute.DoubleStyle.Should().Be(NumberStyles.Float);
        attribute.GlobalNumberStyle.Should().Be(NumberStyles.Any);

        // Assert — culture / pragma / enum settings
        attribute.CultureInfoSetting.Should().Be(CultureInfoSetting.InvariantCulture);
        attribute.PragmaWarning.Should().Be(PragmaWarningSetting.Disable);
        attribute.EnumerableConcreteType.Should().Be(EnumerableConcreteTypeSetting.Array);
        attribute.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Add);
        attribute.EnumStringMapSetting.Should().Be(EnumStringMapSetting.Description);
        attribute.EnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.MemberName);
        attribute.IdentityMapDeepCopy.Should().Be(IdentityMapDeepCopySetting.ShallowCopy);
        attribute.ProjectionEnumStringMapSetting.Should().Be(EnumStringMapSetting.MemberName);
        attribute.ProjectionEnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.MemberName);
        attribute.ProjectionCultureInfoSetting.Should().Be(CultureInfoSetting.CurrentCulture);

        // Assert — boolean settings
        attribute.ProtobufOptional.Should().Be(BooleanSetting.Enable);
        attribute.BsonWellKnownTypes.Should().Be(BooleanSetting.Enable);
        attribute.ProtobufWellKnownTypes.Should().Be(BooleanSetting.Enable);
        attribute.OneOfWellKnownTypes.Should().Be(BooleanSetting.Enable);
        attribute.FastCollections.Should().Be(BooleanSetting.Enable);
        attribute.ContainerCapacityConstructors.Should().Be(BooleanSetting.Enable);
        attribute.PreventEnumerableCount.Should().Be(BooleanSetting.Enable);
        attribute.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Enable);
        attribute.PolymorphicMapMethodWithMatchingDefaultAttribute.Should().Be(BooleanSetting.Enable);
        attribute.CompatibleMapMethod.Should().Be(BooleanSetting.Enable);
        attribute.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
        attribute.CaseInsensitivePropertyMap.Should().Be(BooleanSetting.Enable);
        attribute.IgnoreUnderscoreForPropertyMap.Should().Be(BooleanSetting.Enable);
        attribute.MapSourceExplicitInterfaceProperties.Should().Be(BooleanSetting.Enable);
        attribute.MapTargetExplicitInterfaceProperties.Should().Be(BooleanSetting.Enable);
        attribute.MapSourceFields.Should().Be(BooleanSetting.Enable);
        attribute.MapTargetFields.Should().Be(BooleanSetting.Enable);
        attribute.CaseInsensitiveEnumMap.Should().Be(BooleanSetting.Enable);
        attribute.ReferenceReusing.Should().Be(BooleanSetting.Enable);
        attribute.BreakCompileTimeCycles.Should().Be(BooleanSetting.Enable);
        attribute.ProjectionCaseInsensitiveEnumMap.Should().Be(BooleanSetting.Enable);

        // Assert — depths
        attribute.MaxRuntimeDepth.Should().Be((short)7);
        attribute.MaxCompileTimeDepth.Should().Be((short)42);
    }

    /// <summary>
    /// Tests that <see cref="MappaSettingsAttribute.OneOfWellKnownTypes"/> round-trips
    /// <see cref="BooleanSetting.Enable"/> and <see cref="BooleanSetting.Disable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfWellKnownTypesRoundTripsEnableAndDisable()
    {
        // Act
        var enabled = new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable };
        var disabled = new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Disable };

        // Assert
        enabled.OneOfWellKnownTypes.Should().Be(BooleanSetting.Enable);
        disabled.OneOfWellKnownTypes.Should().Be(BooleanSetting.Disable);
    }
}