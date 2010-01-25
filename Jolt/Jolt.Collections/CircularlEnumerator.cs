// ----------------------------------------------------------------------------
// CircularEnumerator.cs
//
// Contains the definition of the CircularEnumerator class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/14/2009 18:28:37
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements the <see cref="System.Collections.Generic.IEnumerator"/> contract,
    /// providing an enumerator that treats its associated collection as a circular
    /// collection of items.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element stored in the associated collection.
    /// </typeparam>
    public sealed class CircularEnumerator<TElement> : IEnumerator<TElement>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CircularEnumerator"/> class.
        /// </summary>
        /// 
        /// <param name="enumerationSource">
        /// The collection to enumerate.
        /// </param>
        public CircularEnumerator(IEnumerable<TElement> enumerationSource)
        {
            m_getEnumerator = enumerationSource.GetEnumerator;
            m_enumerator = enumerationSource.GetEnumerator();
        }

        #endregion

        #region IEnumerator<TElement> members -----------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.Generic.IEnumerator.Current"/>
        /// </summary>
        public TElement Current
        {
            get { return m_enumerator.Current; }
        }

        #endregion

        #region IEnumerator members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.IEnumerator.Current"/>
        /// </summary>
        object IEnumerator.Current
        {
            get { return (m_enumerator as IEnumerator).Current; }
        }

        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// 
        /// <remarks>
        /// If the call results in passing the end of the associated collection, the
        /// enumerator is positioned at the start of the collection, emulating a cycle.
        /// 
        /// Since there is no end to a cyclical collection, using this type of enumerator
        /// in a foreach loop will result in a non-terminating loop unless an explicit
        /// break condition is authored.
        /// </remarks>
        /// 
        /// <seealso cref="System.Collections.IEnumerator.MoveNext"/>
        /// 
        /// <returns>
        /// Returns true if the enumerator was successfully moved to the next element; false otherwise.
        /// </returns>
        public bool MoveNext()
        {
            bool moreElements = m_enumerator.MoveNext();
            if (!moreElements)
            {
                // Reset() is not guaranteed to return the enumerator
                // to the beginning of the collection.
                m_enumerator = m_getEnumerator();
                moreElements = m_enumerator.MoveNext();
            }

            return moreElements;
        }

        /// <summary>
        /// <see cref="System.Collections.IEnumerator.Reset"/>
        /// </summary>
        public void Reset()
        {
            m_enumerator.Reset();
        }

        #endregion

        #region IDisposable members ---------------------------------------------------------------

        /// <summary>
        /// <see cref="System.Collections.IEnumerator.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            m_enumerator.Dispose();
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private IEnumerator<TElement> m_enumerator;
        private readonly Func<IEnumerator<TElement>> m_getEnumerator;

        #endregion
    }
}