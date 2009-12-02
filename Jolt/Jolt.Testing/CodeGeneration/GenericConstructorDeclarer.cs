// ----------------------------------------------------------------------------
// GenericConstructorDeclarer.cs
//
// Contains the definition of the GenericConstructorDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 9/1/2008 13:01:20
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.Properties;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Defines a concrete <see cref="AbstractMethodDeclarer"/> class that creates
    /// <see cref="System.Reflection.Emit.ConstructorBuilder"/> objects for generic
    /// constructors on the proxy type.
    /// </summary>
    internal sealed class GenericConstructorDeclarer : AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.ctor(TypeBuilder, MethodInfo, ConstructorInfo, AbstractMethodDeclarerImpl&lt;ConstructorBuilder, ConstructorInfo&gt;"/>
        internal GenericConstructorDeclarer(
            TypeBuilder proxyTypeBuilder,
            MethodAttributes constructorAttributes,
            ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation)
            : base(proxyTypeBuilder, constructorAttributes, realSubjectTypeConstructor, implementation) { }

        #endregion

        #region AbstractMethodDeclarer members ----------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.Declare()"/>
        internal override ConstructorBuilder Declare()
        {
            ParameterInfo[] constructorParameters = RealSubjectTypeMethod.GetParameters();

            ConstructorBuilder builder = Builder.DefineConstructor(MethodAttributes, CallingConventions.HasThis,
                Convert.ToParameterTypes(constructorParameters, Builder.GetGenericArguments()));
            Implementation.DefineMethodParameters(builder, RealSubjectTypeMethod);

            return builder;
        }

        /// <summary>
        /// Not supported since a constructor does not have a return type.
        /// </summary>
        ///
        /// <exception cref="System.InvalidOperationException">
        /// Any invocation.
        /// </exception>
        internal override ConstructorBuilder Declare(Type desiredReturnType)
        {
            throw new InvalidOperationException(Resources.Error_OverrideCtorReturnType);
        }

        #endregion
    }
}
