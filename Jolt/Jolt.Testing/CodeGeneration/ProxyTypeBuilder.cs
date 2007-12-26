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
using System.Reflection;
using System.Reflection.Emit;

using JTCG = Jolt.Testing.CodeGeneration;
using Jolt.Testing.Properties;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Defines a delegate that matches the method signatures of
    /// <seealso cref="MethodBuilder.DefineParameter"/> and
    /// <seealso cref="ConstructorBuilder.DefineParameter"/>.
    /// </summary>
    /// 
    /// <param name="nParamPosition">
    /// The one-based index of the parameter in the method signature.
    /// </param>
    /// 
    /// <param name="paramAttributes">
    /// The attributes of the parameter.
    /// </param>
    /// 
    /// <param name="sParamName">
    /// The parameter name.
    /// </param>
    delegate ParameterBuilder DefineParameterDelegate(int nParamPosition, ParameterAttributes paramAttributes, string sParamName);


    /// <summary>
    /// Provides methods to dynamically reverse engineer a proxy and accompanying
    /// interface for a given type.  The proxy and interface are created in a given
    /// namespace, which is appended by the namespace of the real subject type.
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
        /// Initializes the proxy builder.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The type of object for which the proxy forwards to.
        /// </param>
        /// 
        /// <param name="targetModule">
        /// The module in which the types are created.
        /// </param>
        public ProxyTypeBuilder(string sRootNamespace, Type realSubjectType, ModuleBuilder targetModule)
        {
            ValidateRealSubjectType(realSubjectType);

            m_realSubjectType = realSubjectType;
            m_module = targetModule;
            m_addedMembers = new Dictionary<MemberInfo, bool>();

            // Create the type holders for the generated interface and proxy.
            m_proxy = m_module.DefineType(String.Concat(sRootNamespace, '.', m_realSubjectType.FullName, "Proxy"),
                TypeAttributes.Public | TypeAttributes.Sealed);
            m_proxyInterface = m_module.DefineType(String.Concat(sRootNamespace, '.', m_realSubjectType.Namespace, ".I", m_realSubjectType.Name),
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            if (!m_realSubjectType.IsAbstract)
            {
                m_realSubjectField = m_proxy.DefineField("m_realSubject", m_realSubjectType, FieldAttributes.Private | FieldAttributes.InitOnly);
                InitializeProxyConstructors();
            }
        }

        /// <summary>
        /// Initializes the proxy builder with a transient assembly in the current
        /// appdomain.
        /// </summary>
        /// 
        /// <param name="sRootNamespace">
        /// The namespace in which all types are created.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The type of object for which the proxy forwards to.
        /// </param>
        public ProxyTypeBuilder(string sRootNamespace, Type realSubjectType)
            : this(sRootNamespace, realSubjectType, AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("__transientAssembly"),
                    AssemblyBuilderAccess.Run).DefineDynamicModule("__transientModule")) {}

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the real subject type referenced by the proxy.
        /// </summary>
        public Type ProxiedType
        {
            get { return m_realSubjectType; }
        }

        /// <summary>
        /// Gets the module that owns the generated types.
        /// </summary>
        public ModuleBuilder Module
        {
            get { return m_module; }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Adds a method to the proxy builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to add to the builder.
        /// </param>
        public virtual void AddMethod(MethodInfo method)
        {
            ThrowOnNullMember(method);
            ValidateMethod(method);
            
            // Define the methods for the interface and proxy.
            DefineInterfaceAndProxyMethod(method);
        }

        /// <summary>
        /// Adds a property to the proxy builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property to add to the builder.
        /// </param>
        public virtual void AddProperty(PropertyInfo property)
        {
            ThrowOnNullMember(property);
            ValidateProperty(property);

            // Add the property to the interface.
            PropertyBuilder interfacePropertyBuilder = m_proxyInterface.DefineProperty(property.Name,
                property.Attributes, property.PropertyType, JTCG.Convert.ToParameterTypes(property.GetIndexParameters()));

            // Add the property to the proxy.
            // The property explicitly implements the interface method.
            PropertyBuilder proxyPropertyBuilder = m_proxy.DefineProperty(String.Concat(m_proxyInterface.Name, '.', property.Name),
                property.Attributes, property.PropertyType, JTCG.Convert.ToParameterTypes(property.GetIndexParameters()));

            // Define the property methods (get/set) for the interface and proxy.
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder; 
            
            if (property.CanRead)
            {
                DefineInterfaceAndProxyMethod(property.GetGetMethod(), out interfaceMethodBuilder, out proxyMethodBuilder);
                interfacePropertyBuilder.SetGetMethod(interfaceMethodBuilder);
                proxyPropertyBuilder.SetGetMethod(proxyMethodBuilder);
            }

            if (property.CanWrite)
            {
                DefineInterfaceAndProxyMethod(property.GetSetMethod(), out interfaceMethodBuilder, out proxyMethodBuilder);
                interfacePropertyBuilder.SetSetMethod(interfaceMethodBuilder);
                proxyPropertyBuilder.SetSetMethod(proxyMethodBuilder);
            }
        }

        /// <summary>
        /// Adds an event to the proxy builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The event to add to the builder.
        /// </param>
        public virtual void AddEvent(EventInfo eventInfo)
        {
            ThrowOnNullMember(eventInfo);
            ValidateEvent(eventInfo);

            // Add the event to the inteface.
            EventBuilder interfaceEventBuilder = m_proxyInterface.DefineEvent(eventInfo.Name,
                eventInfo.Attributes, eventInfo.EventHandlerType);

            // Add the event to the proxy.
            EventBuilder proxyEventBuilder = m_proxy.DefineEvent(String.Concat(m_proxyInterface.Name, '.', eventInfo.Name),
                eventInfo.Attributes, eventInfo.EventHandlerType);
            
            // Define the event methods (add/remove) for the interface and proxy.
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder;

            DefineInterfaceAndProxyMethod(eventInfo.GetAddMethod(), out interfaceMethodBuilder, out proxyMethodBuilder);
            interfaceEventBuilder.SetAddOnMethod(interfaceMethodBuilder);
            proxyEventBuilder.SetAddOnMethod(proxyMethodBuilder);

            DefineInterfaceAndProxyMethod(eventInfo.GetRemoveMethod(), out interfaceMethodBuilder, out proxyMethodBuilder);
            interfaceEventBuilder.SetRemoveOnMethod(interfaceMethodBuilder);
            proxyEventBuilder.SetRemoveOnMethod(proxyMethodBuilder);

            // NOTE: The raise method on the proxy event is not set since the proxy does
            // not contain any code to raise the event.  Once the subject's event is raised,
            // any method subscribed to the proxy event will be notified.
        }

        /// <summary>
        /// Creates the proxy interface type for the current state of the builder.
        /// </summary>
        public virtual Type CreateInterface()
        {
            return m_proxyInterface.CreateType();
        }

        /// <summary>
        /// Creates the proxy interface type.
        /// </summary>
        public virtual Type CreateProxy()
        {
            m_proxy.AddInterfaceImplementation(CreateInterface());
            return m_proxy.CreateType();
        }

        #endregion

        #region private instance methods ----------------------------------------------------------

        /// <summary>
        /// Copies all public constructors from the real subject type to
        /// the proxy type.  Initializes each proxy constructor to forward to
        /// the real subject's respective constructor.
        /// </summary>
        private void InitializeProxyConstructors()
        {
            // Initialize the base constructor emit call only when the
            // real subject type is a reference type.
            Action<ILGenerator> emitBaseClassConstructorCall = EmitObjectDefaultConstructorCall;
            if (!m_realSubjectType.IsClass) { emitBaseClassConstructorCall = delegate { /* noop */ }; }

            // Create a constructor on the proxy for each public constructor
            // on the real subject type.
            foreach (ConstructorInfo constructor in m_realSubjectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] constructorParameters = constructor.GetParameters();
                ConstructorBuilder proxyConstructorBuilder = m_proxy.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard, JTCG.Convert.ToParameterTypes(constructorParameters));
                DefineParametersWith(proxyConstructorBuilder.DefineParameter, constructorParameters);

                ILGenerator codeGenerator = proxyConstructorBuilder.GetILGenerator();
 
                // Call the base constructor, if it exists.
                emitBaseClassConstructorCall(codeGenerator);

                // Load all arguments on to stack, including this pointer, to
                // prepare to forward to real subject's constructor.
                for (sbyte i = 0; i <= constructorParameters.Length; ++i)
                {
                    codeGenerator.Emit(OpCodes.Ldarg_S, i);
                }
                
                // Instantiate the real subject type and assign to the field in the proxy.
                codeGenerator.Emit(OpCodes.Newobj, constructor);
                codeGenerator.Emit(OpCodes.Stfld, m_realSubjectField);

                // Return from the constructor call.
                codeGenerator.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// Defines a method on the proxy interface <see cref="m_proxyInterface"/> and
        /// on the proxy <see cref="m_proxy"/>, along with an implementation, for the
        /// given method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method from which the build will base its definitions.
        /// </param>
        /// 
        /// <remarks>
        /// Assumes that the method has been validated prior to the call.
        /// <seealso cref="ValidateMethod(MethodInfo)"/>
        /// <seealso cref="ThrowOnNullMember(MemberInfo)"/>
        /// </remarks>
        private void DefineInterfaceAndProxyMethod(MethodInfo method)
        {
            MethodBuilder interfaceMethodBuilder, proxyMethodBuilder;
            DefineInterfaceAndProxyMethod(method, out interfaceMethodBuilder, out proxyMethodBuilder);
        }

        /// <summary>
        /// Defines a method on the proxy interface <see cref="m_proxyInterface"/> and
        /// on the proxy <see cref="m_proxy"/>, along with an implementation, for the
        /// given method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method from which the build will base its definitions.
        /// </param>
        /// 
        /// <param name="interfaceMethodBuilder">
        /// The method builder for the interface method, constructred by this method.
        /// </param>
        /// 
        /// <param name="proxyMethodBuilder">
        /// The method builder for the proxy method, constructed by this method.
        /// </param>
        /// 
        /// <remarks>
        /// Assumes that the method has been validated prior to the call.
        /// <seealso cref="ValidateMethod(MethodInfo)"/>
        /// <seealso cref="ThrowOnNullMember(MemberInfo)"/>
        /// </remarks>
        private void DefineInterfaceAndProxyMethod(MethodInfo method, out MethodBuilder interfaceMethodBuilder, out MethodBuilder proxyMethodBuilder)
        {
            ParameterInfo[] methodParameters = method.GetParameters();
            Type[] methodParameterTypes = JTCG.Convert.ToParameterTypes(methodParameters);

            // Add the method to the interface.
            // All interface methods are public, abstract and virtual.
            interfaceMethodBuilder = m_proxyInterface.DefineMethod(method.Name,
                MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                method.ReturnType, methodParameterTypes);
            DefineParametersWith(interfaceMethodBuilder.DefineParameter, methodParameters);
            
            // Add the method to the proxy.
            // The method explicity implements the interface method.
            proxyMethodBuilder = m_proxy.DefineMethod(String.Concat(m_proxyInterface.Name, '.', method.Name),
                MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Final,
                method.ReturnType, methodParameterTypes);
            DefineParametersWith(proxyMethodBuilder.DefineParameter, methodParameters);

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

            // Load each argument to the function call onto the stack.
            for (sbyte i = 1; i <= methodParameterTypes.Length; ++i)
            {
                codeGenerator.Emit(OpCodes.Ldarg_S, i);
            }

            // Invoke the function and store the return value on the stack.
            codeGenerator.EmitCall(methodCallOpCode, method, null);

            // Return from the proxy method call.
            codeGenerator.Emit(OpCodes.Ret);

            m_proxy.DefineMethodOverride(proxyMethodBuilder, interfaceMethodBuilder);
        }

        /// <summary>
        /// Verifies that the given method is legal for input into the builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to validate.
        /// </param>
        private void ValidateMethod(MethodInfo method)
        {
            if (!method.IsPublic)
            {
                throw new InvalidOperationException(Resources.Error_MemberNotPublic);
            }

            if (m_realSubjectType.IsAbstract && !method.IsStatic)
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_InstanceMethodAddedFromAbstractType, method.Name));
            }

            if (method.DeclaringType != m_realSubjectType && !m_realSubjectType.IsSubclassOf(method.DeclaringType))
            {
                throw new InvalidOperationException(
                    String.Format(Resources.Error_MethodNotMemberOfRealSubject, method.Name));
            }

            if (m_addedMembers.ContainsKey(method))
            {
                throw new ArgumentException(Resources.Error_DuplicateMember);
            }

            m_addedMembers.Add(method, true);
        }

        /// <summary>
        /// Verifies that the given property is legal for input into the builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property to validate.
        /// </param>
        private void ValidateProperty(PropertyInfo property)
        {
            if (!property.CanRead && !property.CanWrite)
            {
                throw new NotSupportedException(String.Format(Resources.Error_InvalidProperty, property.Name));
            }

            if (property.CanRead)   { ValidateMethod(property.GetGetMethod(true)); }
            if (property.CanWrite)  { ValidateMethod(property.GetSetMethod(true)); }
        }

        /// <summary>
        /// Verifies that the given event is legal for input into the builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The property to validate.
        /// </param>
        private void ValidateEvent(EventInfo eventInfo)
        {
            ValidateMethod(eventInfo.GetAddMethod(true));
            ValidateMethod(eventInfo.GetRemoveMethod(true));
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Defines a copy of the given parameters in the order supplied,
        /// using the given delegate.
        /// </summary>
        /// 
        /// <param name="methodBuilder">
        /// The delegate that defines a parameter on an implicit object.
        /// </param>
        /// 
        /// <param name="parameters">
        /// The parameters that model the paramters to define.
        /// </param>
        private static void DefineParametersWith(DefineParameterDelegate defineParameter, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                defineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
            }
        }

        /// <summary>
        /// Throws an exception when the given member is null.
        /// </summary>
        /// 
        /// <param name="member">
        /// The member to validate.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException"/>
        private static void ThrowOnNullMember(MemberInfo member)
        {
            if (member == null) { throw new ArgumentNullException(); }
        }

        /// <summary>
        /// Emits a call to the default constructor of System.Object, using
        /// the given IL generator.
        /// </summary>
        /// 
        /// <param name="codeGenerator">
        /// The IL generator that emits the constructor call.
        /// </param>
        private static void EmitObjectDefaultConstructorCall(ILGenerator codeGenerator)
        {
            codeGenerator.Emit(OpCodes.Ldarg_0);
            codeGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies that a real subject type is legal for input and usage by the builder.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The type to validate.
        /// </param>
        private static void ValidateRealSubjectType(Type realSubjectType)
        {
            // Interface and delegate types can not be abstracted
            // by a proxy and interface.
            if (realSubjectType.IsInterface || typeof(Delegate).IsAssignableFrom(realSubjectType))
            {
                throw new InvalidOperationException(String.Format(Resources.Error_InvalidRealSubjectType, realSubjectType.Name));
            }
        }

        #endregion

        #region private instance fields -----------------------------------------------------------

        private readonly Type m_realSubjectType;
        private readonly FieldInfo m_realSubjectField;
        private readonly ModuleBuilder m_module;
        private readonly TypeBuilder m_proxyInterface;
        private readonly TypeBuilder m_proxy;
        private readonly IDictionary<MemberInfo, bool> m_addedMembers;

        #endregion
    }
}