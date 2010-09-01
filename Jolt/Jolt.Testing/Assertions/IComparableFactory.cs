// ----------------------------------------------------------------------------
// IComparableFactory.cs
//
// Contains the definition of the IComparableFactory interface.
// Copyright 2010 Steve Guidi.
//
// File created: 8/22/2010 21:17:31
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines an interface for constructing instances of
    /// <see cref="System.IComparable&lt;T&gt;"/> as arguments for assertion operations.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of instance operated upon by the factory.
    /// </typeparam>
    public interface IComparableFactory<T> : IArgumentFactory<T>
        where T : IComparable<T>
    {
    }
}