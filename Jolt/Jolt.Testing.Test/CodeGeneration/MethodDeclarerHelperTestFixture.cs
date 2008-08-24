// ----------------------------------------------------------------------------
// MethodDeclarerHelperTestFixture.cs
//
// Contains the definition of the MethodDeclarerHelperTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/26/2008 09:09:35
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class MethodDeclarerHelperTestFixture
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the DeclareParametersWith() method
        /// when no parameters are provided.
        /// </summary>
        [Test]
        public void DeclareParametersWith_NoParameters()
        {
            With.Mocks(delegate
            {
                MethodDeclarerHelper.DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<MethodDeclarerHelper.DefineParameterDelegate>();
                Mocker.Current.ReplayAll();

                MethodDeclarerHelper.DefineParametersWith(defineParameter, new ParameterInfo[0]);
            });
        }

        /// <summary>
        /// Verifies the behavior of the DeclareParametersWith() method
        /// when one parameter is provided.
        /// </summary>
        [Test]
        public void DeclareParametersWith_OneParameter()
        {
            With.Mocks(delegate
            {
                MethodDeclarerHelper.DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<MethodDeclarerHelper.DefineParameterDelegate>();

                // Expectations.
                // The defineParameter delegate is called for each
                // of the given parameters.
                ParameterInfo[] expectedParameters = typeof(__MethodTestType).GetMethod("InstanceMethod", new Type[] { typeof(int) }).GetParameters();
                Expect.Call(defineParameter(1, expectedParameters[0].Attributes, expectedParameters[0].Name))
                    .Return(null);

                Mocker.Current.ReplayAll();

                MethodDeclarerHelper.DefineParametersWith(defineParameter, expectedParameters);
            });
        }

        /// <summary>
        /// Verifies the behavior of the DeclareParametersWith() method
        /// when many parameters are provided.
        /// </summary>
        [Test]
        public void DeclareParametersWith_ManyParameters()
        {
            With.Mocks(delegate
            {
                MethodDeclarerHelper.DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<MethodDeclarerHelper.DefineParameterDelegate>();

                // Expectations.
                // The defineParameter delegate is called for each
                // of the given parameters.
                ParameterInfo[] expectedParameters = typeof(__MethodTestType).GetMethod("ManyArgumentsMethod").GetParameters();

                using (Mocker.Current.Ordered())
                {
                    for (int i = 0; i < expectedParameters.Length; ++i)
                    {
                        Expect.Call(defineParameter(i + 1, expectedParameters[i].Attributes, expectedParameters[i].Name))
                            .Return(null);
                    }
                }

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                MethodDeclarerHelper.DefineParametersWith(defineParameter, expectedParameters);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ValidateMethod() method when
        /// the given method is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage="invalidMethod: ValidateMethod_Invalid")]
        public void ValidateMethod_Invalid()
        {
            MethodDeclarerHelper.ValidateMethod(MethodInfo.GetCurrentMethod(), delegate { return false; }, "invalidMethod: {0}");
        }

        /// <summary>
        /// Verifies the behavior of the ValidateMethod() method when
        /// the given method is valid.
        /// </summary>
        [Test]
        public void ValidateMethod_Valid()
        {
            MethodDeclarerHelper.ValidateMethod(MethodInfo.GetCurrentMethod(), delegate { return true; }, String.Empty);
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when none of the given parameters are generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_None()
        {
            Assert.That(!MethodDeclarerHelper.ContainsGenericParameters(typeof(__MethodTestType).GetMethod("ManyArgumentsMethod").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when none of the given parameters are generic, yet the declaring
        /// type is generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_None_GenericType()
        {
            Assert.That(!MethodDeclarerHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetConstructor(Type.EmptyTypes).GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters are generic and defined on the declaring type.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_TypeOnly()
        {
            Assert.That(MethodDeclarerHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters are generic and defined only on the declaring
        /// method.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_MethodOnly()
        {
            Assert.That(MethodDeclarerHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("GenericFunction").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters vary between genric (method-defined), generic
        /// (type-defined), and non-generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_Mixed()
        {
            Assert.That(MethodDeclarerHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("GenericFunction_MixedArgs").GetParameters()));
        }

        #endregion
    }
}
