namespace MapperToolkit;
public interface IAllTransformConfiguration<TSource>
{
    IAllTransformConfiguration<TSource> Map<TMember>(Func<TSource, TMember> transformExpression, string memberName);
    IAllTransformConfiguration<TSource> Ignore<TMember>(Func<TSource, TMember> transformExpression);


}
