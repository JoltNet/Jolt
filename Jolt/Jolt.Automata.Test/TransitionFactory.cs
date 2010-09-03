// ----------------------------------------------------------------------------
// TransitionFactory.cs
//
// Contains the definition of the TransitionFactory class.
// Copyright 2010 Steve Guidi.
//
// File created: 9/2/2010 22:28:26
// ----------------------------------------------------------------------------

using System;

using Jolt.Functional;
using Jolt.Testing.Assertions;

namespace Jolt.Automata.Test
{
    /// <summary>
    /// Implements a factory for creating and modifying
    /// <see cref="Transition&lt;char&gt;"/> instances.
    /// </summary>
    internal sealed class TransitionFactory : IArgumentFactory<Transition<char>>, IEquatableFactory<Transition<char>>
    {
        /// <summary>
        /// Creates and returns a new instance of
        /// the <see cref="Transition&lt;char&gt;"/> class.
        /// </summary>
        public Transition<char> Create()
        {
            Transition<char> transition = new Transition<char>("start", "final", Char.IsDigit);
            transition.OnTransition += Functor.ToEventHandler(Functor.NoOperation<object, StateTransitionEventArgs<char>>());

            return transition;
        }

        /// <summary>
        /// Modified an existing instance of
        /// the <see cref="Transition&lt;char&gt;"/> class.
        /// </summary>
        public void Modify(ref Transition<char> instance)
        {
            instance.Description = Guid.NewGuid().ToString("N");
        }
    }
}