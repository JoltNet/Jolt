// ----------------------------------------------------------------------------
// ConsumptionResultTestFixture.cs
//
// Contains the definition of the ConsumptionResultTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/23/2008 17:08:32
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Jolt.Automata.Test
{
    [TestFixture]
    public sealed class ConsumptionResultTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class, when given a single state.
        /// </summary>
        [Test]
        public void Construction_SingleState()
        {
            bool isAccepted = true;
            object lastSymbol = this;
            ulong numberOfSymbols = Int64.MaxValue;
            string lastState = new String('a', 123);

            ConsumptionResult<object> result = new ConsumptionResult<object>(isAccepted, lastSymbol, numberOfSymbols, lastState);
            Assert.That(result.IsAccepted, Is.EqualTo(isAccepted));
            Assert.That(result.LastSymbol, Is.SameAs(lastSymbol));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(numberOfSymbols));
            Assert.That(result.LastStates, Is.EqualTo(new[] { lastState }));
        }

        /// <summary>
        /// Verifies the construction of the class, when given many states.
        /// </summary>
        [Test]
        public void Construction_ManyState()
        {
            bool isAccepted = true;
            object lastSymbol = this;
            ulong numberOfSymbols = Int64.MaxValue;
            string[] lastStates = { "aaa", "bbb", "ccc", "ddd", "eee" };

            ConsumptionResult<object> result = new ConsumptionResult<object>(isAccepted, lastSymbol, numberOfSymbols, lastStates);
            Assert.That(result.IsAccepted, Is.EqualTo(isAccepted));
            Assert.That(result.LastSymbol, Is.SameAs(lastSymbol));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(numberOfSymbols));
            Assert.That(result.LastStates, Is.EqualTo(lastStates));
        }

        /// <summary>
        /// Verifies the type of enumerator returned by the LastStates property.
        /// </summary>
        [Test]
        public void LastStates()
        {
            ConsumptionResult<object> result = new ConsumptionResult<object>(true, this, Int64.MaxValue, new[] { "lastState" });
            Assert.That(result.LastStates, Is.Not.InstanceOf<HashSet<string>>());
        }
    }
}