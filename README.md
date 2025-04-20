# 🗺️ Mappa

![Build main](https://github.com/sanelli/Mappa/actions/workflows/merge-main.yml/badge.svg)
![Line Coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fgist.githubusercontent.com%2Fsanelli%2F7f4a85bc809328b4821b03125f9190cb%2Fraw%2FMAPPA-BADGE-LINE-COVERAGE.json)
![Branch Coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fgist.githubusercontent.com%2Fsanelli%2F7f4a85bc809328b4821b03125f9190cb%2Fraw%2FMAPPA-BADGE-BRANCH-COVERAGE.json)
![Method Coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fgist.githubusercontent.com%2Fsanelli%2F7f4a85bc809328b4821b03125f9190cb%2Fraw%2FMAPPA-BADGE-METHOD-COVERAGE.json)

Mappa (italian for _map_) is a source generator for `C#` that can generate code to allow the mapping between types, similarly to what [AutoMapper](https://www.nuget.org/packages/AutoMapper) (and other similar tools) does.
See [Documentation](./Documentation/README.md) for more details.

The main different between Mappa and AutoMapper is that Mappa generates code at compile time while AutoMapper only at runtime;
this has multiple benefits:
- the code generated is optimised by the compiler;
- the code generated is pure C# code that does not require any introspections;
- the code generated works when AOT is required;
- the code can be easily shared across different mappers;
- fine-grained mapping can be obtained via attributes on the mapper methods without having to touch the source or the target classes;
- mapper methods can be inside any class (static class and extension methods are supported!).
- you do not need to specify every type that requires a mapping: if a mapping is missing Mappa will generate it for you;


## 🧑‍🔬 A simple example
Consider the following code snipped:
```csharp
using Mappa;
using System.Collections.Generic;

namespace Sample;
  
public enum MyEnumeration
{
    One,
    Two,
    Three,
}

public class Source
{
    public int PropertyA {get;set;}
    public int[] PropertyB {get;set;}
    public MyEnumeration PropertyC {get;set;}
}

public class Target
{
    public long PropertyA {get;set;}
    public List<string> PropertyB {get;set;}
    public string PropertyC {get;set;}
}

[Mappa]
public partial class Mapper
{
    public partial Target Map(Source input);
}
```

will create mapping code like the following:
```csharp
namespace Sample;

public partial class Mapper
{
   [System.CodeDom.Compiler.GeneratedCodeAttribute("Mappa", "0.0.1.0")]
   public partial Sample.Target Map(Sample.Source input)
   {
       long __mappa_tmp_1 = input.PropertyA;
       int[] __mappa_tmp_2 = input.PropertyB;
       System.Collections.Generic.List<string> __mappa_tmp_3 = new System.Collections.Generic.List<string>(__mappa_tmp_2.Length);
       for (int __mappa_tmp_3 = 0; __mappa_tmp_3 < __mappa_tmp_2.Length; ++__mappa_tmp_3)
       {
           int __mappa_tmp_4 = __mappa_tmp_2[__mappa_tmp_3];
           string __mappa_tmp_5 = __mappa_tmp_4.ToString();
           __mappa_tmp_3.Add(__mappa_tmp_5)
       }
       Sample.MyEnumeration __mappa_tmp_6 = input.PropertyC;
       string __mappa_tmp_7;
       switch(__mappa_tmp_6)
       {
           case Sample.MyEnumeration.One:
              __mappa_tmp_7 = "One";
              break;
           case Sample.MyEnumeration.Two:
              __mappa_tmp_7 = "Two";
              break;
           case Sample.MyEnumeration.Three:
              __mappa_tmp_7 = "Three";
              break;
           default:
              throw new System.OutOfRangeException(nameof(__mappa_tmp_6));
       }
       Sample.Target __mappa_tmp_8 = new Sample.Target()
       {
           PropertyA = __mappa_tmp_1,
           PropertyB = __mappa_tmp_3,
           PropertyC = __mappa_tmp_7;
       }
       return __mappa_tmp_8;
   }
}
```

## 🧑🏻‍💻 How to use Mappa
The easiest way to is import the NuGet packages and apply the `[Mappa]` attribute on the partial classes that contains the partial methods that needs to be generated.

Please see the [tutorial](./Documentation/tutorial.md) in the [documentation](./Documentation/README.md) provided.

You can also find many examples in the [Mappa.Samples](Mappa.Samples) project.

## 📦 NuGet packages
- [Mappa](https://www.nuget.org/packages/Mappa/): source generator that allows to automatically generate mapping between classes and value types;
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that allows to automatically generate mapping between classes and value types;
- [Mappa Protobuf](https://www.nuget.org/packages/Mappa.Dependency.Protobuf/): methods to map `Google.Protobuf.WellKnownTypes` objects from [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) package into common objects.
- [Mappa Protobuf dependency](https://www.nuget.org/packages/Mappa.Dependency.Protobuf.DependencyInjection/): utility methods to register the Protobuf mapper.
- [Mappa Bson](https://www.nuget.org/packages/Mappa.Dependency.Bson/): methods to map `MongoDB.Bson` objects from [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) package into common objects.
- [Mappa Bson dependency](https://www.nuget.org/packages/Mappa.Dependency.Bson.DependencyInjection/): utility methods to register the Bson mapper.

# 📈 Code coverage history 
- [Code Coverage gist](https://gist.github.com/sanelli/7f4a85bc809328b4821b03125f9190cb)
- [Code Coverage history](https://gist.github.com/sanelli/7f4a85bc809328b4821b03125f9190cb#file-mappa-code-coverage-history-md)

<img alt="Code coverage history" src="https://gist.githubusercontent.com/sanelli/7f4a85bc809328b4821b03125f9190cb/raw/MAPPA-CODE-COVERAGE-HISTORY.svg" width="800" height="500"/>