namespace MapperToolkit.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public partial class MapperSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

#if DEBUG
        System.Diagnostics.Debugger.Launch();
#endif
        IncrementalValuesProvider<(ITypeSymbol Symbol, SemanticModel)>
            classSymbols = context.SyntaxProvider.CreateSyntaxProvider
            ((static (node, _)
            => node is ClassDeclarationSyntax { BaseList: not null }),
            (context, _)
            => ((ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!, context.SemanticModel));



        IncrementalValuesProvider<(ITypeSymbol Symbol, SemanticModel SemanticModel)>
            filterClassSymbols = classSymbols.Where((item)
            => item.Symbol.BaseType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::MapperToolkit.Profile");


        IncrementalValuesProvider<(IMethodSymbol Symbol, SemanticModel SemanticModel)> methoSymbols

            = filterClassSymbols.Select((item, _)
            => ((IMethodSymbol)item.Symbol.GetMembers()
                    .First(member => member is IMethodSymbol { MethodKind: MethodKind.Constructor, Parameters.Length: 0 })
            , item.SemanticModel));


        context.RegisterSourceOutput(methoSymbols, (context, item) =>
        {
            var constructorDeclaration = item.Symbol.DeclaringSyntaxReferences.FirstOrDefaultSyntax<ConstructorDeclarationSyntax>();
            if (constructorDeclaration is null or { Body: null })
            {
                return;
            }


            var expressionStatementSyntaxs = constructorDeclaration.Body.Statements.Filter<ExpressionStatementSyntax>();



            foreach (var expressionStatementSyntax in expressionStatementSyntaxs)
            {
                var methoInfos = expressionStatementSyntax.Expression.MethoInfoEnumerator(item.SemanticModel);

                var baseInfo = expressionStatementSyntax.Expression.GetBaseInfo();
                var (filename, code) = CreateCode(methoInfos, baseInfo);
                context.AddSource(filename, code);
            }
        });
    }
}
