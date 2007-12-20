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

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        private void __f() { }
        private void __g(int x, int y, double z, byte b) { }

        #endregion
    }
}
