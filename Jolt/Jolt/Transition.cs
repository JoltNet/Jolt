// ----------------------------------------------------------------------------
// Transition.cs
//
// Contains the definition of the Transition class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 10:54:23
// ----------------------------------------------------------------------------

using System;

using QuickGraph;

namespace Jolt
{
    /// <summary>
    /// Represents a transition between two vertices in a finite
    /// state machine (FSM).
    /// <seealso cref="FiniteStateMachine"/>
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    [Serializable]
    public sealed class Transition<TAlphabet> : Edge<string>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the state of the transition.
        /// </summary>
        /// 
        /// <param name="sourceState">
        /// The source state of the transition.
        /// </param>
        /// 
        /// <param name="targetState">
        /// The destination state of the transition.
        /// </param>
        /// 
        /// <param name="transitionPredicate">
        /// A predicate that determines if the state transition is valid,
        /// given an input value from the FSM alphabet.
        /// </param>
        public Transition(string sourceState, string targetState, Predicate<TAlphabet> transitionPredicate)
            : base(sourceState, targetState)
        {
            m_transitionPredicate = transitionPredicate;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the predicate associate with the transition.
        /// </summary>
        public Predicate<TAlphabet> TransitionPredicate
        {
            get { return m_transitionPredicate; }
        }

        #endregion

        #region public events ---------------------------------------------------------------------

        /// <summary>
        /// The OnTransition event is raised each time the transition's
        /// TransitionPredicate evaluates to TRUE, as a result of input
        /// symbol consumption.
        /// </summary>
        public event EventHandler<StateTransitionEventArgs<TAlphabet>> OnTransition;

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Raises the OnTransition event with the given arugument.
        /// </summary>
        /// 
        /// <param name="eventArgs">
        /// The arguments raised by the event.
        /// </param>
        internal void RaiseOnTransitionEvent(StateTransitionEventArgs<TAlphabet> eventArgs)
        {
            if (OnTransition != null) { OnTransition(null, eventArgs); }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly Predicate<TAlphabet> m_transitionPredicate;

        #endregion
    }
}