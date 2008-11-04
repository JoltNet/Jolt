// ----------------------------------------------------------------------------
// NonGenericConstructorDeclarerTestFixture.cs
//
// Contains the definition of the NonGenericConstructorDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 19:12:33
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using JTCG = Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using RMC = Rhino.Mocks.Constraints;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class NonGenericConstructorDeclarerTestFixture : AbstractMethodDeclarerTestFixture<ConstructorBuilder, ConstructorInfo>
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with no parameters.
        /// </summary>
        [Test]
        public void Create_NoParameters()
        {
            AssertConstructorDeclaredFrom(typeof(__ConstructorTestType).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with one parameter.
        /// </summary>
        [Test]
        public void Create_OneParameter()
        {
            AssertConstructorDeclaredFrom(typeof(__ConstructorTestType).GetConstructor(new Type[] { typeof(int) }));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameter()
        {
            AssertConstructorDeclaredFrom(typeof(__ConstructorTestType).GetConstructor(new Type[] { typeof(int), typeof(int) }));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a given
        /// constructor.
        /// </summary>
        /// 
        /// <param name="expectedConstructor">
        /// The real subject type constructor used as input to the Create()
        /// method.
        /// </param>
        private void AssertConstructorDeclaredFrom(ConstructorInfo expectedConstructor)
        {
            // TODO: Move this method to base class.
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

                NonGenericConstructorDeclarer declarer = new NonGenericConstructorDeclarer(CurrentTypeBuilder, expectedConstructor, implementation);
                ConstructorBuilder constructorBuilder = declarer.Declare();
                constructorBuilder.GetILGenerator().Emit(OpCodes.Ret);
                CurrentTypeBuilder.CreateType();
                Assert.That(constructorBuilder.DeclaringType, Is.EqualTo(CurrentTypeBuilder));

                ConstructorInfo constructor = CurrentTypeBuilder.GetConstructors()[0];
                Assert.That(JTCG.Convert.ToParameterTypes(constructor.GetParameters()),
                    Is.EqualTo(JTCG.Convert.ToParameterTypes(expectedConstructor.GetParameters())));
                Assert.That(constructor.CallingConvention, Is.EqualTo(CallingConventions.Standard | CallingConventions.HasThis));
                Assert.That(constructor.IsPublic);
                Assert.That(constructor.IsSpecialName);
                Assert.That(constructor.Attributes & MethodAttributes.RTSpecialName, Is.EqualTo(MethodAttributes.RTSpecialName));
                Assert.That(implementationArgs.TrueForAll(delegate(ConstructorBuilder storedConstructorBuilder)
                {
                    // The constructor created by the type builder is passed to each implementation
                    // function call.
                    return constructorBuilder == storedConstructorBuilder;
                }));
            });
        }

        #endregion
    }
}
