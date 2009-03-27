// ----------------------------------------------------------------------------
// TestTypes.cs
//
// Contains the definition of the types that support test code in this assembly.
// Copyright 2009 Steve Guidi.
//
// File created: 2/13/2009 11:35:36 AM
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Test.Types
{
    internal abstract class IndexerType<T, U>
    {
        internal int this[int x, T t, U u, T v]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        internal int this[Action<Action<Action<U>>> a]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        internal int this[T[] t, Action<Action<U>[][,]>[][] u, T[][,][, ,][, , ,] v]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    internal abstract class ConstructorType<T, U>
    {
        internal ConstructorType(int x, T t, U u, U v) { }
        internal ConstructorType(Action<Action<Action<T>>> a) { }
        internal ConstructorType(T[] t, out Action<Action<Action<U>[][]>[]>[][] u, U[][,][, ,][, , ,] v) { u = null; }
    }

    internal abstract class FieldType<T, U>
    {
        internal U Field;
    }

    internal unsafe class PointerTestType<T>
	{
        internal PointerTestType(Action<T[]>[] t, out string***[][,][, ,] v) { v = null; }

        internal int this[int*[] t, Action<Action<T[]>[][]>[] a, short***[][,][, ,] v]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        internal void method<U>(int x, ref T[,] t, out Action<U[][,]>*[,][] a, Action<int**[][, ,]> b) { a = null; }
	}

    public class __GenericTestType<R, S, T>
    {
        public R NonGenericFunction(S s, T t) { throw new ApplicationException("non-generic-function"); }
        public R NonGenericFunction_MixedArgs(S s, T t, int i) { throw new ApplicationException("non-generic-function-mixed-args"); }
        public R GenericFunction_MixedArgs<A, B, C>(C c, A a, B b, T t, S s, int i) { throw new ApplicationException("generic-function"); }
        public R GenericFunction<A, B, C>(A a, B b, C c) { throw new ApplicationException("generic-function"); }

        public void NoGenericParameters(int x) { throw new ApplicationException("non-generic-function-parameters"); }
        public void NoParameters() { throw new ApplicationException("no-parameters"); }
        public void NoParameters<A>() { throw new ApplicationException("no-parameters-generic"); }

        public event EventHandler<EventArgs> InstanceEvent;
    }
}