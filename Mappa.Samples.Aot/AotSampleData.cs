// <copyright file="AotSampleData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Globalization;

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

using PolymorphismOne = Mappa.Samples.Models.Polymorphism.One;
using PolymorphismThree = Mappa.Samples.Models.Polymorphism.Three;
using PolymorphismTwo = Mappa.Samples.Models.Polymorphism.Two;

namespace Mappa.Samples.Aot;

/// <summary>
/// Shared sample inputs for AOT mapper invocations.
/// </summary>
internal static class AotSampleData
{
    /// <summary>
    /// Gets the standard dictionary used by dictionary mapper tests.
    /// </summary>
    public static Dictionary<int, CountingValues> IntCountingValuesDictionary { get; } = new()
    {
        { 1, CountingValues.One },
        { 2, CountingValues.Two },
        { 3, CountingValues.Three },
    };

    /// <summary>
    /// Gets the same entries as <see cref="IntCountingValuesDictionary"/> for <see cref="IDictionary{TKey,TValue}"/> inputs.
    /// </summary>
    public static IDictionary<int, CountingValues> IntCountingValuesAsIDictionary { get; } =
        new Dictionary<int, CountingValues>(IntCountingValuesDictionary);

    /// <summary>
    /// Gets the read-only view of the standard dictionary.
    /// </summary>
    public static IReadOnlyDictionary<int, CountingValues> IntCountingValuesAsIReadOnlyDictionary { get; } =
        IntCountingValuesDictionary;

    /// <summary>
    /// Gets the key-value pairs for enumerable dictionary mapping tests.
    /// </summary>
    public static IEnumerable<KeyValuePair<int, CountingValues>> IntCountingValuesAsKeyValuePairs { get; } =
        IntCountingValuesDictionary;

    /// <summary>
    /// Gets the custom generic dictionary sample.
    /// </summary>
    public static CustomDictionaryWithGeneric<int, CountingValues> CustomGenericIntCountingDictionary { get; } = new()
    {
        { 1, CountingValues.One },
        { 2, CountingValues.Two },
        { 3, CountingValues.Three },
    };

    /// <summary>
    /// Gets the custom non-generic dictionary sample.
    /// </summary>
    public static CustomDictionaryIntToCountingValues CustomIntCountingDictionary { get; } = new()
    {
        { 1, CountingValues.One },
        { 2, CountingValues.Two },
        { 3, CountingValues.Three },
    };

    /// <summary>
    /// Gets the sorted dictionary sample.
    /// </summary>
    public static SortedDictionary<int, CountingValues> IntCountingValuesSortedDictionary { get; } = new()
    {
        { 1, CountingValues.One },
        { 2, CountingValues.Two },
        { 3, CountingValues.Three },
    };

    /// <summary>
    /// Gets the concurrent dictionary sample.
    /// </summary>
    public static ConcurrentDictionary<int, CountingValues> IntCountingValuesConcurrentDictionary { get; } = new()
    {
        [1] = CountingValues.One,
        [2] = CountingValues.Two,
        [3] = CountingValues.Three,
    };

    /// <summary>
    /// Gets an integer array sample.
    /// </summary>
    public static int[] IntArray { get; } = [1, 2, 3];

    /// <summary>
    /// Gets an integer enumerable sample.
    /// </summary>
    public static IEnumerable<int> IntEnumerable { get; } = IntArray;

    /// <summary>
    /// Gets a counting values array sample.
    /// </summary>
    public static CountingValues[] CountingValuesArray { get; } =
        [CountingValues.Three, CountingValues.Two, CountingValues.One];

    /// <summary>
    /// Gets a counting values list sample.
    /// </summary>
    public static List<CountingValues> CountingValuesList { get; } =
        [CountingValues.Three, CountingValues.Two, CountingValues.One];

    /// <summary>
    /// Gets a fixed guid sample.
    /// </summary>
    public static Guid SampleGuid { get; } = new("11111111-2222-3333-4444-555555555555");

    /// <summary>
    /// Gets the byte representation of <see cref="SampleGuid"/>.
    /// </summary>
    public static byte[] SampleGuidBytes { get; } = SampleGuid.ToByteArray();

    /// <summary>
    /// Gets a string integer sample for value-type nullable mapping.
    /// </summary>
    public static string IntegerString { get; } = "30";

    /// <summary>
    /// Gets an unused object sample for constant assignment mapping.
    /// </summary>
    public static object UnusedObject { get; } = new();

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 123 and ParamB Three.
    /// </summary>
    public static SourceClassModel SourceClassModel123Three { get; } = new()
    {
        ParamA = 123,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 17 and ParamB Three.
    /// </summary>
    public static SourceClassModel SourceClassModel17Three { get; } = new()
    {
        ParamA = 17,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 13 and ParamB One.
    /// </summary>
    public static SourceClassModel SourceClassModel13One { get; } = new()
    {
        ParamA = 13,
        ParamB = CountingValues.One,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 13 and ParamB Three.
    /// </summary>
    public static SourceClassModel SourceClassModel13Three { get; } = new()
    {
        ParamA = 13,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 10 and ParamB Three.
    /// </summary>
    public static SourceClassModel SourceClassModel10Three { get; } = new()
    {
        ParamA = 10,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassModel"/> with ParamA 33 and ParamB Three.
    /// </summary>
    public static SourceClassModel SourceClassModel33Three { get; } = new()
    {
        ParamA = 33,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceRecordModel"/> with ParamA 123 and ParamB Three.
    /// </summary>
    public static SourceRecordModel SourceRecordModel123Three { get; } = new(123, CountingValues.Three);

    /// <summary>
    /// Gets a <see cref="SourceRecordModel"/> with ParamA 17 and ParamB Three.
    /// </summary>
    public static SourceRecordModel SourceRecordModel17Three { get; } = new(17, CountingValues.Three);

    /// <summary>
    /// Gets a <see cref="SourceRecordModelWithEmptyConstructor"/> with ParamA 123 and ParamB Three.
    /// </summary>
    public static SourceRecordModelWithEmptyConstructor SourceRecordModelWithEmptyConstructor123Three { get; } = new()
    {
        ParamA = 123,
        ParamB = CountingValues.Three,
    };

    /// <summary>
    /// Gets a <see cref="SourceClassWithInnerClassModel"/> with inner ParamA 33 and ParamB One.
    /// </summary>
    public static SourceClassWithInnerClassModel SourceClassWithInnerClassModel33One { get; } = new()
    {
        InnerModel = new SourceClassModel
        {
            ParamA = 33,
            ParamB = CountingValues.One,
        },
    };

    /// <summary>
    /// Gets a <see cref="SourceClassWithInnerClassModel"/> with inner ParamA 13 and ParamB Three.
    /// </summary>
    public static SourceClassWithInnerClassModel SourceClassWithInnerClassModel13Three { get; } = new()
    {
        InnerModel = new SourceClassModel
        {
            ParamA = 13,
            ParamB = CountingValues.Three,
        },
    };

    /// <summary>
    /// Gets a <see cref="SourceClassWithMultipleFieldsForDependencyModel"/> sample.
    /// </summary>
    public static SourceClassWithMultipleFieldsForDependencyModel SourceClassWithMultipleFieldsForDependencyModel { get; } = new()
    {
        InnerModel = new SourceClassModel
        {
            ParamA = 33,
            ParamB = CountingValues.One,
        },
        Property1 = 1,
        Property2 = 2,
        Property3 = 3,
        Property4 = 4,
        Property5 = 5,
        Property6 = 6,
        Property7 = 7,
    };

    /// <summary>
    /// Gets a derived class source model sample.
    /// </summary>
    public static DerivedClassSourceModel DerivedClassSourceModel { get; } = new()
    {
        BooleanProperty = true,
        ByteProperty = 17,
        CharProperty = 'C',
        StringProperty = "hello",
        IntegerProperty = 123,
        LongProperty = long.MaxValue,
    };

    /// <summary>
    /// Gets a derived interface model sample.
    /// </summary>
    public static IDerivedInterfaceModel DerivedInterfaceModel { get; } = new DerivedInterfaceSampleModel();

    /// <summary>
    /// Gets a source class with collections sample.
    /// </summary>
    public static SourceClassWithCollections SourceClassWithCollections { get; } = new(
        [1, 2, 3],
        [4, 5, 6],
        [7, 8, 9],
        [10, 11, 12],
        [13, 14, 15],
        [16, 17, 18],
        new Dictionary<int, string>
        {
            [19] = "119",
            [20] = "120",
            [21] = "121",
        },
        new Dictionary<int, string>
        {
            [22] = "122",
            [23] = "123",
            [24] = "124",
        },
        [25, 26, 27],
        [28, 29, 30],
        new Dictionary<int, string>
        {
            [31] = "131",
            [32] = "132",
            [33] = "133",
        },
        new Dictionary<int, string>
        {
            [34] = "134",
            [35] = "135",
            [36] = "136",
        });

    /// <summary>
    /// Gets a protobuf optional source model with values set.
    /// </summary>
    public static SourceProtobufOptionalModel SourceProtobufOptionalModelWithValues { get; } = new()
    {
        ParamA = 33,
        ParamB = ProtobufCountingValues.Three,
    };

    /// <summary>
    /// Gets a dependency protobuf source record sample.
    /// </summary>
    public static MappaDependencySourceRecord MappaDependencySourceRecord { get; } =
        new(new DateTime(1984, 06, 03, 14, 22, 00, DateTimeKind.Utc));

    /// <summary>
    /// Gets a <see cref="MappaContext"/> assigning paramB to 33.
    /// </summary>
    public static MappaContext ParamBContext33 { get; } = new() { ["paramB"] = 33 };

    /// <summary>
    /// Gets a <see cref="MappaContext"/> with a custom string value.
    /// </summary>
    public static MappaContext CustomValueContext { get; } = new Dictionary<string, object>
    {
        ["CustomValue"] = "Use the custom value",
    };

    /// <summary>
    /// Gets a system tuple sample for tuple mapping tests.
    /// </summary>
    public static Tuple<int, CountingValues, long> IntCountingLongSystemTuple { get; } =
        new(3, CountingValues.Three, 30L);

    /// <summary>
    /// Gets a value tuple sample for tuple mapping tests.
    /// </summary>
    public static (int Item1, CountingValues Item2, long Item3) IntCountingLongValueTuple { get; } =
        (3, CountingValues.Three, 30L);

    /// <summary>
    /// Gets a named value tuple sample for tuple mapping tests.
    /// </summary>
    public static (int Alpha, CountingValues Beta, long Gamma) NamedIntCountingLongValueTuple { get; } =
        (3, CountingValues.Three, 30L);

    /// <summary>
    /// Gets a four-element value tuple sample for tuple mapping tests.
    /// </summary>
    public static (int Item1, CountingValues Item2, long Item3, string Item4) IntCountingLongStringValueTuple { get; } =
        (3, CountingValues.Three, 30L, "Stefano");

    /// <summary>
    /// Gets a fixed UTC <see cref="DateTime"/> sample.
    /// </summary>
    public static DateTime UtcDateTime { get; } = new(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc);

    /// <summary>
    /// Gets a fixed <see cref="DateOnly"/> sample.
    /// </summary>
    public static DateOnly SampleDateOnly { get; } = DateOnly.FromDateTime(UtcDateTime);

    /// <summary>
    /// Gets a fixed UTC <see cref="DateTimeOffset"/> sample.
    /// </summary>
    public static DateTimeOffset UtcDateTimeOffset { get; } = new(UtcDateTime);

    /// <summary>
    /// Gets a fixed <see cref="TimeSpan"/> sample.
    /// </summary>
    public static TimeSpan SampleTimeSpan { get; } = TimeSpan.FromMilliseconds(1234);

    /// <summary>
    /// Gets the long value 100 used by date/time mapper tests.
    /// </summary>
    public static long Long100 { get; } = 100L;

    /// <summary>
    /// Gets the uint value 100 used by date/time mapper tests.
    /// </summary>
    public static uint Uint100 { get; } = 100;

    /// <summary>
    /// Gets the int value 100 used by date/time mapper tests.
    /// </summary>
    public static int Int100 { get; } = 100;

    /// <summary>
    /// Gets the ushort value 100 used by date/time mapper tests.
    /// </summary>
    public static ushort UShort100 { get; } = 100;

    /// <summary>
    /// Gets the short value 100 used by date/time mapper tests.
    /// </summary>
    public static short Short100 { get; } = 100;

    /// <summary>
    /// Gets the sbyte value 100 used by date/time mapper tests.
    /// </summary>
    public static sbyte SByte100 { get; } = 100;

    /// <summary>
    /// Gets the byte value 100 used by date/time mapper tests.
    /// </summary>
    public static byte Byte100 { get; } = 100;

    /// <summary>
    /// Gets the double value 100 used by date/time mapper tests.
    /// </summary>
    public static double Double100 { get; } = 100;

    /// <summary>
    /// Gets the float value 100 used by date/time mapper tests.
    /// </summary>
    public static float Float100 { get; } = 100;

    /// <summary>
    /// Gets the ulong value 100 used by date/time mapper tests.
    /// </summary>
    public static ulong ULong100 { get; } = 100;

    /// <summary>
    /// Gets a counting values array sample matching collection mapper tests.
    /// </summary>
    public static CountingValues[] CountingValuesOneThreeArray { get; } = [CountingValues.One, CountingValues.Three];

    /// <summary>
    /// Gets an enumerable sample matching collection mapper tests.
    /// </summary>
    public static IEnumerable<CountingValues> CountingValuesOneThreeEnumerable { get; } = CountingValuesOneThreeArray;

    /// <summary>
    /// Gets a list sample matching collection mapper tests.
    /// </summary>
    public static List<CountingValues> CountingValuesOneThreeList { get; } = [CountingValues.One, CountingValues.Three];

    /// <summary>
    /// Gets an <see cref="IList{T}"/> sample matching collection mapper tests.
    /// </summary>
    public static IList<CountingValues> CountingValuesOneThreeIList { get; } = CountingValuesOneThreeList;

    /// <summary>
    /// Gets an <see cref="ICollection{T}"/> sample matching collection mapper tests.
    /// </summary>
    public static ICollection<CountingValues> CountingValuesOneThreeICollection { get; } = CountingValuesOneThreeList;

    /// <summary>
    /// Gets an <see cref="IReadOnlyCollection{T}"/> sample matching collection mapper tests.
    /// </summary>
    public static IReadOnlyCollection<CountingValues> CountingValuesOneThreeIReadOnlyCollection { get; } = CountingValuesOneThreeList;

    /// <summary>
    /// Gets a memory sample matching collection mapper tests.
    /// </summary>
    public static Memory<CountingValues> CountingValuesOneThreeMemory { get; } = CountingValuesOneThreeArray;

    /// <summary>
    /// Gets a read-only memory sample matching collection mapper tests.
    /// </summary>
    public static ReadOnlyMemory<CountingValues> CountingValuesOneThreeReadOnlyMemory { get; } = CountingValuesOneThreeArray;

    /// <summary>
    /// Gets a stack sample matching collection mapper tests.
    /// </summary>
    public static Stack<CountingValues> CountingValuesOneThreeStack { get; } = new([CountingValues.One, CountingValues.Three]);

    /// <summary>
    /// Gets a queue sample matching collection mapper tests.
    /// </summary>
    public static Queue<CountingValues> CountingValuesOneThreeQueue { get; } = new([CountingValues.One, CountingValues.Three]);

    /// <summary>
    /// Gets a blocking collection sample matching collection mapper tests.
    /// </summary>
    public static BlockingCollection<CountingValues> CountingValuesOneThreeBlockingCollection { get; } = [CountingValues.One, CountingValues.Three];

    /// <summary>
    /// Gets a concurrent bag sample matching collection mapper tests.
    /// </summary>
    public static ConcurrentBag<CountingValues> CountingValuesOneThreeConcurrentBag { get; } = [CountingValues.One, CountingValues.Three];

    /// <summary>
    /// Gets a concurrent queue sample matching collection mapper tests.
    /// </summary>
    public static ConcurrentQueue<CountingValues> CountingValuesOneThreeConcurrentQueue { get; } = new([CountingValues.One, CountingValues.Three]);

    /// <summary>
    /// Gets a concurrent stack sample matching collection mapper tests.
    /// </summary>
    public static ConcurrentStack<CountingValues> CountingValuesOneThreeConcurrentStack { get; } = new([CountingValues.One, CountingValues.Three]);

    /// <summary>
    /// Gets an <see cref="IProducerConsumerCollection{T}"/> sample matching collection mapper tests.
    /// </summary>
    public static IProducerConsumerCollection<CountingValues> CountingValuesOneThreeIProducerConsumerCollection { get; } =
        new ConcurrentBag<CountingValues>(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom non-generic enumerable sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIEnumerableOfCountingValues CustomIEnumerableOfCountingValuesOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom generic enumerable sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIEnumerable<CountingValues> CustomIEnumerableOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom non-generic list sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIListOfCountingValues CustomIListOfCountingValuesOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom generic list sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIList<CountingValues> CustomIListOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom non-generic collection sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingICollectionOfCountingValues CustomICollectionOfCountingValuesOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom generic collection sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingICollection<CountingValues> CustomICollectionOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom non-generic read-only collection sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIReadOnlyCollectionOfCountingValues CustomIReadOnlyCollectionOfCountingValuesOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets a custom generic read-only collection sample matching collection mapper tests.
    /// </summary>
    public static CustomCollectionImplementingIReadOnlyCollection<CountingValues> CustomIReadOnlyCollectionOneThree { get; } =
        new(CountingValuesOneThreeArray);

    /// <summary>
    /// Gets the UTC date used by polymorphism mapper tests.
    /// </summary>
    public static DateTime PolymorphismUtcDateTime { get; } =
        new(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    /// <summary>
    /// Gets the string numbers used by polymorphism third-class tests.
    /// </summary>
    public static string[] PolymorphismThirdClassNumbers { get; } =
        ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    /// <summary>
    /// Gets the string numbers used by polymorphic method map nested-property tests.
    /// </summary>
    public static string[] PolymorphismNestedThirdClassNumbers { get; } = ["7", "8", "9"];

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceFirstClass"/> sample for polymorphism tests.
    /// </summary>
    public static PolymorphismOne.SourceFirstClass PolymorphismOneSourceFirstClass { get; } = new()
    {
        NumericProperty = 17,
        DateTimeProperty = PolymorphismUtcDateTime,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceSecondClass"/> sample for polymorphism tests.
    /// </summary>
    public static PolymorphismOne.SourceSecondClass PolymorphismOneSourceSecondClass { get; } = new()
    {
        NumericProperty = 17,
        GuidProperty = SampleGuid,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceThirdClass"/> sample for polymorphism tests.
    /// </summary>
    public static PolymorphismOne.SourceThirdClass PolymorphismOneSourceThirdClass { get; } = new()
    {
        NumericProperty = 17,
        GuidProperty = SampleGuid,
        Numbers = PolymorphismThirdClassNumbers,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceBaseClass"/> sample for polymorphism tests.
    /// </summary>
    public static PolymorphismOne.SourceBaseClass PolymorphismOneSourceBaseClass { get; } = new()
    {
        NumericProperty = 17,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceWithDependency"/> sample for polymorphic method map tests.
    /// </summary>
    public static PolymorphismOne.SourceWithDependency PolymorphismOneSourceWithDependency { get; } = new()
    {
        NumericProperty = 125,
        NestedProperty = new PolymorphismOne.SourceThirdClass
        {
            NumericProperty = 456,
            GuidProperty = SampleGuid,
            Numbers = PolymorphismNestedThirdClassNumbers,
        },
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismOne.SourceWithDependencyWithSourceBaseClass"/> sample for polymorphic method map tests.
    /// </summary>
    public static PolymorphismOne.SourceWithDependencyWithSourceBaseClass PolymorphismOneSourceWithDependencyWithSourceBaseClass { get; } = new()
    {
        NumericProperty = 125,
        NestedProperty = new PolymorphismOne.SourceBaseClass
        {
            NumericProperty = 456,
        },
    };

    /// <summary>
    /// Gets a <see cref="MappaContext"/> assigning polymorphism numeric property to 2025.
    /// </summary>
    public static MappaContext PolymorphismNumericPropertyContext2025 { get; } = new()
    {
        [nameof(PolymorphismOne.TargetBaseClass.NumericProperty)] = 2025L,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismTwo.SourceFirstClass"/> sample for interface polymorphism tests.
    /// </summary>
    public static PolymorphismTwo.SourceFirstClass PolymorphismTwoSourceFirstClass { get; } = new()
    {
        NumericProperty = 17,
        DateTimeProperty = PolymorphismUtcDateTime,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismTwo.SourceSecondClass"/> sample for interface polymorphism tests.
    /// </summary>
    public static PolymorphismTwo.SourceSecondClass PolymorphismTwoSourceSecondClass { get; } = new()
    {
        NumericProperty = 17,
        GuidProperty = SampleGuid,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismTwo.SourceThirdClass"/> sample for interface polymorphism tests.
    /// </summary>
    public static PolymorphismTwo.SourceThirdClass PolymorphismTwoSourceThirdClass { get; } = new()
    {
        NumericProperty = 17,
        GuidProperty = SampleGuid,
        Numbers = PolymorphismThirdClassNumbers,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismThree.SourceFirstClass"/> sample for identity polymorphism tests.
    /// </summary>
    public static PolymorphismThree.SourceFirstClass PolymorphismThreeSourceFirstClass { get; } = new()
    {
        BaseProperty = 17,
        DerivedProperty = PolymorphismUtcDateTime,
    };

    /// <summary>
    /// Gets a <see cref="PolymorphismThree.SourceSecondClass"/> sample for identity polymorphism tests.
    /// </summary>
    public static PolymorphismThree.SourceSecondClass PolymorphismThreeSourceSecondClass { get; } = new()
    {
        BaseProperty = 17,
        DerivedProperty = PolymorphismUtcDateTime.ToString(CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// Gets a fixed <see cref="TimeOnly"/> for string-to-system-entity mapping tests.
    /// </summary>
    public static TimeOnly StringToSystemEntitiesTimeOnly { get; } = TimeOnly.FromDateTime(UtcDateTime);

    /// <summary>
    /// Gets a fixed <see cref="TimeSpan"/> for string-to-system-entity mapping tests.
    /// </summary>
    public static TimeSpan StringToSystemEntitiesTimeSpan { get; } =
        UtcDateTime - UtcDateTime.AddHours(7).AddMinutes(13).AddSeconds(17);

    /// <summary>
    /// Gets the default-culture date-time string for <see cref="UtcDateTime"/>.
    /// </summary>
    public static string StringToSystemEntitiesDateTimeInput { get; } =
        UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff", DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the default-culture date-time offset string for <see cref="UtcDateTimeOffset"/>.
    /// </summary>
    public static string StringToSystemEntitiesDateTimeOffsetInput { get; } =
        UtcDateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the default-culture time-span string for <see cref="StringToSystemEntitiesTimeSpan"/>.
    /// </summary>
    public static string StringToSystemEntitiesTimeSpanInput { get; } =
        StringToSystemEntitiesTimeSpan.ToString();

    /// <summary>
    /// Gets the default-culture time-only string for <see cref="StringToSystemEntitiesTimeOnly"/>.
    /// </summary>
    public static string StringToSystemEntitiesTimeOnlyInput { get; } =
        StringToSystemEntitiesTimeOnly.ToString("HH:mm:ss.fffffff", DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the default-culture date-only string for <see cref="SampleDateOnly"/>.
    /// </summary>
    public static string StringToSystemEntitiesDateOnlyInput { get; } =
        SampleDateOnly.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the N-format guid string for <see cref="SampleGuid"/>.
    /// </summary>
    public static string StringToSystemEntitiesGuidInput { get; } =
        SampleGuid.ToString("N", DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the sample URI string for string-to-system-entity mapping tests.
    /// </summary>
    public static string StringToSystemEntitiesUriInput { get; } = "https://github.com/sanelli/Mappa";

    /// <summary>
    /// Gets the settings-based date-time string for <see cref="UtcDateTime"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsDateTimeInput { get; } =
        UtcDateTime.ToString(StringToSystemEntitiesSettings.DateTimeFormat, DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the settings-based date-time offset string for <see cref="UtcDateTimeOffset"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsDateTimeOffsetInput { get; } =
        UtcDateTimeOffset.ToString(StringToSystemEntitiesSettings.DateTimeOffsetFormat, DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the settings-based time-span string for <see cref="StringToSystemEntitiesTimeSpan"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsTimeSpanInput { get; } =
        StringToSystemEntitiesTimeSpan.ToString(StringToSystemEntitiesSettings.TimeSpanFormat, CultureInfo.CurrentCulture);

    /// <summary>
    /// Gets the settings-based time-only string for <see cref="StringToSystemEntitiesTimeOnly"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsTimeOnlyInput { get; } =
        StringToSystemEntitiesTimeOnly.ToString(StringToSystemEntitiesSettings.TimeOnlyFormat, DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the settings-based date-only string for <see cref="SampleDateOnly"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsDateOnlyInput { get; } =
        SampleDateOnly.ToString(StringToSystemEntitiesSettings.DateOnlyFormat, DateTimeFormatInfo.InvariantInfo);

    /// <summary>
    /// Gets the settings-based guid string for <see cref="SampleGuid"/>.
    /// </summary>
    public static string StringToSystemEntitiesSettingsGuidInput { get; } =
        SampleGuid.ToString(StringToSystemEntitiesSettings.GuidFormat, DateTimeFormatInfo.InvariantInfo);

    private sealed class DerivedInterfaceSampleModel : IDerivedInterfaceModel
    {
        public long LongProperty { get; set; } = 124;

        public double DoubleProperty { get; set; } = 12.34;
    }
}