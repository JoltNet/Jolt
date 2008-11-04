// ----------------------------------------------------------------------------
// ConvertTestFixture.cs
//
// Contains the definition of the ConvertTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 8/9/2007 09:24:59
// ----------------------------------------------------------------------------

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

            Assert.That(methodParamTypes.Length, Is.EqualTo(4));
            Assert.That(methodParamTypes.Length, Is.EqualTo(methodParams.Length));
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

            Assert.That(methodParamTypes.Length, Is.EqualTo(0));
            Assert.That(methodParamTypes.Length, Is.EqualTo(methodParams.Length));
        }

        /// <summary>
        /// Verified the behavior of the ToParameterTypes() method when a
        /// given method has generic parameters declared at the the class level.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeParams()
        {
            System.Type[] genericTypeParams = typeof(__GenericTestType<,,>).GetGenericArguments();
            ParameterInfo[] methodParams = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction_MixedArgs").GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeParams);

            Assert.That(methodParamTypes.Length, Is.EqualTo(3));
            Assert.That(methodParamTypes.Length, Is.EqualTo(methodParams.Length));
            Assert.That(methodParamTypes[0], Is.EqualTo(genericTypeParams[1]));
            Assert.That(methodParamTypes[1], Is.EqualTo(genericTypeParams[2]));
            Assert.That(methodParamTypes[2], Is.EqualTo(typeof(int)));
        }

        /// <summary>
        /// Verified the behavior of the ToParameterTypes() method when a
        /// given method has no parameters, but the declaring class has
        /// generic parameters.
        /// </summary>
        [Test]
        public void ToParameterTypes_GenericTypeParams_NoParams()
        {
            System.Type[] genericTypeParams = typeof(__GenericTestType<,,>).GetGenericArguments();
            ParameterInfo[] methodParams = typeof(__GenericTestType<,,>).GetMethod("NoParameters").GetParameters();
            System.Type[] methodParamTypes = Convert.ToParameterTypes(methodParams, genericTypeParams);

            Assert.That(methodParamTypes.Length, Is.EqualTo(0));
            Assert.That(methodParamTypes.Length, Is.EqualTo(methodParams.Length));
        }

        /// <summary>
        /// Verifies the behavior of the ToTypeNames() method.
        /// </summary>
        [Test]
        public void ToTypeNames()
        {
            System.Type[] types = { typeof(int), typeof(int), typeof(double), typeof(byte) };
            string[] typeNames = Convert.ToTypeNames(types);
            
            Assert.That(typeNames.Length, Is.EqualTo(4));
            Assert.That(typeNames.Length, Is.EqualTo(types.Length));
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
