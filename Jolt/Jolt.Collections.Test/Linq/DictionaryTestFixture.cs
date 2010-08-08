// ----------------------------------------------------------------------------
// DictionaryTestFixture.cs
//
// Contains the definition of the DictionaryTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/7/2010 20:09:43
// ----------------------------------------------------------------------------

using System.Collections.Generic;

using Jolt.Collections.Linq;
using NUnit.Framework;

namespace Jolt.Collections.Test.Linq
{
    [TestFixture]
    public sealed class DictionaryTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the AsReadOnly() method.
        /// </summary>
        [Test]
        public void AsReadOnly()
        {
            IDictionary<int, int> dictionary = new Dictionary<int, int>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = dictionary.AsReadOnly();

            Assert.That(readOnlyDictionary.Items, Is.SameAs(dictionary));
        }
    }
}