using System.ComponentModel;

namespace MapperToolkit.Generators;

public partial class MapperGenerator
{
    private SourceObejct CreateMemberWithMapper(SourceObejct @object, MethoInfo methoInfo)
    {
        SourceMember member = new()
        {
            Type = methoInfo.TypeArgument[0].GetInfo(),
            AttributeConfigs = new(),

        };
        if (methoInfo.ExpressionSyntaxes[0].GetLambdaExpressionBodyIdentifier() is string identifier
            && @object.SourceMembersHashMap.TryGetValue(identifier, out var tmpMember))
        {
            member = member with {
                MemberName = identifier,
                DeclaredAccessibility = tmpMember.Accessibility,
                BaseMemberName = identifier };
            @object.SourceMembersHashMap[identifier] = tmpMember with { IsDefault = false };

        }
        else
        {

            var typeArgument = methoInfo.TypeArgument[0].GetInfo();
            var parameterType = methoInfo.ParameterSymbols.First().Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var memberName = methoInfo.ExpressionSyntaxes[1].GetStringLiteralExpressionGetValueText();


            member = member with
            {
                MemberName = methoInfo.ExpressionSyntaxes[1].GetStringLiteralExpressionGetValueText(),
                DeclaredAccessibility = Accessibility.Public,
                NeedMapper = true,
                Type = methoInfo.TypeArgument[0].GetInfo()
            };
            @object.Mappers.Add(new MapperInfo()
            {
                MemberName = memberName,
                TypeName = methoInfo.TypeArgument[0].IsAnonymousType ? parameterType.Replace(typeArgument.FullyQualifiedName, "dynamic") : parameterType,
                Expression = methoInfo.ExpressionSyntaxes.First().GetText()



            });
            @object.SourceMembersHashMap[memberName] = @object.SourceMembersHashMap[memberName] with { IsDefault = false };

        }

        if (methoInfo.ParameterSymbols.Length == 3)
        {
            var attributeConfigs =
            methoInfo.ExpressionSyntaxes.OfType<ObjectCreationExpressionSyntax>();

            foreach (var attributeConfig in attributeConfigs)
            {
                member.AttributeConfigs.Add((attributeConfig.Type.ToFullString()
                    ,attributeConfig.ArgumentList?.Arguments
                    , attributeConfig.Initializer?.Expressions));
                
            }
        }
        @object.Members.Add(member);


        return @object;
    }
    private SourceObejct GenerateTransform(MethoInfo methoInfo, ObjectInfo baseInfo)
        => new()
        {
            NeedIMplicitConversion = methoInfo.GenerateType == GenerateType.All,
            SourceType = methoInfo.TypeArgument[0].GetInfo(),
            DestinationType = new TypeSymbolInfo(methoInfo.ExpressionSyntaxes[1].GetStringLiteralExpressionGetValueText()),
            DeclaredAccessibility = methoInfo.TypeArgument[0].DeclaredAccessibility,
            Type = methoInfo.TypeArgument[0].TypeKind,
            Namespace = methoInfo.ExpressionSyntaxes[0].GetStringLiteralExpressionGetValueText(),
            Mappers = new(),
            Members = new(),
            Usings = baseInfo.Usings,


            SourceMembersHashMap = methoInfo.TypeArgument[0].GetMembersHashMap(new())
        };



    private SourceObejct IgnoreMember(SourceObejct transformObject, MethoInfo methoInfo)
    {
        if (methoInfo.ExpressionSyntaxes[0].GetLambdaExpressionBodyIdentifier() is string identifier)
        {
            transformObject.SourceMembersHashMap[identifier] =

                transformObject.SourceMembersHashMap[identifier] with { IsDefault = false };

            return transformObject;
        }

        throw new ArgumentException();
    }

    private SourceObejct IMplicitConversionTransform(SourceObejct @object)
    {
        @object.Members.AddRange(@object.SourceMembersHashMap.Values.Where(x => x.IsDefault).Select(
            info => new SourceMember()
            {
                MemberName = info.MemberName,
                Type = info.Type,
                DeclaredAccessibility = info.Accessibility,
            }));
        return @object;
    }











}
