namespace MapperToolkit.Generators.Models;

internal struct SourceObejct
{
    internal List<SourceMember> Members { get; set; }
    internal List<SubMapperInfo> SubMappers { get; set; }
    internal List<MapperInfo> Mappers { get; set; }
    //internal Dictionary<string,ExpressionSyntax> Mappers { get; set; }
    internal Dictionary<string, MemberInfo> SourceMembersHashMap { get; set; }
    internal Dictionary<string, MemberInfo>? DestinationMembersHashMap { get; set; }
    internal Accessibility DeclaredAccessibility { get; set; }
    internal TypeKind Type { get; set; }
    internal TypeSymbolInfo SourceType { get; set; }


    internal TypeSymbolInfo DestinationType { get; set; }


    internal string Namespace { get; set; }
    internal SyntaxList<UsingDirectiveSyntax> Usings { get; set; }
    internal FunctionType FunctionType { get; set; }
    internal bool NeedIMplicitConversion { get; set; }

}
