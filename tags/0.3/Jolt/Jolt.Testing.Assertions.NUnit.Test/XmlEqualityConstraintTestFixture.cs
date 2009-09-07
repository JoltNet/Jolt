// ----------------------------------------------------------------------------
// XmlEqualityConstraintTestFixture.cs
//
// Contains the definition of the XmlEqualityConstraintTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/8/2009 17:26:05
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class XmlEqualityConstraintTestFixture
    {
        /// <summary>
        /// Verifies the public construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            ConstraintConstructionTests.XmlEqualityConstraint(xml => new XmlEqualityConstraint(xml));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEqualityAssertion expectedAssertion = new XmlEqualityAssertion();
            XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedXml, expectedAssertion);

            Assert.That(constraint.Assertion, Is.SameAs(expectedAssertion));
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            XmlEqualityConstraint constraint = new XmlEqualityConstraint(null);
            Assert.That(!constraint.Matches(String.Empty));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the contraint assertion succeeds.
        /// </summary>
        [Test]
        public void Matches()
        {
            With.Mocks(delegate
            {
                XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML reader pair are equal.
                    Expect.Call(assertion.AreEqual(expectedReader, actualReader))
                        .Return(new XmlComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                    Assert.That(constraint.Matches(actualReader));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the constraint assertion fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            With.Mocks(delegate
            {
                XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML reader pair are not equal.
                    Expect.Call(assertion.AreEqual(expectedReader, actualReader))
                        .Return(CreateFailedComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                    Assert.That(!constraint.Matches(actualReader));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidProgramException))]
        public void Matches_UnexpectedException()
        {
            With.Mocks(delegate
            {
                XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML reader pair are not equal.
                    assertion.AreEqual(expectedReader, actualReader);
                    LastCall.Throw(new InvalidProgramException());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                    constraint.Matches(actualReader);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void WriteDescriptionTo()
        {
            With.Mocks(delegate
            {
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();
                Mocker.Current.ReplayAll();

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(null);
                constraint.WriteDescriptionTo(writer);
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            With.Mocks(delegate
            {
                XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML reader pair are not equal.
                    XmlComparisonResult assertionResult = CreateFailedComparisonResult();
                    Expect.Call(assertion.AreEqual(expectedReader, actualReader))
                        .Return(assertionResult);                    

                    // The message writer receives the error message.
                    writer.WriteLine("message\r\nXPath: /ns:element");

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                    constraint.Matches(actualReader);
                    constraint.WriteMessageTo(writer);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            With.Mocks(delegate
            {
                XmlEqualityAssertion assertion = Mocker.Current.CreateMock<XmlEqualityAssertion>();
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The message writer receives the "actual" parameter.
                    writer.WriteActualValue(actualReader);
                    Expect.Call(assertion.AreEqual(expectedReader, actualReader))
                        .Return(new XmlComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                    constraint.Matches(actualReader);
                    constraint.WriteActualValueTo(writer);
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

        #endregion
    }
}