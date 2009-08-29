// ----------------------------------------------------------------------------
// IsXmlTestFixture.cs
//
// Contains the definition of the IsXmlTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/23/2009 08:51:12
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class IsXmlTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ValidWith() method, when given a schema set.
        /// </summary>
        [Test]
        public void ValidWith_Schemas()
        {
            ConstraintConstructionTests.XmlValidityConstraint_Schemas(IsXml.ValidWith);
        }

        /// <summary>
        /// Verifies the behavior of the ValidWith() method, when given a schema set
        /// and validation flags.
        /// </summary>
        [Test]
        public void ValidWith_Schemas_Flags()
        {
            ConstraintConstructionTests.XmlValidityConstraint_Schemas_Flags(IsXml.ValidWith);
        }

        /// <summary>
        /// Verifies the behavior of the EqualTo() method.
        /// </summary>
        [Test]
        public void EqualTo()
        {
            ConstraintConstructionTests.XmlEqualityConstraint(IsXml.EqualTo);
        }

        [Test]
        public void EquivalentTo()
        {
            ConstraintConstructionTests.XmlEquivalencyConstraint(IsXml.EquivalentTo);
        }
    }
}