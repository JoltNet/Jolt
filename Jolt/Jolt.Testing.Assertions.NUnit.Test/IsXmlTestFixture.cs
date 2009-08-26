// ----------------------------------------------------------------------------
// IsXmlTestFixture.cs
//
// Contains the definition of the IsXmlTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/23/2009 08:51:12
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    using CreateXmlEquivalencyAssertionDelegate = Func<XmlComparisonFlags, XmlEquivalencyAssertion>;


    [TestFixture]
    public sealed class IsXmlTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ValidWith() method, when given a schema set.
        /// </summary>
        [Test]
        public void ValidWith_Schemas()
        {
            // TODO: Refactor with corresponding XmlValidityConstraint construction test.
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlValidityConstraint constraint = IsXml.ValidWith(expectedSchemas);

            XmlReaderSettings readerSettings = constraint.Assertion.CreateReaderSettings(null);
            Assert.That(readerSettings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(readerSettings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings,
                Is.EqualTo(readerSettings.ValidationFlags));
        }

        /// <summary>
        /// Verifies the behavior of the ValidWith() method, when given a schema set
        /// and validation flags.
        /// </summary>
        [Test]
        public void ValidWith_Schemas_Flags()
        {
            // TODO: Refactor with corresponding XmlValidityConstraint construction test.
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.None;
            XmlValidityConstraint constraint = IsXml.ValidWith(expectedSchemas, expectedFlags);

            XmlReaderSettings readerSettings = constraint.Assertion.CreateReaderSettings(null);
            Assert.That(readerSettings.Schemas, Is.SameAs(expectedSchemas));
            Assert.That(readerSettings.ValidationFlags | expectedFlags, Is.EqualTo(readerSettings.ValidationFlags));
        }

        /// <summary>
        /// Verifies the behavior of the EqualTo() method.
        /// </summary>
        [Test]
        public void EqualTo()
        {
            // TODO: Refactor with corresponding XmlEqualityConstraint construction test.
            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEqualityConstraint constraint = IsXml.EqualTo(expectedXml);

            Assert.That(constraint.Assertion, Is.Not.Null);
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }

        [Test]
        public void EquivalentTo()
        {
            // TODO: Refactor with corresponding XmlEquivalencyConstraint construction test.
            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEquivalencyConstraint constraint = IsXml.EquivalentTo(expectedXml);

            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
            Assert.That(constraint.CreateAssertion, Is.InstanceOfType(typeof(CreateXmlEquivalencyAssertionDelegate)));
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }
    }
}