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
            string[] states = { "start", "1", "2", "3", "4" };

            fsm.AddStates(states);
            fsm.StartState = states[0];

            Predicate<char> equalsA = ('a').Equals;
            Predicate<char> equalsB = ('b').Equals;
            Predicate<char> equalsC = ('c').Equals;

            fsm.AddTransition(new Transition<char>(states[0], states[1], equalsA));
            fsm.AddTransition(new Transition<char>(states[0], states[2], equalsB));
            fsm.AddTransition(new Transition<char>(states[0], states[3], equalsA));

            fsm.AddTransition(new Transition<char>(states[1], states[1], equalsA));
            fsm.AddTransition(new Transition<char>(states[1], states[2], equalsB));
            fsm.AddTransition(new Transition<char>(states[1], states[3], equalsA));
            
            fsm.AddTransition(new Transition<char>(states[2], states[2], equalsB));
            fsm.AddTransition(new Transition<char>(states[2], states[4], equalsC));

            fsm.AddTransition(new Transition<char>(states[3], states[4], equalsA));
            fsm.AddTransition(new Transition<char>(states[3], states[3], equalsC));

            fsm.AddTransition(new Transition<char>(states[4], states[0], equalsA));

            return fsm;
        }

        /// <summary>
        /// Creates a nondeterministic FSM with multiple final states.
        /// </summary>
        internal static FiniteStateMachine<char> CreateNonDeterministicMachine_MultipleFinalStates()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string[] states = { "start", "other", "final-1", "final-2" };
            string[] finalStates = { states[2], states[3] };

            fsm.AddStates(states);
            fsm.StartState = states[0];
            fsm.SetFinalStates(finalStates);

            Predicate<char> transitionPredicate = Functor.ToPredicate(Functor.TrueForAll<char>());
            fsm.AddTransition(new Transition<char>(states[0], states[1], transitionPredicate));
            fsm.AddTransition(new Transition<char>(states[0], states[2], transitionPredicate));
            fsm.AddTransition(new Transition<char>(states[0], states[3], transitionPredicate));

            return fsm;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly Predicate<char> TrueForAll = Functor.ToPredicate(Functor.TrueForAll<char>());

        #endregion
    }
}