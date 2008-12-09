// ----------------------------------------------------------------------------
// FsmEnumerator.cs
//
// Contains the definition of the FsmEnumerator class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 20:17:32
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Jolt.Properties;
using QuickGraph;

namespace Jolt
{
    /// <summary>
    /// Implements the <see cref="IFsmEnumerator"/> interface, providing an
    /// enumerator that traverses a finite state machine in the standard manner.
    /// </summary>
    internal sealed class FsmEnumerator<TAlphabet> : IFsmEnumerator<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the internal state of the enumerator.
        /// </summary>
        /// 
        /// <param name="startState">
        /// The initial state referenced by the enumerator.
        /// </param>
        /// 
        /// <param name="graph">
        /// The FSM to navigate, represented as a graph.
        /// </param>
        internal FsmEnumerator(string startState, BidirectionalGraph<string, Transition<TAlphabet>> graph)
        {
            m_graph = graph;
            CurrentState = startState;
        }

        #endregion

        #region IFsmEnumerator<TAlphabet> Members -------------------------------------------------

        /// <see cref="IFsmEnumerator&lt;TAlphabet&gt;.NextState"/>
        public bool NextState(TAlphabet inputSymbol)
        {
            Transition<TAlphabet> transition;

            try
            {
                transition = m_graph.OutEdges(CurrentState).SingleOrDefault(t => t.TransitionPredicate(inputSymbol));
            }
            catch (InvalidOperationException)
            {
                throw new NotSupportedException(
                    String.Format(Resources.Error_NDFSM_NotSupported, CurrentState, inputSymbol.ToString()));
            }

            bool foundTransition = transition != null;
            if (foundTransition)
            {
                CurrentState = transition.Target;
            }

            return foundTransition;
        }

        /// <see cref="IFsmEnumerator&lt;TAlphabet&gt;.CurrentState"/>
        public string CurrentState { get; private set; }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly BidirectionalGraph<string, Transition<TAlphabet>> m_graph;

        #endregion
    }
}