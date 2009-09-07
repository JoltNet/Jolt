// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderTestFixture.cs
//
// Contains the definition of the ProxyAssemblyBuilderTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 8/9/2007 23:05:51
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using log4net.Config;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    using CreateProxyTypeBuilderDelegate = Func<string, Type, bool, ModuleBuilder, IProxyTypeBuilder>;


    [TestFixture]
    public sealed class ProxyAssemblyBuilderTestFixture
    {
        #region public methods --------------------------------------------------------------------

        #region initialization --------------------------------------------------------------------

        static ProxyAssemblyBuilderTestFixture()
        {
            WorkingDirectoryName = Path.Combine(Path.GetTempPath(), MethodBase.GetCurrentMethod().DeclaringType.Name);
            PublicStaticBinding = BindingFlags.Public | BindingFlags.Static;
            PublicInstanceBinding = BindingFlags.Public | BindingFlags.Instance;

            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.ValidationType = ValidationType.Schema;

            Type schemaAssemblyType = typeof(AbstractXDCReadPolicy);
            using (Stream schema = schemaAssemblyType.Assembly.GetManifestResourceStream(schemaAssemblyType, "Xml.DocComments.xsd"))
            {
                ReaderSettings.Schemas.Add(XmlSchema.Read(schema, null));
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            BasicConfigurator.Configure();

            // Create working directory.
            Directory.CreateDirectory(WorkingDirectoryName);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            // Clean up working directory.
            Directory.Delete(WorkingDirectoryName, true);
        }

        #endregion

        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            string sExpectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string sExpectedAssemblyFileName = sExpectedAssemblyName + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(Environment.CurrentDirectory, sExpectedAssemblyFileName);
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder();

            Assert.That(builder.RootNamespace, Is.EqualTo("Jolt.Testing.CodeGeneration"));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Assembly.FullName, Is.EqualTo(sExpectedAssemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));

            Module[] assemblyModules = builder.Assembly.GetModules();
            Assert.That(assemblyModules, Is.Not.Empty);
            Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
            Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace is provided.
        /// </summary>
        [Test]
        public void Construction_Namespace()
        {
            string sExpectedNamespace = "Unit.Testing.Namespace";
            string sExpectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string sExpectedAssemblyFileName = sExpectedAssemblyName + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(Environment.CurrentDirectory, sExpectedAssemblyFileName);
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(sExpectedNamespace);

            Assert.That(builder.RootNamespace, Is.EqualTo(sExpectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Assembly.FullName, Is.EqualTo(sExpectedAssemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));

            Module[] assemblyModules = builder.Assembly.GetModules();
            Assert.That(assemblyModules, Is.Not.Empty);
            Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
            Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace and assembly full path are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_AssemblyFullPath()
        {
            string sExpectedNamespace = "Internal.Testing.CodeGeneration";
            string sExpectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string sExpectedAssemblyFileName = sExpectedAssemblyName + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), sExpectedAssemblyFileName);

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(sExpectedNamespace, sExpectedAssemblyFullPath);

            Assert.That(builder.RootNamespace, Is.EqualTo(sExpectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Assembly.FullName, Is.EqualTo(sExpectedAssemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));

            Module[] assemblyModules = builder.Assembly.GetModules();
            Assert.That(assemblyModules, Is.Not.Empty);
            Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
            Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace, assembly full path, and configuration settings are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_AssemblyFullPath_Settings()
        {
            string sExpectedNamespace = "Internal.Testing.CodeGeneration";
            string sExpectedAssemblyName = "Proxies";
            string sExpectedAssemblyFileName = sExpectedAssemblyName + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), sExpectedAssemblyFileName);
            ProxyAssemblyBuilderSettings expectedSettings = new ProxyAssemblyBuilderSettings();

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(sExpectedNamespace, sExpectedAssemblyFullPath, expectedSettings);

            Assert.That(builder.RootNamespace, Is.EqualTo(sExpectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Assembly.FullName, Is.EqualTo(sExpectedAssemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Assert.That(builder.Settings, Is.SameAs(expectedSettings));

            Module[] assemblyModules = builder.Assembly.GetModules();
            Assert.That(assemblyModules, Is.Not.Empty);
            Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
            Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
            Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verify the construction of the class when a root assembly
        /// namespace, and assembly full path are provided.  The configuration
        /// settings are retrieved from the application configuration file.
        /// </summary>
        [Test]
        public void Construction_ConfigFileSettings()
        {
            string sAssemblyConfigFileName = Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
            string sSettingsSection = "proxyBuilderSettings";

            try
            {
                // Prime the configuration file to load.
                File.Copy("Construction_ConfigFileSettings.config", sAssemblyConfigFileName);
                ConfigurationManager.RefreshSection(sSettingsSection);

                // Test
                string sExpectedNamespace = "Internal.Testing.CodeGeneration";
                string sExpectedAssemblyName = "Proxies";
                string sExpectedAssemblyFileName = sExpectedAssemblyName + ".dll";
                string sExpectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), sExpectedAssemblyFileName);

                ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(sExpectedNamespace, sExpectedAssemblyFullPath);

                Assert.That(builder.RootNamespace, Is.EqualTo(sExpectedNamespace));
                Assert.That(builder.AssemblyFullPath, Is.EqualTo(sExpectedAssemblyFullPath));
                Assert.That(builder.Assembly.FullName, Is.EqualTo(sExpectedAssemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
                Assert.That(builder.Settings.EmitMethods);
                Assert.That(builder.Settings.EmitProperties);
                Assert.That(builder.Settings.EmitEvents);
                Assert.That(!builder.Settings.EmitStatics);
                Assert.That(builder.Settings.EmitXmlDocComments);

                Module[] assemblyModules = builder.Assembly.GetModules();
                Assert.That(assemblyModules, Is.Not.Empty);
                Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
                Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
                Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));

                XElement validatedDocComments = XElement.Load(XmlReader.Create(builder.CreateXmlDocCommentReader(), ReaderSettings));
                Assert.That(validatedDocComments, Is.Not.Null);
                Assert.That(validatedDocComments.Element(XmlDocCommentNames.AssemblyElement).Element(XmlDocCommentNames.NameElement).Value, Is.EqualTo(sExpectedAssemblyName));
                Assert.That(validatedDocComments.Element(XmlDocCommentNames.MembersElement).IsEmpty);
            }
            finally
            {
                // Reset configuration state for subsequent tests.
                File.Delete(sAssemblyConfigFileName);
                ConfigurationManager.RefreshSection(sSettingsSection);
            }
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method.
        /// </summary>
        [Test]
        public void AddType()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());
            
            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress static methods.
        /// </summary>
        [Test]
        public void AddType_NoStaticMethods()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress static properties.
        /// </summary>
        [Test]
        public void AddType_NoStaticProperties()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress static events.
        /// </summary>
        [Test]
        public void AddType_NoStaticEvents()
        {
            VerifyBehavior_AddType_NoStaticEvents(
                (builder, realSubjectType) => builder.AddType(realSubjectType));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all methods.
        /// </summary>
        [Test]
        public void AddType_NoMethods()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, true, true, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all properties.
        /// </summary>
        [Test]
        public void AddType_NoProperties()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, false, true, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all events.
        /// </summary>
        [Test]
        public void AddType_NoEvents()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, true, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all members.
        /// </summary>
        [Test]
        public void AddType_NoMembers()
        {
            VerifyBehavior_AddType_NoMembers(
                (builder, realSubjectType) => builder.AddType(realSubjectType));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when more
        /// than one type is provided.
        /// </summary>
        [Test]
        public void AddType_ManyTypes()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);
            Type[] expectedSubjectTypes = { typeof(__FirstEmptySubjectType), typeof(__SecondEmptySubjectType) };

            foreach (Type expectedType in expectedSubjectTypes)
            {
                createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("GetType")));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("GetHashCode")));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("ToString")));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("Equals")));
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null).Repeat.Times(expectedSubjectTypes.Length);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader()).Repeat.Times(expectedSubjectTypes.Length);

            Array.ForEach(expectedSubjectTypes, assemblyBuilder.AddType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when
        /// the same type is added to the assembly more than once.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddType_DuplicateType()
        {
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder();
            builder.AddType(typeof(__FirstEmptySubjectType));
            builder.AddType(typeof(__FirstEmptySubjectType));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// underlying type builder throws an exception on  processing
        /// a method.
        /// </summary>
        [Test]
        public void AddType_InvalidMethod()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));
            Type expectedType = typeof(__FirstEmptySubjectType);

            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            foreach (MethodInfo method in expectedType.GetMethods(PublicInstanceBinding))
            {
                proxyTypeBuilder.Expect(pb => pb.AddMethod(method)).Throw(new InvalidOperationException());
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// underlying type builder throws an exception on processing
        /// a property.
        /// </summary>
        [Test]
        public void AddType_InvalidProperty()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));
            Type expectedType = typeof(__PropertyTestType);

            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            foreach (PropertyInfo property in expectedType.GetProperties(PublicInstanceBinding))
            {
                proxyTypeBuilder.Expect(pb => pb.AddProperty(property)).Throw(new InvalidOperationException());
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// underlying type builder throws an exception on processing
        /// an event.
        /// </summary>
        [Test]
        public void AddType_InvalidEvent()
        {
            VerifyBehavior_AddType_InvalidEvent(
                (builder, realSubjectType) => builder.AddType(realSubjectType));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when XML doc comment
        /// production is enabled.
        /// </summary>
        [Test]
        public void AddType_XmlDocCommentsEnabled()
        {
            VerifyBehavior_AddType_XmlDocCommentsEnabled(
                (builder, realSubjectType) => builder.AddType(realSubjectType));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method, when given a collection
        /// of return type overrides.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { {typeof(int), typeof(double)} };

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// static methods.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoStaticMethods()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// static properties.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoStaticProperties()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// static events.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoStaticEvents()
        {
            VerifyBehavior_AddType_NoStaticEvents(
                (builder, realSubjectType) => builder.AddType(realSubjectType, new Dictionary<Type, Type>() { { typeof(int), typeof(double) } }));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// all methods.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoMethods()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, true, true, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// all properties.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoProperties()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, false, true, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type override and the builder is configured to suppress
        /// all events.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoEvents()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, true, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetType")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("ToString")));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedSubjectType.GetMethod("Equals")));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double)));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// all members.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoMembers()
        {
            VerifyBehavior_AddType_NoMembers(
                (builder, realSubectType) => builder.AddType(realSubectType, new Dictionary<Type, Type>() { { typeof(int), typeof(double) } }));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when more
        /// than one type is provided with a collection of return type
        /// overrides.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_ManyTypes()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);
            Type[] expectedSubjectTypes = { typeof(__FirstEmptySubjectType), typeof(__SecondEmptySubjectType) };

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };
            foreach (Type expectedType in expectedSubjectTypes)
            {
                createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("GetType")));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("GetHashCode"), typeof(double)));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("ToString")));
                proxyTypeBuilder.Expect(pb => pb.AddMethod(expectedType.GetMethod("Equals")));
            }
             
            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null).Repeat.Times(expectedSubjectTypes.Length);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader()).Repeat.Times(expectedSubjectTypes.Length);

            foreach (Type expectedType in expectedSubjectTypes)
            {
                assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);
            }

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the same type is added to the assembly
        /// more than once.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddType_ReturnTypeOverride_DuplicateType()
        {
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder();
            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } }; 
            
            builder.AddType(typeof(__FirstEmptySubjectType), desiredReturnTypeOverrides);
            builder.AddType(typeof(__FirstEmptySubjectType), desiredReturnTypeOverrides);
        }

                /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the underlying type builder throws an
        /// exception on processing a method.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_InvalidMethod()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));
            Type expectedType = typeof(__FirstEmptySubjectType);

            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } }; 
            foreach (MethodInfo method in expectedType.GetMethods(PublicInstanceBinding))
            {
                if (method.ReturnType == typeof(int))
                {
                    proxyTypeBuilder.Expect(pb => pb.AddMethod(method, typeof(double))).Throw(new InvalidOperationException());
                }
                else
                {
                    proxyTypeBuilder.Expect(pb => pb.AddMethod(method)).Throw(new InvalidOperationException());
                }
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the underlying type builder throws an
        /// exception on processing a property.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_InvalidProperty()
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));
            Type expectedType = typeof(__PropertyTestType);

            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(string), typeof(object) } }; 
            foreach (PropertyInfo property in expectedType.GetProperties(PublicInstanceBinding))
            {
                if (property.PropertyType == typeof(string))
                {
                    proxyTypeBuilder.Expect(pb => pb.AddProperty(property, typeof(object))).Throw(new InvalidOperationException());
                }
                else
                {
                    proxyTypeBuilder.Expect(pb => pb.AddProperty(property)).Throw(new InvalidOperationException());
                }
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overides and the underlying type builder throws an
        /// exception on processing an event.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_InvalidEvent()
        {
            VerifyBehavior_AddType_InvalidEvent(
                (builder, realSubjectType) => builder.AddType(realSubjectType, new Dictionary<Type, Type>() { { typeof(int), typeof(int) } }));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and XML doc comment production is enabled.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_XmlDocCommentsEnabled()
        {
            VerifyBehavior_AddType_XmlDocCommentsEnabled(
                (builder, realSubjectType) => builder.AddType(realSubjectType, new Dictionary<Type, Type>() { { typeof(int), typeof(int) } }));
        }

        /// <summary>
        /// Verifies the behavior of the CreateAssembly() method.
        /// </summary>
        [Test]
        public void CreateAssembly()
        {
            VerifyBehavior_CreateAssemblyBuilder(true, true);
        }

        /// <summary>
        /// Verifies the behavior of the CreateAssembly() method when
        /// there are no XML doc comments to emit.
        /// </summary>
        [Test]
        public void CreateAssembly_NoXmlDocComments()
        {
            VerifyBehavior_CreateAssemblyBuilder(false, true);
        }

        /// <summary>
        /// Verifies the behavior of the CreateAssembly() method when
        /// XML doc comment production is disabled.
        /// </summary>
        [Test]
        public void CreateAssembly_XmlDocCommentsDisabled()
        {
            VerifyBehavior_CreateAssemblyBuilder(false, false);
        }

        /// <summary>
        /// Verifies the implementation of the CreateXmlCocCommentReader() method.
        /// </summary>
        [Test]
        public void CreateXmlDocCommentReader()
        {
            string sAssemblyFileName = Path.GetRandomFileName() + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(WorkingDirectoryName, sAssemblyFileName);

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(
                "Unit.Testing.Namespace",
                sExpectedAssemblyFullPath,
                new ProxyAssemblyBuilderSettings(false, false, false, false, true));

            XElement xmlDocComments = XElement.Load(XmlReader.Create(builder.CreateXmlDocCommentReader(), ReaderSettings));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the CreateAssembly() method.
        /// </summary>
        /// 
        /// <param name="expectXmlDocComments">
        /// Determines if an XML doc comment file is expected to be created.
        /// </param>
        /// 
        /// <param name="emitComments">
        /// Determines if XML doc comments should be created.
        /// </param>
        private static void VerifyBehavior_CreateAssemblyBuilder(bool expectXmlDocComments, bool emitComments)
        {
            string sAssemblyFileName = Path.GetRandomFileName() + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(WorkingDirectoryName, sAssemblyFileName);

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(
                "Unit.Testing.Namespace",
                sExpectedAssemblyFullPath,
                new ProxyAssemblyBuilderSettings(false, false, false, false, emitComments));

            if (expectXmlDocComments) { builder.AddType(typeof(Console)); }
            builder.CreateAssembly();

            Assert.That(File.Exists(sExpectedAssemblyFullPath));
            Assert.That(File.Exists(Path.ChangeExtension(sExpectedAssemblyFullPath, "pdb")));
            Assert.That(File.Exists(Path.ChangeExtension(sExpectedAssemblyFullPath, "xml")), Is.EqualTo(expectXmlDocComments));
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method overloads, when
        /// XML doc comment production is enabled.
        /// </summary>
        /// 
        /// <param name="addType">
        /// A delegate that invokes the appropriate AddType() overload, given
        /// the proxy assembly builder and expected real subject type parameter.
        /// </param>
        private static void VerifyBehavior_AddType_XmlDocCommentsEnabled(Action<ProxyAssemblyBuilder, Type> addType)
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, false, true));

            Type expectedSubjectType = typeof(__FirstEmptySubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateTestDocCommentReader());

            addType(assemblyBuilder, expectedSubjectType);

            XElement memberElement = XElement.Load(XmlReader.Create(assemblyBuilder.CreateXmlDocCommentReader(), ReaderSettings))
                .Element(XmlDocCommentNames.MembersElement)
                .Element(XmlDocCommentNames.MemberElement);

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute(XmlDocCommentNames.NameAttribute).Value, Is.EqualTo("member-name"));
            Assert.That(memberElement.IsEmpty);
            Assert.That(memberElement.ElementsAfterSelf().Count(), Is.EqualTo(0));

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method overloads, when
        /// the builder is configured to suppress static events.
        /// </summary>
        /// 
        /// <param name="addType">
        /// A delegate that invokes the appropriate AddType() overload, given
        /// the proxy assembly builder and expected real subject type parameter.
        /// </param>
        private static void VerifyBehavior_AddType_NoStaticEvents(Action<ProxyAssemblyBuilder, Type> addType)
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, true, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(cb => cb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1")));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2")));

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            addType(assemblyBuilder, expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method overloads, when
        /// the builder is configured to suppress all members.
        /// </summary>
        /// 
        /// <param name="addType">
        /// A delegate that invokes the appropriate AddType() overload, given
        /// the proxy assembly builder and expected real subject type parameter.
        /// </param>
        private static void VerifyBehavior_AddType_NoMembers(Action<ProxyAssemblyBuilder, Type> addType)
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, false, false, false));

            Type expectedSubjectType = typeof(__RealSubjectType);
            createProxyTypeBuilder.Expect(pb => pb(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            addType(assemblyBuilder, expectedSubjectType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method overloads when
        /// the underlying type builder throws an exception on processing
        /// an event.
        /// </summary>
        /// 
        /// <param name="addType">
        /// A delegate that invokes the appropriate AddType() overload, given
        /// the proxy assembly builder and expected real subject type parameter.
        /// </param>
        private static void VerifyBehavior_AddType_InvalidEvent(Action<ProxyAssemblyBuilder, Type> addType)
        {
            CreateProxyTypeBuilderDelegate createProxyTypeBuilder = MockRepository.GenerateMock<CreateProxyTypeBuilderDelegate>();
            IProxyTypeBuilder proxyTypeBuilder = MockRepository.GenerateMock<IProxyTypeBuilder>();

            ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, true, false));
            Type expectedType = typeof(__EventTestType);

            createProxyTypeBuilder.Expect(pb => pb(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                .Return(proxyTypeBuilder);

            foreach (EventInfo evt in expectedType.GetEvents(PublicInstanceBinding))
            {
                proxyTypeBuilder.Expect(pb => pb.AddEvent(evt)).Throw(new InvalidOperationException());
            }

            proxyTypeBuilder.Expect(pb => pb.CreateProxy()).Return(null);

            proxyTypeBuilder.Expect(pb => pb.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

            addType(assemblyBuilder, expectedType);

            createProxyTypeBuilder.VerifyAllExpectations();
            proxyTypeBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Creates an XmlReader capable of reading a simple XML
        /// doc comment member.
        /// </summary>
        private static XmlReader CreateTestDocCommentReader()
        {
            return new XElement(XmlDocCommentNames.MemberElement, new XAttribute(XmlDocCommentNames.NameAttribute, "member-name")).CreateReader();
        }

        /// <summary>
        /// Creates an XmlReader that reads no XML doc comments.
        /// </summary>
        /// <returns></returns>
        private static XmlReader CreateEmptyDocCommentReader()
        {
            return new XDocument().CreateReader();
        }

        /// <summary>
        /// Constructs a test ProxyAssemblyBuilder with benign values and a
        /// given ProxyTypeBuilder factory method.
        /// </summary>
        /// 
        /// <param name="createProxyTypeBuilder">
        /// The ProxyTypeBuilder factory method.
        /// </param>
        private static ProxyAssemblyBuilder CreateTestAssemblyBuilder(CreateProxyTypeBuilderDelegate createProxyTypeBuilder)
        {
            return CreateTestAssemblyBuilder(createProxyTypeBuilder, ProxyAssemblyBuilderSettings.Default);
        }

        /// <summary>
        /// Constructs a test ProxyAssemblyBuilder with benign values and a
        /// given ProxyTypeBuilder factory method.
        /// </summary>
        /// 
        /// <param name="createProxyTypeBuilder">
        /// The ProxyTypeBuilder factory method.
        /// </param>
        private static ProxyAssemblyBuilder CreateTestAssemblyBuilder(CreateProxyTypeBuilderDelegate createProxyTypeBuilder, ProxyAssemblyBuilderSettings settings)
        {
            return new ProxyAssemblyBuilder("a.b.c", "Proxies.dll", settings, createProxyTypeBuilder);
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly string WorkingDirectoryName;
        private static readonly BindingFlags PublicStaticBinding;
        private static readonly BindingFlags PublicInstanceBinding;
        private static readonly XmlReaderSettings ReaderSettings;

        #endregion
    }
}