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
        /// Verifies the behavior fo the DeclareMethod() method when the
        /// given is method is non-generic, but the encompassing type is generic.
        /// </summary>
        [Test]
        public void DeclareMethod_GenericType_NonGenericMethod()
        {
            MethodInfo method = typeof(__GenericTestType<,,>).GetMethod("NoGenericParameters");
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_NoParameters()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("InstanceMethod", Type.EmptyTypes);
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OneParameter()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("InstanceMethod", new Type[] { typeof(int) });
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DeclareMethod_ManyParameters()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("ManyArgumentsMethod");
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains a params-array parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_ParamsArray()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("ParamsArrayArgumentsMethod");
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an out-parameter.
        /// </summary>
        [Test]
        public void DeclareMethod_OutParameter()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("OutParameterMethod");
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// given method contains an void return value.
        /// </summary>
        [Test]
        public void DeclareMethod_VoidReturnValue()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("VoidReturnValueMethod");
            AssertDeclareMethod(method, method.ReturnType);
        }

        /// <summary>
        /// Verifies the behavior of the DeclareMethod() method when the
        /// declared method's return type is overriden.
        /// </summary>
        [Test]
        public void DeclareMethod_ReturnTypeOverride()
        {
            MethodInfo method = typeof(__MethodTestType).GetMethod("ManyArgumentsMethod");
            AssertDeclareMethod(method, typeof(object));
        }

        /// <summary>
        /// Verifies the behavior fo the DefineMethodParameters() method when the
        /// given is method is non-generic, but the encompassing type is generic.
        /// </summary>
        [Test]
        public void DefineMethodParameters_GenericType_NonGenericMethod()
        {
            AssertDefineMethodParameters(typeof(__GenericTestType<,,>).GetMethod("NoGenericParameters"));
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
        /// 
        /// <param name="returnType">
        /// The declared method's return type, given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        private void AssertDeclareMethod(MethodInfo method, Type returnType)
        {
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation = new NonGenericMethodDeclarerImpl();
            implementation.DeclareMethod(m_defaultMethodBuilder, method, returnType);
            FinalizeDefaultMethodBuilder();

            Assert.That(Convert.ToParameterTypes(m_defaultMethodBuilder.GetParameters()), Is.EqualTo(Convert.ToParameterTypes(method.GetParameters())));
            Assert.That(m_defaultMethodBuilder.ReturnType, Is.EqualTo(returnType));
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
            implementation.DeclareMethod(m_defaultMethodBuilder, method, m_defaultMethodBuilder.ReturnType);
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
