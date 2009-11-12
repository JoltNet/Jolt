// ----------------------------------------------------------------------------
// XmlEquivalencyConstraintTestFixture.cs
//
// Contains the definition of the XmlEquivalencyConstraintTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/8/2009 18:53:31
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
    using CreateXmlEquivalencyAssertionDelegate = Func<XmlComparisonFlags, XmlEquivalencyAssertion>;


    [TestFixture]
    public sealed class XmlEquivalencyConstraintTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the public construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            ConstraintConstructionTests.XmlEquivalencyConstraint(xml => new XmlEquivalencyConstraint(xml));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateStub<CreateXmlEquivalencyAssertionDelegate>();

            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedXml, createAssertion);

            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
            Assert.That(constraint.CreateAssertion, Is.SameAs(createAssertion));
            Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            Assert.That(!constraint.Matches(String.Empty));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the contraint assertion succeeds.
        /// </summary>
        [Test]
        public void Matches()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                createAssertion.Expect(ca => ca(XmlComparisonFlags.Strict)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Return(new XmlComparisonResult());

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                Assert.That(constraint.Matches(actualReader));
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when the constraint
        /// is intialized with a non-default comparison flag.
        /// </summary>
        [Test]
        public void Matches_WithComparisonFlags()
        {
            XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder |
                XmlComparisonFlags.IgnoreAttributes |
                XmlComparisonFlags.IgnoreElementValues;

            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(expectedFlags);

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                createAssertion.Expect(ca => ca(expectedFlags)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Return(new XmlComparisonResult());

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion)
                    .IgnoreAttributes
                    .IgnoreElementValues
                    .IgnoreSequenceOrder;

                Assert.That(constraint.Matches(actualReader));
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the constraint assertion fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                createAssertion.Expect(ca => ca(XmlComparisonFlags.Strict)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Return(CreateFailedComparisonResult());

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                Assert.That(!constraint.Matches(actualReader));
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test]
        public void Matches_UnexpectedException()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                Exception expectedException = new InvalidProgramException();
                createAssertion.Expect(ca => ca(XmlComparisonFlags.Strict)).Return(assertion);
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Throw(expectedException);

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);

                Assert.That(
                    () => constraint.Matches(actualReader),
                    Throws.Exception.SameAs(expectedException));
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void WriteDescriptionTo()
        {
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            constraint.WriteDescriptionTo(writer);
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                createAssertion.Expect(ca => ca(XmlComparisonFlags.Strict)).Return(assertion);

                XmlComparisonResult assertionResult = CreateFailedComparisonResult();
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Return(assertionResult);

                writer.Expect(w => w.WriteLine("message\r\nXPath: /ns:element"));

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                constraint.Matches(actualReader);
                constraint.WriteMessageTo(writer);
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            CreateXmlEquivalencyAssertionDelegate createAssertion = MockRepository.GenerateMock<CreateXmlEquivalencyAssertionDelegate>();
            XmlEquivalencyAssertion assertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                             actualReader = XmlReader.Create(Stream.Null))
            {
                createAssertion.Expect(ca => ca(XmlComparisonFlags.Strict)).Return(assertion);
                writer.Expect(w => w.WriteActualValue(actualReader));
                assertion.Expect(a => a.AreEquivalent(expectedReader, actualReader)).Return(new XmlComparisonResult());

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                constraint.Matches(actualReader);
                constraint.WriteActualValueTo(writer);
            }

            createAssertion.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IgnoreAttributeNamesapce property.
        /// </summary>
        [Test]
        public void IgnoreAttributeNamespace()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            XmlEquivalencyConstraint newConstraint = constraint.IgnoreAttributeNamespaces;

            Assert.That(constraint, Is.SameAs(newConstraint));
            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.IgnoreAttributeNamespaces));
        }

        /// <summary>
        /// Verifies the behavior of the IgnoreAttributes property.
        /// </summary>
        [Test]
        public void IgnoreAttributes()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            XmlEquivalencyConstraint newConstraint = constraint.IgnoreAttributes;

            Assert.That(constraint, Is.SameAs(newConstraint));
            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.IgnoreAttributes));
        }

        /// <summary>
        /// Verifies the behavior of the IgnoreElementNamespaces property.
        /// </summary>
        [Test]
        public void IgnoreElementNamespaces()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            XmlEquivalencyConstraint newConstraint = constraint.IgnoreElementNamespaces;

            Assert.That(constraint, Is.SameAs(newConstraint));
            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.IgnoreElementNamespaces));
        }

        /// <summary>
        /// Verifies the behavior of the IgnoreElementValues property.
        /// </summary>
        [Test]
        public void IgnoreElementValues()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            XmlEquivalencyConstraint newConstraint = constraint.IgnoreElementValues;

            Assert.That(constraint, Is.SameAs(newConstraint));
            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.IgnoreElementValues));
        }

        /// <summary>
        /// Verifies the behavior of the IgnoreSequenceOrder property.
        /// </summary>
        [Test]
        public void IgnoreSequenceOrder()
        {
            XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
            XmlEquivalencyConstraint newConstraint = constraint.IgnoreSequenceOrder;

            Assert.That(constraint, Is.SameAs(newConstraint));
            Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.IgnoreSequenceOrder));
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