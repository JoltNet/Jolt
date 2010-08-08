// ----------------------------------------------------------------------------
// ReadOnlyDictionaryTestFixture.cs
//
// Contains the definition of the ReadOnlyDictionaryTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/7/2010 17:56:48
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Collections.Test
{
    [TestFixture]
    public sealed class ReadOnlyDictionaryTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            IDictionary<int, int> dictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(dictionary);

            Assert.That(readOnlyDictionary.Items, Is.SameAs(dictionary));
        }

        [Test]
        public void Construction_NullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new ReadOnlyDictionary<int, int>(null));
        }

        /// <summary>
        /// Verifies the behavior of the Add(KeyValuePair) method.
        /// </summary>
        [Test]
        public void Add_KeyValuePair()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.Throws<NotSupportedException>(() => dictionary.Add(new KeyValuePair<int, int>(100, 200)));
        }

        /// <summary>
        /// Verifies the behavior of the Clear() method.
        /// </summary>
        [Test]
        public void Clear()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.Throws<NotSupportedException>(() => dictionary.Clear());
        }

        /// <summary>
        /// Verifies the behavior of the Remove(KeyValuePair) method.
        /// </summary>
        [Test]
        public void Remove_KeyValuePair()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.Throws<NotSupportedException>(() => dictionary.Remove(new KeyValuePair<int, int>(100, 200)));
        }

        /// <summary>
        /// Verifies the behavior of the IsReadOnly property.
        /// </summary>
        [Test]
        public void IsReadOnly()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.That(dictionary.IsReadOnly);
        }

        /// <summary>
        /// Verifies the behavior of the Add() method.
        /// </summary>
        [Test]
        public void Add()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.Throws<NotSupportedException>(() => dictionary.Add(100, 200));
        }

        /// <summary>
        /// Verifies the behavior of the Remove() method.
        /// </summary>
        [Test]
        public void Remove()
        {
            IDictionary<int, int> dictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

            Assert.Throws<NotSupportedException>(() => dictionary.Remove(100));
        }

        /// <summary>
        /// Verifies the behavior of the explicitly implemented indexer.
        /// </summary>
        [Test]
        public void Indexer_ExplicitImplementation()
        {
            IDictionary<Uri, Uri> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<Uri, Uri>>();
            IDictionary<Uri, Uri> readOnlyDictionary = new ReadOnlyDictionary<Uri, Uri>(adaptedDictionary);

            adaptedDictionary.Expect(d => d[DefaultKey]).Return(DefaultValue);

            Assert.That(readOnlyDictionary[DefaultKey], Is.SameAs(DefaultValue));
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary[DefaultKey] = null);
        }

        /// <summary>
        /// Verifies the behavior of the non-generic GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator_NonGeneric()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            IEnumerator expectedEnumerator = MockRepository.GenerateStub<IEnumerator>();

            adaptedDictionary.Expect(d => (d as IEnumerable).GetEnumerator()).Return(expectedEnumerator);

            Assert.That((readOnlyDictionary as IEnumerable).GetEnumerator(), Is.SameAs(expectedEnumerator));
        }

        /// <summary>
        /// Verifies the behavior of the Count property.
        /// </summary>
        [Test]
        public void Count()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);

            adaptedDictionary.Expect(d => d.Count).Return(100);

            Assert.That(readOnlyDictionary.Count, Is.EqualTo(100));
        }

        /// <summary>
        /// Verifies the behavior of the indexer.
        /// </summary>
        [Test]
        public void Indexer()
        {
            IDictionary<Uri, Uri> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<Uri, Uri>>();
            ReadOnlyDictionary<Uri, Uri> readOnlyDictionary = new ReadOnlyDictionary<Uri, Uri>(adaptedDictionary);

            adaptedDictionary.Expect(d => d[DefaultKey]).Return(DefaultValue);

            Assert.That(readOnlyDictionary[DefaultKey], Is.SameAs(DefaultValue));
        }

        /// <summary>
        /// Verifies the behavior of the Keys property.
        /// </summary>
        [Test]
        public void Keys()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            ICollection<int> expectedKeys = new int[0];

            adaptedDictionary.Expect(d => d.Keys).Return(expectedKeys);

            Assert.That(readOnlyDictionary.Keys, Is.SameAs(expectedKeys));
        }

        /// <summary>
        /// Verifies the behavior of the Values property.
        /// </summary>
        [Test]
        public void Values()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            ICollection<int> expectedValues = new int[0];

            adaptedDictionary.Expect(d => d.Values).Return(expectedValues);

            Assert.That(readOnlyDictionary.Values, Is.SameAs(expectedValues));
        }

        /// <summary>
        /// Verifies the behavior of the Contains() method.
        /// </summary>
        [Test]
        public void Contains()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            KeyValuePair<int, int> expectedKeyValuePair = new KeyValuePair<int, int>();

            adaptedDictionary.Expect(d => d.Contains(expectedKeyValuePair)).Return(true);

            Assert.That(readOnlyDictionary.Contains(expectedKeyValuePair));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsKey() method.
        /// </summary>
        [Test]
        public void ContainsKey()
        {
            IDictionary<Uri, Uri> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<Uri, Uri>>();
            ReadOnlyDictionary<Uri, Uri> readOnlyDictionary = new ReadOnlyDictionary<Uri, Uri>(adaptedDictionary);

            adaptedDictionary.Expect(d => d.ContainsKey(DefaultKey)).Return(true);

            Assert.That(readOnlyDictionary.ContainsKey(DefaultKey));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTo() method.
        /// </summary>
        [Test]
        public void CopyTo()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            KeyValuePair<int, int>[] expectedArray = new KeyValuePair<int, int>[0];

            adaptedDictionary.Expect(d => d.CopyTo(expectedArray, 123));

            readOnlyDictionary.CopyTo(expectedArray, 123);
        }

        /// <summary>
        /// Verifies the behavior of the GetEnumerator() method.
        /// </summary>
        [Test]
        public void GetEnumerator()
        {
            IDictionary<int, int> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<int, int>>();
            ReadOnlyDictionary<int, int> readOnlyDictionary = new ReadOnlyDictionary<int, int>(adaptedDictionary);
            IEnumerator<KeyValuePair<int, int>> expectedEnumerator = MockRepository.GenerateStub<IEnumerator<KeyValuePair<int, int>>>();

            adaptedDictionary.Expect(d => d.GetEnumerator()).Return(expectedEnumerator);

            Assert.That(readOnlyDictionary.GetEnumerator(), Is.SameAs(expectedEnumerator));
        }

        /// <summary>
        /// Verifies the behavior of the TryGetValue() method.
        /// </summary>
        [Test]
        public void TryGetValue()
        {
            IDictionary<Uri, Uri> adaptedDictionary = MockRepository.GenerateStrictMock<IDictionary<Uri, Uri>>();
            ReadOnlyDictionary<Uri, Uri> readOnlyDictionary = new ReadOnlyDictionary<Uri, Uri>(adaptedDictionary);

            Uri actualValue;
            adaptedDictionary.Expect(d => d.TryGetValue(DefaultKey, out actualValue)).Return(true).OutRef(DefaultValue);

            Uri adaptedValue;
            Assert.That(readOnlyDictionary.TryGetValue(DefaultKey, out adaptedValue));
            Assert.That(adaptedValue, Is.SameAs(DefaultValue));
        }


        private static readonly Uri DefaultKey = new Uri("http://localhost");
        private static readonly Uri DefaultValue = new Uri("http://192.168.0.1");
    }
}