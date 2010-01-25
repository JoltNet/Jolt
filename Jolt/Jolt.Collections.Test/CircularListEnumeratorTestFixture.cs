// ----------------------------------------------------------------------------
// CircularListEnumeratorTestFixture.cs
//
// Contains the definition of the CircularListEnumeratorTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 12/17/2009 21:11:40
// ----------------------------------------------------------------------------

using NUnit.Framework;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class CircularListEnumeratorTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the Current property, prior to the
        /// first MoveNext() call.
        /// </summary>
        [Test]
        public void Current_NewEnumerator()
        {
            string[] collection = { "abc", "def", "ghi", "jkl", "mno", "pqr" };
            int startIndex = collection.Length / 2;
            
            CircularListEnumerator<string> enumerator = new CircularListEnumerator<string>(collection, startIndex);
            Assert.That(enumerator.Current, Is.Null);
        }

        /// <summary>
        /// Verifies the behavior of the MoveNextImpl() method, when the associated
        /// collection is empty.
        /// </summary>
        [Test]
        public void MoveNextImpl_EmptyCollection()
        {
            CircularListEnumerator<string> enumerator = new CircularListEnumerator<string>(new string[0], 0);
            Assert.That(!enumerator.MoveNext());
        }

        /// <summary>
        /// Verifies the behavior of the MoveNextImpl() method.
        /// </summary>
        [Test]
        public void MoveNextImpl()
        {
            string[] collection = { "abc", "def", "ghi", "jkl", "mno", "pqr" };
            int startIndex = 4;
            CircularListEnumerator<string> enumerator = new CircularListEnumerator<string>(collection, startIndex);

            Assert.That(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.SameAs(collection[startIndex]));
            Assert.That(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.SameAs(collection[startIndex + 1]));
        }

        /// <summary>
        /// Verifies the behavior of the MoveNextImpl() method, when cycling to the
        /// beginning of the collection.
        /// </summary>
        [Test]
        public void MoveNextImpl_Cycle()
        {
            string[] collection = { "abc", "def", "ghi", "jkl", "mno", "pqr" };
            int startIndex = collection.Length - 1;
            CircularListEnumerator<string> enumerator = new CircularListEnumerator<string>(collection, startIndex);

            Assert.That(enumerator.MoveNext());
            Assert.That(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.SameAs(collection[0]));
        }
    }
}