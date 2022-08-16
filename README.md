# MapperToolkit
![Nuget](https://img.shields.io/badge/nuget-1.0.1.6--alpha-blue)
![GitHub](https://img.shields.io/badge/license-Apache--2.0-green)


a source generator for object-object mapper
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
    public static global::ConsoleApp.DTO MapperToDTO(this global::ConsoleApp4.Entity source)
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
