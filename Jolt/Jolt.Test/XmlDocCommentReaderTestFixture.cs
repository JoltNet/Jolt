// ----------------------------------------------------------------------------
// XmlDocCommentReaderTestFixture.cs
//
// Contains the definition of the XmlDocCommentReaderTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/4/2009 5:53:13 PM
// ----------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Jolt.GeneratedTypes.System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Test
{
    using CreateReadPolicyDelegate = Func<string, IXmlDocCommentReadPolicy>;


    [TestFixture]
    public sealed class XmlDocCommentReaderTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference and no configuration file is present.
        /// </summary>
        [Test]
        public void Construction_Assembly_DefaultSettings()
        {
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.EqualTo(MscorlibXml));
            Assert.That(reader.ReadPolicy, Is.InstanceOfType(typeof(DefaultXDCReadPolicy)));
            Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference that has no XML doc comments in the search
        /// path and no configuration file is present.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_Assembly_DefaultSettings_FileNotFound()
        {
            XmlDocCommentReader reader = new XmlDocCommentReader(GetType().Assembly);
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference and a configuration file is present.
        /// </summary>
        [Test]
        public void Construction_Assembly_ConfigFileSettings()
        {
            WithConfigurationFile(delegate
            {
                XmlDocCommentReader reader = new XmlDocCommentReader(typeof(Mocker).Assembly);

                Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
                Assert.That(reader.FullPath, Is.EqualTo(Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml")));
                Assert.That(reader.ReadPolicy, Is.InstanceOfType(typeof(DefaultXDCReadPolicy)));
                Assert.That(
                    reader.Settings.DirectoryNames.Cast<XmlDocCommentDirectoryElement>().Select(s => s.Name).ToArray(),
                    Is.EquivalentTo(new string[] { "." }));
            });
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference that has no XML doc comments in the search
        /// path and a configuration file is present.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_Assembly_ConfigFileSettings_FileNotFound()
        {
            WithConfigurationFile(delegate
            {
                XmlDocCommentReader reader = new XmlDocCommentReader(GetType().Assembly);
            });
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference and a configurations settings object.
        /// </summary>
        [Test]
        public void Construction_Assembly_ExplicitSettings()
        {
            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(Mocker).Assembly, settings);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.EqualTo(Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml")));
            Assert.That(reader.ReadPolicy, Is.InstanceOfType(typeof(DefaultXDCReadPolicy)));
            Assert.That(reader.Settings, Is.SameAs(settings));
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference that has no XML doc comments in the search
        /// path, and a configuration settings object is provided.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_Assembly_ExplicitSettings_FileNotFound()
        {
            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings);
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference, configurations settings object, and
        /// a read policy factory method.
        /// </summary>
        [Test]
        public void Construction_Assembly_ExplicitSettings_ReadPolicy()
        {
            With.Mocks(delegate
            {
                CreateReadPolicyDelegate createPolicy = Mocker.Current.CreateMock<CreateReadPolicyDelegate>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // The read policy is created via the factory method.
                string expectedDocCommentsFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
                Expect.Call(createPolicy(expectedDocCommentsFullPath)).Return(readPolicy);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
                XmlDocCommentReader reader = new XmlDocCommentReader(typeof(Mocker).Assembly, settings, createPolicy);

                Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
                Assert.That(reader.FullPath, Is.EqualTo(expectedDocCommentsFullPath));
                Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
                Assert.That(reader.Settings, Is.SameAs(settings));
            });
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference that has no XML doc comments in the search
        /// path, and a configuration settings object and read policy
        /// factory method are provided.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_Assembly_ExplicitSettings_ReadPolicy_FileNotFound()
        {
            With.Mocks(delegate
            {
                CreateReadPolicyDelegate createPolicy = Mocker.Current.CreateMock<CreateReadPolicyDelegate>();
                Mocker.Current.ReplayAll();

                XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
                XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings, createPolicy);
            });
        }

        /// <summary>
        /// Verifies the internal construction of the class when given
        /// an assembly reference.
        /// </summary>
        [Test]
        public void InternalConstruction_Assembly()
        {
            With.Mocks(delegate
            {
                IFile fileSystem = Mocker.Current.CreateMock<IFile>();
                CreateReadPolicyDelegate createPolicy = Mocker.Current.CreateMock<CreateReadPolicyDelegate>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations
                // Each directory in the settings object is searched
                // for the desired XML comments file.
                string[] expectedDirectoryNames = { @"C:\a", @"C:\a\b", @"C:\a\b\c", @"C:\a\b\c\d" };
                string expectedFileName = Path.GetFileName(MscorlibXml);

                for (int i = 0; i < expectedDirectoryNames.Length - 1; ++i)
                {
                    Expect.Call(fileSystem.Exists(Path.Combine(expectedDirectoryNames[i], expectedFileName)))
                        .Return(false);
                }

                // The file is located in the last directory searched.
                string expectedFullPath = Path.Combine(expectedDirectoryNames[expectedDirectoryNames.Length - 1], expectedFileName);
                Expect.Call(fileSystem.Exists(expectedFullPath)).Return(true);

                // The read policy is created via the factory method.
                Expect.Call(createPolicy(expectedFullPath)).Return(readPolicy);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(expectedDirectoryNames);
                XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings, fileSystem, createPolicy);

                Assert.That(reader.FileProxy, Is.SameAs(fileSystem));
                Assert.That(reader.FullPath, Is.EqualTo(expectedFullPath));
                Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
                Assert.That(reader.Settings, Is.SameAs(settings));
            });
        }

        /// <summary>
        /// Verifies the internal construction of the class when given
        /// an assembly reference that has no XML doc comments in the search
        /// path.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void InternalConstruction_Assembly_FileNotFound()
        {
            With.Mocks(delegate
            {
                IFile fileSystem = Mocker.Current.CreateMock<IFile>();
                CreateReadPolicyDelegate createPolicy = Mocker.Current.CreateMock<CreateReadPolicyDelegate>();

                // Expectations
                // Each directory in the settings object is searched
                // for the desired XML comments file.
                string[] expectedDirectoryNames = { @"C:\a", @"C:\a\b", @"C:\a\b\c", @"C:\a\b\c\d" };
                string expectedFileName = Path.GetFileName(MscorlibXml);

                for (int i = 0; i < expectedDirectoryNames.Length; ++i)
                {
                    Expect.Call(fileSystem.Exists(Path.Combine(expectedDirectoryNames[i], expectedFileName)))
                        .Return(false);
                }

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(expectedDirectoryNames);
                XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings, fileSystem, createPolicy);
            });
        }

        /// <summary>
        /// Verifies the construction of the class when given the full path
        /// to an XML doc comments file.
        /// </summary>
        [Test]
        public void Construction_FullPath()
        {
            string expectedFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
            XmlDocCommentReader reader = new XmlDocCommentReader(expectedFullPath);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.SameAs(expectedFullPath));
            Assert.That(reader.ReadPolicy, Is.InstanceOfType(typeof(DefaultXDCReadPolicy)));
            Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));
        }

        /// <summary>
        /// Verifies the construction of the class when given the full path
        /// to a non-existent XML doc comments file.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_FullPath_FileNotFound()
        {
            XmlDocCommentReader reader = new XmlDocCommentReader(Path.GetRandomFileName());
        }

        /// <summary>
        /// Verifies the construction of the class when given the full paht
        /// to an XML doc comments file, and a read policy factory method.
        /// </summary>
        [Test]
        public void Construction_FullPath_ReadPolicy()
        {
            With.Mocks(delegate
            {
                CreateReadPolicyDelegate createPolicy = Mocker.Current.CreateMock<CreateReadPolicyDelegate>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // The read policy is created via the factory method.
                string expectedDocCommentsFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
                Expect.Call(createPolicy(expectedDocCommentsFullPath)).Return(readPolicy);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(expectedDocCommentsFullPath, createPolicy);

                Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
                Assert.That(reader.FullPath, Is.SameAs(expectedDocCommentsFullPath));
                Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
                Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));
            });
        }

        /// <summary>
        /// Verifies the internal construction of the class when given the
        /// full path to an XML doc comments file.
        /// </summary>
        [Test]
        public void InternalConstruction_FullPath()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations
                // XML doc comments file is checked for existence.
                string expectedFullPath = Path.GetRandomFileName();
                Expect.Call(fileProxy.Exists(expectedFullPath)).Return(true);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(expectedFullPath, fileProxy, readPolicy);

                Assert.That(reader.FileProxy, Is.SameAs(fileProxy));
                Assert.That(reader.FullPath, Is.SameAs(expectedFullPath));
                Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
                Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));
            });
        }

        /// <summary>
        /// Verifies the internal construction of the class when given the
        /// full path to a non-existent XML doc comments file.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void InternalConstruction_FullPath_FileNotFound()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations
                // XML doc comments file is checked for existence.
                string expectedFullPath = Path.GetRandomFileName();
                Expect.Call(fileProxy.Exists(expectedFullPath)).Return(false);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(expectedFullPath, fileProxy, readPolicy);
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given a Type.
        /// </summary>
        [Test]
        public void GetComments_Type()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                Type expectedType = GetType();
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedType))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedType), Is.SameAs(expectedComments));
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an EventInfo.
        /// </summary>
        [Test]
        public void GetComments_Event()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                EventInfo expectedEvent = typeof(Console).GetEvent("CancelKeyPress");
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedEvent))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedEvent), Is.SameAs(expectedComments));
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an FieldInfo.
        /// </summary>
        [Test]
        public void GetComments_Field()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                FieldInfo expectedField = typeof(Int32).GetField("MaxValue", BindingFlags.Public | BindingFlags.Static);
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedField))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedField), Is.SameAs(expectedComments));
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an PropertyInfo.
        /// </summary>
        [Test]
        public void GetComments_Property()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                PropertyInfo expectedProperty = typeof(Array).GetProperty("Length");
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedProperty))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedProperty), Is.SameAs(expectedComments));
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an ConstructorInfo.
        /// </summary>
        [Test]
        public void GetComments_Constructor()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                ConstructorInfo expectedConstructor = GetType().GetConstructor(Type.EmptyTypes);
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedConstructor))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedConstructor), Is.SameAs(expectedComments));
            });
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an MethodInfo.
        /// </summary>
        [Test]
        public void GetComments_Method()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                IXmlDocCommentReadPolicy readPolicy = Mocker.Current.CreateMock<IXmlDocCommentReadPolicy>();

                // Expectations.
                // Comments are retrieved for a given entity.
                MethodInfo expectedMethod = MethodInfo.GetCurrentMethod() as MethodInfo;
                XElement expectedComments = new XElement("comments");

                Expect.Call(fileProxy.Exists(String.Empty)).Return(true);
                Expect.Call(readPolicy.ReadMember(Convert.ToXmlDocCommentMember(expectedMethod))).Return(expectedComments);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
                Assert.That(reader.GetComments(expectedMethod), Is.SameAs(expectedComments));
            });
        }

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Initializes and loads the application configuration file
        /// "Constrution_ConfigurationFileSettings.config", then executes a
        /// given method prior to reverting the configuration changes.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to invoke while the configuration file is loaded and active.
        /// </param>
        private static void WithConfigurationFile(Action method)
        {
            // TODO: Share this code with ProxyAssemblyBuilderTextFixture.cs.
            try
            {
                // Load the assembly configuration.
                File.Copy(ConfigFileName, AssemblyConfigFileName);
                ConfigurationManager.RefreshSection(SettingsSection);

                method();
            }
            finally
            {
                // Revert the assembly configuration.
                File.Delete(AssemblyConfigFileName);
                ConfigurationManager.RefreshSection(SettingsSection);
            }
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly string SettingsSection = "XmlDocCommentsReader";
        private static readonly string AssemblyConfigFileName = Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
        private static readonly string ConfigFileName = "Construction_ConfigFileSettings.config";

        // TODO: This needs to be resistant to changes in the location/version of mscorlib.
        private static readonly string MscorlibXml = Path.Combine(Path.Combine(
            Path.GetDirectoryName(Environment.SystemDirectory),
            @"Microsoft.NET\Framework\v2.0.50727\" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
            "mscorlib.xml");

        #endregion
    }
}