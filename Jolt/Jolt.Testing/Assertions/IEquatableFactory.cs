// ----------------------------------------------------------------------------
// IEquatableFactory.cs
//
// Contains the definition of the IEquatableFactory interface.
// Copyright 2010 Steve Guidi.
//
// File created: 8/22/2010 21:13:29
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines an interface for constructing instances of
    /// <see cref="System.IEquatable&lt;T&gt;"/> as arguments for assertion operations.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of instance operated upon by the factory.
    /// </typeparam>
    public interface IEquatableFactory<T> : IArgumentFactory<T>
        where T : IEquatable<T>
    {
    }
}
