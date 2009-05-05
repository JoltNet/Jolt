// ----------------------------------------------------------------------------
// Functor.cs
//
// Contains the definition of the Functor class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/17/2009 03:34:45
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Functional
{
    /// <summary>
    /// Provides methods that transform delegates, as well as factory
    /// methods for creating delegates that perform common operations.
    /// </summary>
    public static class Functor
    {
        /// <summary>
        /// Adapts a Func delegate to the corresponding Action delegate
        /// by ignoring the Func return value.
        /// </summary>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Action ToAction<TResult>(Func<TResult> function)
        {
            return () => function();
        }

        /// <summary>
        /// Adapts a Func delegate to the corresponding Action delegate
        /// by ignoring the Func return value.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Action<T> ToAction<T, TResult>(Func<T, TResult> function)
        {
            return arg => function(arg);
        }

        /// <summary>
        /// Adapts a Func delegate to the corresponding Action delegate
        /// by ignoring the Func return value.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Action<T1, T2> ToAction<T1, T2, TResult>(Func<T1, T2, TResult> function)
        {
            return (arg1, arg2) => function(arg1, arg2);
        }

        /// <summary>
        /// Adapts a Func delegate to the corresponding Action delegate
        /// by ignoring the Func return value.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        ///
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Action<T1, T2, T3> ToAction<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        {
            return (arg1, arg2, arg3) => function(arg1, arg2, arg3);
        }

        /// <summary>
        /// Adapts a Func delegate to the corresponding Action delegate
        /// by ignoring the Func return value.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        ///
        /// <typeparam name="T4">
        /// The type of the fourth function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Action<T1, T2, T3, T4> ToAction<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
        {
            return (arg1, arg2, arg3, arg4) => function(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Adapts a Func delegate to the corresponding Predicate delegate.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the predicate argument.
        /// </typeparam>
        /// 
        /// <param name="function">
        /// The function to adapt.
        /// </param>
        public static Predicate<T> ToPredicate<T>(Func<T, bool> function)
        {
            return new Predicate<T>(function);
        }

        /// <summary>
        /// Adapts a Predicate delegate to the corresponding Func delegate.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the predicate argument.
        /// </typeparam>
        /// 
        /// <param name="predicate">
        /// The predicate to adapt.
        /// </param>
        public static Func<T, bool> ToPredicateFunc<T>(Predicate<T> predicate)
        {
            return new Func<T, bool>(predicate);
        }

        /// <summary>
        /// Creates a functor that returns a constant value for any input.
        /// </summary>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="value">
        /// The constant return value.
        /// </param>
        public static Func<TResult> Idempotency<TResult>(TResult value)
        {
            return () => value;
        }

        /// <summary>
        /// Creates a functor that returns a constant value for any input.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="value">
        /// The constant return value.
        /// </param>
        public static Func<T, TResult> Idempotency<T, TResult>(TResult value)
        {
            return arg => value;
        }

        /// <summary>
        /// Creates a functor that returns a constant value for any input.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="value">
        /// The constant return value.
        /// </param>
        public static Func<T1, T2, TResult> Idempotency<T1, T2, TResult>(TResult value)
        {
            return (arg1, arg2) => value;
        }

        /// <summary>
        /// Creates a functor that returns a constant value for any input.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="value">
        /// The constant return value.
        /// </param>
        public static Func<T1, T2, T3, TResult> Idempotency<T1, T2, T3, TResult>(TResult value)
        {
            return (arg1, arg2, arg3) => value;
        }

        /// <summary>
        /// Creates a functor that returns a constant value for any input.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T4">
        /// The type of the fourth function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of the function's return value.
        /// </typeparam>
        /// 
        /// <param name="value">
        /// The constant return value.
        /// </param>
        public static Func<T1, T2, T3, T4, TResult> Idempotency<T1, T2, T3, T4, TResult>(TResult value)
        {
            return (arg1, arg2, arg3, arg4) => value;
        }

        /// <summary>
        /// Creates a functor that returns immediately, performing no operation.
        /// </summary>
        public static Action NoOperation()
        {
            return delegate { };
        }

        /// <summary>
        /// Creates a functor that returns immediately, performing no operation.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the function argument.
        /// </typeparam>
        public static Action<T> NoOperation<T>()
        {
            return delegate { };
        }

        /// <summary>
        /// Creates a functor that returns immediately, performing no operation.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        public static Action<T1, T2> NoOperation<T1, T2>()
        {
            return delegate { };
        }

        /// <summary>
        /// Creates a functor that returns immediately, performing no operation.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        public static Action<T1, T2, T3> NoOperation<T1, T2, T3>()
        {
            return delegate { };
        }

        /// <summary>
        /// Creates a functor that returns immediately, performing no operation.
        /// </summary>
        /// 
        /// <typeparam name="T1">
        /// The type of the first function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T2">
        /// The type of the second function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T3">
        /// The type of the third function argument.
        /// </typeparam>
        /// 
        /// <typeparam name="T4">
        /// The type of the fourth function argument.
        /// </typeparam>
        public static Action<T1, T2, T3, T4> NoOperation<T1, T2, T3, T4>()
        {
            return delegate { };
        }

        /// <summary>
        /// Creates an identity function, returning its given argument.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the function argument and return value.
        /// </typeparam>
        public static Func<T, T> Identity<T>()
        {
            return arg => arg;
        }

        /// <summary>
        /// Creates a predicate that returns true for any input.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the function argument.
        /// </typeparam>
        public static Func<T, bool> TrueForAll<T>()
        {
            return Idempotency<T, bool>(true);
        }

        /// <summary>
        /// Creates a predicate that returns false for any input.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the function argument.
        /// </typeparam>
        public static Func<T, bool> FalseForAll<T>()
        {
            return Idempotency<T, bool>(false);
        }
    }
}