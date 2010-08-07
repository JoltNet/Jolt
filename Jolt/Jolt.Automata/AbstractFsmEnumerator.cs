// ----------------------------------------------------------------------------
// AbstractFsmEnumerator.cs
//
// Contains the definition of the AbstractFsmEnumerator class.
// Copyright 2010 Steve Guidi.
//
// File created: 2/9/2010 22:28:16
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Jolt.Linq;
using QuickGraph;

namespace Jolt.Automata
{
    /// <summary>
    /// Denotes the type of enumeration and input consumption for an arbitrary
    /// <see cref="FiniteStateMachine&lt;T&gt;"/>.
    /// </summary>
    public enum EnumerationType
    {
        Deterministic,
        Nondeterministic
    }


    /// <summary>
    /// Defines the common functionality for all implementations of 
    /// an enumerator for the a <see cref="FiniteStateMachine&lt;T&gt;"/> (FSM).
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    internal abstract class AbstractFsmEnumerator<TAlphabet> : IFsmEnumerator<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="AbstractFsmEnumerator"/> class.
        /// </summary>
        /// 
        /// <param name="startState">
        /// The initial state referenced by the enumerator.
        /// </param>
        ///
        /// <param name="graph">
        /// The FSM to navigate, represented as a graph.
        /// </param>
        internal AbstractFsmEnumerator(string startState, IImplicitGraph<string, Transition<TAlphabet>> graph)
        {
            IsInErrorState = false;
            m_graph = graph;
            m_currentStates = new HashSet<string>() { startState };
        }

        #endregion

        #region IFsmEnumerator<TAlphabet> members -------------------------------------------------

        /// <summary>
        /// <see cref="IFsmEnumerator&lt;T&gt;.CurrentState"/>
        /// </summary>
        string IFsmEnumerator<TAlphabet>.CurrentState
        {
            get { return m_currentStates.FirstOrDefault(); }
        }

        /// <summary>
        /// <see cref="IFsmEnumerator&lt;T&gt;.CurrentStates"/>
        /// </summary>
        IEnumerable<string> IFsmEnumerator<TAlphabet>.CurrentStates
        {
            get { return m_currentStates.AsNonCastableEnumerable(); }
        }

        /// <summary>
        /// <see cref="IFsmEnumerator&lt;T&gt;.NextState"/>
        /// </summary>
        bool IFsmEnumerator<TAlphabet>.Next(TAlphabet inputSymbol)
        {
            return Next(inputSymbol);
        }

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets the FSM associated with the enumerator, represented as a graph.
        /// </summary>
        protected IImplicitGraph<string, Transition<TAlphabet>> Graph
        {
            get { return m_graph; }
        }

        /// <summary>
        /// Gets/sets thev value denoting if the enumerator is in the error state.
        /// </summary>
        protected bool IsInErrorState { get; set; }

        /// <summary>
        /// Gets a reference to the 'this' pointer casted to an <see cref="IFsmEnumerator&lt;T&gt;"/>.
        /// </summary>
        protected IFsmEnumerator<TAlphabet> This
        {
            get { return this as IFsmEnumerator<TAlphabet>; }
        }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// <see cref="IFsmEnumerator&lt;T&gt;.NextState"/>
        /// </summary>
        protected abstract bool Next(TAlphabet inputSymbol);

        #endregion

        #region protected fields ------------------------------------------------------------------

        protected ICollection<string> m_currentStates;
        
        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IImplicitGraph<string, Transition<TAlphabet>> m_graph;

        #endregion
    }
}
