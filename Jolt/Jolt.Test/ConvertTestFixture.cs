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

using Jolt.Functional;
using Jolt.Test.Types;
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
            Assert.That(Convert.ToXmlDocCommentMember(typeof(__GenericTestType<int, char, byte>)), Is.EqualTo("T:Jolt.Test.Types.__GenericTestType`3"));

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
                Convert.ToXmlDocCommentMember(typeof(__GenericTestType<int, char, byte>).GetEvent("InstanceEvent")),
                Is.EqualTo("E:Jolt.Test.Types.__GenericTestType`3.InstanceEvent"));
            
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
                Convert.ToXmlDocCommentMember(typeof(FieldType<,>).GetField("Field", NonPublicInstance)),
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
                Convert.ToXmlDocCommentMember(typeof(IndexerType<,>).GetProperties(NonPublicInstance).Single(CreateNumIndexParamsPredicate(4))),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(System.Int32,`0,`1,`0)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(IndexerType<,>).GetProperties(NonPublicInstance).Single(CreateNumIndexParamsPredicate(1))),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(System.Action{System.Action{System.Action{`1}}})"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(IndexerType<,>).GetProperties(NonPublicInstance).Single(CreateNumIndexParamsPredicate(3))),
                Is.EqualTo("P:Jolt.Test.Types.IndexerType`2.Item(`0[],System.Action{System.Action{`1}[0:,0:][]}[][],`0[0:,0:,0:,0:][0:,0:,0:][0:,0:][])"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(PointerTestType<>).GetProperties(NonPublicInstance).Single()),
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
                Convert.ToXmlDocCommentMember(typeof(ConstructorType<,>).GetConstructors(NonPublicInstance).Single(CreateNumCtorParamsPredicate(4))),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(System.Int32,`0,`1,`1)"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(ConstructorType<,>).GetConstructors(NonPublicInstance).Single(CreateNumCtorParamsPredicate(1))),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(System.Action{System.Action{System.Action{`0}}})"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(ConstructorType<,>).GetConstructors(NonPublicInstance).Single(CreateNumCtorParamsPredicate(3))),
                Is.EqualTo("M:Jolt.Test.Types.ConstructorType`2.#ctor(`0[],System.Action{System.Action{System.Action{`1}[][]}[]}[][]@,`1[0:,0:,0:,0:][0:,0:,0:][0:,0:][])"));

            Assert.That(
                Convert.ToXmlDocCommentMember(typeof(PointerTestType<>).GetConstructors(NonPublicInstance).Single()),
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
                Convert.ToXmlDocCommentMember(typeof(PointerTestType<>).GetMethod("method", NonPublicInstance)),
                Is.EqualTo("M:Jolt.Test.Types.PointerTestType`1.method(System.Int32,`0[0:,0:]@,System.Action{``0[0:,0:][]}*[][0:,0:]@,System.Action{System.Int32**[0:,0:,0:][]})"));
        }

        /// <summary>
        /// Verifies the behavior of the ToParamterTypes() method.
        /// </summary>
        [Test]
        public void ToParameterTypes()
        {
            ParameterInfo[] methodParams = GetType().GetMethod("__g", NonPublicInstance).GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams);

            Assert.That(methodParamTypes, Has.Length(4));
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
            Assert.That(methodParamTypes[0], Is.EqualTo(typeof(int)));
            Assert.That(methodParamTypes[1], Is.EqualTo(typeof(int)));
            Assert.That(methodParamTypes[2], Is.EqualTo(typeof(double)));
            Assert.That(methodParamTypes[3], Is.EqualTo(typeof(byte)));
        }

        /// <summary>
        /// Verifies the behavior of the ToParamterTypes() method when a
        /// given method has no parameters..
        /// </summary>
        [Test]
        public void ToParameterTypes_NoParams()
        {
            ParameterInfo[] methodParams = GetType().GetMethod("__f", NonPublicInstance).GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams);

            Assert.That(methodParamTypes, Is.Empty);
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterTypes() method when a
        /// given method has generic arguments declared at the class level.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeArguments()
        {
            Type[] genericTypeArguments = typeof(__GenericTestType<,,>).GetGenericArguments();
            ParameterInfo[] methodParams = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction_MixedArgs").GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments);

            Assert.That(methodParamTypes, Has.Length(3));
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
            Assert.That(methodParamTypes[0], Is.EqualTo(genericTypeArguments[1]));
            Assert.That(methodParamTypes[1], Is.EqualTo(genericTypeArguments[2]));
            Assert.That(methodParamTypes[2], Is.EqualTo(typeof(int)));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterTypes() method when a
        /// given method has no parameters, but the declaring class has
        /// generic arguments.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeArguments_NoParams()
        {
            MethodInfo genericMethod = typeof(__GenericTestType<,,>).GetMethods().Single(m => m.Name == "NoParameters" && !m.IsGenericMethod);

            Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            ParameterInfo[] methodParams = genericMethod.GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments);

            Assert.That(methodParamTypes, Is.Empty);
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterTypes() method when a
        /// given method has generic arguments declared at the class
        /// and method level.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeAndMethodArguments()
        {
            MethodInfo genericMethod = typeof(__GenericTestType<,,>).GetMethod("GenericFunction_MixedArgs");
            Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            Type[] genericMethodArguments = genericMethod.GetGenericArguments();

            ParameterInfo[] methodParams = genericMethod.GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments, genericMethodArguments);

            Assert.That(methodParamTypes, Has.Length(6));
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
            Assert.That(methodParamTypes[0], Is.EqualTo(genericMethodArguments[2]));
            Assert.That(methodParamTypes[1], Is.EqualTo(genericMethodArguments[0]));
            Assert.That(methodParamTypes[2], Is.EqualTo(genericMethodArguments[1]));
            Assert.That(methodParamTypes[3], Is.EqualTo(genericTypeArguments[2]));
            Assert.That(methodParamTypes[4], Is.EqualTo(genericTypeArguments[1]));
            Assert.That(methodParamTypes[5], Is.EqualTo(typeof(int)));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterTypes() method when a
        /// given method has no parameters, but the method and declaring class
        /// have generic arguments.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeAndMethodArguments_NoParams()
        {
            MethodInfo genericMethod = typeof(__GenericTestType<,,>).GetMethods().Single(m => m.Name == "NoParameters" && m.IsGenericMethod);

            Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            Type[] genericMethodArguments = genericMethod.GetGenericArguments();

            ParameterInfo[] methodParams = genericMethod.GetParameters();
            Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments, genericMethodArguments);

            Assert.That(methodParamTypes, Is.Empty);
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
        }

        /// <summary>
        /// Verifies the behavior of the ToMethodSignatureType() method when
        /// the given type is not generic.
        /// </summary>
        [Test]
        public void ToMethodSignatureType()
        {
            Type type = Convert.ToMethodSignatureType(
                typeof(__GenericTestType<,,>).GetMethod("NoGenericParameters").GetParameters()[0].ParameterType,
                Type.EmptyTypes,
                Type.EmptyTypes);

            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        /// <summary>
        /// Verifies the behavior of the ToMethodSignatureType() method when
        /// the given type is a generic type argument.
        /// </summary>
        [Test]
        public void ToMethodSignatureType_GenericTypeArgument()
        {
            MethodInfo nonGenericMethod = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction");
            Type[] genericTypeArguments = nonGenericMethod.DeclaringType.GetGenericArguments();

            Type type = Convert.ToMethodSignatureType(
                nonGenericMethod.GetParameters()[0].ParameterType,
                genericTypeArguments,
                Type.EmptyTypes);

            Assert.That(type, Is.EqualTo(genericTypeArguments[1]));
        }

        /// <summary>
        /// Verifies the behavior of the ToMethodSignatureType() method when
        /// the given parameter type is a generic method argument.
        /// </summary>
        [Test]
        public void ToMethodSignatureType_GenericMethodArgument()
        {
            MethodInfo genericMethod = typeof(__GenericTestType<,,>).GetMethod("GenericFunction");
            Type[] genericMethodArguments = genericMethod.GetGenericArguments();

            Type type = Convert.ToMethodSignatureType(
                genericMethod.GetParameters()[0].ParameterType,
                genericMethodArguments,
                Type.EmptyTypes);

            Assert.That(type, Is.EqualTo(genericMethodArguments[0]));
        }

        /// <summary>
        /// Verifies the behavior of the ToTypeNames() method.
        /// </summary>
        [Test]
        public void ToTypeNames()
        {
            Type[] types = { typeof(int), typeof(int), typeof(double), typeof(byte) };
            string[] typeNames = Convert.ToTypeNames(types);

            Assert.That(typeNames, Has.Length(4));
            Assert.That(typeNames, Has.Length(types.Length));
            Assert.That(typeNames[0], Is.EqualTo("Int32"));
            Assert.That(typeNames[1], Is.EqualTo("Int32"));
            Assert.That(typeNames[2], Is.EqualTo("Double"));
            Assert.That(typeNames[3], Is.EqualTo("Byte"));
        }

        /// <summary>
        /// Verifies the behavior o fhte ToTypeNames() method when a
        /// given type list has no items.
        /// </summary>
        [Test]
        public void ToTypeNames_NoTypes()
        {
            Assert.That(Convert.ToTypeNames(Type.EmptyTypes), Is.Empty);
        }

        #region private methods -------------------------------------------------------------------

        private void __f() { }
        private void __g(int x, int y, double z, byte b) { }

        /// <summary>
        /// Creates a predicate that determines if a constructor
        /// has the given number of parameters.
        /// </summary>
        /// 
        /// <param name="desiredNumParameters">
        /// The desired number of parameters.
        /// </param>
        private Func<ConstructorInfo, bool> CreateNumCtorParamsPredicate(int desiredNumParameters)
        {
            return Bind.Second(HasNParameters, desiredNumParameters);
        }

        /// <summary>
        /// Creates a predicate that determines if a property
        /// has the given number of index parameters.
        /// </summary>
        /// 
        /// <param name="desiredNumParameters">
        /// The desired number of parameters.
        /// </param>
        private Func<PropertyInfo, bool> CreateNumIndexParamsPredicate(int desiredNumParameters)
        {
            return Bind.Second(HasNIndexParameters, desiredNumParameters);
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly BindingFlags NonPublicStatic = BindingFlags.Static | BindingFlags.NonPublic;
        private static readonly Func<ConstructorInfo, int, bool> HasNParameters = (ctor, numParams) => ctor.GetParameters().Length == numParams;
        private static readonly Func<PropertyInfo, int, bool> HasNIndexParameters = (property, numParams) => property.GetIndexParameters().Length == numParams;

        #endregion
    }
}