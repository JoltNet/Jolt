// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderSettings.cs
//
// Contains the definition of the ProxyAssemblyBuilderSettings class.
// Copyright 2007 Steve Guidi.
//
// File created: 10/26/2007 15:13:13
// ----------------------------------------------------------------------------

using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides configuration settings to control the type construction
    /// process in the <see cref="ProxyAssemblyBuilder"/> class.
    /// </summary>
    public sealed class ProxyAssemblyBuilderSettings : ConfigurationSection
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilderSettings"/> class
        /// with the default attribute values.
        /// </summary>
        public ProxyAssemblyBuilderSettings() { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilderSettings"/> class
        /// with the given attribute values.
        /// </summary>
        /// 
        /// <param name="emitStatics">
        /// Determines if static methods/properties are generated.
        /// </param>
        /// 
        /// <param name="emitMethods">
        /// Determines if methods are generated.
        /// </param>
        /// 
        /// <param name="emitProperties">
        /// Determines if properties are generated.
        /// </param>
        /// 
        /// <param name="emitEvents">
        /// Determines if events are generated.
        /// </param>
        /// 
        /// <param name="emitXmlDocComments">
        /// Determines if XML doc comments are produced.
        /// </param>
        public ProxyAssemblyBuilderSettings(bool emitStatics, bool emitMethods, bool emitProperties, bool emitEvents, bool emitXmlDocComments)
            : this(emitStatics, emitMethods, emitProperties, emitEvents, emitXmlDocComments, null) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyAssemblyBuilderSettings"/> class
        /// with the given attribute values, enabling assembly signing.
        /// </summary>
        /// 
        /// <param name="emitStatics">
        /// Determines if static methods/properties are generated.
        /// </param>
        /// 
        /// <param name="emitMethods">
        /// Determines if methods are generated.
        /// </param>
        /// 
        /// <param name="emitProperties">
        /// Determines if properties are generated.
        /// </param>
        /// 
        /// <param name="emitEvents">
        /// Determines if events are generated.
        /// </param>
        /// 
        /// <param name="emitXmlDocComments">
        /// Determines if XML doc comments are produced.
        /// </param>
        /// 
        /// <param name="keyPairFullPath">
        /// The full path to a strong-name key-pair file, enabling assembly signing.
        /// </param>
        public ProxyAssemblyBuilderSettings(bool emitStatics, bool emitMethods, bool emitProperties, bool emitEvents, bool emitXmlDocComments, string keyPairFullPath)
        {
            this["emitStatics"] = emitStatics;
            this["emitMethods"] = emitMethods;
            this["emitProperties"] = emitProperties;
            this["emitEvents"] = emitEvents;
            this["emitXmlDocComments"] = emitXmlDocComments;

            if (!String.IsNullOrEmpty(keyPairFullPath))
            {
                this["keyPairFullPath"] = keyPairFullPath;
            }
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the value denoting the ability to generate a static function.
        /// </summary>
        [ConfigurationProperty("emitStatics", IsRequired=false, DefaultValue=true)]
        public bool EmitStatics
        {
            get { return (bool)this["emitStatics"]; }
        }

        /// <summary>
        /// Gets the value denoting the ability to generate a method.
        /// </summary>
        [ConfigurationProperty("emitMethods", IsRequired=false, DefaultValue=true)]
        public bool EmitMethods
        {
            get { return (bool)this["emitMethods"]; }
        }

        /// <summary>
        /// Gets the value denoting the ability to generate a property.
        /// </summary>
        [ConfigurationProperty("emitProperties", IsRequired=false, DefaultValue=true)]
        public bool EmitProperties
        {
            get { return (bool)this["emitProperties"]; }
        }

        /// <summary>
        /// Gets the value denoting the ability to generate an event.
        /// </summary>
        [ConfigurationProperty("emitEvents", IsRequired=false, DefaultValue=true)]
        public bool EmitEvents
        {
            get { return (bool)this["emitEvents"]; }
        }

        /// <summary>
        /// Gets the value denoting the ability to generate XML doc comments.
        /// </summary>
        [ConfigurationProperty("emitXmlDocComments", IsRequired=false, DefaultValue=false)]
        public bool EmitXmlDocComments
        {
            get { return (bool)this["emitXmlDocComments"]; }
        }

        /// <summary>
        /// Gets the value denoting the path of the strong-name key-pair file
        /// that will sign the assembly.
        /// </summary>
        /// 
        /// <remarks>
        /// An empty value denotes that assembly signing is disabled.
        /// </remarks>
        [ConfigurationProperty("keyPairFullPath", IsRequired = false, DefaultValue = "")]
        public string KeyPairFullPath
        {
            get { return (string)this["keyPairFullPath"]; }
        }

        /// <summary>
        /// Gets/sets the <see cref="System.Reflection.StrongNameKeyPair"/> object for signing
        /// a proxy assembly.
        /// </summary>
        /// 
        /// <remarks>
        /// If the backing store for the property is null, the
        /// <see cref="System.Reflection.StrongNameKeyPair"/> object is loaded from the file at
        /// <see cref="KeyPairFullPath"/> and stored for future use.
        /// </remarks>
        public StrongNameKeyPair KeyPair
        {
            get
            {
                if (m_keyPair == null && !String.IsNullOrEmpty(KeyPairFullPath))
                {
                    // We can not use a file proxy to abstract the allow for interception
                    // of the File.Open method, since StrongNameKeyPair requires a FileStream
                    // parameter in its ctor.
                    using (FileStream keyPairFile = File.Open(KeyPairFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        m_keyPair = new StrongNameKeyPair(keyPairFile);
                    }
                }

                return m_keyPair;
            }
            set { m_keyPair = value; }
        }

        #endregion

        #region public fields ---------------------------------------------------------------------

        /// <summary>
        /// Store an instance of the default configuration.
        /// </summary>
        public static readonly ProxyAssemblyBuilderSettings Default = new ProxyAssemblyBuilderSettings();

        #endregion

        #region private fields --------------------------------------------------------------------

        private StrongNameKeyPair m_keyPair;

        #endregion
    }
}
