// ----------------------------------------------------------------------------
// IAssertionFactory.cs
//
// Contains the definition of the IAssertionFactory interface.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 11:14:13
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines a contract for a factory class that creates core assertions.
    /// </summary>
    internal interface IAssertionFactory
    {
        /// <summary>
        /// <see cref="XmlValidityAssertion.ctor(XmlSchemaSet)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas);

        /// <summary>
        /// <see cref="XmlValidityAssertion.ctor(XmlSchemaSet, XmlSchemaValidationFlags)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/> and <paramref name="flags"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags flags);

        /// <summary>
        /// <see cref="XmlEqualityAssertion.ctor()"/>
        /// </summary>
        /// 
        /// <returns>
        /// The newly created instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </returns>
        XmlEqualityAssertion CreateXmlEqualityAssertion();

        /// <summary>
        /// <see cref="XmlEquivalencyAssertion.ctor(XmlComparisonFlags)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlEquivalencyAssertion"/>, initialized with
        /// <paramref name="strictness"/>.
        /// </returns>
        XmlEquivalencyAssertion CreateXmlEquivalencyAssertion(XmlComparisonFlags strictness);
    }
}