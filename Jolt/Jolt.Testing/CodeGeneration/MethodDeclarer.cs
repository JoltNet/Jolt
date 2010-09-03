// ----------------------------------------------------------------------------
// MethodDeclarer.cs
//
// Contains the definition of the MethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 19:09:39
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Defines a concrete <see cref="AbstractMethodDeclarer"/> class that creates
    /// <see cref="System.Reflection.Emit.MethodBuilder"/> objects for declaring methods.
    /// </summary>
    internal sealed class MethodDeclarer : AbstractMethodDeclarer<MethodBuilder, MethodInfo>
    {
        #region constructors ----------------------------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.ctor(TypeBuilder, MethodInfo, AbstractMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;)"/>
        internal MethodDeclarer(
            TypeBuilder builder,
            MethodAttributes methodAttributes,
            MethodInfo realSubjectTypeMethod,
            IMethodDeclarerImpl<MethodBuilder, MethodInfo> implementation)
            : base(builder, methodAttributes, realSubjectTypeMethod, implementation) { }

        #endregion

        #region AbstractMethodDeclarer members ----------------------------------------------------

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.Declare()"/>
        internal override MethodBuilder Declare()
        {
            return Declare(RealSubjectTypeMethod.ReturnType);
        }

        /// <see cref="AbstractMethodDeclarer&lt;MethodBuilder, MethodInfo&gt;.Declare(Type)"/>
        internal override MethodBuilder Declare(Type desiredReturnType)
        {
            MethodBuilder method = Builder.DefineMethod(RealSubjectTypeMethod.Name, MethodAttributes);
            Implementation.DeclareMethod(method, RealSubjectTypeMethod, desiredReturnType);
            Implementation.DefineMethodParameters(method, RealSubjectTypeMethod);

            return method;
        }

        #endregion
    }
}
