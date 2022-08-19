namespace MapperToolkit.Generators.Models;
internal enum TypeSymbolInfoKind
{
    Default,
    Array,
    Enumerable,
    Enumerator,
    Object
}
internal enum FunctionType
{
    Transoform,
    Mapper

}

internal enum GenerateType
{
    All,
    Custom,
}


internal enum MethoType
{
    /// <summary>
    /// 通过定义映射器生成
    /// </summary>
    Map,
    /// <summary>
    /// 忽略, 不生成
    /// </summary>
    Ignore,
    /// <summary>
    /// 默认生成
    /// </summary>
    Generate,
    /// <summary>
    /// 构造
    /// </summary>
    IMplicitConversion

}