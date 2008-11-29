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
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<MethodBuilder, MethodInfo>>();

                List<MethodBuilder> implementationArgs = new List<MethodBuilder>();
                Delegate storeMethodBuilderParameter = CreateDeclareMethodsAttributeDelegate(implementationArgs);

                // Expectations
                // The method and its parameters are defined/declared.
                implementation.DeclareMethod(null, expectedMethod);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod))
                    .Do(storeMethodBuilderParameter);

                implementation.DefineMethodParameters(null, expectedMethod);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod))
                    .Do(storeMethodBuilderParameter);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                MethodDeclarer declarer = new MethodDeclarer(CurrentTypeBuilder, methodDeclarerAttributes, expectedMethod, implementation);
                MethodBuilder method = declarer.Declare();

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
    