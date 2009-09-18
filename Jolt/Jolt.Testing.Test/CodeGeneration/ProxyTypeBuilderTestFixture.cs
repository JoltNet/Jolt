// ----------------------------------------------------------------------------
// ProxyTypeBuilderTestFixture.cs
//
// Contains the definition of the ProxyTypeBuilderTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/31/2007 19:49:19
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;

using Jolt.Functional;
using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Properties;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    using CreateXDCBuilderDelegate = Func<Type, Type, Type, XmlDocCommentBuilderBase>;


    /// <summary>
    /// Verifies the implementation of the ProxyTypeBuilder class.
    /// The methods CreateProxy() and CreateInterface() are tested indirectly
    /// as they validate the effects of the other class methods.
    /// 
    /// Furthermore, this focus of this fixture is to test the implementation
    /// of the generated methods and types.  Testing the declaration of methods
    /// is handled in the method-declarer test fixtures.
    /// </summary>
    [TestFixture]
    public sealed class ProxyTypeBuilderTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class when only the proxied type and
        /// root namesace are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType()
        {
            Type proxiedType = typeof(string);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));

            Assert.That(builder.Module.IsTransient());
            Assert.That(builder.Module.Assembly.GetName().Name, Is.EqualTo("__transientAssembly"));
            Assert.That(!builder.Module.Assembly.ReflectionOnly);
            Assert.That(!builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the construction of the class when only the proxied type, root
        /// namesace, and a directive to produce XML doc comments are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_ProducesXDC()
        {
            Type proxiedType = typeof(TestAttribute);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, true);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));

            Assert.That(builder.Module.IsTransient());
            Assert.That(builder.Module.Assembly.GetName().Name, Is.EqualTo("__transientAssembly"));
            Assert.That(!builder.Module.Assembly.ReflectionOnly);
            Assert.That(builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilder>());

            XmlDocCommentReader reader = (builder.XmlDocCommentBuilder as XmlDocCommentBuilder).XmlDocCommentReader as XmlDocCommentReader;
            Assert.That(reader.ReadPolicy, Is.InstanceOf<DefaultXDCReadPolicy>());
        }

        /// <summary>
        /// Verifies the construction of the class when only the proxied type, root
        /// namesace, and a directive to produce XML doc comments are provided.
        /// XML doc comments are not found for the real subject type.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_ProducesXDC_XDCNotFound()
        {
            Type proxiedType = GetType();
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, true);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));

            Assert.That(builder.Module.IsTransient());
            Assert.That(builder.Module.Assembly.GetName().Name, Is.EqualTo("__transientAssembly"));
            Assert.That(!builder.Module.Assembly.ReflectionOnly);
            Assert.That(!builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the construction of the class when only the proxied type, root
        /// namesace, and a directive to not produce XML doc comments are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_DoesNotProduceXDC()
        {
            Type proxiedType = GetType();
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, false);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));

            Assert.That(builder.Module.IsTransient());
            Assert.That(builder.Module.Assembly.GetName().Name, Is.EqualTo("__transientAssembly"));
            Assert.That(!builder.Module.Assembly.ReflectionOnly);
            Assert.That(!builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type, root namespace,
        /// target module,and a directive to produce XML doc comments are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_Module_ProducesXDC()
        {
            Type proxiedType = typeof(TestAttribute);
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, true, expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
            Assert.That(builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilder>());

            XmlDocCommentReader reader = (builder.XmlDocCommentBuilder as XmlDocCommentBuilder).XmlDocCommentReader as XmlDocCommentReader;
            Assert.That(reader.ReadPolicy, Is.InstanceOf<DefaultXDCReadPolicy>());
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type, root namespace,
        /// target module,and a directive to produce XML doc comments are provided.
        /// XML doc comments are not found for the real subject type.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_Module_ProducesXDC_XDCNotFound()
        {
            Type proxiedType = GetType();
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, true, expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
            Assert.That(!builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type, root namespace,
        /// target module,and a directive to not produce XML doc comments are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_Module_DoesNotProduceXDC()
        {
            Type proxiedType = GetType();
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, false, expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
            Assert.That(!builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.InstanceOf<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            XmlDocCommentBuilderBase xdcBuilder = MockRepository.GenerateStub<XmlDocCommentBuilderBase>();

            Type proxiedType = GetType();
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, CreateXDCBuilderFactoryDelegate(xdcBuilder), expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
            Assert.That(builder.ProducesXmlDocComments);
            Assert.That(builder.XmlDocCommentBuilder, Is.SameAs(xdcBuilder));
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type is abstract.
        /// The generated proxy must not contain a field reference to the proxied type.
        /// </summary>
        [Test]
        public void Construction_AbstractClass()
        {
            Type proxiedType = typeof(__AbstractType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance);

            Assert.That(proxiedTypeField, Is.Null);
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type is generic and abstract.
        /// The generated proxy must not contain a field reference to the proxied type.
        /// </summary>
        [Test]
        public void Construction_AbstractClass_Generic()
        {
            Type proxiedType = typeof(__AbstractType<>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance);

            Assert.That(proxiedTypeField, Is.Null);
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type is abstract and sealed (static).
        /// The generated proxy must not contain a field reference to the proxied type.
        /// </summary>
        [Test]
        public void Construction_StaticClass()
        {
            Type proxiedType = typeof(__AbstractSealedType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance);

            Assert.That(proxiedTypeField, Is.Null);
        }

        /// <summary>
        /// Verifes the construction of the class when the proxied type is generic, abstract, and sealed (static).
        /// The generated proxy must not contain a field reference to the proxied type;
        /// </summary>
        [Test]
        public void Construction_StaticClass_Generic()
        {
            Type proxiedType = typeof(__AbstractSealedType<,,>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance);

            Assert.That(proxiedTypeField, Is.Null);
        }
        /// <summary>
        /// Verifies the construction of the class when the proxied type is not static.
        /// The generated proxy must contain a field reference to the proxied type.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass()
        {
            Type proxiedType = typeof(string);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance );

            Assert.That(proxiedTypeField, Is.Not.Null);
            Assert.That(proxiedTypeField.FieldType, Is.EqualTo(proxiedType));
            Assert.That(proxiedTypeField.IsInitOnly);
            Assert.That(proxiedTypeField.IsPrivate);
            Assert.That(!proxiedTypeField.IsStatic);
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type is generic and not static.
        /// The generated proxy must contain a field reference to the proxied type.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_Generic()
        {
            Type proxiedType = typeof(System.Collections.Generic.List<>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", NonPublicInstance);

            Assert.That(proxiedTypeField, Is.Not.Null);
            Assert.That(proxiedTypeField.FieldType, Is.Not.EqualTo(proxiedType));
            Assert.That(proxiedTypeField.FieldType.Name, Is.EqualTo(proxiedType.Name));
            Assert.That(proxiedTypeField.FieldType.Namespace, Is.EqualTo(proxiedType.Namespace));
            Assert.That(proxiedTypeField.IsInitOnly);
            Assert.That(proxiedTypeField.IsPrivate);
            Assert.That(!proxiedTypeField.IsStatic);
        }

        /// <summary>
        /// Verifies the initialziation of constructors in the proxy type,
        /// when the real subject type is not static.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_ConstructorInitialization()
        {
            Type realSubjectType = typeof(__ConstructorTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
            Type proxy = builder.CreateProxy();
            ConstructorInfo[] expectedConstructors = realSubjectType.GetConstructors();
            ConstructorInfo[] actualConstructors = proxy.GetConstructors();

            Assert.That(actualConstructors, Has.Length.EqualTo(expectedConstructors.Length));

            // Proxy constructor forwards to the proxied type constructor.
            AssertConstructorInvoked(proxy, null, "0");
            AssertConstructorInvoked(proxy, new object[] { 123 }, "123");
            AssertConstructorInvoked(proxy, new object[] { 456, 789 }, "456789");
        }

        /// <summary>
        /// Verifies the initialziation of constructors in the proxy type,
        /// when the real subject type is generic and not static.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_Generic_ConstructorInitialization()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(string), typeof(TimeSpan));
            ConstructorInfo[] expectedConstructors = realSubjectType.GetConstructors();
            ConstructorInfo[] actualConstructors = specializedProxy.GetConstructors();

            Assert.That(actualConstructors, Has.Length.EqualTo(expectedConstructors.Length));

            // Proxy constructor forwards to the proxied type constructor.
            AssertConstructorInvoked(specializedProxy, null, "0");
            AssertConstructorInvoked(specializedProxy, new object[] { "single-generic-param" }, "single-generic-param");
            AssertConstructorInvoked(specializedProxy, new object[] { "two-generic-params", TimeSpan.FromSeconds(1.0) }, "two-generic-params00:00:01");
            AssertConstructorInvoked(specializedProxy, new object[] { "mixed-params", TimeSpan.Zero, 123 }, "mixed-params00:00:00123");
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when an interface type
        /// is given to the proxy type builder as the real subject type.
        /// </summary>
        [Test]
        public void Construction_RealSubjectInterfaceType()
        {
            Assert.That(
                CreateProxyTypeBuilderDelegate(typeof(__InterfaceType)),
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidRealSubjectType, typeof(__InterfaceType).Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when a delegate type
        /// is given to the proxy type builder as the real subject type.
        /// </summary>
        [Test]
        public void Construction_RealSubjectDelegateType()
        {
            Assert.That(
                CreateProxyTypeBuilderDelegate(typeof(Action)),
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidRealSubjectType, typeof(Action).Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when a real
        /// subject type does not contain a public constructor.
        /// </summary>
        [Test]
        public void Construction_NonConstructable()
        {
            Assert.That(
                CreateProxyTypeBuilderDelegate(typeof(__HiddenConstructorType)),
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                    String.Format(Resources.Error_RealSubjectType_LackingConstructor, typeof(__HiddenConstructorType).Name)));
        }

        /// <summary>
        /// Verifies that XML doc comments are produced as part of the constructor
        /// initialization for the proxy type.
        /// </summary>
        [Test]
        public void Construction_ConstructorInitialization_XmlDocComments()
        {
            XmlDocCommentBuilderBase xdcBuilder = MockRepository.GenerateMock<XmlDocCommentBuilderBase>();

            // Expectations
            // Each real subject type constructor is passed to the
            // XML doc comment builder.
            Type realSubjectType = typeof(__ConstructorTestType);
            foreach (ConstructorInfo constructor in realSubjectType.GetConstructors())
            {
                xdcBuilder.Expect(x => x.AddConstuctor(constructor));
            }

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType, CreateXDCBuilderFactoryDelegate(xdcBuilder), CreateTransientModule());

            xdcBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the default behavior of the CreateInterface() method,
        /// prior to adding methods.
        /// </summary>
        [Test]
        public void CreateInterface()
        {
            Type proxyInterface = new ProxyTypeBuilder(DefaultNamespace, typeof(string)).CreateInterface();
            AssertInterfaceAttributes(proxyInterface, "IString", DefaultNamespace + ".System");
        }

        /// <summary>
        /// Verifies the default behavior of the CreateInterface() method,
        /// when the real subject type is nested, and prior to adding methods.
        /// </summary>
        [Test]
        public void CreateInterface_FromNestedType()
        {
            Type proxyInterface = new ProxyTypeBuilder(DefaultNamespace, typeof(WebRequestMethods.Ftp)).CreateInterface();
            AssertInterfaceAttributes(proxyInterface, "IFtp", DefaultNamespace + ".System.Net");
        }

        /// <summary>
        /// Verifies the default behavior of the CreateProxy() method,
        /// prior to adding methods.
        /// </summary>
        [Test]
        public void CreateProxy()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));
            Type proxy = builder.CreateProxy();
            Type proxyInterface = builder.CreateInterface();

            AssertProxyAttributes(proxy, proxyInterface, "StringProxy", DefaultNamespace + ".System");
        }

        /// <summary>
        /// Verifies the behavior of the CreateXmlDocCommentReader() method.
        /// </summary>
        [Test]
        public void CreateXmlDocCommentReader()
        {
            XmlDocCommentBuilderBase xdcBuilder = MockRepository.GenerateMock<XmlDocCommentBuilderBase>();

            XmlReader expectedXmlReader = XmlReader.Create(new StringReader("<xml/>"));
            xdcBuilder.Expect(x => x.CreateReader()).Return(expectedXmlReader);

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType), CreateXDCBuilderFactoryDelegate(xdcBuilder), CreateTransientModule());

            Assert.That(builder.CreateXmlDocCommentReader(), Is.SameAs(expectedXmlReader));

            xdcBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Verifies the default behavior of the CreateProxy() method,
        /// when the real subject type is nested, and prior to adding methods.
        /// </summary>
        [Test]
        public void CreateProxy_FromNestedType()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(WebRequestMethods.Http));
            Type proxy = builder.CreateProxy();
            Type proxyInterface = builder.CreateInterface();

            AssertProxyAttributes(proxy, proxyInterface, "HttpProxy", DefaultNamespace + ".System.Net");
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Instance()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.InstanceMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            DateTime result = (DateTime)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null,  Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.EqualTo(DateTime.Today));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Instance_GenericRealSubectType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<DateTime,StringBuilder,MemoryStream>.InstanceMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));
            DateTime result = (DateTime)specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy), null);
            Assert.That(result, Is.EqualTo(DateTime.MinValue));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic instance method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Instance_GenericMethod()
        {
            MethodInfo realSubejctTypeMethod = __MethodTestType<DateTime, StringBuilder, MemoryStream>.GenericInstanceMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubejctTypeMethod.DeclaringType);
            builder.AddMethod(realSubejctTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));
            DateTime result = (DateTime)specializedProxy.GetMethod(realSubejctTypeMethod.Name).MakeGenericMethod(typeof(DateTime)).Invoke(
                Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null, null, null);
            Assert.That(result, Is.EqualTo(DateTime.MinValue));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a static method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Static()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.StaticMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            TimeSpan result = (TimeSpan)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.EqualTo(TimeSpan.FromHours(12.0)));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a static method from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Static_GenericRealSubjectType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<DateTime, StringBuilder, MemoryStream>.StaticMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));
            StringBuilder result = (StringBuilder)specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy), null);
            Assert.That(result.ToString(), Is.Empty);
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic static method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Static_GenericMethod()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<DateTime, StringBuilder, MemoryStream>.GenericStaticMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));
            UriBuilder result = (UriBuilder)specializedProxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(UriBuilder)).Invoke(
                Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null, null, null);
            Assert.That(result.ToString(), Is.EqualTo("http://localhost/"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with no return value to the builder.
        /// </summary>
        [Test]
        public void AddMethod_VoidReturnValue()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.VoidReturnValueMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            
            try
            {
                proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.That(ex.InnerException.Message, Is.EqualTo("void-return"));
            }
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with no return value from a generic type
        /// to the builder.
        /// </summary>
        [Test]
        public void AddMethod_VoidReturnValue_GenericRealSubjectType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<DateTime, StringBuilder, MemoryStream>.VoidMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));

            try
            {
                specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy), null);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.That(ex.InnerException.Message, Is.EqualTo("void-return"));
            }
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic instance method with no return value to the builder.
        /// </summary>
        [Test]
        public void AddMethod_VoidReturnValue_GenericMethod()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<DateTime, StringBuilder, MemoryStream>.GenericVoidMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(DateTime), typeof(StringBuilder), typeof(MemoryStream));

            try
            {
                specializedProxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(int), typeof(string)).Invoke(
                    Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null, null, null);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.That(ex.InnerException.Message, Is.EqualTo("void-return"));
            }
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with many arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ManyArguments()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.ManyArgumentsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            string sResult = (string)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy),
                new object[] { "-test-", 1, 2.3, DateTime.Today });
            Assert.That(sResult, Is.EqualTo("many-args:-test-12.3" + DateTime.Today.ToString()));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with many arguments from a generic type 
        /// to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ManyArguments_GenericRealSubjectType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, StringBuilder, MemoryStream>.ManyArgsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(StringBuilder), typeof(MemoryStream));
            string sResult = (string)specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy),
                new object[] { TimeSpan.Zero, new StringBuilder("abc"), 2.3, new MemoryStream(new byte[] { 120 }) });
            Assert.That(sResult, Is.EqualTo("many-args:00:00:00abc2.3120"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic instance method with many arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ManyArguments_GenericMethod()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, StringBuilder, MemoryStream>.GenericManyArgsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(StringBuilder), typeof(MemoryStream));
            string sResult = (string)specializedProxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(Uri), typeof(char)).Invoke(
                Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null,
                new object[] { TimeSpan.Zero, new StringBuilder("abc"), new Uri("http://localhost"), 2.3, new MemoryStream(new byte[] { 120 }), 'z'}, null);
            
            Assert.That(sResult, Is.EqualTo("many-args:00:00:00abchttp://localhost/2.3120z"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with params[] arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ParamsArrayArguments()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.ParamsArrayArgumentsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            string sResult = (string)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy),
                new object[] { String.Empty, new object[] { 0, 1, 2, 3, 4, 5, 10 } });
            Assert.That(sResult, Is.EqualTo("params-args:01234510"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with params[] arguments from a generic type
        /// to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ParamsArrayArguments_GenericRealSubjecType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, StringBuilder, MemoryStream>.ParamArrayArgsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(StringBuilder), typeof(MemoryStream));
            string sResult = (string)specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy),
                new object[] { new MemoryStream(new byte[] { 42 }), new TimeSpan[] { TimeSpan.FromSeconds(1.0), TimeSpan.Zero, TimeSpan.FromHours(2.0) } });
            Assert.That(sResult, Is.EqualTo("params-args:4200:00:0100:00:0002:00:00"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic instance method with params[] arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ParamsArrayArguments_GenericMethod()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, StringBuilder, MemoryStream>.GenericParamArrayArgsMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(StringBuilder), typeof(MemoryStream));
            string sResult = (string)specializedProxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(TimeSpan)).Invoke(
                Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null,
                new object[] { new MemoryStream(new byte[] { 42 }), new TimeSpan[] { TimeSpan.FromSeconds(1.0), TimeSpan.Zero, TimeSpan.FromHours(2.0) } }, null);
            Assert.That(sResult, Is.EqualTo("params-args:4200:00:0100:00:0002:00:00"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// two overloaded methods are added to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Overload()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType));

            // Verify that the following operations do not throw.
            builder.AddMethod(__MethodTestType.InstanceMethod);
            builder.AddMethod(__MethodTestType.InstanceMethod_1);
            builder.CreateInterface();
            builder.CreateProxy();
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// overloaded methods from a generic type are added to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Overload_Generics()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType<,,>));

            // Verify that the following operations do not throw.
            builder.AddMethod(__MethodTestType<int, object, Stream>.InstanceMethod);
            builder.AddMethod(__MethodTestType<int, object, Stream>.InstanceMethod_1);
            builder.AddMethod(__MethodTestType<int, object, Stream>.GenericInstanceMethod);
            builder.AddMethod(__MethodTestType<int, object, Stream>.GenericInstanceMethod_1);
            builder.CreateInterface();
            builder.CreateProxy();
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a method that contains an out parameter.
        /// </summary>
        [Test]
        public void AddMethod_OutParameter()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType.OutParameterMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            object[] methodArgs = { null };
            proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), methodArgs, null);
            Assert.That(methodArgs[0], Is.EqualTo("out-param"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a method from a generic type, containing an out parameter.
        /// </summary>
        [Test]
        public void AddMethod_OutParameter_GenericRealSubjectType()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, Exception, MemoryStream>.OutParamMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(Exception), typeof(MemoryStream));
            Exception[] methodArgs = { null };
            specializedProxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(specializedProxy), methodArgs, null);
            Assert.That(methodArgs[0].Message, Is.EqualTo("Exception of type 'System.Exception' was thrown."));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a generic method containing an out parameter.
        /// </summary>
        [Test]
        public void AddMethod_OutParameter_GenericMethod()
        {
            MethodInfo realSubjectTypeMethod = __MethodTestType<TimeSpan, Exception, MemoryStream>.GenericOutParamMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(TimeSpan), typeof(Exception), typeof(MemoryStream));
            object[] methodArgs = { null, null, null };
            specializedProxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(UriBuilder), typeof(Version)).Invoke(
                Activator.CreateInstance(specializedProxy), ProxyMethodInvocationFlags, null, methodArgs, null);

            Type[] expectedTypes = { typeof(UriBuilder), typeof(Version), typeof(Exception) };
            for (int i = 0; i < expectedTypes.Length; ++i)
            {
                Assert.That(methodArgs[i], Is.Not.Null);
                Assert.That(methodArgs[i], Is.InstanceOf(expectedTypes[i]));
            }
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a method from an invalid type.
        /// </summary>
        [Test]
        public void AddMethod_InvalidMethod()
        {
            MethodInfo method = __MethodTestType.InstanceMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddMethod, method)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_MethodNotMemberOfRealSubject, method.Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with an instance method from an abstract type.
        /// </summary>
        [Test]
        public void AddMethod_InvalidInstanceMethod()
        {
            MethodInfo method = typeof(__AbstractType).GetMethod("ToString");
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddMethod, method)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InstanceMethodAddedFromAbstractType, method.Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a private method.
        /// </summary>
        [Test]
        public void AddMethod_PrivateMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType));
            
            Assert.That(
                new TestDelegate(Bind.First(builder.AddMethod, __MethodTestType.PrivateMethod)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(Resources.Error_MemberNotPublic));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a null argument.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddMethod_NullMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType));
            builder.AddMethod(null);
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding an
        /// inherited, non-virtual method.
        /// </summary>
        [Test]
        public void AddMethod_Inherited()
        {
            MethodInfo realSubjectTypeMethod = __DerivedSubjectType.NonVirtualMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            string sResult = (string)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
            Assert.That(sResult, Is.EqualTo("Base:NonVirtual"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding an
        /// virtual, overriden method.
        /// </summary>
        [Test]
        public void AddMethod_Override()
        {
            MethodInfo realSubjectTypeMethod = __DerivedSubjectType.VirtualMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy();
            string sResult = (string)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
            Assert.That(sResult, Is.EqualTo("Derived:Override"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when the
        /// same method is added to the builder more than once.
        /// </summary>
        [Test]
        public void AddMethod_DuplicateMethod()
        {
            MethodInfo method = __MethodTestType.InstanceMethod;

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType));
            builder.AddMethod(method);

            Assert.That(
                () => builder.AddMethod(method),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        /// <summary>
        /// Verifies that XML doc comments are produced as part of adding
        /// a method to the proxy builder.
        /// </summary>
        [Test]
        public void AddMethod_XmlDocComments()
        {
            Type realSubjectType = typeof(__MethodTestType);
            VerifyBehavior_AddMember_XmlDocComments(realSubjectType, realSubjectType.GetMethods());
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when given
        /// an override for the method's return type.
        /// </summary>
        [Test]
        public void AddMethod_ReturnTypeOverride()
        {
            MethodInfo realSubjectTypeMethod = __ReturnTypeOverrideType<FileNotFoundException, IOException>.InstanceMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod, typeof(IOException));

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy().MakeGenericType(typeof(FileNotFoundException), typeof(IOException));
            Assert.That(proxy.GetMethod(realSubjectTypeMethod.Name).ReturnType, Is.EqualTo(typeof(IOException)));

            IOException result = (IOException)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.InstanceOf<PathTooLongException>());
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when given
        /// an override for the method's return type that originates from
        /// the method's declaring type's generic parameter list.
        /// </summary>
        [Test]
        public void AddMethod_ReturnTypeOverride_GenericTypeParameter()
        {
            MethodInfo realSubjectTypeMethod = __ReturnTypeOverrideType<FileNotFoundException, IOException>.GenericTypeParamMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod, typeof(__ReturnTypeOverrideType<,>).GetGenericArguments()[1]);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy().MakeGenericType(typeof(DirectoryNotFoundException), typeof(IOException));
            Assert.That(proxy.GetMethod(realSubjectTypeMethod.Name).ReturnType, Is.EqualTo(typeof(IOException)));

            IOException result = (IOException)proxy.InvokeMember(realSubjectTypeMethod.Name, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.InstanceOf<DirectoryNotFoundException>());
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when given
        /// an override for the method's return type that originates from
        /// the method's generic parameter list.
        /// </summary>
        [Test]
        public void AddMethod_ReturnTypeOverride_GenericMethodParameter()
        {
            MethodInfo realSubjectTypeMethod = __ReturnTypeOverrideType<FileNotFoundException, IOException>.GenericMethodParamMethod;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeMethod.DeclaringType);
            builder.AddMethod(realSubjectTypeMethod, typeof(__ReturnTypeOverrideType<,>).GetGenericArguments()[1]);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy().MakeGenericType(typeof(FileLoadException), typeof(IOException));
            MethodInfo proxyMethod = proxy.GetMethod(realSubjectTypeMethod.Name).MakeGenericMethod(typeof(DirectoryNotFoundException));
            Assert.That(proxyMethod.ReturnType, Is.EqualTo(typeof(IOException)));

            IOException result = (IOException)proxyMethod.Invoke(Activator.CreateInstance(proxy), ProxyMethodInvocationFlags, null, null, null);
            Assert.That(result, Is.InstanceOf<DirectoryNotFoundException>());
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when given
        /// an invalid override for the method's return type.
        /// </summary>
        [Test]
        public void AddMethod_ReturnTypeOverride_InvalidOverride()
        {
            MethodInfo method = __ReturnTypeOverrideType<FileNotFoundException, IOException>.InstanceMethod;
            Type desiredReturnType = typeof(NotImplementedException);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, method.DeclaringType);
            
            Assert.That(
                new TestDelegate(Bind.First(Bind.Second<MethodInfo, Type>(builder.AddMethod, desiredReturnType), method)),  // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidReturnTypeOverride, method.Name, desiredReturnType.Name, method.ReturnType.Name)));
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// an instance property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Instance()
        {
            AssertAddPropertyBehavior(__PropertyTestType.InstanceProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                uint nResult = (uint)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(nResult, Is.EqualTo(0xdeadbeef));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, (uint)0xc000, null);
                nResult = (uint)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(nResult, Is.EqualTo(0xc000));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// an instance property from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Instance_GenericRealSubjectType()
        {
            AssertAddPropertyBehavior(__PropertyTestType<int,int,object>.InstanceProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(DateTime), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(specializedProxy);
                DateTime result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(DateTime.MinValue));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, DateTime.Today, null);
                result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(DateTime.Today));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a static property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Static()
        {
            AssertAddPropertyBehavior(__PropertyTestType.StaticProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                typeof(__PropertyTestType).TypeInitializer.Invoke(null, null);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                DateTime result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(DateTime.Today));

                // Set operation.
                DateTime expectedValue = DateTime.Today.AddMonths(-1);
                proxyProperty.SetValue(proxyInstance, expectedValue, null);
                result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(expectedValue));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a static property from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Static_GenericRealSubjectType()
        {
            AssertAddPropertyBehavior(__PropertyTestType<DateTime, TimeSpan, StringBuilder>.StaticProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(DateTime), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

                typeof(__PropertyTestType<DateTime, TimeSpan, StringBuilder>).TypeInitializer.Invoke(null, null);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(specializedProxy);
                TimeSpan result = (TimeSpan)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(TimeSpan.Zero));

                // Set operation.
                TimeSpan expectedValue = TimeSpan.FromMinutes(72);
                proxyProperty.SetValue(proxyInstance, expectedValue, null);
                result = (TimeSpan)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(expectedValue));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a getter-only property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Getter()
        {
            AssertAddPropertyBehavior(__PropertyTestType.GetterProperty, Functor.NoOperation<Type, PropertyInfo>());
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a setter-only property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Setter()
        {
            AssertAddPropertyBehavior(__PropertyTestType.SetterProperty, Functor.NoOperation<Type, PropertyInfo>());
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a property from an invalid type.
        /// </summary>
        [Test]
        public void AddProperty_InvalidProperty()
        {
            PropertyInfo property = __PropertyTestType.GetterProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddProperty, property)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_MethodNotMemberOfRealSubject, property.GetGetMethod().Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with an instance property from an abstract type.
        /// </summary>
        [Test]
        public void AddProperty_InvalidInstanceProperty()
        {
            PropertyInfo property = __AbstractType.InstanceProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddProperty, property)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InstanceMethodAddedFromAbstractType, property.GetGetMethod().Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a private property.
        /// </summary>
        [Test]
        public void AddProperty_PrivateProperty()
        {
            PropertyInfo property = __PropertyTestType.PrivateProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddProperty, property)),    // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidProperty, property.Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a null argument.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddProperty_NullProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));
            builder.AddProperty(null);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding an
        /// inherited, non-virtual property.
        /// </summary>
        [Test]
        public void AddProperty_Inherited()
        {
            AssertAddPropertyBehavior(__DerivedSubjectType.NonVirtualProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                string sResult = (string)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(sResult, Is.EqualTo("Base:NonVirtual"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "non-virtual-setter", null);
                sResult = (string)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(sResult, Is.EqualTo("non-virtual-setter"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding an
        /// virtual, overriden property.
        /// </summary>
        [Test]
        public void AddProperty_Override()
        {
            AssertAddPropertyBehavior(__DerivedSubjectType.VirtualProperty,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                string sResult = (string)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(sResult, Is.EqualTo("Derived:Override"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "non-virtual-setter", null);
                sResult = (string)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(sResult, Is.EqualTo("fixed-value"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when the
        /// same getter property is added to the builder more than once.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddProperty_DuplicateGetterProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));
            builder.AddProperty(__PropertyTestType.GetterProperty);
            builder.AddProperty(__PropertyTestType.GetterProperty);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when the
        /// same setter property is added to the builder more than once.
        /// </summary>
        [Test]
        public void AddProperty_DuplicateSetterProperty()
        {
            PropertyInfo property = __PropertyTestType.SetterProperty;

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));
            builder.AddProperty(property);

            Assert.That(
                () => builder.AddProperty(property),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when the
        /// getter is less accessible (non-public) than the corresponding
        /// setter.
        /// </summary>
        [Test]
        public void AddProperty_InaccessibleGetter()
        {
            AssertInaccessiblePropertyBehavior(__PropertyTestType.InternalGetterProperty);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when the
        /// setter is less accessible (non-public) than the corresponding
        /// getter.
        /// </summary>
        [Test]
        public void AddProperty_InaccessibleSetter()
        {
            AssertInaccessiblePropertyBehavior(__PropertyTestType.PrivateSetterProeprty);
        }

        /// <summary>
        /// Verifies the behavior fo the AddProperty() method when
        /// an indexer property is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer()
        {
            AssertAddPropertyBehavior(__PropertyTestType.Item_1,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                object[] getterArguments = { 0 };
                string sResult = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(sResult, Is.EqualTo("simple-value"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "new-value-", new object[] { 0xC000 });
                sResult = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(sResult, Is.EqualTo("new-value-49152"));
            });
        }

        /// <summary>
        /// Verifies the behavior fo the AddProperty() method when
        /// an indexer property from a generic type is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer_GenericRealSubjectType()
        {
            AssertAddPropertyBehavior(__PropertyTestType<int,int,object>.Item_1,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(DateTime), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(specializedProxy);
                object[] getterArguments = { DateTime.Now };
                string sResult = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(sResult, Is.EqualTo("simple-value"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "new-value-", new object[] { DateTime.Today });
                sResult = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(sResult, Is.EqualTo("new-value-" + DateTime.Today.ToString()));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// a multi-argument indexer property is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer_ManyArguments()
        {
            AssertAddPropertyBehavior(__PropertyTestType.Item_3,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                int nResult = (int)proxyProperty.GetValue(proxyInstance, new object[] { 0, 10, 20 });
                Assert.That(nResult, Is.EqualTo(0xbeef));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, 0xC000, new object[] { 0x0F00, 0x00F0, 0x000F });
                nResult = (int)proxyProperty.GetValue(proxyInstance, new object[] { 0, 1, 2 });
                Assert.That(nResult, Is.EqualTo(0xCFFF));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// a multi-argument indexer property from a generic type
        /// is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer_ManyArguments_GenericRealSubjectType()
        {
            AssertAddPropertyBehavior(__PropertyTestType<int,int,object>.Item_3,
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(Guid), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(specializedProxy);
                object[] getterArguments = { Guid.NewGuid(), TimeSpan.Zero, new StringBuilder() };
                string result = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(result, Is.EqualTo("complex-value"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "indexer-value", new object[] { Guid.Empty, TimeSpan.FromDays(0.5), new StringBuilder("test") });
                result = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(result, Is.EqualTo("indexer-value00000000-0000-0000-0000-00000000000012:00:00test"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// two overloaded indexers are added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_IndexerOverload()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            // Verify that the following operations do not throw.
            builder.AddProperty(realSubjectType.GetProperty("Item", new Type[] { typeof(int) }));
            builder.AddProperty(realSubjectType.GetProperty("Item", new Type[] { typeof(int), typeof(int), typeof(int) }));
            builder.CreateInterface();
            builder.CreateProxy();
        }

        /// <summary>
        /// Verifies that XML doc comments are produced as part of adding
        /// a property to the proxy builder.
        /// </summary>
        [Test]
        public void AddProperty_XmlDocComments()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            VerifyBehavior_AddMember_XmlDocComments(realSubjectType, realSubjectType.GetProperties());
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when given
        /// an override for the property's return type.
        /// </summary>
        [Test]
        public void AddProperty_ReturnTypeOverride()
        {
            PropertyInfo realSubjectTypeProperty = __ReturnTypeOverrideType<FileNotFoundException, IOException>.InstanceProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeProperty.DeclaringType);
            builder.AddProperty(realSubjectTypeProperty, typeof(IOException));

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy().MakeGenericType(typeof(FileNotFoundException), typeof(IOException));
            PropertyInfo proxyProperty = proxy.GetProperty(realSubjectTypeProperty.Name);
            Assert.That(proxyProperty.PropertyType, Is.EqualTo(typeof(IOException)));
            Assert.That(proxyProperty.GetGetMethod().ReturnType, Is.EqualTo(typeof(IOException)));

            IOException result = (IOException)proxyProperty.GetValue(Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.InstanceOf<PathTooLongException>());
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when given
        /// an override for the property's return type that originates from
        /// the property's declaring type's generic parameter list.
        /// </summary>
        [Test]
        public void AddProperty_ReturnTypeOverride_GenericTypeParameter()
        {
            PropertyInfo realSubjectTypeProperty = __ReturnTypeOverrideType<FileNotFoundException, IOException>.GenericTypeParamProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeProperty.DeclaringType);
            builder.AddProperty(realSubjectTypeProperty, typeof(__ReturnTypeOverrideType<,>).GetGenericArguments()[1]);

            // Verify the behavior of the generated proxy.
            Type proxy = builder.CreateProxy().MakeGenericType(typeof(DirectoryNotFoundException), typeof(IOException));
            PropertyInfo proxyProperty = proxy.GetProperty(realSubjectTypeProperty.Name);
            Assert.That(proxyProperty.PropertyType, Is.EqualTo(typeof(IOException)));
            Assert.That(proxyProperty.GetGetMethod().ReturnType, Is.EqualTo(typeof(IOException)));

            IOException result = (IOException)proxyProperty.GetValue(Activator.CreateInstance(proxy), null);
            Assert.That(result, Is.InstanceOf<DirectoryNotFoundException>());
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when given
        /// an invalid override for the property's return type.
        /// </summary>
        [Test]
        public void AddProperty_ReturnTypeOverride_InvalidOverride()
        {
            PropertyInfo property = __ReturnTypeOverrideType<FileNotFoundException, IOException>.InstanceProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, property.DeclaringType);
            Type desiredReturnType = typeof(NotImplementedException);

            Assert.That(
                () => builder.AddProperty(property, desiredReturnType),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidReturnTypeOverride, property.GetGetMethod().Name, desiredReturnType.Name, property.PropertyType.Name)));
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when attempting
        /// to override an invalid property.
        /// </summary>
        [Test]
        public void AddProperty_ReturnTypeOverride_InvalidProperty()
        {
            PropertyInfo property = __ReturnTypeOverrideType<FileNotFoundException, IOException>.InvalidProperty;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, property.DeclaringType);
            Type desiredReturnType = typeof(IOException);

            Assert.That(
                () => builder.AddProperty(property, desiredReturnType),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InvalidProperty_ReturnTypeOverride, property.Name)));

        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding
        /// an instance event to the builder.
        /// </summary>
        [Test]
        public void AddEvent_Instance()
        {
            AssertAddEventBehavior(__EventTestType.InstanceEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                // Declare methods and data used in verifying event invocation.
                int[] eventData = { 0, 0 };
                object[] incBy10 = { new EventHandler<EventArgs>((o, e) => eventData[0] += 10) };
                object[] incBy20 = { new EventHandler<EventArgs>((o, e) => eventData[1] += 20) };

                // Verify the behavior of the generated proxy.
                object proxyInstance = Activator.CreateInstance(proxy);

                // No event handlers.
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 0, 0 }));

                // Add event handler.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 0 }));

                // Remove event handler.
                proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, incBy10);
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 20 }));

                // Multicast.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 60 }));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding
        /// an instance event from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddEvent_Instance_GenericRealSubjectType()
        {
            AssertAddEventBehavior(__EventTestType<UnhandledExceptionEventArgs>.InstanceEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(UnhandledExceptionEventArgs));
                proxyEvent = specializedProxy.GetEvent(proxyEvent.Name);
                proxyRaiseEventMethod = specializedProxy.GetMethod(proxyRaiseEventMethod.Name);

                // Declare methods and data used in verifying event invocation.
                int[] eventData = { 0, 0 };
                object[] incBy10 = { new EventHandler<UnhandledExceptionEventArgs>((o, e) => eventData[0] += 10) };
                object[] incBy20 = { new EventHandler<UnhandledExceptionEventArgs>((o, e) => eventData[1] += 20) };

                // Verify the behavior of the generated proxy.
                object proxyInstance = Activator.CreateInstance(specializedProxy);

                // No event handlers.
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 0, 0 }));

                // Add event handler.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 0 }));

                // Remove event handler.
                proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, incBy10);
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 20 }));

                // Multicast.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EquivalentTo(new int[] { 10, 60 }));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding
        /// a static event to the builder.
        /// </summary>
        [Test]
        public void AddEvent_Static()
        {
            AssertAddEventBehavior(__EventTestType.StaticEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                typeof(__EventTestType).TypeInitializer.Invoke(null, null);

                try
                {
                    // Declare methods and data used in verifying event invocation.
                    object[] incBy10 = { new EventHandler<EventArgs>(__EventTestType.IncBy10) };
                    object[] incBy20 = { new EventHandler<EventArgs>(__EventTestType.IncBy20) };

                    // Verify the behavior of the generated proxy.
                    object proxyInstance = Activator.CreateInstance(proxy);

                    // No event handlers.
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType.StaticEventData, Is.EquivalentTo(new int[] { 0, 0 }));

                    // Add event handler.
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType.StaticEventData, Is.EquivalentTo(new int[] { 10, 0 }));

                    // Remove event handler.
                    proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, incBy10);
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType.StaticEventData, Is.EquivalentTo(new int[] { 10, 20 }));

                    // Multicast.
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType.StaticEventData, Is.EquivalentTo(new int[] { 10, 60 }));
                }
                finally
                {
                    typeof(__EventTestType).TypeInitializer.Invoke(null, null);
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding
        /// a static event from a generic type to the builder.
        /// </summary>
        [Test]
        public void AddEvent_Static_GenericRealSubjectType()
        {
            AssertAddEventBehavior(__EventTestType<ResolveEventArgs>.StaticEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(ResolveEventArgs));
                proxyEvent = specializedProxy.GetEvent(proxyEvent.Name);
                proxyRaiseEventMethod = specializedProxy.GetMethod(proxyRaiseEventMethod.Name);

                typeof(__EventTestType<ResolveEventArgs>).TypeInitializer.Invoke(null, null);

                try
                {
                    // Declare methods and data used in verifying event invocation.
                    object[] incBy10 = { new EventHandler<ResolveEventArgs>(__EventTestType<ResolveEventArgs>.IncBy10) };
                    object[] incBy20 = { new EventHandler<ResolveEventArgs>(__EventTestType<ResolveEventArgs>.IncBy20) };

                    // Verify the behavior of the generated proxy.
                    object proxyInstance = Activator.CreateInstance(specializedProxy);

                    // No event handlers.
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType<ResolveEventArgs>.StaticEventData, Is.EquivalentTo(new int[] { 0, 0 }));

                    // Add event handler.
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType<ResolveEventArgs>.StaticEventData, Is.EquivalentTo(new int[] { 10, 0 }));

                    // Remove event handler.
                    proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, incBy10);
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType<ResolveEventArgs>.StaticEventData, Is.EquivalentTo(new int[] { 10, 20 }));

                    // Multicast.
                    proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy20);
                    proxyRaiseEventMethod.Invoke(proxyInstance, null);
                    Assert.That(__EventTestType<ResolveEventArgs>.StaticEventData, Is.EquivalentTo(new int[] { 10, 60 }));
                }
                finally
                {
                    typeof(__EventTestType<ResolveEventArgs>).TypeInitializer.Invoke(null, null);
                }
            });
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with an event from an invalid type.
        /// </summary>
        [Test]
        public void AddEvent_InvalidEvent()
        {
            EventInfo eventInfo = __EventTestType.InstanceEvent;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddEvent, eventInfo)),  // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_MethodNotMemberOfRealSubject, eventInfo.GetAddMethod().Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with an instance event from an abstract type.
        /// </summary>
        [Test]
        public void AddEvent_InvalidInstanceEvent()
        {
            EventInfo eventInfo = __AbstractType.InstanceEvent;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddEvent, eventInfo)),  // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(
                    String.Format(Resources.Error_InstanceMethodAddedFromAbstractType, eventInfo.GetAddMethod().Name)));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with a private event.
        /// </summary>
        [Test]
        public void AddEvent_PrivateEvent()
        {
            EventInfo eventInfo = __EventTestType.PrivateEvent;
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));

            Assert.That(
                new TestDelegate(Bind.First(builder.AddEvent, eventInfo)),  // TODO: Remove TestDelegate iff NUnit accepts Action instead of TestDelegate
                Throws.InvalidOperationException.With.Message.EqualTo(Resources.Error_MemberNotPublic));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with a null argument.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddEvent_NullEvent()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));
            builder.AddEvent(null);
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding an
        /// inherited, non-virtual event.
        /// </summary>
        [Test]
        public void AddEvent_Inherited()
        {
            AssertAddEventBehavior(__DerivedSubjectType.NonVirtualEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                // Declare methods and data used in verifying event invocation.
                int eventData = 0;
                object[] incBy10 = { new EventHandler<EventArgs>((o, e) => eventData += 10) };

                // Verify the behavior of the generated proxy.
                object proxyInstance = Activator.CreateInstance(proxy);

                // No event handlers.
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EqualTo(0));

                // Add event handler.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EqualTo(10));

                // Remove event handler.
                proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, incBy10);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EqualTo(10));

                // Multicast.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10);
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, incBy10); 
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
                Assert.That(eventData, Is.EqualTo(30));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when adding an
        /// virtual, overriden property.
        /// </summary>
        [Test]
        public void AddEvent_Override()
        {
            AssertAddEventBehavior(__DerivedSubjectType.VirtualEvent,
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                // Declare methods and data used in verifying event invocation.
                object[] throwException = { new EventHandler<EventArgs>((o, e) => Assert.Fail()) };

                // Verify the behavior of the generated proxy.
                object proxyInstance = Activator.CreateInstance(proxy);

                // No event handlers.
                proxyRaiseEventMethod.Invoke(proxyInstance, null);

                // Add event handler.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, throwException);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);

                // Remove event handler.
                proxyEvent.GetRemoveMethod(true).Invoke(proxyInstance, throwException);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);

                // Multicast.
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, throwException);
                proxyEvent.GetAddMethod(true).Invoke(proxyInstance, throwException);
                proxyRaiseEventMethod.Invoke(proxyInstance, null);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when the
        /// same event is added to the builder more than once.
        /// </summary>
        [Test]
        public void AddEvent_DuplicateEvent()
        {
            EventInfo eventInfo = __EventTestType.InstanceEvent;

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));
            builder.AddEvent(eventInfo);

            Assert.That(
                () => builder.AddEvent(eventInfo),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));            
        }

        /// <summary>
        /// Verifies that XML doc comments are produced as part of adding
        /// an event to the proxy builder.
        /// </summary>
        [Test]
        public void AddEvent_XmlDocComments()
        {
            Type realSubjectType = typeof(__EventTestType);
            VerifyBehavior_AddMember_XmlDocComments(realSubjectType, realSubjectType.GetEvents());
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// the given property (get/set methods) matches the signature
        /// of an existing method in the builder.
        /// </summary>
        [Test]
        public void AddProperty_DuplicatesExistingMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));
        
            builder.AddMethod(__PropertyTestType.GetterProperty.GetGetMethod());

            Assert.That(
                () => builder.AddProperty(__PropertyTestType.GetterProperty),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// the given method matches the signature of an existing
        /// property (get/set methods) in the builder.
        /// </summary>
        [Test]
        public void AddMethod_DuplicatesExistingProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));

            builder.AddProperty(__PropertyTestType.GetterProperty);

            Assert.That(
                () => builder.AddMethod(__PropertyTestType.GetterProperty.GetGetMethod()),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when
        /// the given event (add/remove methods) matches the signature
        /// of an existing method in the builder.
        /// </summary>
        [Test]
        public void AddEvent_DuplicatesExistingMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));

            builder.AddMethod(__EventTestType.InstanceEvent.GetAddMethod());

            Assert.That(
                () => builder.AddEvent(__EventTestType.InstanceEvent),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// the given method matches the signature of an existing
        /// event (add/remove methods) in the builder.
        /// </summary>
        [Test]
        public void AddMethod_DuplicatesExistingEvent()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));

            builder.AddEvent(__EventTestType.InstanceEvent);

            Assert.That(
                () => builder.AddMethod(__EventTestType.InstanceEvent.GetAddMethod()),
                Throws.ArgumentException.With.Message.EqualTo(Resources.Error_DuplicateMember));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Asserts that the given interface contains the expected
        /// attributes as prescribed the implementation of the ProxyTypeBuilder.
        /// </summary>
        /// 
        /// <param name="interfaceType">
        /// The type to validate.
        /// </param>
        /// 
        /// <param name="expectedName">
        /// The expected interface name.
        /// </param>
        /// 
        /// <param name="expectedNamespace">
        /// The expected interface namespace.
        /// </param>
        private static void AssertInterfaceAttributes(Type interfaceType, string expectedName, string expectedNamespace)
        {
            Assert.IsNotNull(interfaceType);
            Assert.That(interfaceType.Name, Is.EqualTo(expectedName));
            Assert.That(interfaceType.Namespace, Is.EqualTo(expectedNamespace));
            Assert.That(interfaceType.IsInterface);
            Assert.That(interfaceType.IsAbstract);
            Assert.That(interfaceType.IsPublic);
        }

        /// <summary>
        /// Asserts that the given proxy contains the expected
        /// attributes as prescribed the implementation of the ProxyTypeBuilder.
        /// </summary>
        /// 
        /// <param name="proxyType">
        /// The type to validate.
        /// </param>
        /// 
        /// <param name="interfaceType">
        /// The interface that the proxy is expected to implement.
        /// </param>
        /// 
        /// <param name="expectedName">
        /// The expected proxy name.
        /// </param>
        /// 
        /// <param name="expectedNamespace">
        /// The expected proxy namespace.
        /// </param>
        private static void AssertProxyAttributes(Type proxyType, Type interfaceType, string expectedName, string expectedNamespace)
        {
            Assert.IsNotNull(proxyType);
            Assert.That(proxyType.Name, Is.EqualTo(expectedName));
            Assert.That(proxyType.Namespace, Is.EqualTo(expectedNamespace));
            Assert.That(proxyType.Namespace, Is.EqualTo(interfaceType.Namespace));
            Assert.That(proxyType.IsClass);
            Assert.That(!proxyType.IsAbstract);
            Assert.That(proxyType.IsPublic);
            Assert.That(proxyType.IsSealed);
            Assert.That(proxyType.GetInterfaces(), Has.Member(interfaceType));
        }

        /// <summary>
        /// Verifies the construction of a given type.
        /// </summary>
        /// 
        /// <param name="typeToInstantiate">
        /// The type to instantiate.
        /// </param>
        /// 
        /// <param name="constructionArgs">
        /// The constructor arguments, if any (null => no arguments).
        /// </param>
        /// 
        /// <param name="expectedValue">
        /// The expected constructor return value (via ApplicationException).
        /// </param>
        private static void AssertConstructorInvoked(Type typeToInstantiate, object[] constructionArgs, string expectedValue)
        {
            try
            {
                Activator.CreateInstance(typeToInstantiate, constructionArgs);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.That(ex.InnerException.Message, Is.EqualTo(expectedValue));
            }
        }

        /// <summary>
        /// Validates the state of a property after construction by the builder.
        /// </summary>
        /// 
        /// <param name="expectedProperty">
        /// The property from the builder's real subject type that models the actual property.
        /// </param>
        /// 
        /// <param name="actualProperty">
        /// The property to validate.
        /// </param>
        private static void AssertPropertyState(PropertyInfo expectedProperty, PropertyInfo actualProperty)
        {
            Assert.That(actualProperty, Is.Not.Null);
            Assert.That(actualProperty.CanRead, Is.EqualTo(expectedProperty.CanRead));
            Assert.That(actualProperty.CanWrite, Is.EqualTo(expectedProperty.CanWrite));

            if (actualProperty.PropertyType.IsGenericParameter)
            {
                Assert.That(actualProperty.PropertyType.Name, Is.EqualTo(expectedProperty.PropertyType.Name));
            }
            else
            {
                Assert.That(actualProperty.PropertyType, Is.EqualTo(expectedProperty.PropertyType));
            }
        }

        /// <summary>
        /// Validates the state of an event after construction by the builder.
        /// </summary>
        /// 
        /// <param name="expectedEvent">
        /// The event from the builder's real subject type that models the actual event.
        /// </param>
        /// 
        /// <param name="actualEvent">
        /// The event to validate.
        /// </param>
        private static void AssertEventState(EventInfo expectedEvent, EventInfo actualEvent)
        {
            Assert.That(actualEvent, Is.Not.Null);
            Assert.That(actualEvent.GetRaiseMethod(), Is.Null);

            if (actualEvent.EventHandlerType.IsGenericType)
            {
                Assert.That(actualEvent.EventHandlerType.Name, Is.EqualTo(expectedEvent.EventHandlerType.Name));
            }
            else
            {
                Assert.That(actualEvent.EventHandlerType, Is.EqualTo(expectedEvent.EventHandlerType));
            }
        }

        /// <summary>
        /// Asserts the expected behavior of the builder's AddProperty() method.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeProperty">
        /// The property from the real subject type that is passed to the
        /// proxy type builder.
        /// </param>
        /// 
        /// <param name="assertPropertyInvocation">
        /// Delegate that asserts the expected behavior of the generated proxy property.
        /// </param>
        private static void AssertAddPropertyBehavior(PropertyInfo realSubjectTypeProperty, Action<Type, PropertyInfo> assertPropertyInvocation)
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeProperty.DeclaringType);
            builder.AddProperty(realSubjectTypeProperty);

            // Verify the property on the proxy inteface.
            Type proxyInterface = builder.CreateInterface();
            AssertPropertyState(realSubjectTypeProperty, proxyInterface.GetProperty(realSubjectTypeProperty.Name));

            // Verify the property on the proxy.
            Type proxy = builder.CreateProxy();
            PropertyInfo proxyProperty = proxy.GetProperty(realSubjectTypeProperty.Name);

            AssertPropertyState(realSubjectTypeProperty, proxyProperty);
            
            // Verify the behavior of the generated proxy.
            assertPropertyInvocation(proxy, proxyProperty);
        }

        /// <summary>
        /// Asserts the expected behavior of the AddProperty() method when the
        /// given property contains a getter/setter that is less accessible than
        /// the access modifier of the given property.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeProperty">
        /// The property from the real subject type that is passed to the
        /// proxy type builder.
        /// </param>
        private static void AssertInaccessiblePropertyBehavior(PropertyInfo realSubjectTypeProperty)
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeProperty.DeclaringType);

            builder.AddProperty(realSubjectTypeProperty);
            PropertyInfo property = builder.CreateProxy().GetProperty(realSubjectTypeProperty.Name);

            Assert.That(property.CanRead, Is.EqualTo(realSubjectTypeProperty.GetGetMethod() != null));
            Assert.That(property.CanWrite, Is.EqualTo(realSubjectTypeProperty.GetSetMethod() != null));
        }

        /// <summary>
        /// Asserts the expected behavior of the builder's AddEvent() method.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeEvent">
        /// The event from the real subject type that is passed to the
        /// proxy type builder.
        /// </param>
        /// 
        /// <param name="assertEventInvocation">
        /// Delegate that asserts the expected behavior of the generated proxy event.
        /// </param>
        private static void AssertAddEventBehavior(EventInfo realSubjectTypeEvent, Action<Type, EventInfo, MethodInfo> assertEventInvocation)
        {
            string raiseEventMethodName = "Raise" + realSubjectTypeEvent.Name;

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectTypeEvent.DeclaringType);
            builder.AddEvent(realSubjectTypeEvent);
            builder.AddMethod(realSubjectTypeEvent.DeclaringType.GetMethod(raiseEventMethodName));

            // Verify the event on the proxy interface.
            Type proxyInterface = builder.CreateInterface();
            AssertEventState(realSubjectTypeEvent, proxyInterface.GetEvent(realSubjectTypeEvent.Name));

            // Verify the event on the proxy.
            Type proxy = builder.CreateProxy();
            EventInfo proxyEvent = proxy.GetEvent(realSubjectTypeEvent.Name);
            AssertEventState(realSubjectTypeEvent, proxyEvent);
            
            // Verify the behavior of the generated event.
            assertEventInvocation(proxy, proxyEvent, proxy.GetMethod(raiseEventMethodName));
        }

        /// <summary>
        /// Creates a transient ModuleBuilder for testing purposes.
        /// </summary>
        private static ModuleBuilder CreateTransientModule()
        {
            return AppDomain.CurrentDomain
                .DefineDynamicAssembly(new AssemblyName("__transientAssembly"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("__transientModule");
        }

        /// <summary>
        /// Creates a factory method that returns the given builder.
        /// </summary>
        /// 
        /// <param name="builderToCreate">
        /// The builder to return from the factory method.
        /// </param>
        private static CreateXDCBuilderDelegate CreateXDCBuilderFactoryDelegate(XmlDocCommentBuilderBase builderToCreate)
        {
            return Functor.Idempotency<Type, Type, Type, XmlDocCommentBuilderBase>(builderToCreate);
        }

        /// <summary>
        /// Verfies the XML doc comment production behavior of the
        /// Add* methods on the ProxyTypeBuilder.
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of member to verify; determines the Add* method overload.
        /// </typeparam>
        /// 
        /// <param name="realSubjectType">
        /// The real subject type to use as part of the test.
        /// </param>
        /// 
        /// <param name="expectedMembers">
        /// The members of the real subject type for which XML doc comments are generated.
        /// </param>
        private static void VerifyBehavior_AddMember_XmlDocComments<TMember>(Type realSubjectType, TMember[] expectedMembers)
            where TMember : MemberInfo
        {
            XmlDocCommentBuilderBase xdcBuilder = MockRepository.GenerateMock<XmlDocCommentBuilderBase>();

            MethodInfo addMember = typeof(XmlDocCommentBuilderBase)
                    .GetMethods(NonPublicInstance)
                    .Single(CreateBuilderMethodPredicate<TMember>());

            foreach (TMember member in expectedMembers)
            {
                xdcBuilder.Expect(x => addMember.Invoke(x, new object[] { member }));
            }

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType, CreateXDCBuilderFactoryDelegate(xdcBuilder), CreateTransientModule());
            Action<TMember> addMemberToProxyTypeBuilder = Delegate.CreateDelegate(
                typeof(Action<TMember>),
                builder,
                builder.GetType().GetMethods().Single(CreateBuilderMethodPredicate<TMember>())) as Action<TMember>;

            Array.ForEach(expectedMembers, addMemberToProxyTypeBuilder);

            xdcBuilder.VerifyAllExpectations();
        }

        /// <summary>
        /// Creates a delegate that is used to filter methods on a builder type.
        /// A method is returned if the method name starts with "Add" and if the
        /// method contains one argument of type TMember.
        /// </summary>
        private static Func<MethodInfo, bool> CreateBuilderMethodPredicate<TMember>()
            where TMember : MemberInfo
        {
            return delegate(MethodInfo method)
            {
                bool result = false;
                if (method.Name.StartsWith("Add"))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    result = parameters.Length == 1 && parameters[0].ParameterType == typeof(TMember);
                }

                return result;
            };
        }

        /// <summary>
        /// Creates a delegate that constructs a proxy type builder
        /// with the default namespace and for the given real subject type.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The real subjec typ used to initialize the proxy type builder.
        /// </param>
        private static TestDelegate CreateProxyTypeBuilderDelegate(Type realSubjectType)
        {
            return () => new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly string DefaultNamespace = "root";
        private static readonly BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly BindingFlags ProxyMethodInvocationFlags =
            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public;

        #endregion
    }
}
