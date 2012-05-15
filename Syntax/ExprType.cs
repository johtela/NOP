using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HindleyMilner
{
    public abstract class ExprType
    {
        public class Lam : ExprType
        {
            public readonly ExprType Input, Result;

            public Lam(ExprType input, ExprType result)
            {
                Input = input;
                Result = result;
            }
        }

        public class Var : ExprType
        {
            public readonly string Name;

            public Var(string name)
            {
                Name = name;
            }
        }

        public class Con : ExprType
        {
            public readonly string Name;
            public readonly IEnumerable<ExprType> TypeArgs;

            public Con(string name, IEnumerable<ExprType> typeArgs)
            {
                Name = name;
                TypeArgs = typeArgs;
            }
        }
    }
}
