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
    /// <summary>
    /// Implements the <see cref="IMethodDeclarerImpl"/> contract for
    /// <see cref="System.Reflection.ConstructorInfo"/> types.
    /// </summary>
    internal sealed class ConstructorDeclarerImpl : IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>
    {
        #region IMethodDeclarerImpl members -------------------------------------------------------

        /// <summary>
        /// Not supported since declaration and definition of a <see cref="System.Reflection.ConstructorInfo"/>
        /// occur when a <see cref="System.Reflection.Emit.ConstructorBuilder"/> is created.
        /// </summary>
        ///
        /// <exception cref="System.NotSupportedException">
        /// Any invocation.
        /// </exception>
        void IMethodDeclarerImpl<ConstructorBuilder, ConstructorInfo>.DeclareMethod(ConstructorBuilder builder, ConstructorInfo realSubjectTypeMethod, Type returnType)
        {
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
