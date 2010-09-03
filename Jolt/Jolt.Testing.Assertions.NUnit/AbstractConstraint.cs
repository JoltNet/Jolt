// ----------------------------------------------------------------------------
// AbstractConstraint.cs
//
// Contains the definition of the AbstractConstraint class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/9/2009 09:09:52
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework.Constraints;


namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Defines an abstract base class that contains functionality
    /// common to all NUnit constraints.
    /// </summary>
    /// 
    /// <typeparam name="TActual">
    /// The type of the "actual" parameter given to the
    /// <see cref="Constraint.Matches(object)"/> method.
    /// </typeparam>
    /// 
    /// <typeparam name="TAssertionResult">
    /// The return type of the encapsulated assertion.
    /// </typeparam>
    /// 
    /// <remarks>
    /// Not intended for external use.
    /// </remarks>
    public abstract class AbstractConstraint<TActual, TAssertionResult> : Constraint
    {
        #region Constraint members ----------------------------------------------------------------

        /// <summary>
        /// Evaluates the constraint as per the concrete evaluation method, <see cref="Assert"/>.
        /// </summary>
        /// 
        /// <param name="actual">
        /// The value given to <see cref="Assert"/> for validation.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the evalualtion is successful, false otherwise</returns>
        public override bool Matches(object actual)
        {
            base.actual = actual;
            if (!(actual is TActual)) { return false; }

            TAssertionResult comparisonResult = Assert((TActual)actual);
            bool matches = ToBoolean(comparisonResult);
            m_assertionErrorMessage = matches ? null : CreateAssertionErrorMessage(comparisonResult);

            return matches;
        }

        /// <summary>
        /// <see cref="Constraint.WriteMessageTo(MessageWriter)"/>
        /// </summary>
        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.WriteLine(m_assertionErrorMessage);
        }

        /// <summary>
        /// <see cref="Constraint.WriteMessageTo(MessageWriter)"/>
        /// </summary>
        /// 
        /// <remarks>
        /// Not implemented since the error message rendering is
        /// accomplished entirely by <see cref="WriteMessageTo"/>.
        /// </remarks>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Performs the evaluation/assertion of the constraint.
        /// </summary>
        /// 
        /// <param name="actual">
        /// The data value to validate.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of <typeparamref name="TAssertionResult"/>, denoting
        /// the success or failure of the assertion.
        /// </returns>
        protected abstract TAssertionResult Assert(TActual actual);

        /// <summary>
        /// Converts a <typeparamref name="TAssertionResult"/> to a Boolean value.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The <typeparamref name="TAssertionResult"/> to convert.
        /// </param>
        /// 
        /// <returns>
        /// The Boolean representation of <paramref name="assertionResult"/>.
        /// </returns>
        protected abstract bool ToBoolean(TAssertionResult assertionResult);

        /// <summary>
        /// Creates an assertion error message for a given <typeparamref name="TAssertionResult"/>.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The <typeparamref name="TAssertionResult"/> from which the error message is derived.
        /// </param>
        /// 
        /// <returns>
        /// A string containing an error message pertaining to <paramref name="assertionResult"/>.
        /// </returns>
        protected abstract string CreateAssertionErrorMessage(TAssertionResult assertionResult);

        #endregion

        #region internal fields -------------------------------------------------------------------

        internal static readonly IAssertionFactory Factory = new AssertionFactory();

        #endregion

        #region private fields --------------------------------------------------------------------

        private string m_assertionErrorMessage;
 
        #endregion
    }
}