using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP.Visuals
{
    /// <summary>
    /// Default generator functions for s-expressions.
    /// </summary>
    public class DefaultVisualGenerator
    {
        public static Visual Literal(SExpr sexp)
        {
            return Visual.Label (((SExpr.Literal)sexp).ToString ());
        }

        public static Visual Symbol(SExpr sexp)
        {
            return Visual.Label (((SExpr.Symbol)sexp).Name);
        }

        public static Visual HList(SExpr sexp)
        {
            return Visual.HorizontalStack (VAlign.Bottom, FormatHList ((SExpr.List)sexp));
        }

        private static IEnumerable<Visual> FormatHList (SExpr.List list)
        {
            yield return Visual.Label ("(");
            foreach (SExpr se in list.Items)
            {
                yield return se.GenerateVisual (se);
                yield return Visual.Label (" ");
            }
            yield return Visual.Label (")");
        }
    }
}
