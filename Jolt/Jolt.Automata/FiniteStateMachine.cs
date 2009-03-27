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

using Jolt.Automata.Properties;
using QuickGraph;

namespace Jolt.Automata
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
    [Serializable]
    public class FiniteStateMachine<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of a finite state machine.
        /// </summary>
        public FiniteStateMachine()
            : this(new BidirectionalGraph<string, Transition<TAlphabet>>()) { }

        /// <summary>
        /// Initializes a new instance of the finite state machine,
        /// copying all vertices and edges from the given graph.
        /// </summary>
        /// 
        /// <param name="graph">
        /// The graph to import into the new finite state machine.
        /// </param>
        public FiniteStateMachine(IBidirectionalGraph<string, Transition<TAlphabet>> graph)
            : this()
        {
            AddStates(graph.Vertices);
            m_graph.AddEdgeRange(graph.Edges);
        }

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
            m_graph.VertexRemoved += v => ClearFinalState(v);
            m_finalStates = new HashSet<string>();
        }

        #endregion

        #region public instance methods -----------------------------------------------------------

        /// <summary>
        /// Adds a state with the given name to the finite state machine,
        /// returning a value denoting if the state was indeed added.
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
        public virtual bool AddState(string state)
        {
            if (state == ErrorState)
            {
                throw new ArgumentException(String.Format(Resources.Error_AddState_ImplicitErrorState, ErrorState));
            }

            return m_graph.AddVertex(state);
        }

        /// <summary>
        /// Adds states with the given names to the finite state machine,
        /// returning a value denoting the number of states added.
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
        public virtual int AddStates(IEnumerable<string> states)
        {
            if (states.Contains(ErrorState))
            {
                throw new ArgumentException(String.Format(Resources.Error_AddStates_ImplicitErrorState, ErrorState));
            }

            return m_graph.AddVertexRange(states);
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
            return m_graph.RemoveVertex(state);
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
        /// Determines if the given state is a final state in the FSM.
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to validate.
        /// </param>
        public virtual bool IsFinalState(string state)
        {
            return m_finalStates.Contains(state);
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
        /// <param name="inputSymbols">
        /// The symbols to process.
        /// </param>
        public virtual ConsumptionResult<TAlphabet> Consume(IEnumerable<TAlphabet> inputSymbols)
        {
            ulong numberOfSymbols = 0;
            TAlphabet lastSymbol = default(TAlphabet);
            IFsmEnumerator<TAlphabet> enumerator = CreateStateEnumerator(m_startState);

            foreach (TAlphabet symbol in inputSymbols)
            {
                ++numberOfSymbols;
                lastSymbol = symbol;

                if (!enumerator.NextState(symbol)) { break; }
            }

            return new ConsumptionResult<TAlphabet>(
                m_finalStates.Contains(enumerator.CurrentState),
                lastSymbol,
                numberOfSymbols,
                enumerator.CurrentState);
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the internal representation of the graph.
        /// </summary>
        public virtual IBidirectionalGraph<string, Transition<TAlphabet>> AsGraph
        {
            get { return m_graph; }
        }

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
        /// Gets the final states that are associated with the finite state machine.
        /// </summary>
        public virtual IEnumerable<string> FinalStates
        {
            // Force creation of an enumerator that can not be down-cast to HashSet<string>.
            get { return m_finalStates.Select(s => s); }
        }

        #endregion

        #region internal data ---------------------------------------------------------------------
        
        internal static readonly string ErrorState = "Jolt.FSM.ImplicitErrorState";
        
        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly IMutableBidirectionalGraph<string, Transition<TAlphabet>> m_graph;
        private HashSet<string> m_finalStates;
        private string m_startState;

        #endregion
    }
}