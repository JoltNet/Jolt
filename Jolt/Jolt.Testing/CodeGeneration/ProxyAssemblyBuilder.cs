// ----------------------------------------------------------------------------
// ProxyAssemblyBuilder.cs
//
// Contains the definition of the ProxyAssemblyBuilder class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/31/2007 14:09:03
// ----------------------------------------------------------------------------

using System;
using System.Configuration;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;

using Jolt.Testing.Properties;
using log4net;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Represents a factory method that constructs a ProxyTypeBuilder
    /// class using the constructor overload matching the delegate
    /// paramter signature.
    /// </summary>
    /// 
    /// <see cref="ProxyTypeBuilder.ProxyTypeBuilder(string, Type, ModuleBuilder)"/>
    /// 
    /// <remarks>
    /// Internal type to support testing.
    /// </remarks>
    internal delegate IProxyTypeBuilder CreateProxyTypeBuilderDelegate(string sRootNamespace, Type realSubjectType, ModuleBuilder module);


    /// <summary>
    /// Provides methods to create an assembly and reverse engineer proxy types
    /// in a given namespace.
    /// 
    /// The default root namespace is "Jolt.Testing.Generated".
    /// 
    /// <seealso cref="ProxyTypeBuilder"/>
    /// </summary>
    public class ProxyAssemblyBuilder
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the assembly builder, using the working directory as the
        /// assembly location and a default assembly namespace and filename.
        /// </summary>
        public ProxyAssemblyBuilder() : this(DefaultNamespace) { }

        /// <summary>
        /// Initializes the assembly builder, using the working directory as the
        /// assembly location, and a default assembly filename.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The assembly's root namespace.
        /// </param>
        public ProxyAssemblyBuilder(string sRootNamespace)
            : this(sRootNamespace, Path.Combine(Environment.CurrentDirectory, DefaultAssemblyFilename)) { }

        /// <summary>
        /// Initializes the assembly builder.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="sAssemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        public ProxyAssemblyBuilder(string sRootNamespace, string sAssemblyFullPath)
            : this (sRootNamespace, sAssemblyFullPath, ConfigurationManager.GetSection("proxyBuilderSettings") as ProxyAssemblyBuilderSettings) { }

        /// <summary>
        /// Initializes the assembly builder.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="sAssemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        /// 
        /// <param name="settings">
        /// The configuration settings for the class.
        /// </param>
        public ProxyAssemblyBuilder(string sRootNamespace, string sAssemblyFullPath, ProxyAssemblyBuilderSettings settings)
            : this(sRootNamespace, sAssemblyFullPath, settings, delegate(string ns, Type t, ModuleBuilder mb)
                   { return new ProxyTypeBuilder(ns, t, mb); }) { }

        /// <summary>
        /// Initializes the assembly builder, overriding the default ProxyTypeBuilder
        /// factory method.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="sAssemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        /// 
        /// <param name="settings">
        /// The configuration settings for the class.
        /// </param>
        /// 
        /// <param name="createTypeBuilder">
        /// The factory method to use for creating a ProxyTypeBuilder object.
        /// </param>
        internal ProxyAssemblyBuilder(string sRootNamespace, string sAssemblyFullPath,
            ProxyAssemblyBuilderSettings settings, CreateProxyTypeBuilderDelegate createTypeBuilder)
        {
            m_sRootNamespace = sRootNamespace;
            m_sAssemblyFullPath = sAssemblyFullPath;
            m_createProxyTypeBuilder = createTypeBuilder;
            m_settings = settings ?? ProxyAssemblyBuilderSettings.Default;

            string sAssemblyFileName = Path.GetFileName(m_sAssemblyFullPath);
            m_assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(String.Concat(sAssemblyFileName, ", Version=1.0.0.0, Culture=en-US, PublicKeyToken=null")),
                AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(m_sAssemblyFullPath));
            m_module = m_assembly.DefineDynamicModule(Path.GetFileNameWithoutExtension(DefaultAssemblyFilename), sAssemblyFileName, true);

            m_methodBindingFlags = ComputeMemberBindingFlags(m_settings.EmitMethods, m_settings.EmitStatics);
            m_propertyBindingFlags = ComputeMemberBindingFlags(m_settings.EmitProperties, m_settings.EmitStatics);
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the root namespace of the assembly.
        /// </summary>
        public string RootNamespace
        {
            get { return m_sRootNamespace; }
        }

        /// <summary>
        /// Gets the full path of the proxy assembly.
        /// </summary>
        public string AssemblyFullPath
        {
            get { return m_sAssemblyFullPath; }
        }

        /// <summary>
        /// Gets the configuration settings associated with this object.
        /// </summary>
        public ProxyAssemblyBuilderSettings Settings
        {
            get { return m_settings; }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a type to the assembly builder, generating an interface and proxy
        /// containing all of its methods, properties and events.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The type for which a proxy and interface are created.
        /// </param>
        public void AddType(Type realSubjectType)
        {
            IProxyTypeBuilder builder = m_createProxyTypeBuilder(m_sRootNamespace, realSubjectType, m_module);
            Array.ForEach(realSubjectType.GetProperties(m_propertyBindingFlags), builder.AddProperty);
            //Array.ForEach(realSubjectType.GetEvents(m_eventBindingFlags), builder.AddEvent);
            Array.ForEach(realSubjectType.GetMethods(m_methodBindingFlags), delegate(MethodInfo m) { AddMethod(m, builder); });

            builder.CreateProxy();
        }

        /// <summary>
        /// Saves the state of the assembly builder to disk in the form
        /// of an assembly.
        /// </summary>
        public Assembly CreateAssembly()
        {
            m_assembly.Save(Path.GetFileName(m_sAssemblyFullPath));
            return m_assembly;
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the current state of the assembly builder.
        /// </summary>
        internal AssemblyBuilder Assembly
        {
            get { return m_assembly; }
        }

        /// <summary>
        /// Gets the current state of the module builder.
        /// </summary>
        internal ModuleBuilder Module
        {
            get { return m_module; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Determines if a given method is defined as part of a property
        /// or event on its declaring type.  Uses the property and event
        /// binding member declarations for lookup.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to validate.
        /// </param>
        private bool IsSpecialMethod(MethodInfo method)
        {
            foreach (string sPrefix in PropertyMethodPrefixes)
            {
                if (method.Name.IndexOf(sPrefix) == 0 &&
                    method.DeclaringType.GetMember(method.Name.Substring(sPrefix.Length), m_propertyBindingFlags) != null)
                {
                    return true;
                }
            }

            // check add_sMethodName, event binding
            // check remove_sMethodName, event binding.

            return false;
        }

        /// <summary>
        /// Adds the given method to the given builder, only if it isn't already part of
        /// another member (property, event) on the proxy class.
        /// </summary>
        /// 
        /// <param name="method">
        /// The member to add to the builder.
        /// </param>
        /// 
        /// <param name="builder">
        /// The builder that accepts the method.
        /// </param>
        private void AddMethod(MethodInfo method, IProxyTypeBuilder builder)
        {
            if (IsSpecialMethod(method))
            {
                // Output a log message stating the method is skipped only
                // when property/event emit is enabled.
                if (m_propertyBindingFlags != BindingFlags.Default)
                {
                    Log.InfoFormat(Resources.Info_SkipDefinedMethod, method.Name);
                }
            }
            else
            {
                try
                {
                    builder.AddMethod(method);
                }
                catch (InvalidOperationException ex)
                {
                    Log.Warn(ex.Message);
                }
            }
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Determines the binding flags for a particular member given a Boolean
        /// representation of the flag values.
        /// </summary>
        /// 
        /// <param name="emitMember">
        /// Denotes if the member type is emitted.
        /// </param>
        /// 
        /// <param name="emitStatics">
        /// Denotes if static types are remitted.
        /// </param>
        private static BindingFlags ComputeMemberBindingFlags(bool emitMember, bool emitStatics)
        {
            BindingFlags result = BindingFlags.Default;
            if (emitMember)
            {
                result = BindingFlags.Public | BindingFlags.Instance;
                if (emitStatics)
                {
                    result |= BindingFlags.Static;
                }
            }

            return result;
        }

        #endregion

        #region private instance fields -----------------------------------------------------------

        private readonly string m_sRootNamespace;
        private readonly AssemblyBuilder m_assembly;
        private readonly ModuleBuilder m_module;
        private readonly string m_sAssemblyFullPath;
        private readonly CreateProxyTypeBuilderDelegate m_createProxyTypeBuilder;
        private readonly ProxyAssemblyBuilderSettings m_settings;
        private readonly BindingFlags m_methodBindingFlags;
        private readonly BindingFlags m_propertyBindingFlags;

        #endregion

        #region private class fields --------------------------------------------------------------

        private static readonly string DefaultNamespace = "Jolt.Testing.CodeGeneration";
        private static readonly string DefaultAssemblyFilename = DefaultNamespace + ".Proxies.dll";
        private static readonly string[] PropertyMethodPrefixes = { "get_", "set_" };
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxyAssemblyBuilder));

        #endregion
    }
}
