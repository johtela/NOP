namespace NOP.Base
{
	using System;
	using NOP.Collections;

	public delegate Either<TResult, string> Parser <TToken, TResult> (ISequence<TToken> stream);

}
