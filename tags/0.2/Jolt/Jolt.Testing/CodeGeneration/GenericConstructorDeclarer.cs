// ----------------------------------------------------------------------------
// GenericConstructorDeclarer.cs
//
// Contains the definition of the GenericConstructorDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 9/1/2008 13:01:20
// ----------------------------------------------------------------------------

using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Creates and defines the ConstructorBuilder used by the
    /// ProxyTypeBuilder for generic constructors on the proxy type.
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

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.Declare()"/>
        internal override ConstructorBuilder Declare()
        {
            ParameterInfo[] constructorParameters = RealSubjectTypeMethod.GetParameters();

            ConstructorBuilder builder = Builder.DefineConstructor(MethodAttributes, CallingConventions.HasThis,
                Convert.ToParameterTypes(constructorParameters, Builder.GetGenericArguments()));
            Implementation.DefineMethodParameters(builder, RealSubjectTypeMethod);

            return builder;
        }

        #endregion
    }
}
