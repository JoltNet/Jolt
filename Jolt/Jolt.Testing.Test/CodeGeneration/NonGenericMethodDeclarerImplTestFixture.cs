// ----------------------------------------------------------------------------
// NonGenericMethodDeclarerImplTestFixture.cs
//
// Contains the definition of the NonGenericMethodDeclarerImplTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 21:00:19
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using JTCG = Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class NonGenericMethodDeclarerImplTestFixture : AbstractMethodDeclarerImplTestFixture<MethodBuilder, MethodInfo>
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
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method is invalid (contains generic type arguments).
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DeclareMethod_InvalidMethod_GenericTypeArgs()
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction"));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method is invalid (contains generic method arguments).
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DeclareMethod_InvalidMethod_GenericMethodArgs()
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, typeof(__GenericTestType<,,>).GetMethod("GenericFunction"));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_NoParameters()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OneParameter()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("InstanceMethod", new Type[] { typeof(int) }));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_ManyParameters()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("ManyArgumentsMethod"));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains a params-array parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_ParamsArray()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("ParamsArrayArgumentsMethod"));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an out-parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OutParameter()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("OutParameterMethod"));
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an void return value.
        /// </summary>
        [Test]
        public void DeclareMethod_VoidReturnValue()
        {
            AssertDeclareMethod(typeof(__MethodTestType).GetMethod("VoidReturnValueMethod"));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method is invalid (contains generic type arguments).
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DefineMethodParameters_InvalidMethod_GenericTypeArgs()
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DefineMethodParameters(m_defaultMethodBuilder, typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction"));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method is invalid (contains generic method arguments).
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DefineMethodParameters_InvalidMethod_GenericMethodArgs()
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DefineMethodParameters(m_defaultMethodBuilder, typeof(__GenericTestType<,,>).GetMethod("GenericFunction"));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_NoParameters()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DefineMethodParameters_OneParameter()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType).GetMethod("InstanceMethod", new Type[] { typeof(int) }));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_ManyParameters()
        {
            AssertDefineMethodParameters(typeof(__MethodTestType).GetMethod("ManyArgumentsMethod"));
        }

        #endregion

        #region private instance methods ----------------------------------------------------------

        /// <summary>
        /// Asserts the expected behavior for the implementation's
        /// DeclareMethod() method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The real subject type method given as a parameter to the
        /// DeclareMethod() method.
        /// </param>
        private void AssertDeclareMethod(MethodInfo method)
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, method);
            FinalizeDefaultMethodBuilder();

            Assert.That(JTCG.Convert.ToParameterTypes(m_defaultMethodBuilder.GetParameters()), Is.EqualTo(JTCG.Convert.ToParameterTypes(method.GetParameters())));
            Assert.That(m_defaultMethodBuilder.ReturnType, Is.EqualTo(method.ReturnType));
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
        private void AssertDefineMethodParameters(MethodInfo method)
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, method);
            implementation.DefineMethodParameters(m_defaultMethodBuilder, method);
            FinalizeDefaultMethodBuilder();

            AssertMethodParametersEqual(m_defaultMethodBuilder.GetParameters(), method.GetParameters());
        }

        /// <summary>
        /// Creates the IL body of the test fixture's method builder
        /// and finalizes the method builder's declaring type.
        /// </summary>
        private void FinalizeDefaultMethodBuilder()
        {
            m_defaultMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            CurrentTypeBuilder.CreateType();
        }

        #endregion

        #region private instance data -------------------------------------------------------------

        private MethodBuilder m_defaultMethodBuilder;

        #endregion
    }
}
