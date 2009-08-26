// ----------------------------------------------------------------------------
// XmlEqualityAssertionTestFixture.cs
//
// Contains the definition of the XmlEqualityAssertionTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 5/29/2009 19:12:47
// ----------------------------------------------------------------------------

using Rhino.Mocks;

using Jolt.Testing.Assertions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Xml;
using System.IO;
using Jolt.Testing.Properties;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class XmlEqualityAssertionTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            XmlEqualityAssertion assertion = new XmlEqualityAssertion();
            Assert.That(assertion.EquivalencyAssertion, Is.Not.Null);
            Assert.That(assertion.EquivalencyAssertion.ComparisonFlags, Is.EqualTo(XmlComparisonFlags.Strict));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void InternalConstruction()
        {
            XmlEquivalencyAssertion equivalencyAssertion = new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder);
            XmlEqualityAssertion assertion = new XmlEqualityAssertion(equivalencyAssertion);

            Assert.That(assertion.EquivalencyAssertion, Is.SameAs(equivalencyAssertion));
        }

        /// <summary>
        /// Verifies the behavior of the AreEqual() method.
        /// </summary>
        [Test]
        public void AreEqual()
        {
            With.Mocks(delegate
            {
                XmlEquivalencyAssertion equivalencyAssertion = Mocker.Current.CreateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

                // Expectations
                // AreEquals() immediately forwards to "strict" AreEquivalent() method.
                XmlReader expectedXml = XmlReader.Create(Stream.Null);
                XmlReader actualXml = XmlReader.Create(Stream.Null);
                XmlComparisonResult expectedResult = new XmlComparisonResult();

                Expect.Call(equivalencyAssertion.AreEquivalent(expectedXml, actualXml))
                    .Return(expectedResult);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlEqualityAssertion assertion = new XmlEqualityAssertion(equivalencyAssertion);
                Assert.That(assertion.AreEqual(expectedXml, actualXml), Is.SameAs(expectedResult));
            });
        }
    }
}