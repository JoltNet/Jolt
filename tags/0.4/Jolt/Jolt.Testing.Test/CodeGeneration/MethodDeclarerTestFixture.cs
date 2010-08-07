// ----------------------------------------------------------------------------
// MethodDeclarerTestFixture.cs
//
// Contains the definition of the MethodDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/23/2008 22:38:14
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Functional;
using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class MethodDeclarerTestFixture : AbstractMethodDeclarerTestFixture<MethodBuilder, MethodInfo>
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the implementation of the Create() method,
        /// when creating an interface method.
        /// </summary>
        [Test]
        public void Create_InterfaceMethod()
        {
            AssertMethodDeclared(
                __MethodTestType.InstanceMethod,
                InterfaceMethodAttributes, delegate(MethodBuilder method)
                {
                    Assert.That(method.IsPublic);
                    Assert.That(method.IsVirtual);
                    Assert.That(method.IsAbstract);
                    Assert.That(!method.IsHideBySig);
                    Assert.That(!method.IsSpecialName);
                    Assert.That(method.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
                });
        }

        /// <summary>
        /// Verifies the implementation of the Create() method,
        /// when creating an interface method and overriding the
        /// method's return type.
        /// </summary>
        [Test]
        public void Create_InterfaceMethod_ReturnTypeOverride()
        {
            AssertMethodDeclared(
                __MethodTestType.ManyArgumentsMethod,
                typeof(object),
                InterfaceMethodAttributes, delegate(MethodBuilder method)
                {
                    Assert.That(method.IsPublic);
                    Assert.That(method.IsVirtual);
                    Assert.That(method.IsAbstract);
                    Assert.That(!method.IsHideBySig);
                    Assert.That(!method.IsSpecialName);
                    Assert.That(method.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
                });
        }

        /// <summary>
        /// Verifies the implementation of the Create() method,
        /// when creating a proxy method.
        /// </summary>
        [Test]
        public void Create_ProxyMethod()
        {
            AssertMethodDeclared(
                __MethodTestType.InstanceMethod,
                ProxyMethodAttributes, delegate(MethodBuilder method)
                {
                    Assert.That(method.IsPublic);
                    Assert.That(method.IsVirtual);
                    Assert.That(!method.IsStatic);
                    Assert.That(method.IsFinal);
                    Assert.That(!method.IsHideBySig);
                    Assert.That(!method.IsSpecialName);
                });
        }

        /// <summary>
        /// Verifies the implementation of the Create() method,
        /// when creating a proxy method and overriding the
        /// method's return type.
        /// </summary>
        [Test]
        public void Create_ProxyMethod_ReturnTypeOverride()
        {
            AssertMethodDeclared(
                __MethodTestType.ManyArgumentsMethod,
                typeof(object),
                ProxyMethodAttributes, delegate(MethodBuilder method)
                {
                    Assert.That(method.IsPublic);
                    Assert.That(method.IsVirtual);
                    Assert.That(!method.IsStatic);
                    Assert.That(method.IsFinal);
                    Assert.That(!method.IsHideBySig);
                    Assert.That(!method.IsSpecialName);
                });
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Asserts the expected behavior of the MethodBuilder.Declare() method.
        /// </summary>
        /// 
        /// <param name="expectedMethod">
        /// The real subject type method used to create a new method.
        /// </param>
        /// 
        /// <param name="methodDeclarerAttributes">
        /// The attributes of the newly created method.
        /// </param>
        /// 
        /// <param name="assertMethodBuilderAttributes">
        /// A delegate that defines a set of custom assertions on the attributes
        /// of the given method builder.
        /// </param>
        private void AssertMethodDeclared(
            MethodInfo expectedMethod,
            MethodAttributes methodDeclarerAttributes,
            Action<MethodBuilder> assertMethodBuilderAttributes)
        {
            AssertMethodDeclared(
                expectedMethod,
                expectedMethod.ReturnType,
                methodDeclarerAttributes,
                (declarer, returnType) => declarer.Declare(),
                assertMethodBuilderAttributes);
        }

        /// <summary>
        /// Asserts the expected behavior of the MethodBuilder.Declare() method.
        /// </summary>
        /// 
        /// <param name="expectedMethod">
        /// The real subject type method used to create a new method.
        /// </param>
        /// 
        /// <param name="expectedMethodReturnType">
        /// The return type of the real subject type method, or an override
        /// for the new method's return type.
        /// </param>
        /// 
        /// <param name="methodDeclarerAttributes">
        /// The attributes of the newly created method.
        /// </param>
        /// 
        /// <param name="assertMethodBuilderAttributes">
        /// A delegate that defines a set of custom assertions on the attributes
        /// of the given method builder.
        /// </param>
        private void AssertMethodDeclared(
            MethodInfo expectedMethod,
            Type expectedMethodReturnType,
            MethodAttributes methodDeclarerAttributes,
            Action<MethodBuilder> assertMethodBuilderAttributes)
        {
            AssertMethodDeclared(
                expectedMethod,
                expectedMethodReturnType,
                methodDeclarerAttributes,
                (declarer, returnType) => declarer.Declare(returnType),
                assertMethodBuilderAttributes);
        }

        /// <summary>
        /// Asserts the expected behavior of the MethodBuilder.Declare() method.
        /// </summary>
        /// 
        /// <param name="expectedMethod">
        /// The real subject type method used to create a new method.
        /// </param>
        /// 
        /// <param name="expectedMethodReturnType">
        /// The return type of the real subject type method, or an override
        /// for the new method's return type.
        /// </param>
        /// 
        /// <param name="methodDeclarerAttributes">
        /// The attributes of the newly created method.
        /// </param>
        /// 
        /// <param name="declareMethodBuilder">
        /// A delegate that invokes a Declare() method overload from the given
        /// MethodDeclarer, optionally using the given method return type overload.
        /// </param>
        /// 
        /// <param name="assertMethodBuilderAttributes">
        /// A delegate that defines a set of custom assertions on the attributes
        /// of the given method builder.
        /// </param>
        private void AssertMethodDeclared(
            MethodInfo expectedMethod,
            Type expectedMethodReturnType,
            MethodAttributes methodDeclarerAttributes,
            Func<MethodDeclarer, Type, MethodBuilder> declareMethodBuilder,
            Action<MethodBuilder> assertMethodBuilderAttributes)
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation =
                MockRepository.GenerateMock<IMethodDeclarerImpl<MethodBuilder, MethodInfo>>();

            List<MethodBuilder> implementationArgs = new List<MethodBuilder>();
            Action<MethodBuilder, MethodInfo, Type> storeMethodBuilderForMethods = CreateStoreMethodBuilderDelegate_3Args(implementationArgs);
            Action<MethodBuilder, MethodInfo> storeMethodBuilderForParameters = Bind.Third(storeMethodBuilderForMethods, GetType());

            implementation.Expect(i => i.DeclareMethod(Arg<MethodBuilder>.Is.Anything, Arg<MethodInfo>.Is.Same(expectedMethod), Arg<Type>.Is.Same(expectedMethodReturnType)))
                .Do(storeMethodBuilderForMethods);

            implementation.Expect(i => i.DefineMethodParameters(Arg<MethodBuilder>.Is.Anything, Arg<MethodInfo>.Is.Same(expectedMethod)))
                .Do(storeMethodBuilderForParameters);

            MethodDeclarer declarer = new MethodDeclarer(CurrentTypeBuilder, methodDeclarerAttributes, expectedMethod, implementation);
            MethodBuilder method = declareMethodBuilder(declarer, expectedMethodReturnType);

            Assert.That(method.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
            Assert.That(method.Name, Is.EqualTo(expectedMethod.Name));
            Assert.That(method.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
            Assert.That(implementationArgs.TrueForAll(storedMethod => method == storedMethod));

            assertMethodBuilderAttributes(method);

            implementation.VerifyAllExpectations();
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly MethodAttributes InterfaceMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract;
        private static readonly MethodAttributes ProxyMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;

        #endregion
    }
}
    