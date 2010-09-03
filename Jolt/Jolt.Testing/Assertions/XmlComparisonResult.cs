// ----------------------------------------------------------------------------
// XmlComparisonResult.cs
//
// Contains the definition of the XmlComparisonResult class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/18/2009 18:08:21
// ----------------------------------------------------------------------------

using System;
using System.Text;
using System.Xml.Linq;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Contains metadata describing the result of executing an XML assertion.
    /// </summary>
    public sealed class XmlComparisonResult : AssertionResult
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a a new instance of the <see cref="XmlComparisonResult"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Initializes <see cref="AssertionResult.Result"/> to true, <see cref="Message"/> to the empty string,
        /// <see cref="ExpectedElement"/> to null, <see cref="ActualElement"/> to null,
        /// and <see cref="XPathHint"/> to null.
        /// </remarks>
        public XmlComparisonResult() : this (true, String.Empty, null, null) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlComparisonResult"/> class, initializing
        /// the values of its attributes.
        /// </summary>
        /// 
        /// <param name="comparisonResult">
        /// The result of the comparison.
        /// </param>
        /// 
        /// <param name="message">
        /// A message describing the result of the comparison.
        /// </param>
        /// 
        /// <param name="expected">
        /// The expected <see cref="System.Xml.XLinq.XElement"/> operated upon by the comparison.
        /// </param>
        /// 
        /// <param name="actual">
        /// The actual <see cref="System.Xml.XLinq.XElement"/> operated upon by the comparison.
        /// </param>
        /// 
        /// <remarks>
        /// Initializes <see cref="XPathHint"/> to an XPath expression that locates <paramref name="actual"/>.
        /// </remarks>
        public XmlComparisonResult(bool comparisonResult, string message, XElement expected, XElement actual)
            : base(comparisonResult, message)
        {
            m_expectedElement = expected;
            m_actualElement = actual;
            m_xPathHint = actual != null ? CreateXPathExpressionFor(actual) : String.Empty;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the expected <see cref="System.Xml.XLinq.XElement"/> operated upon by the comparison.
        /// </summary>
        public XElement ExpectedElement
        {
            get { return m_expectedElement; }
        }

        /// <summary>
        /// Gets the actual <see cref="System.Xml.XLinq.XElement"/> operated upon by the comparison.
        /// </summary>
        public XElement ActualElement
        {
            get { return m_actualElement; }
        }

        /// <summary>
        /// Gets an approximate XPath expression for locating <see cref="ActualElement"/>,
        /// </summary>
        public string XPathHint
        {
            get { return m_xPathHint; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Derives an XPath expression for a given <see cref="System.Xml.XLinq.XElement"/>.
        /// </summary>
        /// 
        /// <param name="element">
        /// The <see cref="System.Xml.XLinq.XElement"/> for which an XPath expression is computed.
        /// </param>
        /// 
        /// <returns>
        /// An approximate XPath expression for locating <paramref name="element"/>.
        /// </returns>
        private static string CreateXPathExpressionFor(XElement element)
        {
            StringBuilder xPathExpression = new StringBuilder();
            while (element != null)
            {
                xPathExpression.Insert(0, element.Name.LocalName);

                if (element.Name.Namespace != null && element.Name.Namespace.NamespaceName != String.Empty)
                {
                    xPathExpression.Insert(0, ':').Insert(0, element.Name.Namespace);
                }

                xPathExpression.Insert(0, '/');
                element = element.Parent;
            }

            return xPathExpression.ToString();
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XElement m_expectedElement;
        private readonly XElement m_actualElement;
        private readonly string m_xPathHint;

        #endregion
    }
}