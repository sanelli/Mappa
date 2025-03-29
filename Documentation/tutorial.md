# Tutorial
This tutorial shows how to use the Mappa source generator.

## Setting up the project
First of all let's create a new project:
```powershell
dotnet new console --name MappaTutorial
```

and let's add the required libraries:
```powershell
dotnet add MappaTutorial package Mappa
dotnet add MappaTutorial package Mappa.Generator
```

Optionally, you can edit your `MappaTutorial.csproj` file to emit the source generated files by adding the following property:
```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
```

## Define some classes
Let's add a couple of new classes to the project.

First lets create the source class in `Source.cs`
```csharp
using Mappa;
using System.Collections.Generic;

namespace MappaTutorial;

public sealed class Source
{
    public int PropertyOne {get; set;}
    public IEnumerable<int> PropertyTwo {get; set;}
    public StringSplitOptions PropertyThree {get; set;}
}
```

Then let's create the target class.
```csharp
using Mappa;
using System.Collections.Generic;

namespace MappaTutorial;

public sealed class Target
{
    public long PropertyOne {get; set;}
    public IEnumerable<string> PropertyTwo {get; set;}
    public string PropertyThree {get; set;}
}
```

## Define the mapper
Now that we have a source and a target class we can define the mapper in `Mapper.cs`:
```csharp
using Mappa;

namespace MappaTutorial;

[Mappa]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

We defined a mapper using a standard class but we could have as well define a static class with extension methods like the following in `SourceExtensions.cs`:
```csharp
using Mappa;

namespace MappaTutorial;

[Mappa]
public static partial class SourceExtensions
{
    public static partial Target ToTarget(this Source input);
}
```

## Compile and run the project
Let's update the main to use the mapper:
```csharp
using MappaTutorial;

Source source = new()
{
    PropertyOne = 45,
    PropertyTwo = ["4", "77", "987"],
    PropertyThree = StringSplitOptions.RemoveEmptyEntries,
}

var mapper = new Mapper();
var target1 = mapper.Map(source);
System.Console.WriteLine(target1.PropertyOne);
foreach(var item in target1.PropertyTwo)
{
    System.Console.WriteLine(item);
}
System.Console.WriteLine(target1.PropertyThree);

var target2 = source.ToTarget();
System.Console.WriteLine(target2.PropertyOne);
foreach(var item in target2.PropertyTwo)
{
    System.Console.WriteLine(item);
}
System.Console.WriteLine(target2.PropertyThree);
```

We can now run the project
```powershell
dotnet run --project MappaTutorial
```

and on the console you will see the following as expected:
```
45
4
77
987
RemoveEmptyEntries
45
4
77
987
RemoveEmptyEntries
```

## Advanced topics
### MappaSettings format
⚠️ TODO

### OptionalProtobuf
⚠️ TODO

### MappaInvokeMethod attribute
⚠️ TODO

### MappaUseProperty attribute
⚠️ TODO

### MappaAssignFromContext attribute
⚠️ TODO

### Get-only collection properties
⚠️ TODO

### MappaSettings ProtobufOptional
⚠️ TODO

### Mappa dependency
⚠️ TODO

#### Protobuf and Bson dependency
⚠️ TODO