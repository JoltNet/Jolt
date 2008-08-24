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

using JTCG = Jolt.Testing.CodeGeneration;
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

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.ctor(TypeBuilder, ConstructorInfo, AbstractMethodDeclarerImpl&lt;ConstructorBuilder,  ConstructorInfo&gt;"/>
        internal NonGenericConstructorDeclarer(TypeBuilder proxyTypeBuilder, ConstructorInfo realSubjectTypeConstructor,
            IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo> implementation)
            : base(proxyTypeBuilder, realSubjectTypeConstructor, implementation)
        {
            MethodDeclarerHelper.ValidateMethod(realSubjectTypeConstructor, IsValid, Resources.Error_NonSupportedGenericConstructor);
        }

        #endregion

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;ConstructorBuilder, ConstructorInfo&gt;.Declare()"/>
        internal override ConstructorBuilder Declare()
        {
            ParameterInfo[] constructorParameters = RealSubjectTypeMethod.GetParameters();
            ConstructorBuilder builder = Builder.DefineConstructor(
                ConstructorAttributes, CallingConventions.Standard, JTCG.Convert.ToParameterTypes(constructorParameters));
            Implementation.DefineMethodParameters(builder, RealSubjectTypeMethod);

            return builder;
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Returns TRUE when the given constructor does not contain generic parameters,
        /// FALSE otherwise.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor to validate.
        /// </param>
        private static bool IsValid(ConstructorInfo constructor)
        {
            // We can not use constructor.ContainsGenericParameters as the
            // property will return false for all states of a ConstructorInfo
            // object.
            return !MethodDeclarerHelper.ContainsGenericParameters(constructor.GetParameters());
        }

        #endregion

        #region private class fields --------------------------------------------------------------

        private static readonly MethodAttributes ConstructorAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        #endregion
    }
}
