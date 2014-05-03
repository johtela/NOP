Basic Concepts
==============

Immutability and Values
-----------------------

Functional programming differs from object-oriented programming in many ways. Perhaps the most fundamental trait separating the two is that in FP value types are prevalent whereas in OOP reference types are the default. Objects have both state and identity, but values have just state. Moreover, once initialized, this state does not change. Value types are *immutable* by default in FP.

The implications of this basic difference are quite far-reaching. In FP computations are performed by functions which take values as arguments and produce new values. In OOP the standard way of computing something is to mutate existing objects. The OO-style is inherently imperative and the language constructs of C# adhere to this style for the most part. Interestingly, many of the more recent features of C# are actually inspired by functional languages. These features include generics (which is called parametric polymorphism in FP), anonymous functions a.k.a. lambdas, and LINQ, which is inspired by list comprehensions.

The new features make it possible to write C# in functional way altough the language itself does not enforce this style. For example, you can write so called "functional objects" that are immutable, but these are still reference types, not values. Any function that takes an object as a parameter has the risk of failing if it does not explicitly check that the object is not null. Sometimes you see code bases where there are tons of null checks for function arguments. This is just silly as most of the time functions require that their arguments have values. However, since objects are inherently nullable it is necessary to practice defensive programming to enforce this fact.

In Flop all types are essntially value types, even if they are implemented as classes. There are no excessive null checks, rather the value semantics is assumed to hold everywhere. If this is not case, most probably a null reference exception is thrown by .NET framework at some point. The responsibility lies with the user of the library to make sure that value semantics are followed. In practice, it is quite unusual to get problems because of this requirement. Once the functional style is embraced, null checks in the code will be few and far between. Almost only reason for seeing a null reference exception is when the initialization of some member field has been forgotten.

Of course C# has also the struct type which is a true value type. Alas, what diminishes the utility of structs is that they are allocated from stack instead of garbage collected object heap. This makes their allocation and deallocation performance much worse than for objects when they contain more than just a couple of fields. Structs are only good for small values that represent simple types like complex numbers or coordinates.


Tuples
------

.NET framework already contains some functional data types that the Flop library uses extensively. First of these is the [Tuple class](http://msdn.microsoft.com/en-us/library/system.tuple(v=vs.110).aspx). Tuples are important in functional programming since they make it possible to create compound types quickly without defining a new named data structure. Functional languages usually provide syntactic support for creating tuples quickly. Here is how a tuple is created in F#:

```Fsharp
let t = (42, "foo")
```

In C# there is no built-in syntactic support for tuples, but we can use a trick that utilizes the generic type inference to make creating tuples a bit easier. Instead of writing:

```Cs
var t = new Tuple<int, string>(42, "foo");
```

We can omit the type parameters by using the static `Create` method in the `Tuple` class:

```Cs
var t = Tuple.Create(42, "foo");
```

The same trick is used in other places of Flop library as well. In functional languages the members of a tuple are anonymous, and they are bound to local variables by pattern matching:


```Fsharp
let (n, s) = t
```

In C# there is no pattern matching, but we can simulate it by creating the following extension method (defined in the `Flop.Extensions` class):

```Cs
public static void Bind<T, U> (this Tuple<T, U> tuple, Action<T, U> action)
{
	action (tuple.Item1, tuple.Item2);
}
```

This extension methods binds the members of a tuple to the arguments of a lambda expression. It can be used like so:

```Cs
t.Bind((n, s) => ...)
```

Since the members of a tuple are just properties of the `Tuple<T1,T2,...>`  class it is naturally possible to refer to the members of a tuple by their names. The name of the *i*th item in the tuple is `Item`*i*.

```Cs
n = t.Item1;
s = t.Item2;
```
