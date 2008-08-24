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

using JTCG = Jolt.Testing.CodeGeneration;
using Jolt.Testing.Properties;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Implements the IMethodDeclarerImpl contract and provides methods
    /// that declare a non-generic method.
    /// </summary>
    internal sealed class NonGenericMethodDeclarerImpl : IMethodDeclarerImpl<MethodBuilder, MethodInfo>
    {
        #region IMethodDeclarerImpl implementation ------------------------------------------------

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DeclareMethod(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DeclareMethod(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            MethodDeclarerHelper.ValidateMethod(realSubjectTypeMethod, IsValid, Resources.Error_NonSupportedGenericMethod);
            
            ParameterInfo[] methodParameters = realSubjectTypeMethod.GetParameters();
            builder.SetParameters(JTCG.Convert.ToParameterTypes(methodParameters));
            builder.SetReturnType(realSubjectTypeMethod.ReturnType);
        }

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DefineMethodParameters(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DefineMethodParameters(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            MethodDeclarerHelper.ValidateMethod(realSubjectTypeMethod, IsValid, Resources.Error_NonSupportedGenericMethod);
            MethodDeclarerHelper.DefineParametersWith(builder.DefineParameter, realSubjectTypeMethod.GetParameters());
        }

        #endregion

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Returns TRUE when the given method does not contain generic parameters,
        /// FALSE otherwise.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to validate.
        /// </param>
        private static bool IsValid(MethodInfo method)
        {
            return !method.ContainsGenericParameters;
        }

        #endregion
    }
}
