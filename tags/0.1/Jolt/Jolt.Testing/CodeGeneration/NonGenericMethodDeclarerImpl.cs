// ----------------------------------------------------------------------------
// NonGenericMethodDeclarerImpl.cs
//
// Contains the definition of the NonGenericMethodDeclarerImpl class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 20:07:35
// ----------------------------------------------------------------------------

using System.Reflection;
using System.Reflection.Emit;

using JTCG = Jolt.Testing.CodeGeneration;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Implements the IMethodDeclarerImpl contract and provides methods
    /// that declare a non-generic method.
    /// </summary>
    internal sealed class NonGenericMethodDeclarerImpl : IMethodDeclarerImpl<MethodBuilder, MethodInfo>
    {
        #region IMethodDeclarerImpl<MethodBuilder,MethodInfo> implementation ----------------------

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DeclareMethod(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DeclareMethod(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            builder.SetParameters(JTCG.Convert.ToParameterTypes(realSubjectTypeMethod.GetParameters()));
            builder.SetReturnType(realSubjectTypeMethod.ReturnType);
        }

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DefineMethodParameters(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DefineMethodParameters(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            DeclarationHelper.DefineParametersWith(builder.DefineParameter, realSubjectTypeMethod.GetParameters());
        }

        #endregion
    }
}
