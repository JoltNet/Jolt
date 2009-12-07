// ----------------------------------------------------------------------------
// XmlEqualityAssertionTestFixture.cs
//
// Contains the definition of the XmlEqualityAssertionTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 5/29/2009 19:12:47
// ----------------------------------------------------------------------------

using System.IO;
using System.Xml;

using Jolt.Testing.Assertions;
using NUnit.Framework;
using Rhino.Mocks;

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
            AssertionConstructionTests.XmlEqualityAssertion(() => new XmlEqualityAssertion());
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
            XmlEquivalencyAssertion equivalencyAssertion = MockRepository.GenerateMock<XmlEquivalencyAssertion>(XmlComparisonFlags.Strict);

            XmlReader expectedXml = XmlReader.Create(Stream.Null);
            XmlReader actualXml = XmlReader.Create(Stream.Null);
            XmlComparisonResult expectedResult = new XmlComparisonResult();
            equivalencyAssertion.Expect(a => a.AreEquivalent(expectedXml, actualXml)).Return(expectedResult);

            XmlEqualityAssertion assertion = new XmlEqualityAssertion(equivalencyAssertion);
            Assert.That(assertion.AreEqual(expectedXml, actualXml), Is.SameAs(expectedResult));

            equivalencyAssertion.VerifyAllExpectations();
        }
    }
}