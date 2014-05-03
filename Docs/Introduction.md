Flop - Library for Writing C# Programs in Functional Style
==========================================================

Introduction
------------

Flop is a general purpose library that helps writing C# programs in the functional style. It contains purely functional data structures, parser combinators, and quickcheck-style testing tools among other things. Programs using Flop look more like their corresponding implementations in F# or Haskell than idiomatic, imperative style C# code. The goal of the library is to make C# programs more succinct and powerful using the principles of functional programming. Thus the benefits of functional style, such as easier reasoning about program correctness and better support for concurrency become more achievable. 

Flop library forms a coherent collection of classes and defines various useful abstractions that unite them. To get started, it is best to learn them bottom-up; first familiarizing oneself with the basic concepts like immutable values, tuples, options, and of course, first class functions and lambdas which are the corner stones of functional programs. These concepts are usually already mastered by self-respecting C# programmers since most of these are implemented in the C# language or in the .NET framework. If this would not been the case, creating Flop library would not have been possible.

The purely functional data structures are next: lists, trees, sets, maps, and so on. The common characteristic in all of these is *immutability*. This means that they are updated by creating new copies of the data structure instead of changing the existing ones. Of course, the copies share major parts of the original data structures so that update operations are very fast in practice. Another essential concept of functional programming, namely laziness, is easy to get acquainted with while learning about lazy lists.

Once the basics are clear it is time to learn about the collection abstractions which classify the immutable data structures. Even more general abstractions are defined to make it possible to build truly composable programs. The term abstraction, in this context, can refer to a C# interface, delegate type, or even set of types and functions that together constitute some higher level concept. Most powerful of these abstractions is the *monad*. One manifestation of this concept is in the LINQ library which also provides some syntactic sugar that we make use of in our own monads.

After all of these fundamentals are learned then it is easier to understand how parser combinators and property based testing work. The former provides a composable parsing framework and the latter makes it possible to test properties stated about a program with automatically generated data. These libraries are shamelessly copied from Haskell but adapted to C# while providing as natural API as possible. The main contribution of the Flop library is that it brings the powerful libraries of functional languages to C# without sacrificing the efficiency or the beauty of C# code.

There are probably differing opinions about the beauty of functional-style C# code. When crafted tastefully, it can be as readable and understandable as idiomatic C# which is not always so clear either. The benefits come from using higher-level concepts and achieving more with less code. The downside of writing functional code in C#, especially with the Flop library which uses heavily lambdas and continuations, is that it makes it harder to follow the code in the debugger. Usually it is faster to deduce the source of a bug by looking at the problematic code and testing specific function inputs than stepping through it in the debugger. This is typical to functional programming in general. The upside is that when most of the functions are pure (without side effects) it is easy to test them separately and pinpoint bugs by calling the functions interactively in the debugger.


Table of Contents
-----------------

[Basic Concepts](Basic Concepts.html)
