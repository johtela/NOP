using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP
{
    public class Ref<T>
    {
        public Ref()
        {}

        public Ref(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
