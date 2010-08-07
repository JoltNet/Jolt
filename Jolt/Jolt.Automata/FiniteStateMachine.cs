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
using Jolt.Linq;
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
        /// Creates a new instance of the <see cref="FiniteStateMachine"/> class.
        /// </summary>
        public FiniteStateMachine()
            : this(new BidirectionalGraph<string, Transition<TAlphabet>>()) { }

        /// <summary>
        /// Creates a new instance of the <see cref="FiniteStateMachine"/> class,
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
        /// Creates a new instance of the <see cref="FiniteStateMachine"/> class,
        /// setting the internal graph reference to the given graph.
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

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a state to the <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <param name="state">
        /// The name of the state to add.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the state was successfully added to the FSM, false otherwise.
        /// </returns>
        /// 
        /// <remarks>
        /// <paramref name="state"/> must be unique within the set of states in
        /// the <see cref="FiniteStateMachine"/>.
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentException">
        /// A reserved name was given for <paramref name="state"/>.
        /// </exception>
        public virtual bool AddState(string state)
        {
            if (state == ErrorState)
            {
                throw new ArgumentException(String.Format(Resources.Error_AddState_ImplicitErrorState, ErrorState));
            }

            return m_graph.AddVertex(state);
        }

        /// <summary>
        /// Adds a collection of states to the <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <param name="states">
        /// The names of the states to add.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the state was successfully added to the FSM, false otherwise.
        /// </returns>
        /// 
        /// <remarks>
        /// <paramref name="states"/> must be unique within the set of states in
        /// the <see cref="FiniteStateMachine"/>.
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentException">
        /// A reserved name was given in <paramref name="states"/>.
        /// </exception>
        public virtual int AddStates(IEnumerable<string> states)
        {
            if (states.Contains(ErrorState))
            {
                throw new ArgumentException(String.Format(Resources.Error_AddStates_ImplicitErrorState, ErrorState));
            }

            return m_graph.AddVertexRange(states);
        }

        /// <summary>
        /// Removes a state from the <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to remove.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <paramref name="state"/> was found and removed from the FSM, false otherwise.
        /// </returns>
        public virtual bool RemoveState(string state)
        {
            return m_graph.RemoveVertex(state);
        }

        /// <summary>
        /// Adds a <see cref="Transition"/> to the <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <param name="transition">
        /// The <see cref="Transition"/> to add.
        /// </param>
        /// 
        /// <remarks>
        /// The states involved in <paramref name="transition"/> must have already
        /// been added to the FSM.
        /// </remarks>
        public virtual void AddTransition(Transition<TAlphabet> transition)
        {
            m_graph.AddEdge(transition);
        }

        /// <summary>
        /// Removes a <see cref="Transition"/> from the <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <param name="transition">
        /// The <see cref="Transition"/> to remove.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <paramref name="transition"/> was found and removed from the FSM, false otherwise.
        /// </returns>
        public virtual bool RemoveTransition(Transition<TAlphabet> transition)
        {
            return m_graph.RemoveEdge(transition);
        }

        /// <summary>
        /// Designates a state as a final state.
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to designate a final state.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// <paramref name="state"/> does not exist in the FSM.
        /// </exception>
        public virtual void SetFinalState(string state)
        {
            if (!m_graph.ContainsVertex(state))
            {
                throw new ArgumentException(String.Format(Resources.Error_SetFinalState_InvalidState, state));
            }

            m_finalStates.Add(state);
        }

        /// <summary>
        /// Designates a collection of states as finals states.
        /// </summary>
        /// 
        /// <param name="states">
        /// The states to designate as final states.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// At least one state in <paramref name="states"/> does not exist in the FSM.
        /// </exception>
        public virtual void SetFinalStates(IEnumerable<string> states)
        {
            if (!states.All(m_graph.ContainsVertex))
            {
                throw new ArgumentException(Resources.Error_SetFinalStates_InvalidState);
            }

            m_finalStates.UnionWith(states);
        }

        /// <summary>
        /// Clears the final state marking of a given state.
        /// </summary>
        /// 
        /// <param name="state">
        /// The final state to unmark.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <paramref name="state"/> exists in the FSM and is successfully
        /// unmarked as a final state, false otherwise.
        /// </returns>
        public virtual bool ClearFinalState(string state)
        {
            return m_finalStates.Remove(state);
        }

        /// <summary>
        /// Clears the final state marking of a given collection of states.
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
        /// Determines if the given state is a final state.
        /// </summary>
        /// 
        /// <param name="state">
        /// The state to validate.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <paramref name="state"/> is a final state, false otherwise.
        /// </returns>
        public virtual bool IsFinalState(string state)
        {
            return m_finalStates.Contains(state);
        }

        /// <summary>
        /// Creates a new concrete instance of the <see cref="IFsmEnumerator&lt;T&gt;"/> type.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type of enumerator to create.
        /// </param>
        /// 
        /// <param name="startState">
        /// The initial state of the FSM enumerator.
        /// </param>
        /// 
        /// <returns>
        /// A new FSM enumerator corresponding to the requested type, with <paramref name="startState"/>
        /// set to the initial state of enumeration.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentException">
        /// <paramref name="startState"/> is not a state in the FSM.
        /// </exception>
        public virtual IFsmEnumerator<TAlphabet> CreateStateEnumerator(EnumerationType type, string startState)
        {
            if (!m_graph.ContainsVertex(startState))
            {
                throw new ArgumentException(String.Format(Resources.Error_CreateEnumerator_InvalidStartState, startState));
            }

            // TODO: replace with a table lookup when EnumerationType grows.
            switch (type)
            {
                case EnumerationType.Nondeterministic:
                    return new NDFsmEnumerator<TAlphabet>(startState, m_graph);

                case EnumerationType.Deterministic:
                default:
                    return new FsmEnumerator<TAlphabet>(startState, m_graph);
            }
        }

        /// <summary>
        /// Determines if the the <see cref="FiniteStateMachine"/> accepts a
        /// given sequence of input symbols.
        /// </summary>
        /// 
        /// <param name="inputSymbols">
        /// The symbols to process.
        /// </param>
        /// 
        /// <returns>
        /// Returns a <see cref="ConsumptionResult"/> object denoting the success
        /// or failure of the input symbol processing.
        /// </returns>
        /// 
        /// <remarks>
        /// Consumes the given set of input symbols from the configured start state of the FSM,
        /// until a symbol is encountered that does not cause a transition, or all symbols are
        /// successfully consumed.
        /// </remarks>
        public virtual ConsumptionResult<TAlphabet> Consume(EnumerationType type, IEnumerable<TAlphabet> inputSymbols)
        {
            ulong numberOfSymbols = 0;
            TAlphabet lastSymbol = default(TAlphabet);
            IFsmEnumerator<TAlphabet> enumerator = CreateStateEnumerator(type, m_startState);

            foreach (TAlphabet symbol in inputSymbols)
            {
                ++numberOfSymbols;
                lastSymbol = symbol;

                if (!enumerator.Next(symbol)) { break; }
            }

            return new ConsumptionResult<TAlphabet>(
                m_finalStates.Overlaps(enumerator.CurrentStates),
                lastSymbol,
                numberOfSymbols,
                enumerator.CurrentStates);
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the internal representation of the <see cref="FiniteStateMachine"/> as a graph.
        /// </summary>
        public virtual IBidirectionalGraph<string, Transition<TAlphabet>> AsGraph
        {
            get { return m_graph; }
        }

        /// <summary>
        /// Gets/marks an existing state with the finite state machine as the start state.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentException">
        /// The state mared as a start state does not exist in the FSM>
        /// </exception>
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
        /// Enumerates the final states of the <see cref="FiniteStateMachine"/>.
        /// </summary>
        public virtual IEnumerable<string> FinalStates
        {
            get { return m_finalStates.AsNonCastableEnumerable(); }
        }

        #endregion

        #region internal fields -------------------------------------------------------------------
        
        /// <summary>
        /// The name of the implicit error state for all FSMs.
        /// </summary>
        internal static readonly string ErrorState = "Jolt.FSM.ImplicitErrorState";
        
        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IMutableBidirectionalGraph<string, Transition<TAlphabet>> m_graph;
        private HashSet<string> m_finalStates;
        private string m_startState;

        #endregion
    }
}