namespace MapperToolkit;

public interface IAllMapperConfiguration<TSource, TDestination>
{
    IAllMapperConfiguration<TSource, TDestination> Map<TMember>(Func<TSource, TMember> mapExpression, Func<TDestination, TMember> destinationMemberName);
    IAllMapperConfiguration<TSource, TDestination> Ignore<TMember>(Func<TDestination, TMember> ignoreExpression);
    
}
1