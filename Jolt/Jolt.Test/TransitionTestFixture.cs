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

            Assert.That(transition.Description, Is.SameAs(transitionPredicate.Method.Name));
            Assert.That(sourceState, Is.SameAs(transition.Source));
            Assert.That(finalState, Is.SameAs(transition.Target));
            Assert.That(transitionPredicate, Is.SameAs(transition.TransitionPredicate));
        }

        /// <summary>
        /// Verifies the construction of the transition class when a
        /// name/description is provided.
        /// </summary>
        [Test]
        public void Construction_NamedTransition()
        {
            string sourceState = "start";
            string finalState = "final";
            Predicate<char> transitionPredicate = Char.IsDigit;
            string description = "this is a transition";

            Transition<char> transition = new Transition<char>(sourceState, finalState, transitionPredicate, description);

            Assert.That(transition.Description, Is.SameAs(description));
            Assert.That(sourceState, Is.SameAs(transition.Source));
            Assert.That(finalState, Is.SameAs(transition.Target));
            Assert.That(transitionPredicate, Is.SameAs(transition.TransitionPredicate));
        }


        /// <summary>
        /// Verifies the behavior of the Description property.
        /// </summary>
        [Test]
        public void Description()
        {
            Transition<char> transition = new Transition<char>("a", "b", Char.IsDigit);
          
            string initialDescription = transition.Description;
            string description = "description-text";
            transition.Description = description;

            Assert.That(transition.Description, Is.Not.EqualTo(initialDescription));
            Assert.That(transition.Description, Is.SameAs(description));
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