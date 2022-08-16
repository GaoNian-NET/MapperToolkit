﻿namespace MapperToolkit;

public interface ICustomTransformConfiguration<TSource>
{
    ICustomTransformConfiguration<TSource> Map<TMember>(Func<TSource, TMember> transformExpression, string memberName);

}