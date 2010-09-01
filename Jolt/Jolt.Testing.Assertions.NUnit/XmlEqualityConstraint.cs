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
    /// Defines an NUnit constraint to verify if two <see cref="System.Xml.XmlReader"/>
    /// objects contain XML that is equal in terms of structure and content of elements.
    /// </summary>
    public sealed class XmlEqualityConstraint : AbstractConstraint<XmlReader, XmlComparisonResult>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityConstraint"/> class.
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// An <see cref="System.Xml.XmlReader"/> object containing the expected XML.
        /// </param>
        public XmlEqualityConstraint(XmlReader expectedXml)
            : this(expectedXml, Factory.CreateXmlEqualityAssertion()) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityConstraint"/> class,
        /// encapsulating an <see cref="XmlEqualityAssertion"/>.
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// An <see cref="System.Xml.XmlReader"/> object containing the expected XML.
        /// </param>
        ///
        /// <param name="assertion">
        /// The <see cref="XmlEqualityAssertion"/> that performs the assertion.
        /// </param>
        /// 
        /// <remarks>
        /// Used internally by test code to override assertion operations.
        /// </remarks>
        internal XmlEqualityConstraint(XmlReader expectedXml, XmlEqualityAssertion assertion)
        {
            m_expectedXml = expectedXml;
            m_assertion = assertion;
        }

        #endregion

        #region AbstractConstraint members --------------------------------------------------------

        /// <summary>
        /// <see cref="AbstractConstraint.Assert"/>
        /// </summary>
        protected override XmlComparisonResult Assert(XmlReader actual)
        {
            return m_assertion.AreEqual(m_expectedXml, actual);
        }

        /// <summary>
        /// <see cref="AbstractConstraint.ToBoolean"/>
        /// </summary>
        protected override bool ToBoolean(XmlComparisonResult assertionResult)
        {
            return assertionResult.Result;
        }

        /// <summary>
        /// <see cref="AbstractConstraint.CreateAssertionErrorMessage"/>
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
        /// Gets the <see cref="XmlEqualityAssertion"/> associated with the instance.
        /// </summary>
        internal XmlEqualityAssertion Assertion
        {
            get { return m_assertion; }
        }

        /// <summary>
        /// Gets the <see cref="System.Xml.XmlReader"/> associated with the instance.
        /// </summary>
        internal XmlReader ExpectedXml
        {
            get { return m_expectedXml; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XmlReader m_expectedXml;
        private readonly XmlEqualityAssertion m_assertion;

        #endregion
    }
}