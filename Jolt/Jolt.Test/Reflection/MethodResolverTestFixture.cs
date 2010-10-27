// ----------------------------------------------------------------------------
// MethodResolverTestFixture.cs
//
// Contains the definition of the MethodResolverTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 10/3/2010 9:59:20
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Reflection;
using NUnit.Framework;

namespace Jolt.Test.Reflection
{
    [TestFixture]
    public sealed class MethodResolverTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the GetProperty method when the given
        /// argument is null.
        /// </summary>
        [Test]
        public void GetProperty_NullArgument()
        {
            Assert.That(() => MethodResolver.GetProperty(null), Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies the behavior of the GetProperty method when the given
        /// method does not resolve to a property.
        /// </summary>
        [Test]
        public void GetProperty_NoMatchFound()
        {
            Assert.That(MethodResolver.GetProperty(typeof(Object).GetMethod("ToString")), Is.Null);
        }

        /// <summary>
        /// Verifies the behavior of the GetProperty method when resolving a
        /// get or set method.
        /// </summary>
        [Test]
        public void GetProperty(
            [Values("get_", "set_")] string methodNamePrefix,
            [Values(PublicMethod, PublicStaticMethod, PrivateMethod, PrivateStaticMethod)] MethodAttributes propertyMethodAttributes)
        {
            Type dynamicPropertyType = CreatePropertyType(propertyMethodAttributes);
            PropertyInfo resolvedProperty = MethodResolver.GetProperty(dynamicPropertyType.GetMethod(methodNamePrefix + "propertyName", CompoundBindingFlags.Any));

            Assert.That(resolvedProperty, Is.Not.Null);
            Assert.That(resolvedProperty, Is.SameAs(dynamicPropertyType.GetProperty("propertyName", CompoundBindingFlags.Any)));
        }

        /// <summary>
        /// Verifies the behavior of the GetProperty method when resolved from a ".other" method.
        /// </summary>
        [Test]
        public void GetProperty_OtherMethods(
            [Values("public", "publicStatic", "private", "privateStatic")] string methodName,
            [Values(PublicMethod, PublicStaticMethod, PrivateMethod, PrivateStaticMethod)] MethodAttributes propertyMethodAttributes)
        {
            Type dynamicPropertyType = CreatePropertyType(propertyMethodAttributes);
            PropertyInfo resolvedProperty = MethodResolver.GetProperty(dynamicPropertyType.GetMethod(methodName, CompoundBindingFlags.Any), true);

            Assert.That(resolvedProperty, Is.Not.Null);
            Assert.That(resolvedProperty, Is.SameAs(dynamicPropertyType.GetProperty("propertyName")));
        }

        /// <summary>
        /// Verifies the behavior of the GetEvent method when the given
        /// argument is null.
        /// </summary>
        [Test]
        public void GetEvent_NullArgument()
        {
            Assert.That(() => MethodResolver.GetEvent(null), Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies the behavior of the GetEvent method when the given
        /// method does not resolve to an event.
        /// </summary>
        [Test]
        public void GetEvent_NoMatchFound()
        {
            Assert.That(MethodResolver.GetEvent(typeof(Object).GetMethod("ToString")), Is.Null);
        }

        /// <summary>
        /// Verifies the behavior of the GetEvent method when resolving an
        /// add, remove, or raise method.
        /// </summary>
        [Test]
        public void GetEvent(
            [Values("add_", "remove_", "raise_")] string methodNamePrefix,
            [Values(PublicMethod, PublicStaticMethod, PrivateMethod, PrivateStaticMethod)] MethodAttributes eventMethodAttributes)
        {
            Type dynamicEventType = CreateEventType(eventMethodAttributes);
            EventInfo resolvedEvent = MethodResolver.GetEvent(dynamicEventType.GetMethod(methodNamePrefix + "eventName", CompoundBindingFlags.Any));

            Assert.That(resolvedEvent, Is.Not.Null);
            Assert.That(resolvedEvent, Is.SameAs(dynamicEventType.GetEvent("eventName", CompoundBindingFlags.Any)));
        }

        /// <summary>
        /// Verifies the behavior of the GetEvent method when resolving a ".other" method.
        /// </summary>
        [Test]
        public void GetEvent_OtherMethods(
            [Values("public", "publicStatic", "private", "privateStatic")] string methodName,
            [Values(PublicMethod, PublicStaticMethod, PrivateMethod, PrivateStaticMethod)] MethodAttributes eventMethodAttributes)
        {
            Type dynamicEventType = CreateEventType(eventMethodAttributes);
            EventInfo resolvedEvent = MethodResolver.GetEvent(dynamicEventType.GetMethod(methodName, CompoundBindingFlags.Any), true);

            Assert.That(resolvedEvent, Is.Not.Null);
            Assert.That(resolvedEvent, Is.SameAs(dynamicEventType.GetEvent("eventName")));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a dynamic type with several events, each event having an
        /// add, remove, and raise method.
        /// </summary>
        /// 
        /// <param name="methodAttributes">
        /// The attributes of the methods that implement the event.
        /// </param>
        ///
        /// <returns>
        /// Returns the newly created type.
        /// </returns>
        private static Type CreateEventType(MethodAttributes methodAttributes)
        {
            TypeBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("abc"), AssemblyBuilderAccess.Run)
                    .DefineDynamicModule("xyz")
                    .DefineType("DynamicEventType", TypeAttributes.Abstract | TypeAttributes.Public);
            
            EventBuilder eventBuilder = builder.DefineEvent("eventName", EventAttributes.None, typeof(Action));
            eventBuilder.SetAddOnMethod(CreateNoOpMethod(builder, "add_eventName", methodAttributes));
            eventBuilder.SetRemoveOnMethod(CreateNoOpMethod(builder, "remove_eventName", methodAttributes));
            eventBuilder.SetRaiseMethod(CreateNoOpMethod(builder, "raise_eventName", methodAttributes));

            eventBuilder.AddOtherMethod(CreateNoOpMethod(builder, "public", MethodAttributes.Public));
            eventBuilder.AddOtherMethod(CreateNoOpMethod(builder, "publicStatic", MethodAttributes.Public | MethodAttributes.Static));
            eventBuilder.AddOtherMethod(CreateNoOpMethod(builder, "private", MethodAttributes.Private));
            eventBuilder.AddOtherMethod(CreateNoOpMethod(builder, "privateStatic", MethodAttributes.Private | MethodAttributes.Static));

            return builder.CreateType();
        }


        /// <summary>
        /// Creates a dynamic type with a single property containing a get and set accessor,
        /// as well as several other methods.
        /// </summary>
        /// 
        /// <param name="methodAttributes">
        /// The attributes of the methods that implement the property.
        /// </param>
        ///
        /// <returns>
        /// Returns the newly created type.
        /// </returns>
        private static Type CreatePropertyType(MethodAttributes methodAttributes)
        {
            TypeBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("def"), AssemblyBuilderAccess.Run)
                    .DefineDynamicModule("xyz")
                    .DefineType("DynamicPropertyType", TypeAttributes.Abstract | TypeAttributes.Public);

            PropertyBuilder propertyBuilder = builder.DefineProperty("propertyName", PropertyAttributes.None, typeof(bool), Type.EmptyTypes);
            MethodBuilder methodBuilder = builder.DefineMethod("get_propertyName", methodAttributes, typeof(bool), Type.EmptyTypes);
            ILGenerator codeGenerator = methodBuilder.GetILGenerator();
            codeGenerator.Emit(OpCodes.Ldc_I4_0);
            codeGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(methodBuilder);
            propertyBuilder.SetSetMethod(CreateNoOpMethod(builder, "set_propertyName", methodAttributes));

            propertyBuilder.AddOtherMethod(CreateNoOpMethod(builder, "public", MethodAttributes.Public));
            propertyBuilder.AddOtherMethod(CreateNoOpMethod(builder, "publicStatic", MethodAttributes.Public | MethodAttributes.Static));
            propertyBuilder.AddOtherMethod(CreateNoOpMethod(builder, "private", MethodAttributes.Private));
            propertyBuilder.AddOtherMethod(CreateNoOpMethod(builder, "privateStatic", MethodAttributes.Private | MethodAttributes.Static));

            return builder.CreateType();
        }

        /// <summary>
        /// Creates a dynamic no-op method with the given name and attributes.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The <see cref="System.Reflection.Emit.TypeBuilder"/> in which the method is created.
        /// </param>
        /// 
        /// <param name="methodName">
        /// The name of the method to create.
        /// </param>
        /// 
        /// <param name="methodAttributes">
        /// The attributes to associate with the method.
        /// </param>
        private static MethodBuilder CreateNoOpMethod(TypeBuilder builder, string methodName, MethodAttributes methodAttributes)
        {
            MethodBuilder methodBuilder = builder.DefineMethod(methodName, methodAttributes);
            ILGenerator codeGenerator = methodBuilder.GetILGenerator();
            codeGenerator.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private const MethodAttributes PublicMethod = MethodAttributes.Public;
        private const MethodAttributes PublicStaticMethod = MethodAttributes.Public | MethodAttributes.Static;
        private const MethodAttributes PrivateMethod = MethodAttributes.Private;
        private const MethodAttributes PrivateStaticMethod = MethodAttributes.Private | MethodAttributes.Static;

        #endregion
    }
}