// ----------------------------------------------------------------------------
// MethodDeclarerFactoryTestFixture.cs
//
// Contains the definition of the MethodDeclarerFactoryTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/29/2008 08:42:45
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class MethodDeclarerFactoryTestFixture
    {
        #region public methods --------------------------------------------------------------------

        #region intialization ---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            ModuleBuilder module = AppDomain.CurrentDomain
                .DefineDynamicAssembly(new AssemblyName("__transientAssembly"), AssemblyBuilderAccess.ReflectionOnly)
                .DefineDynamicModule("__transientModule");
            m_interfaceBuilder = module.DefineType("__transientType_interface");
            m_proxyBuilder = module.DefineType("__transientType_proxy");
        }

        #endregion

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a non-generic interface method declarer.
        /// </summary>
        [Test]
        public void Create_InterfaceMethodDeclarer_NonGeneric()
        {
            MethodInfo expectedMethod = typeof(__GenericTestType<,,>).GetMethod("NoParameters");
            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Interface, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(InterfaceMethodDeclarer)));
            Assert.That(declarer.Builder, Is.SameAs(m_interfaceBuilder));
            Assert.That(declarer.Implementation, Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(declarer.RealSubjectTypeMethod, Is.SameAs(expectedMethod));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a non-generic proxy method declarer.
        /// </summary>
        [Test]
        public void Create_ProxyMethodDeclarer_NonGeneric()
        {
            MethodInfo expectedMethod = typeof(__GenericTestType<,,>).GetMethod("NoParameters");
            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Proxy, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(ProxyMethodDeclarer)));
            Assert.That(declarer.Builder, Is.SameAs(m_proxyBuilder));
            Assert.That(declarer.Implementation, Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(declarer.RealSubjectTypeMethod, Is.SameAs(expectedMethod));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a non-generic constructor declarer.
        /// </summary>
        [Test]
        public void Create_ConstructorDeclarer_NonGeneric()
        {
            ConstructorInfo expectedConstructor = typeof(__GenericTestType<,,>).GetConstructor(Type.EmptyTypes);
            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> declarer = factory.Create(expectedConstructor);

            Assert.That(declarer, Is.InstanceOfType(typeof(NonGenericConstructorDeclarer)));
            Assert.That(declarer.Builder, Is.SameAs(m_proxyBuilder));
            Assert.That(declarer.Implementation, Is.InstanceOfType(typeof(ConstructorDeclarerImpl)));
            Assert.That(declarer.RealSubjectTypeMethod, Is.SameAs(expectedConstructor));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a generic constructor declarer.
        /// </summary>
        [Test]
        public void Create_ConstructorDeclarer_Generic()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            Type[] genericTypeParameters = realSubjectType.GetGenericArguments();
            Type[] constructorParameterTypes = { genericTypeParameters[0], genericTypeParameters[1], typeof(int) };

            ConstructorInfo expectedConstructor = realSubjectType.GetConstructor(constructorParameterTypes);
            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> declarer = factory.Create(expectedConstructor);

            Assert.That(declarer, Is.InstanceOfType(typeof(GenericConstructorDeclarer)));
            Assert.That(declarer.Builder, Is.SameAs(m_proxyBuilder));
            Assert.That(declarer.Implementation, Is.InstanceOfType(typeof(ConstructorDeclarerImpl)));
            Assert.That(declarer.RealSubjectTypeMethod, Is.SameAs(expectedConstructor));
        }
        #endregion

        #region private instance data -------------------------------------------------------------

        private TypeBuilder m_interfaceBuilder;
        private TypeBuilder m_proxyBuilder;

        #endregion
    }
}