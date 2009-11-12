// ----------------------------------------------------------------------------
// NonGenericMethodDeclarerImpl.cs
//
// Contains the definition of the NonGenericMethodDeclarerImpl class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 20:07:35
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Implements the IMethodDeclarerImpl contract and provides methods
    /// that declare a non-generic method.
    /// </summary>
    internal sealed class NonGenericMethodDeclarerImpl : IMethodDeclarerImpl<MethodBuilder, MethodInfo>
    {
        #region IMethodDeclarerImpl<MethodBuilder,MethodInfo> members -----------------------------

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DeclareMethod(MethodBuilder, MethodInfo, Type>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DeclareMethod(MethodBuilder builder, MethodInfo realSubjectTypeMethod, Type returnType)
        {
            builder.SetParameters(Convert.ToParameterTypes(realSubjectTypeMethod.GetParameters()));
            builder.SetReturnType(returnType);
        }

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DefineMethodParameters(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DefineMethodParameters(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            DeclarationHelper.DefineParametersWith(builder.DefineParameter, realSubjectTypeMethod.GetParameters());
        }

        #endregion
    }
}
