// ----------------------------------------------------------------------------
// EquatableAxiomConstraintTestFixture.cs
//
// Contains the definition of the EquatableAxiomConstraintTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/31/2010 08:56:54
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.Assertions.NUnit.Properties;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class EquatableAxiomConstraintTestFixture
    {
        /// <summary>
        /// Verifies the public construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            ConstraintConstructionTests.EquatableAxiomConstraint<int>(factory => new EquatableAxiomConstraint<int>(factory));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            EquatableAxiomAssertion<int> assertion = new EquatableAxiomAssertion<int>(null);
            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);

            Assert.That(constraint.Assertion, Is.SameAs(assertion));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(null as IEquatableFactory<int>);

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
            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(null as IEquatableFactory<int>);

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
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(null as IEquatableFactory<int>);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());

            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);
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
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(null as IEquatableFactory<int>);

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));

            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);
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
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(null as IEquatableFactory<int>);

            Exception expectedException = new InvalidOperationException();
            assertion.Expect(a => a.Validate()).Throw(expectedException);

            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);
            Assert.That(() => constraint.Matches(typeof(int)), Throws.Exception.SameAs(expectedException));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test]
        public void WriteDescriptionTo()
        {
            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(null as IEquatableFactory<int>);
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            Assert.That(() => constraint.WriteDescriptionTo(writer), Throws.InstanceOf<NotImplementedException>());
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(null as IEquatableFactory<int>);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult(false, "assertion-failure"));
            writer.Expect(w => w.WriteLine("assertion-failure"));

            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);
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
            EquatableAxiomAssertion<int> assertion = MockRepository.GenerateMock<EquatableAxiomAssertion<int>>(null as IEquatableFactory<int>);
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            assertion.Expect(a => a.Validate()).Return(new AssertionResult());
            writer.Expect(w => w.WriteActualValue(typeof(int)));

            EquatableAxiomConstraint<int> constraint = new EquatableAxiomConstraint<int>(assertion);
            constraint.Matches(typeof(int));
            constraint.WriteActualValueTo(writer);

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }
    }
}