// ----------------------------------------------------------------------------
// IMethodDeclarerImpl.cs
//
// Contains the definition of the IMethodDeclarerImpl interface.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 18:41:01
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Defines a contract that generalizes the the declaration logic
    /// for any <see cref="System.Reflection.MethodBase"/>.
    /// </summary>
    /// 
    /// <typeparam name="TMethodBuilder">
    /// The type of method builder used to declare the method.
    /// </typeparam>
    /// 
    /// <typeparam name="TMethod">
    /// The type of method created by <typeparamref name="TMethodBuilder"/>.
    /// </typeparam>
    /// 
    /// <remarks>
    /// Used by an <see cref="AbstractMethodDeclarer"/> to customize the style
    /// of method declaration (e.g. generic, non-generic, etc...)
    /// </remarks>
    internal interface IMethodDeclarerImpl<TMethodBuilder, TMethod>
        where TMethodBuilder : TMethod
        where TMethod : MethodBase
    {
        /// <summary>
        /// Declares a <typeparamref name="TMethod"/> using a given
        /// <typeparamref name="TMethodBuilder"/>.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The <typeparamref name="TMethodBuilder"/> used to declare the method.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The <typeparamref name="TMethod"/> to model.
        /// </param>
        /// 
        /// <param name="returnType">
        /// The return <see cref="System.Type"/> of the newly declared method.
        /// </param>
        /// 
        /// <remarks>
        /// Models the declared method after the signature of
        /// <paramref name="realSubjectTypeMethod"/>, and overrides the declared
        /// methods return type to <paramref name="returnType"/>.
        /// </remarks>
        void DeclareMethod(TMethodBuilder builder, TMethod realSubjectTypeMethod, Type returnType);

        /// <summary>
        /// Defines the parameters of a <typeparamref name="TMethod"/> using a
        /// given <typeparamref name="TMethodBuilder"/>.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The <typeparamref name="TMethodBuilder"/> used to define the
        /// method's parameters.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The <typeparamref name="TMethod"/> whose parameters are modelled.
        /// </param>
        /// 
        /// <remarks>
        /// Models the method parameters of <paramref name="realSubjectTypeMethod"/>.
        /// </remarks>
        void DefineMethodParameters(TMethodBuilder builder, TMethod realSubjectTypeMethod);
    }
}
