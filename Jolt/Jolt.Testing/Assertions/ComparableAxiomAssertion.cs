// ----------------------------------------------------------------------------
// ComparableEqualityAxiomAssertion.cs
//
// Contains the definition of the ComparableEqualityAxiomAssertion class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 13:17:56
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides assertion methods for verifiying if the given type <typeparamref name="T"/>
    /// correctly implements equality semantics.  Overrides <see cref="EqualityAxiomAssertion&lt;T&gt;"/>
    /// functionality to validate implementations of <see cref="System.IComparable&lt;T&gt;"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are validated.
    /// </typeparam>
    public class ComparableAxiomAssertion<T> : EqualityAxiomAssertion<T>
        where T : IComparable<T>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="ComparableAxiomAssertion&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public ComparableAxiomAssertion(IComparableFactory<T> factory)
            : base(factory) { }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Determines if two instances of <see cref="System.IComparable&lt;T&gt;"/> are equal.
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
            return x.CompareTo(y) == 0;
        }

        #endregion
    }
}