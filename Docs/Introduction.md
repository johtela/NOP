Flop - Library for Writing C# Programs in Functional Style
==========================================================

Introduction
------------

Flop is a general purpose library that helps writing C# programs in the functional style. It provides various features usually found in functional languages, such as immutable data structures, parser combinators, and QuickCheck style testing tools. By using Flop your programs resemble more code written in F# or Haskell than idiomatic, imperative C# code. The goal of the library is to make C# programs more succinct and powerful using the principles of functional programming. Thus the benefits of functional paradigm, such as easier reasoning about program correctness, and better support for concurrency, become more achievable. 

Flop library consists of a coherent collection of classes and various abstractions uniting them. The documentation explores the library in bottom-up direction; first explaining the basic concepts like immutable values, tuples, options, and of course, first-class functions and lambdas. These concepts are usually mastered by any self-respecting C# programmer since most of them are already implemented in the C# language or in the .NET framework.

Immutable data structures are tackled next: lists, trees, sets, maps, and so on. In practical terms, immutability means that data structures are updated by creating new copies of them, instead of changing or mutating the existing ones. The copies share substantial parts of the original data structure, so the update operations are very fast in practice. Some data structures are also *lazy*, which is another common characteristic in functional languages. It means essentially that some part of a data structure is not created until it is actually used somewhere in the code.

Once the basics are covered, it is time to get acquainted with the various abstractions that classify the immutable data structures. The term abstraction, in this context, can refer to an interface, a delegate type, or a set of types and methods that together constitute some higher level concept. The most powerful, or at least the most hyped abstraction we encounter is the *monad*. One manifestation of this concept is found in the LINQ library. LINQ also provides convenient syntactic sugar that we can make use of in our own monads.

After these ideas and concepts are understood we can delve into how parser combinators and property based testing work. Parser combinators can be used to build complex parsers from simple primitives, and the property based testing library makes it possible to test programs with automatically generated data. These libraries are shamelessly copied from Haskell but adapted to C# providing an API that suits more the language. The main contribution of the Flop library is that it ports these powerful libraries to C# without sacrificing efficiency or beauty of the code.

There are probably differing opinions about the beauty of functional-style C# code. When crafted tastefully, it can be as readable and understandable as idiomatic C#. The benefits of functional style come from working with higher-level concepts and achieving more with less code. For example, instead of using looping through a collection, it can be traversed with higher level function such as `map`, `filter`, or `reduce`. The resulted code is also much more *composable* than traditional, imperative code. Paradoxically, the promise of code reusability, which was a big selling point for OO languages, is finally being fulfilled after the functional paradigm has become more mainstream.

The disadvantage of writing code in functional style is that it makes it harder to follow the code in the debugger. This problem stems from the fact that lot of things are done through lambdas and continuations. Usually it is faster to deduce the source of a bug by testing the problematic code with specific inputs than stepping through it in the debugger. This is typical to functional programming in general. The good news is that when most of the functions are pure (without side effects), it is easy to test them separately and pinpoint bugs by calling the functions interactively in the debugger.


Table of Contents
-----------------

[Basic Concepts](Basic Concepts.html)

