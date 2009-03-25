// ----------------------------------------------------------------------------
// ConsumptionResultTestFixture.cs
//
// Contains the definition of the ConsumptionResultTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/23/2008 17:08:32
// ----------------------------------------------------------------------------

using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class ConsumptionResultTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            bool isAccepted = true;
            object lastSymbol = this;
            ulong numberOfSymbols = Int64.MaxValue;
            string lastState = new String('a', 123);

            ConsumptionResult<object> result = new ConsumptionResult<object>(isAccepted, lastSymbol, numberOfSymbols, lastState);
            Assert.That(result.IsAccepted, Is.EqualTo(isAccepted));
            Assert.That(result.LastSymbol, Is.SameAs(lastSymbol));
            Assert.That(result.NumberOfConsumedSymbols, Is.EqualTo(numberOfSymbols));
            Assert.That(result.LastState, Is.SameAs(lastState));
        }
    }
}