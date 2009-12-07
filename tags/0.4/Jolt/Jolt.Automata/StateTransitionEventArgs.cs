// ----------------------------------------------------------------------------
// StateTransitionEventArgs.cs
//
// Contains the definition of the StateTransitionEventArgs class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/9/2008 18:48:06
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Automata
{
    /// <summary>
    /// Contains the data that is shared to a subscriber of the
    /// <see cref="Transition.OnStateTransition"/> event.
    /// </summary>
    public sealed class StateTransitionEventArgs<TAlphabet> : EventArgs
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="StateTransitionEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="sourceState">
        /// The transition's originating state.
        /// </param>
        /// 
        /// <param name="inputSymbol">
        /// The symbol causing the state transition.
        /// </param>
        internal StateTransitionEventArgs(string sourceState, TAlphabet inputSymbol)
        {
            m_sourceState = sourceState;
            m_inputSymbol = inputSymbol;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the transition's originating state.
        /// </summary>
        public string SourceState
        {
            get { return m_sourceState; }
        }

        /// <summary>
        /// Gets the symbol causing the state transition.
        /// </summary>
        public TAlphabet InputSymbol
        {
            get { return m_inputSymbol; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly string m_sourceState;
        private readonly TAlphabet m_inputSymbol;

        #endregion
    }
}