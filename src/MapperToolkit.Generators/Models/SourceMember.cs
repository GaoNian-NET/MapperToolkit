namespace MapperToolkit.Generators.Models;
/// <summary>
/// 生成的目标成员
/// </summary>
internal record struct SourceMember
{
    /// <summary>
    /// 所声明的访问修饰符
    /// </summary>
    internal Accessibility DeclaredAccessibility;
    /// <summary>
    /// 类型名称
    /// </summary>
    internal TypeSymbolInfo Type;
    /// <summary>
    /// 成员名称
    /// </summary>
    internal string MemberName;
    /// <summary>
    /// 源成员名称
    /// </summary>
    internal string BaseMemberName;
    /// <summary>
    /// 是否产生映射
    /// </summary>
    internal bool NeedMapper;

    internal List<(string AttributeName,

        SeparatedSyntaxList<ArgumentSyntax>? Arguments,
         SeparatedSyntaxList<ExpressionSyntax>? ArgumentExpressions)> AttributeConfigs;


}
