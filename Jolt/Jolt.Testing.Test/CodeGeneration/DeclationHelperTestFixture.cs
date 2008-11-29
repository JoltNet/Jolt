// ----------------------------------------------------------------------------
// DeclarationHelperTestFixture.cs
//
// Contains the definition of the DeclarationHelperTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/26/2008 09:09:35
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using JTCG = Jolt.Testing.CodeGeneration;

namespace Jolt.Testing.Test.CodeGeneration
{
    using DefineParameterDelegate = Func<int, ParameterAttributes, string, ParameterBuilder>;


    [TestFixture]
    public sealed class DeclarationHelperTestFixture
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
                DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<DefineParameterDelegate>();
                Mocker.Current.ReplayAll();

                DeclarationHelper.DefineParametersWith(defineParameter, new ParameterInfo[0]);
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
                DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<DefineParameterDelegate>();

                // Expectations.
                // The defineParameter delegate is called for each
                // of the given parameters.
                ParameterInfo[] expectedParameters = typeof(__MethodTestType).GetMethod("InstanceMethod", new Type[] { typeof(int) }).GetParameters();
                Expect.Call(defineParameter(1, expectedParameters[0].Attributes, expectedParameters[0].Name))
                    .Return(null);

                Mocker.Current.ReplayAll();

                DeclarationHelper.DefineParametersWith(defineParameter, expectedParameters);
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
                DefineParameterDelegate defineParameter = Mocker.Current.CreateMock<DefineParameterDelegate>();

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

                DeclarationHelper.DefineParametersWith(defineParameter, expectedParameters);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when none of the given parameters are generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_None()
        {
            Assert.That(!DeclarationHelper.ContainsGenericParameters(typeof(__MethodTestType).GetMethod("ManyArgumentsMethod").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when none of the given parameters are generic, yet the declaring
        /// type is generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_None_GenericType()
        {
            Assert.That(!DeclarationHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetConstructor(Type.EmptyTypes).GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters are generic and defined on the declaring type.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_TypeOnly()
        {
            Assert.That(DeclarationHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("NonGenericFunction").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters are generic and defined only on the declaring
        /// method.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_MethodOnly()
        {
            Assert.That(DeclarationHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("GenericFunction").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the ContainsGenericParameters() method
        /// when given parameters vary between genric (method-defined), generic
        /// (type-defined), and non-generic.
        /// </summary>
        [Test]
        public void ContainsGenericParameters_Mixed()
        {
            Assert.That(DeclarationHelper.ContainsGenericParameters(typeof(__GenericTestType<,,>).GetMethod("GenericFunction_MixedArgs").GetParameters()));
        }

        /// <summary>
        /// Verifies the behavior of the CopyTypeConstraints() method
        /// when the rank of the input arrays do not match.
        /// </summary>
        [Test, ExpectedException(typeof(RankException))]
        public void CopyTypeConstraints_InputArrayLengthMismatch()
        {
            DeclarationHelper.CopyTypeConstraints(Type.EmptyTypes, new GenericTypeParameterBuilder[5]);
        }

        [Test]
        public void CopyTypeConstraints()
        {
            TypeBuilder builder = AppDomain.CurrentDomain
                .DefineDynamicAssembly(new AssemblyName("__transientAssembly"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("__transientModule")
                .DefineType("__transientType_" + Guid.NewGuid().ToString("N"));

            Type[] sourceTypes = typeof(__GenericTestType<,,>).GetGenericArguments();
            GenericTypeParameterBuilder[] targetTypes = builder.DefineGenericParameters(
                JTCG.Convert.ToTypeNames(sourceTypes));

            DeclarationHelper.CopyTypeConstraints(sourceTypes, targetTypes);
            Type[] genericArguments = builder.CreateType().GetGenericArguments();

            AssertExpectedParameterAttribtues(genericArguments, new GenericParameterAttributes[]
            {
                GenericParameterAttributes.NotNullableValueTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
                GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
                GenericParameterAttributes.None
            });

            AssertExpectedParameterConstraints(genericArguments, new Type[][]
            {
                new Type[] { typeof(ValueType) },
                new Type[] { genericArguments[2] },
                new Type[] { typeof(IDisposable), typeof(MarshalByRefObject) }
            });
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Asserts that each type in a given array contains the expected
        /// generic parameter attributes as denoted in another given array.
        /// </summary>
        /// 
        /// <param name="genericArguments">
        /// The arguments to validate.
        /// </param>
        /// 
        /// <param name="expectedAttributes">
        /// The expected parameter attributes.
        /// </param>
        /// 
        /// <remarks>
        /// The lengths of each given array must match as the attributes in
        /// one array are matched up to the type in the corresponding array
        /// by position.
        /// </remarks>
        private static void AssertExpectedParameterAttribtues(Type[] genericArguments, GenericParameterAttributes[] expectedAttributes)
        {
            if (genericArguments.Length != expectedAttributes.Length) { throw new RankException(); }

            for (int i = 0; i < genericArguments.Length; ++i)
            {
                Assert.That(genericArguments[i].GenericParameterAttributes, Is.EqualTo(expectedAttributes[i]));
            }
        }

        /// <summary>
        /// Asserts that each type in a given array contains the expected
        /// generic parameter constraints as denoted in another given array.
        /// </summary>
        /// 
        /// <param name="genericArguments">
        /// The arguments to validate.
        /// </param>
        /// 
        /// <param name="expectedConstraints">
        /// The expected parameter constraints.
        /// </param>
        /// 
        /// <remarks>
        /// The lengths of each given array must match as the constraints in
        /// one array are matched up to the type in the corresponding array
        /// by position.
        /// </remarks>
        private static void AssertExpectedParameterConstraints(Type[] genericArguments, Type[][] expectedConstraints)
        {
            if (genericArguments.Length != expectedConstraints.Length) { throw new RankException(); }

            for (int i = 0; i < genericArguments.Length; ++i)
            {
                Assert.That(genericArguments[i].GetGenericParameterConstraints(), Is.EquivalentTo(expectedConstraints[i]));
            }
        }

        #endregion
    }
}
