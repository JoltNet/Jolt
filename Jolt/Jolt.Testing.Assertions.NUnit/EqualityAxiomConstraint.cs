// ----------------------------------------------------------------------------
// EqualityAxiomConstraint.cs
//
// Contains the definition of the EqualityAxiomConstraint class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 11:29:11
// ----------------------------------------------------------------------------

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an NUnit constraint that verifies if a given type
    /// implements the correct equality axioms, as specified at
    /// http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are verified.
    /// </typeparam>
    public sealed class EqualityAxiomConstraint<T> : AbstractEqualityAxiomConstraint<T, EqualityAxiomAssertion<T>>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EqualityAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public EqualityAxiomConstraint(IArgumentFactory<T> argumentFactory)
            : this(Factory.CreateEqualityAxiomAssertion(argumentFactory)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="EqualityAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The assertion instance to use for validating equality axioms.
        /// </param>
        internal EqualityAxiomConstraint(EqualityAxiomAssertion<T> assertion)
            : base(assertion) { }
    }
}