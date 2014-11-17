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
using NUnit.Framework.Constraints;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class XmlEqualityConstraintTestFixture
    {
        #region public methods --------------------------------------------------------------------

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
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                assertion.Expect(a => a.AreEqual(expectedReader, actualReader)).Return(new XmlComparisonResult());

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                Assert.That(constraint.Matches(actualReader));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the constraint assertion fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                assertion.Expect(a => a.AreEqual(expectedReader, actualReader)).Return(CreateFailedComparisonResult());

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                Assert.That(!constraint.Matches(actualReader));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test]
        public void Matches_UnexpectedException()
        {
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                Exception expectedException = new InvalidProgramException();
                assertion.Expect(a => a.AreEqual(expectedReader, actualReader)).Throw(expectedException);

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);

                Assert.That(
                    () => constraint.Matches(actualReader),
                    Throws.Exception.SameAs(expectedException));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void WriteDescriptionTo()
        {
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            XmlEqualityConstraint constraint = new XmlEqualityConstraint(null);
            constraint.WriteDescriptionTo(writer);
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                XmlComparisonResult assertionResult = CreateFailedComparisonResult();
                assertion.Expect(a => a.AreEqual(expectedReader, actualReader)).Return(assertionResult);                    
                writer.Expect(w => w.WriteLine("message\r\nXPath: /ns:element"));

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                constraint.Matches(actualReader);
                constraint.WriteMessageTo(writer);
            }

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            XmlEqualityAssertion assertion = MockRepository.GenerateMock<XmlEqualityAssertion>();
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                writer.Expect(w => w.WriteActualValue(actualReader));
                assertion.Expect(a => a.AreEqual(expectedReader, actualReader)).Return(new XmlComparisonResult());

                XmlEqualityConstraint constraint = new XmlEqualityConstraint(expectedReader, assertion);
                constraint.Matches(actualReader);
                constraint.WriteActualValueTo(writer);
            }

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
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

        #endregion
    }
}