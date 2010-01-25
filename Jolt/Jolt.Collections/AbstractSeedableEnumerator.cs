// ----------------------------------------------------------------------------
// AbstractSeedableEnumerator.cs
//
// Contains the definition of the AbstractSeedableEnumeratorclass.
// Copyright 2009 Steve Guidi.
//
// File created: 12/16/2009 15:01:27
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements the <see cref="System.Collections.Generic.IEnumerator"/> contract,
    /// providing an enumerator base type that allows for starting the enumeration
    /// from an arbitrary node.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element stored in the collection.
    /// </typeparam>
    /// 
    /// <typeparam name="TIndex">
    /// The type used to index elements from the collection.
    /// </typeparam>
    internal abstract class AbstractSeedableEnumerator<TElement, TIndex> : IEnumerator<TElement>, IDisposable
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="AbstractSeedableEnumerator"/> class.
        /// </summary>
        /// 
        /// <param name="enumerationSource">
        /// The collection being enumerated.
        /// </param>
        /// 
        /// <param name="startIndex">
        /// An instance of <see cref="TIndex"/> representing the starting position of
        /// the enumeration.
        /// </param>
        /// 
        /// <remarks>
        /// Requires that <paramref name="enumerationSource"/> implement an associated enumerator
        /// that fully conforms to the <see cref="System.Collections.IEnumerable"/> imlementation
        /// conventions.
        /// </remarks>
        internal AbstractSeedableEnumerator(IEnumerable<TElement> enumerationSource, TIndex startIndex)
        {
            m_collectionEnumerator = enumerationSource.GetEnumerator();
            m_startIndex = startIndex;
            CurrentIndex = startIndex;
        }

        #endregion

        #region IEnumerator<TElement> members -----------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.IEnumerator.Current"/>
        /// </summary>
        public abstract TElement Current { get; }

        #endregion

        #region IEnumerator members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.IEnumerator.Current"/>
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// <see cref="System.Collections.IEnumerator.Current"/>
        /// </summary>
        public bool MoveNext()
        {
            ThrowIfCollectionHasChanged();
            return MoveNextImpl();
        }

        /// <summary>
        /// Repositions the enumerator to its initial starting postion given
        /// at when the enumerator object was created.
        /// </summary>
        /// 
        /// <seealso cref="System.Collections.IEnumerator.Reset"/>
        public void Reset()
        {
            ThrowIfCollectionHasChanged();
            CurrentIndex = m_startIndex;
        }

        #endregion

        #region IDisposable members ---------------------------------------------------------------

        /// <summary>
        /// Releases all disposable resources held by this enumerator.
        /// </summary>
        public void Dispose()
        {
            m_collectionEnumerator.Dispose();
        }

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets/sets the current position of the enumerator.
        /// </summary>
        protected internal TIndex CurrentIndex { get; set; }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Provides the implementation of advancing the enumerator to the next
        /// element in the collection.
        /// </summary>
        /// 
        /// <returns>
        /// Returns true if the enumerator was successfully moved to the next element; false otherwise.
        /// </returns>
        protected abstract bool MoveNextImpl();

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Throws an exception if the associated collection has changed while this
        /// enumerator is active.
        /// </summary>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// The associated collection has changed.
        /// </exception>
        private void ThrowIfCollectionHasChanged()
        {
            m_collectionEnumerator.Reset(); // Throws if collection is dirty.
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IEnumerator<TElement> m_collectionEnumerator;
        private readonly TIndex m_startIndex;

        #endregion
    }
}