// ----------------------------------------------------------------------------
// NDFsmEnumerator.cs
//
// Contains the definition of the NDFsmEnumerator class.
// Copyright 2010 Steve Guidi.
//
// File created: 2/9/2010 22:16:48
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Jolt.Linq;
using QuickGraph;

namespace Jolt.Automata
{
    /// <summary>
    /// Implements the <see cref="AbstractFsmEnumerator"/> base class, providing an
    /// enumerator that traverses a nondeterministic finite state machine
    /// in the standard manner.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    internal sealed class NDFsmEnumerator<TAlphabet> : AbstractFsmEnumerator<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="NDFsmEnumerator"/> class.
        /// </summary>
        /// 
        /// <param name="startState">
        /// The initial state referenced by the enumerator.
        /// </param>
        /// 
        /// <param name="graph">
        /// The FSM to navigate, represented as a graph.
        /// </param>
        internal NDFsmEnumerator(string startState, IImplicitGraph<string, Transition<TAlphabet>> graph)
            : base(startState, graph)
        { }

        #endregion

        #region AbstractFsmEnumerator<TAlphabet> members ------------------------------------------

        /// <summary>
        /// <see cref="AbstractFsmEnumerator&lt;TAlphabet&gt;.NextState"/>
        /// </summary>
        /// 
        /// <remarks>
        /// Performs multiple state transitions from the current state(s), if possible,
        /// resulting in more than one target state.
        /// </remarks>
        protected override bool Next(TAlphabet inputSymbol)
        {
            if (IsInErrorState) { return false; }

            HashSet<string> nextStates = new HashSet<string>();
            
            foreach (Transition<TAlphabet> transition in m_currentStates
                .SelectMany<string, Transition<TAlphabet>>(Graph.OutEdges)
                .Where(t => t.TransitionPredicate(inputSymbol)))
            {
                nextStates.Add(transition.Target);
                transition.RaiseOnTransitionEvent(new StateTransitionEventArgs<TAlphabet>(transition.Source, inputSymbol));
            }

            if (nextStates.Count == 0)
            {
                nextStates.Add(FiniteStateMachine<TAlphabet>.ErrorState);
                IsInErrorState = true;
            }

            m_currentStates = nextStates;
            return !IsInErrorState;
        }

        #endregion
    }
}
