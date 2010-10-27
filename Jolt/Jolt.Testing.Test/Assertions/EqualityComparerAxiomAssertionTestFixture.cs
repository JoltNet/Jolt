// ----------------------------------------------------------------------------
// EqualityComparerAxiomAssertionTestFixture.cs
//
// Contains the definition of the EqualityComparerAxiomAssertionTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/12/2010 08:59:04
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

using Jolt.Reflection;
using Jolt.Testing.Assertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.Assertions
{
    using BaseAssertionType = EqualityAxiomAssertion<DateTime>;


    [TestFixture]
    public sealed class EqualityComparerAxiomAssertionTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            AssertionConstructionTests.EqualityComparerAxiomAssertion<int>(
                (factory, comparer) => new EqualityComparerAxiomAssertion<int>(factory, comparer));
        }

        /// <summary>
        /// Verifies the overriden implementation of the AreEqual() method.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the AreEqual() method.
        /// </param>
        [Test]
        public void AreEqual([Values(true, false)] bool expectedResult)
        {
            IArgumentFactory<DateTime> factory = MockRepository.GenerateStub<IArgumentFactory<DateTime>>();
            IEqualityComparer<DateTime> comparer = MockRepository.GenerateMock<IEqualityComparer<DateTime>>();

            DateTime instanceX = DateTime.Now;
            DateTime instanceY = DateTime.Now.AddDays(-123);
            comparer.Expect(c => c.Equals(instanceX, instanceY)).Return(expectedResult);

            BaseAssertionType assertion = new EqualityComparerAxiomAssertion<DateTime>(factory, comparer);
            MethodInfo areEqual = assertion.GetType().GetMethod("AreEqual", CompoundBindingFlags.NonPublicInstance);

            Assert.That((bool)areEqual.Invoke(assertion, new object[] { instanceX, instanceY }), Is.EqualTo(expectedResult));

            comparer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the overriden implementation of the GetHashCode() method.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the GetHashCode() method.
        /// </param>
        [Test]
        public void GetHashCode([Random(Int32.MinValue, Int32.MaxValue, 10)] int expectedHashCode)
        {
            IArgumentFactory<DateTime> factory = MockRepository.GenerateStub<IArgumentFactory<DateTime>>();
            IEqualityComparer<DateTime> comparer = MockRepository.GenerateMock<IEqualityComparer<DateTime>>();

            DateTime instanceX = DateTime.Now;
            comparer.Expect(c => c.GetHashCode(instanceX)).Return(expectedHashCode);

            BaseAssertionType assertion = new EqualityComparerAxiomAssertion<DateTime>(factory, comparer);
            MethodInfo getHashCode = assertion.GetType().GetMethod("GetHashCode", CompoundBindingFlags.NonPublicInstance);

            Assert.That((int)getHashCode.Invoke(assertion, new object[] { instanceX }), Is.EqualTo(expectedHashCode));

            comparer.VerifyAllExpectations();
        }
    }
}