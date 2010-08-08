// ----------------------------------------------------------------------------
// CircularLinkedListTestFixture.cs
//
// Contains the definition of the CircularLinkedListTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/24/2009 12:13:04
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CircularLinkedListTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            CircularLinkedList<int> list = new CircularLinkedList<int>();
            Assert.That(list.Collection, Is.Empty);
        }

        /// <summary>
        /// Verifies the behavior of the class copy constructor.
        /// </summary>
        [Test]
        public void Construction_Copy()
        {
            IEnumerable<int> sourceCollection = new[] { 1, 2, 3, 4, 5 };
            CircularLinkedList<int> list = new CircularLinkedList<int>(sourceCollection);

            Assert.That(list.Collection, Is.Not.SameAs(sourceCollection));
            Assert.That(list.Collection, Is.EqualTo(sourceCollection));
        }

        /// <summary>
        /// Verifies the behvior of the internal constructor.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            ILinkedList<int> collection = new LinkedListProxy<int>();
            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);

            Assert.That(list.Collection, Is.SameAs(collection));
        }

        /// <summary>
        /// Verifies the behavior of the generic GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator()
        {
            Assert.That(new CircularLinkedList<int>().GetEnumerator(), Is.InstanceOf<CircularEnumerator<int>>());
        }

        /// <summary>
        /// Verifies the behavior of the non-generic GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator_NonGeneric()
        {
            IEnumerable collection = new CircularList<int>();
            Assert.That(collection.GetEnumerator(), Is.InstanceOf<CircularListEnumerator<int>>());
        }

        /// <summary>
        /// Verifies the behavior of the Add() method.
        /// </summary>
        [Test]
        public void Add()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            (collection as ICollection<int>).Expect(c => c.Add(expectedElement));

            (new CircularLinkedList<int>(collection) as ICollection<int>).Add(expectedElement);
        }

        /// <summary>
        /// Verifies the behavior of the Clear() method.
        /// </summary>
        [Test]
        public void Clear()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            collection.Expect(c => c.Clear());

            new CircularLinkedList<int>(collection).Clear();
        }

        /// <summary>
        /// Verifies the behavior of the Contains() method.
        /// </summary>
        [Test]
        public void Contains()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            collection.Expect(c => c.Contains(expectedElement)).Return(true);

            Assert.That(new CircularLinkedList<int>(collection).Contains(expectedElement));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTo() method.
        /// </summary>
        [Test]
        public void CopyTo()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int[] expectedArray = new int[0];
            int expectedIndex = 12345;
            collection.Expect(c => c.CopyTo(expectedArray, expectedIndex));

            new CircularLinkedList<int>(collection).CopyTo(expectedArray, expectedIndex);
        }

        /// <summary>
        /// Verifies the behavior of the Count property.
        /// </summary>
        [Test]
        public void Count()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedResult = 12345;
            collection.Expect((ICollection<int> c) => c.Count).Return(expectedResult);

            Assert.That(new CircularLinkedList<int>(collection).Count, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the IsReadOnly property.
        /// </summary>
        [Test]
        public void IsReadOnly()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            collection.Expect(c => c.IsReadOnly).Return(true);

            Assert.That((new CircularLinkedList<int>(collection) as ICollection<int>).IsReadOnly);
        }

        /// <summary>
        /// Verifies the behavior of the Remove() method.
        /// </summary>
        [Test]
        public void Remove()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            collection.Expect(c => c.Remove(expectedElement)).Return(true);

            Assert.That(new CircularLinkedList<int>(collection).Remove(expectedElement));
        }

        /// <summary>
        /// Verifies the behavior of the non-generic CopyTo() method.
        /// </summary>
        [Test]
        public void CopyTo_NonGeneric()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            Array expectedArray = new int[0];
            int expectedIndex = 12345;
            collection.Expect(c => c.CopyTo(expectedArray, expectedIndex));

            (new CircularLinkedList<int>(collection) as ICollection).CopyTo(expectedArray, expectedIndex);
        }

        /// <summary>
        /// Verifies the behavior of the IsSynchronized property.
        /// </summary>
        [Test]
        public void IsSynchronized()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            collection.Expect(c => c.IsSynchronized).Return(true);

            Assert.That((new CircularLinkedList<int>(collection) as ICollection).IsSynchronized);
        }

        /// <summary>
        /// Verifies the behavior of the SyncRoot property.
        /// </summary>
        [Test]
        public void SyncRoot()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            object expectedResult = new object();
            collection.Expect(c => c.SyncRoot).Return(expectedResult);

            Assert.That((new CircularLinkedList<int>(collection) as ICollection).SyncRoot, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the First property.
        /// </summary>
        [Test]
        public void First()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            LinkedListNode<int> expectedResult = new LinkedListNode<int>(12345);
            collection.Expect(c => c.First).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.First;

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the Last property.
        /// </summary>
        [Test]
        public void Last()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            LinkedListNode<int> expectedResult = new LinkedListNode<int>(12345);
            collection.Expect(c => c.Last).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.Last;

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the AddAfter() method, accepting an element object.
        /// </summary>
        [Test]
        public void AddAfter_Element()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            CircularLinkedListNode<int> sourceNode = new CircularLinkedListNode<int>(100);
            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.AddAfter(sourceNode.ListNode, expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.AddAfter(sourceNode, expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the AddAfter() method, accepting a list node.
        /// </summary>
        [Test]
        public void AddAfter_Node()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            CircularLinkedListNode<int> sourceNode = new CircularLinkedListNode<int>(100);
            CircularLinkedListNode<int> expectedNode = new CircularLinkedListNode<int>(12345);

            collection.Expect(c => c.AddAfter(sourceNode.ListNode, expectedNode.ListNode));

            new CircularLinkedList<int>(collection).AddAfter(sourceNode, expectedNode);
        }

        /// <summary>
        /// Verifies the behavior of the AddBefore() method, accepting an element object.
        /// </summary>
        [Test]
        public void AddBefore_Element()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            CircularLinkedListNode<int> sourceNode = new CircularLinkedListNode<int>(100);
            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.AddBefore(sourceNode.ListNode, expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.AddBefore(sourceNode, expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the AddBefore() method, accepting a list node.
        /// </summary>
        [Test]
        public void AddBefore_Node()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            CircularLinkedListNode<int> sourceNode = new CircularLinkedListNode<int>(100);
            CircularLinkedListNode<int> expectedNode = new CircularLinkedListNode<int>(12345);

            collection.Expect(c => c.AddBefore(sourceNode.ListNode, expectedNode.ListNode));

            new CircularLinkedList<int>(collection).AddBefore(sourceNode, expectedNode);
        }

        /// <summary>
        /// Verifies the behavior of the AddFirst() method, accepting an element object.
        /// </summary>
        [Test]
        public void AddFirst_Element()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.AddFirst(expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.AddFirst(expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the AddFirst() method, accepting a list node.
        /// </summary>
        [Test]
        public void AddFirst_Node()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            CircularLinkedListNode<int> expectedNode = new CircularLinkedListNode<int>(12345);

            collection.Expect(c => c.AddFirst(expectedNode.ListNode));

            new CircularLinkedList<int>(collection).AddFirst(expectedNode);
        }

        /// <summary>
        /// Verifies the behavior of the AddLast() method, accepting an element object.
        /// </summary>
        [Test]
        public void AddLast_Element()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.AddLast(expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.AddLast(expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the AddLast() method, accepting a list node.
        /// </summary>
        [Test]
        public void AddLast_Node()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            CircularLinkedListNode<int> expectedNode = new CircularLinkedListNode<int>(12345);

            collection.Expect(c => c.AddLast(expectedNode.ListNode));

            new CircularLinkedList<int>(collection).AddLast(expectedNode);
        }

        /// <summary>
        /// Verifies the behavior of the Find() method.
        /// </summary>
        [Test]
        public void Find()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.Find(expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.Find(expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the FindLast() method.
        /// </summary>
        [Test]
        public void FindLast()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();

            int expectedElement = 12345;
            LinkedListNode<int> expectedResult = new LinkedListNode<int>(expectedElement);

            collection.Expect(c => c.FindLast(expectedElement)).Return(expectedResult);

            CircularLinkedList<int> list = new CircularLinkedList<int>(collection);
            CircularLinkedListNode<int> node = list.FindLast(expectedElement);

            Assert.That(node.List, Is.SameAs(list));
            Assert.That(node.ListNode, Is.SameAs(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the Remove() method, accepting a list node.
        /// </summary>
        [Test]
        public void Remove_Node()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            CircularLinkedListNode<int> expectedElement = new CircularLinkedListNode<int>(12345);

            collection.Expect(c => c.Remove(expectedElement.ListNode));

            new CircularLinkedList<int>(collection).Remove(expectedElement);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveFirst() method.
        /// </summary>
        [Test]
        public void RemoveFirst()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            collection.Expect(c => c.RemoveFirst());

            new CircularLinkedList<int>(collection).RemoveFirst();
        }

        /// <summary>
        /// Verifies the behavior of the RemoveLast() method.
        /// </summary>
        [Test]
        public void RemoveLast()
        {
            ILinkedList<int> collection = MockRepository.GenerateStrictMock<ILinkedList<int>>();
            collection.Expect(c => c.RemoveLast());

            new CircularLinkedList<int>(collection).RemoveLast();
        }

        /// <summary>
        /// Verifies the behavior of the right-shift operator.
        /// </summary>
        [Test]
        public void ForwardShift()
        {
            IList<string> one_two_three = new[] { "one", "two", "three" };
            IList<string> two_three_one = new[] { "two", "three", "one" };
            IList<string> three_one_two = new[] { "three", "one", "two" };

            CircularLinkedList<string> list = new CircularLinkedList<string>(one_two_three);

            Assert.That(list >> 0, Is.SameAs(list));
            Assert.That(list, Is.EqualTo(one_two_three));
            
            Assert.That(list >> 3, Is.EqualTo(one_two_three));
            Assert.That(list >> 2, Is.EqualTo(three_one_two));
            Assert.That(list >> 2, Is.EqualTo(two_three_one));
            Assert.That(list >> -1, Is.EqualTo(one_two_three));
            Assert.That(list >> -2, Is.EqualTo(two_three_one));
            Assert.That(list >> -2, Is.EqualTo(three_one_two));
            Assert.That(list >> -3, Is.EqualTo(three_one_two));
        }

        /// <summary>
        /// Verifies the behavior of the left-shift operator.
        /// </summary>
        [Test]
        public void BackwardsShift()
        {
            IList<string> one_two_three = new[] { "one", "two", "three" };
            IList<string> two_three_one = new[] { "two", "three", "one" };
            IList<string> three_one_two = new[] { "three", "one", "two" };

            CircularLinkedList<string> list = new CircularLinkedList<string>(one_two_three);

            Assert.That(list >> 0, Is.SameAs(list));
            Assert.That(list, Is.EqualTo(one_two_three));
            
            Assert.That(list << 3, Is.EqualTo(one_two_three));
            Assert.That(list << 2, Is.EqualTo(two_three_one));
            Assert.That(list << 2, Is.EqualTo(three_one_two));
            Assert.That(list << -1, Is.EqualTo(one_two_three));
            Assert.That(list << -2, Is.EqualTo(three_one_two));
            Assert.That(list << -2, Is.EqualTo(two_three_one));
            Assert.That(list << -3, Is.EqualTo(two_three_one));
        }

        /// <summary>
        /// Verifies the debugger display configuration of the class.
        /// </summary>
        [Test]
        public void DebuggerDisplayOverride()
        {
            Assert.That(
                typeof(CircularLinkedList<>),
                Has.Attribute<DebuggerTypeProxyAttribute>()
                    .With.Property("ProxyTypeName").EqualTo(typeof(CollectionDebugView<>).AssemblyQualifiedName));

            Assert.That(
                typeof(CircularLinkedList<>),
                Has.Attribute<DebuggerDisplayAttribute>()
                    .With.Property("Value").EqualTo("Count = {Count}"));
        }
    }
}