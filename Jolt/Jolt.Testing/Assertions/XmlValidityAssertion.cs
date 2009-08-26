// ----------------------------------------------------------------------------
// XmlValidityAssertion.cs
//
// Contains the definition of the XmlValidityAssertion class.
// Copyright 2009 Steve Guidi.
//
// File created: 5/23/2009 07:58:41
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.Testing.Properties;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides methods to validate XML against a given schema set.
    /// </summary>
    public class XmlValidityAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the assertion class with the schemas
        /// required to perform the validation.  Treats validation
        /// warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        public XmlValidityAssertion(XmlSchemaSet schemas)
            : this(schemas, XmlSchemaValidationFlags.ReportValidationWarnings) { }

        /// <summary>
        /// Initializes the assertion class with the schemas
        /// required to perform the validation and the validator
        /// configuration.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        /// 
        /// <param name="validationFlags">
        /// The configuration of the XML validator.
        /// </param>
        public XmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags validationFlags)
        {
            m_schemas = schemas;
            m_validationFlags = validationFlags;
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Validates the document contained in the given reader, according
        /// to the configuration of the assertion class.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The reader to validate.
        /// </param>
        public virtual IList<ValidationEventArgs> Validate(XmlReader reader)
        {
            List<ValidationEventArgs> result = new List<ValidationEventArgs>();
            using (XmlReader validatingReader = XmlReader.Create(reader, CreateReaderSettings((s, a) => result.Add(a))))
            {
                while (validatingReader.Read()) { }
            }

            return result;
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Creates the XmlReaderSettings required to perform the validation
        /// as prescribed by the configuration of the class.  Accepts a
        /// validation event handler to receive XML validation errors.
        /// </summary>
        /// 
        /// <param name="validationEventHandler">
        /// The validaition event handler that is associated with the created
        /// reader settings.
        /// </param>
        internal XmlReaderSettings CreateReaderSettings(ValidationEventHandler validationEventHandler)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = m_schemas;
            settings.ValidationFlags |= m_validationFlags;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += validationEventHandler;
            return settings;
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly XmlSchemaSet m_schemas;
        private readonly XmlSchemaValidationFlags m_validationFlags;

        #endregion
    }
}