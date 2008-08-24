// ----------------------------------------------------------------------------
// InterfaceMethodDeclarer.cs
//
// Contains the definition of the InterfaceMethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 19:09:39
// ----------------------------------------------------------------------------

using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Creates the interface MethodBuilder used by the ProxyTypeBuilder
    /// and declares the method.
    /// </summary>
    internal sealed class InterfaceMethodDeclarer : AbstractMethodDeclarer<MethodBuilder, MethodInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.ctor(TypeBuilder, MethodInfo, AbstractMethodDeclarerImpl&lt;MethodBuilder,  MethodInfo&gt;"/>
        internal InterfaceMethodDeclarer(TypeBuilder builder, MethodInfo realSubjectTypeMethod, IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation)
            : base(builder, realSubjectTypeMethod, implementation) { }

        #endregion

        #region AbstractMethodDeclarer implementation ---------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.Declare()"/>
        internal override MethodBuilder Declare()
        {
            MethodBuilder method = Builder.DefineMethod(RealSubjectTypeMethod.Name, InterfaceMethodAttributes);
            Implementation.DeclareMethod(method, RealSubjectTypeMethod);
            Implementation.DefineMethodParameters(method, RealSubjectTypeMethod);

            return method;
        }

        #endregion

        #region private class data ----------------------------------------------------------------

        // All interface methods are public, abstract and virtual.
        private static readonly MethodAttributes InterfaceMethodAttributes =
            MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public |
            MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName;

        #endregion
    }
}
