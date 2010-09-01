// ----------------------------------------------------------------------------
// AssertionResultTestFixture.cs
//
// Contains the definition of the AssertionResultTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 09:07:01
// ----------------------------------------------------------------------------

using Jolt.Testing.Assertions;
using NUnit.Framework;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class AssertionResultTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            AssertionResult result = new AssertionResult();

            Assert.That(result.Result);
            Assert.That(result.Message, Is.Empty);
        }

        [Test]
        public void ExplicitConstruction()
        {
            string expectedMessage = "message";
            AssertionResult result = new AssertionResult(true, expectedMessage);

            Assert.That(result.Result);
            Assert.That(result.Message, Is.SameAs(expectedMessage));
        }
    }
}