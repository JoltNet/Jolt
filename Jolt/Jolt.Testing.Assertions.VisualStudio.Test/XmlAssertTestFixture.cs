// ----------------------------------------------------------------------------
// XmlAssertTestFixture.cs
//
// Contains the definition of the XmlAssertTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 9/1/2009 12:21:51
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using MVTU = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jolt.Testing.Assertions.VisualStudio.Test
{
    [TestFixture]
    public sealed class XmlAssertTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the default static initialization of the class.
        /// </summary>
        [Test]
        public void StaticConstruction()
        {
            // Re-execute the static constructor to guarantee that all default static state
            // is present for this test.
            typeof(XmlAssert).TypeInitializer.Invoke(null, null);
            Assert.That(XmlAssert.Factory, Is.Not.Null);
            Assert.That(XmlAssert.Factory, Is.InstanceOf<AssertionFactory>());
        }

        /// <summary>
        /// Verifies the behavior of the AreEqual() method, for a passing assertion.
        /// </summary>
        [Test]
        public void AreEqual_AssertPassed()
        {
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();

            using (XmlReader expected = XmlReader.Create(Stream.Null),
                             actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlEqualityAssertion()).Return(assertion);
                assertion.Expect(a => a.AreEqual(expected, actual)).Return(new XmlComparisonResult());

                XmlAssert.Factory = assertionFactory;
                XmlAssert.AreEqual(expected, actual);
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AreEqual() method, for a failed assertion.
        /// </summary>
        [Test]
        public void AreEqual_AssertFailed()
        {
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            
            using (XmlReader expected = XmlReader.Create(Stream.Null),
                             actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlEqualityAssertion()).Return(assertion);
                assertion.Expect(a => a.AreEqual(expected, actual)).Return(CreateFailedComparisonResult());

                XmlAssert.Factory = assertionFactory;

                try
                {
                    XmlAssert.AreEqual(expected, actual);
                    Assert.Fail();
                }
                catch (MVTU.AssertFailedException ex)
                {
                    Assert.That(ex.Message, Is.EqualTo("message" + Environment.NewLine + "XPath: /ns:element"));
                }
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, for a passing assertion.
        /// </summary>
        [Test]
        public void AreEquivalent_AssertPassed()
        {       
            XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder | XmlComparisonFlags.IgnoreElementValues;
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(expectedFlags);

            using (XmlReader expected = XmlReader.Create(Stream.Null),
                             actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlEquivalencyAssertion(expectedFlags)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expected, actual)).Return(new XmlComparisonResult());

                XmlAssert.Factory = assertionFactory;
                XmlAssert.AreEquivalent(expectedFlags, expected, actual);
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, for a failed assertion.
        /// </summary>
        [Test]
        public void AreEquivalent_AssertFailed()
        {
            XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder | XmlComparisonFlags.IgnoreElementValues;
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(expectedFlags);

            using (XmlReader expected = XmlReader.Create(Stream.Null),
                             actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlEquivalencyAssertion(expectedFlags)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expected, actual)).Return(CreateFailedComparisonResult());

                XmlAssert.Factory = assertionFactory;

                try
                {
                    XmlAssert.AreEquivalent(expectedFlags, expected, actual);
                    Assert.Fail();
                }
                catch (MVTU.AssertFailedException ex)
                {
                    Assert.That(ex.Message, Is.EqualTo("message" + Environment.NewLine + "XPath: /ns:element"));
                }
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given only
        /// an XML schema set, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_AssertPassed()
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(expectedSchemas);
            
            using (XmlReader actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlValidityAssertion(expectedSchemas)).Return(assertion);
                assertion.Expect(a => a.Validate(actual)).Return(new ValidationEventArgs[0]);

                XmlAssert.Factory = assertionFactory;
                XmlAssert.IsValid(expectedSchemas, actual);
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given only
        /// an XML schema set, for a failing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_AssertFailed()
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(expectedSchemas);

            using (XmlReader actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlValidityAssertion(expectedSchemas)).Return(assertion);
                assertion.Expect(a => a.Validate(actual)).Return(CreateFailedValidationResult());

                XmlAssert.Factory = assertionFactory;

                try
                {
                    XmlAssert.IsValid(expectedSchemas, actual);
                }
                catch (MVTU.AssertFailedException ex)
                {
                    Assert.That(ex.Message, Is.EqualTo("message"));
                }
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given an
        /// XML schema set and schema validation flags, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_Flags_AssertPassed()
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.ProcessInlineSchema;

            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(expectedSchemas, expectedFlags);

            using (XmlReader actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlValidityAssertion(expectedSchemas, expectedFlags)).Return(assertion);
                assertion.Expect(a => a.Validate(actual)).Return(new ValidationEventArgs[0]);

                XmlAssert.Factory = assertionFactory;
                XmlAssert.IsValid(expectedSchemas, expectedFlags, actual);
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given an
        /// XML schema set and schema validation flags, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_Flags_AssertFailed()
        {
            XmlSchemaSet expectedSchemas = new XmlSchemaSet();
            XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.ProcessInlineSchema;

            IAssertionFactory assertionFactory = MockRepository.GenerateMock<IAssertionFactory>();
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(expectedSchemas, expectedFlags);

            using (XmlReader actual = XmlReader.Create(Stream.Null))
            {
                assertionFactory.Expect(af => af.CreateXmlValidityAssertion(expectedSchemas, expectedFlags)).Return(assertion);
                assertion.Expect(a => a.Validate(actual)).Return(CreateFailedValidationResult());

                XmlAssert.Factory = assertionFactory;

                try
                {
                    XmlAssert.IsValid(expectedSchemas, expectedFlags, actual);
                }
                catch (MVTU.AssertFailedException ex)
                {
                    Assert.That(ex.Message, Is.EqualTo("message"));
                }
            }

            assertionFactory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates an <see cref="XmlComparisonResult"/> representing a failed comparison.
        /// </summary>
        private static XmlComparisonResult CreateFailedComparisonResult()
        {
            XElement element = new XElement(XName.Get("element", "ns"));
            return new XmlComparisonResult(false, "message", element, element);
        }

        /// <summary>
        /// Creates a single-element collection of
        /// <see cref="ValidationEventArgs"/> representing a failed comparison.
        /// </summary>
        private static ValidationEventArgs[] CreateFailedValidationResult()
        {
            return new ValidationEventArgs[] {
                Activator.CreateInstance(
                    typeof(ValidationEventArgs),
                    CompoundBindingFlags.NonPublicInstance,
                    null,
                    new object[] { new XmlSchemaException("message") },
                    null) as ValidationEventArgs
            };
        }

        #endregion
    }
}