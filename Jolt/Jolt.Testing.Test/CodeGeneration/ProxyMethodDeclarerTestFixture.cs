// ----------------------------------------------------------------------------
// InterfaceMethodDeclarerTestFixture.cs
//
// Contains the definition of the InterfaceMethodDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/23/2008 17:49:30
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
    public sealed class ProxyMethodDeclarerTestFixture : AbstractMethodDeclarerTestFixture<MethodBuilder, MethodInfo>
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the implementation of the Create() method.
        /// </summary>
        [Test]
        public void Create()
        {
            With.Mocks(delegate
            {
                IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = Mocker.Current.CreateMock<IMethodDeclarerImpl<MethodBuilder, MethodInfo>>();

                List<MethodBuilder> implementationArgs = new List<MethodBuilder>();
                Delegate storeMethodBuilderParameter = CreateDeclareMethodsAttributeDelegate(implementationArgs);

                // Expectations
                // The method and its parameters are defined/declared.
                MethodInfo expectedMethod = typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes);

                implementation.DeclareMethod(null, expectedMethod);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod))
                    .Do(storeMethodBuilderParameter);

                implementation.DefineMethodParameters(null, expectedMethod);
                LastCall.Constraints(RMC.Is.Anything(), RMC.Is.Same(expectedMethod))
                    .Do(storeMethodBuilderParameter);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                ProxyMethodDeclarer declarer = new ProxyMethodDeclarer(CurrentTypeBuilder, expectedMethod, implementation);
                MethodBuilder proxyMethod = declarer.Declare();

                Assert.That(proxyMethod.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                Assert.That(proxyMethod.Name, Is.EqualTo(expectedMethod.Name));
                Assert.That(proxyMethod.IsPublic);
                Assert.That(proxyMethod.IsVirtual);
                Assert.That(!proxyMethod.IsStatic);
                Assert.That(proxyMethod.IsFinal);
                Assert.That(!proxyMethod.IsHideBySig);
                Assert.That(!proxyMethod.IsSpecialName);
                Assert.That(proxyMethod.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
                Assert.That(implementationArgs.TrueForAll(delegate(MethodBuilder method)
                {
                    // The interface method created by the type builder is passed to each implementation
                    // function call.
                    return proxyMethod == method;
                }));
            });
        }

        #endregion
    }
}
