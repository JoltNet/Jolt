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
    /// Defines an NUnit constraint for validating XML against a given
    /// set of XML schemas.
    /// </summary>
    public sealed class XmlValidityConstraint : AbstractConstraint<XmlReader, IList<ValidationEventArgs>>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityConstraint"/>,
        /// treating all validation warnings as errors.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas defining the desired valid XML structure.
        /// </param>
        public XmlValidityConstraint(XmlSchemaSet schemas)
            : this(Factory.CreateXmlValidityAssertion(schemas)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityConstraint"/> class,
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
        public XmlValidityConstraint(XmlSchemaSet schemas, XmlSchemaValidationFlags validationFlags)
            : this(Factory.CreateXmlValidityAssertion(schemas, validationFlags)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlValidityConstraint"/> class,
        /// encapsulating an <see cref="XmlValidityAssertion"/>.
        /// </summary>
        /// 
        /// <param name="assertion">
        /// The <see cref="XmlValidityAssertion"/> that performs the validation.
        /// </param>
        /// 
        /// <remarks>
        /// Used internally by test code to override assertion operations.
        /// </remarks>
        internal XmlValidityConstraint(XmlValidityAssertion assertion)
        {
            m_assertion = assertion;
        }

        #endregion

        #region AbstractXmlConstraint members -----------------------------------------------------

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
        /// Gets the <see cref="XmlValidityAssertion"/> that is associated with the instance.
        /// </summary>
        internal XmlValidityAssertion Assertion
        {
            get { return m_assertion; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XmlValidityAssertion m_assertion;

        #endregion
    }
}