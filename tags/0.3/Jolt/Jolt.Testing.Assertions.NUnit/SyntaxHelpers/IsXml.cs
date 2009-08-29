// ----------------------------------------------------------------------------
// IsXml.cs
//
// Contains the definition of the IsXml class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/22/2009 23:05:27
// ----------------------------------------------------------------------------

using System.Xml;
using System.Xml.Schema;

namespace Jolt.Testing.Assertions.NUnit.SyntaxHelpers
{
    /// <summary>
    /// Defines an NUnit syntax helper class that creates Jolt XML
    /// constraints.
    /// </summary>
    public static class IsXml
    {
        /// <summary>
        /// Creates an <see cref="XmlValidityConstraint"/> for the given schemas.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas to validate with.
        /// </param>
        public static XmlValidityConstraint ValidWith(XmlSchemaSet schemas)
        {
            return new XmlValidityConstraint(schemas);
        }

        /// <summary>
        /// Creates an <see cref="XmlValidityConstraint"/> for the given schemas
        /// and validation flags.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas to validate with.
        /// </param>
        /// 
        /// <param name="validationFlags">
        /// Validation flags that customize the type of validation.
        /// </param>
        public static XmlValidityConstraint ValidWith(XmlSchemaSet schemas, XmlSchemaValidationFlags validationFlags)
        {
            return new XmlValidityConstraint(schemas, validationFlags);
        }

        /// <summary>
        /// Creates an <see cref="XmlEqualityConstraint"/> for the given XML.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML that is applied to the constraint.
        /// </param>
        public static XmlEqualityConstraint EqualTo(XmlReader expected)
        {
            return new XmlEqualityConstraint(expected);
        }

        /// <summary>
        /// Creates an <see cref="XmlEquivalencyConstraint"/> for the given XML.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML that is applied to the constraint.
        /// </param>
        public static XmlEquivalencyConstraint EquivalentTo(XmlReader expected)
        {
            return new XmlEquivalencyConstraint(expected);
        }
    }
}