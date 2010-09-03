// ----------------------------------------------------------------------------
// EquatableAxiomConstraint.cs
//
// Contains the definition of the EquatableAxiomConstraint class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 11:47:21
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an NUnit constraint that verifies if a given <see cref="IEquatable&lt;T&gt;"/>
    /// implements the correct equality axioms, as specified at
    /// http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are verified.
    /// </typeparam>
    public sealed class EquatableAxiomConstraint<T> : AbstractEqualityAxiomConstraint<T, EquatableAxiomAssertion<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EquatableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public EquatableAxiomConstraint(IEquatableFactory<T> argumentFactory)
            : this(Factory.CreateEquatableAxiomAssertion(argumentFactory)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="EquatableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The assertion instance to use for validating equality axioms.
        /// </param>
        internal EquatableAxiomConstraint(EquatableAxiomAssertion<T> assertion)
            : base(assertion) { }
    }
}