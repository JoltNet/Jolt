// ----------------------------------------------------------------------------
// NonGenericConstructorDeclarer.cs
//
// Contains the definition of the NonGenericConstructorDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 22:15:41
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.Properties;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Creates and defines the ConstructorBuilder used by the
    /// ProxyTypeBuilder for non-generic constructors on the proxy type.
    /// </summary>
    internal sealed class NonGenericConstructorDeclarer : AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.ctor(TypeBuilder, MethodAttributes, ConstructorInfo, AbstractMethodDeclarerImpl&lt;ConstructorBuilder,  ConstructorInfo&gt;"/>
        internal NonGenericConstructorDeclarer(
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
                Convert.ToParameterTypes(constructorParameters));
            Implementation.DefineMethodParameters(builder, RealSubjectTypeMethod);

            return builder;
        }

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.Declare(Type)"/>
        internal override ConstructorBuilder Declare(Type desiredReturnType)
        {
            throw new InvalidOperationException(Resources.Error_OverrideCtorReturnType);
        }

        #endregion
    }
}
