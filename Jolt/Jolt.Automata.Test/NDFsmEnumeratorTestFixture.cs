// ----------------------------------------------------------------------------
// NDFsmEnumeratorTestFixture.cs
//
// Contains the definition of the NDFsmEnumeratorTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 3/25/2010 17:21:33
// ----------------------------------------------------------------------------

using System;
using System.Linq;

using Jolt.Automata.Properties;
using NUnit.Framework;

namespace Jolt.Automata.Test
{
    [TestFixture]
    public sealed class NDFsmEnumeratorTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the NextState() method when
        /// a series of valid, deterministric transitions are made.
        /// </summary>
        [Test]
        public void NextState_ValidTransition_Deterministic()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateNonDeterministicMachine();

            string inputSymbols = "bbbca";
            string[] expectedStates = { "2", "2", "2", "4", "start" };

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(EnumerationType.Nondeterministic, fsm.StartState);

            for (int i = 0; i < inputSymbols.Length; ++i)
            {
                Assert.That(enumerator.Next(inputSymbols[i]));
                Assert.That(enumerator.CurrentState, Is.EqualTo(expectedStates[i]));
                Assert.That(enumerator.CurrentStates.ToArray(), Is.EqualTo(new[] { expectedStates[i] }));
            }
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when
        /// a series of valid, nondeterministric transitions are made.
        /// </summary>
        [Test]
        public void NextState_ValidTransition_Nondeterministic()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateNonDeterministicMachine();

            string inputSymbols = "aabbcaa";
            string[][] expectedStates = {
                new[] { "1", "3" },
                new[] { "1", "3", "4" },
                new[] { "2" },
                new[] { "2" },
                new[] { "4" },
                new[] { "start" },
                new[] { "1", "3" }
            };

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(EnumerationType.Nondeterministic, fsm.StartState);

            for (int i = 0; i < inputSymbols.Length; ++i)
            {
                Assert.That(enumerator.Next(inputSymbols[i]));

                string[] currentStates = enumerator.CurrentStates.ToArray();
                Assert.That(currentStates, Is.EqualTo(expectedStates[i]));
                Assert.That(enumerator.CurrentState, Is.EqualTo(expectedStates[i][0]));
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
            FiniteStateMachine<char> fsm = FsmFactory.CreateNonDeterministicMachine();

            string inputSymbols = "acme";
            string one = (1).ToString();
            string three = (3).ToString();

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(EnumerationType.Nondeterministic, fsm.StartState);
            Assert.That(enumerator.Next(inputSymbols[0]));
            Assert.That(enumerator.CurrentStates.ToArray(), Is.EqualTo(new[] {one, three}));
            Assert.That(enumerator.Next(inputSymbols[1]));
            Assert.That(enumerator.CurrentStates.ToArray(), Is.EqualTo(new[] {three}));
            Assert.That(!enumerator.Next(inputSymbols[2]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(FiniteStateMachine<char>.ErrorState));
            Assert.That(!enumerator.Next(inputSymbols[3]));
            Assert.That(enumerator.CurrentState, Is.EqualTo(FiniteStateMachine<char>.ErrorState));
        }

        /// <summary>
        /// Verifies the behavior of the NextState() method when the
        /// transitions in the FSM contain transition-event subcribers.
        /// </summary>
        [Test]
        public void NextState_TransitionEventFired()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateNonDeterministicMachine();
            byte numStateTransitions = 0;
            string sourceState = (1).ToString();
            string inputSymbols = "bbbcaaaac";

            foreach (Transition<char> transition in fsm.AsGraph.Edges.Where(e => e.Source == sourceState))
            {
                transition.OnTransition += delegate(object sender, StateTransitionEventArgs<char> eventArgs)
                {
                    Assert.That(eventArgs.SourceState, Is.EqualTo(sourceState));
                    Assert.That(eventArgs.InputSymbol, Is.EqualTo('a'));
                    ++numStateTransitions;
                };
            }

            IFsmEnumerator<char> enumerator = fsm.CreateStateEnumerator(EnumerationType.Nondeterministic, fsm.StartState);
            foreach (char symbol in inputSymbols)
            {
                Assert.That(enumerator.Next(symbol), "Test FSM is incorrectly initialized");
            }

            Assert.That(numStateTransitions, Is.EqualTo(4));
        }
    }
}
