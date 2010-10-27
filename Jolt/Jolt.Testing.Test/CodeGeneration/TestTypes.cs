// ----------------------------------------------------------------------------
// TestTypes.cs
//
// Contains the definition of the types that support test code in this assembly.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 21:09:52
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Jolt.Functional;
using Jolt.Reflection;

namespace Jolt.Testing.Test.CodeGeneration.Types
{
    // TODO: Make types internal; fix real subject type accessibility issue.
    public static class __AbstractSealedType { }
    public static class __AbstractSealedType<T, U, V> { }

    public abstract class __AbstractType
    {
        private __AbstractType()
        {
            _InstanceEvent += Functor.ToEventHandler(Functor.NoOperation<object, EventArgs>());
        }

        public static PropertyInfo InstanceProperty { get { return ThisType.GetProperty("_InstanceProperty"); } }
        public static EventInfo InstanceEvent { get { return ThisType.GetEvent("_InstanceEvent"); } }
        
        #region property-encapsulated members -----------------------------------------------------

        public int _InstanceProperty { get { return 123; } }
        public event EventHandler<EventArgs> _InstanceEvent;

        #endregion

        private static readonly Type ThisType = typeof(__AbstractType);
    }

    public abstract class __AbstractType<T> { }

    public class __HiddenConstructorType
    {
        protected __HiddenConstructorType() { }
        internal __HiddenConstructorType(int x) { }
        private __HiddenConstructorType(byte y) { }
    }

    public class __MethodTestType
    {
        public static MethodInfo InstanceMethod { get { return ThisType.GetMethod("_InstanceMethod", Type.EmptyTypes); } }
        public static MethodInfo InstanceMethod_1 { get { return ThisType.GetMethod("_InstanceMethod", new[] { typeof(int) }); } }
        public static MethodInfo StaticMethod { get { return ThisType.GetMethod("_StaticMethod", CompoundBindingFlags.PublicStatic); } }
        public static MethodInfo VoidReturnValueMethod { get { return ThisType.GetMethod("_VoidReturnValueMethod"); } }
        public static MethodInfo ManyArgumentsMethod { get { return ThisType.GetMethod("_ManyArgumentsMethod"); } }
        public static MethodInfo ParamsArrayArgumentsMethod { get { return ThisType.GetMethod("_ParamsArrayArgumentsMethod"); } }
        public static MethodInfo OutParameterMethod { get { return ThisType.GetMethod("_OutParameterMethod"); } }
        public static MethodInfo PrivateMethod { get { return ThisType.GetMethod("_PrivateMethod", CompoundBindingFlags.NonPublicInstance); } }
        public static MethodInfo GenericMethod { get { return ThisType.GetMethod("_GenericMethod"); } }

        #region property-encapsulated members -----------------------------------------------------

        public DateTime _InstanceMethod() { return DateTime.Today; }     // Also covers return-value, and no-args method test cases.
        public DateTime _InstanceMethod(int i) { return DateTime.Today.AddMonths(i); }
        public static TimeSpan _StaticMethod() { return TimeSpan.FromHours(12.0); }
        public void _VoidReturnValueMethod() { throw new ApplicationException("void-return"); }
        public string _ManyArgumentsMethod(string s, int x, double y, DateTime d) { return String.Concat("many-args:", s, x, y, d); }
        public string _ParamsArrayArgumentsMethod(string s, params object[] i) { return "params-args:" + String.Concat(i); }
        public void _OutParameterMethod(out string s) { s = "out-param"; }
        public void _GenericMethod<A>() { throw new ApplicationException("generic-method"); }

        private void _PrivateMethod() { }

        #endregion

        private static readonly Type ThisType = typeof(__MethodTestType);
    }

    public class __MethodTestType<R, S, T>
        where R : struct
        where S : class, new()
        where T : Stream
    {
        public static MethodInfo InstanceMethod { get { return ThisType.GetMethods().Single(CreateInstanceMethodSelector(false, 0)); } }
        public static MethodInfo InstanceMethod_1 { get { return ThisType.GetMethods().Single(CreateInstanceMethodSelector(false, 1)); } }
        public static MethodInfo GenericInstanceMethod { get { return ThisType.GetMethods().Single(CreateInstanceMethodSelector(true, 0)); } }
        public static MethodInfo GenericInstanceMethod_1 { get { return ThisType.GetMethods().Single(CreateInstanceMethodSelector(true, 1)); } }
        public static MethodInfo StaticMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_StaticMethod", false)); } }
        public static MethodInfo GenericStaticMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_StaticMethod", true)); } }
        public static MethodInfo VoidMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_VoidMethod", false)); } }
        public static MethodInfo GenericVoidMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_VoidMethod", true)); } }
        public static MethodInfo ManyArgsMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_ManyArgsMethod", false)); } }
        public static MethodInfo GenericManyArgsMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_ManyArgsMethod", true)); } }
        public static MethodInfo OutParamMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_OutParamMethod", false)); } }
        public static MethodInfo GenericOutParamMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_OutParamMethod", true)); } }
        public static MethodInfo ParamArrayArgsMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_ParamArrayArgsMethod", false)); } }
        public static MethodInfo GenericParamArrayArgsMethod { get { return ThisType.GetMethods().Single(CreateMethodSelector("_ParamArrayArgsMethod", true)); } }

        private static Func<MethodInfo, bool> CreateInstanceMethodSelector(bool isGeneric, int numberOfParameters)
        {
            return method => method.Name == "_InstanceMethod" &&
                method.IsGenericMethod == isGeneric &&
                method.GetParameters().Length == numberOfParameters;
        }

        private static Func<MethodInfo, bool> CreateMethodSelector(string methodName, bool isGeneric)
        {
            return method => method.Name == methodName &&
                method.IsGenericMethod == isGeneric;
        }

        #region property-encapsulated methods -----------------------------------------------------

        public R _InstanceMethod() { return default(R); }     // Also covers return-value, and no-args method test cases.
        public R _InstanceMethod(S s) { return default(R); }
        public A _InstanceMethod<A>() where A : struct { return default(A); }
        public void _InstanceMethod<A>(A a) { throw new ArgumentException("one-parameter"); }
        public static S _StaticMethod() { return new S(); }
        public static A _StaticMethod<A>() where A : new() { return new A(); }
        public void _VoidMethod() { throw new ApplicationException("void-return"); }
        public void _VoidMethod<A, B>() { throw new ApplicationException("void-return"); }
        public string _ManyArgsMethod(R r, S s, double y, T t) { return String.Concat("many-args:", r, s, y, t.ReadByte()); }
        public string _ManyArgsMethod<A, B>(R r, S s, A a, double y, T t, B b) { return String.Concat("many-args:", r, s, a, y, t.ReadByte(), b); }
        public void _OutParamMethod(out S s) { s = new S(); }
        
        public void _OutParamMethod<A, B>(out A a, out B b, out S s)
            where A : new()
            where B : new()
        {
            a = new A();
            b = new B();
            s = new S();
        }

        public string _ParamArrayArgsMethod(T t, params R[] r)
        {
            StringBuilder result = new StringBuilder("params-args:");
            result.Append(t.ReadByte().ToString());
            
            for (int i = 0; i < r.Length; ++i)
            {
                result.Append(r[i].ToString());
            }

            return result.ToString();
        }

        public string _ParamArrayArgsMethod<A>(T t, params A[] a) where A : struct
        {
            StringBuilder result = new StringBuilder("params-args:");
            result.Append(t.ReadByte().ToString());

            for (int i = 0; i < a.Length; ++i)
            {
                result.Append(a[i].ToString());
            }

            return result.ToString();
        }

        #endregion

        private static readonly Type ThisType = typeof(__MethodTestType<,,>);
    }

    public class __PropertyTestType
    {
        public static PropertyInfo InstanceProperty { get { return ThisType.GetProperty("_InstanceProperty"); } }
        public static PropertyInfo StaticProperty { get { return ThisType.GetProperty("_StaticProperty", CompoundBindingFlags.PublicStatic); } }
        public static PropertyInfo GetterProperty { get { return ThisType.GetProperty("_Getter"); } }
        public static PropertyInfo SetterProperty { get { return ThisType.GetProperty("_Setter"); } }
        public static PropertyInfo InternalGetterProperty { get { return ThisType.GetProperty("_InternalGetter"); } }
        public static PropertyInfo PrivateSetterProeprty { get { return ThisType.GetProperty("_PrivateSetter"); } }
        public static PropertyInfo Item_1 { get { return ThisType.GetProperty("Item", new[] { typeof(int) }); } }
        public static PropertyInfo Item_3 { get { return ThisType.GetProperty("Item", new[] { typeof(int), typeof(int), typeof(int) }); } }
        public static PropertyInfo PrivateProperty { get { return ThisType.GetProperty("_PrivateProperty", CompoundBindingFlags.NonPublicInstance); } }

        #region property-encapuslated properties --------------------------------------------------

        public uint _InstanceProperty
        {
            get { return m_instancePropertyValue; }
            set { m_instancePropertyValue = value; }
        }

        public static DateTime _StaticProperty
        {
            get { return StaticPropertyValue; }
            set { StaticPropertyValue = value; }
        }

        public string _Getter
        {
            get { return "getter-only"; }
        }

        public uint _Setter
        {
            set { m_instancePropertyValue = value; }
        }

        public uint _InternalGetter
        {
            internal get { return m_instancePropertyValue; }
            set { m_instancePropertyValue = value; }
        }

        public uint _PrivateSetter
        {
            get { return m_instancePropertyValue; }
            private set { m_instancePropertyValue = value; }
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

        private int _PrivateProperty
        {
            get { return 0; }
        }

        #endregion

        private static readonly Type ThisType = typeof(__PropertyTestType);

        private uint m_instancePropertyValue = 0xdeadbeef;
        private static DateTime StaticPropertyValue = DateTime.Today;
        private int m_indexerValue = 0xbeef;
        private string m_simpleIndexerValue = "simple-value";
    }

    public class __PropertyTestType<T, U, V>
        where T : struct
        where U : struct
        where V : class, new()
    {
        public static PropertyInfo InstanceProperty { get { return ThisType.GetProperty("_InstanceProperty"); } }
        public static PropertyInfo StaticProperty { get { return ThisType.GetProperty("_StaticProperty", CompoundBindingFlags.PublicStatic); } }
        public static PropertyInfo Item_1 { get { return ThisType.GetProperty("Item", new[] { ThisType.GetGenericArguments()[0] }); } }
        public static PropertyInfo Item_3 { get { return ThisType.GetProperty("Item", ThisType.GetGenericArguments()); } }

        #region property-encapsulated properties --------------------------------------------------

        public T _InstanceProperty
        {
            get { return m_instancePropertyValue; }
            set { m_instancePropertyValue = value; }
        }

        public static U _StaticProperty
        {
            get { return StaticPropertyValue; }
            set { StaticPropertyValue = value; }
        }

        public string this[T x]
        {
            get { return m_simpleIndexerValue; }
            set { m_simpleIndexerValue = String.Concat(value, x.ToString()); }
        }

        public string this[T x, U y, V z]
        {
            get { return m_indexerValue; }
            set { m_indexerValue = String.Concat(value, x.ToString(), y.ToString(), z.ToString()); }
        }

        #endregion

        private static readonly Type ThisType = typeof(__PropertyTestType<,,>);

        private T m_instancePropertyValue = default(T);
        private static U StaticPropertyValue = default(U);
        private string m_simpleIndexerValue = "simple-value";
        private string m_indexerValue = "complex-value";
    }

    public class __EventTestType
    {
        public __EventTestType()
        {
            _PrivateEvent += Functor.ToEventHandler(Functor.NoOperation<object, EventArgs>());
        }

        static __EventTestType() { _StaticEvent = null; }

        public static EventInfo InstanceEvent { get { return ThisType.GetEvent("_InstanceEvent"); } }
        public static EventInfo StaticEvent { get { return ThisType.GetEvent("_StaticEvent", CompoundBindingFlags.PublicStatic); } }
        public static EventInfo PrivateEvent { get { return ThisType.GetEvent("_PrivateEvent", CompoundBindingFlags.NonPublicInstance); } }

        public void Raise_InstanceEvent() { RaiseEvent(_InstanceEvent); }
        public static void Raise_StaticEvent() { RaiseEvent(_StaticEvent); }

        public static void IncBy10(object sender, EventArgs args) { StaticEventData[0] += 10; }
        public static void IncBy20(object sender, EventArgs args) { StaticEventData[1] += 20; }
        public static int[] StaticEventData = { 0, 0 };

        private static void RaiseEvent(EventHandler<EventArgs> eventToRaise)
        {
            if (eventToRaise != null) { eventToRaise(null, null); }
        }

        #region property-encapsulated events ------------------------------------------------------

        public event EventHandler<EventArgs> _InstanceEvent;
        public static event EventHandler<EventArgs> _StaticEvent;
        private event EventHandler<EventArgs> _PrivateEvent;

        #endregion

        private static readonly Type ThisType = typeof(__EventTestType);
    }

    public class __EventTestType<TEventArgs>
        where TEventArgs : EventArgs
    {
        static __EventTestType() { _StaticEvent = null; }

        public static EventInfo InstanceEvent { get { return ThisType.GetEvent("_InstanceEvent"); } }
        public static EventInfo StaticEvent { get { return ThisType.GetEvent("_StaticEvent", CompoundBindingFlags.PublicStatic); } }

        public void Raise_InstanceEvent() { RaiseEvent(_InstanceEvent); }
        public static void Raise_StaticEvent() { RaiseEvent(_StaticEvent); }

        public static void IncBy10(object sender, TEventArgs args) { StaticEventData[0] += 10; }
        public static void IncBy20(object sender, TEventArgs args) { StaticEventData[1] += 20; }
        public static int[] StaticEventData = { 0, 0 };

        private static void RaiseEvent(EventHandler<TEventArgs> eventToRaise)
        {
            if (eventToRaise != null) { eventToRaise(null, null); }
        }

        #region property-encapsulated events ------------------------------------------------------

        public event EventHandler<TEventArgs> _InstanceEvent;
        public static event EventHandler<TEventArgs> _StaticEvent;

        #endregion

        private static readonly Type ThisType = typeof(__EventTestType<>);
    }

    public class __ConstructorTestType
    {
        public static ConstructorInfo Ctor_ZeroArgs { get { return ThisType.GetConstructor(Type.EmptyTypes); } }
        public static ConstructorInfo Ctor_OneArg { get { return ThisType.GetConstructor(new[] { typeof(int) }); } }
        public static ConstructorInfo Ctor_TwoArgs { get { return ThisType.GetConstructor(new[] { typeof(int), typeof(int) }); } }

        #region property-encapsulated constructors ------------------------------------------------

        public __ConstructorTestType() { throw new ApplicationException("0"); }
        public __ConstructorTestType(int x) { throw new ApplicationException(x.ToString()); }
        public __ConstructorTestType(int x, int y) { throw new ApplicationException(x.ToString() + y.ToString()); }

        #endregion

        private static readonly Type ThisType = typeof(__ConstructorTestType);
    }

    public class __ConstructorTestType<R, S>
    {
        public static ConstructorInfo Ctor_ZeroArgs { get { return ThisType.GetConstructor(Type.EmptyTypes); } }
        public static ConstructorInfo Ctor_OneArg { get { return ThisType.GetConstructor(new[] { ThisType.GetGenericArguments()[0] }); } }
        public static ConstructorInfo Ctor_TwoArgs { get { return ThisType.GetConstructor(ThisType.GetGenericArguments()); } }
        public static ConstructorInfo Ctor_ThreeArgs { get { return ThisType.GetConstructor(new[] { ThisType.GetGenericArguments()[0], ThisType.GetGenericArguments()[1], typeof(int) }); } }

        #region property-encapsulated constructors ------------------------------------------------

        public __ConstructorTestType() { throw new ApplicationException("0"); }
        public __ConstructorTestType(R r) { throw new ApplicationException(r.ToString()); }
        public __ConstructorTestType(R r, S s) { throw new ApplicationException(r.ToString() + s.ToString()); }
        public __ConstructorTestType(R r, S s, int i) { throw new ApplicationException(r.ToString() + s.ToString() + i.ToString()); }

        #endregion

        private static readonly Type ThisType = typeof(__ConstructorTestType<,>);
    }

    public class __BaseSubjectType
    {
        public virtual string _VirtualMethod() { return m_virtualPropertyValue; }
        public string _NonVirtualMethod() { return m_nonVirtualPropertyValue; }

        public virtual string _VirtualProperty
        {
            get { return m_virtualPropertyValue; }
            set { m_virtualPropertyValue = value; }
        }

        public string _NonVirtualProperty
        {
            get { return m_nonVirtualPropertyValue; }
            set { m_nonVirtualPropertyValue = value; }
        }

        public virtual event EventHandler<EventArgs> _VirtualEvent;
        public event EventHandler<EventArgs> _NonVirtualEvent;

        public void Raise_VirtualEvent() { RaiseEvent(_VirtualEvent); }
        public void Raise_NonVirtualEvent() { RaiseEvent(_NonVirtualEvent); }

        private static void RaiseEvent(EventHandler<EventArgs> eventToRaise)
        {
            if (eventToRaise != null) { eventToRaise(null, null); }
        }

        private string m_virtualPropertyValue = "Base:Virtual";
        private string m_nonVirtualPropertyValue = "Base:NonVirtual";
    }

    public class __DerivedSubjectType : __BaseSubjectType
    {
        public static MethodInfo VirtualMethod { get { return ThisType.GetMethod("_VirtualMethod"); } }
        public static PropertyInfo VirtualProperty { get { return ThisType.GetProperty("_VirtualProperty"); } }
        public static EventInfo VirtualEvent { get { return ThisType.GetEvent("_VirtualEvent"); } }
        public static MethodInfo NonVirtualMethod { get { return ThisType.GetMethod("_NonVirtualMethod"); } }
        public static PropertyInfo NonVirtualProperty { get { return ThisType.GetProperty("_NonVirtualProperty"); } }
        public static EventInfo NonVirtualEvent { get { return ThisType.GetEvent("_NonVirtualEvent"); } }

        #region property-encapsulated members -----------------------------------------------------

        public override string _VirtualMethod() { return m_virtualPropertyValue; }

        public override string _VirtualProperty
        {
            get { return m_virtualPropertyValue; }
            set { m_virtualPropertyValue = "fixed-value"; }
        }

        public override event EventHandler<EventArgs> _VirtualEvent
        {
            add { }    // always null
            remove { } // always null
        }

        #endregion

        private static readonly Type ThisType = typeof(__DerivedSubjectType);
        private string m_virtualPropertyValue = "Derived:Override";
    }

    public interface __InterfaceType { }

    public class __GenericTestType<R, S, T>
        where R : struct
        where S : class, T, new()
        where T : MarshalByRefObject, IDisposable
    {
        public static MethodInfo NonGenericFunction { get { return ThisType.GetMethod("_NonGenericFunction"); } }
        public static MethodInfo GenericFunction_MixedArgs { get { return ThisType.GetMethod("_GenericFunction_MixedArgs"); } }
        public static MethodInfo GenericFunction { get { return ThisType.GetMethod("_GenericFunction"); } }
        public static MethodInfo NoGenericParameters { get { return ThisType.GetMethod("_NoGenericParameters"); } }

        #region property-encapsulated members -----------------------------------------------------

        public R _NonGenericFunction(S s, T t) { throw new ApplicationException("non-generic-function"); }
        public R _GenericFunction_MixedArgs<A, B, C>(C c, A a, B b, T t, S s, int i) { throw new ApplicationException("generic-function"); }
        public R _GenericFunction<A, B, C>(A a, B b, C c)
            where A : struct
            where B : class, C, new()
            where C : MarshalByRefObject, ICloneable, IDisposable
        {
            throw new ApplicationException("generic-function");
        }

        public void _NoGenericParameters(int x) { throw new ApplicationException("non-generic-function-parameters"); }

        #endregion

        private static readonly Type ThisType = typeof(__GenericTestType<,,>);
    }

    public class __FirstEmptySubjectType { }
    public class __SecondEmptySubjectType { }

    public class __RealSubjectType
    {
        static __RealSubjectType()
        {
            EventHandler<EventArgs> handler = Functor.ToEventHandler(Functor.NoOperation<object, EventArgs>());
            _PublicEvent_3 += handler;
            _PublicEvent_4 += handler;
            PrivateStaticEvent += handler;
        }

        public __RealSubjectType()
        {
            EventHandler<EventArgs> handler = Functor.ToEventHandler(Functor.NoOperation<object, EventArgs>());
            _PublicEvent_1 += handler;
            _PublicEvent_2 += handler;
            InternalEvent += handler;
            ProtectedEvent += handler;
            PrivateEvent += handler;
        }

        public static MethodInfo GetTypeMethod { get { return ThisType.GetMethod("GetType"); } }
        public static MethodInfo GetHashCodeMethod { get { return ThisType.GetMethod("GetHashCode"); } }
        public static MethodInfo ToStringMethod { get { return ThisType.GetMethod("ToString"); } }
        public static MethodInfo EqualsMethod { get { return ThisType.GetMethod("Equals"); } }
        public static MethodInfo PublicMethod_1 { get { return ThisType.GetMethod("_PublicMethod_1"); } }
        public static MethodInfo PublicMethod_2 { get { return ThisType.GetMethod("_PublicMethod_2"); } }
        public static MethodInfo PublicMethod_3 { get { return ThisType.GetMethod("_PublicMethod_3", CompoundBindingFlags.PublicStatic); } }
        public static MethodInfo PublicMethod_4 { get { return ThisType.GetMethod("_PublicMethod_4", CompoundBindingFlags.PublicStatic); } }
        public static PropertyInfo PublicProperty_1 { get { return ThisType.GetProperty("_PublicProperty_1"); } }
        public static PropertyInfo PublicProperty_2 { get { return ThisType.GetProperty("_PublicProperty_2"); } }
        public static PropertyInfo PublicProperty_3 { get { return ThisType.GetProperty("_PublicProperty_3", CompoundBindingFlags.PublicStatic); } }
        public static PropertyInfo PublicProperty_4 { get { return ThisType.GetProperty("_PublicProperty_4", CompoundBindingFlags.PublicStatic); } }
        public static EventInfo PublicEvent_1 { get { return ThisType.GetEvent("_PublicEvent_1"); } }
        public static EventInfo PublicEvent_2 { get { return ThisType.GetEvent("_PublicEvent_2"); } }
        public static EventInfo PublicEvent_3 { get { return ThisType.GetEvent("_PublicEvent_3", CompoundBindingFlags.PublicStatic); } }
        public static EventInfo PublicEvent_4 { get { return ThisType.GetEvent("_PublicEvent_4", CompoundBindingFlags.PublicStatic); } }

        #region property-encapsulated members -----------------------------------------------------

        public void _PublicMethod_1() { }
        public int _PublicMethod_2() { return 0; }
        public static int _PublicMethod_3() { return 0; }
        public static void _PublicMethod_4() { }

        public int _PublicProperty_1 { get { return 0; } set { } }
        public int _PublicProperty_2 { get { return 0; } set { } }
        public static int _PublicProperty_3 { get { return 0; } set { } }
        public static int _PublicProperty_4 { get { return 0; } set { } }

        public event EventHandler<EventArgs> _PublicEvent_1;
        public event EventHandler<EventArgs> _PublicEvent_2;
        public static event EventHandler<EventArgs> _PublicEvent_3;
        public static event EventHandler<EventArgs> _PublicEvent_4;

        internal event EventHandler<EventArgs> InternalEvent;
        protected event EventHandler<EventArgs> ProtectedEvent;
        private event EventHandler<EventArgs> PrivateEvent;
        private static event EventHandler<EventArgs> PrivateStaticEvent;

        #endregion

        internal void InternalMethod() { }
        protected void ProtectedMethod() { }
        private void PrivateMethod() { }
        private static void PrivateStaticMethod() { }

        internal int InternalProperty { get { return 0; } set { } }
        protected int ProtectedProperty { get { return 0; } set { } }
        private int PrivateProperty { get { return 0; } set { } }
        private static int PrivateStaticProperty { get { return 0; } set { } }

        private static readonly Type ThisType = typeof(__RealSubjectType);
    }

    public class __ReturnTypeOverrideType<T, U>
        where T : U, new()
        where U : Exception, new()
    {
        public static MethodInfo InstanceMethod { get { return ThisType.GetMethod("_InstanceMethod"); } }
        public static MethodInfo GenericTypeParamMethod { get { return ThisType.GetMethod("_GenericTypeParamMethod"); } }
        public static MethodInfo GenericMethodParamMethod { get { return ThisType.GetMethod("_GenericMethodParamMethod"); } }
        public static PropertyInfo InstanceProperty { get { return ThisType.GetProperty("_InstanceProperty"); } }
        public static PropertyInfo GenericTypeParamProperty { get { return ThisType.GetProperty("_GenericTypeParamProperty"); } }
        public static PropertyInfo InvalidProperty { get { return ThisType.GetProperty("_InvalidProperty"); } }

        #region property-encapsulated members -----------------------------------------------------

        public PathTooLongException _InstanceMethod() { return new PathTooLongException(); }
        public T _GenericTypeParamMethod() { return new T(); }
        public V _GenericMethodParamMethod<V>() where V : U, new() { return new V(); }

        public PathTooLongException _InstanceProperty { get { return new PathTooLongException(); } }
        public T _GenericTypeParamProperty { get { return new T(); } }
        public PathTooLongException _InvalidProperty
        {
            get { return new PathTooLongException(); }
            set { Exception e = value; }
        }

        #endregion

        private static readonly Type ThisType = typeof(__ReturnTypeOverrideType<,>);
    }
}
