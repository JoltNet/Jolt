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
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;

using Jolt.Testing.Properties;
using log4net;

namespace Jolt.Testing.CodeGeneration
{
    // Represents a factory method that constructs a ProxyTypeBuilder
    // class using the constructor overload matching the delegate
    // parameter signature.
    using CreateProxyTypeBuilderDelegate = Func<string, Type, bool, ModuleBuilder, IProxyTypeBuilder>;


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
            : this(sRootNamespace, sAssemblyFullPath, settings, (ns, t, xml, mb) => new ProxyTypeBuilder(ns, t, xml, mb)) { }

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

            AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(sAssemblyFullPath));
            assemblyName.Version = new Version(1, 0);
            m_assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave,
                Path.GetDirectoryName(m_sAssemblyFullPath));
            m_module = m_assembly.DefineDynamicModule(DefaultAssemblyFilename, Path.GetFileName(sAssemblyFullPath), true);

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
        public virtual void AddType(Type realSubjectType)
        {
            IProxyTypeBuilder builder = m_createProxyTypeBuilder(m_sRootNamespace, realSubjectType, m_settings.EmitXmlDocComments, m_module);
            Array.ForEach(realSubjectType.GetProperties(m_propertyBindingFlags), property => HandleExceptionsIn(() => builder.AddProperty(property)));
            Array.ForEach(realSubjectType.GetEvents(m_eventBindingFlags), evt => HandleExceptionsIn(() => builder.AddEvent(evt)));
            Array.ForEach(realSubjectType.GetMethods(m_methodBindingFlags), method => AddMethod(method, builder));

            builder.CreateProxy();

            using (XmlReader xmlDocCommentReader = builder.CreateXmlDocCommentReader())
            {
                AppendXmlDocComments(xmlDocCommentReader);
            }
        }

        /// <summary>
        /// Saves the state of the assembly builder to disk in the form
        /// of an assembly.
        /// </summary>
        public virtual Assembly CreateAssembly()
        {
            m_assembly.Save(Path.GetFileName(m_sAssemblyFullPath));

            if (m_xmlDocComments.Root != null &&
                !m_xmlDocComments.Root.Element(XmlDocCommentNames.MembersElement).IsEmpty)
            {
                m_xmlDocComments.Save(Path.ChangeExtension(m_sAssemblyFullPath, "xml"));
            }

            return m_assembly;
        }

        /// <summary>
        /// Creates an XmlReader capable of reading any produced XML doc comments.
        /// </summary>
        public virtual XmlReader CreateXmlDocCommentReader()
        {
            return m_xmlDocComments.CreateReader();
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
        /// Reads the XML doc comments from the given reader, appending
        /// them to the XML doc comments stored by the class.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The XmlReader containing the XML doc comments to append.
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
                HandleExceptionsIn(() => builder.AddMethod(method));
            }
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Determines if a given method is defined as part of a property
        /// or event on its declaring type.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to validate.
        /// </param>
        /// 
        /// <param name="methodPrefixes">
        /// The method prefixes to use as part of validation (e.g. "add_", "get_").
        /// </param>
        /// 
        /// <param name="bindings">
        /// The property or event method bindings to use during validation.
        /// </param>
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

        /// <summary>
        /// Invokes a given delegate logging any caught InvalidOperationException.
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

        #endregion

        #region private instance fields -----------------------------------------------------------

        private readonly string m_sRootNamespace;
        private readonly AssemblyBuilder m_assembly;
        private readonly ModuleBuilder m_module;
        private readonly string m_sAssemblyFullPath;
        private readonly XDocument m_xmlDocComments;
        private readonly CreateProxyTypeBuilderDelegate m_createProxyTypeBuilder;
        private readonly ProxyAssemblyBuilderSettings m_settings;
        private readonly BindingFlags m_methodBindingFlags;
        private readonly BindingFlags m_propertyBindingFlags;
        private readonly BindingFlags m_eventBindingFlags;

        #endregion

        #region private class fields --------------------------------------------------------------

        private static readonly string DefaultNamespace = "Jolt.Testing.CodeGeneration";
        private static readonly string DefaultAssemblyFilename = DefaultNamespace + ".Proxies.dll";
        private static readonly string[] PropertyMethodPrefixes = { "get_", "set_" };
        private static readonly string[] EventMethodPrefixes = { "add_", "remove_" };
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxyAssemblyBuilder));

        #endregion
    }
}
