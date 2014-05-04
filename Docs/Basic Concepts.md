Basic Concepts
==============

Values and Immutability 
-----------------------

Functional programming differs from object-oriented programming in many ways. Perhaps the most fundamental trait separating the two is that in FP value types are prevalent whereas in OOP reference types are the default. Objects have both state and identity, and the reference usually acts as object's identity. Value types used in FP have just state. Moreover, once initialized, the state does not change. Value types are said to be *immutable* in FP.

The implications of this basic difference are quite far-reaching. In FP computations are performed by functions which take values as arguments and produce new values. In OOP computations happen through side effects when existing objects are mutated. The OO-style is inherently imperative and the language constructs of C# adhere to this style for the most part. Interestingly, many of the more recent features of C# are actually inspired by functional languages. These features include generics (which is called parametric polymorphism in FP), anonymous functions a.k.a. lambdas, and LINQ, which is inspired by list comprehensions.

The new features make it possible to write C# in functional way although the language itself does not enforce this style. For example, you can write so called "functional objects" that are immutable, but these are still reference types, not values. Any function that takes an object as a argument carries the risk of failing unexpectedly, if it does not explicitly check that the argument is not null. Sometimes you see code bases where functions *are*  littered with tons of null argument checks. This is just silly as most of the time functions require that their arguments actually have values. However, since objects are inherently nullable it is necessary to practice defensive programming to enforce this fact.

In Flop all types are essentially value types, even if they are implemented as classes. There are no excessive null checks, rather the value semantics is assumed to hold everywhere. If this is not case, a null reference exception is probably thrown by the .NET framework at some point. The responsibility lies with the user to make sure that no null references are passed to Flop. In practice, it is quite unusual to get problems because of this requirement. Once the functional style is embraced, null values in the code will be few and far between. Almost only way to trip with this is to forget to initialize some class member field in the constructor.

Of course C# has also the struct type which *is* a true value type. Alas, the utility of structs is diminished by the fact that they are allocated from stack instead of garbage collected object heap. This makes their performance worse than objects' when they contain more than just a couple of fields. Consequently, structs are only good for types that are sufficiently small and simple.

Option and Either
-----------------

TBD.


Tuples
------

.NET framework already contains some functional data types that the Flop library uses extensively. One of these is the [Tuple class](http://msdn.microsoft.com/en-us/library/system.tuple(v=vs.110).aspx). Tuples are important in functional programming since they make it possible to create compound types quickly without defining a new named data structure. Functional languages usually have special syntax for creating tuples. Here is how a tuple is created in F#:

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


Functions
---------

The bread and butter of functional programming is, of course, functions. Functions in C# come in many forms: as instance methods, static methods, and delegates. None of these actually corresponds to the mathematical notion of function that is prevalent in FP. A function, in its purest form, is just a black box that takes an input and returns a result. A function is said to be *pure*, if for the same argument it always returns the same result. In other words, the result of a function only depends on its argument and *not* on any kind of shared state.

In C# functions are always defined in context of a class, hence they are called methods. The closest thing to a "real" function in C# is static method which does not carry the implicit `this` parameter that instance methods do. In FP, functions are also first class citizens. This means that they can be bound to values, given as argument, or returned from functions. In C#, the same thing can be achieved with delegates which can be bound to both static and instance methods. 

Delegates were originally used just for callbacks and events. After generic types were added to .NET 2.0 and especially after LINQ was introduced in C# 3.0,  delegates were promoted to more significant role. Generic function types `Func<T1, ...>` and `Action<T1, ...>` were added to the `System` namespace. These types can represent any function, as long as it has reasonable number of arguments. The difference between `Func` and `Action` is that the latter does not return anything, i.e. it is bound to a `void` method. In FP, a special "Unit" type is used in these cases. Unit is a regular type, but it only has one possible value that represents "no value". So in functional setting, there is no distinction between functions that return a value or the ones that do not. To simplify working with functions and delegates, Flop library uses only the `Func` types and defines the missing Unit type in the `Flop.Unit` class.

Another important feature, that was added to C# 3.0, is support for lambda expressions. These can be used to declare anonymous functions succinctly without the need for writing down the type declarations. They also create a closure of the local variables referenced in the lambda expression, which is arguably the most important enabling mechanism for functional programming. Lambda expressions can be used to implement any delegate type, but most of the time they are used to implement functions of the `Func<T1, ...>` types. Funcs are not compatible to other delegate types even when their signatures match. This is somewhat confusing but reflects the fact that Funcs are just parameterized delegate types.  

As first class values functions can also be manipulated generically. The most common operations performed for functions are partial evaluation, composition, and *currying*. All of these operations are defined as extension methods in the static `Flop.Fun` class.

Partial evaluation generates a new function from an existing function by fixing some of its arguments. For example, if you define a function: 

```Cs
Func<string, int, string> f = (s, i) => s + i.ToString();
```

you can partially evaluate it by writing:

```Cs
var p = f.Partial("foo");
```

The type of the `p` is `Func<int, string>` since the first `string` argument is fixed. You can also fix all the arguments, which leaves only the return type:

```Cs
var p2 = f.Partial("foo", 42);
```

p2 has now the type `Func<string>`, which means that it can be now called without arguments:

```Cs
var s = p2();
```

There are different extension methods in `Flop.Fun` for all parameter combinations. The maximum number of parameters is currently restricted to four, however.

Two functions can be composed together when the return type of the first function matches the argument type of the second. The composed function calls the first function and then uses its result as the argument for the second function. In functional syntax the type signature of the compose function is:

```Fsharp
(a -> b) -> (b -> c) -> a -> c 
```

In Flop the composition function is defined as extension method. Its type signature is a bit more cluttered:

```Cs
public static Func<T1, TRes> Compose<T1, T2, TRes> (this Func<T1, T2> func1, Func<T2, TRes> func2)
```

Nevertheless, the usage of compose function is simple:

```Cs
Func<double, double> sin = Math.Sin;
Func<double, double> cos = Math.Cos; 
var sincos = sin.Compose (cos);
```

Unfortunately the following shorthand does not go pass the C# compiler. This is due to the fact the methods do not have implicit coercion to `Func` types.

```Cs
// DOES NOT WORK
var sincos = Math.Sin.Compose (Math.Cos);

// ALSO DOES NOT WORK
var sin = Math.Sin;
var cos = Math.Cos; 
var sincos = sin.Compose (cos);
```

Omitting the types of the local variables does not work either, since the C# compiler does not know to which delegate types to cast the methods to. The type inference of C# only looks at the right hand side of the var statement, so the compiler does not utilize the fact that `sin` and `cos` variables are used as Funcs later in the code. Because of these limitations the compose operation is less useful in C# than in functional languages. In most cases, writing the composition inside a lambda expression results in shorter code, as shown below. Be that as it may, it is still good to formalize the concept and define it centrally. 

```Cs
Func<double, double> sincos = d => cos(sin(d))
```

In lambda calculus, which forms the theoretical foundation for functional programming, all functions have exactly one argument. A function with two arguments is defined in lambda calculus as a function that, given the first argument, returns another function that takes the second argument. This idea is shown in the function signatures of F# and Haskell, which do not distinguish argument types from return types. For example, in F# the type of the "or" operator (||) is:

```Fsharp
bool -> bool -> bool 
```

This type signature can be read in two ways: given two `bool`s the function returns a `bool`; or given a `bool` the function returns another function which, given a `bool`, returns `bool`. In fact, the `->` operator is right associative, so the same signature can be written differently to better convey this idea:

```Fsharp
bool -> (bool -> bool) 
```

By generalizing this notion a function with arbitrary number of arguments can be transformed to an equivalent function that has only one argument. This process is called *currying* according to mathematician Haskell Curry who, along with Alonzo church and others, developed the mathematical theorems behind lambda calculus. 

Currying differs from partial application at least in two ways: partial application fixes one or more arguments whereas currying does not actually fix any arguments - it essentially returns the same function in a different form. In addition, partial application always returns a function that may have zero or more arguments whereas curried function returns either the final value or another function with exactly one argument. The differences are subtle but significant.

Currying operation is also defined as an extension method in the `Flop.Fun` class:

```Cs
public static Func<T1, Func<T2, TRes>> Curry<T1, T2, TRes> (this Func<T1, T2, TRes> func)
```

Calling a curried function looks a bit weird in C# because of the parentheses around each argument:

```Cs
Func<string, int, string> f = (s, i) => s + i.ToString();
var c = f.Curry();
var i = c("foo")(42);
```

In F# and Haskell function arguments are listed without parentheses, so currying does not change how a function is called. Partial application happens automatically in these languages as they use the Hindley-Milner type system in which function types are already defined in the curried form. Because of the different type system the need for currying arises quite seldom in C#. It is more a curiosity than a practical tool in that setting. Partial application, on the other hand, is very common in functional-style C# code.

There is an overloaded version of the `Curry` method to functions with 2-4 arguments. It does not make sense to define currying for a function with single argument since it is already in the curried form.

Finally, there are couple of generic functions defined `Flop.Fun` class that are useful in many occasions. First of these is the identity function that just returns its argument as a result.

```Cs
public static T Identity<T> (T arg)
{
	return arg;
}
```

Another trivial but important generic function is Ignore, which essentially just looses its argument. It allows expressions of any type to be used as statements, which is necessary sometimes.

```Cs
public static void Ignore<T> (T value)
{	
}
```
