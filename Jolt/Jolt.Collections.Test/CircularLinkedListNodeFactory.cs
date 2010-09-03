// ----------------------------------------------------------------------------
// CircularLinkedListNodeFactory.cs
//
// Contains the definition of the CircularLinkedListNodeFactory class.
// Copyright 2010 Steve Guidi.
//
// File created: 9/2/2010 23:10:21
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

using Jolt.Testing.Assertions;

namespace Jolt.Collections.Test
{
    /// <summary>
    /// Implements a factory for creating and modifying
    /// instances of <see cref="CircularLinkedListNode&lt;int&gt;"/>
    /// </summary>
    internal sealed class CircularLinkedListNodeFactory : IArgumentFactory<CircularLinkedListNode<int>>, IEquatableFactory<CircularLinkedListNode<int>>
    {
        /// <summary>
        /// Creates and returns a new instance of the
        /// <see cref="CircularLinkedListNode&lt;int&gt;"/> class.
        /// </summary>
        public CircularLinkedListNode<int> Create()
        {
            return new CircularLinkedListNode<int>(null, DefaultNode);
        }

        /// <summary>
        /// Modifies an existing instance of the 
        /// <see cref="CircularLinkedListNode&lt;int&gt;"/> class.
        /// </summary>
        public void Modify(ref CircularLinkedListNode<int> instance)
        {
            instance.GetType()
                    .GetField("m_node", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(instance, new LinkedListNode<int>(345));
        }


        private static readonly LinkedListNode<int> DefaultNode = new LinkedListNode<int>(123);
    }
}