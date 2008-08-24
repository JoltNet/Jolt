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
        /// Verifies the behavior of the Create() method when the
        /// real subject type constructor is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Create_InvalidType()
        {
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>>();
                Mocker.Current.ReplayAll();

                Type[] genericTypes = typeof(__GenericTestType<,,>).GetGenericArguments();
                ConstructorInfo invalidConstructor = typeof(__GenericTestType<,,>).GetConstructor(new Type[] { genericTypes[1], genericTypes[2], typeof(int) });
                NonGenericConstructorDeclarer declarer = new NonGenericConstructorDeclarer(CurrentTypeBuilder, invalidConstructor, implementation);
            });
        }

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
                ConstructorBuilder constructor = declarer.Declare();
                constructor.GetILGenerator().Emit(OpCodes.Ret);
                CurrentTypeBuilder.CreateType();

                Assert.That(constructor.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                Assert.That(JTCG.Convert.ToParameterTypes(constructor.GetParameters()),
                    Is.EqualTo(JTCG.Convert.ToParameterTypes(expectedConstructor.GetParameters())));
                Assert.That(constructor.CallingConvention, Is.EqualTo(CallingConventions.Standard));
                Assert.That(constructor.IsPublic);
                Assert.That(constructor.IsSpecialName);
                Assert.That(constructor.Attributes & MethodAttributes.RTSpecialName, Is.EqualTo(MethodAttributes.RTSpecialName));
                Assert.That(implementationArgs.TrueForAll(delegate(ConstructorBuilder storedConstructor)
                {
                    // The constructor created by the type builder is passed to each implementation
                    // function call.
                    return constructor == storedConstructor;
                }));
            });
        }

        #endregion
    }
}
