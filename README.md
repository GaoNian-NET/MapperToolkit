# MapperToolkit
![Nuget](https://img.shields.io/badge/nuget-1.0.1.7--alpha-blue)
![GitHub](https://img.shields.io/badge/license-Apache--2.0-green)
This project is a Mapper library use  Source Generator with function such as:
- mapper: object-object mapper .
- transoform:by object (entity) build specified model (DTO) .

MapperToolkit is the fastest .NET object mapper
if member inclube List IEnumerable 
surpassing even the manual mapping and Mapperly
The benchmark was generated with(https://github.com/GaoNian-NET/Benchmark.netCoreMappers).

|        Method |       Mean |    Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
|-------------- |-----------:|---------:|----------:|-------:|-------:|----------:|
|   AgileMapper | 2,832.5 ns | 56.29 ns |  80.73 ns | 0.5035 | 0.0038 |      3 KB |
|               |            |          |           |        |        |           |
|    TinyMapper | 3,195.5 ns | 62.88 ns | 113.39 ns | 0.3853 |      - |      2 KB |
|               |            |          |           |        |        |           |
| ExpressMapper | 2,957.5 ns | 84.74 ns | 248.54 ns | 0.8049 | 0.0038 |      5 KB |
|               |            |          |           |        |        |           |
|    AutoMapper | 1,497.6 ns | 33.87 ns |  98.79 ns | 0.3452 | 0.0019 |      2 KB |
|               |            |          |           |        |        |           |
| ManualMapping |   459.0 ns | 14.02 ns |  41.13 ns | 0.2103 | 0.0005 |      1 KB |
|               |            |          |           |        |        |           |
|       Mapster |   465.3 ns | 13.84 ns |  40.60 ns | 0.3285 | 0.0019 |      2 KB |
|               |            |          |           |        |        |           |
|      Mapperly |   484.8 ns |  9.02 ns |  16.71 ns | 0.2613 | 0.0010 |      2 KB |
|               |            |          |           |        |        |           |
| MapperToolkit |   303.2 ns |  6.11 ns |   5.10 ns | 0.1884 | 0.0005 |      1 KB |
## Quickstart
### Installation

Add the NuGet Package to your project:
```bash
dotnet add package MapperToolkit
```

### Create your first mapper
```c#
//example Entity
public class Entity
{
    public string Account { get; set; }
    public DateTime CreateTime { get; set; }
}

//example DTO
public class DTO
{
    public string EmpNo { get; set; }
    public string EmpName { get; set; }
    public DateTime CreateTime { get; set; }
}
```
Create profile class inherit MapperToolkit.Profile
in constructors type mapper code 
```c#
// Profile declaration
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        GenerateAllMapper<Entity, DTO>()
        .Map(src => src.Account[0..4],dest=>dest.EmpNo)
        .Map(src => src.Account[4..^0],dest=>dest.EmpName);
    }
}

```
```c#
//Generator code
 public static partial class DTOMapper
{
    public static global::ConsoleApp.DTO MapperToDTO(this global::ConsoleApp.Entity source)
    {
        global::ConsoleApp4.DTO result = new();
        global::System.Func<global::ConsoleApp.Entity, string> EmpNoMapper = src => src.Account[0..4];
        global::System.Func<global::ConsoleApp.Entity, string> EmpNameMapper = src => src.Account[4..^0];
        result.EmpNo = EmpNoMapper.Invoke(source);
        result.EmpName = EmpNameMapper.Invoke(source);
        result.CreateTime = source.CreateTime;
        return result;
    }
}
```
```c#
//Mapper usage
var entity = new Entity() { Account = "A123Bojack" ,CreateTime = DateTime.Now };
var dto = entity.MapperToDTO();
Console.WriteLine($"EmpNo:{dto.EmpNo};EmpName:{dto.EmpName};CreateTime:{dto.CreateTime}");
// print EmpNo:A123;EmpName:Bojack;CreateTime:2022/8/16 12:13:15
```
