// ----------------------------------------------------------------------------
// ProxyAssemblyBuilderTestFixture.cs
//
// Contains the definition of the ProxyAssemblyBuilderTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 8/9/2007 23:05:51
// ----------------------------------------------------------------------------

using System;
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
    using System.Collections.Generic;


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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);

                // Expectations 
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method and property
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();
                
                assemblyBuilder.AddType(expectedSubjectType);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress static methods.
        /// </summary>
        [Test]
        public void AddType_NoStaticMethods()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress static properties.
        /// </summary>
        [Test]
        public void AddType_NoStaticProperties()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property and method
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, true, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all properties.
        /// </summary>
        [Test]
        public void AddType_NoProperties()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, false, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// builder is configured to suppress all events.
        /// </summary>
        [Test]
        public void AddType_NoEvents()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, true, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);
                Type[] expectedSubjectTypes = { typeof(__FirstEmptySubjectType), typeof(__SecondEmptySubjectType) };

                // Expectations
                foreach (Type expectedType in expectedSubjectTypes)
                {
                    // The proxy type builder for each type is created.
                    Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                        .Return(proxyTypeBuilder);

                    // The proxy builder is invoked for each public method
                    // on the first subject type.
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("GetType"));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("GetHashCode"));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("ToString"));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("Equals"));

                    // The proxy type and its interface are created.
                    Expect.Call(proxyTypeBuilder.CreateProxy())
                        .Return(null);

                    // The XML doc comments for the proxy and interface type are retrieved.
                    Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());
                }

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                Array.ForEach(expectedSubjectTypes, assemblyBuilder.AddType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));
                Type expectedType = typeof(__FirstEmptySubjectType);

                // Expectations
                // The proxy type builder is created.
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the subject type, each call raising an exception.
                foreach (MethodInfo method in expectedType.GetMethods(PublicInstanceBinding))
                {
                    proxyTypeBuilder.AddMethod(method);
                    LastCall.Throw(new InvalidOperationException());
                }

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedType);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when the
        /// underlying type builder throws an exception on processing
        /// a property.
        /// </summary>
        [Test]
        public void AddType_InvalidProperty()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));
                Type expectedType = typeof(__PropertyTestType);

                // Expectations
                // The proxy type builder is created.
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the subject type, each call raising an exception.
                foreach (PropertyInfo property in expectedType.GetProperties(PublicInstanceBinding))
                {
                    proxyTypeBuilder.AddProperty(property);
                    LastCall.Throw(new InvalidOperationException());
                }

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);

                // Expectations 
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method and property
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { {typeof(int), typeof(double)} };

                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// static methods.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoStaticMethods()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// static properties.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoStaticProperties()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property and method
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, true, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the builder is configured to suppress
        /// all properties.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoProperties()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, false, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", PublicStaticBinding));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", PublicStaticBinding));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type override and the builder is configured to suppress
        /// all events.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_NoEvents()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, true, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };

                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", PublicStaticBinding));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"), typeof(double));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", PublicStaticBinding), typeof(double));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", PublicStaticBinding), typeof(double));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType, desiredReturnTypeOverrides);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder);
                Type[] expectedSubjectTypes = { typeof(__FirstEmptySubjectType), typeof(__SecondEmptySubjectType) };

                // Expectations
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } };
                foreach (Type expectedType in expectedSubjectTypes)
                {
                    // The proxy type builder for each type is created.
                    Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                        .Return(proxyTypeBuilder);

                    // The proxy builder is invoked for each public method
                    // on the first subject type.
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("GetType"));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("GetHashCode"), typeof(double));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("ToString"));
                    proxyTypeBuilder.AddMethod(expectedType.GetMethod("Equals"));

                    // The proxy type and its interface are created.
                    Expect.Call(proxyTypeBuilder.CreateProxy())
                        .Return(null);

                    // The XML doc comments for the proxy and interface type are retrieved.
                    Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());
                }

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                foreach (Type expectedType in expectedSubjectTypes)
                {
                    assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);
                }
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false, false));
                Type expectedType = typeof(__FirstEmptySubjectType);

                // Expectations
                // The proxy type builder is created.
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the subject type, each call raising an exception.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(int), typeof(double) } }; 
                foreach (MethodInfo method in expectedType.GetMethods(PublicInstanceBinding))
                {
                    if (method.ReturnType == typeof(int))
                    {
                        proxyTypeBuilder.AddMethod(method, typeof(double));
                    }
                    else
                    {
                        proxyTypeBuilder.AddMethod(method);
                    }

                    LastCall.Throw(new InvalidOperationException());
                }

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddType() method when given a collection
        /// of return type overrides and the underlying type builder throws an
        /// exception on processing a property.
        /// </summary>
        [Test]
        public void AddType_ReturnTypeOverride_InvalidProperty()
        {
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false, false));
                Type expectedType = typeof(__PropertyTestType);

                // Expectations
                // The proxy type builder is created.
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the subject type, each call raising an exception.
                IDictionary<Type, Type> desiredReturnTypeOverrides = new Dictionary<Type, Type>() { { typeof(string), typeof(object) } }; 
                foreach (PropertyInfo property in expectedType.GetProperties(PublicInstanceBinding))
                {
                    if (property.PropertyType == typeof(string))
                    {
                        proxyTypeBuilder.AddProperty(property, typeof(object));
                    }
                    else
                    {
                        proxyTypeBuilder.AddProperty(property);
                    }

                    LastCall.Throw(new InvalidOperationException());
                }

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedType, desiredReturnTypeOverrides);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, false, true));

                // Expectations 
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__FirstEmptySubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateTestDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                addType(assemblyBuilder, expectedSubjectType);

                XElement memberElement = XElement.Load(XmlReader.Create(assemblyBuilder.CreateXmlDocCommentReader(), ReaderSettings))
                    .Element(XmlDocCommentNames.MembersElement)
                    .Element(XmlDocCommentNames.MemberElement);

                Assert.That(memberElement, Is.Not.Null);
                Assert.That(memberElement.Attribute(XmlDocCommentNames.NameAttribute).Value, Is.EqualTo("member-name"));
                Assert.That(memberElement.IsEmpty);
                Assert.That(memberElement.ElementsAfterSelf().Count(), Is.EqualTo(0));
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property and method
                // on the real subject type.
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                addType(assemblyBuilder, expectedSubjectType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, false, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                addType(assemblyBuilder, expectedSubjectType);
            });
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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, true, false));
                Type expectedType = typeof(__EventTestType);

                // Expectations
                // The proxy type builder is created.
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Settings.EmitXmlDocComments, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the subject type, each call raising an exception.
                foreach (EventInfo evt in expectedType.GetEvents(PublicInstanceBinding))
                {
                    proxyTypeBuilder.AddEvent(evt);
                    LastCall.Throw(new InvalidOperationException());
                }

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // The XML doc comments for the proxy and interface type are retrieved.
                Expect.Call(proxyTypeBuilder.CreateXmlDocCommentReader()).Return(CreateEmptyDocCommentReader());

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                addType(assemblyBuilder, expectedType);
            });
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