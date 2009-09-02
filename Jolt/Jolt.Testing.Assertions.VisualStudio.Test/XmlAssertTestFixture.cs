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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using MVTU = Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Schema;
using System.Reflection;

namespace Jolt.Testing.Assertions.VisualStudio.Test
{
    [TestFixture]
    public sealed class XmlAssertTestFixture
    {
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
            Assert.That(XmlAssert.Factory, Is.InstanceOfType(typeof(AssertionFactory)));
        }

        /// <summary>
        /// Verifies the behavior of the AreEqual() method, for a passing assertion.
        /// </summary>
        [Test]
        public void AreEqual_AssertPassed()
        {
            With.Mocks(delegate
            {
                using (XmlReader expected = XmlReader.Create(Stream.Null),
                                 actual = XmlReader.Create(Stream.Null))
                {
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();

                    Expect.Call(assertionFactory.CreateXmlEqualityAssertion()).Return(assertion);
                    Expect.Call(assertion.AreEqual(expected, actual)).Return(new XmlComparisonResult());
                    Mocker.Current.ReplayAll();

                    XmlAssert.Factory = assertionFactory;
                    XmlAssert.AreEqual(expected, actual);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the AreEqual() method, for a failed assertion.
        /// </summary>
        [Test]
        public void AreEqual_AssertFailed()
        {
            With.Mocks(delegate
            {
                using (XmlReader expected = XmlReader.Create(Stream.Null),
                                 actual = XmlReader.Create(Stream.Null))
                {
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();

                    Expect.Call(assertionFactory.CreateXmlEqualityAssertion()).Return(assertion);
                    Expect.Call(assertion.AreEqual(expected, actual)).Return(CreateFailedComparisonResult());
                    Mocker.Current.ReplayAll();

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
            });
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, for a passing assertion.
        /// </summary>
        [Test]
        public void AreEquivalent_AssertPassed()
        {
            With.Mocks(delegate
            {
                using (XmlReader expected = XmlReader.Create(Stream.Null),
                                 actual = XmlReader.Create(Stream.Null))
                {
                    XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder | XmlComparisonFlags.IgnoreElementValues;
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(expectedFlags);

                    Expect.Call(assertionFactory.CreateXmlEquivalencyAssertion(expectedFlags)).Return(assertion);
                    Expect.Call(assertion.AreEquivalent(expected, actual)).Return(new XmlComparisonResult());
                    Mocker.Current.ReplayAll();

                    XmlAssert.Factory = assertionFactory;
                    XmlAssert.AreEquivalent(expectedFlags, expected, actual);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, for a failed assertion.
        /// </summary>
        [Test]
        public void AreEquivalent_AssertFailed()
        {
            With.Mocks(delegate
            {
                using (XmlReader expected = XmlReader.Create(Stream.Null),
                                 actual = XmlReader.Create(Stream.Null))
                {
                    XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder | XmlComparisonFlags.IgnoreElementValues;
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(expectedFlags);

                    Expect.Call(assertionFactory.CreateXmlEquivalencyAssertion(expectedFlags)).Return(assertion);
                    Expect.Call(assertion.AreEquivalent(expected, actual)).Return(CreateFailedComparisonResult());
                    Mocker.Current.ReplayAll();

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
            });
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given only
        /// an XML schema set, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_AssertPassed()
        {
            With.Mocks(delegate
            {
                using (XmlReader actual = XmlReader.Create(Stream.Null))
                {
                    XmlSchemaSet expectedSchemas = new XmlSchemaSet();
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(expectedSchemas);

                    Expect.Call(assertionFactory.CreateXmlValidityAssertion(expectedSchemas)).Return(assertion);
                    Expect.Call(assertion.Validate(actual)).Return(new ValidationEventArgs[0]);
                    Mocker.Current.ReplayAll();

                    XmlAssert.Factory = assertionFactory;
                    XmlAssert.IsValid(expectedSchemas, actual);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given only
        /// an XML schema set, for a failing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_AssertFailed()
        {
            With.Mocks(delegate
            {
                using (XmlReader actual = XmlReader.Create(Stream.Null))
                {
                    XmlSchemaSet expectedSchemas = new XmlSchemaSet();
                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(expectedSchemas);

                    Expect.Call(assertionFactory.CreateXmlValidityAssertion(expectedSchemas)).Return(assertion);
                    Expect.Call(assertion.Validate(actual)).Return(CreateFailedValidationResult());
                    Mocker.Current.ReplayAll();

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
            });
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given an
        /// XML schema set and schema validation flags, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_Flags_AssertPassed()
        {
            With.Mocks(delegate
            {
                using (XmlReader actual = XmlReader.Create(Stream.Null))
                {
                    XmlSchemaSet expectedSchemas = new XmlSchemaSet();
                    XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.ProcessInlineSchema;

                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(expectedSchemas, expectedFlags);

                    Expect.Call(assertionFactory.CreateXmlValidityAssertion(expectedSchemas, expectedFlags)).Return(assertion);
                    Expect.Call(assertion.Validate(actual)).Return(new ValidationEventArgs[0]);
                    Mocker.Current.ReplayAll();

                    XmlAssert.Factory = assertionFactory;
                    XmlAssert.IsValid(expectedSchemas, expectedFlags, actual);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the IsValid() method, when given an
        /// XML schema set and schema validation flags, for a passing assertion.
        /// </summary>
        [Test]
        public void IsValid_Schema_Flags_AssertFailed()
        {
            With.Mocks(delegate
            {
                using (XmlReader actual = XmlReader.Create(Stream.Null))
                {
                    XmlSchemaSet expectedSchemas = new XmlSchemaSet();
                    XmlSchemaValidationFlags expectedFlags = XmlSchemaValidationFlags.ProcessInlineSchema;

                    IAssertionFactory assertionFactory = Mocker.Current.CreateMock<IAssertionFactory>();
                    XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(expectedSchemas, expectedFlags);

                    Expect.Call(assertionFactory.CreateXmlValidityAssertion(expectedSchemas, expectedFlags)).Return(assertion);
                    Expect.Call(assertion.Validate(actual)).Return(CreateFailedValidationResult());
                    Mocker.Current.ReplayAll();

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
            });
        }

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
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { new XmlSchemaException("message") },
                    null) as ValidationEventArgs
            };
        }

        #endregion
    }
}