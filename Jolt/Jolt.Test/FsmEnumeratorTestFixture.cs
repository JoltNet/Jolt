// ----------------------------------------------------------------------------
// FsmEnumeratorTestFixture.cs
//
// Contains the definition of the FsmEnumeratorTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/8/2008 18:52:03
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class FsmEnumeratorTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the NextState() method when
        /// a series of valid transitions are made.
        /// </summary>
        [Test]
        public void NextState_ValidTransition()
        {
            // Create an FSM that determines the length of a character sequence.
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string oddState = "odd-length";
            string evenState = "even-length";

            fsm.AddState(oddState);
            fsm.AddState(evenState);
            fsm.AddTransition(new Transition<char>(oddState, evenState, ch => true));
            fsm.AddTransition(new Transition<char>(evenState, oddState, ch => true));

            string inputSymbols = "even";
            string[] expectedStates = { oddState, evenState, oddState, evenState };

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(evenState);

            for (int i = 0; i < inputSymbols.Length; ++i)
            {
                Assert.That(enumerator.NextState(inputSymbols[i]));
                Assert.That(enumerator.CurrentState, Is.EqualTo(expectedStates[i]));
            }
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when
        /// an invalid transition occurs during input symbol
        /// consumption.
        /// </summary>
        [Test]
        public void NextState_InvalidTransition()
        {
            // Create an FSM that determines if an input string has an odd
            // or even number of zeroes.
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string oddState = "odd-number";
            string evenState = "even-number";

            fsm.AddState(oddState);
            fsm.AddState(evenState);
            fsm.AddTransition(new Transition<char>(oddState, oddState, ch => ch == '1'));
            fsm.AddTransition(new Transition<char>(evenState, evenState, ch => ch == '1'));
            fsm.AddTransition(new Transition<char>(oddState, evenState, ch => ch == '0'));
            fsm.AddTransition(new Transition<char>(evenState, oddState, ch => ch == '0'));

            string inputSymbols = "0120";
            
            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(evenState);
            Assert.That(enumerator.NextState(inputSymbols[0]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(oddState));
            Assert.That(enumerator.NextState(inputSymbols[1]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(oddState));
            Assert.That(!enumerator.NextState(inputSymbols[2]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(oddState));
            Assert.That(enumerator.NextState(inputSymbols[3]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(evenState));
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when a non-
        /// deterministic transition is detected.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void NextState_NonDeterministic()
        {
            // Create an non-deterministic FSM.
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string startState = "start";
            string aState = "a";
            string bState = "b";

            fsm.AddState(startState);
            fsm.AddState(aState);
            fsm.AddState(bState);
            fsm.AddTransition(new Transition<char>(startState, aState, ch => true));
            fsm.AddTransition(new Transition<char>(startState, bState, ch => true));

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(startState);
            enumerator.NextState('a');
        }
    }
}