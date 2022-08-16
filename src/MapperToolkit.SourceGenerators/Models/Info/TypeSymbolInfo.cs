namespace MapperToolkit.SourceGenerators.Models.Info;




internal struct TypeSymbolInfo
{
    public string FullyQualifiedName;
    public string Name;
    public string OriginalFullyQualifiedName;
    public TypeSymbolInfoKind TypeKind;
    public string? LengthExpression;
    public string? IndexExpression;



    public TypeSymbolInfo(string name)
    {
        Name = name;
        FullyQualifiedName = name;
        OriginalFullyQualifiedName = name;
        TypeKind = TypeSymbolInfoKind.Default;
        LengthExpression = null;
        IndexExpression = null;
    }
}
