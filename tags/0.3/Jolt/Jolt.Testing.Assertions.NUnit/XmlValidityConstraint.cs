// ----------------------------------------------------------------------------
// XmlValidityConstraint.cs
//
// Contains the definition of the XmlValidityConstraint class.
// Copyright 2009 Steve Guidi.
//
// File created: 7/7/2009 09:06:09
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Jolt.Testing.Assertions.NUnit
{
    /// <summary>
    /// Implements a constraint to validate XML against a given
    /// set of XML schemas.
    /// </summary>
    public sealed class XmlValidityConstraint : AbstractConstraint<XmlReader, IList<ValidationEventArgs>>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the constraint with the schemas
        /// required to perform the validation.  Treats validation
        /// warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the valid XML structure.
        /// </param>
        public XmlValidityConstraint(XmlSchemaSet schemas)
            : this(new XmlValidityAssertion(schemas)) { }

        /// <summary>
        /// Initializes the constraint with the schemas
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
        public XmlValidityConstraint(XmlSchemaSet schemas, XmlSchemaValidationFlags validationFlags)
            : this(new XmlValidityAssertion(schemas, validationFlags)) { }

        /// <summary>
        /// Initializes the constraint with the assertion
        /// used to perform the validation.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The assertion that performs the validation.
        /// </param>
        internal XmlValidityConstraint(XmlValidityAssertion assertion)
        {
            m_assertion = assertion;
        }

        #endregion

        #region AbstractXmlConstraint overrides ---------------------------------------------------

        /// <summary>
        /// <see cref="AbstractXmlConstraint.Assert"/>
        /// </summary>
        protected override IList<ValidationEventArgs> Assert(XmlReader actual)
        {
            return m_assertion.Validate(actual);
        }

        /// <summary>
        /// <see cref="AbstractXmlConstraint.ToBoolean"/>
        /// </summary>
        protected override bool ToBoolean(IList<ValidationEventArgs> assertionResult)
        {
            return assertionResult.Count == 0;
        }

        /// <summary>
        /// <see cref="AbstractXmlConstraint.CreateAssertionErrorMessage"/>
        /// </summary>
        protected override string CreateAssertionErrorMessage(IList<ValidationEventArgs> assertionResult)
        {
            // For simplicity, report only the first validation error.
            return assertionResult[0].Message;
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the XmlValidityAssertion associated with the class.
        /// </summary>
        internal XmlValidityAssertion Assertion
        {
            get { return m_assertion; }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly XmlValidityAssertion m_assertion;

        #endregion
    }
}