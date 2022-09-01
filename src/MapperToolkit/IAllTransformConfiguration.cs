namespace MapperToolkit;
public interface IAllTransformConfiguration<TSource>
{

    IAllTransformConfiguration<TSource> Map<TMember, TAttributes>
        (Func<TSource, TMember> transformExpression, string memberName, params TAttributes[] attrs) where TAttributes : Attribute;

    IAllTransformConfiguration<TSource> Map<TMember>
        (Func<TSource, TMember> transformExpression, string memberName);
    IAllTransformConfiguration<TSource> Ignore<TMember>(Func<TSource, TMember> transformExpression);


}
