namespace MapperToolkit;
#pragma warning disable IDE0060
public class Profile
{
    public static ICustomMapperConfiguration<TSource, TDestination> GenerateCustomMapper<TSource, TDestination>() => default!;

    public static IAllMapperConfiguration<TSource, TDestination> GenerateAllMapper<TSource, TDestination>() => default!;
    public static IAllTransformConfiguration<TSource> GenerateAllTransoform<TSource>(string namespaceDeclaration, string declaration) => default!;
    public static ICustomTransformConfiguration<TSource> GenerateCustomTransoform<TSource>(string namespaceDeclaration, string declaration) => default!;
}
#pragma warning restore