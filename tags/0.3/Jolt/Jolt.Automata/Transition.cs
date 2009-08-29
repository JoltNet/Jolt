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

namespace Jolt.Automata
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
    public sealed class Transition<TAlphabet> : Edge<string>, IEquatable<Transition<TAlphabet>>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the state of the transition, setting the transition's
        /// description to the name of the method stored in the transition predicate.
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
            : this(sourceState, targetState, transitionPredicate, transitionPredicate.Method.Name) { }

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
        /// 
        /// <param name="description">
        /// A name or description for the transition.
        /// </param>
        public Transition(string sourceState, string targetState, Predicate<TAlphabet> transitionPredicate, string description)
            : base(sourceState, targetState)
        {
            m_transitionPredicate = transitionPredicate;
            Description = description;
        }

        #endregion

        #region Object method overrides -----------------------------------------------------------

        /// <summary>
        /// Performs a value-based equality comparison for a given
        /// Transition.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The transition to compare with.
        /// </param>
        /// 
        /// <remarks>
        /// The properties that participate in the quality expression
        /// are: <see cref="Source"/>, <see cref="Target"/>, <see cref="TransitionPredicate"/>
        /// <see cref="Description"/>, and <see cref="OnTranstion"/>.
        /// 
        /// Results may vary for types TAlphabet that do not implement
        /// value-based equality semantics.
        /// </remarks>
        public override bool Equals(object obj)
        {
            Transition<TAlphabet> transition = obj as Transition<TAlphabet>;
            return Equals(transition);
        }

        /// <summary>
        /// <see cref="Object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.Source.GetHashCode() ^
                           base.Target.GetHashCode() ^
                           m_transitionPredicate.GetHashCode();
            
            if (Description != null) { hashCode ^= Description.GetHashCode(); }
            if (OnTransition != null) { hashCode ^= OnTransition.GetHashCode(); }

            return hashCode;
        }

        #endregion

        #region IEquatable<Transition<TAlphabet>> implementation ----------------------------------

        /// <summary>
        /// <see cref="Equals(Object)"/>
        /// </summary>
        public bool Equals(Transition<TAlphabet> transition)
        {
            bool areEqual = transition != null &&
                   base.Source == transition.Source &&
                   base.Target == transition.Target &&
                   m_transitionPredicate.Equals(transition.m_transitionPredicate) &&
                   Description == transition.Description;

            // Prevent transition dereference when transition == null.
            if (areEqual)
            {
                areEqual &= OnTransition == null ?
                    transition.OnTransition == null :
                    OnTransition.Equals(transition.OnTransition);

            }

            return areEqual;
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

        /// <summary>
        /// Gets/sets the transition's description.
        /// </summary>
        public string Description { get; set; }

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

        #region private instance data -------------------------------------------------------------

        private readonly Predicate<TAlphabet> m_transitionPredicate;

        #endregion
    }
}