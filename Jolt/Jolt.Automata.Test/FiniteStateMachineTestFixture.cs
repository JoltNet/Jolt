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

using Jolt.Automata.Properties;
using Jolt.Functional;
using NUnit.Framework;
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
            Assert.That(fsm.AsGraph, Is.InstanceOf<BidirectionalGraph<string, Transition<char>>>());
            Assert.That(fsm.AsGraph.AllowParallelEdges);
            Assert.That(fsm.AsGraph.IsDirected);

            Assert.That(fsm.AsGraph.EdgeCount, Is.EqualTo(0));
            Assert.That(fsm.AsGraph.VertexCount, Is.EqualTo(0));
            Assert.That(fsm.StartState, Is.Null);
            Assert.That(fsm.FinalStates, Is.Empty);
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
            Assert.That(fsm.FinalStates, Is.Empty);
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
            Assert.That(fsm.AsGraph, Is.InstanceOf<BidirectionalGraph<string, Transition<char>>>());
            Assert.That(fsm.AsGraph.AllowParallelEdges);
            Assert.That(fsm.AsGraph.IsDirected);

            Assert.That(fsm.AsGraph.Edges, Is.EquivalentTo(expectedTransitions));
            Assert.That(fsm.AsGraph.Vertices, Is.EquivalentTo(expectedStates));
            Assert.That(fsm.StartState, Is.Null);
            Assert.That(fsm.FinalStates, Is.Empty);
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
        [Test]
        public void AddState_InvalidState()
        {
            FiniteStateMachine<char> fsm = new FiniteStateMachine<char>();

            Assert.That(
                () => fsm.AddState(FiniteStateMachine<char>.ErrorState),    // TODO: Use Jolt.Bind iff NUnit accepts Action instead of TestDelegate
                Throws.ArgumentException.With.Message.EqualTo(
                    String.Format(Resources.Error_AddState_ImplicitErrorState, FiniteStateMachine<char>.ErrorState)));
        }

        /// <summary>
        /// Verifies the behavior of the AddStates() method when adding
        /// new states to an FSM.
        /// </summary>
        [Test]
        public void AddStates()
        {
            BidirectionalGraph<string, Transition<int>> graph = MockRepository.GenerateMock<BidirectionalGraph<string, Transition<int>>>();

            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            graph.Expect(g => g.AddVertexRange(expectedStates)).Return(expectedStates.Length);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            Assert.That(fsm.AddStates(expectedStates), Is.EqualTo(expectedStates.Length));

            graph.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when
        /// the given state is the implicit error state.
        /// </summary>
        [Test]
        public void AddStates_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] states = { "start-state", "state-0", FiniteStateMachine<int>.ErrorState, "state-2", "end-state" };

            Assert.That(
                () => fsm.AddStates(states),    // TODO: Use Jolt.Bind iff NUnit accepts Action instead of TestDelegate
                Throws.ArgumentException.With.Message.EqualTo(
                    String.Format(Resources.Error_AddStates_ImplicitErrorState, FiniteStateMachine<int>.ErrorState)));
        }

        /// <summary>
        /// Verifies the behavior of the AddTransition() method when
        /// adding a new transition to an FSM.
        /// </summary>
        [Test]
        public void AddTransition()
        {
            BidirectionalGraph<string, Transition<int>> graph = MockRepository.GenerateMock<BidirectionalGraph<string, Transition<int>>>();

            Transition<int> expectedTransition = new Transition<int>("start-state", "end-state", (100).Equals);
            graph.Expect(g => g.AddEdge(expectedTransition)).Return(true);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            fsm.AddTransition(expectedTransition);

            graph.VerifyAllExpectations();
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
            Assert.That(fsm.FinalStates, Is.Empty);
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
            Assert.That(fsm.FinalStates, Is.Empty);

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

            Assert.That(fsm.FinalStates, Is.EqualTo(new[] { expectedState }));
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalState() method when
        /// the given state is not a valid state in the FSM.
        /// </summary>
        [Test]
        public void SetFinalState_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string finalState = "final-state";

            Assert.That(
                new TestDelegate(Bind.First(fsm.SetFinalState, finalState)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.ArgumentException.With.Message.EqualTo(
                    String.Format(Resources.Error_SetFinalState_InvalidState, finalState)));
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
            Assert.That(fsm.FinalStates, Is.Empty);

            fsm.SetFinalStates(expectedStates);
            Assert.That(fsm.FinalStates, Is.EquivalentTo(expectedStates));
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

            Assert.That(fsm.FinalStates, Is.EquivalentTo(expectedStates));
        }

        /// <summary>
        /// Verifies the behavior of the SetFinalStateS() method when at
        /// least one of the given states is not a valid state in the FSM.
        /// </summary>
        [Test]
        public void SetFinalStates_InvalidState()
        {
            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>();
            string[] expectedStates = { "start-state", "state-0", "state-1", "state-2", "end-state" };
            
            fsm.AddStates(expectedStates.TakeWhile(state => state.StartsWith("s")));

            Assert.That(
                new TestDelegate(Bind.First(fsm.SetFinalStates, expectedStates)),   // TODO: Use Jolt.Bind iff NUnit accepts Action instead of TestDelegate
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_SetFinalStates_InvalidState));
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
            Assert.That(fsm.FinalStates, Has.No.Member(expectedState));
            Assert.That(fsm.AsGraph.Vertices, Has.Member(expectedState));
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

            Assert.That(fsm.FinalStates, Is.Empty);
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

            Assert.That(fsm.FinalStates, Is.Empty);
            Assert.That(fsm.AsGraph.Vertices, Has.No.Member("end-state"));
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

            Assert.That(enumerator, Is.InstanceOf<FsmEnumerator<int>>());
            Assert.That(enumerator.CurrentState, Is.SameAs(startState));
        }

        /// <summary>
        /// Verifies the behavior of the CreateStateEnumerator() method
        /// when the given start state is invalid.
        /// </summary>
        [Test]
        public void CreateStateEnumerator_InvalidStartState()
        {
            BidirectionalGraph<string, Transition<int>> graph = new BidirectionalGraph<string, Transition<int>>();
            graph.AddVertex("start-state");

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            string initialState = "end-state";

            Assert.That(
                () => fsm.CreateStateEnumerator(initialState),  // TODO: Use Jolt.Bind iff NUnit accepts Action instead of TestDelegate
                Throws.ArgumentException.With.Message.EqualTo(
                    String.Format(Resources.Error_CreateEnumerator_InvalidStartState, initialState)));
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
        /// Verifies the behavior of the Consume() method when the FSM
        /// contains an invalid start state.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Consume_InvalidStartState()
        {
            new FiniteStateMachine<char>().Consume(Enumerable.Repeat('a', 10));
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
        [Test]
        public void StartState_InvalidState()
        {
            string invalidState = "start-state";

            Assert.That(
                () => new FiniteStateMachine<char>().StartState = invalidState,
                Throws.ArgumentException.With.Message.EqualTo(
                    String.Format(Resources.Error_SetStartState_InvalidState, invalidState)));
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
            Assert.That(finalStates, Is.Not.InstanceOf<HashSet<string>>());
            Assert.That(finalStates, Is.Not.SameAs(fsm.FinalStates));
            Assert.That(finalStates, Is.EquivalentTo(expectedStates));
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
            Assert.That(fsm.AsGraph.Vertices, Has.No.Member(expectedStates[2]));
            Assert.That(fsm.FinalStates, Has.No.Member(expectedStates[2]));
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
            BidirectionalGraph<string, Transition<int>> graph = MockRepository.GenerateMock<BidirectionalGraph<string, Transition<int>>>();

            string expectedState = "start-state";
            graph.Expect(g => g.AddVertex(expectedState)).Return(!stateExists);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            Assert.That(fsm.AddState(expectedState), Is.EqualTo(!stateExists));

            graph.VerifyAllExpectations();
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
            BidirectionalGraph<string, Transition<int>> graph = MockRepository.GenerateMock<BidirectionalGraph<string, Transition<int>>>();

            string expectedState = "start-state";
            graph.Expect(g => g.RemoveVertex(expectedState)).Return(stateExists);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            Assert.That(fsm.RemoveState(expectedState), Is.EqualTo(stateExists));

            graph.VerifyAllExpectations();
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
            BidirectionalGraph<string, Transition<int>> graph = MockRepository.GenerateMock<BidirectionalGraph<string, Transition<int>>>();

            Transition<int> expectedTransition = new Transition<int>("start-state", "end-state", (100).Equals);
            graph.Expect(g => g.RemoveEdge(expectedTransition)).Return(transitionExists);

            FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
            Assert.That(fsm.RemoveTransition(expectedTransition), Is.EqualTo(transitionExists));

            graph.VerifyAllExpectations();
        }

        #endregion
    }
}
