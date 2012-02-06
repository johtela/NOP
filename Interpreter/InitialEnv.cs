namespace NOP
{
	using System;
	using SysColl = System.Collections.Generic;
	using System.Linq;
	using NOP.Collections;

	internal class InitialEnv
	{
//		private static Expr Comparison (Func<Expr, Expr, bool> compare)
//		{
//			return new Expr.Function (args =>
//			{
//				if (args.Length < 2)
//					Interpreter.Error (args.First, "Expected at least two arguments");
//				var result = true;
//				do
//				{
//					result = result && compare (args.First, args.Rest.First);
//					args = args.Rest;
//				}
//				while (result && !args.Rest.IsEmpty);
//				return new Expr.Atom<bool> (result);
//			});
//		}
//
//		private static double CalculateFloat (Func<double, double, double> operFloat, List<Expr> args)
//		{
//			var result = Convert.ToDouble (args.First.GetInnerValue ());
//			var list = args.Rest;
//			while (!list.IsEmpty)
//			{
//				result = operFloat (result, Convert.ToDouble (list.First.GetInnerValue ()));
//				list = list.Rest;
//			}
//			return result;
//		}
//
//		private static int CalculateInt (Func<int, int, int> operInt, List<Expr> args)
//		{
//			var result = Convert.ToInt32 (args.First.GetInnerValue ());
//			var list = args.Rest;
//			while (!list.IsEmpty)
//			{
//				result = operInt (result, Convert.ToInt32 (list.First.GetInnerValue ()));
//				list = list.Rest;
//			}
//			return result;
//		}
//
//		private static Expr Arithmetic (Func<double, double, double> operFloat, Func<int, int, int> operInt)
//		{
//			return new Expr.Function (args =>
//			{
//				if (args.IsEmpty)
//					Interpreter.Error (args.First, "Expected at least one argument");
//				var tc = Convert.GetTypeCode (args.First.GetInnerValue ());
//				return args.First.SetInnerValue (Convert.ChangeType (
//						tc == TypeCode.Double || tc == TypeCode.Single ? 
//							CalculateFloat (operFloat, args) : 
//							CalculateInt (operInt, args), tc));
//			});
//		}
//
//		public static readonly Map<string, Expr> Bindings = Map<string, Expr>.FromPairs (Operators.Concat (Types));
//
//		private static Tuple<string, Expr>[] Operators
//		{
//			get
//			{ 
//				return new Tuple<string, Expr>[] 
//				{
//					new Tuple<string, Expr> ("not", Expression.AsFunction<bool, bool> (b => !b)),
//					new Tuple<string, Expr> ("=", Comparison ((fst, snd) => fst.Equals (snd))),
//					new Tuple<string, Expr> ("<", Comparison ((fst, snd) => fst.CompareTo (snd) < 0)),
//					new Tuple<string, Expr> (">", Comparison ((fst, snd) => fst.CompareTo (snd) > 0)),
//					new Tuple<string, Expr> ("<=", Comparison ((fst, snd) => fst.CompareTo (snd) <= 0)),
//					new Tuple<string, Expr> (">=", Comparison ((fst, snd) => fst.CompareTo (snd) >= 0)),
//					new Tuple<string, Expr> ("+", Arithmetic ((x, y) => x + y, (x, y) => x + y)),
//					new Tuple<string, Expr> ("-", Arithmetic ((x, y) => x - y, (x, y) => x - y)),
//					new Tuple<string, Expr> ("*", Arithmetic ((x, y) => x * y, (x, y) => x * y)),
//					new Tuple<string, Expr> ("/", Arithmetic ((x, y) => x / y, (x, y) => x / y))
//				}; 
//			}
//		}
//
//		private static SysColl.IEnumerable<Tuple<string, Expr>> Types
//		{
//			get
//			{
//				return AppDomain.CurrentDomain.GetAssemblies ()
//					.SelectMany (assy => assy.GetTypes ()
//						.Where (t => t.IsPublic)
//						.Select (t => new Tuple<string, Expr> (t.FullName, new Expr.Atom<Type> (t)))); 
//			}
//		}
		
		public Environment NamespaceToEnv (string ns)
		{
			return null;
		}
		
		public Environment TypeToEnv (Type type)
		{
			type.GetMembers ();
			return null;
		}
	}
}
