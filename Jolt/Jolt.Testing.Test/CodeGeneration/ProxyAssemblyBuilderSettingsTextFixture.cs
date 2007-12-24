// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderSettingsTextFixture.cs
//
// Contains the definition of the ProxyAssemblyBuilderSettingsTextFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 10/26/2007 20:20:13
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.CodeGeneration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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

            ProxyAssemblyBuilderSettings settings = new ProxyAssemblyBuilderSettings(expectedStaticsSetting, expectedMethodSetting, expectedPropertiesSetting, expectedEventSetting);

            Assert.That(settings.EmitMethods, Is.EqualTo(expectedMethodSetting));
            Assert.That(settings.EmitProperties, Is.EqualTo(expectedPropertiesSetting));
            Assert.That(settings.EmitEvents, Is.EqualTo(expectedEventSetting));
            Assert.That(settings.EmitStatics, Is.EqualTo(expectedStaticsSetting));
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
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly Random RandomNumbers = new Random();

        #endregion
    }
}
