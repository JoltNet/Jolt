// ----------------------------------------------------------------------------
// AbstractEqualityAxiomConstraint.cs
//
// Contains the definition of the AbstractEqualityAxiomConstraint class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 11:04:15
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.Assertions.NUnit.Properties;


namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an abstract base class that contains functionality
    /// common to all NUnit axiom constraints for equality.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are validated.
    /// </typeparam>
    /// 
    /// <typeparam name="TAssertion">
    /// The type of assertion that implements the constraint's core functionality..
    /// </typeparam>
    /// 
    /// <remarks>
    /// Not intended for external use.
    /// </remarks>
    public abstract class AbstractEqualityAxiomConstraint<T, TAssertion> : AbstractConstraint<Type, AssertionResult>
        where TAssertion : EqualityAxiomAssertion<T>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="AbstractEqualityAxiomConstraint&lt;T, TArgFactory&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The assertion instance to use for validating equality axioms.
        /// </param>
        protected AbstractEqualityAxiomConstraint(TAssertion assertion)
        {
            m_assertion = assertion;
        }

        #endregion

        #region AbstractConstraint<Type, AssertionResult> members ---------------------------------

        /// <summary>
        /// <see cref="AbstractConstraint&lt;T, R&gt;.Assert"/>
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// The type represented by <paramref name="actual"/> differs from <typeparamref name="T"/>.
        /// </exception>
        protected override AssertionResult Assert(Type actual)
        {
            // The function parameter is redundant, and only provided for
            // readability and syntax sugar when using the constraint.
            if (typeof(T) != actual)
            {
                throw new ArgumentException(
                    String.Format(Resources.AxiomConstraintFailure_TypeMismatch, actual.FullName, typeof(T).FullName));
            }

            return m_assertion.Validate();
        }

        /// <summary>
        /// <see cref="AbstractConstraint&lt;T, R&gt;.ToBoolean"/>
        /// </summary>
        protected override bool ToBoolean(AssertionResult assertionResult)
        {
            return assertionResult.Result;
        }

        /// <summary>
        /// <see cref="AbstractConstraint&lt;T, R&gt;.CreateAssertionErrorMessage"/>
        /// </summary>
        protected override string CreateAssertionErrorMessage(AssertionResult assertionResult)
        {
            return assertionResult.Message;
        }

        #endregion

        #region protected internal properties -----------------------------------------------------

        /// <summary>
        /// Gets the associated assertion instance.
        /// </summary>
        protected internal TAssertion Assertion
        {
            get { return m_assertion; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly TAssertion m_assertion;

        #endregion
    }
}