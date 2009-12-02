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
    /// Provides assertion methods that verify if two XML readers contain XML
    /// that is equal in terms of structure and content of elements.
    /// </summary>
    public class XmlEqualityAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </summary>
        public XmlEqualityAssertion()
            : this (new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityAssertion"/> class,
        /// encapsulating an <see cref="XmlEquivalencyAssertion"/>.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The equivalency assertion to associate with the instance.
        /// </param>
        /// 
        /// <remarks>
        /// Used internally by test code to override assertion operations.
        /// </remarks>
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
        /// The expected XML, referenced by an <see cref="System.Xml.XmlReader"/>.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate, referenced by an <see cref="System.Xml.XmlReader"/>.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the assertion.
        /// </returns>
        public virtual XmlComparisonResult AreEqual(XmlReader expected, XmlReader actual)
        {
            return m_assert.AreEquivalent(expected, actual);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="XmlEquivalencyAssertion"/> that is associated with the instance.
        /// </summary>
        internal XmlEquivalencyAssertion EquivalencyAssertion
        {
            get { return m_assert; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XmlEquivalencyAssertion m_assert;

        #endregion
    }
}