namespace MapperToolkit
{
    public interface ICustomMapperConfiguration<TSource, TDestination>
    {
        ICustomMapperConfiguration<TSource, TDestination> Map<TMember>(Func<TSource, TMember> mapExpression, Func<TDestination, TMember> destinationMemberName);

    }
}
