using System.Diagnostics;

namespace MapperToolkit.SourceGenerators.Extensions;

internal static class MapperToolkitExtension
{
    internal static (TypeSymbolInfoKind Kind, ITypeSymbol TypeSymbol) GetTypeSymbolInfoKind(this ITypeSymbol symbol) =>
       symbol switch
       {
           IArrayTypeSymbol { ElementType.IsValueType: false, MetadataName: not "String" } arrayTypeSymbol => (TypeSymbolInfoKind.Array, arrayTypeSymbol.ElementType),

           INamedTypeSymbol { Arity: > 0 } namedTypeSymbol => CheckIEnumerator(namedTypeSymbol),

           INamedTypeSymbol { Arity: 0, IsValueType: false, MetadataName: not "String" } => (TypeSymbolInfoKind.Object, symbol),
           _ => (TypeSymbolInfoKind.Default, symbol),
       };


    /// <summary>
    /// 检查是否为迭代器接口
    /// </summary>
    /// <param name="namedTypeSymbol"></param>
    /// <returns></returns>
    private static (TypeSymbolInfoKind, ITypeSymbol) CheckIEnumerator(INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            is "global::System.Collections.Generic.IEnumerator<T>"
            or "global::System.Collections.Generic.IEnumerable<T>"
            or "global::System.Collections.Generic.IAsyncEnumerable<T>")
        {
            return (TypeSymbolInfoKind.Enumerator, namedTypeSymbol.TypeArguments.First());
        }
        else if (namedTypeSymbol.Interfaces.FirstOrDefault(x => x.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) is "global::System.Collections.Generic.IEnumerable<T>") is not null
            && namedTypeSymbol.TypeArguments.FirstOrDefault()?.IsValueType == false)
        {
            return (TypeSymbolInfoKind.Enumerable, namedTypeSymbol.TypeArguments.First());

        }
        return (TypeSymbolInfoKind.Default, namedTypeSymbol);

    }

    public static string? GetIndexExpression(this ITypeSymbol symbol, TypeSymbolInfoKind kind) => kind switch
    {
        TypeSymbolInfoKind.Array => "[i]",
        TypeSymbolInfoKind.Enumerator => ".ElementAt(i)",
        TypeSymbolInfoKind.Enumerable => symbol.GetMembers().FirstOrDefault(member => member is IPropertySymbol
        {
            Name: "this[int index]"
        }) is not null ? "[i]" : ".ElementAt(i)",
        _ => null
    };
    public static string? GetLengthExpression(this ITypeSymbol symbol, TypeSymbolInfoKind kind) => kind switch
    {
        TypeSymbolInfoKind.Array => "Length",
        TypeSymbolInfoKind.Enumerator => "Count()",
        TypeSymbolInfoKind.Enumerable => symbol.GetMembers().FirstOrDefault(member => member is IPropertySymbol
        {
            Name: "Count" or "Length", Type.SpecialType: SpecialType.System_Int32
        })?.Name ?? "Count()",
        _ => null
    };


    internal static TypeSymbolInfo GetInfo(this ITypeSymbol symbol)
    {
        if (symbol.IsAnonymousType)
        {
            return new TypeSymbolInfo("dynamic");
        }

        var (Kind, TypeSymbol) = symbol.GetTypeSymbolInfoKind();
        return new()
        {
            OriginalFullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            FullyQualifiedName = TypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            Name = TypeSymbol.Name,
            TypeKind = Kind,
            LengthExpression = TypeSymbol.GetLengthExpression(Kind),
            IndexExpression = TypeSymbol.GetIndexExpression(Kind)

        };
    }



    internal static Dictionary<string, MemberInfo> GetMembersHashMap(this ITypeSymbol typeSymbol, Dictionary<string, MemberInfo> membersHashMap)
    {
        if (typeSymbol.BaseType is ITypeSymbol symbol)
        {
            var members = typeSymbol.GetMembers()
                .Filter<IPropertySymbol>(m => m.IsVisible() && m.IsAbstract == false);
            foreach (var member in members)
            {
                try
                {
                    membersHashMap.Add(member.Name, new MemberInfo()
                    {
                        IsDefault = true,
                        Accessibility = member.DeclaredAccessibility,
                        Type = member.Type.GetInfo(),
                        MemberName = member.Name,
                    });
                }
                catch (Exception ex)
                {

                    Debugger.Log(2, "GetMembersHashMap", ex.Message);
                }

            }
            GetMembersHashMap(typeSymbol.BaseType, membersHashMap);


        }
        return membersHashMap;
    }
    internal static FunctionType GetFunctionType(this IMethodSymbol symbol) =>
        symbol.OriginalDefinition.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
        {
            "global::MapperToolkit.ICustomTransformConfiguration<TSource>"
            or "global::MapperToolkit.IAllTransformConfiguration<TSource>"
            => FunctionType.Transoform,

            "global::MapperToolkit.IAllMapperConfiguration<TSource, TDestination>"
            or "global::MapperToolkit.ICustomMapperConfiguration<TSource, TDestination>" => FunctionType.Mapper,
            _ => throw new ArgumentException()
        };

    internal static GenerateType GetGenerateType(this IMethodSymbol symbol) =>
       symbol.OriginalDefinition.ReturnType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
       {

           "global::MapperToolkit.IAllTransformConfiguration<TSource>"
           or "global::MapperToolkit.IAllMapperConfiguration<TSource, TDestination>" => GenerateType.All,
           "global::MapperToolkit.ICustomTransformConfiguration<TSource>"
           or "global::MapperToolkit.ICustomMapperConfiguration<TSource, TDestination>" => GenerateType.Custom,
           _ => throw new ArgumentException(),
       };


    /// <summary>
    /// 获取所配置的方法类型
    /// 根据该类型来决定如何映射
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">不包含已经预定义的映射方式</exception>
    internal static MethoType GetMethoType(this IMethodSymbol symbol) =>
        symbol.Name switch
        {
            "GenerateAllTransoform" or "GenerateCustomTransoform" or "GenerateCustomMapper" or "GenerateAllMapper" => MethoType.Generate,
            "Map" => MethoType.Map,
            "Ignore" => MethoType.Ignore,
            "IMplicitConversion" => MethoType.IMplicitConversion,
            var other => throw new ArgumentOutOfRangeException($"[{other}]不是预定义的映射方式")
        };


    /// <summary>
    /// 筛选指定表达式
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    internal static ExpressionSyntax? GetExpression(ExpressionSyntax syntax) => (syntax, syntax.Kind()) switch
    {
        (_, SyntaxKind.InvocationExpression) => ((InvocationExpressionSyntax)syntax).Expression,
        (_, SyntaxKind.SimpleMemberAccessExpression) => ((MemberAccessExpressionSyntax)syntax).Expression,
        _ => null,
    };
    /// <summary>
    /// 递归获取全部表达式
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="expressionSyntaxes"></param>
    /// <returns></returns>
    private static IEnumerable<ExpressionSyntax> ExpressionEnumerable(ExpressionSyntax syntax, List<ExpressionSyntax> expressionSyntaxes)
    {
        var expression = GetExpression(syntax);
        if (expression is not null)
        {
            expressionSyntaxes.Add(expression);
            ExpressionEnumerable(expression, expressionSyntaxes);
        }

        return expressionSyntaxes;
    }

    internal static ObjectInfo GetBaseInfo<T>(this T syntax) where T : ExpressionSyntax
        => new()
        {
            BaseNamespace = syntax.GetParentSyntax<BaseNamespaceDeclarationSyntax>().Name.ToString(),
            Usings = syntax.GetParentSyntax<CompilationUnitSyntax>().Usings
        };


    /// <summary>
    /// 获取映射函数
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="semanticModel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static IEnumerable<MethoInfo> MethoInfoEnumerator<T>(this T syntax, SemanticModel semanticModel) where T : ExpressionSyntax
    {
        List<ExpressionSyntax> expressionSyntax = new() { syntax };

        ExpressionEnumerable(syntax, expressionSyntax);


        for (int i = expressionSyntax.Count; i > 0; i--)
        {
            if (expressionSyntax[i - 1] is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                if (semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol symbol)
                {
                    yield return new MethoInfo()
                    {
                        MethoSymbol = symbol,
                        TypeArgument = symbol.TypeArguments,
                        FunctionType = symbol.GetFunctionType(),
                        GenerateType = symbol.GetGenerateType(),
                        MethoType = symbol.GetMethoType(),
                        // TypeParameters = symbol.TypeParameters.Select(p => p.ToDisplayString()).ToImmutableArray(),

                        ParameterSymbols = symbol.Parameters,
                        ExpressionSyntaxes = invocationExpressionSyntax.ArgumentList.Arguments.Select(a => a.Expression).ToImmutableArray(),
                    };

                }


            }
        }
    }
}
