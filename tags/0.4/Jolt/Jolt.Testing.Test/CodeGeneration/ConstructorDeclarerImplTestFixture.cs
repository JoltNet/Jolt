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
using Jolt.Testing.Properties;
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
        [Test]
        public void DeclareMethod()
        {
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation = new ConstructorDeclarerImpl();
            ConstructorInfo constructor = __ConstructorTestType.Ctor_ZeroArgs;
            ConstructorBuilder builder = CreateConstructorBuilder(constructor);

            Assert.That(
                () => implementation.DeclareMethod(builder, constructor, GetType()),
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                    String.Format(Resources.Error_DelayedConstructorDeclaration, builder.DeclaringType.Name)));

        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has no parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_NoParameters()
        {
            AssertDefineMethodParameters(__ConstructorTestType.Ctor_ZeroArgs);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has one parameter.
        /// </summary>
        [Test]
        public void DefineMethodParameters_OneParameter()
        {
            AssertDefineMethodParameters(__ConstructorTestType.Ctor_OneArg);
        }

        /// <summary>
        /// Verifies the behavior of the DefineMethodParameters() method when the
        /// given method has many parameters.
        /// </summary>
        [Test]
        public void DefineMethodParameters_ManyParameters()
        {
            AssertDefineMethodParameters(__ConstructorTestType.Ctor_TwoArgs);
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