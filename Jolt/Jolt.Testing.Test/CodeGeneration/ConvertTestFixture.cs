// ----------------------------------------------------------------------------
// ConvertTestFixture.cs
//
// Contains the definition of the ConvertTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 8/9/2007 09:24:59
// ----------------------------------------------------------------------------

using System.Linq;
using System.Reflection;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class ConvertTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the ToParamterTypes() method.
        /// </summary>
        [Test]
        public void ToParameterTypes()
        {
            ParameterInfo[] methodParams = GetType().GetMethod("__g", BindingFlags.NonPublic | BindingFlags.Instance).GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams);

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
            ParameterInfo[] methodParams = GetType().GetMethod("__f", BindingFlags.NonPublic | BindingFlags.Instance).GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams);

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
            System.Type[] genericTypeArguments = typeof(__GenericTestType<,,>).GetGenericArguments();
            ParameterInfo[] methodParams = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction_MixedArgs").GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments);

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

            System.Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            ParameterInfo[] methodParams = genericMethod.GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments);

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
            System.Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            System.Type[] genericMethodArguments = genericMethod.GetGenericArguments();
            
            ParameterInfo[] methodParams = genericMethod.GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments, genericMethodArguments);

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

            System.Type[] genericTypeArguments = genericMethod.DeclaringType.GetGenericArguments();
            System.Type[] genericMethodArguments = genericMethod.GetGenericArguments();

            ParameterInfo[] methodParams = genericMethod.GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeArguments, genericMethodArguments);

            Assert.That(methodParamTypes, Is.Empty);
            Assert.That(methodParamTypes, Has.Length(methodParams.Length));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterType() method when
        /// the given parameter is not generic.
        /// </summary>
        [Test]
        public void ToParameterType()
        {
            System.Type parameterType = Convert.ToParameterType(
                typeof(__GenericTestType<,,>).GetMethod("NoGenericParameters").GetParameters()[0],
                System.Type.EmptyTypes,
                System.Type.EmptyTypes);

            Assert.That(parameterType, Is.EqualTo(typeof(int)));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterType() method when
        /// the given parameter type is a generic type argument.
        /// </summary>
        [Test]
        public void ToParameterType_GenericTypeArgument()
        {
            MethodInfo nonGenericMethod = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction");
            System.Type[] genericTypeArguments = nonGenericMethod.DeclaringType.GetGenericArguments();
            
            System.Type parameterType = Convert.ToParameterType(
                nonGenericMethod.GetParameters()[0],
                genericTypeArguments,
                System.Type.EmptyTypes);

            Assert.That(parameterType, Is.EqualTo(genericTypeArguments[1]));
        }

        /// <summary>
        /// Verifies the behavior of the ToParameterType() method when
        /// the given parameter type is a generic method argument.
        /// </summary>
        [Test]
        public void ToParameterType_GenericMethodArgument()
        {
            MethodInfo genericMethod = typeof(__GenericTestType<,,>).GetMethod("GenericFunction");
            System.Type[] genericMethodArguments = genericMethod.GetGenericArguments();

            System.Type parameterType = Convert.ToParameterType(
                genericMethod.GetParameters()[0],
                genericMethodArguments,
                System.Type.EmptyTypes);

            Assert.That(parameterType, Is.EqualTo(genericMethodArguments[0]));
        }

        /// <summary>
        /// Verifies the behavior of the ToTypeNames() method.
        /// </summary>
        [Test]
        public void ToTypeNames()
        {
            System.Type[] types = { typeof(int), typeof(int), typeof(double), typeof(byte) };
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
            Assert.That(Convert.ToTypeNames(System.Type.EmptyTypes), Is.Empty);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        private void __f() { }
        private void __g(int x, int y, double z, byte b) { }

        #endregion
    }
}
