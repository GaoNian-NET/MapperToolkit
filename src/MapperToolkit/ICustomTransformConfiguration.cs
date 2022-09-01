namespace MapperToolkit;

public interface ICustomTransformConfiguration<TSource>
{
    ICustomTransformConfiguration<TSource> Map<TMember>
        (Func<TSource, TMember> transformExpression , params Attribute[] attrs);


    ICustomTransformConfiguration<TSource> Map<TMember>
        (Func<TSource, TMember> transformExpression, string memberName , params Attribute[] attrs);

    ICustomTransformConfiguration<TSource> Map<TMember>
       (Func<TSource, TMember> transformExpression, string? memberName = null);


}
