// ----------------------------------------------------------------------------
// StateTransitionEventArgsTestFixture.cs
//
// Contains the definition of the StateTransitionEventArgsTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/9/2008 19:52:39
// ----------------------------------------------------------------------------

using NUnit.Framework;

namespace Jolt.Automata.Test
{
    [TestFixture]
    public sealed class StateTransitionEventArgsTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            string sourceState = "start-state";
            int inputSymbol = 1234;

            StateTransitionEventArgs<int> eventArgs = new StateTransitionEventArgs<int>(sourceState, inputSymbol);

            Assert.That(sourceState, Is.SameAs(eventArgs.SourceState));
            Assert.That(inputSymbol, Is.EqualTo(eventArgs.InputSymbol));
        }
    }
}