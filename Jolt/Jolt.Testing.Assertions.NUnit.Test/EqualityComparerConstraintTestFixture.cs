// ----------------------------------------------------------------------------
// EqualityComparerConstraintTestFixture.cs
//
// Contains the definition of the EqualityComparerConstraintTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/31/2010 20:42:05
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.Assertions.NUnit.Properties;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class EqualityComparerConstraintTestFixture
    {
        /// <summary>
        /// Verifies the public construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            ConstraintConstructionTests.EqualityComparerAxiomConstraint<int>(
                (factory, comparer) => new EqualityComparerAxiomConstraint<int>(factory, comparer));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            EqualityComparerAxiomAssertion<int> assertion = new EqualityComparerAxiomAssertion<int>(null, null);
            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);

            Assert.That(constraint.Assertion, Is.SameAs(assertion));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(null, null);

            Assert.That(!constraint.Matches(String.Empty));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an <see cref="System.Type"/> object that mismatches
        /// the specialization of the constraint.
        /// </summary>
        [Test]
        public void Matches_ArgumentTypeMismatch()
        {
            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(null, null);

            Exception ex = Assert.Throws<ArgumentException>(() => constraint.Matches(typeof(string)));
            Assert.That(ex.Message, Is.EqualTo(String.Format(Resources.AxiomConstraintFailure_TypeMismatch, "System.String", "System.Int32")));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the contraint assertion succeeds.
        /// </summary>
        [Test]
        public void Matches()
        {
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(null, null);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);
            Assert.That(constraint.Matches(typeof(int)));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the constraint assertion fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(null, null);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));

            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);
            Assert.That(!constraint.Matches(typeof(int)));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test]
        public void Matches_UnexpectedException()
        {
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(null, null);

            Exception expectedException = new InvalidOperationException();
            assertion.Expect(a => a.Validate()).Throw(expectedException);

            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);
            Assert.That(() => constraint.Matches(typeof(int)), Throws.Exception.SameAs(expectedException));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test]
        public void WriteDescriptionTo()
        {
            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(null, null);
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            Assert.That(() => constraint.WriteDescriptionTo(writer), Throws.InstanceOf<NotImplementedException>());
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(null, null);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));
            writer.Expect(w => w.WriteLine("assertion-failure"));

            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);
            constraint.Matches(typeof(int));
            constraint.WriteMessageTo(writer);

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            EqualityComparerAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityComparerAxiomAssertion<int>>(null, null);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());
            writer.Expect(w => w.WriteActualValue(typeof(int)));

            EqualityComparerAxiomConstraint<int> constraint = new EqualityComparerAxiomConstraint<int>(assertion);
            constraint.Matches(typeof(int));
            constraint.WriteActualValueTo(writer);

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }
    }
}