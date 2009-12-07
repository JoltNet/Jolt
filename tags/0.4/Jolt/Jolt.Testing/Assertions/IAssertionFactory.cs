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
    /// Defines a contract for a factory class that creates core assertions.
    /// </summary>
    internal interface IAssertionFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class.
        /// Treats validation warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas);

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        /// 
        /// <param name="flags">
        /// The configuration of the XML validator.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/> and <paramref name="flags"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags flags);

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </summary>
        /// 
        /// <returns>
        /// The newly created instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </returns>
        XmlEqualityAssertion CreateXmlEqualityAssertion();

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEquivalencyAssertion"/> class.
        /// </summary>
        /// 
        /// <param name="strictness">
        /// A set of flags denoting the definition of equivalency, for this instance.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlEquivalencyAssertion"/>, initialized with
        /// <paramref name="strictness"/>.
        /// </returns>
        XmlEquivalencyAssertion CreateXmlEquivalencyAssertion(XmlComparisonFlags strictness);
    }
}