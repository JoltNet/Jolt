// ----------------------------------------------------------------------------
// CircularListEnumerator.cs
//
// Contains the definition of the CircularListEnumerator class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/16/2009 07:29:22
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements the <see cref="AbstractSeedableEnumerator"/> base class providing
    /// an enumerator that treats its associated <see cref="System.Collections.Generic.IList"/>
    /// as a circular collection of items.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element stored in the associated collection.
    /// </typeparam>
    internal sealed class CircularListEnumerator<TElement> : AbstractSeedableEnumerator<TElement, int>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CircularListEnumerator"/> class.
        /// </summary>
        /// 
        /// <param name="enumerationSource">
        /// The list being enumerated.
        /// </param>
        /// 
        /// <param name="startIndex">
        /// The index of the list representing the starting position of the enumeration.
        /// </param>
        internal CircularListEnumerator(IList<TElement> enumerationSource, int startIndex)
            : base(enumerationSource, startIndex)
        {
            m_collection = enumerationSource;
            m_enumerationStarted = false;
        }

        #endregion

        #region AbstractSeedableEnumerator members ------------------------------------------------

        /// <summary>
        /// Gets the element positioned at <see cref="CurrentIndex"/>.
        /// </summary>
        public override TElement Current
        {
            get { return m_enumerationStarted ? m_collection[CurrentIndex] : default(TElement); }
        }

        /// <summary>
        /// Advances the enumerator to the next element in the list.
        /// </summary>
        /// 
        /// <remarks>
        /// If the enumerator currently references the end of the list, the
        /// enumerator is positioned at the start of the list, emulating a cycle.
        /// </remarks>
        /// 
        /// <returns>
        /// Returns true if the enumerator was successfully moved to the next element;
        /// false if the collection is empty.
        /// </returns>
        protected override bool MoveNextImpl()
        {
            if (m_collection.Count == 0) { return false; }
            
            if (!m_enumerationStarted)
            {
                // Emulates the enumerator being positioned before the start element.
                m_enumerationStarted = true;
            }
            else
            {
                CurrentIndex = ++CurrentIndex % m_collection.Count;
            }
            
            return true;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IList<TElement> m_collection;
        private bool m_enumerationStarted;

        #endregion
    }
}