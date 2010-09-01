// ----------------------------------------------------------------------------
// XmlEquivalencyConstraint.cs
//
// Contains the definition of the XmlEquivalencyConstraint class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/8/2009 12:31:36
// ----------------------------------------------------------------------------

using System;
using System.Xml;

namespace Jolt.Testing.Assertions.NUnit
{
    using CreateXmlEquivalencyAssertionDelegate = Func<XmlComparisonFlags, XmlEquivalencyAssertion>;


    /// <summary>
    /// Defines an NUnit constraint to verify if two <see cref="System.Xml.XmlReader"/>
    /// objects contain XML that is equivalent in terms of a given user-definition.
    /// </summary>
    public sealed class XmlEquivalencyConstraint : AbstractConstraint<XmlReader, XmlComparisonResult>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEquivalencyConstraint"/> class,
        /// with the strictest level of equivalency (i.e. strict equality).
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// An <see cref="System.Xml.XmlReader"/> object containing the expected XML.
        /// </param>
        public XmlEquivalencyConstraint(XmlReader expectedXml)
            : this(expectedXml, Factory.CreateXmlEquivalencyAssertion) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEquivalencyConstraint"/> class,
        /// with the strictest level of equivalency (i.e. strict equality), and
        /// encapsulating an <see cref="XmlEquivalencyAssertion"/>.
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// An <see cref="System.Xml.XmlReader"/> object containing the expected XML.
        /// </param>
        ///
        /// <param name="createAssertion">
        /// A factory method for creating the <see cref="XmlEquivalencyAssertion"/> that performs the assertion.
        /// </param>
        /// 
        /// <remarks>
        /// Used internally by test code to override assertion operations.
        /// </remarks>
        internal XmlEquivalencyConstraint(XmlReader expectedXml, CreateXmlEquivalencyAssertionDelegate createAssertion)
        {
            m_comparisonFlags = XmlComparisonFlags.Strict;
            m_expectedXml = expectedXml;
            m_createAssertion = createAssertion;
        }

        #endregion

        #region AbstractConstraint members --------------------------------------------------------

        /// <summary>
        /// <see cref="AbstractConstraint.Assert"/>
        /// </summary>
        protected override XmlComparisonResult Assert(XmlReader actual)
        {
            return m_createAssertion(m_comparisonFlags).AreEquivalent(m_expectedXml, actual);
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

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Directs the constraint to ignore attribute namespaces during evaluation.
        /// </summary>
        public XmlEquivalencyConstraint IgnoreAttributeNamespaces
        {
            get { return ApplyComparisonFlag(XmlComparisonFlags.IgnoreAttributeNamespaces); }
        }

        /// <summary>
        /// Directs the constraint to ignore attributes during evaluation.
        /// </summary>
        public XmlEquivalencyConstraint IgnoreAttributes
        {
            get { return ApplyComparisonFlag(XmlComparisonFlags.IgnoreAttributes); }
        }

        /// <summary>
        /// Directs the constraint to ignore element namespaces during evaluation.
        /// </summary>
        public XmlEquivalencyConstraint IgnoreElementNamespaces
        {
            get { return ApplyComparisonFlag(XmlComparisonFlags.IgnoreElementNamespaces); }
        }

        /// <summary>
        /// Directs the constraint to ignore element values during evaluation.
        /// </summary>
        public XmlEquivalencyConstraint IgnoreElementValues
        {
            get { return ApplyComparisonFlag(XmlComparisonFlags.IgnoreElementValues); }
        }

        /// <summary>
        /// Directs the constraint to ignore element sequence ordering during evaluation.
        /// </summary>
        public XmlEquivalencyConstraint IgnoreSequenceOrder
        {
            get { return ApplyComparisonFlag(XmlComparisonFlags.IgnoreSequenceOrder); }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the factory method that creates the <see cref="XmlEquivalencyAssertion"/>
        /// that is used by the instance.
        /// </summary>
        internal Delegate CreateAssertion
        {
            get { return m_createAssertion; }
        }

        /// <summary>
        /// Gets the <see cref="XmlComparisonFlags"/> value that is used to initialize an
        /// the <see cref="XmlEquivalencyAssertion"/>.
        /// </summary>
        internal XmlComparisonFlags ComparisonFlags
        {
            get { return m_comparisonFlags; }
        }

        /// <summary>
        /// Gets the <see cref="System.Xml.XmlReader"/> associated with the instance.
        /// </summary>
        internal XmlReader ExpectedXml
        {
            get { return m_expectedXml; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Accumulates a <see cref="XmlComparisonFlags"/> value to the value stored by the instance.
        /// </summary>
        /// 
        /// <param name="flag">
        /// The <see cref="XmlComparisonFlags"/> to accumulate.
        /// </param>
        /// 
        /// <returns>
        /// A reference to the modified <see cref="XmlEquivalencyConstraint"/> instance.
        /// </returns>
        private XmlEquivalencyConstraint ApplyComparisonFlag(XmlComparisonFlags flag)
        {
            m_comparisonFlags |= flag;
            return this;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private XmlComparisonFlags m_comparisonFlags;

        private readonly XmlReader m_expectedXml;
        private readonly CreateXmlEquivalencyAssertionDelegate m_createAssertion;

        #endregion
    }
}