// ----------------------------------------------------------------------------
// ILinkedList.cs
//
// Contains the definition of the ILinkedList interface.
// Copyright 2009 Steve Guidi.
//
// File created: 12/27/2009 13:04:38
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Jolt.Collections
{
    /// <summary>
    /// Do not use this type directly!
    /// It serves as a proxy interface to simplify testing of the
    /// <see cref="CirculaLinkedList"/> class, and will be auto-generated in the future.
    /// </summary>
    internal interface ILinkedList<TElement> : ICollection<TElement>, ICollection
    {
        LinkedListNode<TElement> AddAfter(LinkedListNode<TElement> node, TElement value);
        void AddAfter(LinkedListNode<TElement> node, LinkedListNode<TElement> newNode);
        void AddBefore(LinkedListNode<TElement> node, LinkedListNode<TElement> newNode);
        LinkedListNode<TElement> AddBefore(LinkedListNode<TElement> node, TElement value);
        LinkedListNode<TElement> AddFirst(TElement value);
        void AddFirst(LinkedListNode<TElement> node);
        LinkedListNode<TElement> AddLast(TElement value);
        void AddLast(LinkedListNode<TElement> node);
        LinkedListNode<TElement> Find(TElement value);
        LinkedListNode<TElement> FindLast(TElement value);
        void Remove(LinkedListNode<TElement> node);
        void RemoveFirst();
        void RemoveLast();

        LinkedListNode<TElement> First { get; }
        LinkedListNode<TElement> Last { get; }
    }
}
