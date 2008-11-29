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
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using log4net.Config;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    using CreateProxyTypeBuilderDelegate = Func<string, Type, ModuleBuilder, IProxyTypeBuilder>;


    [TestFixture]
    public sealed class ProxyAssemblyBuilderTestFixture
    {
        #region public methods --------------------------------------------------------------------

        #region initialization --------------------------------------------------------------------

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

                Module[] assemblyModules = builder.Assembly.GetModules();
                Assert.That(assemblyModules, Is.Not.Empty);
                Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
                Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(sExpectedAssemblyFullPath));
                Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
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
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method and property
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

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

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, true, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
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

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property and method
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(false, false, false, true));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property and method
                // on the real subject type.
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
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

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, true, true));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public property
                // on the real subject type.
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", BindingFlags.Public | BindingFlags.Static));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

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

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, false, true));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_1"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_2"));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddEvent(expectedSubjectType.GetEvent("PublicEvent_4", BindingFlags.Public | BindingFlags.Static));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

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

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, true, true, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy builder is invoked for each public method
                // on the real subject type.
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_1"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_2"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("PublicMethod_4", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetType"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("GetHashCode"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("ToString"));
                proxyTypeBuilder.AddMethod(expectedSubjectType.GetMethod("Equals"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_1"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_2"));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_3", BindingFlags.Public | BindingFlags.Static));
                proxyTypeBuilder.AddProperty(expectedSubjectType.GetProperty("PublicProperty_4", BindingFlags.Public | BindingFlags.Static));

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

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
            With.Mocks(delegate
            {
                CreateProxyTypeBuilderDelegate createProxyTypeBuilder = Mocker.Current.CreateMock<CreateProxyTypeBuilderDelegate>();
                IProxyTypeBuilder proxyTypeBuilder = Mocker.Current.CreateMock<IProxyTypeBuilder>();

                ProxyAssemblyBuilder assemblyBuilder = CreateTestAssemblyBuilder(createProxyTypeBuilder, new ProxyAssemblyBuilderSettings(true, false, false, false));

                // Expectations
                // The proxy type builder is created.
                Type expectedSubjectType = typeof(__RealSubjectType);
                Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedSubjectType, assemblyBuilder.Module))
                    .Return(proxyTypeBuilder);

                // The proxy type and its interface are created.
                Expect.Call(proxyTypeBuilder.CreateProxy())
                    .Return(null);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectType);
            });
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
                    Expect.Call(createProxyTypeBuilder(assemblyBuilder.RootNamespace, expectedType, assemblyBuilder.Module))
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
                }

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                assemblyBuilder.AddType(expectedSubjectTypes[0]);
                assemblyBuilder.AddType(expectedSubjectTypes[1]);
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
        /// Verifies the behavior of the CreateAssembly() method.
        /// </summary>
        [Test]
        public void CreateAssembly()
        {
            string sAssemblyFileName = MethodInfo.GetCurrentMethod().Name + ".dll";
            string sExpectedAssemblyFullPath = Path.Combine(WorkingDirectoryName, sAssemblyFileName);

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder("Unit.Testing.Namespace", sExpectedAssemblyFullPath);
            builder.CreateAssembly();

            Assert.That(File.Exists(sExpectedAssemblyFullPath));
            Assert.That(File.Exists(Path.Combine(Path.GetDirectoryName(sExpectedAssemblyFullPath),
                Path.GetFileNameWithoutExtension(sAssemblyFileName) + ".pdb")));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

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

        private static readonly string WorkingDirectoryName = Path.Combine(Path.GetTempPath(), MethodBase.GetCurrentMethod().DeclaringType.Name);

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        public class __FirstEmptySubjectType { }
        public class __SecondEmptySubjectType { }

        public class __RealSubjectType
        {
            public void PublicMethod_1() { }
            public void PublicMethod_2() { }
            public static void PublicMethod_3() { }
            public static void PublicMethod_4() { }

            public int PublicProperty_1 { get { return 0; } set { } }
            public int PublicProperty_2 { get { return 0; } set { } }
            public static int PublicProperty_3 { get { return 0; } set { } }
            public static int PublicProperty_4 { get { return 0; } set { } }
            
            public event EventHandler<EventArgs> PublicEvent_1;
            public event EventHandler<EventArgs> PublicEvent_2;
            public static event EventHandler<EventArgs> PublicEvent_3;
            public static event EventHandler<EventArgs> PublicEvent_4;

            internal void InternalMethod() { }
            protected void ProtectedMethod() { }
            private void PrivateMethod() { }
            private static void PrivateStaticMethod() { }

            internal int InternalProperty { get { return 0; } set { } }
            protected int ProtectedProperty { get { return 0; } set { } }
            private int PrivateProperty { get { return 0; } set { } }
            private static int PrivateStaticProperty { get { return 0; } set { } }

            internal event EventHandler<EventArgs> InternalEvent;
            protected event EventHandler<EventArgs> ProtectedEvent;
            private event EventHandler<EventArgs> PrivateEvent;
            private static event EventHandler<EventArgs> PrivateStaticEvent;
        }

        #endregion
    }
}
