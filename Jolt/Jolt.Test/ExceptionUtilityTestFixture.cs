// ----------------------------------------------------------------------------
// ExceptionUtilityTestFixture.cs
//
// Contains the definition of the ExceptionUtilityTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 10/1/2010 09:35:16
// ----------------------------------------------------------------------------

using System;

using Jolt;
using NUnit.Framework;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class ExceptionUtilityTestFixture
    {
        [Test]
        public void ThrowOnNullArgument()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => ExceptionUtility.ThrowOnNullArgument(null as object));
            Assert.That(exception.ParamName, Is.Empty);
        }

        [Test]
        public void ThrowOnNullArgument_ArgNotNull()
        {
            Assert.That(() => ExceptionUtility.ThrowOnNullArgument(new object()), Throws.Nothing);
        }

        [Test]
        public void ThrowOnNullArgument_WithArgName()
        {
            string argumentName = GetType().Name;
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => ExceptionUtility.ThrowOnNullArgument(null as object, argumentName));
            Assert.That(exception.ParamName, Is.SameAs(argumentName));
        }

        [Test]
        public void ThrowOnNullArgument_WithArgName_ArgNotNull()
        {
            Assert.That(() => ExceptionUtility.ThrowOnNullArgument(new object(), GetType().Name), Throws.Nothing);
        }
    }
}