// ----------------------------------------------------------------------------
// AssertionResult.cs
//
// Contains the definition of the AssertionResult class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 08:58:53
// ----------------------------------------------------------------------------

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Contains metadata describing the result of an assertion.
    /// </summary>
    public class AssertionResult
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionResult"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Initializes <see cref="Result"/> to true and <see cref="Message"/> to the empty string.
        /// </remarks>
        public AssertionResult() : this(true, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionResult"/> class.
        /// </summary>
        /// 
        /// <param name="result">
        /// The result of the assertion.
        /// </param>
        /// 
        /// <param name="message">
        /// A message describing the result of the assertion.
        /// </param>
        public AssertionResult(bool result, string message)
        {
            m_assertionResult = result;
            m_message = message;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the result of the assertion.
        /// </summary>
        public bool Result
        {
            get { return m_assertionResult; }
        }

        /// <param name="message">
        /// Gets the message describing the result of the assertion.
        /// </param>
        public string Message
        {
            get { return m_message; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly bool m_assertionResult;
        private readonly string m_message;

        #endregion
    }
}