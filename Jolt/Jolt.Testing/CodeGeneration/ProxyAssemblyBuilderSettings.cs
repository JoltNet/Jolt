// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderSettings.cs
//
// Contains the definition of the ProxyAssemblyBuilderSettings class.
// Copyright 2007 Steve Guidi.
//
// File created: 10/26/2007 15:13:13
// ----------------------------------------------------------------------------

using System.Configuration;

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
        {
            this["emitStatics"] = emitStatics;
            this["emitMethods"] = emitMethods;
            this["emitProperties"] = emitProperties;
            this["emitEvents"] = emitEvents;
            this["emitXmlDocComments"] = emitXmlDocComments;
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

        #endregion

        #region public fields ---------------------------------------------------------------------

        /// <summary>
        /// Store an instance of the default configuration.
        /// </summary>
        public static readonly ProxyAssemblyBuilderSettings Default = new ProxyAssemblyBuilderSettings();

        #endregion
    }
}
