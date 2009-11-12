// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderSettingsTextFixture.cs
//
// Contains the definition of the ProxyAssemblyBuilderSettingsTextFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 10/26/2007 20:20:13
// ----------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Text;

using Jolt.Testing.CodeGeneration;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class ProxyAssemblyBuilderSettingsTextFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            ProxyAssemblyBuilderSettings settings = new ProxyAssemblyBuilderSettings();

            Assert.That(settings.EmitMethods);
            Assert.That(settings.EmitProperties);
            Assert.That(settings.EmitStatics);
            Assert.That(settings.EmitEvents);
            Assert.That(!settings.EmitXmlDocComments);
        }

        /// <summary>
        /// Verifies the explicit construction of the class.
        /// </summary>
        [Test]
        public void ExplicitConstruction()
        {
            bool expectedMethodSetting = RandomNumbers.Next(0, 1) == 0;
            bool expectedPropertiesSetting = RandomNumbers.Next(0, 1) == 0;
            bool expectedEventSetting = RandomNumbers.Next(0, 1) == 0;
            bool expectedStaticsSetting = RandomNumbers.Next(0, 1) == 0;
            bool expectedXmlDocCommentsSetting = RandomNumbers.Next(0, 1) == 0;

            ProxyAssemblyBuilderSettings settings
                = new ProxyAssemblyBuilderSettings(expectedStaticsSetting, expectedMethodSetting, expectedPropertiesSetting, expectedEventSetting, expectedXmlDocCommentsSetting);

            Assert.That(settings.EmitMethods, Is.EqualTo(expectedMethodSetting));
            Assert.That(settings.EmitProperties, Is.EqualTo(expectedPropertiesSetting));
            Assert.That(settings.EmitEvents, Is.EqualTo(expectedEventSetting));
            Assert.That(settings.EmitStatics, Is.EqualTo(expectedStaticsSetting));
            Assert.That(settings.EmitXmlDocComments, Is.EqualTo(expectedXmlDocCommentsSetting));
        }

        /// <summary>
        /// Verifies the initialization of the default settings field.
        /// </summary>
        [Test]
        public void DefaultSettings()
        {
            Assert.That(ProxyAssemblyBuilderSettings.Default.EmitMethods);
            Assert.That(ProxyAssemblyBuilderSettings.Default.EmitProperties);
            Assert.That(ProxyAssemblyBuilderSettings.Default.EmitEvents);
            Assert.That(ProxyAssemblyBuilderSettings.Default.EmitStatics);
            Assert.That(!ProxyAssemblyBuilderSettings.Default.EmitXmlDocComments);
        }

        /// <summary>
        /// Verifies the static configuration of the EmitStatics property.
        /// </summary>
        [Test]
        public void EmitStatics_Configuration()
        {
            AssertStaticConfiguration_EmitProperty("EmitStatics", false, true);
        }

        /// <summary>
        /// Verifies the static configuration of the EmitMethods property.
        /// </summary>
        [Test]
        public void EmitMethods_Configuration()
        {
            AssertStaticConfiguration_EmitProperty("EmitMethods", false, true);
        }

        /// <summary>
        /// Verifies the static configuration of the EmitProperties property.
        /// </summary>
        [Test]
        public void EmitProperties_Configuration()
        {
            AssertStaticConfiguration_EmitProperty("EmitProperties", false, true);
        }

        /// <summary>
        /// Verifies the static configuration of the EmitEvents property.
        /// </summary>
        [Test]
        public void EmitEvents_Configuration()
        {
            AssertStaticConfiguration_EmitProperty("EmitEvents", false, true);
        }

        /// <summary>
        /// Verifies the static configuration of the EmitXmlDocComments property.
        /// </summary>
        [Test]
        public void EmitXmlDocComments_Configuration()
        {
            AssertStaticConfiguration_EmitProperty("EmitXmlDocComments", false, false);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the static configuration of a given property from the
        /// <seealso cref="ProxyAssemblyBuilderSettings"/> type.
        /// </summary>
        /// 
        /// <param name="propertyName">
        /// The name of the property to validate.
        /// </param>
        /// 
        /// <param name="isRequired">
        /// The expected requirement status of the configuration property.
        /// </param>
        /// 
        /// <param name="expectedDefaultValue">
        /// The expected default value of the property.
        /// </param>
        private void AssertStaticConfiguration_EmitProperty(string propertyName, bool isRequired, bool expectedDefaultValue)
        {
            StringBuilder builder = new StringBuilder(propertyName);
            builder[0] = Char.ToLower(builder[0]);

            Assert.That(
                typeof(ProxyAssemblyBuilderSettings).GetProperty(propertyName),
                Has.Attribute<ConfigurationPropertyAttribute>()
                    .With.Property("Name").EqualTo(builder.ToString())
                    .And.Property("IsRequired").EqualTo(isRequired)
                    .And.Property("DefaultValue").EqualTo(expectedDefaultValue));
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly Random RandomNumbers = new Random();

        #endregion
    }
}
