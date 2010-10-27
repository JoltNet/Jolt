// ----------------------------------------------------------------------------
// EquatableAxiomAssertionTestFixture.cs
//
// Contains the definition of the EquatableAxiomAssertionTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/11/2010 21:30:46
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using Jolt.Reflection;
using Jolt.Testing.Assertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.Assertions
{
    using BaseAssertionType = EqualityAxiomAssertion<EquatableAxiomAssertionTestFixture.Equatable>;


    [TestFixture]
    public sealed class EquatableAxiomAssertionTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            AssertionConstructionTests.EquatableAxiomAssertion<int>(factory => new EquatableAxiomAssertion<int>(factory)); 
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
            IEquatableFactory<Equatable> factory = MockRepository.GenerateStub<IEquatableFactory<Equatable>>();

            Equatable instanceX = MockRepository.GenerateMock<Equatable>();
            Equatable instanceY = new Equatable();

            instanceX.Expect(x => x.Equals(instanceY)).Return(expectedResult);

            BaseAssertionType assertion = new EquatableAxiomAssertion<Equatable>(factory);
            MethodInfo areEqual = assertion.GetType().GetMethod("AreEqual", CompoundBindingFlags.NonPublicInstance);

            Assert.That((bool)areEqual.Invoke(assertion, new[] { instanceX, instanceY }), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
        }

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        public class Equatable : IEquatable<Equatable>
        {
            public virtual bool Equals(Equatable other)
            {
                return base.Equals(other);
            }
        }

        #endregion
    }
}