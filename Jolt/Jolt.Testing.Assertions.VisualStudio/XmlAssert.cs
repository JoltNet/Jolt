// ----------------------------------------------------------------------------
// XmlAssert.cs
//
// Contains the definition of the XmlAssert class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 10:46:58
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jolt.Testing.Assertions.VisualStudio
{
    /// <summary>
    /// Provides assertion methods for verifying and querying the
    /// contents and structure of XML.
    /// </summary>
    public static class XmlAssert
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Asserts that the two given XML elements contain the same contents and structure.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML.
        /// </param>
        /// 
        /// <param name="actual">
        /// The actual XML.
        /// </param>
        public static void AreEqual(XmlReader expected, XmlReader actual)
        {
            ProcessXmlComparisonResult(
                Factory.CreateXmlEqualityAssertion().AreEqual(expected, actual));
        }

        /// <summary>
        /// Asserts that the two given XML elements are equivalent according to the
        /// given definition of equivalency strictness.
        /// </summary>
        /// 
        /// <param name="flags">
        /// A set of flags denoting the definition of equivalency.
        /// </param>
        /// 
        /// <param name="expected">
        /// The expected XML.
        /// </param>
        /// 
        /// <param name="actual">
        /// The actual XML.
        /// </param>
        public static void AreEquivalent(XmlComparisonFlags flags, XmlReader expected, XmlReader actual)
        {
            ProcessXmlComparisonResult(
                Factory.CreateXmlEquivalencyAssertion(flags).AreEquivalent(expected, actual));
        }

        /// <summary>
        /// Asserts that the given XML document is valid with the given schemas.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas used to validate.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate.
        /// </param>
        public static void IsValid(XmlSchemaSet schemas, XmlReader actual)
        {
            ProcessValidationResult(
                Factory.CreateXmlValidityAssertion(schemas).Validate(actual));
        }

        /// <summary>
        /// Asserts that the given XML document is valid with the given schemas and
        /// validator configuration.
        /// </summary>
        /// 
        /// <param name="flags">
        /// The configuration of the XML validator.
        /// </param>
        /// 
        /// <param name="schemas">
        /// The schemas used to validate.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate.
        /// </param>
        public static void IsValid(XmlSchemaSet schemas, XmlSchemaValidationFlags flags, XmlReader actual)
        {
            ProcessValidationResult(
                Factory.CreateXmlValidityAssertion(schemas, flags).Validate(actual));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies an <seealso cref="XmlComparisonResult"/> for assertion validity, raising
        /// an <seealso cref="AssertionFailedException"/> if the assertion failed.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The result to verify.
        /// </param>
        private static void ProcessXmlComparisonResult(XmlComparisonResult assertionResult)
        {
            if (!assertionResult.Result)
            {
                throw new AssertFailedException(String.Concat(
                    assertionResult.Message,
                    Environment.NewLine,
                    "XPath: ",
                    assertionResult.XPathHint));
            }
        }

        /// <summary>
        /// Verifies an <code>IList&lt;ValidationEventArgs&gt</code> for assertion validity, raising
        /// an <seealso cref="AssertionFailedException"/> if the assertion failed.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The result to verify.
        /// </param>
        private static void ProcessValidationResult(IList<ValidationEventArgs> assertionResult)
        {
            if (assertionResult.Count > 0)
            {
                throw new AssertFailedException(assertionResult[0].Message);
            }
        }

        #endregion

        #region internal fields -------------------------------------------------------------------

        internal static IAssertionFactory Factory = new AssertionFactory();

        #endregion
    }
}