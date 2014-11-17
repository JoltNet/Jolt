// ----------------------------------------------------------------------------
// XmlValidityConstraintTestFixture.cs
//
// Contains the definition of the XmlValidityConstraintTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/7/2009 16:04:49
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Jolt.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class XmlValidityConstraintTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class, when given an XmlSchemaSet.
        /// </summary>
        [Test]
        public void Construction_Schemas()
        {
            ConstraintConstructionTests.XmlValidityConstraint_Schemas(schemas => new XmlValidityConstraint(schemas));
        }

        /// <summary>
        /// Verifies the construction of the class, when given an XmlSchemaSet
        /// and an XmlSchemaValidationFlags instance.
        /// </summary>
        [Test]
        public void Construction_Schemas_Flags()
        {
            ConstraintConstructionTests.XmlValidityConstraint_Schemas_Flags((schemas, flags) => new XmlValidityConstraint(schemas, flags));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            XmlValidityAssertion expectedAssertion = new XmlValidityAssertion(new XmlSchemaSet());
            XmlValidityConstraint constraint = new XmlValidityConstraint(expectedAssertion);

            Assert.That(constraint.Assertion, Is.SameAs(expectedAssertion));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method, when
        /// given an object of an invalid type.
        /// </summary>
        [Test]
        public void Matches_InvalidType()
        {
            XmlValidityConstraint constraint = new XmlValidityConstraint(new XmlSchemaSet());
            Assert.That(!constraint.Matches(String.Empty));
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the validation succeeds.
        /// </summary>
        [Test]
        public void Matches()
        {
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(new XmlSchemaSet());
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
            {
                assertion.Expect(a => a.Validate(expectedReader)).Return(new ValidationEventArgs[0]);

                XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                Assert.That(constraint.Matches(expectedReader));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the validation fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(new XmlSchemaSet());
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
            {
                assertion.Expect(a => a.Validate(expectedReader)).Return(CreateFailedComparisonResult());

                XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                Assert.That(!constraint.Matches(expectedReader));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test]
        public void Matches_UnexpectedException()
        {
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(new XmlSchemaSet());
            using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
            {
                Exception expectedException = new InvalidProgramException();
                assertion.Expect(a => a.Validate(expectedReader)).Throw(expectedException);

                XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);

                Assert.That(
                    () => constraint.Matches(expectedReader),
                    Throws.Exception.SameAs(expectedException));
            }

            assertion.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void WriteDescriptionTo()
        {
            MessageWriter writer = MockRepository.GenerateStub<MessageWriter>();

            XmlValidityConstraint constraint = new XmlValidityConstraint(new XmlSchemaSet());
            constraint.WriteDescriptionTo(writer);
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(new XmlSchemaSet());
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
            {
                IList<ValidationEventArgs> assertionResult = CreateFailedComparisonResult();
                assertion.Expect(a => a.Validate(expectedReader)).Return(assertionResult);

                writer.Expect(w => w.WriteLine("message"));

                XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                constraint.Matches(expectedReader);
                constraint.WriteMessageTo(writer);
            }

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            XmlValidityAssertion assertion = MockRepository.GenerateMock<XmlValidityAssertion>(new XmlSchemaSet());
            MessageWriter writer = MockRepository.GenerateMock<MessageWriter>();

            using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
            {
                writer.Expect(w => w.WriteActualValue(expectedReader));
                assertion.Expect(a => a.Validate(expectedReader)).Return(new ValidationEventArgs[0]);

                XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                constraint.Matches(expectedReader);
                constraint.WriteActualValueTo(writer);
            }

            assertion.VerifyAllExpectations();
            writer.VerifyAllExpectations();
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a single-element collection of <see cref="ValidationEventArgs"/> representing a failed comparison.
        /// </summary>
        private static IList<ValidationEventArgs> CreateFailedComparisonResult()
        {
            return new ValidationEventArgs[] {
                Activator.CreateInstance(
                    typeof(ValidationEventArgs),
                    CompoundBindingFlags.NonPublicInstance,
                    null,
                    new object[] { new XmlSchemaException("message") },
                    null) as ValidationEventArgs
            };
        }

        #endregion
    }
}