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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using log4net.Config;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    using CreateProxyTypeBuilderDelegate = Func<string, Type, bool, ModuleBuilder, IProxyTypeBuilder>;


    [TestFixture]
    public sealed class ProxyAssemblyBuilderTestFixture
    {
        #region constructors ----------------------------------------------------------------------

        static ProxyAssemblyBuilderTestFixture()
        {
            WorkingDirectoryName = Path.Combine(Path.GetTempPath(), MethodBase.GetCurrentMethod().DeclaringType.Name);
            KeyPairPath = "CodeGeneration\\jolt-test.snk";
            PublicInstanceBinding = BindingFlags.Public | BindingFlags.Instance;

            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.ValidationType = ValidationType.Schema;

            Type schemaAssemblyType = typeof(AbstractXDCReadPolicy);
            using (Stream schema = schemaAssemblyType.Assembly.GetManifestResourceStream(schemaAssemblyType, "Xml.DocComments.xsd"))
            {
                ReaderSettings.Schemas.Add(XmlSchema.Read(schema, null));
            }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

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


        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            string expectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string expectedAssemblyFullPath = Path.Combine(Environment.CurrentDirectory, expectedAssemblyName + ".dll");
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder();

            Assert.That(builder.RootNamespace, Is.EqualTo("Jolt.Testing.CodeGeneration"));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(expectedAssemblyFullPath));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));
            
            VerifyAttributes_AssemblyName(builder.Assembly.GetName(), expectedAssemblyName, new byte[0]);
            VerifyAttributes_ModuleBuilder(builder, expectedAssemblyFullPath);

            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace is provided.
        /// </summary>
        [Test]
        public void Construction_Namespace()
        {
            string expectedNamespace = "Unit.Testing.Namespace";
            string expectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string expectedAssemblyFullPath = Path.Combine(Environment.CurrentDirectory, expectedAssemblyName + ".dll");
            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(expectedNamespace);

            Assert.That(builder.RootNamespace, Is.EqualTo(expectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(expectedAssemblyFullPath));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));

            VerifyAttributes_AssemblyName(builder.Assembly.GetName(), expectedAssemblyName, new byte[0]);
            VerifyAttributes_ModuleBuilder(builder, expectedAssemblyFullPath);

            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace and assembly full path are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_AssemblyFullPath()
        {
            string expectedNamespace = "Internal.Testing.CodeGeneration";
            string expectedAssemblyName = "Jolt.Testing.CodeGeneration.Proxies";
            string expectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), expectedAssemblyName + ".dll");

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(expectedNamespace, expectedAssemblyFullPath);

            Assert.That(builder.RootNamespace, Is.EqualTo(expectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(expectedAssemblyFullPath));
            Assert.That(builder.Settings, Is.SameAs(ProxyAssemblyBuilderSettings.Default));
            
            VerifyAttributes_AssemblyName(builder.Assembly.GetName(), expectedAssemblyName, new byte[0]);
            VerifyAttributes_ModuleBuilder(builder, expectedAssemblyFullPath);

            Assert.That(!builder.CreateXmlDocCommentReader().Read());
        }

        /// <summary>
        /// Verifies the construction of the class when a root assembly
        /// namespace, assembly full path, and configuration settings are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_AssemblyFullPath_Settings()
        {
            string expectedNamespace = "Internal.Testing.CodeGeneration";
            string expectedAssemblyName = "Proxies";
            string expectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), expectedAssemblyName + ".dll");
            ProxyAssemblyBuilderSettings expectedSettings = new ProxyAssemblyBuilderSettings();

            ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(expectedNamespace, expectedAssemblyFullPath, expectedSettings);

            Assert.That(builder.RootNamespace, Is.EqualTo(expectedNamespace));
            Assert.That(builder.AssemblyFullPath, Is.EqualTo(expectedAssemblyFullPath));
            Assert.That(builder.Settings, Is.SameAs(expectedSettings));
            
            VerifyAttributes_AssemblyName(builder.Assembly.GetName(), expectedAssemblyName, new byte[0]);
            VerifyAttributes_ModuleBuilder(builder, expectedAssemblyFullPath);

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
            WithConfigurationFile(delegate
            {
                string expectedNamespace = "Internal.Testing.CodeGeneration";
                string expectedAssemblyName = "Proxies";
                string expectedAssemblyFileName = expectedAssemblyName + ".dll";
                string expectedAssemblyFullPath = Path.Combine(Path.GetTempPath(), expectedAssemblyFileName);

                ProxyAssemblyBuilder builder = new ProxyAssemblyBuilder(expectedNamespace, expectedAssemblyFullPath);

                Assert.That(builder.RootNamespace, Is.EqualTo(expectedNamespace));
                Assert.That(builder.AssemblyFullPath, Is.EqualTo(expectedAssemblyFullPath));
                Assert.That(builder.Settings.EmitMethods);
                Assert.That(builder.Settings.EmitProperties);
                Assert.That(builder.Settings.EmitEvents);
                Assert.That(!builder.Settings.EmitStatics);
                Assert.That(builder.Settings.EmitXmlDocComments);
                Assert.That(builder.Settings.KeyPairFullPath, Is.EqualTo(KeyPairPath));
                
                VerifyAttributes_AssemblyName(builder.Assembly.GetName(), expectedAssemblyName, builder.Settings.KeyPair.PublicKey);
                VerifyAttributes_ModuleBuilder(builder, expectedAssemblyFullPath);

                XElement validatedDocComments = XElement.Load(XmlReader.Create(builder.CreateXmlDocCommentReader(), ReaderSettings));
                Assert.That(validatedDocComments, Is.Not.Null);
                Assert.That(validatedDocComments.Element(XmlDocCommentNames.AssemblyElement).Element(XmlDocCommentNames.NameElement).Value, Is.EqualTo(expectedAssemblyName));
                Assert.That(validatedDocComments.Element(XmlDocCommentNames.MembersElement).IsEmpty);
            });
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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));

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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2));

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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4));

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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));

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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2, typeof(double)));

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

            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_3));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_4));

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

            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_1));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.PublicMethod_4));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetType));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.GetHashCode, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.ToString));
            proxyTypeBuilder.Expect(pb => pb.AddMethod(__RealSubjectType.Equals));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_1, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_2, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_3, typeof(double)));
            proxyTypeBuilder.Expect(pb => pb.AddProperty(__RealSubjectType.PublicProperty_4, typeof(double)));

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

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the attributes of a given AssemblyName.
        /// </summary>
        /// 
        /// <param name="assemblyName">
        /// The AssemblyName to validate.
        /// </param>
        /// 
        /// <param name="expectedName">
        /// The expected assembly name.
        /// </param>
        /// 
        /// <param name="expectedPublicKey">
        /// The expected public key.
        /// </param>
        private static void VerifyAttributes_AssemblyName(AssemblyName assemblyName, string expectedName, byte[] expectedPublicKey)
        {
            Assert.That(assemblyName.Name, Is.EqualTo(expectedName));
            Assert.That(assemblyName.Version, Is.EqualTo(new Version(1, 0, 0, 0)));
            Assert.That(assemblyName.CultureInfo, Is.EqualTo(CultureInfo.InvariantCulture));
            Assert.That(assemblyName.GetPublicKey(), Is.EqualTo(expectedPublicKey));
        }

        /// <summary>
        /// Verifies the Module attributes of the given ProxyAssemblyBuilder.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The ProxyAssemblyBuilder to validate.
        /// </param>
        /// 
        /// <param name="expectedAssemblyFullPath">
        /// The expected full path of the assembly being built.
        /// </param>
        private static void VerifyAttributes_ModuleBuilder(ProxyAssemblyBuilder builder, string expectedAssemblyFullPath)
        {
            Module[] assemblyModules = builder.Assembly.GetModules();

            Assert.That(assemblyModules, Is.Not.Empty);
            Assert.That(assemblyModules[assemblyModules.Length - 1], Is.SameAs(builder.Module));
            Assert.That(builder.Module.FullyQualifiedName, Is.EqualTo(expectedAssemblyFullPath));
            Assert.That(builder.Module.ScopeName, Is.EqualTo("Jolt.Testing.CodeGeneration.Proxies.dll"));
        }

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
            Assert.That(memberElement.ElementsAfterSelf(), Is.Empty);

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

            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_1));
            proxyTypeBuilder.Expect(pb => pb.AddEvent(__RealSubjectType.PublicEvent_2));

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

        /// <summary>
        /// Initializes the application configuration file for this test fixture,
        /// then executes a given method prior to reverting the configuration changes.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to invoke while the configuration is loaded and active.
        /// </param>
        private static void WithConfigurationFile(Action method)
        {
            // Create the assembly configuration.
            string settingsSection = "proxyBuilderSettings";
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Sections.Add(settingsSection, new ProxyAssemblyBuilderSettings(false, true, true, true, true, KeyPairPath));
            config.Save();

            try
            {
                // Invoke the method with the new configuration.
                ConfigurationManager.RefreshSection(settingsSection);
                method();
            }
            finally
            {
                // Revert the assembly configuration.
                File.Delete(config.FilePath);
                ConfigurationManager.RefreshSection(settingsSection);
            }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly string WorkingDirectoryName;
        private static readonly string KeyPairPath;
        private static readonly BindingFlags PublicInstanceBinding;
        private static readonly XmlReaderSettings ReaderSettings;

        #endregion
    }
}