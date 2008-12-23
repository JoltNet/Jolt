// ----------------------------------------------------------------------------
// TransitionTestFixture.cs
//
// Contains the definition of the TransitionTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 11:30:00
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class TransitionTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the transition class.
        /// </summary>
        [Test]
        public void Construction()
        {
            string sourceState = "start";
            string finalState = "final";
            Predicate<char> transitionPredicate = Char.IsDigit;

            Transition<char> transition = new Transition<char>(sourceState, finalState, transitionPredicate);

            Assert.That(sourceState, Is.SameAs(transition.Source));
            Assert.That(finalState, Is.SameAs(transition.Target));
            Assert.That(transitionPredicate, Is.SameAs(transition.TransitionPredicate));
        }

        /// <summary>
        /// Verifies the behavior of the RaiseOnTransitionEvent() method.
        /// </summary>
        [Test]
        public void RaiseOnTransitionEvent()
        {
            bool isEventFired = false;
            StateTransitionEventArgs<char> eventArgs = new StateTransitionEventArgs<char>("start", 'c');
            
            Transition<char> transition = new Transition<char>("start", "start", ch => ch == 'c');
            transition.OnTransition += delegate(object sender, StateTransitionEventArgs<char> actualArgs)
            {
                isEventFired = true;
                Assert.That(sender, Is.Null);
                Assert.That(actualArgs, Is.SameAs(eventArgs));
            };

            transition.RaiseOnTransitionEvent(eventArgs);
            Assert.That(isEventFired);
        }

        /// <summary>
        /// Verifies the behavior of the RaiseOnTransitionEvent() method
        /// when the OnTransition event contains no subscribers.
        /// </summary>
        [Test]
        public void RaiseOnTransitionEvent_NoSubscriber()
        {
            byte raiseEventCount = 0;
            StateTransitionEventArgs<char> eventArgs = new StateTransitionEventArgs<char>("start", 'c');
            EventHandler<StateTransitionEventArgs<char>> eventHandler = (s, a) => ++raiseEventCount;

            Transition<char> transition = new Transition<char>("start", "start", ch => ch == 'c');
            transition.OnTransition += eventHandler;

            transition.RaiseOnTransitionEvent(eventArgs);
            Assert.That(raiseEventCount, Is.EqualTo(1));

            transition.OnTransition -= eventHandler;
            transition.RaiseOnTransitionEvent(eventArgs);
            Assert.That(raiseEventCount, Is.EqualTo(1));

            transition.OnTransition += eventHandler;
            transition.RaiseOnTransitionEvent(eventArgs);
            Assert.That(raiseEventCount, Is.EqualTo(2));
        }

        #endregion
    }
}