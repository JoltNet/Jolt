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
        #region internal instance methods ---------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a given
        /// constructor.
        /// </summary>
        /// 
        /// <param name="expectedConstructor">
        /// The real subject type constructor used as input to the Create()
        /// method.
        /// </param>
        internal void AssertConstructorDeclaredFrom<TDeclarer>(ConstructorInfo expectedConstructor, Action<ConstructorInfo, ConstructorInfo> assertConstructorAttributes)
            where TDeclarer : AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo>
        {
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>>();

                List<ConstructorBuilder> implementationArgs = new List<ConstructorBuilder>();
                Delegate storeConstructorBuilderParameter = CreateDeclareMethodsAttributeDelegate(implementationArgs);

                // Expectations
                // The constructor's parameters are defined.
                implementation.DefineMethodParameters(null, expectedConstructor);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedConstructor))
                    .Do(storeConstructorBuilderParameter);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                TDeclarer declarer = (TDeclarer)Activator.CreateInstance(
                    typeof(TDeclarer), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null,
                    new object[] {
                        CurrentTypeBuilder,
                        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                        expectedConstructor,
                        implementation },
                    null);
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
    }
}
