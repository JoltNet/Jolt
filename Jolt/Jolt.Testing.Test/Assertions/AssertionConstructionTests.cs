// ----------------------------------------------------------------------------
// AssertionConstructionTests.cs
//
// Contains the definition of the AssertionConstructionTests class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 12:01:38
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using Jolt.Reflection;
using Jolt.Testing.Assertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.Assertions
{
    internal static class AssertionConstructionTests
    {
        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlValidityAssertion"/>
        /// class, when given an XmlSchemaSet.
        /// </summary>
        internal static void XmlValidityAssertion_Schemas(Func<XmlSchemaSet, XmlValidityAssertion> createAssertion)
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlValidityAssertion assertion = createAssertion(expectedSchemas);

            XmlReaderSettings settings = assertion.CreateReaderSettings(null);
            Assert.That(settings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings, Is.EqualTo(settings.ValidationFlags));
            Assert.That(settings.ValidationType, Is.EqualTo(ValidationType.Schema));
            Assert.That(GetValidationEventHandler(settings), Is.Null);
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlValidityAssertion"/>
        /// class, when given an XmlSchemaSet and XmlValidityConstraint
        /// </summary>
        internal static void XmlValidityAssertion_Schemas_Flags(Func<XmlSchemaSet, XmlSchemaValidationFlags, XmlValidityAssertion> createAssertion)
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.None;
            XmlValidityAssertion assertion = createAssertion(expectedSchemas, expectedFlags);
            ValidationEventHandler handler = (s, a) => { };

            XmlReaderSettings settings = assertion.CreateReaderSettings(handler);
            Assert.That(settings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(settings.ValidationFlags | expectedFlags, Is.EqualTo(settings.ValidationFlags));
            Assert.That(settings.ValidationType, Is.EqualTo(ValidationType.Schema));
            Assert.That(GetValidationEventHandler(settings), Is.SameAs(handler.Method));
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlEqualityAssertion"/>
        /// class.
        /// </summary>
        internal static void XmlEqualityAssertion(Func<XmlEqualityAssertion> createAssertion)
        {
            XmlEqualityAssertion assertion = createAssertion();
            Assert.That(assertion.EquivalencyAssertion, Is.Not.Null);
            Assert.That(assertion.EquivalencyAssertion.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlEquivalencyAssertion"/>
        /// class.
        /// </summary>
        internal static void XmlEquivalencyAssertion(Func<XmlComparisonFlags, XmlEquivalencyAssertion> createAssertion)
        {
            XmlComparisonFlags[] enumValues = Enum.GetValues(typeof(XmlComparisonFlags)) as XmlComparisonFlags[];
            XmlComparisonFlags expectedValue = enumValues[new Random().Next(enumValues.Length - 1)];

            XmlEquivalencyAssertion assertion = createAssertion(expectedValue);
            Assert.That(assertion.ComparisonFlags, Is.EqualTo(expectedValue));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Retrieves the method that is subscribed to the validation event
        /// handler of the given XML reader settings.
        /// </summary>
        /// 
        /// <param name="settings">
        /// The XML reader settings whose validation event handler is retrieved.
        /// </param>
        private static MethodInfo GetValidationEventHandler(XmlReaderSettings settings)
        {
            ValidationEventHandler handler = typeof(XmlReaderSettings)
                .GetField("valEventHandler", CompoundBindingFlags.NonPublicInstance)
                .GetValue(settings) as ValidationEventHandler;
            return handler == null ? null : handler.Method;
        }

        #endregion
    }
}