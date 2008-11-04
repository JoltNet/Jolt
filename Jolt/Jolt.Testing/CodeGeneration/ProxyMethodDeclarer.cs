// ----------------------------------------------------------------------------
// ProxyMethodDeclarer.cs
//
// Contains the definition of the ProxyMethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 20:32:07
// ----------------------------------------------------------------------------

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

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.ctor(TypeBuilder, MethodInfo, AbstractMethodDeclarerImpl&lt;MethodBuilder,  MethodInfo&gt;"/>
        internal ProxyMethodDeclarer(TypeBuilder builder, MethodInfo realSubjectTypeMethod, IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation)
            : base(builder, realSubjectTypeMethod, implementation) { }

        #endregion

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.Declare()"/>
        internal override MethodBuilder Declare()
        {
            MethodBuilder method = Builder.DefineMethod(RealSubjectTypeMethod.Name, ProxyMethodAttributes);
            Implementation.DeclareMethod(method, RealSubjectTypeMethod);
            Implementation.DefineMethodParameters(method, RealSubjectTypeMethod);

            return method;
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        // All proxy methods are public, virtual, and explicitly sealed.
        private static readonly MethodAttributes ProxyMethodAttributes =
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

        #endregion
    }
}
