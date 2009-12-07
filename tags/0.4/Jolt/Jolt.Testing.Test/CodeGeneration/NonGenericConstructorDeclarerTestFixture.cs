// ----------------------------------------------------------------------------
// NonGenericConstructorDeclarerTestFixture.cs
//
// Contains the definition of the NonGenericConstructorDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 19:12:33
// ----------------------------------------------------------------------------

using System.Reflection;
using System.Reflection.Emit;

using Jolt.Functional;
using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class NonGenericConstructorDeclarerTestFixture : AbstractConstructorDeclarerTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with no parameters.
        /// </summary>
        [Test]
        public void Create_NoParameters()
        {
            AssertConstructorDeclaredFrom(
                __ConstructorTestType.Ctor_ZeroArgs,
                Functor.NoOperation<ConstructorInfo, ConstructorInfo>());
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with one parameter.
        /// </summary>
        [Test]
        public void Create_OneParameter()
        {
            AssertConstructorDeclaredFrom(
                __ConstructorTestType.Ctor_OneArg,
                Functor.NoOperation<ConstructorInfo, ConstructorInfo>());
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameter()
        {
            AssertConstructorDeclaredFrom(
                __ConstructorTestType.Ctor_TwoArgs,
                Functor.NoOperation<ConstructorInfo, ConstructorInfo>());
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <see cref="AbstractConstructorDeclarerTestFixture.CreateConstructorDeclarer"/>
        internal override AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> CreateConstructorDeclarer(
            ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation)
        {
            return new NonGenericConstructorDeclarer(
                CurrentTypeBuilder,
                ConstructorAttributes,
                realSubjectTypeConstructor,
                implementation);
        }

        #endregion
    }
}
