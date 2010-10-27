// ----------------------------------------------------------------------------
// EqualityAxiomAssertionTestFixture.cs
//
// Contains the definition of the EqualityAxiomAssertionTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/9/2010 17:58:07
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using Jolt.Reflection;
using Jolt.Testing.Assertions;
using Jolt.Testing.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public class EqualityAxiomAssertionTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            AssertionConstructionTests.EqualityAxiomAssertion<int>(factory => new EqualityAxiomAssertion<int>(factory));
        }

        /// <summary>
        /// Verifies the class structure, by asserting a set of
        /// protected methods to be virtual.
        /// </summary>
        /// 
        /// <remarks>
        /// This test validates an assumption that simplifies implementation
        /// of test fixtures for types derived from <see cref="EqualityAxiomAssertion&lt;T&gt;"/>.
        /// </remarks>
        [Test]
        public void ProtectedVirtualMethods()
        {
            Type genericTypeArgument = typeof(EqualityAxiomAssertion<>).GetGenericArguments()[0];

            Assert.That(GetNonPublicInstanceMethod("AreEqual", genericTypeArgument, genericTypeArgument).IsVirtual);
            Assert.That(GetNonPublicInstanceMethod("GetHashCode", genericTypeArgument).IsVirtual);
        }

        /// <summary>
        /// Verifies the behavior of the IsReflexive() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsReflexive([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instance = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instance.Expect(x => x.Equals(x)).Return(expectedResult);

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsReflexive(instance), Is.EqualTo(expectedResult));

            instance.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsSymmetric() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsSymmetric([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instanceX = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instanceX.Expect(x => x.Equals(instanceY)).Return(true);
            instanceY.Expect(y => y.Equals(instanceX)).Return(expectedResult);

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsSymmetric(instanceX, instanceY), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
            instanceY.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsTransitive() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsTransitive_IfCase([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instanceX = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceZ = new ObjectProxy();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instanceX.Expect(x => x.Equals(instanceY)).Return(true);
            instanceY.Expect(y => y.Equals(instanceZ)).Return(true);
            instanceX.Expect(x => x.Equals(instanceZ)).Return(expectedResult);

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsTransitive(instanceX, instanceY, instanceZ), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
            instanceY.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsTransitive() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsTransitive_OnlyIfCase([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instanceX = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceZ = new ObjectProxy();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instanceX.Expect(x => x.Equals(instanceY)).Return(true);
            instanceY.Expect(y => y.Equals(instanceZ)).Return(expectedResult);
            
            if (expectedResult)
            {
                instanceX.Expect(x => x.Equals(instanceZ)).Return(true);
            }

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsTransitive(instanceX, instanceY, instanceZ), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
            instanceY.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsStateless() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsStateless([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instanceX = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            int numberOfStatelessCalls = expectedResult ? SByte.MaxValue : Rng.Next(1, SByte.MaxValue);
            instanceX.Expect(x => x.Equals(instanceY)).Return(true).Repeat.Times(numberOfStatelessCalls);
            instanceX.Expect(x => x.Equals(instanceY)).Return(expectedResult).Repeat.Once();

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsStateless(instanceX, instanceY), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
        }

        /// <summary>
        /// Veriifes the behavior of the IsNotEqualToNull() method, for value types.
        /// </summary>
        [Test]
        public void IsNotEqualToNull_ValueType()
        {
            IArgumentFactory<DateTime> factory = MockRepository.GenerateStub<IArgumentFactory<DateTime>>();
            EqualityAxiomAssertion<DateTime> assertion = new EqualityAxiomAssertion<DateTime>(factory);
            Assert.That(assertion.IsNotEqualToNull(DateTime.Now));
        }

        /// <summary>
        /// Verifies the behavior of the IsNotEqualToNull() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsNotEqualToNull([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instance = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instance.Expect(x => x.Equals(null)).Return(!expectedResult);

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsNotEqualToNull(instance), Is.EqualTo(expectedResult));

            instance.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsHashCodeConsistent() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsHashCodeConsistent([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instanceX = MockRepository.GenerateMock<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateStub<IArgumentFactory<ObjectProxy>>();

            instanceX.Expect(x => x.Equals(instanceY)).Return(true);
            instanceX.Expect(x => x.GetHashCode()).Return(12345);
            instanceY.Expect(y => y.GetHashCode()).Return(expectedResult ? 12345 : 0);

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsHashCodeConsistent(instanceX, instanceY), Is.EqualTo(expectedResult));

            instanceX.VerifyAllExpectations();
            instanceY.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the IsHashCodeDistinct() method, for a given expected result.
        /// </summary>
        /// 
        /// <param name="expectedResult">
        /// The expected result of the axiom.
        /// </param>
        [Test]
        public void IsHashCodeDistinct([Values(true, false)] bool expectedResult)
        {
            ObjectProxy instance = MockRepository.GenerateMock<ObjectProxy>();
            IArgumentFactory<ObjectProxy> factory = MockRepository.GenerateMock<IArgumentFactory<ObjectProxy>>();

            instance.Expect(x => x.GetHashCode()).Return(12345).Repeat.Once();
            factory.Expect(f => f.Modify(ref instance));
            instance.Expect(x => x.GetHashCode()).Return(expectedResult ? 0 : 12345).Repeat.Once();

            EqualityAxiomAssertion<ObjectProxy> assertion = new EqualityAxiomAssertion<ObjectProxy>(factory);
            Assert.That(assertion.IsHashCodeDistinct(instance), Is.EqualTo(expectedResult));

            instance.VerifyAllExpectations();
            factory.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the reflexivity
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_ReflexivityFailure()
        {
            ObjectProxy instance = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instance));

            assertion.Expect(a => a.IsReflexive(instance)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_ReflexivityAxiom, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the symmetry
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_SymmetryFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_SymmetryAxiom, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the transitivity
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_TransitivityFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_TransitivityAxiom, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the statelessness
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_StatelessnessFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(true);
            assertion.Expect(a => a.IsStateless(instanceX, instanceY)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_StatelessnessAxiom, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the null-inequality
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_NullInequalityFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(true);
            assertion.Expect(a => a.IsStateless(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsNotEqualToNull(instanceX)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_NullInequality, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the hash-code consistency
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_ConsistentHashCodeFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(true);
            assertion.Expect(a => a.IsStateless(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsNotEqualToNull(instanceX)).Return(true);
            assertion.Expect(a => a.IsHashCodeConsistent(instanceX, instanceY)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_HashCodeInconsistent, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when the distinct hash-code
        /// axiom fails.
        /// </summary>
        [Test]
        public void Validate_DistinctHashCodeFailure()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(true);
            assertion.Expect(a => a.IsStateless(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsNotEqualToNull(instanceX)).Return(true);
            assertion.Expect(a => a.IsHashCodeConsistent(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsHashCodeDistinct(instanceX)).Return(false);

            AssertionResult result = assertion.Validate();

            Assert.That(!result.Result);
            Assert.That(result.Message, Is.EqualTo(String.Format(Resources.AssertionFailure_Equality_HashCodeNotModified, typeof(ObjectProxy).ToString())));

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Validate() method, when all axioms pass.
        /// </summary>
        [Test]
        public void Validate()
        {
            ObjectProxy instanceX = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceY = MockRepository.GenerateStub<ObjectProxy>();
            ObjectProxy instanceZ = MockRepository.GenerateStub<ObjectProxy>();
            EqualityAxiomAssertion<ObjectProxy> assertion =
                MockRepository.GeneratePartialMock<EqualityAxiomAssertion<ObjectProxy>>(CreateFactory(instanceX, instanceY, instanceZ));

            assertion.Expect(a => a.IsReflexive(instanceX)).Return(true);
            assertion.Expect(a => a.IsSymmetric(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsTransitive(instanceX, instanceY, instanceZ)).Return(true);
            assertion.Expect(a => a.IsStateless(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsNotEqualToNull(instanceX)).Return(true);
            assertion.Expect(a => a.IsHashCodeConsistent(instanceX, instanceY)).Return(true);
            assertion.Expect(a => a.IsHashCodeDistinct(instanceX)).Return(true);

            AssertionResult result = assertion.Validate();

            Assert.That(result.Result);
            Assert.That(result.Message, Is.Empty);

            assertion.VerifyAllExpectations();
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates an <see cref="IArgumentFactory&lt;T&gt;"/> that returns the
        /// given parameters on each Create() call.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the returned argument.
        /// </typeparam>
        /// 
        /// <param name="instancesToReturn">
        /// The instances to return from the Create() method.
        /// </param>
        private static IArgumentFactory<T> CreateFactory<T>(params T[] instancesToReturn)
        {
            IArgumentFactory<T> factory = MockRepository.GenerateMock<IArgumentFactory<T>>();

            for (int i = 0; i < instancesToReturn.Length; ++i)
            {
                factory.Expect(f => f.Create()).Return(instancesToReturn[i]).Repeat.Once();
            }

            return factory;
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        private static bool RandomBoolean()
        {
            return Rng.Next() % 2 == 0;
        }

        /// <summary>
        /// Returns a <see cref="System.Reflection.MethodInfo"/> representing a
        /// non-public instance method of <see cref="EqualityAxiomAssertion&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the method to retrieve.
        /// </param>
        /// 
        /// <param name="args">
        /// The types of the method's arguments.
        /// </param>
        /// 
        /// <returns>
        /// Returns a <see cref="System.Reflection.MethodInfo"/> representing the
        /// requested method if the method was found with the given parameters.
        /// Returns null otherwise.
        /// </returns>
        private static MethodInfo GetNonPublicInstanceMethod(string name, params Type[] args)
        {
            return typeof(EqualityAxiomAssertion<>).GetMethod(name, CompoundBindingFlags.NonPublicInstance, null, args, null);
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly Random Rng = new Random();

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        public class ObjectProxy
        {
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion
    }
}