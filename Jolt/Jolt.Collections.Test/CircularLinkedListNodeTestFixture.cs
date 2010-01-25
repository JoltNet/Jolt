// ----------------------------------------------------------------------------
// CircularLinkedListNodeTestFixture.cs
//
// Contains the definition of the CircularLinkedListNodeTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/20/2009 11:01:17
// ----------------------------------------------------------------------------

using System.Collections.Generic;

using NUnit.Framework;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CircularLinkedListNodeTestFixture
    {
        /// <summary>
        /// Verifies the explicit construction of the struct.
        /// </summary>
        [Test]
        public void Construction()
        {
            string expectedValue = "node-value";
            CircularLinkedListNode<string> node = new CircularLinkedListNode<string>(expectedValue);

            Assert.That(node.List, Is.Null);
            Assert.That(node.ListNode, Is.Not.Null);
            Assert.That(node.ListNode.List, Is.Null);
            Assert.That(node.ListNode.Value, Is.SameAs(expectedValue));
            Assert.That(node.Value, Is.SameAs(expectedValue));
        }

        /// <summary>
        /// Verifies the internal construction of the struct.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            CircularLinkedList<string> list = new CircularLinkedList<string>();
            LinkedListNode<string> listNode = new LinkedListNode<string>("NodeValue");
            CircularLinkedListNode<string> node = new CircularLinkedListNode<string>(list, listNode);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(listNode));
            Assert.That(node.Value, Is.SameAs(listNode.Value));
        }

        /// <summary>
        /// Verifies the behavior of the Equals() method.
        /// </summary>
        [Test]
        public void Equals_Equatable()
        {
            CircularLinkedListNode<int> node = new CircularLinkedListNode<int>(123);

            Assert.That(node.Equals(new CircularLinkedListNode<int>(null, node.ListNode)));
            Assert.That(!node.Equals(new CircularLinkedListNode<int>(null, new LinkedListNode<int>(123))));
        }

        /// <summary>
        /// Verifies the behavior of the Object.Equals() method override,
        /// when the given operand is of an invalid type.
        /// </summary>
        [Test]
        public void Equals_Override_InvalidOperand()
        {
            CircularLinkedListNode<int> node = new CircularLinkedListNode<int>(12345);
            Assert.That(!node.Equals("invalid-type"));
        }

        /// <summary>
        /// Verifies the behavior of the Object.Equals() method.
        /// </summary>
        [Test]
        public void Equals_Override()
        {
            CircularLinkedListNode<int> node = new CircularLinkedListNode<int>(123);

            Assert.That(node.Equals((object)new CircularLinkedListNode<int>(null, node.ListNode)));
            Assert.That(!node.Equals((object)new CircularLinkedListNode<int>(null, new LinkedListNode<int>(123))));
        }

        /// <summary>
        /// Verifies the behavior of the GetHashCode() method.
        /// </summary>
        [Test]
        public void HashCode()
        {
            CircularLinkedListNode<int> node = new CircularLinkedListNode<int>(123);
            Assert.That(node.GetHashCode(), Is.EqualTo(node.ListNode.GetHashCode()));
        }

        /// <summary>
        /// Verifies the behavior of the Value property setter.
        /// </summary>
        [Test]
        public void SetValue()
        {
            CircularLinkedListNode<string> node = new CircularLinkedListNode<string>("node-value");

            string expectedValue = "new-node-value";
            node.Value = expectedValue;

            Assert.That(node.ListNode.Value, Is.SameAs(expectedValue));
            Assert.That(node.Value, Is.SameAs(expectedValue));
        }

        /// <summary>
        /// Verifies the behavior of the Next property.
        /// </summary>
        [Test]
        public void Next()
        {
            CircularLinkedList<int> list = new CircularLinkedList<int>(new[] { 1, 2, 3, 4, 5 });
            CircularLinkedListNode<int> node = list.Find(4);

            Assert.That(node.Next.List, Is.SameAs(list));
            Assert.That(node.Next.ListNode, Is.SameAs(node.ListNode.Next));
            Assert.That(node.Next.Value, Is.EqualTo(5));

            Assert.That(node.Next.Next.List, Is.SameAs(list));
            Assert.That(node.Next.Next.ListNode, Is.SameAs(list.First.ListNode));
            Assert.That(node.Next.Next.Value, Is.EqualTo(1));
        }

        /// <summary>
        /// Verifies the behavior of the Previous property.
        /// </summary>
        [Test]
        public void Previous()
        {
            CircularLinkedList<int> list = new CircularLinkedList<int>(new[] { 1, 2, 3, 4, 5 });
            CircularLinkedListNode<int> node = list.Find(2);

            Assert.That(node.Previous.List, Is.SameAs(list));
            Assert.That(node.Previous.ListNode, Is.SameAs(node.ListNode.Previous));
            Assert.That(node.Previous.Value, Is.EqualTo(1));

            Assert.That(node.Previous.Previous.List, Is.SameAs(list));
            Assert.That(node.Previous.Previous.ListNode, Is.SameAs(list.Last.ListNode));
            Assert.That(node.Previous.Previous.Value, Is.EqualTo(5));
        }
    }
}