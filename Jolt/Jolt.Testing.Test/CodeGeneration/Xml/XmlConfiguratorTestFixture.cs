// ----------------------------------------------------------------------------
// XmlConfiguratorTestFixture.cs
//
// Contains the definition of the XmlConfiguratorTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 6/29/2007 20:38:55
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Xml.Schema;

using Jolt.Testing.CodeGeneration.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration.Xml
{
    [TestFixture]
    public sealed class XmlConfiguratorTestFixture
    {
        #region public methods --------------------------------------------------------------------

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            log4net.Config.BasicConfigurator.Configure();
        }


        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method.
        /// </summary>
        [Test]
        public void LoadRealSubjectTypes()
        {
            Type[] expectedTypes = {typeof(string), typeof(System.IO.File), typeof(System.Text.Decoder)};
            using (Stream resource = GetEmbeddedResource("ValidConfiguration.xml"))
            {
                foreach (TypeDescriptor typeDescriptor in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.That(expectedTypes, List.Contains(typeDescriptor.RealSubjectType));
                    Assert.That(typeDescriptor.ReturnTypeOverrides, Is.Empty);
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream contains invalid types.
        /// </summary>
        [Test]
        public void LoadRealSubjectTypes_InvalidTypes()
        {
            Type[] expectedTypes = { typeof(string), typeof(System.IO.File), typeof(System.Text.Decoder) };
            using (Stream resource = GetEmbeddedResource("ContainsInvalidTypes.xml"))
            {
                foreach (TypeDescriptor typeDescriptor in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.That(expectedTypes, List.Contains(typeDescriptor.RealSubjectType));
                    Assert.That(typeDescriptor.ReturnTypeOverrides.Count, Is.EqualTo(0));
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(XmlSchemaValidationException))]
        public void LoadRealSubjectTypes_InvalidConfigurationStream()
        {
            using (Stream resource = GetEmbeddedResource("InvalidConfiguration.xml"))
            {
                foreach (TypeDescriptor typeDescriptor in XmlConfigurator.LoadRealSubjectTypes(resource)) { };
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream contains duplicate types.
        /// </summary>
        [Test, ExpectedException(typeof(XmlSchemaValidationException))]
        public void LoadRealSubjectTypes_DuplicateTypesInStream()
        {
            using (Stream resource = GetEmbeddedResource("DuplicateTypesConfiguration.xml"))
            {
                foreach (TypeDescriptor typeDescriptor in XmlConfigurator.LoadRealSubjectTypes(resource)) { }
                
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream is invalid.
        /// </summary>
        [Test]
        public void LoadRealSubjectTypes_EmptyConfigurationStream()
        {
            using (Stream resource = GetEmbeddedResource("EmptyConfiguration.xml"))
            {
                foreach (TypeDescriptor typeDescriptor in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.Fail();
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream contains valid return type overrides.
        /// </summary>
        [Test]
        public void LoadRealSubjectType_ReturnTypeOverride()
        {
            using (Stream resource = GetEmbeddedResource("ValidConfigurationWithOverrides.xml"))
            {
                TypeDescriptor typeDescriptor = XmlConfigurator.LoadRealSubjectTypes(resource)
                    .First(descriptor => descriptor.RealSubjectType == typeof(System.IO.File));
                Assert.That(typeDescriptor.ReturnTypeOverrides.Count, Is.EqualTo(2));

                Assert.That(typeDescriptor.ReturnTypeOverrides.ContainsKey(typeof(System.IO.FileStream)));
                Assert.That(typeDescriptor.ReturnTypeOverrides[typeof(System.IO.FileStream)], Is.EqualTo(typeof(System.IO.Stream)));

                Assert.That(typeDescriptor.ReturnTypeOverrides.ContainsKey(typeof(System.IO.StreamReader)));
                Assert.That(typeDescriptor.ReturnTypeOverrides[typeof(System.IO.StreamReader)], Is.EqualTo(typeof(System.IO.TextReader)));
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream contains invalid return type overrides.
        /// </summary>
        [Test]
        public void LoadRealSubjectType_InvalidReturnTypeOverride()
        {
            using (Stream resource = GetEmbeddedResource("InvalidReturnTypeOverride.xml"))
            {
                TypeDescriptor typeDescriptor = XmlConfigurator.LoadRealSubjectTypes(resource)
                    .First(descriptor => descriptor.RealSubjectType == typeof(System.IO.File));
                Assert.That(typeDescriptor.ReturnTypeOverrides.Count, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadRealSubjectTypes method when
        /// the configuration stream contains an ambiguous return type override.
        /// </summary>
        [Test]
        public void LoadRealSubjectType_AmbiguousReturnTypeOverride()
        {
            using (Stream resource = GetEmbeddedResource("AmbiguousReturnTypeOverride.xml"))
            {
                TypeDescriptor typeDescriptor = XmlConfigurator.LoadRealSubjectTypes(resource)
                    .First(descriptor => descriptor.RealSubjectType == typeof(System.IO.File));
                Assert.That(typeDescriptor.ReturnTypeOverrides.Count, Is.EqualTo(1));

                Assert.That(typeDescriptor.ReturnTypeOverrides.ContainsKey(typeof(System.IO.FileStream)));
                Assert.That(typeDescriptor.ReturnTypeOverrides[typeof(System.IO.FileStream)], Is.EqualTo(typeof(System.IO.Stream)));
            }
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Retrieves a stream that references an embedded resource in
        /// the same namespace as this file.
        /// </summary>
        /// 
        /// <param name="sResourceName">
        /// The name of the embedded resource to retrieve.
        /// </param>
        private static Stream GetEmbeddedResource(string sResourceName)
        {
            Type thisType = typeof(XmlConfiguratorTestFixture);
            return thisType.Assembly.GetManifestResourceStream(thisType, sResourceName);
        }

        #endregion
    }
}
