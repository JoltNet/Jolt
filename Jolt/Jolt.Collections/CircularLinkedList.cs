// ----------------------------------------------------------------------------
// CircularLinkedList.cs
//
// Contains the definition of the CircularLinkedList class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/19/2009 14:32:22
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements both the <see cref="System.Collections.ICollection"/> and
    /// <see cref="System.Collections.Generic.ICollection"/> contracts, providing a
    /// circular linked-list collection.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element stored in the collection.
    /// </typeparam>
    /// 
    /// <remarks>
    /// Mimics the interface of the <see cref="System.Collections.Generic.LinkedList"/> class,
    /// using the same implementation format for the <see cref="System.Collections.ICollection"/>
    /// and <see cref="System.Collections.Generic.ICollection"/> contracts.
    /// </remarks>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>)), DebuggerDisplay("Count = {Count}")]
    public sealed class CircularLinkedList<TElement> : ICollection<TElement>, ICollection
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CircularLinkedList"/> class
        /// that is empty.
        /// </summary>
        public CircularLinkedList()
            : this(new LinkedListProxy<TElement>() as ILinkedList<TElement>) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularLinkedList" /> class
        /// that contains elements copied from the specified
        /// <see cref="System.Collections.Generic.IEnumerable" />.
        /// </summary>
        /// 
        /// <param name="collection">
        /// The collection whose elements are copied to the new linked list.
        /// </param>
        public CircularLinkedList(IEnumerable<TElement> collection)
            : this(new LinkedListProxy<TElement>(collection) as ILinkedList<TElement>) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularLinkedList"/> class, adapting
        /// the <see cref="CircularLinkedList"/> interface to an existing collection.
        /// </summary>
        /// 
        /// <param name="collection">
        /// The collection to adapt.
        /// </param>
        internal CircularLinkedList(ILinkedList<TElement> collection)
        {
            m_collection = collection;
        }

        #endregion

        #region IEnumerable<TElement> members -----------------------------------------------------

        /// <summary>
        /// Creates a new instance of an <see cref="System.Collections.Generic.IEnumerable"/>
        /// implementation, suitable for enumerating the <see cref="CircularLinkedList"/> class.
        /// </summary>
        /// 
        /// <returns>
        /// Returns an <see cref="System.Collections.Generic.IEnumerable"/> positioned at the
        /// element prior to the beginning of the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return new CircularEnumerator<TElement>(m_collection);
        }

        #endregion

        #region IEnumerable members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="GetEnumerator()"/>
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection<TElement> members -----------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Add"/>
        /// </summary>
        void ICollection<TElement>.Add(TElement item)
        {
            (m_collection as ICollection<TElement>).Add(item);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Clear"/>
        /// </summary>
        public void Clear()
        {
            m_collection.Clear();
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Contains"/>
        /// </summary>
        public bool Contains(TElement item)
        {
            return m_collection.Contains(item);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.CopyTo(TElement[], int)"/>
        /// </summary>
        public void CopyTo(TElement[] array, int arrayIndex)
        {
            m_collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Count"/>
        /// </summary>
        public int Count
        {
            get { return (m_collection as ICollection<TElement>).Count; }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.IsReadOnly"/>
        /// </summary>
        bool ICollection<TElement>.IsReadOnly
        {
            get { return (m_collection as ICollection<TElement>).IsReadOnly; }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Remove"/>
        /// </summary>
        public bool Remove(TElement item)
        {
            return m_collection.Remove(item);
        }

        #endregion

        #region ICollection members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.CopyTo(Array, int)"/>
        /// </summary>
        void ICollection.CopyTo(Array array, int index)
        {
            (m_collection as ICollection).CopyTo(array, index);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.IsSynchronized"/>
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return (m_collection as ICollection).IsSynchronized; }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.SyncRoot"/>
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return (m_collection as ICollection).SyncRoot; }
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.First"/>
        /// </summary>
        public CircularLinkedListNode<TElement> First
        {
            get { return new CircularLinkedListNode<TElement>(this, m_collection.First); }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Last"/>
        /// </summary>
        public CircularLinkedListNode<TElement> Last
        {
            get { return new CircularLinkedListNode<TElement>(this, m_collection.Last); }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddAfter"/>
        /// </summary>
        public void AddAfter(CircularLinkedListNode<TElement> node, CircularLinkedListNode<TElement> newNode)
        {
            m_collection.AddAfter(node.ListNode, newNode.ListNode);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddAfter"/>
        /// </summary>
        public CircularLinkedListNode<TElement> AddAfter(CircularLinkedListNode<TElement> node, TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.AddAfter(node.ListNode, value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddBefore"/>
        /// </summary>
        public void AddBefore(CircularLinkedListNode<TElement> node, CircularLinkedListNode<TElement> newNode)
        {
            m_collection.AddBefore(node.ListNode, newNode.ListNode);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddBefore"/>
        /// </summary>
        public CircularLinkedListNode<TElement> AddBefore(CircularLinkedListNode<TElement> node, TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.AddBefore(node.ListNode, value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddFirst"/>
        /// </summary>
        public void AddFirst(CircularLinkedListNode<TElement> node)
        {
            m_collection.AddFirst(node.ListNode);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddFirst"/>
        /// </summary>
        public CircularLinkedListNode<TElement> AddFirst(TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.AddFirst(value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddLast"/>
        /// </summary>
        public void AddLast(CircularLinkedListNode<TElement> node)
        {
            m_collection.AddLast(node.ListNode);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.AddLast"/>
        /// </summary>
        public CircularLinkedListNode<TElement> AddLast(TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.AddLast(value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Find"/>
        /// </summary>
        public CircularLinkedListNode<TElement> Find(TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.Find(value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.FindLast"/>
        /// </summary>
        public CircularLinkedListNode<TElement> FindLast(TElement value)
        {
            return new CircularLinkedListNode<TElement>(this, m_collection.FindLast(value));
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.Remove"/>
        /// </summary>
        public void Remove(CircularLinkedListNode<TElement> node)
        {
            m_collection.Remove(node.ListNode);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.RemoveFirst"/>
        /// </summary>
        public void RemoveFirst()
        {
            m_collection.RemoveFirst();
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.LinkedList.RemoveLast"/>
        /// </summary>
        public void RemoveLast()
        {
            m_collection.RemoveLast();
        }


        /// <summary>
        /// Rotates a given <see cref="CircularLinkedList"/> such that the head of the list
        /// is repositioned a given number of elements forwards.
        /// </summary>
        /// 
        /// <param name="list">
        /// The <see cref="CircularLinkedList"/> to forward-shift.
        /// </param>
        /// 
        /// <param name="numberOfElements">
        /// The number of elements that the head of the list is shifted.
        /// </param>
        /// 
        /// <returns>
        /// A reference to <paramref name="list"/>.
        /// </returns>
        public static CircularLinkedList<TElement> operator >>(CircularLinkedList<TElement> list, int numberOfElements)
        {
            // Minimize the number of shifts by shifting forward or backwards up to
            // half the number of elements in the collection.
            int collectionSize = (list.m_collection as ICollection<TElement>).Count;
            numberOfElements %= collectionSize;
            int shiftMagnitude = Math.Abs(numberOfElements);

            if (shiftMagnitude > collectionSize / 2)
            {
                numberOfElements = (collectionSize - shiftMagnitude) * -(shiftMagnitude / numberOfElements);
            }

            while (numberOfElements > 0)
            {
                LinkedListNode<TElement> node = list.m_collection.First;
                list.m_collection.RemoveFirst();
                list.m_collection.AddLast(node);
                --numberOfElements;
            }

            while (numberOfElements < 0)
            {
                LinkedListNode<TElement> node = list.m_collection.Last;
                list.m_collection.RemoveLast();
                list.m_collection.AddFirst(node);
                ++numberOfElements;
            }

            return list;
        }

        /// <summary>
        /// Rotates a given <see cref="CircularLinkedList"/> such that the head of the list
        /// is repositioned a given number of elements backwards.
        /// </summary>
        /// 
        /// <param name="list">
        /// The <see cref="CircularLinkedList"/> to backward-shift.
        /// </param>
        /// 
        /// <param name="numberOfElements">
        /// The number of elements that the head of the list is shifted.
        /// </param>
        /// 
        /// <returns>
        /// A reference to <paramref name="list"/>.
        /// </returns>
        public static CircularLinkedList<TElement> operator <<(CircularLinkedList<TElement> list, int numberOfElements)
        {
            return list >> -numberOfElements;
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets a reference to the internal <see cref="System.Collections.Generic.linkedList"/> 
        /// encapsulated by this instance.
        /// </summary>
        internal ILinkedList<TElement> Collection
        {
            get { return m_collection; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly ILinkedList<TElement> m_collection;

        #endregion
    }
}