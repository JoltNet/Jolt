// ----------------------------------------------------------------------------
// CollectionDebuggerViewTestFixture.cs
//
// Contains the definition of the CollectionDebuggerViewTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 1/24/2010 20:35:29
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CollectionDebuggerViewTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            IEnumerable<int> collection = new int[0];
            CollectionDebugView<int> view = new CollectionDebugView<int>(collection);

            Assert.That(view.Collection, Is.SameAs(collection));
        }

        [Test]
        public void Construction_NullArgument()
        {
            Assert.That(() => new CollectionDebugView<int>(null), Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies the debugger display configuration of the class.
        /// </summary>
        [Test]
        public void DebuggerDisplayOVerride()
        {
            Assert.That(
                typeof(CollectionDebugView<>).GetProperty("Items"),
                Has.Attribute<DebuggerBrowsableAttribute>()
                    .With.Property("State").EqualTo(DebuggerBrowsableState.RootHidden));
        }
        
        /// <summary>
        /// Verifies the behavior of the Items property.
        /// </summary>
        [Test]
        public void Items()
        {
            Random rng = new Random();
            List<int> sourceCollection = new List<int>(Enumerable.Range(rng.Next(10000), rng.Next(10000)));

            int[] expectedArray = new int[sourceCollection.Count];
            sourceCollection.CopyTo(expectedArray);

            Assert.That(new CollectionDebugView<int>(sourceCollection).Items, Is.EqualTo(expectedArray));
        }
    }
}