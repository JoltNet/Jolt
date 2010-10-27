// ----------------------------------------------------------------------------
// ComparableAxiomAssertionTestFixture.cs
//
// Contains the definition of the ComparableAxiomAssertionTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/12/2010 08:51:56
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using Jolt.Reflection;
using Jolt.Testing.Assertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.Assertions
{
    using BaseAssertionType = EqualityAxiomAssertion<ComparableAxiomAssertionTestFixture.Comparable>;


    [TestFixture]
    public sealed class ComparableAxiomAssertionTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            AssertionConstructionTests.ComparableAxiomAssertion<int>(factory => new ComparableAxiomAssertion<int>(factory));
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
            IComparableFactory<Comparable> factory = MockRepository.GenerateStub<IComparableFactory<Comparable>>();

            Comparable instanceX = MockRepository.GenerateMock<Comparable>();
            Comparable instanceY = new Comparable();

            instanceX.Expect(x => x.CompareTo(instanceY)).Return(expectedResult ? 0 : RandomNonZeroInteger());

            BaseAssertionType assertion = new ComparableAxiomAssertion<Comparable>(factory);
            MethodInfo areEqual = assertion.GetType().GetMethod("AreEqual", CompoundBindingFlags.NonPublicInstance);

            Assert.That((bool)areEqual.Invoke(assertion, new[] { instanceX, instanceY }), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Returns a random, non-zero integer.
        /// </summary>
        private static int RandomNonZeroInteger()
        {
            while (true)
            {
                int result = Rng.Next(Int32.MinValue, Int32.MaxValue);
                if (result != 0) { return result; }
            }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly Random Rng = new Random();

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        public class Comparable : IComparable<Comparable>
        {
            public virtual int CompareTo(Comparable other)
            {
                return 0;
            }
        }

        #endregion
    }
}