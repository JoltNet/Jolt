// ----------------------------------------------------------------------------
// XmlEqualityAssertion.cs
//
// Contains the definition of the XmlEqualityAssertion class.
// Copyright 2009 Steve Guidi.
//
// File created: 5/25/2009 10:56:59
// ----------------------------------------------------------------------------

using System.Xml;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides methods to verify if two XML readers contain XML
    /// that is equal in terms of structure and content of elements.
    /// </summary>
    public class XmlEqualityAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes an instance of the XmlEqualityAssertion class.
        /// </summary>
        public XmlEqualityAssertion()
            : this (new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)) { }

        /// <summary>
        /// Initializes an instance of the XmlEqualityAssertion class
        /// with the given equivalency assertion.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The equivalency assertion to associate with the instance.
        /// </param>
        internal XmlEqualityAssertion(XmlEquivalencyAssertion assertion)
        {
            m_assert = assertion;
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Compares two given elements for equality in terms of
        /// structure and element contents.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected element.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element being validated for equality.
        /// </param>
        public virtual XmlComparisonResult AreEqual(XmlReader expected, XmlReader actual)
        {
            return m_assert.AreEquivalent(expected, actual);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the equivalency assertion that is associated with the instance.
        /// </summary>
        internal XmlEquivalencyAssertion EquivalencyAssertion
        {
            get { return m_assert; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly XmlEquivalencyAssertion m_assert;

        #endregion
    }
}