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
using System.Xml.Schema;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides assertion methods that validate XML against a schema set.
    /// </summary>
    public class XmlValidityAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class,
        /// treating all validation warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the desired valid XML structure.
        /// </param>
        public XmlValidityAssertion(XmlSchemaSet schemas)
            : this(schemas, XmlSchemaValidationFlags.ReportValidationWarnings) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityAssertion"/> class,
        /// and configures the validator with a given set of rules.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        /// 
        /// <param name="validationFlags">
        /// An <see cref="XmlSchemaValidationFlags"/> enumeration that configures the XML validator.
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
        /// The <see cref="System.Xml.XmlReader"/> to validate.
        /// </param>
        /// 
        /// <returns>
        /// Returns a new <see cref="System.Collections.Generic.IList"/> containing each
        /// validation error raised during the validation process.
        /// </returns>
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
        /// Creates the <see cref="XmlReaderSettings"/> required to perform the validation
        /// as prescribed by the configuration of the class.
        /// </summary>
        /// 
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that is associated with the created
        /// reader settings.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlReaderSetting"/> class, intialized with
        /// <paramref name="validationEventHandler"/>, and the configured schemas and validation
        /// flags.
        /// </returns>
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

        #region private fields --------------------------------------------------------------------

        private readonly XmlSchemaSet m_schemas;
        private readonly XmlSchemaValidationFlags m_validationFlags;

        #endregion
    }
}