// ----------------------------------------------------------------------------
// ConstraintConstructionTests.cs
//
// Contains the definition of the ConstraintConstructionTests class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/26/2009 09:48:01
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    using CreateXmlEquivalencyAssertionDelegate = Func<XmlComparisonFlags, XmlEquivalencyAssertion>;

    
    internal static class ConstraintConstructionTests
    {
        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlValidityConstraint"/>
        /// class, when given an XmlSchemaSet.
        /// </summary>
        internal static void XmlValidityConstraint_Schemas(Func<XmlSchemaSet, XmlValidityConstraint> createConstraint)
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlValidityConstraint constraint = createConstraint(expectedSchemas);

            XmlReaderSettings readerSettings = constraint.Assertion.CreateReaderSettings(null);
            Assert.That(readerSettings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(readerSettings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings,
                Is.EqualTo(readerSettings.ValidationFlags));
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlValidityConstraint"/>
        /// class, when given an XmlSchemaSet and XmlValidityConstraint
        /// </summary>
        internal static void XmlValidityConstraint_Schemas_Flags(Func<XmlSchemaSet, XmlSchemaValidationFlags, XmlValidityConstraint> createConstraint)
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.None;
            XmlValidityConstraint constraint = createConstraint(expectedSchemas, expectedFlags);

            XmlReaderSettings readerSettings = constraint.Assertion.CreateReaderSettings(null);
            Assert.That(readerSettings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(readerSettings.ValidationFlags | expectedFlags, Is.EqualTo(readerSettings.ValidationFlags));
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlEqualityConstraint"/>
        /// class.
        /// </summary>
        internal static void XmlEqualityConstraint(Func<XmlReader, XmlEqualityConstraint> createConstraint)
        {
            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEqualityConstraint constraint = createConstraint(expectedXml);

            Assert.That(constraint.Assertion, Is.Not.Null);
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }

        /// <summary>
        /// Verifies the construction of the <seealso cref="XmlEquivalencyConstraint"/>
        /// class.
        /// </summary>
        internal static void XmlEquivalencyConstraint(Func<XmlReader, XmlEquivalencyConstraint> createConstraint)
        {
            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEquivalencyConstraint constraint = createConstraint(expectedXml);

            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
            Assert.That(constraint.CreateAssertion, Is.InstanceOf<CreateXmlEquivalencyAssertionDelegate>());
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }
    }
}