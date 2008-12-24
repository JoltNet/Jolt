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
            MethodInfo expectedMethod = typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes);

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Interface, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_interfaceBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a non-generic proxy method declarer.
        /// </summary>
        [Test]
        public void Create_ProxyMethodDeclarer_NonGeneric()
        {
            MethodInfo expectedMethod = typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes);

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Proxy, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_proxyBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a generic interface method declarer from a generic method.
        /// </summary>
        [Test]
        public void Create_InterfaceMethodDeclarer_Generic()
        {
            MethodInfo expectedMethod = typeof(__MethodTestType).GetMethod("GenericMethod");

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Interface, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_interfaceBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(GenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract));
        }
        
        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a generic interface method declarer from a generic type.
        /// </summary>
        [Test]
        public void Create_InterfaceMethodDeclarer_Generic_FromType()
        {
            MethodInfo expectedMethod = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction");

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Interface, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_interfaceBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract));
        }
        
        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a generic proxy method declarer from a generic method.
        /// </summary>
        [Test]
        public void Create_ProxyMethodDeclarer_Generic()
        {
            MethodInfo expectedMethod = typeof(__MethodTestType).GetMethod("GenericMethod");

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Proxy, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_proxyBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(GenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final));
        }

        /// <summary>
        /// Verifies the behavior of the Create() method for constructing
        /// a generic proxy method declarer from a generic type.
        /// </summary>
        [Test]
        public void Create_ProxyMethodDeclarer_Generic_FromType()
        {
            MethodInfo expectedMethod = typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction");

            MethodDeclarerFactory factory = new MethodDeclarerFactory(m_interfaceBuilder, m_proxyBuilder);
            AbstractMethodDeclarer<MethodBuilder, MethodInfo> declarer = factory.Create(MethodDeclarerTypes.Proxy, expectedMethod);

            Assert.That(declarer, Is.InstanceOfType(typeof(MethodDeclarer)));
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_proxyBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(NonGenericMethodDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedMethod));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final));
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
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_proxyBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(ConstructorDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedConstructor));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName));
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
            Assert.That(GetPropertyValue(declarer, "Builder"), Is.SameAs(m_proxyBuilder));
            Assert.That(GetPropertyValue(declarer, "Implementation"), Is.InstanceOfType(typeof(ConstructorDeclarerImpl)));
            Assert.That(GetPropertyValue(declarer, "RealSubjectTypeMethod"), Is.SameAs(expectedConstructor));
            Assert.That(GetPropertyValue(declarer, "MethodAttributes"), Is.EqualTo(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Obtains the value of a given non-static, non-public property, from
        /// a given method declarer.
        /// </summary>
        /// 
        /// <param name="declarer">
        /// The declarer to query.
        /// </param>
        /// 
        /// <param name="propertyName">
        /// The name of the property to invoke.
        /// </param>
        private static object GetPropertyValue<TMethodBuilder, TMethod>(AbstractMethodDeclarer<TMethodBuilder, TMethod> declarer, string propertyName)
            where TMethodBuilder : TMethod
            where TMethod : MethodBase
        {
            return declarer.GetType()
                .GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(declarer, null);
        }

        #endregion

        #region private instance data -------------------------------------------------------------

        private TypeBuilder m_interfaceBuilder;
        private TypeBuilder m_proxyBuilder;

        #endregion
    }
}