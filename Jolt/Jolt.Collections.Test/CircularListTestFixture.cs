// ----------------------------------------------------------------------------
// CircularListTestFixture.cs
//
// Contains the definition of the CircularListTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/18/2009 15:36:49
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Jolt.Collections.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CircularListTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            CircularList<int> list = new CircularList<int>();

            Assert.That(list.Collection, Is.Empty);
            Assert.That(list.Collection, Is.InstanceOf<List<int>>());
        }

        /// <summary>
        /// Verifies the behavior of the construction that initializes
        /// the capacity of the new object.
        /// </summary>
        [Test]
        public void Construction_Capacity()
        {
            int expectedCapacity = 123456;
            CircularList<int> list = new CircularList<int>(expectedCapacity);

            Assert.That(list.Collection, Is.Empty);
            Assert.That(list.Collection, Is.InstanceOf<List<int>>());
            Assert.That((list.Collection as List<int>).Capacity, Is.EqualTo(expectedCapacity));
        }

        /// <summary>
        /// Verifies the behavior of the class copy constructor.
        /// </summary>
        [Test]
        public void Construction_Copy()
        {
            IEnumerable<int> sourceCollection = new[] { 1, 2, 3, 4, 5 };
            CircularList<int> list = new CircularList<int>(sourceCollection);

            Assert.That(list.Collection, Is.Not.SameAs(sourceCollection));
            Assert.That(list.Collection, Is.EqualTo(sourceCollection));
        }

        /// <summary>
        /// Verifies the behvior of the internal constructor.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            List<int> collection = new List<int>();
            CircularList<int> list = new CircularList<int>(collection);

            Assert.That(list.Collection, Is.SameAs(collection));
        }

        /// <summary>
        /// Verifies the behavior of the generic GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator()
        {
            CircularList<int> collection = new CircularList<int>(Enumerable.Range(0, 100));

            int expectedStartIndex = 50;
            IEnumerator<int> enumerator = (collection >> expectedStartIndex).GetEnumerator();

            Assert.That(enumerator, Is.InstanceOf<CircularListEnumerator<int>>());
            Assert.That((enumerator as CircularListEnumerator<int>).CurrentIndex, Is.EqualTo(expectedStartIndex));
        }

        /// <summary>
        /// Verifies the behavior of the non-generic GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator_NonGeneric()
        {
            CircularList<int> collection = new CircularList<int>(Enumerable.Range(0, 100));

            int expectedStartIndex = 50;
            IEnumerator enumerator = ((collection >> expectedStartIndex) as IEnumerable).GetEnumerator();

            Assert.That(enumerator, Is.InstanceOf<CircularListEnumerator<int>>());
            Assert.That((enumerator as CircularListEnumerator<int>).CurrentIndex, Is.EqualTo(expectedStartIndex));
        }

        /// <summary>
        /// Verifies the behavior of the Add() method.
        /// </summary>
        [Test]
        public void Add()
        {
            IList<string> expectedElements = new List<string>(new[] { "aaa", "bbb", "ccc", "ddd" });
            CircularList<string> list = new CircularList<string>(expectedElements as IEnumerable<string>);

            string expectedItem = "Add()";
            
            expectedElements.Add(expectedItem);
            list.Add(expectedItem);
            Assert.That(list.ToArray(), Is.EqualTo(expectedElements));

            expectedElements.Add(expectedItem);
            list.Add(expectedItem);
            Assert.That(list.ToArray(), Is.EqualTo(expectedElements));
        }

        /// <summary>
        /// Verifies the behavior of the Add() method, when the circular list
        /// has been shifted.
        /// </summary>
        [Test]
        public void Add_Shifted()
        {
            CircularList<string> list = new CircularList<string>(new[] { "aaa", "bbb", "ccc", "ddd" } as IEnumerable<string>);
            string expectedItem = "Add_Shifted()";

            list >>= 3;
            list.Add(expectedItem);
            Assert.That(list.ToArray(), Is.EqualTo(new[] { "ddd", "aaa", "bbb", "ccc", expectedItem }));

            list <<= 2;
            list.Add(expectedItem);
            Assert.That(list.ToArray(), Is.EqualTo(new[] { "ccc", expectedItem, "ddd", "aaa", "bbb", expectedItem }));
        }

        /// <summary>
        /// Verifies the behavior of the Clear() method.
        /// </summary>
        [Test]
        public void Clear()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Clear());

            new CircularList<string>(collection).Clear();
        }

        /// <summary>
        /// Verifies the behavior of the Contains() method.
        /// </summary>
        [Test]
        public void Contains()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();

            string expectedItem = "Contains()";
            collection.Expect(c => c.Contains(expectedItem)).Return(true);

            Assert.That(new CircularList<string>(collection).Contains(expectedItem));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTo() method.
        /// </summary>
        [Test]
        public void CopyTo()
        {
            IList<int> expectedArray = new[] { 30, 20, 50, 40, 70, 100, 25 };
            CircularList<int> collection = new CircularList<int>(expectedArray);

            int[] array = new int[collection.Count];
            collection.CopyTo(array, 0);

            Assert.That(array, Is.EqualTo(expectedArray));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTo() method, when the circular
        /// list has been shifted.
        /// </summary>
        [Test]
        public void CopyTo_Shifted()
        {
            CircularList<int> collection = new CircularList<int>(new[] { 30, 20, 50, 40, 70, 100, 25, 15 });
            collection >>= collection.Count / 2;

            int[] array = new int[collection.Count];
            collection.CopyTo(array, 0);

            Assert.That(array, Is.EqualTo(new[] { 70, 100, 25, 15, 30, 20, 50, 40 }));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTo() method, when the collection is
        /// copied to a non-zero index.
        /// </summary>
        [Test]
        public void CopyTo_NonZeroIndex()
        {
            CircularList<int> collection = new CircularList<int>(new[] { 30, 20, 50, 40, 70, 100, 25, 15 });
            collection >>= collection.Count / 2;

            int arrayLengthOffset = 3;
            int[] array = new int[arrayLengthOffset + collection.Count];
            collection.CopyTo(array, arrayLengthOffset);

            Assert.That(array, Is.EqualTo(new[] { 0, 0, 0, 70, 100, 25, 15, 30, 20, 50, 40 }));
        }

        /// <summary>
        /// Verifies the behavior fo the CopyTo() method, when the collection is
        /// copied to a null array.
        /// </summary>
        [Test]
        public void CopyTo_NullArray()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CircularList<int>(new int[0]).CopyTo(null, 0),
                Resources.Error_CircularList_CopyTo_NullArray);
        }

        /// <summary>
        /// Verifies the behavior fo the CopyTo() method, when the given array index
        /// is negative.
        /// </summary>
        [Test]
        public void CopyTo_NegativeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CircularList<int>(new int[0]).CopyTo(new int[0], -100),
                Resources.Error_CircularList_CopyTo_NegativeIndex);
        }

        /// <summary>
        /// Verifies the behavior fo the CopyTo() method, when the given array index
        /// exceeds the array boundaries..
        /// </summary>
        [Test]
        public void CopyTo_IndexOutOfBounds()
        {
            Assert.Throws<ArgumentException>(
                () => new CircularList<int>(new int[0]).CopyTo(new int[0], 100),
                Resources.Error_CircularList_CopyTo_IndexOutOfBounds);
        }

        /// <summary>
        /// Verifies the behavior fo the CopyTo() method, when there is insufficient
        /// space in the given array to copy the collection.
        /// </summary>
        [Test]
        public void CopyTo_InsufficientSpace()
        {
            Assert.Throws<ArgumentException>(
                () => new CircularList<int>(new int[5]).CopyTo(new int[5], 1),
                Resources.Error_CircularList_CopyTo_IndexOutOfBounds);
        }

        /// <summary>
        /// Verifies the behavior of the Count property.
        /// </summary>
        [Test]
        public void Count()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();

            int expectedCount = 200;
            collection.Expect(c => c.Count).Return(expectedCount);

            Assert.That(new CircularList<string>(collection).Count, Is.EqualTo(expectedCount));
        }

        /// <summary>
        /// Verifies the behavior of the IsReadOnly property.
        /// </summary>
        [Test]
        public void IsReadOnly()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();
            collection.Expect(c => c.IsReadOnly).Return(true);

            Assert.That(new CircularList<string>(collection).IsReadOnly);
        }

        /// <summary>
        /// Verifies the behavior of the Remove() method.
        /// </summary>
        [Test]
        public void Remove()
        {
            CircularList<int> list = new CircularList<int>(new[] { 0, 1, 2, 2, 3, 3, 2, 1, 0 } as IEnumerable<int>);

            Assert.That(list.Remove(0));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 2, 3, 3, 2, 1, 0 }));

            Assert.That(list.Remove(0));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 2, 3, 3, 2, 1 }));

            Assert.That(list.Remove(3));
            Assert.That(list.Remove(3));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 2, 2, 1 }));

            Assert.That(!list.Remove(0));
            Assert.That(!list.Remove(3));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 2, 2, 1 }));
        }

        /// <summary>
        /// Verifies the behavior of the Remove() method when a shift
        /// has been applied to the collections.
        /// </summary>
        [Test]
        public void Remove_Shifted()
        {
            CircularList<int> list = new CircularList<int>(new[] { 0, 1, 2, 0, 4, 3, 2, 1, 0 } as IEnumerable<int>);

            list >>= 3;

            Assert.That(list.Remove(0));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 4, 3, 2, 1, 0, 0, 1, 2 }));

            list >>= 1;

            Assert.That(list.Remove(4));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 3, 2, 1, 0, 0, 1, 2 }));

            Assert.That(list.Remove(3));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 2, 1, 0, 0, 1, 2 }));

            list >>= 3;
            Assert.That(!list.Remove(4));
            Assert.That(!list.Remove(3));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 2, 1, 0 }));
        }

        /// <summary>
        /// Verifies the behavior of the IndexOf() method.
        /// </summary>
        [Test]
        public void IndexOf()
        {
            CircularList<int> list = new CircularList<int>(Enumerable.Range(0, 100));
            for (int i = 99; i >= 0; --i)
            {
                Assert.That(list.IndexOf(i), Is.EqualTo(i));
            }

            Assert.That(list.IndexOf(-100), Is.EqualTo(-1));
            Assert.That(list.IndexOf(100), Is.EqualTo(-1));
        }

        /// <summary>
        /// Verifies the behavior of the IndexOf() method when a shift
        /// has been applied to the collection.
        /// </summary>
        [Test]
        public void IndexOf_Shifted()
        {
            CircularList<int> list = new CircularList<int>(Enumerable.Range(0, 100));
            list >>= 75;

            Assert.That(list.IndexOf(30), Is.EqualTo(55));
            Assert.That(list.IndexOf(-100), Is.EqualTo(-1));
            Assert.That(list.IndexOf(200), Is.EqualTo(-1));
        }

        /// <summary>
        /// Verifies the behavior of the Insert() method.
        /// </summary>
        [Test]
        public void Insert()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100);

            string expectedItem = "Insert()";
            int expectedIndex = 50;
            collection.Expect(c => c.Insert(expectedIndex, expectedItem));

            new CircularList<string>(collection).Insert(expectedIndex, expectedItem);
        }

        /// <summary>
        /// Verifies the behavior of the Insert() method, when the given index
        /// exceeds the number of items in the collection.
        /// </summary>
        [Test]
        public void Insert_Cyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100);

            string expectedItem = "Insert()";
            int expectedIndex = 50;
            collection.Expect(c => c.Insert(expectedIndex, expectedItem));

            new CircularList<string>(collection).Insert(5 * expectedIndex, expectedItem);
        }

        /// <summary>
        /// Verifies the behavior of the Insert() method, when a shift
        /// has been applied to the collection.
        /// </summary>
        [Test]
        public void Insert_Shifted()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100).Repeat.Any();

            string expectedItem = "Insert()";
            
            // Insert before and after the head index
            collection.Expect(c => c.Insert(5, expectedItem));
            collection.Expect(c => c.Insert(81, expectedItem));

            CircularList<string> list = new CircularList<string>(collection);
            list >>= 75;
            list.Insert(30, expectedItem);
            list.Insert(5, expectedItem);
        }

        /// <summary>
        /// Verifies the behavior of the Insert() method, when the given index
        /// is negative.
        /// </summary>
        [Test]
        public void Insert_NegativeCyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(27).Repeat.Any();

            string expectedItem = "Insert()";
            int expectedIndex = 9;
            collection.Expect(c => c.Insert(expectedIndex, expectedItem));

            new CircularList<string>(collection).Insert(-5 * expectedIndex, expectedItem);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveAt() method.
        /// </summary>
        [Test]
        public void RemoveAt()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 50;
            collection.Expect(c => c.RemoveAt(expectedIndex));

            new CircularList<string>(collection).RemoveAt(expectedIndex);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveAt() method, when the given index
        /// exceeds the number of items in the collection.
        /// </summary>
        [Test]
        public void RemoveAt_Cyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100).Repeat.Any();

            int expectedIndex = 50;
            collection.Expect(c => c.RemoveAt(expectedIndex));

            new CircularList<string>(collection).RemoveAt(5 * expectedIndex);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveAt() method when a shift
        /// has been applied to the collection.
        /// </summary>
        [Test]
        public void RemoveAt_Shifted()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100).Repeat.Any();

            int expectedIndex = 5;
            collection.Expect(c => c.RemoveAt(expectedIndex));

            CircularList<string> list = new CircularList<string>(collection);
            list >>= 75;
            list.RemoveAt(30);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveAt() method, when the given index
        /// is negative.
        /// </summary>
        [Test]
        public void RemoveAt_NegativeCyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(27).Repeat.Any();

            int expectedIndex = 9;
            collection.Expect(c => c.RemoveAt(expectedIndex));

            new CircularList<string>(collection).RemoveAt(-5 * expectedIndex);
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for get operations.
        /// </summary>
        [Test]
        public void Item_Get()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 50;
            string expectedItem = "get_Item";
            collection.Expect(c => c[expectedIndex]).Return(expectedItem);

            Assert.That(new CircularList<string>(collection)[expectedIndex], Is.SameAs(expectedItem));
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for get operations,
        /// when the given index exceeds the number of items in the collection.
        /// </summary>
        [Test]
        public void Item_Get_Cyclical()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 50;
            string expectedItem = "get_Item";
            collection.Expect(c => c[expectedIndex]).Return(expectedItem);

            Assert.That(new CircularList<string>(collection)[5 * expectedIndex], Is.SameAs(expectedItem));
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for get operations,
        /// when a shift has been applied to the collection.
        /// </summary>
        [Test]
        public void Item_Get_Shifted()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 5;
            string expectedItem = "get_Item";
            collection.Expect(c => c[expectedIndex]).Return(expectedItem);

            CircularList<string> list = new CircularList<string>(collection);
            list >>= 75;

            Assert.That(list[30], Is.SameAs(expectedItem));
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for get operations,
        /// when the given index is negative
        /// </summary>
        [Test]
        public void Item_Get_NegativeCyclical()
        {
            IList<string> collection = MockRepository.GenerateMock<IList<string>>();
            collection.Expect(c => c.Count).Return(27);

            int expectedIndex = 9;
            string expectedItem = "get_Item";
            collection.Expect(c => c[expectedIndex]).Return(expectedItem);

            Assert.That(new CircularList<string>(collection)[-5 * expectedIndex], Is.SameAs(expectedItem));
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for set operations.
        /// </summary>
        [Test]
        public void Item_Set()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 50;
            string expectedItem = "set_Item";
            collection.Expect(c => c[expectedIndex] = expectedItem);

            new CircularList<string>(collection)[expectedIndex] = expectedItem;
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for set operations,
        /// when the given index exceeds the number of items in the collection.
        /// </summary>
        [Test]
        public void Item_Set_Cyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100);

            int expectedIndex = 50;
            string expectedItem = "set_Item";
            collection.Expect(c => c[expectedIndex] = expectedItem);

            new CircularList<string>(collection)[5 * expectedIndex] = expectedItem;
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for set operations,
        /// when a shift has been applied to the collection.
        /// </summary>
        [Test]
        public void Item_Set_Shifted()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(100).Repeat.Any();

            int expectedIndex = 5;
            string expectedItem = "set_Item";
            collection.Expect(c => c[expectedIndex] = expectedItem);

            CircularList<string> list = new CircularList<string>(collection);
            list >>= 75;
            list[30] = expectedItem;
        }

        /// <summary>
        /// Verifies the behavior of the Item property, for set operations,
        /// when the given index is negative.
        /// </summary>
        [Test]
        public void Item_Set_NegativeCyclical()
        {
            IList<string> collection = CreateMockList<string>();
            collection.Expect(c => c.Count).Return(27).Repeat.Any();

            int expectedIndex = 9;
            string expectedItem = "set_Item";
            collection.Expect(c => c[expectedIndex] = expectedItem);

            new CircularList<string>(collection)[-5 * expectedIndex] = expectedItem;
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

            CircularList<string> list = new CircularList<string>(one_two_three);

            Assert.That(list >> 0, Is.SameAs(list));
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list >>= 3;
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list >>= 2;
            Assert.That(list.ToArray(), Is.EqualTo(three_one_two));

            list >>= 2;
            Assert.That(list.ToArray(), Is.EqualTo(two_three_one));

            list >>= -1;
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list >>= -2;
            Assert.That(list.ToArray(), Is.EqualTo(two_three_one));

            list >>= -2;
            Assert.That(list.ToArray(), Is.EqualTo(three_one_two));

            list >>= -3;
            Assert.That(list.ToArray(), Is.EqualTo(three_one_two));
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

            CircularList<string> list = new CircularList<string>(one_two_three);

            Assert.That(list >> 0, Is.SameAs(list));
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list <<= 3;
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list <<= 2;
            Assert.That(list.ToArray(), Is.EqualTo(two_three_one));

            list <<= 2;
            Assert.That(list.ToArray(), Is.EqualTo(three_one_two));

            list <<= -1;
            Assert.That(list.ToArray(), Is.EqualTo(one_two_three));

            list <<= -2;
            Assert.That(list.ToArray(), Is.EqualTo(three_one_two));

            list <<= -2;
            Assert.That(list.ToArray(), Is.EqualTo(two_three_one));

            list <<= -3;
            Assert.That(list.ToArray(), Is.EqualTo(two_three_one));
        }

        /// <summary>
        /// Verifies the debugger display configuration of the class.
        /// </summary>
        [Test]
        public void DebuggerDisplayOVerride()
        {
            Assert.That(
                typeof(CircularList<>),
                Has.Attribute<DebuggerTypeProxyAttribute>()
                    .With.Property("ProxyTypeName").EqualTo(typeof(CollectionDebugView<>).AssemblyQualifiedName));

            Assert.That(
                typeof(CircularList<>),
                Has.Attribute<DebuggerDisplayAttribute>()
                    .With.Property("Value").EqualTo("Count = {Count}"));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a strict mock of an IList.
        /// </summary>
        /// 
        /// <typeparam name="TElement">
        /// The type of element stored in the list.
        /// </typeparam>
        /// 
        /// <returns>
        /// The requested <see cref="System.Collections.Generic.IList"/> mock,
        /// with the expectation that the GetEnumerator() method is called.
        /// </returns>
        private static IList<TElement> CreateMockList<TElement>()
        {
            IList<TElement> list = MockRepository.GenerateStrictMock<IList<TElement>>();
            list.Expect(l => l.GetEnumerator()).Return(null);

            return list;
        }

        #endregion
    }
}