using System;
using System.Collections.Generic;
using System.Linq;

using Jolt.Linq;
using NUnit.Framework;

namespace Jolt.Test.Linq
{
    [TestFixture]
    public sealed class EnumerableTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the NonCastableIterator() method.
        /// </summary>
        [Test]
        public void NonCastableIterator()
        {
            Random rng = new Random();
            int[] expectedCollection = System.Linq.Enumerable.Range(rng.Next(1000), rng.Next(10000)).ToArray();
            IEnumerable<int> actualCollection = expectedCollection.AsNonCastableEnumerable();

            Assert.That(actualCollection, Is.Not.InstanceOf<int[]>());
            Assert.That(actualCollection, Is.EqualTo(expectedCollection));
        }
    }
}