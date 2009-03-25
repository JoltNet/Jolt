// ----------------------------------------------------------------------------
// GenericMethodDeclarerImplTestFixture.cs
//
// Contains the definition of the GenericMethodDeclarerImplTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 11/9/2008 09:26:16
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
            MethodInfo realSubjectTypeMethod = typeof(__GenericTestType<,,>).GetMethod("GenericFunction");
            
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new GenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod);
            FinalizeDefaultMethodBuilder(realSubjectTypeMethod.GetParameters());

            MethodInfo method = CurrentTypeBuilder.GetMethod("__transientMethod");
            Assert.That(method.IsGenericMethod);

            Type[] genericMethodArguments = method.GetGenericArguments();
            Assert.That(Convert.ToTypeNames(genericMethodArguments), Is.EquivalentTo(Convert.ToTypeNames(realSubjectTypeMethod.GetGenericArguments())));
            Assert.That(genericMethodArguments[0].GenericParameterAttributes,
                Is.EqualTo(GenericParameterAttributes.NotNullableValueTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint));

            Type[] constraints = genericMethodArguments[1].GetGenericParameterConstraints();
            Assert.That(constraints, Is.EquivalentTo(new Type[] { genericMethodArguments[2] }));
            Assert.That(genericMethodArguments[1].GenericParameterAttributes,
                Is.EqualTo(GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint));

            constraints = genericMethodArguments[2].GetGenericParameterConstraints();
            Assert.That(constraints, Is.EquivalentTo(new Type[] { typeof(IDisposable), typeof(ICloneable), typeof(MarshalByRefObject) }));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_NoParameters()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "InstanceMethod" && m.IsGenericMethod && m.GetParameters().Length == 0));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OneParameter()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "InstanceMethod" && m.IsGenericMethod && m.GetParameters().Length == 1));
        }

        ///// <summary>
        ///// Verifies the behavior of the DeclareMethod() method when the
        ///// given method has many parameters.
        ///// </summary>
        [Test]
        public void DeclareMethod_ManyParameters()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "ManyArgumentsMethod" && m.IsGenericMethod));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains a params-array parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_ParamsArray()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "ParamsArrayArgumentsMethod" && m.IsGenericMethod));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an out-parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OutParameter()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "OutParameterMethod" && m.IsGenericMethod));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an void return value.
        /// </summary>
        [Test]
        public void DeclareMethod_VoidReturnValue()
        {
            AssertDeclareMethod(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "VoidReturnValueMethod" && m.IsGenericMethod));
        }

        /// <summary>
        /// Verifies the behavior fo the DefineMethodParameters() method when the
        /// given is method is generic along with the method's declaring type.
        /// </summary>
        [Test]
        public void DefineMethodParameters_GenericType_GenericMethod()
        {
            AssertDeclareMethod(typeof(__GenericTestType<,,>).GetMethods().Single(
                m => m.Name == "GenericFunction" && m.IsGenericMethod));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_NoParameters()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "InstanceMethod" && m.IsGenericMethod && m.GetParameters().Length == 0));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DefineMethodParameters_OneParameter()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "InstanceMethod" && m.IsGenericMethod && m.GetParameters().Length == 1));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_ManyParameters()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType<,,>).GetMethods().Single(
                m => m.Name == "ManyArgumentsMethod" && m.IsGenericMethod));
        }

        #endregion

        #region private instance methods ----------------------------------------------------------

        /// <summary>
        /// Asserts the expected behavior for the implementation's
        /// DeclareMethod() method.
        /// </summary>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The real subject type method given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        private void AssertDeclareMethod(MethodInfo realSubjectTypeMethod)
        {
            CurrentTypeBuilder.DefineGenericParameters(Convert.ToTypeNames(realSubjectTypeMethod.DeclaringType.GetGenericArguments()));

            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new GenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod);
            FinalizeDefaultMethodBuilder(realSubjectTypeMethod.GetParameters());

            MethodInfo method = CurrentTypeBuilder.GetMethod("__transientMethod");
            Assert.That(method.IsGenericMethod);
            AssertTypeEquivalence(
                Convert.ToParameterTypes(method.GetParameters()),
                Convert.ToParameterTypes(realSubjectTypeMethod.GetParameters()));
            AssertTypeEquivalence(method.ReturnType, realSubjectTypeMethod.ReturnType);
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
            implementation.DeclareMethod(m_defaultMethodBuilder, realSubjectTypeMethod);
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
            Assert.That(actualTypes, Has.Length(expectedTypes.Length));

            if (actualTypes.Length > 0)
            {
                //// Compare the generic array types from both type arrays.
                //AssertTypeEquivalence(
                //    actualTypes.Where(t => t.IsArray).Select(t => t.GetElementType()).ToArray(),
                //    expectedTypes.Where(t => t.IsArray).Select(t => t.GetElementType()).ToArray());

                // Compare the generic types from both type arrays.
                Assert.That(actualTypes.Where(t => t.ContainsGenericParameters).Select(t => t.Name).ToList(),
                    Is.EqualTo(expectedTypes.Where(t => t.ContainsGenericParameters).Select(t => t.Name).ToList()));

                // Compare the non-generic types from both type arrays.
                Assert.That(actualTypes.Where(t => !t.ContainsGenericParameters).ToList(),
                    Is.EqualTo(expectedTypes.Where(t => !t.ContainsGenericParameters).ToList()));
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
            AssertTypeEquivalence(new Type[] { actualType }, new Type[] { expectedType });
        }

        #endregion

        #region private instance data -------------------------------------------------------------

        private MethodBuilder m_defaultMethodBuilder;

        #endregion
    }
}
