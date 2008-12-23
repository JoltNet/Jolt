// ----------------------------------------------------------------------------
// FiniteStateMachine.cs
//
// Contains the definition of the FiniteStateMachine class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 08:43:25
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Jolt.Properties;
using QuickGraph;

namespace Jolt
{
    /// <summary>
    /// Provides methods to create and manage a finite state machine (FSM),
    /// its states, and its transitions.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    /// 
    /// <remarks>
    /// The FSM is modelled as a graph, and may converted
    /// to and from the <see cref="QuickGraph.IGraph"/> data type.
    /// </remarks>
    public class FiniteStateMachine<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of a finite state machine.
        /// </summary>
        public FiniteStateMachine()
            : this(new BidirectionalGraph<string, Transition<TAlphabet>>()) { }

        /// <summary>
        /// Initializes a new instance of a finite state machine
        /// from a predefined graph.
        /// </summary>
        /// 
        /// <param name="graph">
        /// A graph containing transitions and states.
        /// </param>
        internal FiniteStateMachine(IMutableBidirectionalGraph<string, Transition<TAlphabet>> graph)
        {
            m_graph = graph;
            m_finalStates = new HashSet<string>();
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a state with the given name to the finite state machine.
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to be added.
        /// </param>
        /// 
        /// <remarks>
        /// The given state name must be unique within the set of states in
        /// the finite state machine.
        /// </remarks>
        public virtual void AddState(string state)
        {
            m_graph.AddVertex(state);
        }

        /// <summary>
        /// Adds states with the given names to the finite state machine.
        /// </summary>
        /// 
        /// <param name="states">
        /// The states to be added.
        /// </param>
        /// 
        /// <remarks>
        /// The given state names must be unique within the set of states in
        /// the finite state machine.
        /// </remarks>
        public virtual void AddStates(IEnumerable<string> states)
        {
            m_graph.AddVertexRange(states);
        }

        /// <summary>
        /// Removes the state with the given name from the finite state machine,
        /// returning a value denoting if the state was removed (i.e. if it exists
        /// in the finite state machine prior to removing).
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to remove.
        /// </param>
        public virtual bool RemoveState(string state)
        {
            bool isRemoved = m_graph.RemoveVertex(state);
            if (isRemoved) { ClearFinalState(state); }

            return isRemoved;
        }

        /// <summary>
        /// Adds the given transition to the finite state machine.
        /// </summary>
        /// 
        /// <param name="transition">
        /// The transition to be added.
        /// </param>
        /// 
        /// <remarks>
        /// The states involved in the given transition must exist as
        /// part of the finite state machine.
        /// </remarks>
        public virtual void AddTransition(Transition<TAlphabet> transition)
        {
            m_graph.AddEdge(transition);
        }

        /// <summary>
        /// Removes the given transition from the finite state machine,
        /// returning a value denoting if the transition was removed.
        /// </summary>
        /// 
        /// <param name="transition">
        /// The transition to remote.
        /// </param>
        public virtual bool RemoveTransition(Transition<TAlphabet> transition)
        {
            return m_graph.RemoveEdge(transition);
        }

        /// <summary>
        /// Marks an existing state within the finite state machine as a final state.
        /// </summary>
        public virtual void SetFinalState(string state)
        {
            if (!m_graph.ContainsVertex(state))
            {
                throw new ArgumentException(String.Format(Resources.Error_SetFinalState_InvalidState, state));
            }

            m_finalStates.Add(state);
        }

        /// <summary>
        /// Marks existing states within the finite state machine as final states.
        /// </summary>
        public virtual void SetFinalStates(IEnumerable<string> states)
        {
            if (!states.All(m_graph.ContainsVertex))
            {
                throw new ArgumentException(Resources.Error_SetFinalStates_InvalidState);
            }

            m_finalStates.UnionWith(states);
        }

        /// <summary>
        /// Removes the final state marking from the given state, returns a value
        /// denoting if the final state was unmarked (i.e. it was previously marked as
        /// a final state).
        /// </summary>
        /// 
        /// <param name="state">
        /// The final state to unmark.
        /// </param>
        public virtual bool ClearFinalState(string state)
        {
            return m_finalStates.Remove(state);
        }

        /// <summary>
        /// Removes the final state marking from any of the given states that are
        /// final states within the finite state machine.
        /// </summary>
        /// 
        /// <param name="states">
        /// The final states to unmark.
        /// </param>
        public virtual void ClearFinalStates(IEnumerable<string> states)
        {
            m_finalStates.ExceptWith(states);
        }

        /// <summary>
        /// Creates an enumerator for exercising the transitions of the FSM.
        /// </summary>
        /// 
        /// <param name="startState">
        /// The initial state of the FSM enumerator.
        /// </param>
        public virtual IFsmEnumerator<TAlphabet> CreateStateEnumerator(string startState)
        {
            if (!m_graph.ContainsVertex(startState))
            {
                throw new ArgumentException(String.Format(Resources.Error_CreateEnumerator_InvalidStartState, startState));
            }

            return new FsmEnumerator<TAlphabet>(startState, m_graph);
        }

        /// <summary>
        /// Determines if the the finite state machine accepts the
        /// given sequence of input symbols.
        /// </summary>
        /// 
        /// <param name="startState">
        /// The from which the acceptance processing begins.
        /// </param>
        /// 
        /// <param name="inputSymbols">
        /// The symbols to process.
        /// </param>
        public virtual bool Accepts(IEnumerable<TAlphabet> inputSymbols)
        {
            IFsmEnumerator<TAlphabet> enumerator = CreateStateEnumerator(m_startState);
            return inputSymbols.All(enumerator.NextState) && m_finalStates.Contains(enumerator.CurrentState);
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets/marks an existing state with the finite state machine as the start state.
        /// </summary>
        public virtual string StartState
        {
            get { return m_startState; }
            set
            {
                if (!m_graph.ContainsVertex(value))
                {
                    throw new ArgumentException(String.Format(Resources.Error_SetStartState_InvalidState, value));
                }

                m_startState = value;
            }
        }

        /// <summary>
        /// Gets a the final states that are associated with the finite state machine.
        /// </summary>
        public virtual ICollection<string> FinalStates
        {
            // Create a shallow copy of the final state collection, preventing external
            // modification of the private member data.
            get { return new HashSet<string>(m_finalStates); }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the internal representation of the graph.
        /// </summary>
        internal IBidirectionalGraph<string, Transition<TAlphabet>> Graph
        {
            get { return m_graph; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly IMutableBidirectionalGraph<string, Transition<TAlphabet>> m_graph;
        private HashSet<string> m_finalStates;
        private string m_startState;

        #endregion
    }
}