// ----------------------------------------------------------------------------
// EquatableAxiomAssertion.cs
//
// Contains the definition of the EquatableAxiomAssertion class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 11:38:38
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides assertion methods for verifiying if the given type <typeparamref name="T"/>
    /// correctly implements equality semantics.  Overrides <see cref="EqualityAxiomAssertion&lt;T&gt;"/>
    /// functionality to validate implementations of <see cref="System.IEquatable&lt;T&gt;"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are validated.
    /// </typeparam>
    public class EquatableAxiomAssertion<T> : EqualityAxiomAssertion<T>
        where T : IEquatable<T>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="EquatableAxiomAssertion&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public EquatableAxiomAssertion(IEquatableFactory<T> factory)
            : base(factory) { }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Determines if two instances of <see cref="System.IEquatable&lt;T&gt;"/> are equal.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance to validate for equality.
        /// </param>
        /// 
        /// <param name="y">
        /// The second instance to validate for equality.
        /// </param>
        /// 
        /// <returns>
        /// True if <paramref name="x"/> equals <paramref name="y"/>.
        /// False otherwise.
        /// </returns>
        protected override bool AreEqual(T x, T y)
        {
            // Implicitly resolves to (x as IEquatable<T>).Equals(y).
            return x.Equals(y);
        }

        #endregion
    }
}