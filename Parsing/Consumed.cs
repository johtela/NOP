namespace NOP.Parsing
{
	using NOP;

	public class Consumed<T, S, P>
	{
		private Lazy<Reply<T, S, P>> _reply;

		public Consumed (Lazy<Reply<T, S, P>> reply)
		{
			_reply = reply;
		}

		public virtual bool IsEmpty
		{ 
			get { return false; }
		}

		public Reply<T, S, P> Reply
		{ 
			get { return _reply;}
		}
	}

	public class Empty<T, S, P> : Consumed<T, S, P>
	{
		public Empty (Lazy<Reply<T, S, P>> reply) : base (reply)
		{
			// Evaluate the lazy value.
			Fun.Ignore (Reply);
		}

		public override bool IsEmpty
		{
			get { return true; }
		}
	}
}