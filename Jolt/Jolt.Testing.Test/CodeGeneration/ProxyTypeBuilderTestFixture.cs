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

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
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
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type, root namespace,
        /// and target module are provided.
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_Module()
        {
            Type proxiedType = typeof(string);
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
        }

        /// <summary>
        /// Verifies the internal construction of the class.
        /// </summary>
        [Test]
        public void Construction_Internal()
        {
            Type proxiedType = typeof(string);
            ModuleBuilder expectedModule = CreateTransientModule();

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, proxiedType, expectedModule);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));
            Assert.That(builder.Module, Is.SameAs(expectedModule));
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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance);

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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance);

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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance);

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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance);

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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance );

            Assert.That(proxiedTypeField, Is.Not.Null);
            Assert.That(proxiedTypeField.FieldType, Is.EqualTo(proxiedType));
            Assert.That(proxiedTypeField.IsInitOnly && proxiedTypeField.IsPrivate && !proxiedTypeField.IsStatic);
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

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(proxiedTypeField, Is.Not.Null);
            Assert.That(proxiedTypeField.FieldType, Is.Not.EqualTo(proxiedType));
            Assert.That(proxiedTypeField.FieldType.Name, Is.EqualTo(proxiedType.Name));
            Assert.That(proxiedTypeField.FieldType.Namespace, Is.EqualTo(proxiedType.Namespace));
            Assert.That(proxiedTypeField.IsInitOnly && proxiedTypeField.IsPrivate && !proxiedTypeField.IsStatic);
        }

        /// <summary>
        /// Verifies the initialziation of constructors in the proxied type,
        /// when the proxied type is not static.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_ConstructorInitialization()
        {
            Type realSubjectType = typeof(__ConstructorTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
            Type proxy = builder.CreateProxy();
            ConstructorInfo[] expectedConstructors = realSubjectType.GetConstructors();
            ConstructorInfo[] actualConstructors = proxy.GetConstructors();

            // Both proxy and proxied have the same number of constructors and same arguments.
            Assert.That(actualConstructors, Has.Length(expectedConstructors.Length));

            // Proxy constructor forwards to the proxied type constructor.
            AssertConstructorInvoked(proxy, null, "0");
            AssertConstructorInvoked(proxy, new object[] { 123 }, "123");
            AssertConstructorInvoked(proxy, new object[] { 456, 789 }, "456789");
        }

        /// <summary>
        /// Verifies the initialziation of constructors in the proxied type,
        /// when the proxied type is generic and not static.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_Generic_ConstructorInitialization()
        {
            Type realSubjectType = typeof(__ConstructorTestType<,>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
            Type specializedProxy = builder.CreateProxy().MakeGenericType(typeof(string), typeof(TimeSpan));
            ConstructorInfo[] expectedConstructors = realSubjectType.GetConstructors();
            ConstructorInfo[] actualConstructors = specializedProxy.GetConstructors();

            // Both proxy and proxied have the same number of constructors and same arguments.
            Assert.That(actualConstructors, Has.Length(expectedConstructors.Length));

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
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void Construction_RealSubjectInterfaceType()
        {
            new ProxyTypeBuilder(DefaultNamespace, typeof(__InterfaceType));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when a delegate type
        /// is given to the proxy type builder as the real subject type.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void Construction_RealSubjectDelegateType()
        {
            new ProxyTypeBuilder(DefaultNamespace, typeof(Action));
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "InstanceMethod" && !m.IsGenericMethod && m.GetParameters().Length == 0);
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
            MethodInfo realSubejctTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "InstanceMethod" && m.IsGenericMethod && m.GetParameters().Length == 0);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("StaticMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "StaticMethod" && !m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "StaticMethod" && m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("VoidReturnValueMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "VoidReturnValueMethod" && !m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "VoidReturnValueMethod" && m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("ManyArgumentsMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "ManyArgumentsMethod" && !m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "ManyArgumentsMethod" && m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("ParamsArrayArgumentsMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "ParamsArrayArgumentsMethod" && !m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "ParamsArrayArgumentsMethod" && m.IsGenericMethod);
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
            Type realSubjectType = typeof(__MethodTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            // Verify that the following operations do not throw.
            builder.AddMethod(realSubjectType.GetMethod("InstanceMethod", Type.EmptyTypes));
            builder.AddMethod(realSubjectType.GetMethod("InstanceMethod", new Type[] { typeof(int) }));
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
            Type realSubjectType = typeof(__MethodTestType<,,>);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            // Verify that the following operations do not throw.
            builder.AddMethod(realSubjectType.GetMethods().Single(m => m.Name == "InstanceMethod" && m.GetParameters().Length == 0 && !m.IsGenericMethod));
            builder.AddMethod(realSubjectType.GetMethods().Single(m => m.Name == "InstanceMethod" && m.GetParameters().Length == 1 && !m.IsGenericMethod));
            builder.AddMethod(realSubjectType.GetMethods().Single(m => m.Name == "InstanceMethod" && m.GetParameters().Length == 0 && m.IsGenericMethod));
            builder.AddMethod(realSubjectType.GetMethods().Single(m => m.Name == "InstanceMethod" && m.GetParameters().Length == 1 && m.IsGenericMethod));
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType).GetMethod("OutParameterMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "OutParameterMethod" && !m.IsGenericMethod);
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
            MethodInfo realSubjectTypeMethod = typeof(__MethodTestType<,,>).GetMethods().Single(m => m.Name == "OutParameterMethod" && m.IsGenericMethod);
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
                Assert.That(methodArgs[i], Is.InstanceOfType(expectedTypes[i]));
            }
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a method from an invalid type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_InvalidMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));
            builder.AddMethod(typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with an instance method from an abstract type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_InvalidInstanceMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));
            builder.AddMethod(typeof(__AbstractType).GetMethod("ToString"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a private method.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_PrivateMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__MethodTestType));
            builder.AddMethod(typeof(__MethodTestType).GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance,
                null, Type.EmptyTypes, null));
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
            MethodInfo realSubjectTypeMethod = typeof(__DerivedSubjectType).GetMethod("NonVirtualMethod");
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
            MethodInfo realSubjectTypeMethod = typeof(__DerivedSubjectType).GetMethod("VirtualMethod");
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
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddMethod_DuplicateMethod()
        {
            Type expectedType = typeof(__MethodTestType);
            MethodInfo method = expectedType.GetMethod("InstanceMethod", Type.EmptyTypes);

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, expectedType);
            builder.AddMethod(method);
            builder.AddMethod(method);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// an instance property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Instance()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("InstanceProperty"),
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
            AssertAddPropertyBehavior(typeof(__PropertyTestType<,,>).GetProperty("InstanceProperty"),
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
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("StaticProperty"),
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
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
            AssertAddPropertyBehavior(typeof(__PropertyTestType<,,>).GetProperty("StaticProperty"),
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(DateTime), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

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
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("Getter"), delegate { });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a setter-only property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Setter()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("Setter"), delegate { });
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a property from an invalid type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_InvalidProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));
            builder.AddProperty(typeof(__PropertyTestType).GetProperty("Getter"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with an instance property from an abstract type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_InvalidInstanceProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));
            builder.AddProperty(typeof(__AbstractType).GetProperty("InstanceProperty"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a private property.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_PrivateProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__PropertyTestType));
            builder.AddProperty(typeof(__PropertyTestType).GetProperty("PrivateProperty", BindingFlags.NonPublic | BindingFlags.Instance));
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
            AssertAddPropertyBehavior(typeof(__DerivedSubjectType).GetProperty("NonVirtualProperty"),
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
            AssertAddPropertyBehavior(typeof(__DerivedSubjectType).GetProperty("VirtualProperty"),
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
            Type expectedType = typeof(__PropertyTestType);
            PropertyInfo property = expectedType.GetProperty("Getter");

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, expectedType);
            builder.AddProperty(property);
            builder.AddProperty(property);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when the
        /// same setter property is added to the builder more than once.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddProperty_DuplicateSetterProperty()
        {
            Type expectedType = typeof(__PropertyTestType);
            PropertyInfo property = expectedType.GetProperty("Setter");

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, expectedType);
            builder.AddProperty(property);
            builder.AddProperty(property);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// a proprety with no getter or setter is added to the builder.
        /// </summary>
        [Test, Ignore]
        public void AddProperty_UnsupportedProperty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies the behavior fo the AddProperty() method when
        /// an indexer property is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("Item", new Type[] {typeof(int)}),
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
            Type[] genericArguments = typeof(__PropertyTestType<,,>).GetGenericArguments();
            AssertAddPropertyBehavior(typeof(__PropertyTestType<,,>).GetProperty("Item", new Type[] { genericArguments[0] }),
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
            AssertAddPropertyBehavior(typeof(__PropertyTestType).GetProperty("Item", new Type[] {typeof(int), typeof(int), typeof(int)}),
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
            AssertAddPropertyBehavior(typeof(__PropertyTestType<,,>).GetProperty("Item", typeof(__PropertyTestType<,,>).GetGenericArguments()),
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(DateTime), typeof(TimeSpan), typeof(StringBuilder));
                proxyProperty = specializedProxy.GetProperty(proxyProperty.Name);

                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(specializedProxy);
                object[] getterArguments = { DateTime.Now, TimeSpan.Zero, new StringBuilder() };
                string result = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(result, Is.EqualTo("complex-value"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "indexer-value", new object[] { DateTime.Parse("2008-01-01"), TimeSpan.FromDays(0.5), new StringBuilder("test") });
                result = (string)proxyProperty.GetValue(proxyInstance, getterArguments);
                Assert.That(result, Is.EqualTo("indexer-value1/1/2008 00:00:0012:00:00test"));
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
        /// Verifies the behavior of the AddEvent() method when adding
        /// an instance event to the builder.
        /// </summary>
        [Test]
        public void AddEvent_Instance()
        {
            AssertAddEventBehavior(typeof(__EventTestType).GetEvent("InstanceEvent"),
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
            AssertAddEventBehavior(typeof(__EventTestType<>).GetEvent("InstanceEvent"),
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
            AssertAddEventBehavior(typeof(__EventTestType).GetEvent("StaticEvent"),
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                int[] initialState = __EventTestType.StaticEventData.Clone() as int[];

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
                    __EventTestType.StaticEventData = initialState;
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
            AssertAddEventBehavior(typeof(__EventTestType<>).GetEvent("StaticEvent"),
            delegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod)
            {
                Type specializedProxy = proxy.MakeGenericType(typeof(ResolveEventArgs));
                proxyEvent = specializedProxy.GetEvent(proxyEvent.Name);
                proxyRaiseEventMethod = specializedProxy.GetMethod(proxyRaiseEventMethod.Name);

                int[] initialState = __EventTestType<ResolveEventArgs>.StaticEventData.Clone() as int[];

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
                    __EventTestType<ResolveEventArgs>.StaticEventData = initialState;
                }
            });
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with an event from an invalid type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddEvent_InvalidEvent()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(string));
            builder.AddEvent(typeof(__EventTestType).GetEvent("InstanceEvent"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with an instance event from an abstract type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddEvent_InvalidInstanceEvent()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__AbstractType));
            builder.AddEvent(typeof(__AbstractType).GetEvent("InstanceEvent"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddEvent()
        /// method is invoked with a private event.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddEvent_PrivateEvent()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, typeof(__EventTestType));
            builder.AddEvent(typeof(__EventTestType).GetEvent("PrivateEvent", BindingFlags.NonPublic | BindingFlags.Instance));
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
            AssertAddEventBehavior(typeof(__DerivedSubjectType).GetEvent("NonVirtualEvent"),
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
            AssertAddEventBehavior(typeof(__DerivedSubjectType).GetEvent("VirtualEvent"),
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
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddEvent_DuplicateEvent()
        {
            Type realSubjectType = typeof(__EventTestType);
            EventInfo eventInfo = realSubjectType.GetEvent("InstanceEvent");

            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);
            builder.AddEvent(eventInfo);
            builder.AddEvent(eventInfo);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// the given property (get/set methods) matches the signature
        /// of an existing method in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddProperty_DuplicatesExistingMethod()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            builder.AddMethod(realSubjectType.GetMethod("get_Getter"));
            builder.AddProperty(realSubjectType.GetProperty("Getter"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// the given method matches the signature of an existing
        /// property (get/set methods) in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddMethod_DuplicatesExistingProperty()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            builder.AddProperty(realSubjectType.GetProperty("Getter"));
            builder.AddMethod(realSubjectType.GetMethod("get_Getter"));
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when
        /// the given event (add/remove methods) matches the signature
        /// of an existing method in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddEvent_DuplicatesExistingMethod()
        {
            Type realSubjectType = typeof(__EventTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            builder.AddMethod(realSubjectType.GetMethod("add_InstanceEvent"));
            builder.AddEvent(realSubjectType.GetEvent("InstanceEvent"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// the given method matches the signature of an existing
        /// event (add/remove methods) in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddMethod_DuplicatesExistingEvent()
        {
            Type realSubjectType = typeof(__EventTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder(DefaultNamespace, realSubjectType);

            builder.AddEvent(realSubjectType.GetEvent("InstanceEvent"));
            builder.AddMethod(realSubjectType.GetMethod("add_InstanceEvent"));
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
            Assert.That(proxyType.GetInterfaces(), List.Contains(interfaceType));
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
        private static void AssertAddPropertyBehavior(PropertyInfo realSubjectTypeProperty, AssertDynamicPropertyInvocationDelegate assertPropertyInvocation)
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
        private static void AssertAddEventBehavior(EventInfo realSubjectTypeEvent, AssertDynamicEventInvocationDelegate assertEventInvocation)
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
                .DefineDynamicAssembly(new AssemblyName("__transientAssembly"), AssemblyBuilderAccess.ReflectionOnly)
                .DefineDynamicModule("__transientModule");
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly string DefaultNamespace = "root";
        private static readonly BindingFlags ProxyMethodInvocationFlags =
            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public;

        #endregion

        #region delegate types supporting unit tests ----------------------------------------------

        private delegate void AssertDynamicPropertyInvocationDelegate(Type proxy, PropertyInfo proxyProperty);
        private delegate void AssertDynamicEventInvocationDelegate(Type proxy, EventInfo proxyEvent, MethodInfo proxyRaiseEventMethod);

        #endregion
    }
}
