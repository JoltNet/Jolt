// ----------------------------------------------------------------------------
// TransitionTestFixture.cs
//
// Contains the definition of the TransitionTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/7/2008 11:30:00
// ----------------------------------------------------------------------------

using System;

using Jolt.Functional;
using NUnit.Framework;

namespace Jolt.Automata.Test
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
            
            Transition<char> transition = new Transition<char>("start", "start", 'c'.Equals);
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

            Transition<char> transition = new Transition<char>("start", "start", 'c'.Equals);
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

        /// <summary>
        /// Verifies the behavior of the Equals(object) method
        /// when the given object for comparison is not a transition.
        /// </summary>
        [Test]
        public void Equals_Object_NotTransition()
        {
            Transition<char> trans_x = new Transition<char>("start", "final", Char.IsDigit);
            Assert.That(!trans_x.Equals(this));
        }

        /// <summary>
        /// Verifies the axiomatic behavior of the Equals(Transition) method.
        /// </summary>
        [Test]
        public void Equals()
        {
            EventHandler<StateTransitionEventArgs<char>> eventHandler = Functor.ToEventHandler(Functor.NoOperation<object, StateTransitionEventArgs<char>>());
            Transition<char> trans_x = new Transition<char>("start", "final", Char.IsDigit);
            trans_x.OnTransition += eventHandler;

            Transition<char> trans_y = new Transition<char>("start", "final", Char.IsDigit);
            trans_y.OnTransition += eventHandler;

            Transition<char> trans_z = new Transition<char>("start", "final", Char.IsDigit);
            trans_z.OnTransition += eventHandler;

            // Value-based equality assertions.
            Assert.That(trans_x, Is.EqualTo(trans_y));
            Assert.That(trans_y, Is.EqualTo(trans_z));

            // Equality axiom assertions.
            Assert.That(trans_x.Equals(trans_x));
            Assert.That(trans_x.Equals(trans_y), Is.EqualTo(trans_y.Equals(trans_x)));
            
            if (trans_x.Equals(trans_y) && trans_y.Equals(trans_z))
            {
                Assert.That(trans_x.Equals(trans_z));
            }

            if (trans_x.Equals(trans_z))
            {
                Assert.That(trans_x.Equals(trans_y) && trans_y.Equals(trans_z));
            }

            Assert.That(!trans_x.Equals(null));
        }

        /// <summary>
        /// Verifies the behavior of the Equals(Transition) method
        /// when two transitions are not equal by source state.
        /// </summary>
        [Test]
        public void Equals_NotEqualBySource()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("target", "target", Char.IsDigit);

            Assert.That(trans_x, Is.Not.EqualTo(trans_y));
        }

        /// <summary>
        /// Verifies the behavior of the Equals(Transition) method
        /// when two transitions are not equal by target state.
        /// </summary>
        [Test]
        public void Equals_NotEqualByTarget()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("source", "source", Char.IsDigit);

            Assert.That(trans_x, Is.Not.EqualTo(trans_y));
        }

        /// <summary>
        /// Verifies the behavior of the Equals(Transition) method
        /// when two transitions are not equal by transition predicate.
        /// </summary>
        [Test]
        public void Equals_NotEqualByPredicate()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("source", "target", Char.IsLetter);

            Assert.That(trans_x, Is.Not.EqualTo(trans_y));
        }

        /// <summary>
        /// Verifies the behavior of the Equals(Transition) method
        /// when two transitions are not equal by description.
        /// </summary>
        [Test]
        public void Equals_NotEqualByDescription()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("source", "target", Char.IsDigit, "description");

            Assert.That(trans_x, Is.Not.EqualTo(trans_y));
        }

        /// <summary>
        /// Verifies the behavior of the Equals(Transition) method
        /// when two transitions are not equal by event handler.
        /// </summary>
        [Test]
        public void Equals_NotEqualByEventHandler()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("source", "target", Char.IsDigit);
            trans_y.OnTransition += Functor.ToEventHandler(Functor.NoOperation<object, StateTransitionEventArgs<char>>());

            Assert.That(trans_x, Is.Not.EqualTo(trans_y));
        }

        /// <summary>
        /// Verifies the behavior of the GetHashCode() method when
        /// two transition objects are equal.
        /// </summary>
        [Test]
        public new void GetHashCode()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            Transition<char> trans_y = new Transition<char>("source", "target", Char.IsDigit);

            Assert.That(trans_x, Is.EqualTo(trans_y));
            Assert.That(trans_x.GetHashCode(), Is.EqualTo(trans_y.GetHashCode()));
        }

        /// <summary>
        /// Verifies the behavior of the GetHashCode() method when
        /// a state change occurs as part of the Description property.
        /// </summary>
        [Test]
        public void GetHashCode_DescriptionChange()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            int hashCode = trans_x.GetHashCode();
            trans_x.Description = null;

            Assert.That(hashCode, Is.Not.EqualTo(trans_x.GetHashCode()));
        }

        /// <summary>
        /// Verifies the behavior of the GetHashCode() method when
        /// a state change occurs as part of the OnTransition event.
        /// </summary>
        [Test]
        public void GetHashCode_EventHandlerChange()
        {
            Transition<char> trans_x = new Transition<char>("source", "target", Char.IsDigit);
            int hashCode = trans_x.GetHashCode();
            trans_x.OnTransition += Functor.ToEventHandler(Functor.NoOperation<object, StateTransitionEventArgs<char>>());

            Assert.That(hashCode, Is.Not.EqualTo(trans_x.GetHashCode()));
        }

        #endregion
    }
}