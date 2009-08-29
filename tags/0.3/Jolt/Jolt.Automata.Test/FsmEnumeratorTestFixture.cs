// ----------------------------------------------------------------------------
// FsmEnumeratorTestFixture.cs
//
// Contains the definition of the FsmEnumeratorTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/8/2008 18:52:03
// ----------------------------------------------------------------------------

using System;
using System.Linq;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Automata.Test
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
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();

            string inputSymbols = "mod3";
            string[] expectedStates = { "mod3(len) = 2", "mod3(len) = 1", "mod3(len) = 0", "mod3(len) = 2" };

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(fsm.StartState);

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
            FiniteStateMachine<char> fsm = FsmFactory.CreateEvenNumberOfZeroesMachine();

            string oddState = "odd-number";
            string inputSymbols = "0120";
            
            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(fsm.StartState);
            Assert.That(enumerator.NextState(inputSymbols[0]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(oddState));
            Assert.That(enumerator.NextState(inputSymbols[1]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(oddState));
            Assert.That(!enumerator.NextState(inputSymbols[2]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(FiniteStateMachine<char>.ErrorState));
            Assert.That(!enumerator.NextState(inputSymbols[3]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(FiniteStateMachine<char>.ErrorState));
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when a non-
        /// deterministic transition is detected.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void NextState_NonDeterministic()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateNonDeterministicMachine();

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(fsm.StartState);
            enumerator.NextState('a');
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when the
        /// transitions in the FSM contain transition-event subcribers.
        /// </summary>
        [Test]
        public void NextState_TransitionEventFired()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateEvenNumberOfZeroesMachine();

            string oddState = "odd-number";
            string inputSymbols = "101010101010";
            byte raiseEventCount = 0;

            fsm.AsGraph.Edges
                .Single(e => e.Source == oddState && e.Target == "even-number")
                .OnTransition += delegate(object sender, StateTransitionEventArgs<char> eventArgs)
            {
                Assert.That(eventArgs.SourceState, Is.SameAs(oddState));
                Assert.That(eventArgs.InputSymbol, Is.EqualTo(inputSymbols[4 * raiseEventCount + 1]));
                ++raiseEventCount;
            };

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(fsm.StartState);
            foreach (char symbol in inputSymbols)
            {
                Assert.That(enumerator.NextState(symbol), "Test FSM is incorrectly initialized");
            }

            Assert.That(raiseEventCount, Is.EqualTo(inputSymbols.Length / 4));
        }
    }
}
