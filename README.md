# MapperToolkit
![Nuget](https://img.shields.io/badge/nuget-1.0.1.7--alpha-blue)
![GitHub](https://img.shields.io/badge/license-Apache--2.0-green)

MapperToolkit is the fastest .NET object mapper
if member inclube List<T> IEnumerable<T> 
surpassing even the manual mapping and Mapperly
The benchmark was generated with(https://github.com/GaoNian-NET/Benchmark.netCoreMappers).
|        Method |       Mean |    Error |    StdDev |     Median |  Gen 0 |  Gen 1 | Allocated |
|-------------- |-----------:|---------:|----------:|-----------:|-------:|-------:|----------:|
|   AgileMapper | 2,510.8 ns | 40.22 ns |  37.62 ns | 2,519.2 ns | 0.5035 | 0.0038 |   3,160 B |
|               |            |          |           |            |        |        |           |
|    TinyMapper | 2,722.7 ns | 60.70 ns | 178.97 ns | 2,695.0 ns | 0.3433 |      - |   2,160 B |
|               |            |          |           |            |        |        |           |
| ExpressMapper | 2,409.4 ns | 29.58 ns |  26.22 ns | 2,411.2 ns | 0.7782 | 0.0038 |   4,904 B |
|               |            |          |           |            |        |        |           |
|    AutoMapper | 1,202.7 ns | 24.01 ns |  46.82 ns | 1,210.4 ns | 0.3033 |      - |   1,904 B |
|               |            |          |           |            |        |        |           |
| ManualMapping |   372.1 ns | 10.31 ns |  30.09 ns |   364.1 ns | 0.1845 | 0.0005 |   1,160 B |
|               |            |          |           |            |        |        |           |
|       Mapster |   369.5 ns | 12.91 ns |  36.00 ns |   360.0 ns | 0.3033 | 0.0014 |   1,904 B |
|               |            |          |           |            |        |        |           |
|      Mapperly |   229.6 ns |  6.14 ns |  17.91 ns |   228.7 ns | 0.1464 |      - |     920 B |
|               |            |          |           |            |        |        |           |
| MapperToolkit |   196.9 ns |  2.58 ns |   3.53 ns |   197.2 ns | 0.1466 | 0.0002 |     920 B |
   

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
