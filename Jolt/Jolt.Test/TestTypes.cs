// ----------------------------------------------------------------------------
// TestTypes.cs
//
// Contains the definition of the types that support test code in this assembly.
// Copyright 2009 Steve Guidi.
//
// File created: 2/13/2009 11:35:36 AM
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

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
}