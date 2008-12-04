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
    /// process in the ProxyAssemblyBuilder class.
    /// </summary>
    public sealed class ProxyAssemblyBuilderSettings : ConfigurationSection
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the object with the default configuration settings.
        /// </summary>
        public ProxyAssemblyBuilderSettings() { }

        /// <summary>
        /// Initializes the object's configuration setting fields.
        /// </summary>
        /// 
        /// <param name="emitStatics">
        /// Determines if static methods and properties are generated.
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
        public ProxyAssemblyBuilderSettings(bool emitStatics, bool emitMethods, bool emitProperties, bool emitEvents)
        {
            this["emitStatics"] = emitStatics;
            this["emitMethods"] = emitMethods;
            this["emitProperties"] = emitProperties;
            this["emitEvents"] = emitEvents;
        }

        #endregion

        /// <summary>
        /// Initializes a field to store an instance of the default configuration.
        /// </summary>
        public static readonly ProxyAssemblyBuilderSettings Default = new ProxyAssemblyBuilderSettings();


        /// <summary>
        /// Gets the value that controls static function generation.
        /// </summary>
        [ConfigurationProperty("emitStatics", IsRequired=false, DefaultValue=true)]
        public bool EmitStatics
        {
            get { return (bool)this["emitStatics"]; }
        }

        /// <summary>
        /// Gets the value that controls method generation.
        /// </summary>
        [ConfigurationProperty("emitMethods", IsRequired=false, DefaultValue=true)]
        public bool EmitMethods
        {
            get { return (bool)this["emitMethods"]; }
        }

        /// <summary>
        /// Gets the value that controls property generation.
        /// </summary>
        [ConfigurationProperty("emitProperties", IsRequired=false, DefaultValue=true)]
        public bool EmitProperties
        {
            get { return (bool)this["emitProperties"]; }
        }

        /// <summary>
        /// Gets the value that controls event generation.
        /// </summary>
        [ConfigurationProperty("emitEvents", IsRequired=false, DefaultValue=true)]
        public bool EmitEvents
        {
            get { return (bool)this["emitEvents"]; }
        }
    }
}
