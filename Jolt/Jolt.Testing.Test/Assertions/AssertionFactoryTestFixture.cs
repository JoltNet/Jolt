// ----------------------------------------------------------------------------
// AssertionFactoryTestFixture.cs
//
// Contains the definition of the AssertionFactoryTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 12:18:56
// ----------------------------------------------------------------------------

using Jolt.Testing.Assertions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class AssertionFactoryTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the CreateXmlValidityAsserition() method,
        /// for the overload accepting an XmlSchemaSet.
        /// </summary>
        [Test]
        public void CreateXmlValidityAssertion_Schemas()
        {
            IAssertionFactory factory = new AssertionFactory();
            AssertionConstructionTests.XmlValidityAssertion_Schemas(factory.CreateXmlValidityAssertion);
        }

        /// <summary>
        /// Verifies the behavior of the CreateXmlValidityAsserition() method,
        /// for the overload accepting an XmlSchemaSet and XmlSchemaValidationFlags.
        /// </summary>
        [Test]
        public void CreateXmlValidityAssertion_Schemas_Flags()
        {
            IAssertionFactory factory = new AssertionFactory();
            AssertionConstructionTests.XmlValidityAssertion_Schemas_Flags(factory.CreateXmlValidityAssertion);
        }

        /// <summary>
        /// Verifies the behavior of the CreateXmlEqualityAssertion() method.
        /// </summary>
        [Test]
        public void CreateXmlEqualityAssertion()
        {
            IAssertionFactory factory = new AssertionFactory();
            AssertionConstructionTests.XmlEqualityAssertion(factory.CreateXmlEqualityAssertion);
        }

        /// <summary>
        /// Verifies the behavior of the CreateXmlEquivalencyAssertion() method.
        /// </summary>
        [Test]
        public void CreateXmlEquivalencyAssertion()
        {
            IAssertionFactory factory = new AssertionFactory();
            AssertionConstructionTests.XmlEquivalencyAssertion(factory.CreateXmlEquivalencyAssertion);
        }
    }
}