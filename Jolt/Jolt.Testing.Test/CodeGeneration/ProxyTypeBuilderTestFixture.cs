// ----------------------------------------------------------------------------
// ProxyTypeBuilderTestFixture.cs
//
// Contains the definition of the ProxyTypeBuilderTestFixture class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/31/2007 19:49:19
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using JTCG = Jolt.Testing.CodeGeneration;
using Jolt.Testing.CodeGeneration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    /// <summary>
    /// Verifies the implementation of the ProxyTypeBuilder class.
    /// The methods CreateProxy() and CreateInterface() are tested indirectly
    /// as they validate the effects of the other class methods.
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
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType);

            Assert.That(builder.ProxiedType, Is.SameAs(proxiedType));

            Assert.That(builder.Module.IsTransient());
            Assert.That(builder.Module.Assembly.GetName().Name, Is.EqualTo("__transientAssembly"));
            Assert.That(!builder.Module.Assembly.ReflectionOnly);
        }

        /// <summary>
        /// Verifies the construction of the class when the proxied type. root namespace,
        /// and target module are provided..
        /// </summary>
        [Test]
        public void Construction_Namespace_ProxiedType_Module()
        {
            Type proxiedType = typeof(string);

            ModuleBuilder expectedModule = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("__transientAssembly"),
                    AssemblyBuilderAccess.ReflectionOnly).DefineDynamicModule("__transientModule");

            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType, expectedModule);

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
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType);

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
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType);

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
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType);

            FieldInfo proxiedTypeField = builder.CreateProxy().GetField("m_realSubject", BindingFlags.NonPublic | BindingFlags.Instance );

            Assert.That(proxiedTypeField, Is.Not.Null);
            Assert.That(proxiedTypeField.IsInitOnly && proxiedTypeField.IsPrivate && !proxiedTypeField.IsStatic);
        }

        /// <summary>
        /// Verifies the initialziation of constructors in the proxied type,
        /// when the proxied type is not static.
        /// </summary>
        [Test]
        public void Construction_NonStaticClass_ConstructorInitialization()
        {
            Type proxiedType = typeof(__ConstructorTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", proxiedType);
            Type proxy = builder.CreateProxy();
            ConstructorInfo[] proxiedTypeConstructors = proxiedType.GetConstructors();
            ConstructorInfo[] proxyTypeConstructors = proxy.GetConstructors();

            // Both proxy and proxied have the same number of constructors and same arguments.
            Assert.That(proxiedTypeConstructors.Length, Is.EqualTo(proxyTypeConstructors.Length));
            for (int i = 0; i < proxiedTypeConstructors.Length; ++i)
            {
                Assert.That(JTCG.Convert.ToParameterTypes(proxiedTypeConstructors[i].GetParameters()),
                    Is.EqualTo(JTCG.Convert.ToParameterTypes(proxyTypeConstructors[i].GetParameters())));
            }

            // Proxy constructor forwards to the proxied type constructor.
            AssertConstructorInvoked(proxy, null, "0");
            AssertConstructorInvoked(proxy, new object[] { 123 }, "123");
            AssertConstructorInvoked(proxy, new object[] { 456, 789 }, "456789");
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when an interface type
        /// is given to the proxy type builder as the real subject type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Construction_RealSubjectInterfaceType()
        {
            new ProxyTypeBuilder("root", typeof(__InterfaceType));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when a delegate type
        /// is given to the proxy type builder as the real subject type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Construction_RealSubjectDelegateType()
        {
            new ProxyTypeBuilder("root", typeof(AssertDynamicMethodInvocationDelegate));
        }

        /// <summary>
        /// Verifies the default behavior of the CreateInterface() method,
        /// prior to adding methods.
        /// </summary>
        [Test]
        public void CreateInterface()
        {
            Type proxyInterface = new ProxyTypeBuilder("root", typeof(string)).CreateInterface();

            Assert.That(proxyInterface.IsInterface);
            Assert.That(proxyInterface.IsAbstract);
            Assert.That(proxyInterface.IsPublic);
            Assert.That(proxyInterface.Namespace, Is.EqualTo("root.System"));
            Assert.That(proxyInterface.Name, Is.EqualTo("IString"));
        }

        /// <summary>
        /// Verifies the default behavior of the CreateProxy() method,
        /// prior to adding methods.
        /// </summary>
        [Test]
        public void CreateProxy()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(string));
            Type proxy = builder.CreateProxy();
            Type proxyInterface = builder.CreateInterface();

            Assert.That(proxy.IsClass);
            Assert.That(!proxy.IsAbstract);
            Assert.That(proxy.IsPublic);
            Assert.That(proxy.IsSealed);
            Assert.That(proxy.Namespace, Is.EqualTo(proxyInterface.Namespace));
            Assert.That(proxy.Name, Is.EqualTo("StringProxy"));
            Assert.That(proxy.GetInterfaces(), List.Contains(proxyInterface));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Instance()
        {
            AssertAddMethodBehavior(typeof(__MethodTestType), "InstanceMethod", Type.EmptyTypes, delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                DateTime result = (DateTime)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null,  Activator.CreateInstance(proxy), null);
                Assert.That(result, Is.EqualTo(DateTime.Today));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// a static method to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Static()
        {
            AssertAddMethodBehavior(typeof(__MethodTestType), "StaticMethod", Type.EmptyTypes, delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                TimeSpan result = (TimeSpan)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
                Assert.That(result, Is.EqualTo(TimeSpan.FromHours(12.0)));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with no return value to the builder.
        /// </summary>
        [Test]
        public void AddMethod_VoidReturnValue()
        {
            AssertAddMethodBehavior(typeof(__MethodTestType), "VoidReturnValueMethod", Type.EmptyTypes, delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                try
                {
                    proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
                    Assert.Fail();
                }
                catch (ApplicationException ex)
                {
                    Assert.That(ex.InnerException.Message, Is.EqualTo("void-return"));
                }
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with many arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ManyArguments()
        {
            AssertAddMethodBehavior(typeof(__MethodTestType), "ManyArgumentsMethod", new Type[] { typeof(string), typeof(int), typeof(double), typeof(DateTime) },
            delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                string sResult = (string)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy),
                    new object[] { "-test-", 1, 2.3, DateTime.Today });
                Assert.That(sResult, Is.EqualTo("many-args:-test-12.3" + DateTime.Today.ToString()));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding
        /// an instance method with params[] arguments to the builder.
        /// </summary>
        [Test]
        public void AddMethod_ParamsArrayArguments()
        {
            AssertAddMethodBehavior(typeof(__MethodTestType), "ParamsArrayArgumentsMethod", new Type[] { typeof(string), typeof(object[]) },
            delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                string sResult = (string)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy),
                    new object[] { String.Empty, new object[] { 0, 1, 2, 3, 4, 5, 10 } });
                Assert.That(sResult, Is.EqualTo("params-args:01234510"));
            });
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a method from an invalid type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_InvalidMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(string));
            builder.AddMethod(typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with an instance method from an abstract type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_InvalidInstanceMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__AbstractType));
            builder.AddMethod(typeof(__AbstractType).GetMethod("ToString"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddMethod()
        /// method is invoked with a private method.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddMethod_PrivateMethod()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__MethodTestType));
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
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__MethodTestType));
            builder.AddMethod(null);
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding an
        /// inherited, non-virtual method.
        /// </summary>
        [Test]
        public void AddMethod_Inherited()
        {
            AssertAddMethodBehavior(typeof(__DerivedSubjectType), "NonVirtualMethod", Type.EmptyTypes, delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                string sResult = (string)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
                Assert.That(sResult, Is.EqualTo("Base:NonVirtual"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when adding an
        /// virtual, overriden method.
        /// </summary>
        [Test]
        public void AddMethod_Override()
        {
            AssertAddMethodBehavior(typeof(__DerivedSubjectType), "VirtualMethod", Type.EmptyTypes, delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                string sResult = (string)proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), null);
                Assert.That(sResult, Is.EqualTo("Derived:Override"));
            });
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

            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", expectedType);
            builder.AddMethod(method);
            builder.AddMethod(method);
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// two overloaded methods are added to the builder.
        /// </summary>
        [Test]
        public void AddMethod_Overload()
        {
            Type realSubjectType = typeof(__MethodTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);

            // Verify that the following operations do not throw.
            builder.AddMethod(realSubjectType.GetMethod("InstanceMethod",Type.EmptyTypes));
            builder.AddMethod(realSubjectType.GetMethod("InstanceMethod", new Type[] { typeof(int)}));
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
            AssertAddMethodBehavior(typeof(__MethodTestType), "OutParameterMethod", new Type[] { typeof(string).MakeByRefType() },
            delegate(Type proxy, string sProxyMethodName)
            {
                // Verify the behavior of the generated proxy.
                object[] methodArgs = { null };
                proxy.InvokeMember(sProxyMethodName, ProxyMethodInvocationFlags, null, Activator.CreateInstance(proxy), methodArgs, null);
                Assert.That(methodArgs[0], Is.EqualTo("out-param"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// an instance property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Instance()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "InstanceProperty",
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
        /// a static property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Static()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "StaticProperty",
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                DateTime result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(DateTime.Today));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, DateTime.Today.AddMonths(-1), null);
                result = (DateTime)proxyProperty.GetValue(proxyInstance, null);
                Assert.That(result, Is.EqualTo(DateTime.Today.AddMonths(-1)));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a getter-only property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Getter()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "Getter", delegate { });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding
        /// a setter-only property to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Setter()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "Setter", delegate { });
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a property from an invalid type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_InvalidProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(string));
            builder.AddProperty(typeof(__PropertyTestType).GetProperty("Getter"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// property is invoked with an instance method from an abstract type.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_InvalidInstanceProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__AbstractType));
            builder.AddProperty(typeof(__AbstractType).GetProperty("InstanceProperty"));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// property is invoked with a private property.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddProperty_PrivateProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__PropertyTestType));
            builder.AddProperty(typeof(__PropertyTestType).GetProperty("PrivateProperty", BindingFlags.NonPublic | BindingFlags.Instance));
        }

        /// <summary>
        /// Verifies that an appropriate exception is thrown when the AddProperty()
        /// method is invoked with a null argument.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddProperty_NullProperty()
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", typeof(__PropertyTestType));
            builder.AddProperty(null);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when adding an
        /// inherited, non-virtual property.
        /// </summary>
        [Test]
        public void AddProperty_Inherited()
        {
            AssertAddPropertyBehavior(typeof(__DerivedSubjectType), "NonVirtualProperty",
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
            AssertAddPropertyBehavior(typeof(__DerivedSubjectType), "VirtualProperty",
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

            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", expectedType);
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

            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", expectedType);
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
        /// an indexer proeprty is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "Item", new Type[] {typeof(int)},
            delegate(Type proxy, PropertyInfo proxyProperty)
            {
                // Verify the behavior of the generated proxy.
                // Get operation.
                object proxyInstance = Activator.CreateInstance(proxy);
                string sResult = (string)proxyProperty.GetValue(proxyInstance, new object[] { 0 });
                Assert.That(sResult, Is.EqualTo("simple-value"));

                // Set operation.
                proxyProperty.SetValue(proxyInstance, "new-value-", new object[] { 0xC000 });
                sResult = (string)proxyProperty.GetValue(proxyInstance, new object[] { 0 });
                Assert.That(sResult, Is.EqualTo("new-value-49152"));
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// a multi-argument indexer property is added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_Indexer_ManyArguments()
        {
            AssertAddPropertyBehavior(typeof(__PropertyTestType), "Item", new Type[] {typeof(int), typeof(int), typeof(int)},
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
        /// two overloaded indexers are added to the builder.
        /// </summary>
        [Test]
        public void AddProperty_IndexerOverload()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);

            // Verify that the following operations do not throw.
            builder.AddProperty(realSubjectType.GetProperty("Item", new Type[] { typeof(int) }));
            builder.AddProperty(realSubjectType.GetProperty("Item", new Type[] { typeof(int), typeof(int), typeof(int) }));
            builder.CreateInterface();
            builder.CreateProxy();
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when
        /// the given property (get/set methods) matches the signature
        /// of existing methods in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddProperty_DuplicatesExistingMethod()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);

            builder.AddMethod(realSubjectType.GetMethod("get_Getter"));
            builder.AddProperty(realSubjectType.GetProperty("Getter"));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when
        /// the given method matches the signature of existing
        /// properties (get/set methods) in the builder.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void AddMethod_DuplicatesExistingProperty()
        {
            Type realSubjectType = typeof(__PropertyTestType);
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);

            builder.AddProperty(realSubjectType.GetProperty("Getter"));
            builder.AddMethod(realSubjectType.GetMethod("get_Getter"));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

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
        /// Validates the state of a method after construction by the builder.
        /// </summary>
        /// 
        /// <param name="expectedMethod">
        /// The method from the builder's real subject type that models the actual method.
        /// </param>
        /// 
        /// <param name="actualMethod">
        /// The method to validate.
        /// </param>
        private static void AssertMethodState(MethodInfo expectedMethod, MethodInfo actualMethod)
        {
            Assert.That(actualMethod, Is.Not.Null);
            Assert.That(!actualMethod.IsStatic);
            Assert.That(JTCG.Convert.ToParameterTypes(actualMethod.GetParameters()),
                Is.EqualTo(JTCG.Convert.ToParameterTypes(expectedMethod.GetParameters())));
            Assert.That(actualMethod.ReturnType, Is.EqualTo(expectedMethod.ReturnType));
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
            Assert.That(actualProperty.PropertyType, Is.EqualTo(expectedProperty.PropertyType));

            Assert.That(actualProperty.CanRead, Is.EqualTo(expectedProperty.CanRead));
            if (actualProperty.CanRead)
            {
                Assert.That(!actualProperty.GetGetMethod(true).IsStatic);
                Assert.That(JTCG.Convert.ToParameterTypes(actualProperty.GetGetMethod(true).GetParameters()),
                    Is.EqualTo(JTCG.Convert.ToParameterTypes(expectedProperty.GetGetMethod().GetParameters())));
            }

            Assert.That(actualProperty.CanWrite, Is.EqualTo(expectedProperty.CanWrite));
            if (actualProperty.CanWrite)
            {
                Assert.That(!actualProperty.GetSetMethod(true).IsStatic);
                Assert.That(JTCG.Convert.ToParameterTypes(actualProperty.GetSetMethod(true).GetParameters()),
                    Is.EqualTo(JTCG.Convert.ToParameterTypes(expectedProperty.GetSetMethod().GetParameters())));
            }
        }

        /// <summary>
        /// Asserts the expected behavior of the builder's AddMethod() method.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The builder's real subject type.
        /// </param>
        /// 
        /// <param name="methodName">
        /// The name of the method to add to the builder.
        /// </param>
        /// 
        /// <param name="assertMethodInvocation">
        /// Delegate that asserts the expected behavior of the generated proxy method.
        /// </param>
        private static void AssertAddMethodBehavior(Type realSubjectType, string sMethodName, Type[] methodParamterTypes,
            AssertDynamicMethodInvocationDelegate assertMethodInvocation)
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);

            MethodInfo expectedMethod = realSubjectType.GetMethod(sMethodName, methodParamterTypes);
            builder.AddMethod(expectedMethod);

            // Verify the method on the proxy inteface.
            Type proxyInterface = builder.CreateInterface();
            MethodInfo interfaceMethod = proxyInterface.GetMethod(sMethodName, methodParamterTypes);
            AssertMethodState(expectedMethod, interfaceMethod);

            // Verify the method on the proxy.
            Type proxy = builder.CreateProxy();
            string sProxyMethodName = String.Concat(proxyInterface.Name, '.', sMethodName);
            MethodInfo proxyMethod = proxy.GetMethod(sProxyMethodName, BindingFlags.Instance | BindingFlags.NonPublic, null,
                methodParamterTypes, null);
            AssertMethodState(expectedMethod, proxyMethod);

            // Verify the behavior of the generated proxy.
            assertMethodInvocation(proxy, sProxyMethodName);
        }

        /// <summary>
        /// Asserts the expected behavior of the builder's AddProperty() method.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The builder's real subject type.
        /// </param>
        /// 
        /// <param name="sPropertyName">
        /// The name of the property to add to the builder.
        /// </param>
        /// 
        /// <param name="propertyParamerterTypes">
        /// The property's parameter types.
        /// </param>
        /// 
        /// <param name="assertPropertyInvocation">
        /// Delegate that asserts the expected behavior of the generated proxy property.
        /// </param>
        private static void AssertAddPropertyBehavior(Type realSubjectType, string sPropertyName, Type[] propertyParamerterTypes,
            AssertDynamicPropertyInvocationDelegate assertPropertyInvocation)
        {
            ProxyTypeBuilder builder = new ProxyTypeBuilder("root", realSubjectType);
            PropertyInfo expectedProperty = realSubjectType.GetProperty(sPropertyName, propertyParamerterTypes);
            builder.AddProperty(expectedProperty);

            // Verify the property on the proxy inteface.
            Type proxyInterface = builder.CreateInterface();
            AssertPropertyState(expectedProperty, proxyInterface.GetProperty(sPropertyName));

            // Verify the property on the proxy.
            Type proxy = builder.CreateProxy();
            PropertyInfo proxyProperty = proxy.GetProperty(String.Concat(proxyInterface.Name, '.', sPropertyName),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, expectedProperty.PropertyType, propertyParamerterTypes, null);

            AssertPropertyState(expectedProperty, proxyProperty);
            
            // Verify the behavior of the generated proxy.
            assertPropertyInvocation(proxy, proxyProperty);
        }

        /// <summary>
        /// Asserts the expected behavior of the builder's AddProperty() method,
        /// for non-indexed properties (no parameters).
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The builder's real subject type.
        /// </param>
        /// 
        /// <param name="sPropertyName">
        /// The name of the property to add to the builder.
        /// </param>
        /// 
        /// <param name="assertPropertyInvocation">
        /// Delegate that asserts the expected behavior of the generated proxy property.
        /// </param>
        private static void AssertAddPropertyBehavior(Type realSubjectType, string sPropertyName, AssertDynamicPropertyInvocationDelegate assertPropertyInvocation)
        {
            AssertAddPropertyBehavior(realSubjectType, sPropertyName, Type.EmptyTypes, assertPropertyInvocation);
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly BindingFlags ProxyMethodInvocationFlags =
            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic;

        #endregion

        #region nested types supporting unit tests ------------------------------------------------

        private delegate void AssertDynamicMethodInvocationDelegate(Type proxy, string sProxyMethodName);
        private delegate void AssertDynamicPropertyInvocationDelegate(Type proxy, PropertyInfo proxyProperty);

        private static class __AbstractSealedType { }

        private abstract class __AbstractType
        {
            public int InstanceProperty { get { return 123; } }
        }
        
        public class __MethodTestType
        {
            public DateTime InstanceMethod() { return DateTime.Today; }     // Also covers return-value, and no-args method test cases.
            public DateTime InstanceMethod(int i) { return DateTime.Today.AddMonths(i); }
            public static TimeSpan StaticMethod() { return TimeSpan.FromHours(12.0);  }
            public void VoidReturnValueMethod() { throw new ApplicationException("void-return"); }
            public string ManyArgumentsMethod(string s, int x, double y, DateTime d) { return String.Concat("many-args:", s, x, y, d); }
            public string ParamsArrayArgumentsMethod(string s, params object[] i) { return "params-args:" + String.Concat(i); }
            public void OutParameterMethod(out string s) { s = "out-param"; }

            private void PrivateMethod() { }
        }

        public class __PropertyTestType
        {
            public uint InstanceProperty
            {
                get { return m_instancePropertyValue; }
                set { m_instancePropertyValue = value; }
            }

            public static DateTime StaticProperty
            {
                get { return StaticPropertyValue; }
                set { StaticPropertyValue = value; }
            }

            public string Getter
            {
                get { return "getter-only"; }
            }

            public uint Setter
            {
                set { m_instancePropertyValue = value; }
            }

            public int this[int x, int y, int z]
            {
                get { return m_indexerValue; }
                set { m_indexerValue = value + x + y + z; }
            }

            public string this[int x]
            {
                get { return m_simpleIndexerValue; }
                set { m_simpleIndexerValue = String.Concat(value, x); }
            }

            private int PrivateProperty
            {
                get { return 0; }
            }


            private uint m_instancePropertyValue = 0xdeadbeef;
            private static DateTime StaticPropertyValue = DateTime.Today;
            private int m_indexerValue = 0xbeef;
            private string m_simpleIndexerValue = "simple-value";
        }

        public class __ConstructorTestType
        {
            public __ConstructorTestType() { throw new ApplicationException("0"); }
            public __ConstructorTestType(int x) { throw new ApplicationException(x.ToString()); }
            public __ConstructorTestType(int x, int y) { throw new ApplicationException(x.ToString() + y.ToString()); }
        }

        public class __BaseSubjectType
        {
            public virtual string VirtualMethod() { return m_virtualPropertyValue; }
            public string NonVirtualMethod() { return m_nonVirtualPropertyValue; }

            public virtual string VirtualProperty
            {
                get { return m_virtualPropertyValue; }
                set { m_virtualPropertyValue = value; }
            }

            public string NonVirtualProperty
            {
                get { return m_nonVirtualPropertyValue; }
                set { m_nonVirtualPropertyValue = value; }
            }


            private string m_virtualPropertyValue = "Base:Virtual";
            private string m_nonVirtualPropertyValue = "Base:NonVirtual";
        }

        public class __DerivedSubjectType : __BaseSubjectType
        {
            public override string VirtualMethod() { return m_virtualPropertyValue; }

            public override string VirtualProperty
            {
                get { return m_virtualPropertyValue; }
                set { m_virtualPropertyValue = "fixed-value"; }
            }

            private string m_virtualPropertyValue = "Derived:Override";
        }

        private interface __InterfaceType { }

        #endregion
    }
}