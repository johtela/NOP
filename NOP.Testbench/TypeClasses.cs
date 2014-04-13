using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOP.Collections;
using NOP.Testing;

namespace NOP.Testbench
{
    public interface Num<T>
    {
        T Add (T x, T y);
        T Mul (T x, T y);
        T Neg (T x);
    }

    public class NumInt : Num<int>
    {
        public int Add (int x, int y)
        {
            return x + y;
        }

        public int Mul (int x, int y)
        {
            return x * y;
        }

        public int Neg (int x)
        {
            return -x;
        }
    }

    public class NumFloat : Num<float>
    {
        public float Add (float x, float y)
        {
            return x + y;
        }

        public float Mul (float x, float y)
        {
            return x * y;
        }

        public float Neg (float x)
        {
            return -x;
        }
    }

    public interface Eq<T>
    {
        bool Equals (T x, T y);
    }

    public class EqInt : Eq<int>
    {
        public bool Equals (int x, int y)
        {
            return x == y;
        }
    }

    public class EqFloat : Eq<float>
    {
        public bool Equals (float x, float y)
        {
            return x == y;
        }
    }

    public class EqTuple<T, U> : Eq<Tuple<T, U>>
    {
        private Eq<T> _eqT;
        private Eq<U> _eqU;

        public EqTuple (Eq<T> eqT, Eq<U> eqU)
        {
            _eqT = eqT;
            _eqU = eqU;
        }

        public bool Equals (Tuple<T, U> x, Tuple<T, U> y)
        {
            return _eqT.Equals (x.Item1, y.Item1) && _eqU.Equals (x.Item2, y.Item2);
        }
    }

    public class EqStrictList<T> : Eq<StrictList<T>>
    {
        private Eq<T> _eqT;

        public EqStrictList (Eq<T> eqT)
        {
            _eqT = eqT;
        }

        public bool Equals (StrictList<T> x, StrictList<T> y)
        {
            while (!(x.IsEmpty || y.IsEmpty))
            {
                if (!_eqT.Equals (x.First, y.First))
                    return false;
                x = x.Rest;
                y = y.Rest;
            }
            return x.IsEmpty && y.IsEmpty;
        }
    }

    public class TypeClassTests
    {
        public static T Square<T> (Num<T> num, T x)
        {
            return num.Mul (x, x);
        }

        public static Tuple<T, U> Squares<T, U> (Num<T> numT, Num<U> numU, Tuple<T, U> x)
        {
            return Tuple.Create (numT.Mul (x.Item1, x.Item1), numU.Mul (x.Item2, x.Item2));
        }

        public static bool Member<T> (Eq<T> eqT, StrictList<T> list, T item)
        {
            for (var l = list; !l.IsEmpty; l = l.Rest )
                if (eqT.Equals (list.First, item))
                    return true;
            return false;
        }

        [Test]
        public void TestMember ()
        {
            var l = List.Create (1, 2, 3, 4, 5);
            Check.IsTrue (Member (new EqInt (), l, 3));
        }   
    }
}
