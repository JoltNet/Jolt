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
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    using CreateXmlEquivalencyAssertionDelegate = Func<XmlComparisonFlags, XmlEquivalencyAssertion>;


    [TestFixture]
    public sealed class XmlEquivalencyConstraintTestFixture
    {
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
            With.Mocks(delegate
            {
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                Mocker.Current.ReplayAll();

                XmlReader expectedXml = XmlReader.Create(Stream.Null);
                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedXml, createAssertion);

                Assert.That(constraint.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
                Assert.That(constraint.CreateAssertion, Is.SameAs(createAssertion));
                Assert.That(constraint.ExpectedXml, Is.SameAs(expectedXml));
            });
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
            With.Mocks(delegate
            {
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(XmlComparisonFlags.Strict)).Return(assertion);

                    // The XML reader pair are equivalent.
                    Expect.Call(assertion.AreEquivalent(expectedReader, actualReader))
                        .Return(new XmlComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                    Assert.That(constraint.Matches(actualReader));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when the constraint
        /// is intialized with a non-default comparison flag.
        /// </summary>
        [Test]
        public void Matches_WithComparisonFlags()
        {
            With.Mocks(delegate
            {
                XmlComparisonFlags expectedFlags = XmlComparisonFlags.IgnoreSequenceOrder |
                    XmlComparisonFlags.IgnoreAttributes |
                    XmlComparisonFlags.IgnoreElementValues;

                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(expectedFlags);

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(expectedFlags)).Return(assertion);

                    // The XML reader pair are equivalent.
                    Expect.Call(assertion.AreEquivalent(expectedReader, actualReader))
                        .Return(new XmlComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion)
                        .IgnoreAttributes
                        .IgnoreElementValues
                        .IgnoreSequenceOrder;

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
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(XmlComparisonFlags.Strict)).Return(assertion);

                    // The XML reader pair are not equivalent.
                    Expect.Call(assertion.AreEquivalent(expectedReader, actualReader))
                        .Return(CreateFailedComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
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
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(XmlComparisonFlags.Strict)).Return(assertion);

                    // The XML reader pair are not equivalent.
                    assertion.AreEquivalent(expectedReader, actualReader);
                    LastCall.Throw(new InvalidProgramException());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
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

                XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(null);
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
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(XmlComparisonFlags.Strict)).Return(assertion);

                    // The XML reader pair are not equal.
                    XmlComparisonResult assertionResult = CreateFailedComparisonResult();
                    Expect.Call(assertion.AreEquivalent(expectedReader, actualReader))
                        .Return(assertionResult);

                    // The message writer receives the error message.
                    writer.WriteLine("message\r\nXPath: /ns:element");

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
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
                CreateXmlEquivalencyAssertionDelegate createAssertion = Mocker.Current.CreateMock<CreateXmlEquivalencyAssertionDelegate>();
                XmlEquivalencyAssertion assertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null),
                                 actualReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The XML eqivalency assertion is created.
                    Expect.Call(createAssertion(XmlComparisonFlags.Strict)).Return(assertion);

                    // The message writer receives the "actual" parameter.
                    writer.WriteActualValue(actualReader);
                    Expect.Call(assertion.AreEquivalent(expectedReader, actualReader))
                        .Return(new XmlComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlEquivalencyConstraint constraint = new XmlEquivalencyConstraint(expectedReader, createAssertion);
                    constraint.Matches(actualReader);
                    constraint.WriteActualValueTo(writer);
                }
            });
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