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
        internal FiniteStateMachine(BidirectionalGraph<string, Transition<TAlphabet>> graph)
        {
            m_graph = graph;
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
                throw new ArgumentException(String.Format(Resources.Error_InvalidStartState, startState));
            }

            return new FsmEnumerator<TAlphabet>(startState, m_graph);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the internal representation of the graph.
        /// </summary>
        internal IGraph<string, Transition<TAlphabet>> Graph
        {
            get { return m_graph; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly BidirectionalGraph<string, Transition<TAlphabet>> m_graph;

        #endregion
    }
}