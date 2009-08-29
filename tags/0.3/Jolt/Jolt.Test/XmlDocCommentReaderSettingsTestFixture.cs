// ----------------------------------------------------------------------------
// XmlDocCommentReaderSettingsTestFixture.cs
//
// Contains the definition of the XmlDocCommentReaderSettingsTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/2/2009 10:58:33 PM
// ----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    // TODO: Static property tests; interrogate ElementInformation.
    [TestFixture]
    public sealed class XmlDocCommentReaderSettingsTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings();
            Assert.That(settings.DirectoryNames, Is.Empty);
        }

        /// <summary>
        /// Verifies the explicit construction of the class.
        /// </summary>
        [Test]
        public void ExplicitConstruction()
        {
            string[] expectedDirectoryNames = { @"\\server\a", @"\\server\b", @"\\server\c" };
            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(expectedDirectoryNames);

            Assert.That(
                settings.DirectoryNames.Cast<XmlDocCommentDirectoryElement>().Select(e => e.Name).ToList(),
                Is.EquivalentTo(expectedDirectoryNames));
        }

        /// <summary>
        /// Verifies the behavior of the default settings.
        /// </summary>
        [Test]
        public void Default()
        {
            string programFilesDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string windowsDirectoryName = Path.GetDirectoryName(Environment.SystemDirectory);
            string currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            string[] expectedDirectoryNames = {
                Environment.CurrentDirectory,
                Path.Combine(programFilesDirectoryName + " (x86)", @"Reference Assemblies\Microsoft\Framework\3.5"),
                Path.Combine(programFilesDirectoryName, @"Reference Assemblies\Microsoft\Framework\3.5"),
                Path.Combine(programFilesDirectoryName + " (x86)", @"Reference Assemblies\Microsoft\Framework\3.0\" + currentCulture),
                Path.Combine(programFilesDirectoryName, @"Reference Assemblies\Microsoft\Framework\3.0\" + currentCulture),
                Path.Combine(windowsDirectoryName, @"Microsoft.NET\Framework\v2.0.50727\" + currentCulture),
                Path.Combine(windowsDirectoryName, @"Microsoft.NET\Framework\v1.1.4322"),
                Path.Combine(windowsDirectoryName, @"Microsoft.NET\Framework\v1.0.3705") };

            Assert.That(
                XmlDocCommentReaderSettings.Default.DirectoryNames.Cast<XmlDocCommentDirectoryElement>().Select(e => e.Name).ToArray(),
                Is.EqualTo(expectedDirectoryNames));
        }
    }
}