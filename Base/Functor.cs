namespace NOP
{
	using System;
	using NOP.Collections;

	public interface IFunctor<T> 
	{
		IFunctor<U> Map<U> (Func<T, U> map);
	}
}