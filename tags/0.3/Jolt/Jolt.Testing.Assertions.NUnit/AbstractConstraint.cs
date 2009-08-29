// ----------------------------------------------------------------------------
// AbstractConstraint.cs
//
// Contains the definition of the AbstractConstraint class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/9/2009 09:09:52
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Contains functionaly that is common to all XML constraints.
    /// </summary>
    /// 
    /// <typeparam name="TActual">
    /// The type of the "actual" parameter given to the
    /// <seealso cref="Constraint.Matches"/> method.
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
        #region Constraint overrides --------------------------------------------------------------

        /// <summary>
        /// Evaluates the constraint as per the concrete evaluation
        /// method, returning true if the evalualtion is successful.
        /// </summary>
        /// 
        /// <param name="actual">
        /// The value given to the evaluation method for validation.
        /// </param>
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
        /// Perfoms the evaluation/assertion of the constraint.
        /// </summary>
        /// 
        /// <param name="actual">
        /// The data value to validate.
        /// </param>
        protected abstract TAssertionResult Assert(TActual actual);

        /// <summary>
        /// Converts a given assertion result to a Boolean value indicating
        /// success or failure.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The result to convert.
        /// </param>
        protected abstract bool ToBoolean(TAssertionResult assertionResult);

        /// <summary>
        /// Creates an assertion error message for a given assertion result.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The result from which the error message is derived.
        /// </param>
        protected abstract string CreateAssertionErrorMessage(TAssertionResult assertionResult);

        #endregion

        #region private data ----------------------------------------------------------------------

        private string m_assertionErrorMessage;

        #endregion
    }
}