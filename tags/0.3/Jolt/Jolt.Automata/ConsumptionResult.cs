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
    /// Contains metadata describing the result of an FSM consuming a sequence
    /// of input symbols.
    /// </summary>
    public sealed class ConsumptionResult<TAlphabet>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the fields of the class.
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
        /// Determines if the FSM accepted the sequence of input symbols.
        /// </summary>
        public bool IsAccepted
        {
            get { return m_isAccepted; }
        }

        /// <summary>
        /// Retrieves the last symbol processed by the FSM.
        /// </summary>
        public TAlphabet LastSymbol
        {
            get { return m_lastSymbol; }
        }

        /// <summary>
        /// Retrieves the number of symbols consumed by the FSM.
        /// </summary>
        public ulong NumberOfConsumedSymbols
        {
            get { return m_numberOfSymbols; }
        }

        /// <summary>
        /// Retrieves the last state visited by the FSM.
        /// </summary>
        public string LastState
        {
            get { return m_lastState; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly bool m_isAccepted;
        private readonly TAlphabet m_lastSymbol;
        private readonly ulong m_numberOfSymbols;
        private readonly string m_lastState;

        #endregion
    }
}