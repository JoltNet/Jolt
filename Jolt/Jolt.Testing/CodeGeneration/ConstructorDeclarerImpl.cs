// ----------------------------------------------------------------------------
// ConstructorDelcarerImpl.cs
//
// Contains the definition of the ConstructorDelcarerImpl class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/23/2008 22:28:31
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using Jolt.Testing.Properties;

namespace Jolt.Testing.CodeGeneration
{
    internal sealed class ConstructorDeclarerImpl : IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>
    {
        #region IMethodDeclarerImpl implementation ------------------------------------------------

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DeclareMethod(ConstructorBuilder, ConstructorInfo>"/>
        void IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>.DeclareMethod(ConstructorBuilder builder, ConstructorInfo realSubjectTypeMethod, Type returnType)
        {
            // The requested method is already declared as part of the
            // constructor of the given constructor builder.
            throw new NotSupportedException(String.Format(Resources.Error_DelayedConstructorDeclaration, builder.DeclaringType.Name));
        }

        /// <see cref="IMethodDeclarerImpl&lt;MethodBuilder, MethodInfo&gt;.DefineMethodParameters(ConstructorBuilder, ConstructorInfo>"/>
        void IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>.DefineMethodParameters(ConstructorBuilder builder, ConstructorInfo realSubjecTypeConstructor)
        {
            DeclarationHelper.DefineParametersWith(builder.DefineParameter, realSubjecTypeConstructor.GetParameters());
        }

        #endregion
    }
}
