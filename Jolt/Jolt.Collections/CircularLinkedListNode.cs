// ----------------------------------------------------------------------------
// CircularLinkedListNode.cs
//
// Contains the definition of the CircularLinkedListNode class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/19/2009 14:45:04
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Jolt.Collections
{
    public class CircularLinkedListNode<TElement> : IEquatable<CircularLinkedListNode<TElement>>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="CircularLinkedListNode"/> structure.
        /// </summary>
        /// 
        /// <param name="value">
        /// The value contained in the <see cref="CircularLinkedListNode"/>.
        /// </param>
        public CircularLinkedListNode(TElement value)
            : this(null, new LinkedListNode<TElement>(value)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularLinkedListNode"/> structure,
        /// adapting a given <see cref="System.Collections.Generic.LinkedListNode"/> object,
        /// and associating the node with a given <see cref="CircualrLinkedList"/>.
        /// </summary>
        /// 
        /// <param name="list">
        /// The <see cref="CircualrLinkedList"/> to associate with the node.
        /// </param>
        /// 
        /// <param name="node">
        /// The adapted <see cref="System.Collections.Generic.LinkedListNode"/>.
        /// </param>
        internal CircularLinkedListNode(CircularLinkedList<TElement> list, LinkedListNode<TElement> node)
        {
            m_list = list;
            m_node = node;
        }

        #endregion

        #region IEquatable<CircularLinkedListNode<TElement>> members ------------------------------

        public bool Equals(CircularLinkedListNode<TElement> other)
        {
            return Object.ReferenceEquals(m_node, other.m_node);
        }

        #endregion

        #region Object members --------------------------------------------------------------------

        public override bool Equals(object obj)
        {
            return obj is CircularLinkedListNode<TElement> &&
                   Equals(obj as CircularLinkedListNode<TElement>);
        }

        public override int GetHashCode()
        {
            return m_node.GetHashCode();
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets/sets the value contained in the <see cref="CircularLinkedListNode"/>.
        /// </summary>
        public TElement Value
        {
            get { return m_node.Value; }
            set { m_node.Value = value; }
        }

        /// <summary>
        /// Gets the <see cref="CircularLinkedList"/> associated with this node.
        /// </summary>
        public CircularLinkedList<TElement> List
        {
            get { return m_list; }
        }

        /// <summary>
        /// Gets the next node in the <see cref="CircularLinkedList"/>, immediately
        /// following this node.
        /// </summary>
        public CircularLinkedListNode<TElement> Next
        {
            get
            {
                LinkedListNode<TElement> nextNode;
                if (m_node == m_node.List.Last) { nextNode = m_node.List.First; }
                else { nextNode = m_node.Next; }

                return new CircularLinkedListNode<TElement>(m_list, nextNode);
            }
        }

        /// <summary>
        /// Gets the next node in the <see cref="CircularLinkedList"/>, immediately
        /// preceding this node.
        /// </summary>
        public CircularLinkedListNode<TElement> Previous
        {
            get
            {
                LinkedListNode<TElement> previousNode;
                if (m_node == m_node.List.First) { previousNode = m_node.List.Last; }
                else { previousNode = m_node.Previous; }

                return new CircularLinkedListNode<TElement>(m_list, previousNode);
            }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="System.Collections.Generic.LinkedListNode"/> encapsulated
        /// by this instance.
        /// </summary>
        internal LinkedListNode<TElement> ListNode
        {
            get { return m_node; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly CircularLinkedList<TElement> m_list;
        private LinkedListNode<TElement> m_node;

        #endregion
    }
}