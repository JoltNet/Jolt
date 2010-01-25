// ----------------------------------------------------------------------------
// CircularList.cs
//
// Contains the definition of the CircularList class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/18/2009 08:53:41
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Jolt.Collections.Properties;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements the <see cref="System.Collections.Generic.IList"/> contract,
    /// providing a circular list collection.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element stored in the collection.
    /// </typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>)), DebuggerDisplay("Count = {Count}")]
    public sealed class CircularList<TElement> : IList<TElement>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CircularList"/> class that is empty
        /// and has the default initial capacity.
        /// </summary>
        public CircularList()
            : this(new List<TElement>()) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularList"/> class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// 
        /// <param name="capacity">
        /// The number of elements that the new list can initially store.
        /// </param>
        public CircularList(int capacity)
            : this(new List<TElement>(capacity)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularList"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// 
        /// <param name="collection">
        /// The collection whose elements are copied to the new list..
        /// </param>
        public CircularList(IEnumerable<TElement> collection)
            : this(new List<TElement>(collection)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularList"/> class, adapting
        /// the <see cref="CircularList"/> interface to an existing collection.
        /// </summary>
        /// 
        /// <param name="collection">
        /// The collection to adapt.
        /// </param>
        internal CircularList(IList<TElement> collection)
        {
            m_collection = collection;
            m_headIndex = 0;
        }

        #endregion

        #region IEnumerable<TElement> members -----------------------------------------------------

        /// <summary>
        /// Creates a new instance of an <see cref="System.Collections.Generic.IEnumerable"/>
        /// implementation, suitable for enumerating the <see cref="CircularList"/> class.
        /// </summary>
        /// 
        /// <returns>
        /// Returns an <see cref="System.Collections.Generic.IEnumerable"/> positioned at the
        /// element prior to the beginning of the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return new CircularListEnumerator<TElement>(m_collection, m_headIndex);
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
        /// <see cref="System.Collections.Generic.ICollection.Add"/>
        /// </summary>
        public void Add(TElement item)
        {
            // insert item at the logical beginning of the list, then increment the head index
            // to simulate adding the element at the end.
            m_collection.Insert(m_headIndex++, item);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.Clear"/>
        /// </summary>
        public void Clear()
        {
            m_collection.Clear();
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.Contains"/>
        /// </summary>
        public bool Contains(TElement item)
        {
            return m_collection.Contains(item);
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.CopyTo"/>
        /// </summary>
        public void CopyTo(TElement[] array, int arrayIndex)
        {
            if (array == null) { throw new ArgumentNullException(Resources.Error_CircularList_CopyTo_NullArray); }
            if (arrayIndex < 0) { throw new ArgumentOutOfRangeException(Resources.Error_CircularList_CopyTo_NegativeIndex); }
            if (arrayIndex >= array.Length) { throw new ArgumentException(Resources.Error_CircularList_CopyTo_IndexOutOfBounds); }
            if (m_collection.Count > array.Length - arrayIndex) { throw new ArgumentException(Resources.Error_CircularList_CopyTo_InsufficientSpace); }

            for (int i = 0; i < m_collection.Count; ++i)
            {
                array[i + arrayIndex] = m_collection[ToInternalIndex(i)];
            }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.Count"/>
        /// </summary>
        public int Count
        {
            get { return m_collection.Count; }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.IsReadOnly"/>
        /// </summary>
        public bool IsReadOnly
        {
            get { return (m_collection as ICollection<TElement>).IsReadOnly; }
        }

        /// <summary>
        /// <see cref="System.Collections.Generic.ICollection.Remove"/>
        /// </summary>
        public bool Remove(TElement item)
        {
            int itemIndex = IndexOf(item);
            if (itemIndex < 0) { return false; }

            RemoveAt(itemIndex);
            return true;
        }

        #endregion

        #region IList<TElement> members -----------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.IList.IndexOf"/>
        /// </summary>
        public int IndexOf(TElement item)
        {
            foreach (int index in Enumerable.Range(m_headIndex, m_collection.Count - m_headIndex)
                                            .Union(Enumerable.Range(0, m_headIndex)))
            {
                if (m_collection[index].Equals(item)) { return ToExternalIndex(index); }
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="System.Collections.Generic.IList"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index at which the item should be inserted.
        /// </param>
        /// 
        /// <param name="item">
        /// The object to insert into the <see cref="System.Collections.Generic.IList"/>.
        /// </param>
        /// 
        /// <remarks>
        /// This method accounts for cycles, and thus <paramref name="index"/> values greater than
        /// the number of elements in the list are accepted.
        /// </remarks>
        /// 
        /// <seealso cref="System.Collections.Generic.IList.Insert"/>
        public void Insert(int index, TElement item)
        {
            index = ToInternalIndex(index);
            m_collection.Insert(index, item);

            // Adding elements before the head index implicitly
            // changes the head index value due to the copying of elements
            // after the insertion.
            if (index < m_headIndex) { ++m_headIndex; }
        }

        /// <summary>
        /// Removes the <see cref="System.Collections.Generic.IList"/> item at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// 
        /// <remarks>
        /// This method accounts for cycles, and thus <paramref name="index"/> values greater than
        /// the number of elements in the list are accepted.
        /// </remarks>
        /// 
        /// <seealso cref="System.Collections.Generic.IList.RemoveAt"/>
        public void RemoveAt(int index)
        {
            index = ToInternalIndex(index);
            m_collection.RemoveAt(index);

            // Removing elements before the head index implicitly
            // changes the head index value due to the copying of elements
            // after the removal.
            if (index < m_headIndex) { --m_headIndex; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        ///
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// 
        /// <remarks>
        /// This method accounts for cycles, and thus <paramref name="index"/> values greater than
        /// the number of elements in the list are accepted.
        /// </remarks>
        ///
        /// <seealso cref="System.Collections.Generic.IList.Item"/>
        public TElement this[int index]
        {
            get { return m_collection[ToInternalIndex(index)]; }
            set { m_collection[ToInternalIndex(index)] = value; }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Rotates the <see cref="CircularList"/> such that the head of the list
        /// is repositioned a given number of elements forward.
        /// </summary>
        /// 
        /// <param name="list">
        /// The <see cref="CircularList"/> to forward-shift.
        /// </param>
        /// 
        /// <param name="numberOfElements">
        /// The number of elements that the head of the list is shifted.
        /// </param>
        /// 
        /// <returns>
        /// A reference to <paramref name="list"/>.
        /// </returns>
        public static CircularList<TElement> operator >>(CircularList<TElement> list, int numberOfElements)
        {
            list.m_headIndex = list.ToInternalIndex(numberOfElements);
            return list;
        }

        /// <summary>
        /// Rotates a given <see cref="CircularList"/> such that the head of the list
        /// is repositioned a given number of elements backwards.
        /// </summary>
        /// 
        /// <param name="list">
        /// The <see cref="CircularList"/> to backward-shift.
        /// </param>
        /// 
        /// <param name="numberOfElements">
        /// The number of elements that the head of the list is shifted.
        /// </param>
        /// 
        /// <returns>
        /// A reference to <paramref name="list"/>.
        /// </returns>
        public static CircularList<TElement> operator <<(CircularList<TElement> list, int numberOfElements)
        {
            return list >> -numberOfElements;
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets a reference to the internal <see cref="System.Collections.Generic.List"/> 
        /// encapsulated by this instance.
        /// </summary>
        internal IList<TElement> Collection
        {
            get { return m_collection; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Normalizes a user-facing element index to the
        /// internal element index from the associated list.
        /// </summary>
        /// 
        /// <param name="index">
        /// The index to translate.
        /// </param>
        /// 
        /// <returns>
        /// The normalized, internal representation of <paramref name="index"/>.
        /// </returns>
        private int ToInternalIndex(int index)
        {
            index = (index + m_headIndex) % m_collection.Count;
            return index < 0 ? m_collection.Count + index : index;
        }

        /// <summary>
        /// Normalizes an internal element index from the associated list
        /// to the user-facing element index .
        /// </summary>
        /// 
        /// <param name="index">
        /// The index to translate.
        /// </param>
        /// 
        /// <returns>
        /// The normalized, user-facing representation of <paramref name="index"/>.
        /// </returns>
        private int ToExternalIndex(int index)
        {
            // Internal index may represent error; return as-is.
            if (index < 0 || index >= m_collection.Count) { return index; }

            return (m_collection.Count - m_headIndex + index) % m_collection.Count;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IList<TElement> m_collection;
        private int m_headIndex;

        #endregion
    }
}