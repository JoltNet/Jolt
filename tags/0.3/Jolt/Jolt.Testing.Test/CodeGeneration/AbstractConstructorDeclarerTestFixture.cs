// ----------------------------------------------------------------------------
// AbstractConstructorDeclarerTestFixture.cs
//
// Contains the definition of the AbstractConstructorDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 11/5/2008 22:38:51
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using RMC = Rhino.Mocks.Constraints;

namespace Jolt.Testing.Test.CodeGeneration
{
    public abstract class AbstractConstructorDeclarerTestFixture : AbstractMethodDeclarerTestFixture<ConstructorBuilder, ConstructorInfo>
    {
        /// <summary>
        /// Verifies the behavior of the Create() method,
        /// accepting a Type to override the constructor's
        /// return type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Create_OverrideReturnType()
        {
            AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> declarer = CreateConstructorDeclarer(null, null);
            ConstructorBuilder constructorBuilder = declarer.Declare(GetType());
        }

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Creates concrete constructor declarer for test purposes.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeConstructor">
        /// The constructor that the declarer will use as a
        /// model for creating new constructors.
        /// </param>
        /// 
        /// <param name="implementation">
        /// The declarer's implementation.
        /// </param>
        internal abstract AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> CreateConstructorDeclarer(
            ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation);

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a given
        /// constructor.
        /// </summary>
        /// 
        /// <param name="expectedConstructor">
        /// The real subject type constructor used as input to the Create()
        /// method.
        /// </param>
        protected void AssertConstructorDeclaredFrom(ConstructorInfo expectedConstructor, Action<ConstructorInfo, ConstructorInfo> assertConstructorAttributes)
        {
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>>();

                List<ConstructorBuilder> implementationArgs = new List<ConstructorBuilder>();

                // Expectations
                // The constructor's parameters are defined.
                implementation.DefineMethodParameters(null, expectedConstructor);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedConstructor))
                    .Do(CreateStoreMethodBuilderDelegate_2Args(implementationArgs));

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> declarer = CreateConstructorDeclarer(expectedConstructor, implementation);
                ConstructorBuilder constructorBuilder = declarer.Declare();
                constructorBuilder.GetILGenerator().Emit(OpCodes.Ret);
                CurrentTypeBuilder.CreateType();

                Assert.That(constructorBuilder.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                Assert.That(implementationArgs.TrueForAll(storedConstructorBuilder => constructorBuilder == storedConstructorBuilder));
                ConstructorInfo constructor = CurrentTypeBuilder.GetConstructors()[0];
                Assert.That(constructor.CallingConvention, Is.EqualTo(CallingConventions.Standard | CallingConventions.HasThis));
                Assert.That(constructor.IsPublic);
                Assert.That(constructor.IsSpecialName);
                Assert.That(constructor.Attributes & MethodAttributes.RTSpecialName, Is.EqualTo(MethodAttributes.RTSpecialName));
                assertConstructorAttributes(constructor, expectedConstructor);
            });
        }

        #endregion

        #region protected data --------------------------------------------------------------------

        protected static readonly MethodAttributes ConstructorAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        #endregion
    }
}
