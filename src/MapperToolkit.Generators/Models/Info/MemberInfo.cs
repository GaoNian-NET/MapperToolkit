namespace MapperToolkit.SourceGenerators.Models.Info;

/// <summary>
/// 成员信息
/// </summary>
internal struct MemberInfo
{
    /// <summary>
    /// 可访问性类别
    /// </summary>
    internal Accessibility Accessibility;
    /// <summary>
    /// 类型名称
    /// </summary>
    internal TypeSymbolInfo Type;
    /// <summary>
    /// 成员名称
    /// </summary>
    internal string MemberName;
    /// <summary>
    /// 是否为默认
    /// </summary>
    internal bool IsDefault;

}
