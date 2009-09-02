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
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class XmlValidityConstraintTestFixture
    {
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
            With.Mocks(delegate
            {
                XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(new XmlSchemaSet());
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // A given XmlReader is successfully validated.
                    Expect.Call(assertion.Validate(expectedReader))
                        .Return(new ValidationEventArgs[0]);

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                    Assert.That(constraint.Matches(expectedReader));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when the validation fails.
        /// </summary>
        [Test]
        public void Matches_DoesNotMatch()
        {
            With.Mocks(delegate
            {
                XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(new XmlSchemaSet());
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // A given XmlReader is contains invalid XML.
                    Expect.Call(assertion.Validate(expectedReader))
                        .Return(CreateFailedComparisonResult());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                    Assert.That(!constraint.Matches(expectedReader));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the Matches() method,
        /// when an unexpected exception is raised.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidProgramException))]
        public void Matches_UnexpectedException()
        {
            With.Mocks(delegate
            {
                XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(new XmlSchemaSet());
                using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // An error occurs during validation.
                    assertion.Validate(expectedReader);
                    LastCall.Throw(new InvalidProgramException());

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                    constraint.Matches(expectedReader);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteDescriptionTo() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void WriteDescriptionTo()
        {
            With.Mocks(delegate
            {
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();
                Mocker.Current.ReplayAll();

                XmlValidityConstraint constraint = new XmlValidityConstraint(new XmlSchemaSet());
                constraint.WriteDescriptionTo(writer);
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteMessageTo() method.
        /// </summary>
        [Test]
        public void WriteMessageTo()
        {
            With.Mocks(delegate
            {
                XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(new XmlSchemaSet());
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // A given XmlReader is contains invalid XML.
                    IList<ValidationEventArgs> assertionResult = CreateFailedComparisonResult();
                    Expect.Call(assertion.Validate(expectedReader))
                        .Return(assertionResult);

                    // The message writer receives the error message.
                    writer.WriteLine("message");

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                    constraint.Matches(expectedReader);
                    constraint.WriteMessageTo(writer);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the WriteActualValueTo() method.
        /// </summary>
        [Test]
        public void WriteActualValueTo()
        {
            With.Mocks(delegate
            {
                XmlValidityAssertion assertion = Mocker.Current.CreateMock<XmlValidityAssertion>(new XmlSchemaSet());
                MessageWriter writer = Mocker.Current.CreateMock<MessageWriter>();

                using (XmlReader expectedReader = XmlReader.Create(Stream.Null))
                {
                    // Expectations
                    // The message writer receives the "actual" parameter.
                    writer.WriteActualValue(expectedReader);
                    Expect.Call(assertion.Validate(expectedReader))
                        .Return(new ValidationEventArgs[0]);

                    // Verification and assertions.
                    Mocker.Current.ReplayAll();

                    XmlValidityConstraint constraint = new XmlValidityConstraint(assertion);
                    constraint.Matches(expectedReader);
                    constraint.WriteActualValueTo(writer);
                }
            });
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a single-element collection of <see cref="ValidationEventArgs"/> representing a failed comparison.
        /// </summary>
        private static IList<ValidationEventArgs> CreateFailedComparisonResult()
        {
            return new ValidationEventArgs[] {
                Activator.CreateInstance(
                    typeof(ValidationEventArgs),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { new XmlSchemaException("message") },
                    null) as ValidationEventArgs
            };
        }

        #endregion
    }
}