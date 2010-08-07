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
    /// Implements the <see cref="AbstractFsmEnumerator"/> base class, providing an
    /// enumerator that traverses a deterministic finite state machine in the
    /// standard manner.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    internal sealed class FsmEnumerator<TAlphabet> : AbstractFsmEnumerator<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="FsmEnumerator"/> class.
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
            : base(startState, graph)
        { }

        #endregion

        #region AbstractFsmEnumerator<TAlphabet> members ------------------------------------------

        /// <summary>
        /// <see cref="AbstractFsmEnumerator&lt;TAlphabet&gt;.Next"/>
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// A non deterministic transition is detected in the FSM.
        /// </exception>
        protected override bool Next(TAlphabet inputSymbol)
        {
            if (IsInErrorState) { return false; }

            Transition<TAlphabet> transition;

            try
            {
                // Find the single transtrition that is accepted by the input symbol.
                transition = Graph.OutEdges(This.CurrentState).SingleOrDefault(t => t.TransitionPredicate(inputSymbol));
            }
            catch (InvalidOperationException)
            {
                throw new NotSupportedException(
                    String.Format(Resources.Error_NondeterministicEnumeration, This.CurrentState, inputSymbol.ToString()));
            }

            m_currentStates.Clear();
            if (transition != null)
            {
                // This code must not run in the try block as a user-defined event handler may raise
                // the InvalidOperationException.
                transition.RaiseOnTransitionEvent(new StateTransitionEventArgs<TAlphabet>(transition.Source, inputSymbol));
                m_currentStates.Add(transition.Target);
            }
            else
            {
                m_currentStates.Add(FiniteStateMachine<TAlphabet>.ErrorState);
                IsInErrorState = true;
            }

            return !IsInErrorState;
        }

        #endregion
    }
}