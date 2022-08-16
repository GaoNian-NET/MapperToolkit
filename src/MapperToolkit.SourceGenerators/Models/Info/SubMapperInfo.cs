namespace MapperToolkit.SourceGenerators.Models.Info
{
    internal struct SubMapperInfo
    {
        public string MapperExpression;
        internal MemberInfo SourceMember;
        internal MemberInfo DestinationMember;
        internal TypeSymbolInfoKind Kind;
    }
}
