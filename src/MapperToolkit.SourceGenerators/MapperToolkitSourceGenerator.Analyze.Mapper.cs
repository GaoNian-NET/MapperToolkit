namespace MapperToolkit.SourceGenerators;

public partial class MapperSourceGenerator
{
    private SourceObejct IgnoreMapper(SourceObejct @Object, MethoInfo methoInfo)
    {
        if (methoInfo.ExpressionSyntaxes[0].GetLambdaExpressionBodyIdentifier() is string identifier)
        {
            @Object.DestinationMembersHashMap![identifier] =
                @Object.DestinationMembersHashMap[identifier] with { IsDefault = false };

            return @Object;
        }

        throw new ArgumentException();
    }
    private SourceObejct IMplicitConversionMapper(SourceObejct @object)
    {

        var keys = @object.DestinationMembersHashMap!.Keys.Intersect(@object.SourceMembersHashMap.Keys);

        foreach (var key in keys)
        {
            var sourceMember = @object.SourceMembersHashMap[key];
            var destinationMember = @object.DestinationMembersHashMap[key];


            if (destinationMember.IsDefault)
            {
                if (destinationMember.Type.TypeKind == TypeSymbolInfoKind.Default || destinationMember.Type.FullyQualifiedName == sourceMember.Type.FullyQualifiedName)
                {
                    @object.Members.Add(new()
                    {
                        MemberName = destinationMember.MemberName,
                        Type = destinationMember.Type,
                        DeclaredAccessibility = destinationMember.Accessibility,

                    });
                }
                else if ((destinationMember.Type.TypeKind, sourceMember.Type.TypeKind) is not (TypeSymbolInfoKind.Default, TypeSymbolInfoKind.Default))
                {
                    @object.SubMappers.Add(new()
                    {
                        SourceMember = sourceMember,
                        DestinationMember = destinationMember,
                        MapperExpression = $"{sourceMember.MemberName}.MapperTo{destinationMember.Type.Name}()",
                        Kind = destinationMember.Type.TypeKind

                    });

                }


            }
        }

        return @object;
    }

    private SourceObejct CreateMapper(SourceObejct @object, MethoInfo methoInfo)
    {
        if (methoInfo.ExpressionSyntaxes[1].GetLambdaExpressionBodyIdentifier() is string destinationIdentifier)
        {
            SourceMember member = new()
            {
                MemberName = destinationIdentifier,
                Type = methoInfo.TypeArgument[0].GetInfo(),

            };
            @object.DestinationMembersHashMap![destinationIdentifier] =
                @object.DestinationMembersHashMap[destinationIdentifier] with { IsDefault = false };
            if (methoInfo.ExpressionSyntaxes[0].GetLambdaExpressionBodyIdentifier() is string souceIdentifier
                && @object.SourceMembersHashMap.TryGetValue(souceIdentifier, out var tmpMember))
            {

                member = member with { BaseMemberName = tmpMember.MemberName, DeclaredAccessibility = tmpMember.Accessibility };
            }
            else
            {
                var typeArgument = methoInfo.TypeArgument[0].GetInfo();
                var parameterType = methoInfo.ParameterSymbols.First().Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                member = member with { NeedMapper = true, Type = methoInfo.TypeArgument[0].GetInfo() };

                //var lambda = methoInfo.ExpressionSyntaxes.OfType<SimpleLambdaExpressionSyntax>().First();
                //var tokens = lambda.ExpressionBody!.ChildNodes().GetChildToken(lambda.Parameter.Identifier.Value!, new());
                //static string NameMapper(dynamic src) => src.Name.Substring(1, 2);

                @object.Mappers.Add(new MapperInfo()
                {
                    MemberName = destinationIdentifier,
                    TypeName = methoInfo.TypeArgument[0].IsAnonymousType ? parameterType.Replace(typeArgument.FullyQualifiedName, "dynamic") : parameterType,
                    Expression = methoInfo.ExpressionSyntaxes.First().GetText()



                });

            }

            @object.Members.Add(member);
        }
        else
        {
            throw new ArgumentException();


        }


        return @object;
    }
    private SourceObejct GenerateMapper(MethoInfo methoInfo, ObjectInfo baseInfo)
    {

        return new()
        {
            FunctionType = methoInfo.FunctionType!.Value,
            NeedIMplicitConversion = methoInfo.GenerateType == GenerateType.All,
            SourceType = methoInfo.TypeArgument[0].GetInfo(),
            DestinationType = methoInfo.TypeArgument[1].GetInfo(),
            DeclaredAccessibility = methoInfo.TypeArgument[1].DeclaredAccessibility,
            Type = methoInfo.TypeArgument[0].TypeKind,
            Namespace = baseInfo.BaseNamespace,
            Mappers = new(),
            Members = new(),
            SubMappers = new(),
            Usings = baseInfo.Usings,
            SourceMembersHashMap = methoInfo.TypeArgument[0].GetMembersHashMap(new()),
            DestinationMembersHashMap = methoInfo.TypeArgument[1].GetMembersHashMap(new())
        };
    }





}
