// ----------------------------------------------------------------------------
// ProxyAssemblyBuilder.cs
//
// Contains the definition of the ProxyAssemblyBuilder class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/31/2007 14:09:03
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;

using Jolt.Reflection;
using Jolt.Testing.Properties;
using log4net;

namespace Jolt.Testing.CodeGeneration
{
    // Represents a factory method that constructs a ProxyTypeBuilder
    // class using the constructor overload matching the delegate
    // parameter signature.
    using CreateProxyTypeBuilderDelegate = Func<string, Type, bool, ModuleBuilder, IProxyTypeBuilder>;


    /// <summary>
    /// Provides methods to create an <see cref="System.Reflection.Assembly"/>
    /// and reverse engineer proxy types.
    /// </summary>
    /// 
    /// <remarks>
    /// The default root namespace is "Jolt.Testing.Generated".
    /// </remarks>
    /// 
    /// <seealso cref="ProxyTypeBuilder"/>
    public class ProxyAssemblyBuilder
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilder"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Initializes the assembly builder, using the working directory as the
        /// assembly location and a default assembly namespace and filename.
        /// </remarks>
        public ProxyAssemblyBuilder() : this(DefaultNamespace) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilder"/> class,
        /// allowing specification of the assembly namespace.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The assembly's root namespace.
        /// </param>
        ///
        /// <remarks>
        /// Initializes the assembly builder, using the working directory as the
        /// assembly location, and a default assembly filename.
        /// </remarks>
        public ProxyAssemblyBuilder(string rootNamespace)
            : this(rootNamespace, Path.Combine(Environment.CurrentDirectory, DefaultAssemblyFilename)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilder"/> class,
        /// allowing specification of the assembly namespace and path.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="assemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        public ProxyAssemblyBuilder(string rootNamespace, string assemblyFullPath)
            : this (rootNamespace, assemblyFullPath, ConfigurationManager.GetSection("proxyBuilderSettings") as ProxyAssemblyBuilderSettings) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilder"/> class,
        /// allowing specification of the assembly namespace, path, and configuration.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="assemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        /// 
        /// <param name="settings">
        /// A <see cref="ProxyAssemblyBuilderSettings"/> object containing configuration settings.
        /// </param>
        public ProxyAssemblyBuilder(string rootNamespace, string assemblyFullPath, ProxyAssemblyBuilderSettings settings)
            : this(rootNamespace, assemblyFullPath, settings, (ns, t, xml, mb) => new ProxyTypeBuilder(ns, t, xml, mb)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilder"/> class,
        /// allowing specification of the assembly namespace, path, and configuration.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The assembly's root namespace.
        /// </param>
        /// 
        /// <param name="assemblyFullPath">
        /// The full path of the proxy assembly.
        /// </param>
        /// 
        /// <param name="settings">
        /// A <see cref="ProxyAssemblyBuilderSettings"/> object containing configuration settings.
        /// </param>
        /// 
        /// <param name="createTypeBuilder">
        /// The factory method to use for creating a <see cref="ProxyTypeBuilder"/> object.
        /// </param>
        /// 
        /// <remarks>
        /// Initializes the assembly builder, overriding the default <see cref="ProxyTypeBuilder"/>
        /// factory method.  Used internally by test code to override <see cref="ProxyTypeBuilder"/>
        /// operations.
        /// </remarks>
        internal ProxyAssemblyBuilder(string rootNamespace, string assemblyFullPath,
            ProxyAssemblyBuilderSettings settings, CreateProxyTypeBuilderDelegate createTypeBuilder)
        {
            m_rootNamespace = rootNamespace;
            m_assemblyFullPath = assemblyFullPath;
            m_createProxyTypeBuilder = createTypeBuilder;
            m_settings = settings ?? ProxyAssemblyBuilderSettings.Default;

            AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyFullPath));
            assemblyName.KeyPair = m_settings.KeyPair;
            assemblyName.Version = new Version(1, 0);

            m_assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave,
                Path.GetDirectoryName(m_assemblyFullPath));
            m_module = m_assembly.DefineDynamicModule(DefaultAssemblyFilename, Path.GetFileName(assemblyFullPath), true);

            m_methodBindingFlags = ComputeMemberBindingFlags(m_settings.EmitMethods, m_settings.EmitStatics);
            m_propertyBindingFlags = ComputeMemberBindingFlags(m_settings.EmitProperties, m_settings.EmitStatics);
            m_eventBindingFlags = ComputeMemberBindingFlags(m_settings.EmitEvents, m_settings.EmitStatics);

            m_xmlDocComments = new XDocument();
            if (m_settings.EmitXmlDocComments)
            {
                m_xmlDocComments.Add(
                    new XElement(XmlDocCommentNames.DocElement,
                        new XElement(XmlDocCommentNames.AssemblyElement,
                            new XElement(XmlDocCommentNames.NameElement, assemblyName.Name)),
                        new XElement(XmlDocCommentNames.MembersElement)));
            }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a <see cref="System.Type"/> to the assembly builder.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> to which a proxy and interface are created.
        /// </param>
        /// 
        /// <remarks>
        /// Generates an interface and proxy type containing all of the methods,
        /// properties and events of <paramref name="realSubjectType"/>.
        /// </remarks>
        public virtual void AddType(Type realSubjectType)
        {
            AddType(realSubjectType, EmptyDictionary);
        }

        /// <summary>
        /// Adds a <see cref="System.Type"/> to the assembly builder, allowing
        /// the option to override the return types of the given type's methods.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> to which a proxy and interface are created.
        /// </param>
        /// 
        /// <param name="desiredReturnTypeOverrides">
        /// A collection of desired return type overrides for <paramref name="realSubjectType"/>.
        /// </param>
        ///
        /// <remarks>
        /// Generates an interface and proxy type containing all of the methods,
        /// properties and events of <paramref name="realSubjectType"/>.
        /// </remarks>
        public virtual void AddType(Type realSubjectType, IDictionary<Type, Type> desiredReturnTypeOverrides)
        {
            IProxyTypeBuilder builder = m_createProxyTypeBuilder(m_rootNamespace, realSubjectType, m_settings.EmitXmlDocComments, m_module);
            Array.ForEach(realSubjectType.GetProperties(m_propertyBindingFlags), property => HandleExceptionsIn(() => AddProperty(property, desiredReturnTypeOverrides, builder)));
            Array.ForEach(realSubjectType.GetEvents(m_eventBindingFlags), evt => HandleExceptionsIn(() => builder.AddEvent(evt)));
            Array.ForEach(realSubjectType.GetMethods(m_methodBindingFlags), method => AddMethod(method, desiredReturnTypeOverrides, builder));

            builder.CreateProxy();

            using (XmlReader xmlDocCommentReader = builder.CreateXmlDocCommentReader())
            {
                AppendXmlDocComments(xmlDocCommentReader);
            }
        }

        /// <summary>
        /// Writes the state of the <see cref="ProxyAssemblyBuilder"/> to disk
        /// in the form of an <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Returns a reference to the created <see cref="System.Reflection.Assembly"/> .
        /// </returns>
        /// 
        /// <remarks>
        /// The <see cref="Assembly"/> is written to <see cref="AssemblyFullPath"/>.
        /// </remarks>
        public virtual Assembly CreateAssembly()
        {
            m_assembly.Save(Path.GetFileName(m_assemblyFullPath));

            if (m_xmlDocComments.Root != null &&
                !m_xmlDocComments.Root.Element(XmlDocCommentNames.MembersElement).IsEmpty)
            {
                m_xmlDocComments.Save(Path.ChangeExtension(m_assemblyFullPath, "xml"));
            }

            return m_assembly;
        }

        /// <summary>
        /// Creates an <see cref="System.Xml.XmlReader"/> to read the XML doc comments of
        /// created <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A new <see cref="System.Xml.XmlReader"/> capable of reading any
        /// produced XML doc comments.
        /// </returns>
        public virtual XmlReader CreateXmlDocCommentReader()
        {
            return m_xmlDocComments.CreateReader();
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the root namespace of the assembly.
        /// </summary>
        public string RootNamespace
        {
            get { return m_rootNamespace; }
        }

        /// <summary>
        /// Gets the full path of the proxy assembly.
        /// </summary>
        public string AssemblyFullPath
        {
            get { return m_assemblyFullPath; }
        }

        /// <summary>
        /// Gets the <see cref="ProxyAssemblyBuilderSettings"/> object associated with this object.
        /// </summary>
        public ProxyAssemblyBuilderSettings Settings
        {
            get { return m_settings; }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets a reference to the encapsulated <see cref="System.Reflection.Emit.AssemblyBuilder"/>.
        /// </summary>
        internal AssemblyBuilder Assembly
        {
            get { return m_assembly; }
        }

        /// <summary>
        /// Gets a reference to the encapsulated <see cref="System.Reflection.Emit.ModuleBuilder"/>.
        /// </summary>
        internal ModuleBuilder Module
        {
            get { return m_module; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Appends the given XML doc comments to the internally stored XML doc comments.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The <see cref="System.Xml.XmlReader"/> containing the XML doc comments to append.
        /// </param>
        private void AppendXmlDocComments(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement(XmlDocCommentNames.MemberElement))
                {
                    m_xmlDocComments.Root.Element(XmlDocCommentNames.MembersElement)
                        .Add(XElement.Load(reader.ReadSubtree()));
                }
            }
        }

        /// <summary>
        /// Adds the given method to a given <see cref="IProxyTypeBuilder"/>
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="MethodInfo"/> representing the member to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnTypeOverrides">
        /// A collection of desired return type overrides for <paramref name="method"/>.
        /// </param>
        /// 
        /// <param name="builder">
        /// The <see cref="IProxyTypeBuilder"/> instance that accepts the method.
        /// </param>
        /// 
        /// <remarks>
        /// <paramref name="method"/> is added to <paramref name="builder"/> if
        /// it isn't already part of another member (property, event) on the proxy type.
        /// </remarks>
        private void AddMethod(MethodInfo method, IDictionary<Type, Type> desiredReturnTypeOverrides, IProxyTypeBuilder builder)
        {
            if (IsSpecialMethod(method, PropertyMethodPrefixes, m_propertyBindingFlags))
            {
                if (m_propertyBindingFlags != BindingFlags.Default)
                {
                    // Property method detected and property-emit is enabled.
                    // Emit warning as property already added to the builder.
                    Log.InfoFormat(Resources.Info_SkipDefinedPropertyMethod, method.Name);
                }
            }
            else if (IsSpecialMethod(method, EventMethodPrefixes, m_eventBindingFlags))
            {
                if (m_eventBindingFlags != BindingFlags.Default)
                {
                    // Event method detected and event-emit is enabled.
                    // Emit warning as event already added to the builder.
                    Log.InfoFormat(Resources.Info_SkipDefinedEventMethod, method.Name);
                }
            }
            else
            {
                HandleExceptionsIn(delegate
                {
                    if (desiredReturnTypeOverrides.ContainsKey(method.ReturnType))
                    {
                        builder.AddMethod(method, desiredReturnTypeOverrides[method.ReturnType]);
                    }
                    else
                    {
                        builder.AddMethod(method);
                    }
                });
            }
        }


        /// <summary>
        /// Determines if a given <see cref="System.Reflection.MethodInfo"/> is
        /// defined as part of a property or event on its declaring type.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to validate.
        /// </param>
        /// 
        /// <param name="methodPrefixes">
        /// The method prefixes to use as part of validation (e.g. "add_", "get_").
        /// </param>
        /// 
        /// <param name="bindings">
        /// The <see cref="System.Reflection.BindingFlags"/> to use when searching for
        /// a special method.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the <paramref name="method"/> belongs to a property or event,
        /// false otherwise.
        /// </returns>
        private static bool IsSpecialMethod(MethodInfo method, string[] methodPrefixes, BindingFlags bindings)
        {
            foreach (string sPrefix in methodPrefixes)
            {
                if (method.Name.IndexOf(sPrefix) == 0 &&
                    method.DeclaringType.GetMember(method.Name.Substring(sPrefix.Length), bindings) != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a <see cref="System.Reflection.BindingFlags"/> enumeration representing
        /// the given configuration.
        /// </summary>
        /// 
        /// <param name="emitMember">
        /// Denotes if the member type is emitted.
        /// </param>
        /// 
        /// <param name="emitStatics">
        /// Denotes if static types are emitted.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="System.Reflection.BindingFlags"/> enumeration,
        /// representing the requested configuration.
        /// </returns>
        private static BindingFlags ComputeMemberBindingFlags(bool emitMember, bool emitStatics)
        {
            BindingFlags result = BindingFlags.Default;
            if (emitMember)
            {
                result = CompoundBindingFlags.PublicInstance;
                if (emitStatics)
                {
                    result |= BindingFlags.Static;
                }
            }

            return result;
        }

        /// <summary>
        /// Invokes a given delegate logging any caught <see cref="System.InvalidOperationException"/>.
        /// </summary>
        /// 
        /// <param name="action">
        /// The function to invoke.
        /// </param>
        private static void HandleExceptionsIn(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                Log.Warn(ex.Message);
            }
        }

        /// <summary>
        /// Adds a given <see cref="System.Reflection.PropertyInfo"/> to the given <see cref="IProxyTypeBuilder"/>.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnTypeOverrides">
        /// A collection of return type overrides for <paramref name="property"/>.
        /// </param>
        /// 
        /// <param name="builder">
        /// The <see cref="IProxyTypeBuilder"/> that accepts the property.
        /// </param>
        /// 
        /// <remarks>
        /// Overrides the property return type when it is located in
        /// <paramref name="desiredReturnTypeOverrides"/>.
        /// </remarks>
        private static void AddProperty(PropertyInfo property, IDictionary<Type, Type> desiredReturnTypeOverrides, IProxyTypeBuilder builder)
        {
            if (desiredReturnTypeOverrides.ContainsKey(property.PropertyType))
            {
                builder.AddProperty(property, desiredReturnTypeOverrides[property.PropertyType]);
            }
            else
            {
                builder.AddProperty(property);
            }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly string m_rootNamespace;
        private readonly AssemblyBuilder m_assembly;
        private readonly ModuleBuilder m_module;
        private readonly string m_assemblyFullPath;
        private readonly XDocument m_xmlDocComments;
        private readonly CreateProxyTypeBuilderDelegate m_createProxyTypeBuilder;
        private readonly ProxyAssemblyBuilderSettings m_settings;
        private readonly BindingFlags m_methodBindingFlags;
        private readonly BindingFlags m_propertyBindingFlags;
        private readonly BindingFlags m_eventBindingFlags;

        private static readonly string DefaultNamespace = "Jolt.Testing.CodeGeneration";
        private static readonly string DefaultAssemblyFilename = DefaultNamespace + ".Proxies.dll";
        private static readonly string[] PropertyMethodPrefixes = { "get_", "set_" };
        private static readonly string[] EventMethodPrefixes = { "add_", "remove_" };
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxyAssemblyBuilder));
        private static readonly Dictionary<Type, Type> EmptyDictionary = new Dictionary<Type, Type>(0);

        #endregion
    }
}
