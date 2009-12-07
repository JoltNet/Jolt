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
        /// Asserts that the XML referenced by two given <see cref="System.Xml.XmlReader"/> objects
        /// are equal in terms of contents and structure.
        /// </summary>
        /// 
        /// <param name="expected">
        /// An <see cref="System.Xml.XmlReader"/> referencing the expected XML, 
        /// </param>
        /// 
        /// <param name="actual">
        /// An <see cref="System.Xml.XmlReader"/>, referencing the XML to validate.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// The assertion failed.
        /// </exception>
        public static void AreEqual(XmlReader expected, XmlReader actual)
        {
            ProcessXmlComparisonResult(
                Factory.CreateXmlEqualityAssertion().AreEqual(expected, actual));
        }

        /// <summary>
        /// Asserts that the XML referenced by two given <see cref="System.Xml.XmlReader"/> objects
        /// are equivalent according to the given equivalency configuration.
        /// </summary>
        /// 
        /// <param name="flags">
        /// A <see cref="XmlComparisonFlags"/> value denoting the definition of equivalency.
        /// </param>
        /// 
        /// <param name="expected">
        /// An <see cref="System.Xml.XmlReader"/> referencing the expected XML, 
        /// </param>
        /// 
        /// <param name="actual">
        /// An <see cref="System.Xml.XmlReader"/>, referencing the XML to validate.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// The assertion failed.
        /// </exception>
        public static void AreEquivalent(XmlComparisonFlags flags, XmlReader expected, XmlReader actual)
        {
            ProcessXmlComparisonResult(
                Factory.CreateXmlEquivalencyAssertion(flags).AreEquivalent(expected, actual));
        }

        /// <summary>
        /// Asserts that the XML referenced by the given <see cref="System.Xml.XmlReader"/> object
        /// is valid with the given schemas.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas used to validate.
        /// </param>
        /// 
        /// <param name="actual">
        /// An <see cref="System.Xml.XmlReader"/>, referencing the XML to validate.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// The assertion failed.
        /// </exception>
        public static void IsValid(XmlSchemaSet schemas, XmlReader actual)
        {
            ProcessValidationResult(
                Factory.CreateXmlValidityAssertion(schemas).Validate(actual));
        }

        /// <summary>
        /// Asserts that the XML referenced by the given <see cref="System.Xml.XmlReader"/> object
        /// is valid with the given schemas and validator configuration.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas used to validate.
        /// </param>
        /// 
        /// <param name="flags">
        /// An <see cref="XmlSchemaValidationFlags"/> value that configures the XML validator.
        /// </param>
        ///
        /// <param name="actual">
        /// An <see cref="System.Xml.XmlReader"/>, referencing the XML to validate.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// The assertion failed.
        /// </exception>
        public static void IsValid(XmlSchemaSet schemas, XmlSchemaValidationFlags flags, XmlReader actual)
        {
            ProcessValidationResult(
                Factory.CreateXmlValidityAssertion(schemas, flags).Validate(actual));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies an <see cref="XmlComparisonResult"/> for assertion validity
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The <see cref="XmlComparisonResult"/> to verify.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// <paramref name="assertionResult"/> describes a failed assertion.
        /// </exception>
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
        /// Verifies a <see cref="System.Collections.Generic.IList"/> of <see cref="ValidationEventArgs"/>
        /// for assertion validity.
        /// </summary>
        /// 
        /// <param name="assertionResult">
        /// The list of <see cref="ValidationEventArgs"/> to verify.
        /// </param>
        /// 
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// <paramref name="assertionResult"/> describes a failed assertion.
        /// </exception>
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