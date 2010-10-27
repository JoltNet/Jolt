// ----------------------------------------------------------------------------
// AbstractSeedableEnumeratorTestFixture.cs
//
// Contains the definition of the AbstractSeedableEnumeratorTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/17/2009 08:34:19
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Jolt.Reflection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Collections.Test
{
    using SeedableEnumerator = AbstractSeedableEnumerator<int, int>;


    [TestFixture]
    public sealed class AbstractSeedableEnumeratorTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the non-generic Current property.
        /// </summary>
        [Test]
        public void Current_NonGeneric()
        {
            SeedableEnumerator enumerator = MockRepository.GenerateStub<SeedableEnumerator>(new int[0], 0);

            int expectedResult = 123;
            enumerator.Expect(e => e.Current).Return(expectedResult);

            Assert.That((enumerator as IEnumerator).Current, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// Verifies the behavior of the MoveNext() method.
        /// </summary>
        [Test]
        public void MoveNext()
        {
            IEnumerable<int> collection = MockRepository.GenerateStub<IList<int>>();
            IEnumerator<int> collectionEnumeretor = CreateMockEnumerator(collection);
            SeedableEnumerator enumerator = MockRepository.GenerateStub<SeedableEnumerator>(collection, 0);

            collectionEnumeretor.Expect(e => e.Reset());
            enumerator.Expect(e =>
            {
                return (bool)e.GetType().GetMethod("MoveNextImpl", CompoundBindingFlags.NonPublicInstance).Invoke(e, null);
            }).Return(true);
                
            Assert.That(enumerator.MoveNext());
        }

        /// <summary>
        /// Verifies the behavior of the Reset() method.
        /// </summary>
        [Test]
        public void Reset()
        {
            IEnumerable<int> collection = MockRepository.GenerateStub<IList<int>>();
            IEnumerator<int> collectionEnumeretor = CreateMockEnumerator(collection);

            int startIndex = 123;
            SeedableEnumerator enumerator = MockRepository.GenerateStub<SeedableEnumerator>(collection, startIndex);

            collectionEnumeretor.Expect(e => e.Reset());

            PropertyInfo currentIndex = GetCurrentIndexProperty();
            currentIndex.SetValue(enumerator, 456, null);

            enumerator.Reset();
            Assert.That(currentIndex.GetValue(enumerator, null), Is.EqualTo(startIndex));
        }

        /// <summary>
        /// Verifies the behavior of the Dispose() method.
        /// </summary>
        [Test]
        public void Dispose()
        {
            IEnumerable<int> collection = MockRepository.GenerateStub<IList<int>>();
            IEnumerator<int> collectionEnumeretor = CreateMockEnumerator(collection);
            SeedableEnumerator enumerator = MockRepository.GenerateStub<SeedableEnumerator>(collection, 0);

            collectionEnumeretor.Expect(e => e.Dispose());

            enumerator.Dispose();
        }

        /// <summary>
        /// Verifies the behavior of the CurrentIndex property.
        /// </summary>
        [Test]
        public void CurrentIndex()
        {
            SeedableEnumerator enumerator = MockRepository.GenerateStub<SeedableEnumerator>(new int[0], 0);

            int expectedValue = 123;
            PropertyInfo currentIndex = GetCurrentIndexProperty();

            int initialIndexValue = (int)currentIndex.GetValue(enumerator, null);
            Assert.That(currentIndex.GetValue(enumerator, null), Is.Not.EqualTo(expectedValue), "Test case precondition failure");
            
            currentIndex.SetValue(enumerator, expectedValue, null);
            Assert.That(currentIndex.GetValue(enumerator, null), Is.EqualTo(expectedValue));
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

        /// <summary>
        /// Gets the CurrentIndex property of an <see cref="AbstractCiruclarListEnumerator"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="System.Reflection.PropertyInfo"/> object for the requested property.
        /// </returns>
        private static PropertyInfo GetCurrentIndexProperty()
        {
            return typeof(SeedableEnumerator).GetProperty("CurrentIndex", CompoundBindingFlags.NonPublicInstance);
        }

        #endregion
    }
}