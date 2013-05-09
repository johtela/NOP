namespace NOP.IO
{
	public abstract class SExprStore<R, W>
	{
		public abstract SExpr Read (R reader);
		public abstract void Write (SExpr sexp, W writer);
	}
}