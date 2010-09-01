// ----------------------------------------------------------------------------
// AxiomAssertTestFixture.cs
//
// Contains the definition of the AxiomAssertTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/23/2010 21:24:08
// ----------------------------------------------------------------------------

using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;
using MVTU = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jolt.Testing.Assertions.VisualStudio.Test
{
    [TestFixture]
    public sealed class AxiomAssertTestFixture
    {
        /// <summary>
        /// Verifies the default static initialization of the class.
        /// </summary>
        [Test]
        public void StaticConstruction()
        {
            // Re-execute the static constructor to guarantee that all default static state
            // is present for this test.
            typeof(AxiomAssert).TypeInitializer.Invoke(null, null);
            Assert.That(AxiomAssert.Factory, Is.Not.Null);
            Assert.That(AxiomAssert.Factory, Is.InstanceOf<AssertionFactory>());
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method for a passing assertion.
        /// </summary>
        [Test]
        public void Equality_AssertPassed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IArgumentFactory<int> argFactory = MockRepository.GenerateStub<IArgumentFactory<int>>();
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateEqualityAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            AxiomAssert.Factory = factory;
            AxiomAssert.Equality(argFactory);

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method for a failing assertion.
        /// </summary>
        [Test]
        public void Equality_AssertFailed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IArgumentFactory<int> argFactory = MockRepository.GenerateStub<IArgumentFactory<int>>();
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateEqualityAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(CreateFailedAssertionResult());

            AxiomAssert.Factory = factory;

            try
            {
                AxiomAssert.Equality(argFactory);
                Assert.Fail();
            }
            catch (MVTU.AssertFailedException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("message"));
            }

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for equatable implementations and a passing assertion.
        /// </summary>
        [Test]
        public void Equatable_AssertPassed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IEquatableFactory<int> argFactory = MockRepository.GenerateStub<IEquatableFactory<int>>();
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateEquatableAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            AxiomAssert.Factory = factory;
            AxiomAssert.Equality(argFactory);

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for equatable implementations and a failing assertion.
        /// </summary>
        [Test]
        public void Equatable_AssertFailed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IEquatableFactory<int> argFactory = MockRepository.GenerateStub<IEquatableFactory<int>>();
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateEquatableAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(CreateFailedAssertionResult());

            AxiomAssert.Factory = factory;

            try
            {
                AxiomAssert.Equality(argFactory);
                Assert.Fail();
            }
            catch (MVTU.AssertFailedException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("message"));
            }

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for comparable implementations and a passing assertion.
        /// </summary>
        [Test]
        public void Comparable_AssertPassed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IComparableFactory<int> argFactory = MockRepository.GenerateStub<IComparableFactory<int>>();
            ComparableAxiomAssertion<int> assertion = MockRepository.GenerateMock<ComparableAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateComparableAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            AxiomAssert.Factory = factory;
            AxiomAssert.Equality(argFactory);

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for comparable implementations and a failing assertion.
        /// </summary>
        [Test]
        public void Comparable_AssertFailed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IComparableFactory<int> argFactory = MockRepository.GenerateStub<IComparableFactory<int>>();
            ComparableAxiomAssertion<int> assertion = MockRepository.GenerateMock<ComparableAxiomAssertion<int>>(argFactory);

            factory.Expect(f => f.CreateComparableAxiomAssertion(argFactory)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(CreateFailedAssertionResult());

            AxiomAssert.Factory = factory;

            try
            {
                AxiomAssert.Equality(argFactory);
                Assert.Fail();
            }
            catch (MVTU.AssertFailedException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("message"));
            }

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for equality-comparer implementations and a passing assertion.
        /// </summary>
        [Test]
        public void EqualityComparer_AssertPassed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IArgumentFactory<int> argFactory = MockRepository.GenerateStub<IArgumentFactory<int>>();
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(argFactory, EqualityComparer<int>.Default);

            factory.Expect(f => f.CreateEqualityComparerAxiomAssertion(argFactory, EqualityComparer<int>.Default)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            AxiomAssert.Factory = factory;
            AxiomAssert.Equality(argFactory, EqualityComparer<int>.Default);

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the implementation of the Equality() method,
        /// for equality-comparer implementations and a passing assertion.
        /// </summary>
        [Test]
        public void EqualityComparer_AssertFailed()
        {
            IAssertionFactory factory = MockRepository.GenerateMock<IAssertionFactory>();
            IArgumentFactory<int> argFactory = MockRepository.GenerateStub<IArgumentFactory<int>>();
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(argFactory, EqualityComparer<int>.Default);

            factory.Expect(f => f.CreateEqualityComparerAxiomAssertion(argFactory, EqualityComparer<int>.Default)).Return(assertion);
            assertion.Expect(a => a.Validate()).Return(CreateFailedAssertionResult());

            AxiomAssert.Factory = factory;

            try
            {
                AxiomAssert.Equality(argFactory, EqualityComparer<int>.Default);
                Assert.Fail();
            }
            catch (MVTU.AssertFailedException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("message"));
            }

            factory.VerifyAllExpectations();
            assertion.VerifyAllExpectations();
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates an <see cref="AssertionResult"/> representing a failed assertion.
        /// </summary>
        private static AssertionResult CreateFailedAssertionResult()
        {
            return new AssertionResult(false, "message");
        }

        #endregion
    }
}