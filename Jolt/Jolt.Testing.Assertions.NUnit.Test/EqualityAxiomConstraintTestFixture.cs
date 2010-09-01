// ----------------------------------------------------------------------------
// EqualityAxiomConstraintTestFixture.cs
//
// Contains the definition of the EqualityAxiomConstraintTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/29/2010 12:33:07
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.Assertions.NUnit.Properties;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class EqualityAxiomConstraintTestFixture
    {
        /// <summary>
        /// Verifies the public construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            ConstraintConstructionTests.EqualityAxiomConstraint<int>(factory => new EqualityAxiomConstraint<int>(factory));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            EqualityAxiomAssertion<int> assertion = new EqualityAxiomAssertion<int>(null);
            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);

            Assert.That(constraint.Assertion, Is.SameAs(assertion));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(null as IArgumentFactory<int>);

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
            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(null as IArgumentFactory<int>);

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
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(null as IArgumentFactory<int>);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);
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
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(null as IArgumentFactory<int>);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));

            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);
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
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(null as IArgumentFactory<int>);

            Exception expectedException = new InvalidOperationException();
            assertion.Expect(a => a.Validate()).Throw(expectedException);

            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);
            Assert.That(() => constraint.Matches(typeof(int)), Throws.Exception.SameAs(expectedException));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test]
        public void WriteDescriptionTo()
        {
            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(null as IArgumentFactory<int>);
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            Assert.That(() => constraint.WriteDescriptionTo(writer), Throws.InstanceOf<NotImplementedException>());
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(null as IArgumentFactory<int>);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));
            writer.Expect(w => w.WriteLine("assertion-failure"));

            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);
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
            EqualityAxiomAssertion<int> assertion = MockRepository.GenerateMock<EqualityAxiomAssertion<int>>(null as IArgumentFactory<int>);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());
            writer.Expect(w => w.WriteActualValue(typeof(int)));

            EqualityAxiomConstraint<int> constraint = new EqualityAxiomConstraint<int>(assertion);
            constraint.Matches(typeof(int));
            constraint.WriteActualValueTo(writer);

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }
    }
}