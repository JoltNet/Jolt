// ----------------------------------------------------------------------------
// AbstractMethodDeclarerImpl.cs
//
// Contains the definition of the IMethodDeclarerImpl interface.
// Copyright 2008 Steve Guidi.
//
// File created: 7/21/2008 18:41:01
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides methods that implement the declaration logic for an
    /// AbstractMethodDeclarer concrete specialization.  Used by the
    /// AbstractMethodDeclarer to customize the style of method declaration
    /// (e.g. generic, non-generic, etc...)
    /// </summary>
    /// 
    /// <typeparam name="TMethodBuilder">
    /// The type of the method buider used for declaring the method.
    /// </typeparam>
    /// 
    /// <typeparam name="TMethod">
    /// The type of the method created by the method builder.
    /// </typeparam>
    internal interface IMethodDeclarerImpl<TMethodBuilder, TMethod>
        where TMethodBuilder : TMethod
        where TMethod : MethodBase
    {
        /// <summary>
        /// Declares the method using the given method builder, modelling
        /// the method signature from the given real subject type method.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The method builder used to declare the method.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The method to model.
        /// </param>
        void DeclareMethod(TMethodBuilder builder, TMethod realSubjectTypeMethod);

        /// <summary>
        /// Defines the methods parameters using the given method builder,
        /// modelling the parameters from the given real subject type method.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The method builder used to define the method's parameters.
        /// </param>
        /// 
        /// <param name="realSubjecTypeMethod">
        /// The method whose parameters are modelled.
        /// </param>
        void DefineMethodParameters(TMethodBuilder builder, TMethod realSubjecTypeMethod);
    }
}
