// ----------------------------------------------------------------------------
// GenericConstructorDeclarerTestFixture.cs
//
// Contains the definition of the GenericConstructorDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 9/3/2008 09:54:10
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public class GenericConstructorDeclarerTestFixture : AbstractConstructorDeclarerTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with no parameters.
        /// </summary>
        [Test]
        public void Create_NoParameters()
        {
            InitializeCurrentTypeBuilder(typeof(__ConstructorTestType<,>));
            AssertConstructorDeclaredFrom(__ConstructorTestType.Ctor_ZeroArgs, AssertConstructorAttributes);
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with one parameter.
        /// </summary>
        [Test]
        public void Create_OneParameter()
        {
            InitializeCurrentTypeBuilder(typeof(__ConstructorTestType<,>));
            AssertConstructorDeclaredFrom(__ConstructorTestType<int, int>.Ctor_OneArg, AssertConstructorAttributes);
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one generic parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameters()
        {
            InitializeCurrentTypeBuilder(typeof(__ConstructorTestType<,>));
            AssertConstructorDeclaredFrom(__ConstructorTestType<int, int>.Ctor_TwoArgs, AssertConstructorAttributes);
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameters_Mixed()
        {
            InitializeCurrentTypeBuilder(typeof(__ConstructorTestType<,>));
            AssertConstructorDeclaredFrom(__ConstructorTestType<int, int>.Ctor_ThreeArgs, AssertConstructorAttributes);
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <see cref="AbstractConstructorDeclarerTestFixture.CreateConstructorDeclarer"/>
        internal override AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> CreateConstructorDeclarer(
            ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation)
        {
            return new GenericConstructorDeclarer(
                CurrentTypeBuilder,
                ConstructorAttributes,
                realSubjectTypeConstructor,
                implementation);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Adds generic parameters to the current type builder, modelled from
        /// the given generic type.
        /// </summary>
        /// 
        /// <param name="genericType">
        /// The type from which the generic parameters are dervied.
        /// </param>
        private void InitializeCurrentTypeBuilder(Type genericType)
        {
            CurrentTypeBuilder.DefineGenericParameters(Convert.ToTypeNames(genericType.GetGenericArguments()));
        }

        /// <summary>
        /// Asserts on the attributes of the given constructors.  Verifies
        /// that the actual attributes match the expected attributes.
        /// </summary>
        /// 
        /// <param name="actualConstructor">
        /// The constructor to verify.
        /// </param>
        /// 
        /// <param name="expectedConstructor">
        /// The constructor with the expected values.
        /// </param>
        private void AssertConstructorAttributes(ConstructorInfo actualConstructor, ConstructorInfo expectedConstructor)
        {
            Type[] constructorParameterTypes = Convert.ToParameterTypes(actualConstructor.GetParameters());
            Type[] expectedConstructorParameterTypes = Convert.ToParameterTypes(expectedConstructor.GetParameters());
            Assert.That(constructorParameterTypes, Has.Length.EqualTo(expectedConstructorParameterTypes.Length));

            for (int i = 0; i < constructorParameterTypes.Length; ++i)
            {
                if (constructorParameterTypes[i].IsGenericParameter)
                {
                    Assert.That(expectedConstructorParameterTypes[i].IsGenericParameter);
                    Assert.That(constructorParameterTypes[i].DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                    Assert.That(constructorParameterTypes[i].GenericParameterPosition, Is.EqualTo(expectedConstructorParameterTypes[i].GenericParameterPosition));
                }
                else
                {
                    Assert.That(constructorParameterTypes[i], Is.EqualTo(expectedConstructorParameterTypes[i]));
                }
            }
        }

        #endregion
    }
}
