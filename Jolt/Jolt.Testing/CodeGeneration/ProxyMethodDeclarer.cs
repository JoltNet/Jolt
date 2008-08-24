// ----------------------------------------------------------------------------
// ProxyMethodDeclarer.cs
//
// Contains the definition of the ProxyMethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 20:32:07
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Creates the proxy MethodBuilder used by the ProxyTypeBuilder
    /// and declares the method.
    /// </summary>
    internal sealed class ProxyMethodDeclarer : AbstractMethodDeclarer<MethodBuilder, MethodInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the proxy method declarer.
        /// </summary>
        /// 
        /// <param name="interfaceName">
        /// The name of the interface that is explicitly implemented by the proxy type.
        /// </param>
        /// 
        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.ctor(TypeBuilder, MethodInfo, AbstractMethodDeclarerImpl&lt;MethodBuilder,  MethodInfo&gt;"/>
        internal ProxyMethodDeclarer(string interfaceName, TypeBuilder builder, MethodInfo realSubjectTypeMethod, IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation)
            : base(builder, realSubjectTypeMethod, implementation)
        {
            m_sInterfaceName = interfaceName;
        }

        #endregion

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.Declare()"/>
        internal override MethodBuilder Declare()
        {
            // The method explicity implements the interface method.
            MethodBuilder method = Builder.DefineMethod(String.Concat(m_sInterfaceName, '.', RealSubjectTypeMethod.Name), ProxyMethodAttributes);
            Implementation.DeclareMethod(method, RealSubjectTypeMethod);
            Implementation.DefineMethodParameters(method, RealSubjectTypeMethod);

            return method;
        }

        #endregion

        #region private instance data -------------------------------------------------------------

        private readonly string m_sInterfaceName;

        #endregion

        #region private class data ----------------------------------------------------------------

        private static readonly MethodAttributes ProxyMethodAttributes =
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
            MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Final;

        #endregion
    }
}
