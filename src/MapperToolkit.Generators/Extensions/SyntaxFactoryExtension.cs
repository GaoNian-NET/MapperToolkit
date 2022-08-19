namespace MapperToolkit.SourceGenerators.Extensions;

internal static class SyntaxFactoryExtension
{
    public static TStatement ParseStatement<TStatement>(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true) where TStatement : StatementSyntax
    {
        if (SyntaxFactory.ParseStatement(text, offset, options, consumeFullText) is not TStatement statment)
        {
            throw new ArgumentNullException(nameof(statment));
        }
        else
        {
            return statment;
        }
    }


    public static TMember ParseMemberDeclaration<TMember>(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true) where TMember : MemberDeclarationSyntax
    {
        if (SyntaxFactory.ParseMemberDeclaration(text, offset, options, consumeFullText) is not TMember member)
        {
            throw new ArgumentNullException(nameof(member));
        }
        else
        {
            return member;
        }
    }
    public static ParameterSyntax WithModifiers(this ParameterSyntax parameter, params SyntaxKind[] kinds)
    {
        var tokens = new SyntaxToken[kinds.Length];
        for (var i = 0; i < kinds.Length; i++)
        {
            tokens[i] = SyntaxFactory.Token(kinds[i]);
        }

        return parameter.WithModifiers(SyntaxFactory.TokenList(tokens));
    }
    public static TMember WithModifiers<TMember>(this TMember member, params SyntaxKind[] kinds) where TMember : MemberDeclarationSyntax
    {
        var tokens = new SyntaxToken[kinds.Length];
        for (var i = 0; i < kinds.Length; i++)
        {
            tokens[i] = SyntaxFactory.Token(kinds[i]);
        }

        if (member.WithModifiers(SyntaxFactory.TokenList(tokens)) is not TMember result)
        {
            throw new ArgumentNullException(nameof(member));
        }

        return result;
    }

    public static MethodDeclarationSyntax WithBody(this MethodDeclarationSyntax metho, params StatementSyntax[] statement)
        => metho.WithBody(SyntaxFactory.Block(statement));


    public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode => new(nodes);


    public static TypeDeclarationSyntax ParseTypeDeclaration(TypeKind typeKind, string identifier) => typeKind switch
    {
        TypeKind.Class => SyntaxFactory.ClassDeclaration(identifier),
        TypeKind.Struct => SyntaxFactory.StructDeclaration(identifier),
        _ => SyntaxFactory.ClassDeclaration(identifier),
    };
    public static SyntaxTriviaList PragmaWarningDirectiveTrivia(params string[] errorCodes) =>
        SyntaxFactory.ParseTrailingTrivia(string.Join(Environment.NewLine, errorCodes));


    public static ForStatementSyntax ParseForStatement(string forStatement, params string[] statementStrings)
    {
        var statements = new StatementSyntax[statementStrings.Length];
        for (var i = 0; i < statements.Length; i++)
        {
            statements[i] = SyntaxFactory.ParseStatement(statementStrings[i]);
        }
        return ParseStatement<ForStatementSyntax>(forStatement)
            .WithStatement(SyntaxFactory.Block(statements));
    }

}
