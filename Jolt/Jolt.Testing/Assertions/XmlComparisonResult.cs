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
    /// Denotes an assertion failure during the execution of an
    /// XML comparison assertion.
    /// </summary>
    public sealed class XmlComparisonResult
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlComparisonResult"/>
        /// class.
        /// </summary>
        public XmlComparisonResult() : this (true, String.Empty, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlComparisonResult"/>
        /// class.
        /// </summary>
        /// 
        /// <param name="comparisonResult">
        /// Denotes the result of the comparison of hte given elements, as prescribed
        /// by the configuration of the assertion that created the comparison result.
        /// </param>
        /// 
        /// <param name="message">
        /// A message describinng the result of the comparison..
        /// </param>
        /// 
        /// <param name="expected">
        /// The expected <see cref="XElement"/> referred to by the comparison.
        /// </param>
        /// 
        /// <param name="actual">
        /// The actual <see cref="XElement"/> referred to by the comparison.
        /// </param>
        public XmlComparisonResult(bool comparisonResult, string message, XElement expected, XElement actual)
        {
            m_comparisonResult = comparisonResult;
            m_message = message;
            m_expectedElement = expected;
            m_actualElement = actual;
            m_xPathHint = actual != null ? CreateXPathExpressionFor(actual) : String.Empty;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the result of the comparison of the <seealso cref="ExpectedElement"/>
        /// and <seealso cref="ActualElement"/>, as prescribed by the configuration of the
        /// assertion that created the comparison result.
        /// </summary>
        public bool Result
        {
            get { return m_comparisonResult; }
        }

        /// <param name="message">
        /// Gets the message describinng the result of the comparison..
        /// </param>
        public string Message
        {
            get { return m_message; }
        }

        /// <summary>
        /// Gets the expected <see cref="XElement"/> operated upon by the comparison.
        /// </summary>
        public XElement ExpectedElement
        {
            get { return m_expectedElement; }
        }

        /// <summary>
        /// Gets the actual <see cref="XElement"/> operated upon by the comparison.
        /// </summary>
        public XElement ActualElement
        {
            get { return m_actualElement; }
        }

        /// <summary>
        /// Gets an approximate XPath expression for <seealso cref="ActualElement"/>,
        /// suggesting which element may have been operated upon by the comparison.
        /// </summary>
        public string XPathHint
        {
            get { return m_xPathHint; }
        }


        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Derives an XPath expression for the given element.
        /// </summary>
        /// 
        /// <param name="element">
        /// The element for which an XPath expression is computed.
        /// </param>
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

        #region private data ----------------------------------------------------------------------

        private readonly bool m_comparisonResult;
        private readonly string m_message;
        private readonly XElement m_expectedElement;
        private readonly XElement m_actualElement;
        private readonly string m_xPathHint;

        #endregion
    }
}