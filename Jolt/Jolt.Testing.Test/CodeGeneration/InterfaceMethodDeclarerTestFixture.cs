// ----------------------------------------------------------------------------
// InterfaceMethodDeclarerTestFixture.cs
//
// Contains the definition of the InterfaceMethodDeclarerTestFixture class.
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
    public sealed class InterfaceMethodDeclarerTestFixture : AbstractMethodDeclarerTestFixture<MethodBuilder, MethodInfo>
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

                InterfaceMethodDeclarer declarer = new InterfaceMethodDeclarer(CurrentTypeBuilder, expectedMethod, implementation);
                MethodBuilder interfaceMethod = declarer.Declare();

                Assert.That(interfaceMethod.DeclaringType, Is.EqualTo(CurrentTypeBuilder));
                Assert.That(interfaceMethod.Name, Is.EqualTo(expectedMethod.Name));
                Assert.That(interfaceMethod.IsPublic);
                Assert.That(interfaceMethod.IsVirtual);
                Assert.That(interfaceMethod.IsAbstract);
                Assert.That(!interfaceMethod.IsHideBySig);
                Assert.That(!interfaceMethod.IsSpecialName);
                Assert.That(interfaceMethod.Attributes & MethodAttributes.NewSlot, Is.Not.EqualTo(MethodAttributes.NewSlot));
                Assert.That(implementationArgs.TrueForAll(delegate(MethodBuilder method)
                {
                    // The interface method created by the type builder is passed to each implementation
                    // function call.
                    return interfaceMethod == method;
                }));
            });
        }

        #endregion
    }
}
