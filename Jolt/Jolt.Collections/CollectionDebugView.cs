// ----------------------------------------------------------------------------
// CollectionDebugView.cs
//
// Contains the definition of the CollectionDebugView class.
// Copyright 2010 Steve Guidi.
//
// File created: 1/24/2010 20:06:35
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Jolt.Collections
{
    /// <summary>
    /// Implements a debugger type proxy for rendering implementations
    /// of the <see cref="System.Collections.Generic.IEnumerable"/> interface
    /// in the Visual Studio debugger.
    /// </summary>
    /// 
    /// <typeparam name="TElement">
    /// The type of element contained in the collection.
    /// </typeparam>
    internal sealed class CollectionDebugView<TElement>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionDebugView"/> class.
        /// </summary>
        /// 
        /// <param name="collection">
        /// The collection of items to be rendered by the debugger.
        /// </param>
        public CollectionDebugView(IEnumerable<TElement> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }

            m_collection = collection;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the collection of items to be rendered by the debugger as an array.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TElement[] Items
        {
            get { return m_collection.ToArray(); }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the collection of items associated with the class.
        /// </summary>
        internal IEnumerable<TElement> Collection
        {
            get { return m_collection; }
        }
        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IEnumerable<TElement> m_collection;

        #endregion
    }
}