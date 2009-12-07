// ----------------------------------------------------------------------------
// ConsumptionResult.cs
//
// Contains the definition of the ConsumptionResult class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/23/2008 16:34:47
// ----------------------------------------------------------------------------

namespace Jolt.Automata
{
    /// <summary>
    /// Contains metadata describing the result of a <see cref="FiniteStateMachine"/>
    /// consuming a sequence of input symbols.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    public sealed class ConsumptionResult<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="ConsumptionResult"/> class.
        /// </summary>
        /// 
        /// <param name="isAccepted"><see cref="ConsumptionResult.IsAccepted"/></param>
        /// <param name="lastSymbol"><see cref="ConsumptionResult.LastSymbol"/></param>
        /// <param name="numberOfSymbols"><see cref="ConsumptionResult.NumberOfSymbols"/></param>
        /// <param name="lastState"><see cref="ConsumptionResult.LastState"/></param>
        internal ConsumptionResult(bool isAccepted, TAlphabet lastSymbol, ulong numberOfSymbols, string lastState)
        {
            m_isAccepted = isAccepted;
            m_lastSymbol = lastSymbol;
            m_numberOfSymbols = numberOfSymbols;
            m_lastState = lastState;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets a Boolean value denoting if the <see cref="FiniteStateMachine"/> accepted the
        /// sequence of input symbols.
        /// </summary>
        public bool IsAccepted
        {
            get { return m_isAccepted; }
        }

        /// <summary>
        /// Gets the last symbol processed by the <see cref="FiniteStateMachine"/>.
        /// </summary>
        public TAlphabet LastSymbol
        {
            get { return m_lastSymbol; }
        }

        /// <summary>
        /// Gets the number of symbols consumed by the <see cref="FiniteStateMachine"/>.
        /// </summary>
        public ulong NumberOfConsumedSymbols
        {
            get { return m_numberOfSymbols; }
        }

        /// <summary>
        /// Gets the last state visited by the <see cref="FiniteStateMachine"/>.
        /// </summary>
        public string LastState
        {
            get { return m_lastState; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly bool m_isAccepted;
        private readonly TAlphabet m_lastSymbol;
        private readonly ulong m_numberOfSymbols;
        private readonly string m_lastState;

        #endregion
    }
}