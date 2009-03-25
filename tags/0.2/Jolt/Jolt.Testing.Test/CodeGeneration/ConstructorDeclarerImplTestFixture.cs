// ----------------------------------------------------------------------------
// ConstructorDeclarerImplTestFixture.cs
//
// Contains the definition of the ConstructorDeclarerImplTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/28/2008 17:38:54
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class ConstructorDeclarerImplTestFixture : AbstractMethodDeclarerImplTestFixture<ConstructorBuilder, ConstructorInfo>
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the implementation of the DeclareMethod() method.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void DeclareMethod()
        {
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = new ConstructorDeclarerImpl();
            ConstructorInfo constructor = typeof(__ConstructorTestType).GetConstructor(Type.EmptyTypes);
            implementation.DeclareMethod(CreateConstructorBuilder(constructor), constructor);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_NoParameters()
        {
            AssertDefineMethodParameters(typeof(__ConstructorTestType).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DefineMethodParameters_OneParameter()
        {
            AssertDefineMethodParameters(typeof(__ConstructorTestType).GetConstructor(new Type[] { typeof(int) }));
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_ManyParameters()
        {
            AssertDefineMethodParameters(typeof(__ConstructorTestType).GetConstructor(new Type[] { typeof(int), typeof(int) }));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Asserts the expected behavior for the implementation's
        /// DefineMethodParameters() method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The real subject type constructor given as a parameter to the
        /// DefineMethodParameters() method.
        /// </param>
        private void AssertDefineMethodParameters(ConstructorInfo constructor)
        {
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = new ConstructorDeclarerImpl();
            ConstructorBuilder builder = CreateConstructorBuilder(constructor);
            implementation.DefineMethodParameters(builder, constructor);
            
            builder.GetILGenerator().Emit(OpCodes.Ret);
            CurrentTypeBuilder.CreateType();

            AssertMethodParametersEqual(builder.GetParameters(), constructor.GetParameters());
        }

        /// <summary>
        /// Defines a constructor on the default type builder, copying
        /// from the given constructor's parameters.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor whose parameters are copied.
        /// </param>
        private ConstructorBuilder CreateConstructorBuilder(ConstructorInfo constructor)
        {
            return CurrentTypeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, Convert.ToParameterTypes(constructor.GetParameters()));
        }

        #endregion
    }
}
