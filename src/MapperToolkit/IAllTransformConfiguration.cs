namespace MapperToolkit;
public interface IAllTransformConfiguration<TSource>
{
    IAllTransformConfiguration<TSource> Map<TMember>
        (Func<TSource, TMember> transformExpression, string? memberName = null, params Attribute[] attrs);

    IAllTransformConfiguration<TSource> Map<TMember>
        (Func<TSource, TMember> transformExpression, string? memberName = null);

    IAllTransformConfiguration<TSource> Ignore<TMember>(Func<TSource, TMember> transformExpression);


}
