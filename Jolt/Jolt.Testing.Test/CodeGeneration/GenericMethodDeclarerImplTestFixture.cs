// ----------------------------------------------------------------------------
// GenericMethodDeclarerImplTestFixture.cs
//
// Contains the definition of the GenericMethodDeclarerImplTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 11/9/2008 09:26:16
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class GenericMethodDeclarerImplTestFixture : AbstractMethodDeclarerImplTestFixture<MethodBuilder, MethodInfo>
    {
        #region public methods --------------------------------------------------------------------

        #region initialization -------------------------------------------------------------------

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            m_defaultMethodBuilder = CurrentTypeBuilder.DefineMethod("__transientMethod", MethodAttributes.Public);
        }

        #endregion

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method, specifically
        /// the copying of generic method constraints.
        /// </summary>
        [Test]
        public void DeclareMethod_GenericMethodConstraints()
        {
            CurrentTypeBuilder.DefineGenericParameters(Convert.ToTypeNames(typeof(__GenericTestType<,,>).GetGenericArguments()));
            MethodInfo realSubjectTypeMethod = __GenericTestType<int,MemoryStream,Stream>.GenericFunction;
            
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new GenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod, realSubjectTypeMethod.ReturnType);
            FinalizeDefaultMethodBuilder(realSubjectTypeMethod.GetParameters());

            MethodInfo method = CurrentTypeBuilder.GetMethod("__transientMethod");
            Assert.That(method.IsGenericMethod);

            Type[] genericMethodArguments = method.GetGenericArguments();
            Assert.That(Convert.ToTypeNames(genericMethodArguments), Is.EquivalentTo(Convert.ToTypeNames(realSubjectTypeMethod.GetGenericArguments())));
            Assert.That(genericMethodArguments[0].GenericParameterAttributes,
                Is.EqualTo(GenericParameterAttributes.NotNullableValueTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint));

            Type[] constraints = genericMethodArguments[1].GetGenericParameterConstraints();
            Assert.That(constraints, Is.EquivalentTo(new[] { genericMethodArguments[2] }));
            Assert.That(genericMethodArguments[1].GenericParameterAttributes,
                Is.EqualTo(GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint));

            constraints = genericMethodArguments[2].GetGenericParameterConstraints();
            Assert.That(constraints, Is.EquivalentTo(new[] { typeof(IDisposable), typeof(ICloneable), typeof(MarshalByRefObject) }));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_NoParameters()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericInstanceMethod;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OneParameter()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericInstanceMethod_1;
            AssertDeclareMethod(method, method.ReturnType);
        }

        ///// <summary>
        ///// Verifies the behavior of the DeclareMethod() method when the
        ///// given method has many parameters.
        ///// </summary>
        [Test]
        public void DeclareMethod_ManyParameters()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericManyArgsMethod;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains a params-array parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_ParamsArray()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericParamArrayArgsMethod;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an out-parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OutParameter()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericOutParamMethod;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an void return value.
        /// </summary>
        [Test]
        public void DeclareMethod_VoidReturnValue()
        {
            MethodInfo method = __MethodTestType<int, object, Stream>.GenericVoidMethod;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// declared method's return type is overriden.
        /// </summary>
        [Test]
        public void DeclareMethod_ReturnTypeOverride()
        {
            AssertDeclareMethod(__MethodTestType<int, object, Stream>.GenericManyArgsMethod, typeof(object));
        }

        /// <summary>
        /// Verifies the behavior fo the DefineMethodParameters() method when the
        /// given is method is generic along with the method's declaring type.
        /// </summary>
        [Test]
        public void DefineMethodParameters_GenericType_GenericMethod()
        {
            MethodInfo method = __GenericTestType<int, MemoryStream, Stream>.GenericFunction;
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_NoParameters()
        {
            AssertDefineMethodParameters(__MethodTestType<int, object, Stream>.GenericInstanceMethod);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DefineMethodParameters_OneParameter()
        {
            AssertDefineMethodParameters(__MethodTestType<int, object, Stream>.GenericInstanceMethod_1);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_ManyParameters()
        {
            AssertDefineMethodParameters(__MethodTestType<int,object,Stream>.GenericManyArgsMethod);
        }

        #endregion

        #region private instance methods ----------------------------------------------------------

        /// <summary>
        /// Asserts the expected behavior for the implementation's
        /// DeclareMethod() method.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The real subject type method, given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        /// 
        /// <param name="returnType">
        /// The declared method's return type, given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        private void AssertDeclareMethod(MethodInfo realSubjectTypeMethod, Type returnType)
        {
            CurrentTypeBuilder.DefineGenericParameters(Convert.ToTypeNames(realSubjectTypeMethod.DeclaringType.GetGenericArguments()));

            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new GenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod, returnType);
            FinalizeDefaultMethodBuilder(realSubjectTypeMethod.GetParameters());

            MethodInfo method = CurrentTypeBuilder.GetMethod("__transientMethod");
            Assert.That(method.IsGenericMethod);
            AssertTypeEquivalence(
                Convert.ToParameterTypes(method.GetParameters()),
                Convert.ToParameterTypes(realSubjectTypeMethod.GetParameters()));
            AssertTypeEquivalence(method.ReturnType, returnType);
        }

        /// <summary>
        /// Asserts the expected behavior for the implementation's
        /// DefineMethodParameters() method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The real subject type method given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        private void AssertDefineMethodParameters(MethodInfo realSubjectTypeMethod)
        {
            CurrentTypeBuilder.DefineGenericParameters(Convert.ToTypeNames(realSubjectTypeMethod.DeclaringType.GetGenericArguments()));

            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new GenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod, realSubjectTypeMethod.ReturnType);
            implementation.DefineMethodParameters(m_defaultMethodBuilder, realSubjectTypeMethod);
            FinalizeDefaultMethodBuilder(realSubjectTypeMethod.GetParameters());

            MethodInfo method = CurrentTypeBuilder.GetMethod("__transientMethod");
            AssertMethodParametersEqual(method.GetParameters(), realSubjectTypeMethod.GetParameters());
        }

        /// <summary>
        /// Creates the IL body of the test fixture's method builder
        /// and finalizes the method builder's declaring type.
        /// </summary>
        /// 
        /// <param name="expectedMethodParams">
        /// The expected method's parameters, which are modelled by
        /// m_defaultMethodBuilder.
        /// </param>
        private void FinalizeDefaultMethodBuilder(ParameterInfo[] expectedMethodParams)
        {
            // Each method parameter must be defined in order to
            // assure that generic arguments are bound correctly
            // to the method and declaring type, 
            DeclarationHelper.DefineParametersWith(m_defaultMethodBuilder.DefineParameter, expectedMethodParams);
            m_defaultMethodBuilder.GetILGenerator().ThrowException(typeof(NotImplementedException));
            CurrentTypeBuilder.CreateType();
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Asserts that the two given type arrays are equivalent (same parameters
        /// but arbitrary ordering).  If a type is generic, then comparison is
        /// done by type name, otherwise the comparison is by strict type equality.
        /// </summary>
        /// 
        /// <param name="actualTypes">
        /// The types to verify.
        /// </param>
        /// 
        /// <param name="expectedTypes">
        /// The types to compare to.
        /// </param>
        private static void AssertTypeEquivalence(Type[] actualTypes, Type[] expectedTypes)
        {
            Assert.That(actualTypes, Has.Length.EqualTo(expectedTypes.Length));

            if (actualTypes.Length > 0)
            {
                // Compare the generic types from both type arrays.
                Assert.That(actualTypes.Where(ContainsGenericParameters).Select(GetTypeName),
                    Is.EqualTo(expectedTypes.Where(ContainsGenericParameters).Select(GetTypeName)));

                // Compare the non-generic types from both type arrays.
                Assert.That(actualTypes.Where(ContainsNoGenericParameters),
                    Is.EqualTo(expectedTypes.Where(ContainsNoGenericParameters)));
            }
        }

        /// <summary>
        /// Asserts that the two given types are equivalent. If a type is generic,
        /// then comparison is done by type name, otherwise the comparison is by
        /// strict type equality.
        /// </summary>
        /// 
        /// <param name="actualType">
        /// The type to verify.
        /// </param>
        /// 
        /// <param name="expectedType">
        /// The type to compare to.
        /// </param>
        private static void AssertTypeEquivalence(Type actualType, Type expectedType)
        {
            AssertTypeEquivalence(new[] { actualType }, new[] { expectedType });
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private MethodBuilder m_defaultMethodBuilder;

        private static readonly Func<Type, bool> ContainsGenericParameters = t => t.ContainsGenericParameters;
        private static readonly Func<Type, bool> ContainsNoGenericParameters = t => !ContainsGenericParameters(t);  // TODO: requires NOT functor.
        private static readonly Func<Type, string> GetTypeName = t => t.Name;

        #endregion
    }
}
