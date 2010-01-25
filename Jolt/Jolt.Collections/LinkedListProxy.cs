// ----------------------------------------------------------------------------
// LinkedListProxy.cs
//
// Contains the definition of the LinkedListProxy class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/27/2009 12:50:49
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Jolt.Collections
{
    /// <summary>
    /// Do not use this type directly!
    /// It serves as a proxy to simplify testing of the <see cref="CirculaLinkedList"/>
    /// class, and will be auto-generated in the future.
    /// </summary>
    internal class LinkedListProxy<TElement> : ILinkedList<TElement>, ICollection<TElement>, ICollection
    {
        #region constructors ----------------------------------------------------------------------

        public LinkedListProxy()
        {
            m_collection = new LinkedList<TElement>();
        }

        public LinkedListProxy(IEnumerable<TElement> collection)
        {
            m_collection = new LinkedList<TElement>(collection);
        }

        #endregion

        #region IEnumerable<TElement> members -----------------------------------------------------

        public IEnumerator<TElement> GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable members ---------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (m_collection as IEnumerable).GetEnumerator();
        }

        #endregion

        #region ICollection<TElement> members -----------------------------------------------------

        void ICollection<TElement>.Add(TElement item)
        {
            (m_collection as ICollection<TElement>).Add(item);
        }

        public void Clear()
        {
            m_collection.Clear();
        }

        public bool Contains(TElement item)
        {
            return m_collection.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            m_collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_collection.Count; }
        }

        bool ICollection<TElement>.IsReadOnly
        {
            get { return (m_collection as ICollection<TElement>).IsReadOnly; }
        }

        public bool Remove(TElement item)
        {
            return m_collection.Remove(item);
        }

        #endregion

        #region ICollection members ---------------------------------------------------------------

        void ICollection.CopyTo(Array array, int index)
        {
            (m_collection as ICollection).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return (m_collection as ICollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return (m_collection as ICollection).SyncRoot; }
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        public LinkedListNode<TElement> First
        {
            get { return m_collection.First; }
        }

        public LinkedListNode<TElement> Last
        {
            get { return m_collection.Last; }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        public void AddAfter(LinkedListNode<TElement> node, LinkedListNode<TElement> newNode)
        {
            m_collection.AddAfter(node, newNode);
        }

        public LinkedListNode<TElement> AddAfter(LinkedListNode<TElement> node, TElement value)
        {
            return m_collection.AddAfter(node, value);
        }

        public void AddBefore(LinkedListNode<TElement> node, LinkedListNode<TElement> newNode)
        {
            m_collection.AddBefore(node, newNode);
        }

        public LinkedListNode<TElement> AddBefore(LinkedListNode<TElement> node, TElement value)
        {
            return m_collection.AddBefore(node, value);
        }

        public void AddFirst(LinkedListNode<TElement> node)
        {
            m_collection.AddFirst(node);
        }

        public LinkedListNode<TElement> AddFirst(TElement value)
        {
            return m_collection.AddFirst(value);
        }

        public void AddLast(LinkedListNode<TElement> node)
        {
            m_collection.AddLast(node);
        }

        public LinkedListNode<TElement> AddLast(TElement value)
        {
            return m_collection.AddLast(value);
        }

        public LinkedListNode<TElement> Find(TElement value)
        {
            return m_collection.Find(value);
        }

        public LinkedListNode<TElement> FindLast(TElement value)
        {
            return m_collection.FindLast(value);
        }

        public void Remove(LinkedListNode<TElement> node)
        {
            m_collection.Remove(node);
        }

        public void RemoveFirst()
        {
            m_collection.RemoveFirst();
        }

        public void RemoveLast()
        {
            m_collection.RemoveLast();
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly LinkedList<TElement> m_collection;

        #endregion
    }
}