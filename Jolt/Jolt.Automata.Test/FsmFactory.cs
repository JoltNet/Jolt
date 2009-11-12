// ----------------------------------------------------------------------------
// FsmFactory.cs
//
// Contains the definition of the FsmFactory class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/21/2008 20:14:37
// ----------------------------------------------------------------------------

using System;

using Jolt.Functional;

namespace Jolt.Automata.Test
{
    /// <summary>
    /// Contains methods that create FSM instances that support unit tests.
    /// </summary>
    internal static class FsmFactory
    {
        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Creates an FSM that accepts strings with a length divisible by three.
        /// </summary>
        internal static FiniteStateMachine<char> CreateLengthMod3Machine()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string modIs2 = "mod3(len) = 2";
            string modIs1 = "mod3(len) = 1";
            string modIs0 = "mod3(len) = 0";

            fsm.AddStates(new string[] { modIs2, modIs1, modIs0 });
            fsm.StartState = modIs0;
            fsm.SetFinalState(modIs0);

            fsm.AddTransition(new Transition<char>(modIs0, modIs2, TrueForAll));
            fsm.AddTransition(new Transition<char>(modIs2, modIs1, TrueForAll));
            fsm.AddTransition(new Transition<char>(modIs1, modIs0, TrueForAll));

            return fsm;
        }

        /// <summary>
        /// Creates an FSM that accepts a string representation of a binary number
        /// containing an even number of zeroes,
        /// </summary>
        internal static FiniteStateMachine<char> CreateEvenNumberOfZeroesMachine()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string oddState = "odd-number";
            string evenState = "even-number";

            fsm.AddStates(new string[] { oddState, evenState });
            fsm.StartState = evenState;
            fsm.SetFinalState(evenState);

            fsm.AddTransition(new Transition<char>(oddState, oddState, ('1').Equals));
            fsm.AddTransition(new Transition<char>(evenState, evenState, ('1').Equals));
            fsm.AddTransition(new Transition<char>(oddState, evenState, ('0').Equals));
            fsm.AddTransition(new Transition<char>(evenState, oddState, ('0').Equals));

            return fsm;
        }

        /// <summary>
        /// Creates a nondeterministic FSM.
        /// </summary>
        internal static FiniteStateMachine<char> CreateNonDeterministicMachine()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string startState = "start";
            string aState = "a";
            string bState = "b";

            fsm.AddStates(new string[] { startState, aState, bState });
            fsm.StartState = startState;

            fsm.AddTransition(new Transition<char>(startState, aState, TrueForAll));
            fsm.AddTransition(new Transition<char>(startState, bState, TrueForAll));

            return fsm;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly Predicate<char> TrueForAll = Functor.ToPredicate(Functor.TrueForAll<char>());

        #endregion
    }
}