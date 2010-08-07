// ----------------------------------------------------------------------------
// IFsmEnumerator.cs
//
// Contains the definition of the IFsmEnumerator interface.
// Copyright 2008 Steve Guidi.
//
// File created: 12/8/2008 17:55:17
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Jolt.Automata
{
    /// <summary>
    /// Defines a contract for enumerating states in a <see cref="FiniteStateMachine"/> (FSM).
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    public interface IFsmEnumerator<TAlphabet>
    {
        /// <summary>
        /// Performs a state transition from the current enumeration state
        /// using the given input symbol.
        /// </summary>
        /// 
        /// <param name="inputSymbol">
        /// The symbol that exercises a state transition.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if a state transition occurs, false otherwise.
        /// </returns>
        bool Next(TAlphabet inputSymbol);

        /// <summary>
        /// Gets the state referenced by the enumerator.  If there is more than one
        /// state referenced, the first visited state is returned.
        /// </summary>
        string CurrentState { get; }

        /// <summary>
        /// Gets all states referenced by the enumerator.
        /// </summary>
        IEnumerable<string> CurrentStates { get; }
    }
}