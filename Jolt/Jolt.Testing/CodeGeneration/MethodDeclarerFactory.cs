// ----------------------------------------------------------------------------
// MethodDeclarerFactory.cs
//
// Contains the definition of the MethodDeclarerFactory class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/28/2008 21:01:47
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    // Represents a factory method for creating an AbstractMethodDeclarer
    // that is specialized for methods.
    using CreateMethodDeclarerDelegate = Func<MethodInfo, AbstractMethodDeclarer<MethodBuilder, MethodInfo>>;


    /// <summary>
    /// Enumerates the concrete <see cref="AbstractMethodDeclarer"/> types.
    /// </summary>
    internal enum MethodDeclarerTypes
    {
        /// <summary>
        /// Denotes the interface method declarer type.
        /// </summary>
        Interface,

        /// <summary>
        /// Denotes the proxy method declarer type.
        /// </summary>
        Proxy
    }


    /// <summary>
    /// Defines a factory class for constructing concrete instances of
    /// the <see cref="AbstractMethodDeclarer"/> family of types.
    /// </summary>
    internal sealed class MethodDeclarerFactory
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="MethodDeclarerFactory"/> class,
        /// intializing the factory with the factory-method required
        /// <see cref="System.Reflection.Emit.TypeBuilder"/> instances.
        /// </summary>
        /// 
        /// <param name="interfaceBuilder">
        /// The <see cref="System.Reflection.Emit.TypeBuilder"/> that represents the proxy interface type.
        /// </param>
        /// 
        /// <param name="proxyBuilder">
        /// The <see cref="System.Reflection.Emit.TypeBuilder"/> that represents the the proxy type.
        /// </param>
        internal MethodDeclarerFactory(TypeBuilder interfaceBuilder, TypeBuilder proxyBuilder)
        {
            m_interface = interfaceBuilder;
            m_proxy = proxyBuilder;

            // Initialize factory method registry.
            m_methodDeclarerFactoryMethods = new Dictionary<MethodDeclarerTypes, CreateMethodDeclarerDelegate>();

            m_methodDeclarerFactoryMethods.Add(MethodDeclarerTypes.Interface, realSubjectTypeMethod => realSubjectTypeMethod.IsGenericMethod ?
                new MethodDeclarer(m_interface, InterfaceMethodAttributes, realSubjectTypeMethod, new GenericMethodDeclarerImpl()) :
                new MethodDeclarer(m_interface, InterfaceMethodAttributes, realSubjectTypeMethod, new NonGenericMethodDeclarerImpl()));
            
            m_methodDeclarerFactoryMethods.Add(MethodDeclarerTypes.Proxy, realSubjectTypeMethod => realSubjectTypeMethod.IsGenericMethod ?
                new MethodDeclarer(m_proxy, ProxyMethodAttributes, realSubjectTypeMethod, new GenericMethodDeclarerImpl()) :
                new MethodDeclarer(m_proxy, ProxyMethodAttributes, realSubjectTypeMethod, new NonGenericMethodDeclarerImpl()));
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Creates new instance of an <see cref="AbstractMethodDeclarer"/> class,
        /// corresponding to a given <see cref="MethodDeclarerTypes"/> value.
        /// </summary>
        /// 
        /// <param name="typeId">
        /// The <see cref="MethodDeclarerTypes"/> value denoting the concrete type to create.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The <see cref="System.Reflection.MethodInfo"/> object used to initialize
        /// the method declarer.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="AbstractMethodDeclarer"/>, specialized to create methods.
        /// </returns>
        internal AbstractMethodDeclarer<MethodBuilder, MethodInfo> Create(MethodDeclarerTypes typeId, MethodInfo realSubjectTypeMethod)
        {
            return m_methodDeclarerFactoryMethods[typeId](realSubjectTypeMethod);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="AbstractMethodDeclarer"/> class,
        /// specialized to create constructors for a proxy type.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeConstructor">
        /// The <see cref="System.Reflection.ConstructorInfo"/> object used to initialize
        /// the constructor declarer.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="AbstractMethodDeclarer"/>, specialized to create constructors.
        /// </returns>
        internal AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo> Create(ConstructorInfo realSubjectTypeConstructor)
        {
            if (DeclarationHelper.ContainsGenericParameters(realSubjectTypeConstructor.GetParameters()))
            {
                return new GenericConstructorDeclarer(m_proxy, ConstructorAttributes, realSubjectTypeConstructor, new ConstructorDeclarerImpl());
            }

            return new NonGenericConstructorDeclarer(m_proxy, ConstructorAttributes, realSubjectTypeConstructor, new ConstructorDeclarerImpl());
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly TypeBuilder m_interface;
        private readonly TypeBuilder m_proxy;
        private readonly IDictionary<MethodDeclarerTypes, CreateMethodDeclarerDelegate> m_methodDeclarerFactoryMethods;

        private static readonly MethodAttributes InterfaceMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract;
        private static readonly MethodAttributes ProxyMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
        private static readonly MethodAttributes ConstructorAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        #endregion
    }
}
