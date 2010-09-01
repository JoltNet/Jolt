// ----------------------------------------------------------------------------
// EqualityComparerAxiomConstraint.cs
//
// Contains the definition of the EqualityComparerAxiomConstraint class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 11:54:10
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an NUnit constraint that verifies if a given <see cref="IEqualityComparer&lt;T&gt;"/>
    /// implements the correct equality axioms, as specified at
    /// http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type that the equality comparer validates.
    /// </typeparam>
    public sealed class EqualityComparerAxiomConstraint<T> : AbstractEqualityAxiomConstraint<T, EqualityComparerAxiomAssertion<T>>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EqualityComparerAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <param name="comparer">
        /// The equality comparer instance whose equality semantics are verified.
        /// </param>
        public EqualityComparerAxiomConstraint(IArgumentFactory<T> argumentFactory, IEqualityComparer<T> comparer)
            : this(Factory.CreateEqualityComparerAxiomAssertion(argumentFactory, comparer)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="EqualityComparerAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The assertion instance to use for validating equality axioms.
        /// </param>
        internal EqualityComparerAxiomConstraint(EqualityComparerAxiomAssertion<T> assertion)
            : base(assertion) { }
    }
}