// ----------------------------------------------------------------------------
// FsmEnumerator.cs
//
// Contains the definition of the FsmEnumerator class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 20:17:32
// ----------------------------------------------------------------------------

using System;
using System.Linq;

using Jolt.Automata.Properties;
using QuickGraph;

namespace Jolt.Automata
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
        internal FsmEnumerator(string startState, IImplicitGraph<string, Transition<TAlphabet>> graph)
        {
            m_graph = graph;
            CurrentState = startState;
        }

        #endregion

        #region IFsmEnumerator<TAlphabet> members -------------------------------------------------

        /// <see cref="IFsmEnumerator&lt;TAlphabet&gt;.NextState"/>
        public bool NextState(TAlphabet inputSymbol)
        {
            if (!m_isInErrorState)
            {
                Transition<TAlphabet> transition;

                try
                {
                    // Find the single transtrition that is accepted by the input symbol.
                    transition = m_graph.OutEdges(CurrentState).SingleOrDefault(t => t.TransitionPredicate(inputSymbol));
                }
                catch (InvalidOperationException)
                {
                    throw new NotSupportedException(
                        String.Format(Resources.Error_NDFSM_NotSupported, CurrentState, inputSymbol.ToString()));
                }

                if (transition != null)
                {
                    transition.RaiseOnTransitionEvent(new StateTransitionEventArgs<TAlphabet>(transition.Source, inputSymbol));
                    CurrentState = transition.Target;
                    return true;
                }

                CurrentState = FiniteStateMachine<TAlphabet>.ErrorState;
                m_isInErrorState = true;
            }

            return false;
        }

        /// <see cref="IFsmEnumerator&lt;TAlphabet&gt;.CurrentState"/>
        public string CurrentState { get; private set; }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IImplicitGraph<string, Transition<TAlphabet>> m_graph;
        private bool m_isInErrorState;

        #endregion
    }
}