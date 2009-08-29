// ----------------------------------------------------------------------------
// FiniteStateMachineTestFixture.cs
//
// Contains the definition of the FiniteStateMachineTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 12:21:47
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Jolt.Functional;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using QuickGraph;
using Rhino.Mocks;

namespace Jolt.Automata.Test
{
    [TestFixture]
    public sealed class FiniteStateMachineTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the FiniteStateMachine class.
        /// </summary>
        [Test]
        public void Construction()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            Assert.That(fsm.AsGraph, Is.Not.Null);
            Assert.That(fsm.AsGraph, Is.InstanceOfType(typeof(BidirectionalGraph<string, Transition<char>>)));
            Assert.That(fsm.AsGraph.AllowParallelEdges);
            Assert.That(fsm.AsGraph.IsDirected);

            Assert.That(fsm.AsGraph.EdgeCount, Is.EqualTo(0));
            Assert.That(fsm.AsGraph.VertexCount, Is.EqualTo(0));
            Assert.That(fsm.StartState, Is.Null);
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the internal construction of the FiniteStateMachine class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            BidirectionalGraph<string, Transition<Version>> graph = new BidirectionalGraph<string, Transition<Version>>();
            FiniteStateMachine<Version> fsm = new FiniteStateMachine<Version>(graph);

            Assert.That(fsm.AsGraph, Is.SameAs(graph));
            Assert.That(fsm.StartState, Is.Null);
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the construction ofhte FiniteStateMachine class,
        /// when importing a copy of a graph.
        /// </summary>
        [Test]
        public void Construction_CopyGraph()
        {
            string[] expectedStates = { "start", "abc", "def", "final" };
            Predicate<char> trueForAll = Functor.ToPredicate(Functor.TrueForAll<char>());

            Transition<char>[] expectedTransitions = {
                 new Transition<char>("start", "abc", trueForAll),
                 new Transition<char>("abc", "def", trueForAll),
                 new Transition<char>("def", "final", trueForAll)
            };

            BidirectionalGraph<string, Transition<char>> graph = new BidirectionalGraph<string, Transition<char>>();
            graph.AddVertexRange(expectedStates);
            graph.AddEdgeRange(expectedTransitions);

            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>(graph as IBidirectionalGraph<string, Transition<char>>);
            Assert.That(fsm.AsGraph, Is.Not.Null);
            Assert.That(fsm.AsGraph, Is.Not.SameAs(graph));
            Assert.That(fsm.AsGraph, Is.InstanceOfType(typeof(BidirectionalGraph<string, Transition<char>>)));
            Assert.That(fsm.AsGraph.AllowParallelEdges);
            Assert.That(fsm.AsGraph.IsDirected);

            Assert.That(fsm.AsGraph.Edges.ToList(), Is.EquivalentTo(expectedTransitions));
            Assert.That(fsm.AsGraph.Vertices.ToList(), Is.EquivalentTo(expectedStates));
            Assert.That(fsm.StartState, Is.Null);
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when adding
        /// a new state to an FSM.
        /// </summary>
        [Test]
        public void AddState()
        {
            AssertAddState(false);
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when adding
        /// an existing state to an FSM.
        /// </summary>
        [Test]
        public void AddState_StateExists()
        {
            AssertAddState(true);
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when
        /// the given state is the implicit error state.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddState_InvalidState()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            fsm.AddState(FiniteStateMachine<char>.ErrorState);
        }

        /// <summary>
        /// Verifies the behavior of the AddStates() method when adding
        /// new states to an FSM.
        /// </summary>
        [Test]
        public void AddStates()
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Expectations
                // The states are added to the graph.
                string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
                Expect.Call(graph.AddVertexRange(expectedStates)).Return(expectedStates.Length);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                Assert.That(fsm.AddStates(expectedStates), Is.EqualTo(expectedStates.Length));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when
        /// the given state is the implicit error state.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddStates_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            fsm.AddStates(new string[] { "start-state", "state-0", FiniteStateMachine<int>.ErrorState, "state-2", "end-state" });
        }

        /// <summary>
        /// Verifies the behavior of the AddTransition() method when
        /// adding a new transition to an FSM.
        /// </summary>
        [Test]
        public void AddTransition()
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Expectations
                // The transition is added to the graph.
                Transition<int> expectedTransition = new Transition<int>("start-state", "end-state", (100).Equals);
                Expect.Call(graph.AddEdge(expectedTransition)).Return(true);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                fsm.AddTransition(expectedTransition);
            });
        }

        /// <summary>
        /// Verifies the behavior of the RemoveState() opereation
        /// when the requested state exists.
        /// </summary>
        [Test]
        public void RemoveState()
        {
            AssertRemoveState(true);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveState() opereation
        /// when the requested state does not exist.
        /// </summary>
        [Test]
        public void RemoveState_StateDoesNotExist()
        {
            AssertRemoveState(false);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveState() operation when
        /// the requested state is a final state.
        /// </summary>
        [Test]
        public void RemoveState_FinalState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string expectedState = "final-state";

            fsm.AddState(expectedState);
            fsm.SetFinalState(expectedState);

            Assert.That(fsm.RemoveState(expectedState));
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the behavior of the RemoveTransition() operation
        /// when the requested transition exists.
        /// </summary>
        [Test]
        public void RemoveTransition()
        {
            AssertRemoveTransition(true);
        }

        /// <summary>
        /// Verifies the behavior of the RemoveTransition() operation
        /// when the requested transition does not exist.
        /// </summary>
        [Test]
        public void RemoveTransition_TransitionDoesNotExist()
        {
            AssertRemoveTransition(false);
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalState() method.
        /// </summary>
        [Test]
        public void SetFinalState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string expectedState = "final-state";

            fsm.AddState(expectedState);
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));

            fsm.SetFinalState(expectedState);
            Assert.That(fsm.FinalStates.First(), Is.SameAs(expectedState));
        }

        /// <summary>
        /// Verifies the idempotency property of the SetFinalState() method.
        /// </summary>
        [Test]
        public void SetFinalState_Idempotent()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string expectedState = "final-state";

            fsm.AddState(expectedState);
            fsm.SetFinalState(expectedState);
            fsm.SetFinalState(expectedState);

            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(1));
            Assert.That(fsm.FinalStates.First(), Is.SameAs(expectedState));
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalState() method when
        /// the given state is not a valid state in the FSM.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void SetFinalState_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            fsm.SetFinalState("final-state");
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalStates() method.
        /// </summary>
        [Test]
        public void SetFinalStates()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            
            fsm.AddStates(expectedStates);
            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));

            fsm.SetFinalStates(expectedStates);
            Assert.That(fsm.FinalStates.ToList(), Is.EquivalentTo(expectedStates));
        }

        /// <summary>
        /// Verifies the idempotency property of the SetFinalStates() method.
        /// </summary>
        [Test]
        public void SetFinalStates_Idempotent()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            
            fsm.AddStates(expectedStates);
            fsm.SetFinalStates(expectedStates);
            fsm.SetFinalStates(expectedStates);

            Assert.That(fsm.FinalStates.ToList(), Is.EquivalentTo(expectedStates));
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalStateS() method when at
        /// least one of the given states is not a valid state in the FSM.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void SetFinalStates_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            
            fsm.AddStates(expectedStates.TakeWhile(state => state.StartsWith("s")));
            fsm.SetFinalStates(expectedStates);
        }

        /// <summary>
        /// Verifies the behavior of the ClearFinalState() method.
        /// </summary>
        [Test]
        public void ClearFinalState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string expectedState = "final-state";

            fsm.AddState(expectedState);
            fsm.SetFinalState(expectedState);

            Assert.That(fsm.ClearFinalState(expectedState));
            Assert.That(!fsm.FinalStates.Contains(expectedState));
            Assert.That(fsm.AsGraph.Vertices.Contains(expectedState));
        }

        /// <summary>
        /// Verifies the behavior of the ClearFinalState() method when
        /// the given state is not valid in the FSM.
        /// </summary>
        [Test]
        public void ClearFinalState_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string expectedState = "final-state";

            Assert.That(!fsm.ClearFinalState(expectedState));
        }

        /// <summary>
        /// Verifies the behavior of the ClearFinalStates() method.
        /// </summary>
        [Test]
        public void ClearFinalStates()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };

            fsm.AddStates(expectedStates);
            fsm.SetFinalStates(expectedStates);
            fsm.ClearFinalStates(expectedStates);

            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
            Assert.That(fsm.AsGraph.Vertices, Is.EquivalentTo(expectedStates));
        }

        /// <summary>
        /// Verifies the behavior of the ClearFinalStates() method when
        /// at least one of the given states is not valid in the FSM.
        /// </summary>
        [Test]
        public void ClearFinalStates_InvalidStates()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            Func<string, bool> isLastCharDigit = state => Char.IsDigit(state, state.Length - 1);

            fsm.AddStates(expectedStates.TakeWhile(isLastCharDigit));
            fsm.SetFinalStates(expectedStates.TakeWhile(isLastCharDigit));
            fsm.ClearFinalStates(expectedStates);

            Assert.That(fsm.FinalStates.Count(), Is.EqualTo(0));
            Assert.That(!fsm.AsGraph.Vertices.Contains("end-state"));
        }

        /// <summary>
        /// Verifies the behavior of the IsFinalState() method.
        /// </summary>
        [Test]
        public void IsFinalState()
        {
            string[] states = { "start", "final" };
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            fsm.AddStates(states);
            fsm.SetFinalState(states[1]);

            Assert.That(!fsm.IsFinalState(states[0]));
            Assert.That(fsm.IsFinalState(states[1]));

            fsm.ClearFinalStates(states);

            Assert.That(!fsm.IsFinalState(states[0]));
            Assert.That(!fsm.IsFinalState(states[1]));
        }
        
        /// <summary>
        /// Verifies the behavior of the CreateStateEnumerator() method.
        /// </summary>
        [Test]
        public void CreateStateEnumerator()
        {
            BidirectionalGraph<string, Transition<int>> graph = new BidirectionalGraph<string,Transition<int>>();
            string startState = "start-state";
            graph.AddVertex(startState);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            IFsmEnumerator<int> enumerator = fsm.CreateStateEnumerator(startState);

            Assert.That(enumerator, Is.InstanceOfType(typeof(FsmEnumerator<int>)));
            Assert.That(enumerator.CurrentState, Is.SameAs(startState));
        }

        /// <summary>
        /// Verifies the behavior of the CreateStateEnumerator() method
        /// when the given start state is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void CreateStateEnumerator_InvalidStartState()
        {
            BidirectionalGraph<string, Transition<int>> graph = new BidirectionalGraph<string, Transition<int>>();
            graph.AddVertex("start-state");

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            IFsmEnumerator<int> enumerator = fsm.CreateStateEnumerator("end-state");
        }

        /// <summary>
        /// Verifies the behavior of the Consume() method when an FSM
        /// accepts a sequence of input symbols.
        /// </summary>
        [Test]
        public void Consume()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            
            ConsumptionResult<char> result = fsm.Consume(String.Empty);
            Assert.That(result.IsAccepted);
            Assert.That(result.LastState, Is.SameAs(fsm.StartState));
            Assert.That(result.LastSymbol, Is.EqualTo(default(char)));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(0));

            string inputSymbols = "abcdefghijklmnopqrstuvwxyz!";
            result = fsm.Consume(inputSymbols);
            Assert.That(result.IsAccepted);
            Assert.That(result.LastState, Is.SameAs(fsm.StartState));
            Assert.That(result.LastSymbol, Is.EqualTo('!'));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(inputSymbols.Length));
        }

        /// <summary>
        /// Verifies the behavior of the Consume() method when an FSM
        /// rejects a sequence of input symbols.
        /// </summary>
        [Test]
        public void Consume_RejectSymbols()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            string inputSymbols = "abcdefghijklmnopqrstuvwxyz";

            ConsumptionResult<char> result = fsm.Consume(inputSymbols);
            Assert.That(!result.IsAccepted);
            Assert.That(result.LastState, Is.EqualTo("mod3(len) = 1"));
            Assert.That(result.LastSymbol, Is.EqualTo('z'));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(inputSymbols.Length));
        }

        /// <summary>
        /// Verifies the behavior of the Consume() method when an invalid
        /// character is detected in the sequence of input symbols.
        /// </summary>
        [Test]
        public void Consume_InvalidInputSymbols()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateEvenNumberOfZeroesMachine();
            string inputSymbols = "01001123450000";

            ConsumptionResult<char> result = fsm.Consume(inputSymbols);
            Assert.That(!result.IsAccepted);
            Assert.That(result.LastState, Is.SameAs(FiniteStateMachine<char>.ErrorState));
            Assert.That(result.LastSymbol, Is.EqualTo('2'));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(inputSymbols.IndexOf('2') + 1));
        }

        /// <summary>
        /// Verifies the behavior of the Consume() method when no
        /// input symbols are given.
        /// </summary>
        [Test]
        public void Consume_NoSymbols()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateEvenNumberOfZeroesMachine();
            ConsumptionResult<char> result = fsm.Consume(Enumerable.Empty<char>());

            Assert.That(result.IsAccepted);
            Assert.That(result.LastState, Is.SameAs(fsm.StartState));
            Assert.That(result.LastSymbol, Is.EqualTo('\0'));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the behavior of the Consume() mehtod when the FSM
        /// contains an invalid start state.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Consume_InvalidStartState()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            fsm.Consume(Enumerable.Repeat('a', 10));
        }

        /// <summary>
        /// Verifies the behavior of the StartState property.
        /// </summary>
        [Test]
        public void StartState()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string expectedState = "start-state";

            fsm.AddState(expectedState);
            fsm.StartState = expectedState;

            Assert.That(fsm.StartState, Is.SameAs(expectedState));
        }

        /// <summary>
        /// Verifies the behavior of the StartState property when
        /// the given state is not valid in the FSM.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void StartState_InvalidState()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            fsm.StartState = "start-state";
        }

        /// <summary>
        /// Verifies the behavior of the FinalStates property.
        /// </summary>
        [Test]
        public void FinalStates()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string[] expectedStates = { "start-state", "state-0", "final-state" };
            fsm.AddStates(expectedStates);
            fsm.SetFinalStates(expectedStates);

            IEnumerable<string> finalStates = fsm.FinalStates;
            Assert.That(finalStates, Is.Not.InstanceOfType(typeof(HashSet<string>)));
            Assert.That(finalStates, Is.Not.SameAs(fsm.FinalStates));
            Assert.That(finalStates.ToList(), Is.EquivalentTo(expectedStates));
        }

        /// <summary>
        /// Verifies that the final states collection is updated when
        /// the RemoveVertex method is called from the graph representation
        /// of the FSM.
        /// </summary>
        [Test]
        public void FinalStates_RemoveVertex()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();
            string[] expectedStates = { "start-state", "state-0", "final-state" };
            fsm.AddStates(expectedStates);
            fsm.SetFinalState(expectedStates[2]);

            bool isRemoved = (fsm.AsGraph as IMutableBidirectionalGraph<string, Transition<char>>).RemoveVertex(expectedStates[2]);
            
            Assert.That(isRemoved);
            Assert.That(!fsm.AsGraph.Vertices.Contains(expectedStates[2]));
            Assert.That(!fsm.FinalStates.Contains(expectedStates[2]));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Asserts that a state is added to an FSM, if it does
        /// not exist.
        /// </summary>
        /// 
        /// <param name="stateExists">
        /// Denotes if the state-to-add exists in the FSM.
        /// </param>
        private static void AssertAddState(bool stateExists)
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Expectations
                // The state is added to the graph.
                string expectedState = "start-state";
                Expect.Call(graph.AddVertex(expectedState)).Return(!stateExists);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                Assert.That(fsm.AddState(expectedState), Is.EqualTo(!stateExists));
            });
        }

        /// <summary>
        /// Asserts that a state is removed from an FSM, if it exists.
        /// </summary>
        /// 
        /// <param name="stateExists">
        /// Denotes if the state-to-remove exists in the FSM.
        /// </param>
        private static void AssertRemoveState(bool stateExists)
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Attempt to remove the state from the graph.
                string expectedState = "start-state";
                Expect.Call(graph.RemoveVertex(expectedState)).Return(stateExists);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                Assert.That(fsm.RemoveState(expectedState), Is.EqualTo(stateExists));
            });
        }

        /// <summary>
        /// Asserts that a transition is removed from an FSM, if it exists.
        /// </summary>
        /// 
        /// <param name="transitionExists">
        /// Denotes if the transition-to-remove exists in the FSM.
        /// </param>
        private static void AssertRemoveTransition(bool transitionExists)
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Attempt to remove the transition from the graph.
                Transition<int> expectedTransition = new Transition<int>("start-state", "end-state", (100).Equals);
                Expect.Call(graph.RemoveEdge(expectedTransition)).Return(transitionExists);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                Assert.That(fsm.RemoveTransition(expectedTransition), Is.EqualTo(transitionExists));
            });
        }

        #endregion
    }
}
