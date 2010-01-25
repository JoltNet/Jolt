// ----------------------------------------------------------------------------
// CircularEnumeratorTestFixture.cs
//
// Contains the definition of the CircularEnumeratorTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/15/2009 09:51:30
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CircularEnumeratorTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Current property.
        /// </summary>
        [Test]
        public void Current()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            int expectedElement = 0;
            enumerator.Expect(e => e.Current).Return(expectedElement);

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            Assert.That(circularEnumerator.Current, Is.EqualTo(expectedElement));
        }

        /// <summary>
        /// Verifies the behavior of the non-generic Current property.
        /// </summary>
        [Test]
        public void Current_NonGeneric()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            object expectedElement = this;
            enumerator.Expect(e => (e as IEnumerator).Current).Return(expectedElement);

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            Assert.That((circularEnumerator as IEnumerator).Current, Is.SameAs(expectedElement));
        }

        /// <summary>
        /// Verifies the behavior of the MoveNext() method.
        /// </summary>
        [Test]
        public void MoveNext()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            bool expectedResult = true;
            enumerator.Expect(e => e.MoveNext()).Return(expectedResult);

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            Assert.That(circularEnumerator.MoveNext(), Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the MoveNext() method when the
        /// enumerator cycles back to the front of the collection.
        /// </summary>
        [Test]
        public void MoveNext_Cycle()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            bool expectedResult = true;
            enumerator.Expect(e => e.MoveNext()).Return(false);             // Reached the end of the collection.
            collection.Expect(c => c.GetEnumerator()).Return(enumerator);   // Cycles to the beginning of the collection.
            enumerator.Expect(e => e.MoveNext()).Return(expectedResult);    // Sets the position of the enumerator to the first element. 

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            Assert.That(circularEnumerator.MoveNext(), Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the Reset() method.
        /// </summary>
        [Test]
        public void Reset()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            enumerator.Expect(e => e.Reset());

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            circularEnumerator.Reset();
        }

        /// <summary>
        /// Verifies the behavior of the Dispose() method.
        /// </summary>
        [Test]
        public void Dispose()
        {
            IEnumerable<int> collection = MockRepository.GenerateStrictMock<IEnumerable<int>>();
            IEnumerator<int> enumerator = CreateMockEnumerator(collection);

            enumerator.Expect(e => e.Dispose());

            CircularEnumerator<int> circularEnumerator = new CircularEnumerator<int>(collection);
            circularEnumerator.Dispose();
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a mock enumerator from the given collection.
        /// </summary>
        /// 
        /// <typeparam name="TElement">
        /// The type of element that specializes the mock enumerator.
        /// </typeparam>
        /// 
        /// <param name="collection">
        /// The collection from which the mock enumerator is created.
        /// </param>
        /// 
        /// <returns>
        /// The requested mock enumerator.
        /// </returns>
        /// 
        /// <remarks>
        /// Sets an expectation on the given collection, configuring it to return
        /// the mock enumerator when an enumerator is requested.
        /// </remarks>
        private static IEnumerator<TElement> CreateMockEnumerator<TElement>(IEnumerable<TElement> collection)
        {
            IEnumerator<TElement> enumerator = MockRepository.GenerateStrictMock<IEnumerator<TElement>>();
            collection.Expect(c => c.GetEnumerator()).Return(enumerator);

            return enumerator;
        }

        #endregion
    }
}