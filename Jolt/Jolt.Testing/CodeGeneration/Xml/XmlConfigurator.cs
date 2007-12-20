// ----------------------------------------------------------------------------
// XmlConfigurator.cs
//
// Contains the definition of the XmlConfigurator class.
// Copyright 2007 Steve Guidi.
//
// File created: 6/30/2007 00:20:00
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

using log4net;
using Jolt.Testing.Properties;

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
        public static IEnumerable<Type> LoadRealSubjectTypes(Stream xmlConfiguration)
        {
            using (XmlReader reader = XmlReader.Create(xmlConfiguration, ReaderSettings))
            {
                while (reader.Read())
                {
                    if (reader.LocalName == "Type")
                    {
                        Type realSubjectType = Type.GetType(reader.GetAttribute("name"));
                        if (realSubjectType != null)
                        {
                            yield return realSubjectType;
                        }
                        else
                        {
                            Log.WarnFormat(Resources.Warn_TypeNotLoaded, reader.GetAttribute("name"));
                        }
                    }
                }
            }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly XmlReaderSettings ReaderSettings;
        private static readonly ILog Log;

        #endregion
    }
}
