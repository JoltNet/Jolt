// ----------------------------------------------------------------------------
// XmlValidityAssertionTestFixture.cs
//
// Contains the definition of the XmlValidityAssertionTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 5/23/2009 09:19:51
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Jolt.Testing.Assertions;
using NUnit.Framework;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class XmlValidityAssertionTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class, when given only
        /// an XmlSchemaSet.
        /// </summary>
        [Test]
        public void Construction_SchemaSet()
        {
            AssertionConstructionTests.XmlValidityAssertion_Schemas(schemas => new XmlValidityAssertion(schemas));
        }

        /// <summary>
        /// Verifies the construction of the class, when given an
        /// XmlSchemaSet and XmlSchemaValidationFlags.
        /// </summary>
        [Test]
        public void Construction_SchemaSet_Flags()
        {
            AssertionConstructionTests.XmlValidityAssertion_Schemas_Flags((schemas, flags) => new XmlValidityAssertion(schemas, flags));
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when
        /// given a valid XmlReader.
        /// </summary>
        [Test]
        public void Validate_XmlReader()
        {
            XmlValidityAssertion assertion = new XmlValidityAssertion(GetTestSchemas());
            using (XmlReader xmlReader = XmlReader.Create(GetEmbeddedResource("ValidConfigurationWithOverrides.xml")))
            {
                Assert.That(assertion.Validate(xmlReader), Is.Empty);
            }
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when
        /// given an invalid XmlReader.
        /// </summary>
        [Test]
        public void Validate_XmlReader_Invalid()
        {
            XmlValidityAssertion assertion = new XmlValidityAssertion(GetTestSchemas());
            using (XmlReader xmlReader = XmlReader.Create(GetEmbeddedResource("InvalidConfiguration.xml")))
            {
                IList<ValidationEventArgs> validationErrors = assertion.Validate(xmlReader);
                Assert.That(validationErrors, Has.Count.EqualTo(1));
                Assert.That(validationErrors[0].Exception, Is.Not.Null);
                Assert.That(validationErrors[0].Message, Is.Not.Empty);
                Assert.That(validationErrors[0].Severity, Is.EqualTo(XmlSeverityType.Error));
            }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Gets an XmlSchemaSet containing a schema suitable for use
        /// in unit tests.
        /// </summary>
        private static XmlSchemaSet GetTestSchemas()
        {
            Type testSchemaSiblingType = typeof(Jolt.Testing.CodeGeneration.Xml.XmlConfigurator);
            using (Stream schemaStream = testSchemaSiblingType.Assembly.GetManifestResourceStream(testSchemaSiblingType, "RealSubjectTypes.xsd"))
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(XmlSchema.Read(schemaStream, null));
                return schemas;
            }
        }

        /// <summary>
        /// Retrieves a stream that references an embedded resource from
        /// the Jolt.Testing.CodeGeneration.Test.Xml namespace.
        /// </summary>
        /// 
        /// <param name="resourceName">
        /// The name of the embedded resource to retrieve.
        /// </param>
        private static Stream GetEmbeddedResource(string resourceName)
        {
            Type resourceSiblingType = typeof(Jolt.Testing.Test.CodeGeneration.Xml.XmlConfiguratorTestFixture);
            return resourceSiblingType.Assembly.GetManifestResourceStream(resourceSiblingType, resourceName);
        }

        #endregion
    }
}