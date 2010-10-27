// ----------------------------------------------------------------------------
// ReadOnlyDictionary.cs
//
// Contains the definition of the ReadOnlyDictionary class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/7/2010 16:12:00
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jolt.Collections
{
    /// <summary>
    /// Provides a read-only adaptor class for any <see cref="IDictionary&ltTKey, TValue&gt;"/> implementation.
    /// </summary>
    /// 
    /// <typeparam name="TKey">
    /// The type of keys in the dictionary.
    /// </typeparam>
    /// 
    /// <typeparam name="TValue">
    /// The type of values in the dictionary.
    /// </typeparam>
    /// 
    /// <remarks>
    /// Modifications to the adapted dictionary are visible throug the read-only adaptor.
    /// </remarks>
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary&ltTKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="dictionary">
        /// The dictionary that is adapted to the read-only interface.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dictionary"/> is null.
        /// </exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            ExceptionUtility.ThrowOnNullArgument(dictionary);
            m_dictionary = dictionary;
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> members -----------------------------------

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Add(KeyValuePair&lt;TKey, TValue&gt;)"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always thrown.  Operations that modify the collection are not supported.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Clear()"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always thrown.  Operations that modify the collection are not supported.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Remove(KeyValuePair&lt;TKey, TValue&gt;)"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always thrown.  Operations that modify the collection are not supported.
        /// </exception>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.IsReadOnly"/>
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        #endregion

        #region IDictionary<TKey, TValue> members -------------------------------------------------

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Add(TKey, TValue)"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always thrown.  Operations that modify the collection are not supported.
        /// </exception>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Remove(TKey)"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always thrown.  Operations that modify the collection are not supported.
        /// </exception>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Item(TKey)"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// Always when setter is invoked.  Operations that modify the collection are not supported.
        /// </exception>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return m_dictionary[key]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region IEnumerable members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.GetEnumerator()"/>
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (m_dictionary as IEnumerable).GetEnumerator();
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Count"/>
        /// </summary>
        public int Count
        {
            get { return m_dictionary.Count; }
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Item(TKey)"/>
        /// </summary>
        public TValue this[TKey key]
        {
            get { return (this as IDictionary<TKey, TValue>)[key]; }
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Keys"/>
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return m_dictionary.Keys; }
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Keys"/>
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return m_dictionary.Values; }
        }

        /// <summary>
        /// Gets a reference to the internal <see cref="System.Collections.IDictionary&lt;TKey, TValue&gt;"/> 
        /// encapsulated by this instance.
        /// </summary>
        public IDictionary<TKey, TValue> Items
        {
            get { return m_dictionary; }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Contains(KeyValuePair&lt;TKey, TValue&gt;)"/>
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_dictionary.Contains(item);
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Contains(TKey)"/>
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return m_dictionary.ContainsKey(key);
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.CopyTo(KeyValuePair&lt;TKey, TValue&gt;[], int)"/>
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            m_dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.GetEnumerator()"/>
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_dictionary.GetEnumerator();
        }

        /// <summary>
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.TryGetValue(TKey, out TValue)"/>
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IDictionary<TKey, TValue> m_dictionary;

        #endregion
    }
}