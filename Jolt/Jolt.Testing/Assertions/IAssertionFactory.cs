// ----------------------------------------------------------------------------
// IAssertionFactory.cs
//
// Contains the definition of the IAssertionFactory interface.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 11:14:13
// ----------------------------------------------------------------------------

using System.Xml.Schema;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines an interface for a factory class that creates a core assertions.
    /// </summary>
    internal interface IAssertionFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class
        /// Treats validation warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas);

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        /// 
        /// <param name="validationFlags">
        /// The configuration of the XML validator.
        /// </param>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags flags);

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </summary>
        XmlEqualityAssertion CreateXmlEqualityAssertion();

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEquivalencyAssertion"/> class.
        /// </summary>
        /// 
        /// <param name="strictness">
        /// A set of flags denoting the definition of equivalency, for this instance.
        /// </param>
        XmlEquivalencyAssertion CreateXmlEquivalencyAssertion(XmlComparisonFlags strictness);
    }
}