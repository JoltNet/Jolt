// ----------------------------------------------------------------------------
// TestTypes.cs
//
// Contains the definition of the types that support test code in this assembly.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 21:09:52
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Test.CodeGeneration.Types
{
    // TODO: Make types internal; fix real subject type accessibility issue.
    public static class __AbstractSealedType { }

    public abstract class __AbstractType
    {
        public int InstanceProperty { get { return 123; } }
        public event EventHandler<EventArgs> InstanceEvent;
    }

    public class __MethodTestType
    {
        public DateTime InstanceMethod() { return DateTime.Today; }     // Also covers return-value, and no-args method test cases.
        public DateTime InstanceMethod(int i) { return DateTime.Today.AddMonths(i); }
        public static TimeSpan StaticMethod() { return TimeSpan.FromHours(12.0); }
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

    public class __EventTestType
    {
        public event EventHandler<EventArgs> InstanceEvent;
        public static event EventHandler<EventArgs> StaticEvent;
        private event EventHandler<EventArgs> PrivateEvent;

        public void RaiseInstanceEvent() { RaiseEvent(InstanceEvent); }
        public static void RaiseStaticEvent() { RaiseEvent(StaticEvent); }

        public static void IncBy10(object sender, EventArgs args) { StaticEventData[0] += 10; }
        public static void IncBy20(object sender, EventArgs args) { StaticEventData[1] += 20; }
        public static int[] StaticEventData = { 0, 0 };

        private static void RaiseEvent(EventHandler<EventArgs> eventToRaise)
        {
            if (eventToRaise != null) { eventToRaise(null, null); }
        }
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

        public virtual event EventHandler<EventArgs> VirtualEvent;
        public event EventHandler<EventArgs> NonVirtualEvent;

        public void RaiseVirtualEvent() { RaiseEvent(VirtualEvent); }
        public void RaiseNonVirtualEvent() { RaiseEvent(NonVirtualEvent); }

        private static void RaiseEvent(EventHandler<EventArgs> eventToRaise)
        {
            if (eventToRaise != null) { eventToRaise(null, null); }
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

        public override event EventHandler<EventArgs> VirtualEvent
        {
            add { }    // always null
            remove { } // always null
        }

        private string m_virtualPropertyValue = "Derived:Override";
    }

    public interface __InterfaceType { }

    public class __GenericTestType<R, S, T>
    {
        public __GenericTestType() { throw new ApplicationException("non-generic-ctor"); }
        public __GenericTestType(S s, T t, int i) { throw new ApplicationException("generic-ctor"); }

        public void NoParameters() { throw new ApplicationException("no-parameters"); }
        public R NonGenericFunction_MixedArgs(S s, T t, int i) { throw new ApplicationException("non-generic-function"); }
        public R NonGenericFunction(S s, T t) { throw new ApplicationException("non-generic-function"); }
        public R GenericFunction_MixedArgs<A, B, C>(A a, B b, C c, S s, T t, int i) { throw new ApplicationException("generic-function"); }
        public R GenericFunction<A, B, C>(A a, B b, C c) { throw new ApplicationException("generic-function"); }
    }
}
