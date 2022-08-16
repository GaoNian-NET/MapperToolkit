namespace MapperToolkit.SourceGenerators.Models.Info;

/// <summary>
/// 方法信息
/// </summary>
internal record struct MethoInfo
{
    /// <summary>
    /// 方法特征标记
    /// </summary>
    internal IMethodSymbol MethoSymbol;
    internal ImmutableArray<ITypeSymbol> TypeArgument;
    /// <summary>
    /// 方法主体
    /// </summary>
    internal ImmutableArray<ExpressionSyntax> ExpressionSyntaxes;

    /// <summary>
    /// 参数
    /// </summary>
    internal ImmutableArray<IParameterSymbol> ParameterSymbols;

    /// <summary>
    /// 返回类型
    /// </summary>
    internal FunctionType? FunctionType;

    internal GenerateType GenerateType;
    /// <summary>
    /// 所配置的方法类型
    /// </summary>
    internal MethoType MethoType;
}
