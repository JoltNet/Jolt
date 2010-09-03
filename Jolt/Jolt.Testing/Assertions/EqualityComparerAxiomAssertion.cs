// ----------------------------------------------------------------------------
// EqualityComparerAxiomAssertion.cs
//
// Contains the definition of the EqualityComparerAxiomAssertion class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 21:31:48
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides assertion methods for verifiying if implementations of
    /// <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/>
    /// correctly implement equality semantics.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type that the equality comparer validates.
    /// </typeparam>
    public class EqualityComparerAxiomAssertion<T> : EqualityAxiomAssertion<T>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="EqualityComparerAxiomAssertion&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <param name="comparer">
        /// The equality comparer instance whose equality semantics are verified.
        /// </param>
        public EqualityComparerAxiomAssertion(IArgumentFactory<T> factory, IEqualityComparer<T> comparer)
            : base(factory)
        {
            m_comparer = comparer;
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the associated equality comparer.
        /// </summary>
        internal IEqualityComparer<T> Comparer
        {
            get { return m_comparer; }
        }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Determines if two instances of <typeparamref name="T"/> are equal according
        /// to <see cref="IEqualityComparer&lt;T&gt;.Equals"/>.
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
            return m_comparer.Equals(x, y);
        }

        /// <summary>
        /// Retrieves the hash code for a given instance of <typeparamref name="T"/>,
        /// using <see cref="IEqualityComparer&lt;T&gt;.GetHashCode"/>.
        /// </summary>
        /// 
        /// <param name="x">
        /// The instance whose hash code is retrieved.
        /// </param>
        protected override int GetHashCode(T x)
        {
            return m_comparer.GetHashCode(x);
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IEqualityComparer<T> m_comparer;

        #endregion
    }
}