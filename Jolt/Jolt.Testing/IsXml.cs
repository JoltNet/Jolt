// ----------------------------------------------------------------------------
// IsXml.cs
//
// Contains the definition of the IsXml class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/22/2009 23:05:27
// ----------------------------------------------------------------------------

using System;
using System.Xml;
using System.Xml.Schema;

using Jolt.Testing.Assertions.NUnit;

namespace Jolt.Testing
{
    /// <summary>
    /// Defines an NUnit syntax helper class that creates Jolt XML constraints.
    /// </summary>
    public static class IsXml
    {
        /// <summary>
        /// Creates a new instance of an <see cref="XmlValidityConstraint"/> for the given schemas.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas to validate with.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of an <see cref="XmlValidityConstraint"/>.
        /// </returns>
        public static XmlValidityConstraint ValidWith(XmlSchemaSet schemas)
        {
            return new XmlValidityConstraint(schemas);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="XmlValidityConstraint"/> for the given schemas
        /// and <see cref="XmlSchemaValidationFlags"/> value.
        /// </summary>
        /// 
        /// <param name="schemas">
        /// The schemas to validate with.
        /// </param>
        /// 
        /// <param name="validationFlags">
        /// The <see cref="XmlSchemaValidationFlags"/> values that customize the type of validation.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of an <see cref="XmlValidityConstraint"/>.
        /// </returns>
        public static XmlValidityConstraint ValidWith(XmlSchemaSet schemas, XmlSchemaValidationFlags validationFlags)
        {
            return new XmlValidityConstraint(schemas, validationFlags);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="XmlEqualityConstraint"/> for the given
        /// <see cref="System.Xml.XmlReader"/>.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XmlReader"/>,
        /// that is applied to the constraint.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of an <see cref="XmlEqualityConstraint"/>.
        /// </returns>
        public static XmlEqualityConstraint EqualTo(XmlReader expected)
        {
            return new XmlEqualityConstraint(expected);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="XmlEquivalencyConstraint"/> for the given
        /// <see cref="System.Xml.XmlReader"/>.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XmlReader"/>,
        /// that is applied to the constraint.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of an <see cref="XmlEquivalencyConstraint"/>.
        /// </returns>
        public static XmlEquivalencyConstraint EquivalentTo(XmlReader expected)
        {
            return new XmlEquivalencyConstraint(expected);
        }
    }
}