// ----------------------------------------------------------------------------
// IFsmEnumerator.cs
//
// Contains the definition of the IFsmEnumerator interface.
// Copyright 2008 Steve Guidi.
//
// File created: 12/8/2008 17:55:17
// ----------------------------------------------------------------------------

namespace Jolt
{
    /// <summary>
    /// Defines an interface for enumerating states in a finite state machine (FSM).
    /// <seealso cref="FiniteStateMachine"/>
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the FSM.
    /// </typeparam>
    public interface IFsmEnumerator<TAlphabet>
    {
        /// <summary>
        /// Performs a state transition from the current enumeration state
        /// using the given input symbol.  Returns TRUE if a state transition
        /// occurs, FALSE otherwise.
        /// </summary>
        /// 
        /// <param name="inputSymbol">
        /// The symbol that exercises a state transition.
        /// </param>
        bool NextState(TAlphabet inputSymbol);

        /// <summary>
        /// Retrieves the state referenced by the enumerator.
        /// </summary>
        string CurrentState { get; }
    }
}