namespace MapperToolkit;
#pragma warning disable IDE0060
public class Profile
{
    /// <summary>
    ///  Generate  mapper only create explicit mapper 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static ICustomMapperConfiguration<TSource, TDestination> GenerateCustomMapper<TSource, TDestination>() => default!;
    /// <summary>
    /// Generate  mapper with IMplicit Conversion
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static IAllMapperConfiguration<TSource, TDestination> GenerateAllMapper<TSource, TDestination>() => default!;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="namespaceDeclaration"></param>
    /// <param name="declaration"></param>
    /// <returns></returns>
    public static IAllTransformConfiguration<TSource> GenerateAllTransoform<TSource>(string namespaceDeclaration, string declaration) => default!;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="namespaceDeclaration"></param>
    /// <param name="declaration"></param>
    /// <returns></returns>
    public static ICustomTransformConfiguration<TSource> GenerateCustomTransoform<TSource>(string namespaceDeclaration, string declaration) => default!;
}
#pragma warning restore