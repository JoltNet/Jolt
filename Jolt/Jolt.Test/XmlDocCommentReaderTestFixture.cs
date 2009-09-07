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
        /// assembly reference and a read policy factory method.
        /// </summary>
        [Test]
        public void Construction_Assembly_ReadPolicy()
        {
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateMock<CreateReadPolicyDelegate>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            string expectedDocCommentsFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
            createPolicy.Expect(cp => cp(expectedDocCommentsFullPath)).Return(readPolicy);
            
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(Mocker).Assembly, createPolicy);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.EqualTo(Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml")));
            Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
            Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));

            createPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference that has no XML doc comments in the search
        /// path, and a read policy factory method.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Construction_Assembly_ReadPolicy_FileNotFound()
        {
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateStub<CreateReadPolicyDelegate>();
            new XmlDocCommentReader(GetType().Assembly, createPolicy);
        }

        /// <summary>
        /// Verifies the construction of the class when given an
        /// assembly reference, configurations settings object, and
        /// a read policy factory method.
        /// </summary>
        [Test]
        public void Construction_Assembly_ExplicitSettings_ReadPolicy()
        {
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateMock<CreateReadPolicyDelegate>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            // Expectations.
            // The read policy is created via the factory method.
            string expectedDocCommentsFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
            createPolicy.Expect(cp => cp(expectedDocCommentsFullPath)).Return(readPolicy);

            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(Mocker).Assembly, settings, createPolicy);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.EqualTo(expectedDocCommentsFullPath));
            Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
            Assert.That(reader.Settings, Is.SameAs(settings));

            createPolicy.VerifyAllExpectations();
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
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateStub<CreateReadPolicyDelegate>();

            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(new string[] { Environment.CurrentDirectory });
            new XmlDocCommentReader(typeof(int).Assembly, settings, createPolicy);
        }

        /// <summary>
        /// Verifies the internal construction of the class when given
        /// an assembly reference.
        /// </summary>
        [Test]
        public void InternalConstruction_Assembly()
        {
            IFile fileSystem = MockRepository.GenerateMock<IFile>();
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateMock<CreateReadPolicyDelegate>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            string[] expectedDirectoryNames = { @"C:\a", @"C:\a\b", @"C:\a\b\c", @"C:\a\b\c\d" };
            string expectedFileName = Path.GetFileName(MscorlibXml);
            for (int i = 0; i < expectedDirectoryNames.Length - 1; ++i)
            {
                fileSystem.Expect(fs => fs.Exists(Path.Combine(expectedDirectoryNames[i], expectedFileName)))
                    .Return(false);
            }

            string expectedFullPath = Path.Combine(expectedDirectoryNames[expectedDirectoryNames.Length - 1], expectedFileName);
            fileSystem.Expect(fs => fs.Exists(expectedFullPath)).Return(true);
            createPolicy.Expect(cp => cp(expectedFullPath)).Return(readPolicy);

            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(expectedDirectoryNames);
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings, fileSystem, createPolicy);

            Assert.That(reader.FileProxy, Is.SameAs(fileSystem));
            Assert.That(reader.FullPath, Is.EqualTo(expectedFullPath));
            Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
            Assert.That(reader.Settings, Is.SameAs(settings));

            fileSystem.VerifyAllExpectations();
            createPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the internal construction of the class when given
        /// an assembly reference that has no XML doc comments in the search
        /// path.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void InternalConstruction_Assembly_FileNotFound()
        {
            IFile fileSystem = MockRepository.GenerateMock<IFile>();
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateStub<CreateReadPolicyDelegate>();

            string[] expectedDirectoryNames = { @"C:\a", @"C:\a\b", @"C:\a\b\c", @"C:\a\b\c\d" };
            string expectedFileName = Path.GetFileName(MscorlibXml);
            for (int i = 0; i < expectedDirectoryNames.Length; ++i)
            {
                fileSystem.Expect(fs => fs.Exists(Path.Combine(expectedDirectoryNames[i], expectedFileName)))
                    .Return(false);
            }

            XmlDocCommentReaderSettings settings = new XmlDocCommentReaderSettings(expectedDirectoryNames);
            XmlDocCommentReader reader = new XmlDocCommentReader(typeof(int).Assembly, settings, fileSystem, createPolicy);

            fileSystem.VerifyAllExpectations();
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
            CreateReadPolicyDelegate createPolicy = MockRepository.GenerateMock<CreateReadPolicyDelegate>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            string expectedDocCommentsFullPath = Path.Combine(Environment.CurrentDirectory, "Rhino.Mocks.xml");
            createPolicy.Expect(cp => cp(expectedDocCommentsFullPath)).Return(readPolicy);

            XmlDocCommentReader reader = new XmlDocCommentReader(expectedDocCommentsFullPath, createPolicy);

            Assert.That(reader.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            Assert.That(reader.FullPath, Is.SameAs(expectedDocCommentsFullPath));
            Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
            Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));

            createPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the internal construction of the class when given the
        /// full path to an XML doc comments file.
        /// </summary>
        [Test]
        public void InternalConstruction_FullPath()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            string expectedFullPath = Path.GetRandomFileName();
            fileProxy.Expect(fp => fp.Exists(expectedFullPath)).Return(true);

            XmlDocCommentReader reader = new XmlDocCommentReader(expectedFullPath, fileProxy, readPolicy);

            Assert.That(reader.FileProxy, Is.SameAs(fileProxy));
            Assert.That(reader.FullPath, Is.SameAs(expectedFullPath));
            Assert.That(reader.ReadPolicy, Is.SameAs(readPolicy));
            Assert.That(reader.Settings, Is.SameAs(XmlDocCommentReaderSettings.Default));

            fileProxy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the internal construction of the class when given the
        /// full path to a non-existent XML doc comments file.
        /// </summary>
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void InternalConstruction_FullPath_FileNotFound()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateStub<IXmlDocCommentReadPolicy>();

            string expectedFullPath = Path.GetRandomFileName();
            fileProxy.Expect(fp => fp.Exists(expectedFullPath)).Return(false);

            XmlDocCommentReader reader = new XmlDocCommentReader(expectedFullPath, fileProxy, readPolicy);

            fileProxy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given a Type.
        /// </summary>
        [Test]
        public void GetComments_Type()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            Type expectedType = GetType();
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedType))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedType), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an EventInfo.
        /// </summary>
        [Test]
        public void GetComments_Event()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            EventInfo expectedEvent = typeof(Console).GetEvent("CancelKeyPress");
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedEvent))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedEvent), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an FieldInfo.
        /// </summary>
        [Test]
        public void GetComments_Field()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            FieldInfo expectedField = typeof(Int32).GetField("MaxValue", BindingFlags.Public | BindingFlags.Static);
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedField))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedField), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an PropertyInfo.
        /// </summary>
        [Test]
        public void GetComments_Property()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            PropertyInfo expectedProperty = typeof(Array).GetProperty("Length");
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedProperty))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedProperty), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an ConstructorInfo.
        /// </summary>
        [Test]
        public void GetComments_Constructor()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            ConstructorInfo expectedConstructor = GetType().GetConstructor(Type.EmptyTypes);
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedConstructor))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedConstructor), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the GetComments method when given an MethodInfo.
        /// </summary>
        [Test]
        public void GetComments_Method()
        {
            IFile fileProxy = MockRepository.GenerateMock<IFile>();
            IXmlDocCommentReadPolicy readPolicy = MockRepository.GenerateMock<IXmlDocCommentReadPolicy>();

            MethodInfo expectedMethod = MethodInfo.GetCurrentMethod() as MethodInfo;
            XElement expectedComments = new XElement("comments");

            fileProxy.Expect(fp => fp.Exists(String.Empty)).Return(true);
            readPolicy.Expect(rp => rp.ReadMember(Convert.ToXmlDocCommentMember(expectedMethod))).Return(expectedComments);

            XmlDocCommentReader reader = new XmlDocCommentReader(String.Empty, fileProxy, readPolicy);
            Assert.That(reader.GetComments(expectedMethod), Is.SameAs(expectedComments));

            fileProxy.VerifyAllExpectations();
            readPolicy.VerifyAllExpectations();
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