// ----------------------------------------------------------------------------
// ConvertTestFixture.cs
//
// Contains the definition of the ConvertTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/6/2009 7:09:48 PM
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class ConvertTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a Type object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Type()
        {
            Assert.That(Convert.ToXmlDocCommentMember(typeof(int)), Is.EqualTo("T:System.Int32"));
            Assert.That(Convert.ToXmlDocCommentMember(typeof(System.Xml.XmlDocument)), Is.EqualTo("T:System.Xml.XmlDocument"));
            Assert.That(Convert.ToXmlDocCommentMember(GetType()), Is.EqualTo("T:Jolt.Test.ConvertTestFixture"));
            
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Net.WebRequestMethods.File)),
                Is.EqualTo("T:System.Net.WebRequestMethods.File"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a Type object representing a generic
        /// type.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Type_Generic()
        {
            Assert.That(Convert.ToXmlDocCommentMember(typeof(System.Action<,,,>)), Is.EqualTo("T:System.Action`4"));
            Assert.That(Convert.ToXmlDocCommentMember(typeof(FiniteStateMachine<int>)), Is.EqualTo("T:Jolt.FiniteStateMachine`1"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>)),
                Is.EqualTo("T:System.Collections.Generic.List`1"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>.Enumerator)),
                Is.EqualTo("T:System.Collections.Generic.List`1.Enumerator"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is an EventInfo object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Event()
        {
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Transition<>).GetEvent("OnTransition")),
                Is.EqualTo("E:Jolt.Transition`1.OnTransition"));
            
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Console).GetEvent("CancelKeyPress")),
                Is.EqualTo("E:System.Console.CancelKeyPress"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a FieldInfo object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Field()
        {
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(int).GetField("MaxValue")),
                Is.EqualTo("F:System.Int32.MaxValue"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.FieldType<,>).GetField("Field", InternalInstance)),
                Is.EqualTo("F:Jolt.Test.Types.FieldType`2.Field"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a PropertyInfo object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Property()
        {
            Assert.That(Convert.ToXmlDocCommentMember(typeof(string).GetProperty("Length")), Is.EqualTo("P:System.String.Length"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.KeyValuePair<,>).GetProperty("Value")),
                Is.EqualTo("P:System.Collections.Generic.KeyValuePair`2.Value"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a PropertyInfo object representing
        /// an indexer.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Indexer()
        {
            // TODO: Factor out common anonymous delegates.
            Assert.That(Convert.ToXmlDocCommentMember(typeof(string).GetProperty("Chars")), Is.EqualTo("P:System.String.Chars(System.Int32)"));
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>).GetProperty("Item")),
                Is.EqualTo("P:System.Collections.Generic.List`1.Item(System.Int32)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<int>).GetProperty("Item")),
                Is.EqualTo("P:System.Collections.Generic.List`1.Item(System.Int32)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.IndexerType<,>).GetProperties(InternalInstance).Single(p => p.GetIndexParameters().Length == 4)),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(System.Int32,`0,`1,`0)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.IndexerType<,>).GetProperties(InternalInstance).Single(p => p.GetIndexParameters().Length == 1)),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(System.Action{System.Action{System.Action{`1}}})"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.IndexerType<,>).GetProperties(InternalInstance).Single(p => p.GetIndexParameters().Length == 3)),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(`0[],System.Action{System.Action{`1}[0:,0:][]}[][],`0[0:,0:,0:,0:][0:,0:,0:][0:,0:][])"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.PointerTestType<>).GetProperties(InternalInstance).Single()),
                Is.EqualTo("P:Jolt.Test.Types.PointerTestType`1.Item(System.Int32*[],System.Action{System.Action{`0[]}[][]}[],System.Int16***[0:,0:,0:][0:,0:][])"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a ConstructorInfo object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Constructor()
        {
            // TODO: Factor out common anonymous delegates.
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>).GetConstructor(NonPublicStatic, null, Type.EmptyTypes, null)),
                Is.EqualTo("M:System.Collections.Generic.List`1.#cctor"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(string).GetConstructor(NonPublicStatic, null, Type.EmptyTypes, null)),
                Is.EqualTo("M:System.String.#cctor"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Exception).GetConstructor(Type.EmptyTypes)),
                Is.EqualTo("M:System.Exception.#ctor"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>).GetConstructor(new Type[] { typeof(int) })),
                Is.EqualTo("M:System.Collections.Generic.List`1.#ctor(System.Int32)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.ConstructorType<,>).GetConstructors(InternalInstance).Single(c => c.GetParameters().Length == 4)),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(System.Int32,`0,`1,`1)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.ConstructorType<,>).GetConstructors(InternalInstance).Single(c => c.GetParameters().Length == 1)),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(System.Action{System.Action{System.Action{`0}}})"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.ConstructorType<,>).GetConstructors(InternalInstance).Single(c => c.GetParameters().Length == 3)),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(`0[],System.Action{System.Action{System.Action{`1}[][]}[]}[][]@,`1[0:,0:,0:,0:][0:,0:,0:][0:,0:][])"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.PointerTestType<>).GetConstructors(InternalInstance).Single()),
                Is.EqualTo("M:Jolt.Test.Types.PointerTestType`1.#ctor(System.Action{`0[]}[],System.String***[0:,0:,0:][0:,0:][]@)"));
        }

        /// <summary>
        /// Verifies the behavior of the ToXmlDocCommentMember() method
        /// when the given parameter is a ConstructorInfo object.
        /// </summary>
        [Test]
        public void ToXmlDocCommentMember_Method()
        {
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(int).GetMethod("GetHashCode")),
                Is.EqualTo("M:System.Int32.GetHashCode"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(string).GetMethod("Insert")),
                Is.EqualTo("M:System.String.Insert(System.Int32,System.String)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>).GetMethod("Clear")),
                Is.EqualTo("M:System.Collections.Generic.List`1.Clear"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(System.Collections.Generic.List<>).GetMethod("ConvertAll")),
                Is.EqualTo("M:System.Collections.Generic.List`1.ConvertAll(System.Converter{`0,``0})"));
            
            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Enumerable).GetMethods().Single(m => m.Name == "ToLookup" && m.GetParameters().Length == 4)),
                Is.EqualTo("M:System.Linq.Enumerable.ToLookup(System.Collections.Generic.IEnumerable{``0},System.Func{``0,``1},System.Func{``0,``2},System.Collections.Generic.IEqualityComparer{``1})"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(Types.PointerTestType<>).GetMethod("method", InternalInstance)),
                Is.EqualTo("M:Jolt.Test.Types.PointerTestType`1.method(System.Int32,`0[0:,0:]@,System.Action{``0[0:,0:][]}*[][0:,0:]@,System.Action{System.Int32**[0:,0:,0:][]})"));
        }

        #region private class data ----------------------------------------------------------------

        private static readonly BindingFlags InternalInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly BindingFlags NonPublicStatic = BindingFlags.Static | BindingFlags.NonPublic;

        #endregion
    }
}