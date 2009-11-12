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
    /// Enumerates the AbstractMethodDeclarer types specialized
    /// for creating methods.
    /// </summary>
    internal enum MethodDeclarerTypes
    {
        Interface,
        Proxy
    }


    /// <summary>
    /// The MethodDeclarerFactory class provides methods for constructing
    /// concrete instances of the AbstractMethodDeclarer family of types.
    /// </summary>
    internal sealed class MethodDeclarerFactory
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the factory with the TypeBuilder instances required for
        /// creating AbstractMethodDeclarer types.
        /// </summary>
        /// 
        /// <param name="interfaceBuilder">
        /// The type builder that represents the proxy's interface.
        /// </param>
        /// 
        /// <param name="proxyBuilder">
        /// The type builder that represents the interface to the proxy.
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
        /// Creates a method declarer for a given type
        /// </summary>
        /// 
        /// <param name="typeId">
        /// The type of method declarer to create.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The method used by the created method declarer.
        /// </param>
        internal AbstractMethodDeclarer<MethodBuilder, MethodInfo> Create(MethodDeclarerTypes typeId, MethodInfo realSubjectTypeMethod)
        {
            return m_methodDeclarerFactoryMethods[typeId](realSubjectTypeMethod);
        }

        /// <summary>
        /// Creates a constructor declarer for the proxy type.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeConstructor">
        /// The constructor used by the created constructor declarer.
        /// </param>
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
