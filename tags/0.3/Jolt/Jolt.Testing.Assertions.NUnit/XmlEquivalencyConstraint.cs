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
    /// Implements a constraint to verify if two XML readers contain XML
    /// that is equivalent in terms of a given user-definition.
    /// </summary>
    public sealed class XmlEquivalencyConstraint : AbstractConstraint<XmlReader, XmlComparisonResult>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the constraint with the expected XML, settings
        /// the equivalency stricness level to the strictest level (i.e.
        /// strict equality).
        /// </summary>
        /// 
        /// <param name="expectedXml">
        /// The expected XML.
        /// </param>
        public XmlEquivalencyConstraint(XmlReader expectedXml)
            : this(expectedXml, flags => new XmlEquivalencyAssertion(flags)) { }

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
        internal XmlEquivalencyConstraint(XmlReader expectedXml, CreateXmlEquivalencyAssertionDelegate createAssertion)
        {
            m_comparisonFlags = XmlComparisonFlags.Strict;
            m_expectedXml = expectedXml;
            m_createAssertion = createAssertion;
        }

        #endregion

        #region AbstractXmlConstraint overrides ---------------------------------------------------

        /// <summary>
        /// <see cref="AbstractXmlConstraint.Assert"/>
        /// </summary>
        protected override XmlComparisonResult Assert(XmlReader actual)
        {
            return m_createAssertion(m_comparisonFlags).AreEquivalent(m_expectedXml, actual);
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

        /// <summar y>
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
        /// Gets the factory method that creates the XmlEquivalencyAssertion
        /// that is used by the class.
        /// </summary>
        internal Delegate CreateAssertion
        {
            get { return m_createAssertion; }
        }

        /// <summary>
        /// Gets the comparison flags value that is used to initialize an
        /// XmlEquivalencyAssertion.
        /// </summary>
        internal XmlComparisonFlags ComparisonFlags
        {
            get { return m_comparisonFlags; }
        }

        /// <summary>
        /// Gets the XmlReader associated with the class.
        /// </summary>
        internal XmlReader ExpectedXml
        {
            get { return m_expectedXml; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Adds the given comparison flag to the stored flag and returns an
        /// instance of the modified object.
        /// </summary>
        /// 
        /// <param name="flag">
        /// The flag to add to the stored flag instance.
        /// </param>
        private XmlEquivalencyConstraint ApplyComparisonFlag(XmlComparisonFlags flag)
        {
            m_comparisonFlags |= flag;
            return this;
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private XmlComparisonFlags m_comparisonFlags;

        private readonly XmlReader m_expectedXml;
        private readonly CreateXmlEquivalencyAssertionDelegate m_createAssertion;

        #endregion
    }
}