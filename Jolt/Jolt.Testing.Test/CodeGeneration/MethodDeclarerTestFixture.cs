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
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using RMC = Rhino.Mocks.Constraints;

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
                typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes),
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
                typeof(__MethodTestType).GetMethod("ManyArgumentsMethod"),
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
                typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes),
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
                typeof(__MethodTestType).GetMethod("ManyArgumentsMethod"),
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

        #region private instance methods ----------------------------------------------------------

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
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<MethodBuilder, MethodInfo>>();

                List<MethodBuilder> implementationArgs = new List<MethodBuilder>();
                Action<MethodBuilder, MethodInfo, Type> storeMethodBuilderForMethods = CreateStoreMethodBuilderDelegate_3Args(implementationArgs);
                Action<MethodBuilder, MethodInfo> storeMethodBuilderForParameters = Bind.Third(storeMethodBuilderForMethods, GetType());

                // Expectations
                // The method and its parameters are defined/declared.
                implementation.DeclareMethod(null, expectedMethod, expectedMethodReturnType);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod), RMC.Is.Same(expectedMethodReturnType))
                    .Do(storeMethodBuilderForMethods);

                implementation.DefineMethodParameters(null, expectedMethod);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod))
                    .Do(storeMethodBuilderForParameters);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                MethodDeclarer declarer = new MethodDeclarer(CurrentTypeBuilder, methodDeclarerAttributes, expectedMethod, implementation);
                MethodBuilder method = declareMethodBuilder(declarer, expectedMethodReturnType);

                Assert.That(method.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                Assert.That(method.Name, Is.EqualTo(expectedMethod.Name));
                Assert.That(method.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
                Assert.That(implementationArgs.TrueForAll(storedMethod => method == storedMethod));

                assertMethodBuilderAttributes(method);
            });
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly MethodAttributes InterfaceMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract;
        private static readonly MethodAttributes ProxyMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;

        #endregion
    }
}
    