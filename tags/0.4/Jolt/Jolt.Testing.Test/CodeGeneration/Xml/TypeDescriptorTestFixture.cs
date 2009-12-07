// ----------------------------------------------------------------------------
// TypeDescriptorTestFixture.cs
//
// Contains the definition of the TypeDescriptorTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 4/13/2009 16:12:00
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Jolt.Testing.CodeGeneration.Xml;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration.Xml
{
    [TestFixture]
    public sealed class TypeDescriptorTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            Type expectedType = typeof(string);
            IDictionary<Type, Type> expectedCollection = new Dictionary<Type, Type>();

            TypeDescriptor descriptor = new TypeDescriptor(expectedType, expectedCollection);

            Assert.That(descriptor.RealSubjectType, Is.SameAs(expectedType));
            Assert.That(descriptor.ReturnTypeOverrides, Is.SameAs(expectedCollection));
        }
    }
}