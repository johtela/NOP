										using System;
using System.Linq;
using LinqExpr = System.Linq.Expressions;
using LExpr = System.Linq.Expressions.Expression;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List<object>;
using SysColl = System.Collections.Generic;

namespace NOP
{
	/// <summary>
	/// Delegate type for (static) functions.
	/// </summary>
	public delegate object Func (ExprList args);
	
	/// <summary>
	/// Delegate type for (dynamic) methods.
	/// </summary>
	public delegate object Meth (object obj, ExprList args);
	
	/// <summary>
	/// Extension methods for expressions. 
	/// </summary>
	public static class Expression
	{
		public static readonly ExprList NoArgs = ExprList.Empty;
		
		public static Symbol AsSymbol (this string name)
		{
			return new Symbol (name);
		}

		public static Func AsFunction<TResult> (this Func<TResult> func)
		{
			return new Func (args => func ());
		}

		public static Func AsFunction<T1, TResult> (this Func<T1, TResult> func)
		{
			return new Func (args => func ((T1)args.First));
		}

		public static Func AsFunction<T1, T2, TResult> (this Func<T1, T2, TResult> func)
		{
			return new Func (args => 
			{
				T1 arg1 = (T1)args.First;
				args = args.Rest;
				T2 arg2 = (T2)args.First;
				return func (arg1, arg2);
			});
		}
		
		public static Func AsFunction (this ConstructorInfo ci)
		{
			var par = LExpr.Parameter (typeof(ExprList), "args");
			var pis = ci.GetParameters ();
			
			if (pis.Length > 0)
			{
				var vars = new SysColl.List<LinqExpr.ParameterExpression> ();
				var block = EvalParams (par, pis, vars);
				block.Add (LExpr.New (ci, vars));
				return new Func (LExpr.Lambda<Func> (LExpr.Block (vars, block), par).Compile ());
			}
			else
				return new Func (LExpr.Lambda<Func> (LExpr.New (ci), par).Compile ());
		}
		
		public static Func AsFunction (this MethodInfo mi)
		{
			if (!mi.IsStatic)
				throw new ArgumentException ("Method should be static", "mi");
			var par = LExpr.Parameter (typeof(ExprList), "args");
			var vars = new SysColl.List<LinqExpr.ParameterExpression> ();
			var pis = mi.GetParameters ();
			
			var block = pis.Length > 0 ?
				EvalParams (par, pis, vars) : 
				new SysColl.List<LExpr> ();
			var call = LExpr.Call (mi, vars);
			if (mi.ReturnType == typeof(void))
			{
				block.Add (call);
				block.Add (LExpr.Constant (null, typeof(object)));
			}
			else
				block.Add (LExpr.Convert (call, typeof(object)));
			return new Func (LExpr.Lambda<Func> (LExpr.Block (vars, block), par).Compile ());			
		}
		
		public static Func<object> AsValueGetter (this PropertyInfo pi)
		{
			CheckNoIndexers (pi);
			return new Func<object> (
				LExpr.Lambda<Func<object>> (LExpr.Property (null, pi)).Compile ());
		}
		
		public static Func<object> AsValueGetter (this FieldInfo fi)
		{
			return new Func<object> (
				LExpr.Lambda<Func<object>> (LExpr.Field (null, fi)).Compile ());
		}
		
		public static Action<object> AsVariableSetter (this PropertyInfo pi)
		{
			CheckNoIndexers (pi);
			var value = LExpr.Parameter (typeof(object), "value");
			return new Action<object> (
				LExpr.Lambda<Action<object>> (
					LExpr.Assign (LExpr.Property (null, pi), LExpr.Convert (value, pi.PropertyType)), value).Compile ());
		}
		
		public static Action<object> AsVariableSetter (this FieldInfo fi)
		{
			var value = LExpr.Parameter (typeof(object), "value");
			return new Action<object> (
				LExpr.Lambda<Action<object>> (
					LExpr.Assign (LExpr.Field (null, fi), LExpr.Convert (value, fi.FieldType)), value).Compile ());
		}

		public static Func<object, object> AsPropertyGetter (this PropertyInfo pi)
		{
			CheckNoIndexers (pi);
			var obj = LExpr.Parameter (typeof(object), "obj");
			return new Func<object, object> (
				LExpr.Lambda<Func<object, object>> (LExpr.Property (obj, pi)).Compile ());
			
		}
		
		public static Action<object, object> AsPropertySetter (this PropertyInfo pi)
		{
			CheckNoIndexers (pi);
			var obj = LExpr.Parameter (typeof(object), "obj");
			var value = LExpr.Parameter (typeof(object), "value");
			return new Action<object, object> (
				LExpr.Lambda<Action<object, object>> (
					LExpr.Assign (LExpr.Property (obj, pi), LExpr.Convert (value, pi.PropertyType)), obj, value).Compile ());			
		}
		
		public static Meth AsMethod (this MethodInfo mi)
		{
			if (mi.IsStatic)
				throw new ArgumentException ("Method needs to be instance method", "mi");
			var obj = LExpr.Parameter (typeof(object), "obj");
			var args = LExpr.Parameter (typeof(ExprList), "args");
			var vars = new SysColl.List<LinqExpr.ParameterExpression> ();
			var pis = mi.GetParameters ();
			
			var block = pis.Length > 0 ?
				EvalParams (args, pis, vars) : 
				new SysColl.List<LExpr> ();
			var call = LExpr.Call (LExpr.Convert (obj, mi.DeclaringType), mi, vars);
			if (mi.ReturnType == typeof(void))
			{
				block.Add (call);
				block.Add (LExpr.Constant (null, typeof(object)));
			}
			else 
				block.Add (LExpr.Convert (call, typeof(object)));
			return new Meth (LExpr.Lambda<Meth> (LExpr.Block (vars, block), obj, args).Compile ());			
		}
		
		private static void CheckNoIndexers (PropertyInfo pi)
		{
			if (pi.GetIndexParameters ().Length > 0)
				throw new ArgumentException ("Property cannot have indexers");
		}
		
		private static SysColl.List<LExpr> EvalParams (LinqExpr.ParameterExpression par, ParameterInfo[] pis, 
			SysColl.List<LinqExpr.ParameterExpression> vars)
		{
			vars.AddRange (pis.Select (pi => LExpr.Parameter (pi.ParameterType, pi.Name)));
			var exps = new SysColl.List<LExpr> ();
			exps.Add (AssignParam (par, vars [0], pis [0]));
			for (int i = 1; i < pis.Length; i++)
			{
				exps.Add (LExpr.Assign (par, LExpr.Property (par, "Rest")));
				exps.Add (AssignParam (par, vars [i], pis [i]));
			}
			return exps;
		}
		
		private static LExpr AssignParam (LinqExpr.ParameterExpression param, 
			LinqExpr.ParameterExpression variable, ParameterInfo pi)
		{
			return LExpr.Assign (variable, 
				LExpr.Convert (LExpr.Property (param, "First"), pi.ParameterType));
		}
	}
}