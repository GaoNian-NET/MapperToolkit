using static MapperToolkit.SourceGenerators.Extensions.SyntaxFactoryExtension;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MapperToolkit.SourceGenerators;

public partial class MapperSourceGenerator
{


    private MemberDeclarationSyntax[] CreateMember(IEnumerable<SourceMember> members)
    {
        MemberDeclarationSyntax[] memberDeclarationSyntaxes = new MemberDeclarationSyntax[members.Count()];

        for (var i = 0; i < members.Count(); i++)
        {
            var crruent = members.ElementAt(i);
            var tree =
            PropertyDeclaration(
                    IdentifierName(crruent.Type.OriginalFullyQualifiedName),
                    Identifier(crruent.MemberName))
                .WithModifiers(crruent.DeclaredAccessibility.ConvertSyntaxKind())
                .WithAccessorList(
                    AccessorList(
                        List(
                            new AccessorDeclarationSyntax[]{
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(
                                SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken))})));
            memberDeclarationSyntaxes[i] = tree;
        }
        return memberDeclarationSyntaxes;

    }






    private IEnumerable<MemberDeclarationSyntax> CreateIEnumerableMapperMetho(SourceObejct @object)
    {
        if (@object.SubMappers is null)
        {
            yield break;
        }

        var GetBody = (SubMapperInfo subMapper) => subMapper.Kind switch
        {
            TypeSymbolInfoKind.Array => Block(
                ParseStatement($"var result = new {subMapper.DestinationMember.Type.OriginalFullyQualifiedName.TrimEnd(']')}source.{subMapper.SourceMember.Type.LengthExpression}];"),
                ParseForStatement(
                        $"for (var i=0; i < source.{subMapper.SourceMember.Type.LengthExpression}; i++)"
                        , $"result[i] = source{subMapper.SourceMember.Type.IndexExpression}.MapperTo{subMapper.DestinationMember.Type.Name}();"),
                    ParseStatement("return result;")),
            TypeSymbolInfoKind.Enumerator => Block(ParseForStatement(
                    $"for (var i=0; i < source.{subMapper.SourceMember.Type.LengthExpression}; i++)"
                    , $"yield return source{subMapper.SourceMember.Type.IndexExpression}.MapperTo{subMapper.DestinationMember.Type.Name}();")),
            TypeSymbolInfoKind.Enumerable => Block(ParseStatement($"var result = new {subMapper.DestinationMember.Type.OriginalFullyQualifiedName}();"),
                ParseForStatement(
                    $"for (var i=0; i < source.{subMapper.SourceMember.Type.LengthExpression}; i++)"
                , $"result.Add(source{subMapper.SourceMember.Type.IndexExpression}.MapperTo{subMapper.DestinationMember.Type.Name}());"),
                ParseStatement("return result;")),
            _ => null
        };





        List<(string, string)> exist = new();
        foreach (var subMapper in @object.SubMappers)
        {
            if (exist.Contains((subMapper.DestinationMember.Type.OriginalFullyQualifiedName, subMapper.SourceMember.Type.OriginalFullyQualifiedName)))
            {
                continue;
            }
            else
            {
                exist.Add((subMapper.DestinationMember.Type.OriginalFullyQualifiedName, subMapper.SourceMember.Type.OriginalFullyQualifiedName));
            }
            if (GetBody.Invoke(subMapper) is BlockSyntax body)
            {
                yield return MethodDeclaration(ParseName(subMapper.DestinationMember.Type.OriginalFullyQualifiedName), Identifier($"MapperTo{subMapper.DestinationMember.Type.Name}"))
               .WithModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword)
               .WithParameterList(
                   ParameterList(
                       SingletonSeparatedList(
                           Parameter(Identifier("source"))
                           .WithModifiers(SyntaxKind.ThisKeyword)
                           .WithType(ParseName(subMapper.SourceMember.Type.OriginalFullyQualifiedName)))))
               .WithBody(body);
            }


        }

    }
    private StatementSyntax[] CreateBaseMapper(SourceObejct @object)
    {
        List<StatementSyntax> statementSyntaxes = new()
        {
            ParseStatement($"{@object.DestinationType.FullyQualifiedName } result = new();")
        };

        @object.Mappers.ForEach(mapper =>
        statementSyntaxes.Add(ParseStatement($"{mapper.TypeName} {mapper.MemberName}Mapper = {mapper.Expression};")));

        @object.Members.ForEach(member =>
        {
            var tmpExpression = member.NeedMapper
                   ? $"result.{member.MemberName} = {member.MemberName}Mapper.Invoke(source);"
                   : $"result.{member.MemberName} = source.{member.BaseMemberName ?? member.MemberName};";
            statementSyntaxes.Add(ParseStatement(tmpExpression));

        });
        @object.SubMappers?.ForEach(mapper =>
        statementSyntaxes.Add(ParseStatement($"result.{mapper.DestinationMember.MemberName} = source.{mapper.MapperExpression};")));

        statementSyntaxes.Add(ParseStatement("return result;"));
        return statementSyntaxes.ToArray();
    }
    private MemberDeclarationSyntax CreateBaseMapperMethod(SourceObejct @object)
        => ParseMemberDeclaration<MethodDeclarationSyntax>($"{@object.DestinationType.FullyQualifiedName} MapperTo{@object.DestinationType.Name}(this {@object.SourceType.FullyQualifiedName} source)")
         .WithModifiers(@object.DeclaredAccessibility.ConvertSyntaxKind(), SyntaxKind.StaticKeyword)
        .WithBody(CreateBaseMapper(@object));

    private MemberDeclarationSyntax CreateTransformObject(SourceObejct @object) =>
        ParseTypeDeclaration(@object.Type, @object.DestinationType.Name)
        .WithModifiers(@object.DeclaredAccessibility.ConvertSyntaxKind())
        .WithMembers(CreateMember(@object.Members).ToSyntaxList());

    private MemberDeclarationSyntax CreateMapper(SourceObejct @object)
    {
        List<MemberDeclarationSyntax> members = new()
        {
            CreateBaseMapperMethod(@object)
        };
        members.AddRange(CreateIEnumerableMapperMetho(@object));
        return ClassDeclaration($"{@object.DestinationType.Name}Mapper")
        .WithModifiers(@object.DeclaredAccessibility.ConvertSyntaxKind(), SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword).WithMembers(members.ToSyntaxList());
    }

    private CompilationUnitSyntax CreateCompilationUnitSyntax(
        SyntaxList<UsingDirectiveSyntax> usingDirectiveSyntaxes
        , string @namespace
        , params MemberDeclarationSyntax[] memberDeclarationSyntaxes)
    {
        var tree =
            CompilationUnit().WithUsings(usingDirectiveSyntaxes)
            .WithMembers(SingletonList<MemberDeclarationSyntax>(
                NamespaceDeclaration(ParseName(@namespace))
                .WithNamespaceKeyword(
                    Token(PragmaWarningDirectiveTrivia("#nullable enable", "#pragma warning disable CS8618")
                    , SyntaxKind.NamespaceKeyword
                    , TriviaList()))
                .WithMembers(memberDeclarationSyntaxes.ToSyntaxList())))
            .NormalizeWhitespace();

        return tree;
    }
    private CompilationUnitSyntax CreateCompilationUnitSyntax(SourceObejct @object) =>
        @object.DestinationMembersHashMap switch
        {
            null => CreateCompilationUnitSyntax(@object.Usings, @object.Namespace
                , CreateTransformObject(@object)
                , CreateMapper(@object)),

            _ => CreateCompilationUnitSyntax(@object.Usings, @object.Namespace, CreateMapper(@object))
        };

    internal (string fileName, string code) CreateCode(IEnumerable<MethoInfo> methoInfos, ObjectInfo baseInfo)
    {
        SourceObejct @object = default;
        foreach (var methoInfo in methoInfos)
        {
            @object = AnalyzeMetho(@object, methoInfo, baseInfo);
        }

        @object = IMplicitConversion(@object);

        var tree = CreateCompilationUnitSyntax(@object);
        return ($"{@object.SourceType.Name}.{@object.DestinationType.Name}.g.cs", tree.ToFullString());
    }

    private SourceObejct IMplicitConversion(SourceObejct @object) => (@object.NeedIMplicitConversion, @object.FunctionType) switch
    {
        (true, FunctionType.Mapper) => IMplicitConversionMapper(@object),
        (true, FunctionType.Transoform) => IMplicitConversionTransform(@object),
        _ => @object
    };
    private SourceObejct AnalyzeMetho(SourceObejct @object, MethoInfo methoInfo, ObjectInfo baseInfo) =>

       (methoInfo.FunctionType, methoInfo.MethoType) switch
       {
           (FunctionType.Mapper, MethoType.Generate) => GenerateMapper(methoInfo, baseInfo),
           (FunctionType.Mapper, MethoType.Map) => CreateMapper(@object, methoInfo),
           (FunctionType.Mapper, MethoType.Ignore) => IgnoreMapper(@object, methoInfo),

           (FunctionType.Transoform, MethoType.Generate) => GenerateTransform(methoInfo, baseInfo),
           (FunctionType.Transoform, MethoType.Map) => CreateMemberWithMapper(@object, methoInfo),
           (FunctionType.Transoform, MethoType.Ignore) => IgnoreMember(@object, methoInfo),

           _ => throw new ArgumentException()

       };
}
