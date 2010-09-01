// ----------------------------------------------------------------------------
// ComparableAxiomConstraint.cs
//
// Contains the definition of the ComparableAxiomConstraint class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 11:50:26
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an NUnit constraint that verifies if a given <see cref="IComparable&lt;T&gt;"/>
    /// implements the correct equality axioms, as specified at
    /// http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are verified.
    /// </typeparam>
    public sealed class ComparableAxiomConstraint<T> : AbstractEqualityAxiomConstraint<T, ComparableAxiomAssertion<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ComparableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public ComparableAxiomConstraint(IComparableFactory<T> argumentFactory)
            : this(Factory.CreateComparableAxiomAssertion(argumentFactory)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ComparableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="createAssertion">
        /// The assertion instance to use for validating equality axioms.
        /// </param>
        internal ComparableAxiomConstraint(ComparableAxiomAssertion<T> assertion)
            : base(assertion) { }
    }
}