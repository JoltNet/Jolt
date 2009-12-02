// ----------------------------------------------------------------------------
// ProxyTypeBuilder.cs
//
// Contains the definition of the ProxyTypeBuilder class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/31/2007 14:09:25
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;

using Jolt.Functional;
using Jolt.Testing.Properties;
using log4net;

namespace Jolt.Testing.CodeGeneration
{
    // Represents a factory method for creating an XML doc comment builder.
    using CreateXDCBuilderDelegate = Func<Type, Type, Type, XmlDocCommentBuilderBase>;


    /// <summary>
    /// Provides methods to dynamically reverse engineer a proxy and accompanying
    /// interface for a given type.  The proxy and interface are created in a given
    /// namespace, which is appended to the namespace of the real subject type.
    /// </summary>
    /// 
    /// <example>
    /// If the namespace "Jolt.Testing.Generated" is given to the builder, along with
    /// the real subject type System.String, the generated proxy and interface are created
    /// in the "Jolt.Testing.Generated.System" namespace.
    /// </example>
    /// 
    /// <remarks>
    /// For a given type named SubjectType:
    /// The generated inteface is named ISubjectType.
    /// The generated proxy is named SubjectTypeProxy.
    /// </remarks>
    public class ProxyTypeBuilder : IProxyTypeBuilder 
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyTypeBuilder"/> class,
        /// with a transient assembly in the current appdomain.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> of object for which the proxy forwards to.
        /// </param>
        /// 
        /// <remarks>
        /// Does not produce XML doc comments for generated types.
        /// </remarks>
        public ProxyTypeBuilder(string rootNamespace, Type realSubjectType)
            : this(rootNamespace, realSubjectType, false) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyTypeBuilder"/> class,
        /// with a transient assembly in the current appdomain, optionally producing
        /// XML doc comments.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> of object for which the proxy forwards to.
        /// </param>
        /// 
        /// <param name="produceXmlDocComments">
        /// Determines if the <see cref="ProxyTypeBuilder"/> should attempt to produce
        /// XML doc comments for each generated type.
        /// </param>
        public ProxyTypeBuilder(string rootNamespace, Type realSubjectType, bool produceXmlDocComments)
            : this(rootNamespace, realSubjectType, produceXmlDocComments, AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("__transientAssembly"),
                    AssemblyBuilderAccess.RunAndSave).DefineDynamicModule("__transientModule")) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyTypeBuilder"/> class,
        /// with a user-specified <see cref="System.Reflection.Emit.ModuleBuilder"/>.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/ >of object for which the proxy forwards to.
        /// </param>
        /// 
        /// <param name="produceXmlDocComments">
        /// Determines if the <see cref="ProxyTypeBuilder"/> should attempt to produce
        /// XML doc comments for each generated type.
        /// </param>
        /// 
        /// <param name="targetModule">
        /// The <see cref="System.Reflection.Emit.ModuleBuilder"/> in which the types are created.
        /// </param>
        public ProxyTypeBuilder(string rootNamespace, Type realSubjectType, bool produceXmlDocComments, ModuleBuilder targetModule)
            : this(rootNamespace,
                   realSubjectType,
                   produceXmlDocComments ? CreateXmlDocCommentBuilder : (CreateXDCBuilderDelegate)delegate { return new XmlDocCommentBuilderBase(); },
                   targetModule) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxyTypeBuilder"/> class,
        /// with a user-specified <see cref="XmlDocCommentBuilderBase"/> factory
        /// method and <see cref="System.Reflection.Emit.ModuleBuilder"/>.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> of object for which the proxy forwards to.
        /// </param>
        /// 
        /// <param name="createXDCBuilder">
        /// A delegate containing a factory method for creating the proxy
        /// builder's <see cref="XmlDocCommentBuilderBase"/>.
        /// </param>
        /// 
        /// <param name="targetModule">
        /// The <see cref="System.Reflection.Emit.ModuleBuilder"/> in which the types are created.
        /// </param>
        /// 
        /// <remarks>
        /// Used internally by test code to override
        /// <see cref="System.Reflection.Emit.ModuleBuilder"/> and <see cref="XmlDocCommentBuilderBase"/>
        /// operations.
        /// </remarks>
        internal ProxyTypeBuilder(string rootNamespace, Type realSubjectType, CreateXDCBuilderDelegate createXDCBuilder, ModuleBuilder targetModule)
        {
            ValidateRealSubjectType(realSubjectType);

            m_realSubjectType = realSubjectType;
            m_module = targetModule;
            m_addedMembers = new HashSet<MemberInfo>();

            // Create the type holders for the generated interface and proxy.
            m_proxy = m_module.DefineType(CreateProxyName(rootNamespace, m_realSubjectType), TypeAttributes.Public | TypeAttributes.Sealed);
            m_proxyInterface = m_module.DefineType(CreateInterfaceName(rootNamespace, m_realSubjectType),
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            m_methodDeclarerFactory = new MethodDeclarerFactory(m_proxyInterface, m_proxy);
            m_xmlDocCommentBuilder = createXDCBuilder(realSubjectType, m_proxy, m_proxyInterface);

            Type realSubjectFieldType = realSubjectType;
            if (realSubjectType.ContainsGenericParameters)
            {
                Type[] realSubjectGenericParameters = realSubjectType.GetGenericArguments();
                Type[] proxyGenericParameters = InitializeGenericTypeArguments(realSubjectGenericParameters, m_proxy);
                InitializeGenericTypeArguments(realSubjectGenericParameters, m_proxyInterface);

                // A generic real subject type requires specialization.
                realSubjectFieldType = realSubjectType.MakeGenericType(proxyGenericParameters);
            }

            if (!m_realSubjectType.IsAbstract)
            {
                m_realSubjectField = m_proxy.DefineField("m_realSubject", realSubjectFieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
                InitializeProxyConstructors();
            }
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the real subject <see cref="System.Type"/> referenced by the <see cref="ProxyTypeBuilder"/>.
        /// </summary>
        public Type ProxiedType
        {
            get { return m_realSubjectType; }
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.Emit.ModuleBuilder"/> that owns the generated types.
        /// </summary>
        public ModuleBuilder Module
        {
            get { return m_module; }
        }

        /// <summary>
        /// Determines if the <see cref="ProxyTypeBuilder"/> produces XML doc comments.
        /// </summary>
        public bool ProducesXmlDocComments
        {
            get { return m_xmlDocCommentBuilder.GetType() !=  typeof(XmlDocCommentBuilderBase); }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a <see cref="System.Reflection.MethodInfo"/> to the proxy builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <remarks>
        /// Equivalent to calling <see cref="AddMethod(MethodInfo, Type)"/> with the
        /// implict returnm type of <paramref name="method"/>.
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="method"/> is not public.
        /// <paramref name="method"/> is not a member of the builder's real subject type.
        /// <paramref name="method"/> is not static but the builder's real subject type is abstract.
        /// <paramref name="desiredReturnType"/> is not a base type of the return type of <paramref name="method"/>.
        /// </exception>
        /// 
        /// <seealso cref="AddMethod(MethodInfo, Type)"/>
        public virtual void AddMethod(MethodInfo method)
        {
            ThrowOnNullMember(method);
            AddMethod_NonNull(method, method.ReturnType);
        }

        /// <summary>
        /// Adds a <see cref="System.Reflection.MethodInfo"/> to the proxy builder,
        /// overriding the given method's return <see cref="System.Type"/>.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The <see cref="System.Type"/> of the return value on the builder's generated method.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="method"/> is not public.
        /// <paramref name="method"/> is not a member of the builder's real subject type.
        /// <paramref name="method"/> is not static but the builder's real subject type is abstract.
        /// <paramref name="desiredReturnType"/> is not a base type of the return type of <paramref name="method"/>.
        /// </exception>
        public virtual void AddMethod(MethodInfo method, Type desiredReturnType)
        {
            ThrowOnNullMember(method);
            AddMethod_NonNull(method, desiredReturnType);
        }

        /// <summary>
        /// Adds a <see cref="System.Reflection.PropertyInfo"/> to the proxy builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <remarks>
        /// Equivalent to calling <see cref="AddProperty(PropertyInfo, Type)"/> with the
        /// implict property type of <paramref name="property"/>.
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="property"/> is null.
        /// </exception>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="property"/> does not define a get or set method.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="property"/> defines a set method whose return type does not
        /// match <paramref name="desiredReturnType"/>.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="property"/> is not a member of the builder's real subject type.
        /// <paramref name="property"/> is not static but the builder's real subject type is abstract.
        /// <paramref name="desiredReturnType"/> is not a base type of the property type of <paramref name="property"/>.
        /// </exception>
        ///
        /// <seealso cref="AddProperty(PropertyInfo, Type)"/>
        public virtual void AddProperty(PropertyInfo property)
        {
            ThrowOnNullMember(property);
            AddProperty_NonNull(property, property.PropertyType);
        }

        /// <summary>
        /// Adds a <see cref="System.Reflection.PropertyInfo"/> to the proxy builder,
        /// overriding its return <see cref="System.Type"/> on the get method.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The <see cref="System.Type"/> of the return value on the builder's generated get method.
        /// </param>
        /// 
        /// <remarks>
        /// It is an error to provide a return type override for a property with a set method.
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="property"/> is null.
        /// </exception>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="property"/> does not define a get or set method.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="property"/> defines a set method whose return type does not
        /// match <paramref name="desiredReturnType"/>.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="property"/> is not a member of the builder's real subject type.
        /// <paramref name="property"/> is not static but the builder's real subject type is abstract.
        /// <paramref name="desiredReturnType"/> is not a base type of the property type of <paramref name="property"/>.
        /// </exception>
        public virtual void AddProperty(PropertyInfo property, Type desiredReturnType)
        {
            ThrowOnNullMember(property);
            AddProperty_NonNull(property, desiredReturnType);
        }

        /// <summary>
        /// Adds an <see cref="System.Reflection.EventInfo"/> to the proxy builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The <see cref="System.Reflection.EventInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="eventInfo"/> is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="eventInfo"/> is not public.
        /// <paramref name="eventInfo"/> is not a member of the builder's real subject type.
        /// <paramref name="eventInfo"/> is not static but the builder's real subject type is abstract.
        /// </exception>
        public virtual void AddEvent(EventInfo eventInfo)
        {
            ThrowOnNullMember(eventInfo);
            ValidateEvent(eventInfo);

            // Add the event to the inteface.
            EventBuilder interfaceEventBuilder = m_proxyInterface.DefineEvent(eventInfo.Name,
                eventInfo.Attributes, eventInfo.EventHandlerType);

            // Add the event to the proxy.
            EventBuilder proxyEventBuilder = m_proxy.DefineEvent(eventInfo.Name,
                eventInfo.Attributes, eventInfo.EventHandlerType);
            
            // Define the event methods (add/remove) for the interface and proxy.
            // NOTE: The raise method on the proxy event is not set since the proxy does
            // not contain any code to raise the event.  Once the subject's event is raised,
            // any method subscribed to the proxy event will be notified.
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder;

            MethodInfo addMethod = eventInfo.GetAddMethod();
            MethodInfo removeMethod = eventInfo.GetRemoveMethod();

            DefineInterfaceAndProxyMethod(addMethod, addMethod.ReturnType, out interfaceMethodBuilder, out proxyMethodBuilder);
            interfaceEventBuilder.SetAddOnMethod(interfaceMethodBuilder);
            proxyEventBuilder.SetAddOnMethod(proxyMethodBuilder);

            DefineInterfaceAndProxyMethod(removeMethod, removeMethod.ReturnType, out interfaceMethodBuilder, out proxyMethodBuilder);
            interfaceEventBuilder.SetRemoveOnMethod(interfaceMethodBuilder);
            proxyEventBuilder.SetRemoveOnMethod(proxyMethodBuilder);

            m_xmlDocCommentBuilder.AddEvent(eventInfo);
        }

        /// <summary>
        /// Creates the proxy interface <see cref="System.Type"/> object.
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the proxy interface <see cref="System.Type"/>, as per the
        /// current state of the <see cref="ProxyTypeBuilder"/>.
        /// </returns>
        public virtual Type CreateInterface()
        {
            return m_proxyInterface.CreateType();
        }

        /// <summary>
        /// Creates the proxy interface and proxy <see cref="System.Type"/> objects.
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the proxy <see cref="System.Type"/>, as per the
        /// current state of the <see cref="ProxyTypeBuilder"/>.
        /// </returns>
        /// 
        /// <remarks>
        /// The proxy interface is is also created as it is required to complete the
        /// declaration of the proxy type.
        /// </remarks>
        public virtual Type CreateProxy()
        {
            m_proxy.AddInterfaceImplementation(CreateInterface());
            return m_proxy.CreateType();
        }

        /// <summary>
        /// Creates a new <see cref="System.Xml.XmlReader"/> capable of reading any produced XML doc comments.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="System.Xml.XmlReader"/> for reading the builder's XML doc comments.
        /// </returns>
        public virtual XmlReader CreateXmlDocCommentReader()
        {
            return m_xmlDocCommentBuilder.CreateReader();
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="XmlDocCommentBuilderBase"/> used by this instance.
        /// </summary>
        internal XmlDocCommentBuilderBase XmlDocCommentBuilder
        {
            get { return m_xmlDocCommentBuilder; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Initializes the required constructors on the proxy type.
        /// </summary>
        /// 
        /// <remarks>
        /// Copies all public constructors from the real subject type to the proxy type.
        /// Initializes each proxy constructor to forward to the real subject's respective constructor.
        /// </remarks>
        private void InitializeProxyConstructors()
        {
            // Initialize the base constructor emit call only when the
            // real subject type is a reference type.
            Action<ILGenerator> emitBaseClassConstructorCall = m_realSubjectType.IsClass ?
                EmitObjectDefaultConstructorCall : Functor.NoOperation<ILGenerator>();

            // Create a constructor on the proxy for each public constructor
            // on the real subject type and emit XML doc comments.
            foreach (ConstructorInfo constructor in m_realSubjectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                ILGenerator codeGenerator = m_methodDeclarerFactory.Create(constructor).Declare().GetILGenerator();
 
                // Call the base constructor, if it exists.
                emitBaseClassConstructorCall(codeGenerator);

                // Load all arguments on to stack, including this pointer, to
                // prepare to forward to real subject's constructor.
                for (sbyte i = 0; i <= constructor.GetParameters().Length; ++i)
                {
                    codeGenerator.Emit(OpCodes.Ldarg_S, i);
                }
                
                // Instantiate the real subject type and assign to the field in the proxy.
                codeGenerator.Emit(OpCodes.Newobj, constructor);
                codeGenerator.Emit(OpCodes.Stfld, m_realSubjectField);

                // Return from the constructor call.
                codeGenerator.Emit(OpCodes.Ret);

                m_xmlDocCommentBuilder.AddConstuctor(constructor);
            }
        }

        /// <summary>
        /// Defines and implements a method on the proxy interface and proxy types,
        /// modelling a given <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> from which the builder
        /// will base its definitions.
        /// </param>
        /// 
        /// <param name="methodReturnType">
        /// The return <see cref="System.Type"/> for the generated methods, overriding
        /// the return type of the given method.
        /// </param>
        ///
        /// <remarks>
        /// Assumes that the method has been validated prior to the call.
        /// </remarks>
        /// 
        /// <seealso cref="ValidateMethod"/>
        /// <seealso cref="ThrowOnNullMember"/>
        private void DefineInterfaceAndProxyMethod(MethodInfo method, Type methodReturnType)
        {
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder;
            DefineInterfaceAndProxyMethod(method, methodReturnType, out interfaceMethodBuilder, out proxyMethodBuilder);
        }

        /// <summary>
        /// Defines and implements a method on the proxy interface and proxy types,
        /// returning the respective <see cref="System.Reflection.Emit.MethodBuilder"/> objects,
        /// modelling a given <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> from which the builder
        /// will base its definitions.
        /// </param>
        /// 
        /// <param name="methodReturnType">
        /// The return <see cref="System.Type"/> for the generated methods, overriding
        /// the return type of the given method.
        /// </param>
        /// 
        /// <param name="interfaceMethodBuilder">
        /// The <see cref="System.Reflection.Emit.MethodBuilder"/> for the interface method,
        /// constructred by this method.
        /// </param>
        /// 
        /// <param name="proxyMethodBuilder">
        /// The <see cref="System.Reflection.Emit.MethodBuilder"/> for the proxy method,
        /// constructed by this method.
        /// </param>
        /// 
        /// <remarks>
        /// Assumes that the method has been validated prior to the call.
        /// </remarks>
        /// 
        /// <seealso cref="ValidateMethod"/>
        /// <seealso cref="ThrowOnNullMember"/>
        private void DefineInterfaceAndProxyMethod(MethodInfo method, Type methodReturnType, out MethodBuilder interfaceMethodBuilder, out MethodBuilder proxyMethodBuilder)
        {
            // Declare the interface and proxy methods.
            interfaceMethodBuilder = m_methodDeclarerFactory.Create(MethodDeclarerTypes.Interface, method).Declare(methodReturnType);
            proxyMethodBuilder = m_methodDeclarerFactory.Create(MethodDeclarerTypes.Proxy, method).Declare(methodReturnType);

            // Generate the IL that represents the proxy's method call.
            ILGenerator codeGenerator = proxyMethodBuilder.GetILGenerator();
            OpCode methodCallOpCode = OpCodes.Call;
            
            if (!method.IsStatic)
            {
                // Load the instance field on the stack as this is the target
                // of the method call.
                codeGenerator.Emit(OpCodes.Ldarg_0);
                codeGenerator.Emit(OpCodes.Ldfld, m_realSubjectField);
                methodCallOpCode = OpCodes.Callvirt;
            }

            // Load each funciton argument onto the stack.
            for (sbyte i = 1; i <= method.GetParameters().Length; ++i)
            {
                codeGenerator.Emit(OpCodes.Ldarg_S, i);
            }

            // Invoke the function and store the return value on the stack.
            codeGenerator.EmitCall(methodCallOpCode, method, null);

            // Return from the proxy method call.
            codeGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Verifies that the given <see cref="System.Reflection.MethodInfo"/> is legal
        /// as input into the builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to validate.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The desired <see cref="System.Type"/> of the return value on the builder's generated method.
        /// </param>
        /// 
        /// <remarks>
        /// Invokes <see cref="ValidateAccessibleMethod"/> after confirming that <paramref name="method"/>
        /// is publicly accessible.
        /// </remarks>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="method"/> is not public.
        /// </exception>
        /// 
        /// <seealso cref="ValidateAccessibleMethod"/>
        private void ValidateMethod(MethodInfo method, Type desiredReturnType)
        {
            if (!method.IsPublic)
            {
                throw new InvalidOperationException(Resources.Error_MemberNotPublic);
            }

            ValidateAccessibleMethod(method, desiredReturnType);
        }

        /// <summary>
        /// Verifies that the given public <see cref="System.Reflection.MethodInfo"/>
        /// is legal as input into the builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to validate.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The desired <see cref="System.Type"/> of the return value on the builder's generated method.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// A method with a signature matching <paramref name="method"/> has already been
        /// added to the builder.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="method"/> is not a member of the builder's real subject type.
        /// <paramref name="method"/> is not static but the builder's real subject type is abstract.
        /// <paramref name="desiredReturnType"/> is not a base type of the return type of <paramref name="method"/>.
        /// </exception>
        private void ValidateAccessibleMethod(MethodInfo method, Type desiredReturnType)
        {
            if (m_addedMembers.Contains(method))
            {
                throw new ArgumentException(Resources.Error_DuplicateMember);
            }

            if (method.DeclaringType != m_realSubjectType && !m_realSubjectType.IsSubclassOf(method.DeclaringType))
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_MethodNotMemberOfRealSubject, method.Name));
            }

            if (m_realSubjectType.IsAbstract && !method.IsStatic)
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_InstanceMethodAddedFromAbstractType, method.Name));
            }

            if (!desiredReturnType.IsAssignableFrom(method.ReturnType))
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_InvalidReturnTypeOverride, method.Name, desiredReturnType.Name, method.ReturnType.Name));
            }

            m_addedMembers.Add(method);
        }

        /// <summary>
        /// Verifies that the given <see cref="System.Reflection.PropertyInfo"/> is legal as
        /// input into the builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> to validate.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The desired <see cref="System.Type"/> of the return value on the builder's generated get method.
        /// </param>
        /// 
        /// <remarks>
        /// Validates the get and set methods of <paramref name="property"/>
        /// as per the rules in <see cref="ValidateAccessibleMethod"/>.
        /// </remarks>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="property"/> does not define a get or set method.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="property"/> defines a set method whose return type does not
        /// match <paramref name="desiredReturnType"/>.
        /// </exception>
        private void ValidateProperty(PropertyInfo property, Type desiredReturnType)
        {
            MethodInfo getMethod = property.GetGetMethod();
            MethodInfo setMethod = property.GetSetMethod();

            if (getMethod == null && setMethod == null)
            {
                throw new NotSupportedException(String.Format(Resources.Error_InvalidProperty, property.Name));
            }

            if (setMethod != null && property.PropertyType != desiredReturnType)
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_InvalidProperty_ReturnTypeOverride, property.Name));
            }

            if (getMethod != null) { ValidateAccessibleMethod(getMethod, desiredReturnType); }
            if (setMethod != null) { ValidateAccessibleMethod(setMethod, setMethod.ReturnType); }
        }

        /// <summary>
        /// Verifies that the given <see cref="System.Reflection.EventInfo"/> is legal as
        /// input into the builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The <see cref="System.Reflection.EventInfo"/> to validate.
        /// </param>
        /// 
        /// <remarks>
        /// Validates the add and remove methods of <paramref name="eventInfo"/>
        /// as per the rules in <see cref="ValidateMethod"/>.
        /// </remarks>
        private void ValidateEvent(EventInfo eventInfo)
        {
            MethodInfo addMethod = eventInfo.GetAddMethod(true);
            MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);

            ValidateMethod(addMethod, addMethod.ReturnType);
            ValidateMethod(removeMethod, removeMethod.ReturnType);
        }

        /// <summary>
        /// Adds a non-null <see cref="System.Reflection.MethodInfo"/> to the proxy builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The <see cref="System.Type"/> of the return value for the builder's generated method.
        /// </param>
        /// 
        /// <remarks>
        /// Validates <paramref name="method"/> as per the rules in <see cref="ValidateMethod"/>.
        /// </remarks>
        /// 
        /// <seealso cref="ValidateMethod"/>
        private void AddMethod_NonNull(MethodInfo method, Type desiredReturnType)
        {
            ValidateMethod(method, desiredReturnType);
            DefineInterfaceAndProxyMethod(method, desiredReturnType);

            m_xmlDocCommentBuilder.AddMethod(method);
        }

        /// <summary>
        /// Adds a non-null <see cref="System.Reflection.PropertyInfo"/> to the proxy builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> to add to the builder.
        /// </param>
        /// 
        /// <param name="desiredReturnType">
        /// The <see cref="System.Type"/> of the return value for the builder's generated get method.
        /// </param>
        /// 
        /// <remarks>
        /// Validates <paramref name="property"/> as per the rules in <see cref="ValidateProperty"/>.
        /// It is an error to provide a return type override for a property with a set method.
        /// </remarks>
        /// 
        /// <seealso cref="ValidateProperty"/>
        private void AddProperty_NonNull(PropertyInfo property, Type desiredReturnType)
        {
            ValidateProperty(property, desiredReturnType);

            // Add the property to the interface.
            Type[] indexerParameterTypes = Convert.ToParameterTypes(property.GetIndexParameters());
            PropertyBuilder interfacePropertyBuilder = m_proxyInterface.DefineProperty(property.Name,
                property.Attributes, desiredReturnType, indexerParameterTypes);

            // Add the property to the proxy.
            PropertyBuilder proxyPropertyBuilder = m_proxy.DefineProperty(property.Name,
                property.Attributes, desiredReturnType, indexerParameterTypes);

            // Define the property methods (get/set) for the interface and proxy,
            // when applicable.
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder;

            MethodInfo getMethod = property.GetGetMethod();
            MethodInfo setMethod = property.GetSetMethod();

            if (getMethod != null)
            {
                DefineInterfaceAndProxyMethod(getMethod, desiredReturnType, out interfaceMethodBuilder, out proxyMethodBuilder);
                interfacePropertyBuilder.SetGetMethod(interfaceMethodBuilder);
                proxyPropertyBuilder.SetGetMethod(proxyMethodBuilder);
            }

            if (setMethod != null)
            {
                DefineInterfaceAndProxyMethod(setMethod, setMethod.ReturnType, out interfaceMethodBuilder, out proxyMethodBuilder);
                interfacePropertyBuilder.SetSetMethod(interfaceMethodBuilder);
                proxyPropertyBuilder.SetSetMethod(proxyMethodBuilder);
            }

            m_xmlDocCommentBuilder.AddProperty(property);
        }


        /// <summary>
        /// Creates an <see cref="XmlDocCommentBuilderBase"/> for a real subject type.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The real subject type for which the <see cref="XmlDocCommentBuilderBase"/>
        /// is created.
        /// </param>
        /// 
        /// <param name="proxyType">
        /// The <see cref="System.Type"/> containing the generated proxy.
        /// </param>
        /// 
        /// <param name="proxyInterfaceType">
        /// The <see cref="System.Type"/> containing the generated proxy interface.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of an <see cref="XmlDocCommentBuilder"/>, when XML doc comments
        /// exist for the assembly of <paramref name="realSubjectType"/>, or a new instance
        /// of <see cref="XmlDocCommentBuilderBase"/> otherwise.
        /// </returns>
        private static XmlDocCommentBuilderBase CreateXmlDocCommentBuilder(Type realSubjectType, Type proxyType, Type proxyInterfaceType)
        {
            // Create an XMLDocCommentReader for the real subject type.
            // If the XML doc comments are not discovered for the given type,
            // disable production of XML doc comments for generated types.
            try
            {
                XmlDocCommentReader reader = new XmlDocCommentReader(realSubjectType.Assembly);
                return new XmlDocCommentBuilder(reader, realSubjectType, proxyType, proxyInterfaceType);
            }
            catch (FileNotFoundException)
            {
                Log.InfoFormat(Resources.Info_XmlDocCommentsDisabled, realSubjectType.Assembly.GetName().Name);
                return new XmlDocCommentBuilderBase();
            }
        }

        /// <summary>
        /// Initializes the generic type arguments of a given
        /// <see cref="System.Reflection.Emit.TypeBuilder"/>.
        /// </summary>
        /// 
        /// <param name="genericTypeArguments">
        /// The generic arguments to model.
        /// </param>
        /// 
        /// <param name="typeBuilder">
        /// The TypeBuilder (proxy or proxyInterface) to initialize.
        /// </param>
        /// 
        /// <returns>
        /// A reference to the newly initialized generic type arguments.
        /// </returns>
        /// 
        /// <remarks>
        /// Copies <paramref name="genericTypeArguments"/> (of the real subject type)
        /// to <paramref name="typeBuilder"/>.
        /// </remarks>
        private static GenericTypeParameterBuilder[] InitializeGenericTypeArguments(Type[] genericTypeArguments, TypeBuilder typeBuilder)
        {
            GenericTypeParameterBuilder[] genericParameterBuilders = typeBuilder.DefineGenericParameters(Convert.ToTypeNames(genericTypeArguments));
            DeclarationHelper.CopyTypeConstraints(genericTypeArguments, genericParameterBuilders);

            return genericParameterBuilders;
        }

        /// <summary>
        /// Throws an exception when the given parameter is null.
        /// </summary>
        /// 
        /// <param name="member">
        /// The <see cref="System.Reflection.MemberInfo"/> to validate.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is null.
        /// </exception>
        private static void ThrowOnNullMember(MemberInfo member)
        {
            if (member == null) { throw new ArgumentNullException(); }
        }

        /// <summary>
        /// Emits a call to the default <see cref="System.Object"/> constructor, using
        /// a given <see cref="System.Reflection.Emit.ILGenerator"/>.
        /// </summary>
        /// 
        /// <param name="codeGenerator">
        /// The <see cref="System.Reflection.Emit.ILGenerator"/> that emits the constructor call.
        /// </param>
        private static void EmitObjectDefaultConstructorCall(ILGenerator codeGenerator)
        {
            codeGenerator.Emit(OpCodes.Ldarg_0);
            codeGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies that a real subject type is valid input for a <see cref="ProxyTypeBuilder"/>.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> to validate.
        /// </param>
        /// 
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="realSubjectType"/> is an interface, delegate, or a non-abstract
        /// class with no public constructors.
        /// </exception>
        private static void ValidateRealSubjectType(Type realSubjectType)
        {
            // Interface and delegate types can not be abstracted
            // by a proxy and interface.
            if (realSubjectType.IsInterface || typeof(Delegate).IsAssignableFrom(realSubjectType))
            {
                throw new NotSupportedException(String.Format(Resources.Error_InvalidRealSubjectType, realSubjectType.Name));
            }
            
            // In order for the proxy to forward/emulate construction,
            // a non-abstract class must have at least one public constructor.
            if (!realSubjectType.IsAbstract && realSubjectType.GetConstructors().Length == 0)
            {
                throw new NotSupportedException(String.Format(Resources.Error_RealSubjectType_LackingConstructor, realSubjectType.Name));   
            }

            // TODO: Validate accessibility of realSubjectType, including its parent types (for nested types).
        }

        /// <summary>
        /// Creates the namespace-qualified name of the proxy type.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which the proxy type will exist.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The real subject type from which the name is derived.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="System.String"/> representing the requested type name.
        /// </returns>
        private static string CreateProxyName(string rootNamespace, Type realSubjectType)
        {
            return String.Concat(rootNamespace, '.', realSubjectType.Namespace, '.', NormalizeTypeName(realSubjectType.Name), "Proxy");
        }

        /// <summary>
        /// Creates the namespace-qualified name of the interface type.
        /// </summary>
        /// 
        /// <param name="rootNamespace">
        /// The namespace in which the interface type will exist.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The real subject type from which the name is derived.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="System.String"/> representing the requested type name.
        /// </returns>
        private static string CreateInterfaceName(string rootNamespace, Type realSubjectType)
        {
            return String.Concat(rootNamespace, '.', realSubjectType.Namespace, ".I", NormalizeTypeName(realSubjectType.Name));
        }

        /// <summary>
        /// Removes a substring starting with the ` character from a given string.
        /// </summary>
        /// 
        /// <param name="typeName">
        /// The string to normalize.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="System.String"/> equal to <paramref name="typeName"/> with the
        /// identified substring removed.
        /// </returns>
        private static string NormalizeTypeName(string typeName)
        {
            int charPos = typeName.IndexOf('`');
            if (charPos > 0)
            {
                typeName = typeName.Substring(0, charPos);
            }

            return typeName;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly Type m_realSubjectType;
        private readonly FieldInfo m_realSubjectField;
        private readonly ModuleBuilder m_module;
        private readonly TypeBuilder m_proxyInterface;
        private readonly TypeBuilder m_proxy;
        private readonly HashSet<MemberInfo> m_addedMembers;
        private readonly MethodDeclarerFactory m_methodDeclarerFactory;
        private readonly XmlDocCommentBuilderBase m_xmlDocCommentBuilder;

        private static readonly ILog Log = LogManager.GetLogger(MethodInfo.GetCurrentMethod().DeclaringType);

        #endregion
    }
}
