using Microsoft.CodeAnalysis.Text;

namespace MapperToolkit.SourceGenerators.Models.Info;

/// <summary>
/// 映射器基本信息
/// </summary>
internal struct MapperInfo
{
    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName;
    /// <summary>
    /// 成员名称
    /// </summary>
    public string MemberName;
    /// <summary>
    /// 内部表达式
    /// </summary>
    public SourceText Expression;


}
