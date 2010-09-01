// ----------------------------------------------------------------------------
// IArgumentFactory.cs
//
// Contains the definition of the IArgumentFactory interface.
// Copyright 2010 Steve Guidi.
//
// File created: 8/22/2010 21:07:16
// ----------------------------------------------------------------------------

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines an interface for constructing instances of <typeparamref name="T"/>,
    /// as arguments for assertion operations.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of instance operated upon by the factory.
    /// </typeparam>
    public interface IArgumentFactory<T>
    {
        /// <summary>
        /// Creates and returns new instance of <typeparamref name="T"/>.
        /// </summary>
        T Create();

        /// <summary>
        /// Modifies the value of an existing <typeparamref name="T"/> instance.
        /// </summary>
        /// 
        /// <param name="instance">
        /// The instance to modify.
        /// </param>
        void Modify(ref T instance);
    }
}