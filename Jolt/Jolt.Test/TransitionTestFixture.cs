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
using QuickGraph;

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
            Predicate<char> transitionPredicate = ch => Char.IsDigit(ch);

            Transition<char> transition = new Transition<char>(sourceState, finalState, transitionPredicate);

            Assert.That(sourceState, Is.SameAs(transition.Source));
            Assert.That(finalState, Is.SameAs(transition.Target));
            Assert.That(transitionPredicate, Is.SameAs(transitionPredicate));
        }

        #endregion
    }
}