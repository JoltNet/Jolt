// ----------------------------------------------------------------------------
// XmlEqualityConstraint.cs
//
// Contains the definition of the XmlEqualityConstraint class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/8/2009 17:17:02
// ----------------------------------------------------------------------------

using System;
using System.Xml;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Implements a constraint to verify if two XML readers contain XML
    /// that is equal in terms of structure and content of elements.
    /// </summary>
    public sealed class XmlEqualityConstraint : AbstractConstraint<XmlReader, XmlComparisonResult>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the constraint with the expected XML.
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// The expected XML.
        /// </param>
        public XmlEqualityConstraint(XmlReader expectedXml)
            : this(expectedXml, new XmlEqualityAssertion()) { }

        /// <summary>
        /// Initializes the constraint with the expected XML
        /// and the assertion that the constraint delegates to.
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// The expected XML.
        /// </param>
        ///
        /// <param name="assertion">
        /// The assertion delegated to by the constraint.
        /// </param>
        internal XmlEqualityConstraint(XmlReader expectedXml, XmlEqualityAssertion assertion)
        {
            m_expectedXml = expectedXml;
            m_assertion = assertion;
        }

        #endregion

        #region AbstractXmlConstraint overrides ---------------------------------------------------

        /// <summary>
        /// <see cref="AbstractXmlConstraint.Assert"/>
        /// </summary>
        protected override XmlComparisonResult Assert(XmlReader actual)
        {
            return m_assertion.AreEqual(m_expectedXml, actual);
        }

        /// <summary>
        /// <see cref="AbstractXmlConstraint.ToBoolean"/>
        /// </summary>
        protected override bool ToBoolean(XmlComparisonResult assertionResult)
        {
            return assertionResult.Result;
        }

        /// <summary>
        /// <see cref="AbstractXmlConstraint.CreateAssertionErrorMessage"/>
        /// </summary>
        protected override string CreateAssertionErrorMessage(XmlComparisonResult assertionResult)
        {
            return String.Concat(
                assertionResult.Message,
                Environment.NewLine,
                "XPath: ",
                assertionResult.XPathHint);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the XmlEqualityAssertion associated with the class.
        /// </summary>
        internal XmlEqualityAssertion Assertion
        {
            get { return m_assertion; }
        }

        /// <summary>
        /// Gets the XmlReader associated with the class.
        /// </summary>
        internal XmlReader ExpectedXml
        {
            get { return m_expectedXml; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly XmlReader m_expectedXml;
        private readonly XmlEqualityAssertion m_assertion;

        #endregion
    }
}