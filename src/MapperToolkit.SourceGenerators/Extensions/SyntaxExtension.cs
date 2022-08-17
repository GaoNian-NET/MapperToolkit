namespace MapperToolkit.SourceGenerators.Extensions;

internal static class SyntaxExtension
{
    internal static List<SyntaxToken> GetChildToken(this IEnumerable<SyntaxNode> syntaxNodes
        , object value, List<SyntaxToken> syntaxTokens)
    {
        if (!syntaxNodes.Any())
        {
            return syntaxTokens;
        }

        for (var i = 0; i < syntaxNodes.Count(); i++)
        {
            if (syntaxNodes.ElementAt(i) is IdentifierNameSyntax identifierName && identifierName.Identifier.Value == value)
            {
                syntaxTokens.Add(identifierName.Identifier);
            }
        }

        var tmpNodes = syntaxNodes.Where(x => x.ChildNodes().Any()).SelectMany(x => x.ChildNodes());

        return GetChildToken(tmpNodes, value, syntaxTokens);


    }

    internal static T GetChildSyntax<T>(this SyntaxNode syntaxNode) where T : SyntaxNode
    {
        if (syntaxNode is T { } node)
        {
            return node;

        }
        else if (syntaxNode.ChildNodes().FirstOrDefault(c => c.ChildNodes().Any() || c is T) is SyntaxNode childNode)
        {
            // syntaxNode.ChildNodes().FirstOrDefault(c=>c is SyntaxNode)
            return GetChildSyntax<T>(childNode);
        }
        else
        {
            throw new ArgumentNullException(nameof(syntaxNode));
        }
    }
    internal static T GetParentSyntax<T>(this SyntaxNode? syntaxNode) where T : SyntaxNode
    {
        if (syntaxNode is T node)
        {
            return node;

        }
        else if (syntaxNode is not null)
        {
            return GetParentSyntax<T>(syntaxNode.Parent);
        }
        else
        {
            throw new ArgumentNullException(nameof(syntaxNode));
        }
    }



    /// <summary>
    /// 可访问性修饰符转换为<see cref="SyntaxKind"/>
    /// </summary>
    /// <param name="accessibility">可访问性修饰符</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal static SyntaxKind ConvertSyntaxKind(this Accessibility accessibility) => accessibility switch
    {
        Accessibility.Public => SyntaxKind.PublicKeyword,
        Accessibility.Private or Accessibility.NotApplicable => SyntaxKind.PrivateKeyword,
        Accessibility.Protected => SyntaxKind.ProtectedKeyword,
        Accessibility.Internal or Accessibility.ProtectedOrInternal or Accessibility.ProtectedAndInternal => SyntaxKind.InternalKeyword,
        _ => throw new ArgumentException($"{nameof(accessibility)} illegal"),
    };
    /// <summary>
    /// 获取<see cref="SimpleLambdaExpressionSyntax"/>内的标识符名称
    /// </summary>
    /// <param name="expressionSyntax"></param>
    /// <returns></returns>
    public static string? GetLambdaExpressionBodyIdentifier(this ExpressionSyntax expressionSyntax)
    {
        if (expressionSyntax is SimpleLambdaExpressionSyntax
            {
                ExpressionBody: MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax, Name: IdentifierNameSyntax identifierNameSyntax }
            })
        {
            return identifierNameSyntax.Identifier.ValueText;

        }
        return null;

    }

    /// <summary>
    /// 获取字符串文字表达式值字面量
    /// </summary>
    /// <param name="expressionSyntax">表达式语法</param>
    /// <returns>字面量</returns>
    /// <exception cref="ArgumentException">传参非字符串文字表达式</exception>
    internal static string GetStringLiteralExpressionGetValueText(this ExpressionSyntax expressionSyntax)
    {
        // 强制检查表达式是字符串常量字面量
        if (expressionSyntax is LiteralExpressionSyntax { RawKind: 8750 } stringLiteralExpression)
        {
            return stringLiteralExpression.Token.ValueText;
        }

        throw new ArgumentException("ExpressionSyntax must be LiteralExpressionSyntax");
    }
    /// <summary>
    /// 类型转换为指定语法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="syntaxReferences"></param>
    /// <returns></returns>
    internal static T? FirstOrDefaultSyntax<T>(this ImmutableArray<SyntaxReference> syntaxReferences) where T : SyntaxNode
    {
        foreach (var @ref in syntaxReferences)
        {
            if (@ref.GetSyntax() is T syntax)
            {
                return syntax;
            }
        }
        return null;
    }








    internal static ImmutableArray<T> Filter<T>(this SyntaxList<StatementSyntax> statementSyntaxs) where T : StatementSyntax
        => statementSyntaxs.OfType<T>().ToImmutableArray();


    internal static ImmutableArray<T> Filter<T>(this ImmutableArray<ISymbol> symbols, Func<ISymbol, bool>? filterFunc = null) where T : ISymbol => filterFunc switch

    {
        null => symbols.OfType<T>().ToImmutableArray(),
        _ => symbols.Where(filterFunc).OfType<T>().ToImmutableArray()
    };



    internal static bool IsVisible(this ISymbol symbol) => symbol.DeclaredAccessibility switch
    {
        Accessibility.Public => true,


        Accessibility.Internal or Accessibility.ProtectedOrInternal or Accessibility.ProtectedAndInternal => true,
        _ => false,

    };


}