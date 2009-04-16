// ----------------------------------------------------------------------------
// XmlConfigurator.cs
//
// Contains the definition of the XmlConfigurator class.
// Copyright 2007 Steve Guidi.
//
// File created: 6/30/2007 00:20:00
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.Testing.Properties;
using log4net;

namespace Jolt.Testing.CodeGeneration.Xml
{
    /// <summary>
    /// Defines methods to read the XML configuration of the
    /// <seealso cref="ProxyAssemblyBuilder"/> class.
    /// </summary>
    public static class XmlConfigurator
    {
        #region constructors ----------------------------------------------------------------------

        static XmlConfigurator()
        {
            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.ValidationType = ValidationType.Schema;
            ReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            Type thisType = typeof(XmlConfigurator);
            using (Stream schema = thisType.Assembly.GetManifestResourceStream(thisType, "RealSubjectTypes.xsd"))
            {
                ReaderSettings.Schemas.Add(XmlSchema.Read(schema, null));
            }

            Log = LogManager.GetLogger(typeof(XmlConfigurator));
            XmlNamespace = "{urn:Jolt.Testing.CodeGeneration.Xml}";
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Loads all of the types contained in the XML stream.
        /// </summary>
        /// 
        /// <param name="xmlConfiguration">
        /// The stream containing the XML configuration/enumeration of the real subject types.
        /// </param>
        /// 
        /// <retuns>
        /// Iterator containing the loaded types.
        /// </retuns>
        /// 
        /// <remarks>
        /// The given stream is not closed by this function.
        /// </remarks>
        public static IEnumerable<TypeDescriptor> LoadRealSubjectTypes(Stream xmlConfiguration)
        {
            XDocument realSubjectTypes = XDocument.Load(XmlReader.Create(xmlConfiguration, ReaderSettings));
            foreach(XElement typeElement in realSubjectTypes.Root.Elements(XmlNamespace + "Type"))
            {
                Type realSubjectType;
                if (LoadType(typeElement.Attribute("name"), out realSubjectType))
                {
                    IDictionary<Type, Type> returnTypeOverrides = new Dictionary<Type, Type>();
                    foreach (XElement overrideElement in typeElement.Elements(XmlNamespace + "OverrideReturnType"))
                    {
                        Type originalReturnType, desiredReturnType;
                        if (LoadType(overrideElement.Attribute("name"), out originalReturnType) &&
                            LoadType(overrideElement.Attribute("desiredTypeName"), out desiredReturnType))
                        {
                            if (!returnTypeOverrides.ContainsKey(originalReturnType))
                            {
                                returnTypeOverrides.Add(originalReturnType, desiredReturnType);
                            }
                            else
                            {
                                Log.WarnFormat(Resources.Warn_IgnoreReturnTypeOverride_Ambiguous, originalReturnType.Name, desiredReturnType.Name, realSubjectType.Name);
                            }
                        }
                    }

                    yield return new TypeDescriptor(realSubjectType, returnTypeOverrides);
                }
            }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Loads the type described by the given attribute, logging a warning if the type
        /// can not be loaded.  Returns a value denoting the success of the load operation.
        /// </summary>
        /// 
        /// <param name="typeAttribute">
        /// An attribute containing the name of the type to load.
        /// </param>
        /// 
        /// <param name="type">
        /// Contains the loaded type, or a null value if the loading is not successful.
        /// </param>
        private static bool LoadType(XAttribute typeAttribute, out Type type)
        {
            type = Type.GetType(typeAttribute.Value);
            bool isLoaded = type != null;

            if (!isLoaded)
            {
                Log.WarnFormat(Resources.Warn_TypeNotLoaded, typeAttribute.Value);
            }

            return isLoaded;
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly XmlReaderSettings ReaderSettings;
        private static readonly ILog Log;
        private static readonly string XmlNamespace;

        #endregion
    }
}
