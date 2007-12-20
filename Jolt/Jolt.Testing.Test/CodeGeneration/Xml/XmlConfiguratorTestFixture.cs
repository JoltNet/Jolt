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
using System.Xml;
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
        /// Verifies the behavior of the LoadAdapteeTypes method.
        /// </summary>
        [Test]
        public void LoadAdapteeTypes()
        {
            Type[] expectedTypes = {typeof(string), typeof(System.IO.File), typeof(System.Text.Decoder)};
            using (Stream resource = GetEmbeddedResource("ValidConfiguration.xml"))
            {
                foreach (Type loadedType in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.That(expectedTypes, List.Contains(loadedType));
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadAdapteeTypes method when
        /// the configuration stream contains invalid types.
        /// </summary>
        [Test]
        public void LoadAdapteeTypes_InvalidTypes()
        {
            Type[] expectedTypes = { typeof(string), typeof(System.IO.File), typeof(System.Text.Decoder) };
            using (Stream resource = GetEmbeddedResource("ContainsInvalidTypes.xml"))
            {
                foreach (Type loadedType in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.That(expectedTypes, List.Contains(loadedType));
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadAdapteeTypes method when
        /// the configuration stream is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(XmlSchemaValidationException))]
        public void LoadAdapteeTypes_InvalidConfigurationStream()
        {
            using (Stream resource = GetEmbeddedResource("InvalidConfiguration.xml"))
            {
                foreach (Type loadedType in XmlConfigurator.LoadRealSubjectTypes(resource)) { };
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadAdapteeTypes method when
        /// the configuration stream contains duplicate types.
        /// </summary>
        [Test, ExpectedException(typeof(XmlSchemaValidationException))]
        public void LoadAdapteeTypes_DuplicateTypesInStream()
        {
            using (Stream resource = GetEmbeddedResource("DuplicateTypesConfiguration.xml"))
            {
                foreach (Type loadedType in XmlConfigurator.LoadRealSubjectTypes(resource)) { }
                
            }
        }

        /// <summary>
        /// Verifies the behavior of the LoadAdapteeTypes method when
        /// the configuration stream is invalid.
        /// </summary>
        [Test]
        public void LoadAdapteeTypes_EmptyConfigurationStream()
        {
            using (Stream resource = GetEmbeddedResource("EmptyConfiguration.xml"))
            {
                foreach (Type loadedType in XmlConfigurator.LoadRealSubjectTypes(resource))
                {
                    Assert.Fail();
                }
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
