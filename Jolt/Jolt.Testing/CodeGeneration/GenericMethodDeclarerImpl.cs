// ----------------------------------------------------------------------------
// GenericMethodDeclarerImpl.cs
//
// Contains the definition of the GenericMethodDeclarerImpl class.
// Copyright 2008 Steve Guidi.
//
// File created: 11/8/2008 15:01:27
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Implements the IMethodDeclarerImpl contract and provides methods
    /// that declare a generic method.
    /// </summary>
    internal sealed class GenericMethodDeclarerImpl : IMethodDeclarerImpl<MethodBuilder, MethodInfo>
    {
        #region IMethodDeclarerImpl<MethodBuilder,MethodInfo> members -----------------------------

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DeclareMethod(MethodBuilder, MethodInfo, Type>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DeclareMethod(MethodBuilder builder, MethodInfo realSubjectTypeMethod, Type returnType)
        {
            // Obtain all generic arguments that may be required by the method.
            Type[] genericTypeArguments = builder.DeclaringType.GetGenericArguments();
            Type[] realSubjectGenericMethodArguments = realSubjectTypeMethod.GetGenericArguments();

            // Create any generic method arguments required by the method.
            GenericTypeParameterBuilder[] genericMethodArguments = builder.DefineGenericParameters(
                Convert.ToTypeNames(realSubjectGenericMethodArguments));
            DeclarationHelper.CopyTypeConstraints(realSubjectGenericMethodArguments, genericMethodArguments);

            // Initialize the signature of the method.
            builder.SetParameters(Convert.ToParameterTypes(
                realSubjectTypeMethod.GetParameters(),
                genericTypeArguments,
                genericMethodArguments));
            builder.SetReturnType(Convert.ToMethodSignatureType(
                returnType,
                genericTypeArguments,
                genericMethodArguments));
        }

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DefineMethodParameters(MethodBuilder, MethodInfo>"/>
        void IMethodDeclarerImpl<MethodBuilder, MethodInfo>.DefineMethodParameters(MethodBuilder builder, MethodInfo realSubjectTypeMethod)
        {
            DeclarationHelper.DefineParametersWith(builder.DefineParameter, realSubjectTypeMethod.GetParameters());
        }

        #endregion
    }
}
