// ----------------------------------------------------------------------------
// FiniteStateMachineTestFixture.cs
//
// Contains the definition of the FiniteStateMachineTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 12:21:47
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using QuickGraph;
using Rhino.Mocks;

namespace Jolt.Test
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
            Assert.That(fsm.Graph, Is.Not.Null);
            Assert.That(fsm.Graph, Is.InstanceOfType(typeof(BidirectionalGraph<string, Transition<char>>)));
            Assert.That(fsm.Graph.AllowParallelEdges);
            Assert.That(fsm.Graph.IsDirected);

            BidirectionalGraph<string, Transition<char>> graph = fsm.Graph as BidirectionalGraph<string, Transition<char>>;
            Assert.That(graph.EdgeCount, Is.EqualTo(0));
            Assert.That(graph.VertexCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies the internal construction of the FiniteStateMachine class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            BidirectionalGraph<string, Transition<Version>> graph = new BidirectionalGraph<string, Transition<Version>>();
            FiniteStateMachine<Version> fsm = new FiniteStateMachine<Version>(graph);

            Assert.That(fsm.Graph, Is.SameAs(graph));
        }

        /// <summary>
        /// Verifies the behavior of the AddState() method when adding
        /// a new state to the FiniteStateMachine.
        /// </summary>
        [Test]
        public void AddState()
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Expectations
                // The state is added to the graph.
                string expectedState = "start-state";
                graph.AddVertex(expectedState);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                fsm.AddState(expectedState);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddTransition() method when
        /// adding a new transition to the FiniteStateMachine.
        /// </summary>
        [Test]
        public void AddTransition()
        {
            With.Mocks(delegate
            {
                BidirectionalGraph<string, Transition<int>> graph = Mocker.Current.CreateMock<BidirectionalGraph<string, Transition<int>>>();

                // Expectations
                // The transition is added to the graph.
                Transition<int> expectedTransition = new Transition<int>("start-state", "end-state", n => n == 100);
                Expect.Call(graph.AddEdge(expectedTransition)).Return(true);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                FiniteStateMachine<int> fsm = new FiniteStateMachine<int>(graph);
                fsm.AddTransition(expectedTransition);
            });
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

        #endregion
    }
}