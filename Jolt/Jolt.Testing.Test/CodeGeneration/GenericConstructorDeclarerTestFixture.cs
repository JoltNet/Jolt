// ----------------------------------------------------------------------------
// GenericConstructorDeclarerTestFixture.cs
//
// Contains the definition of the GenericConstructorDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 9/3/2008 09:54:10
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using JTCG = Jolt.Testing.CodeGeneration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using RMC = Rhino.Mocks.Constraints;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public class GenericConstructorDeclarerTestFixture : AbstractMethodDeclarerTestFixture<ConstructorBuilder, ConstructorInfo>
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with no parameters.
        /// </summary>
        [Test]
        public void Create_NoParameters()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            InitializeCurrentTypeBuilder(realSubjectType);
            AssertConstructorDeclaredFrom(realSubjectType.GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with one parameter.
        /// </summary>
        [Test]
        public void Create_OneParameter()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            InitializeCurrentTypeBuilder(realSubjectType);

            Type[] constructorParameterTypes = { realSubjectType.GetGenericArguments()[0] };
            AssertConstructorDeclaredFrom(realSubjectType.GetConstructor(constructorParameterTypes));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one generic parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameters()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            InitializeCurrentTypeBuilder(realSubjectType);

            Type[] genericTypeParameters = realSubjectType.GetGenericArguments();
            Type[] constructorParameterTypes = { genericTypeParameters[0], genericTypeParameters[1] };
            AssertConstructorDeclaredFrom(realSubjectType.GetConstructor(constructorParameterTypes));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for a constructor
        /// with more than one parameter.
        /// </summary>
        [Test]
        public void Create_ManyParameters_Mixed()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            InitializeCurrentTypeBuilder(realSubjectType);

            Type[] genericTypeParameters = realSubjectType.GetGenericArguments();
            Type[] constructorParameterTypes = { genericTypeParameters[0], genericTypeParameters[1], typeof(int) };
            AssertConstructorDeclaredFrom(realSubjectType.GetConstructor(constructorParameterTypes));
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
            CurrentTypeBuilder.DefineGenericParameters(JTCG.Convert.ToTypeNames(genericType.GetGenericArguments()));
        }

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

                GenericConstructorDeclarer declarer = new GenericConstructorDeclarer(CurrentTypeBuilder, expectedConstructor, implementation);
                ConstructorBuilder constructorBuilder = declarer.Declare();
                constructorBuilder.GetILGenerator().Emit(OpCodes.Ret);
                CurrentTypeBuilder.CreateType();
                Assert.That(constructorBuilder.DeclaringType, Is.EqualTo(CurrentTypeBuilder));

                ConstructorInfo constructor = CurrentTypeBuilder.GetConstructors()[0];
                Type[] constructorParameterTypes = JTCG.Convert.ToParameterTypes(constructor.GetParameters());
                Type[] expectedConstructorParameterTypes = JTCG.Convert.ToParameterTypes(expectedConstructor.GetParameters());
                Assert.That(constructorParameterTypes.Length, Is.EqualTo(expectedConstructorParameterTypes.Length));

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
