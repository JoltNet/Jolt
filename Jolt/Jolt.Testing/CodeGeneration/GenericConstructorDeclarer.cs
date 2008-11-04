// ----------------------------------------------------------------------------
// GenericConstructorDeclarer.cs
//
// Contains the definition of the GenericConstructorDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 9/1/2008 13:01:20
// ----------------------------------------------------------------------------

using System;
using System.Reflection.Emit;
using System.Reflection;

using JTCG = Jolt.Testing.CodeGeneration;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Creates and defines the ConstructorBuilder used by the
    /// ProxyTypeBuilder for generic constructors on the proxy type.
    /// </summary>
    internal sealed class GenericConstructorDeclarer : AbstractMethodDeclarer<ConstructorBuilder, ConstructorInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.ctor(TypeBuilder, ConstructorInfo, AbstractMethodDeclarerImpl&lt;ConstructorBuilder,  ConstructorInfo&gt;"/>
        internal GenericConstructorDeclarer(
            TypeBuilder proxyTypeBuilder,
            ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation)
            : base(proxyTypeBuilder, realSubjectTypeConstructor, implementation) { }

        #endregion

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.Declare()"/>
        internal override ConstructorBuilder Declare()
        {
            ParameterInfo[] constructorParameters = RealSubjectTypeMethod.GetParameters();

            ConstructorBuilder builder = Builder.DefineConstructor(ConstructorAttributes, CallingConventions.HasThis,
                JTCG.Convert.ToParameterTypes(constructorParameters, Builder.GetGenericArguments()));
            Implementation.DefineMethodParameters(builder, RealSubjectTypeMethod);

            return builder;
        }

        #endregion

        #region private class fields --------------------------------------------------------------

        // TODO: Move to a shared or base class.
        private static readonly MethodAttributes ConstructorAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        #endregion
    }
}
