// ----------------------------------------------------------------------------
// AbstractMethodDeclarer.cs
//
// Contains the definition of the AbstractMethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/19/2008 13:58:33
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides methods that implement a construction mechanism for
    /// method declarations.  Used by the ProxyTypeBuilder for creating
    /// constructor, interface method and proxy method declarations.
    /// </summary>
    /// 
    /// <typeparam name="TMethodBuilder">
    /// The type of the method buider used for declaring the method.
    /// </typeparam>
    /// 
    /// <typeparam name="TMethod">
    /// The type of the method created by the method builder.
    /// </typeparam>
    internal abstract class AbstractMethodDeclarer<TMethodBuilder, TMethod>
        where TMethodBuilder: TMethod
        where TMethod : MethodBase
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the method declarer with a given type builder, real subject
        /// type method, and declarer implementation.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The type builder used to create the resulting method.
        /// </param>
        /// 
        /// <param name="methodAttributes">
        /// The attributes applied to the method upon declaration.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The real subject type method from which the resulting method is modelled.
        /// </param>
        /// 
        /// <param name="implementation">
        /// The declarer implementation.
        /// </param>
        internal AbstractMethodDeclarer(
            TypeBuilder builder,
            MethodAttributes methodAttributes,
            TMethod realSubjectTypeMethod,
            IMethodDeclarerImpl<TMethodBuilder, TMethod> implementation)
        {
            m_builder = builder;
            m_methodAttributes = methodAttributes;
            m_realSubjectTypeMethod = realSubjectTypeMethod;
            m_implementation = implementation;
        }

        #endregion

        #region internal instance methods ---------------------------------------------------------

        /// <summary>
        /// Declares a new method, modelling it after the associated method
        /// from the real subject type.
        /// </summary>
        internal abstract TMethodBuilder Declare();

        /// <summary>
        /// Declares a new method, modelling it after the associated method
        /// from the real subject type.  Suggests an override for the new
        /// method's return type.
        /// </summary>
        internal abstract TMethodBuilder Declare(Type desiredReturnType);

        #endregion

        #region protected instance properties -----------------------------------------------------

        /// <summary>
        /// Gets the associated type builder .
        /// </summary>
        protected TypeBuilder Builder
        {
            get { return m_builder; }
        }

        /// <summary>
        /// Gets the associated method attributes.
        /// </summary>
        protected MethodAttributes MethodAttributes
        {
            get { return m_methodAttributes; }
        }

        /// <summary>
        /// Gets the associated real subject type method.
        /// </summary>
        protected TMethod RealSubjectTypeMethod
        {
            get { return m_realSubjectTypeMethod; }
        }

        /// <summary>
        /// Gets the implementation of the declarer.
        /// </summary>
        protected IMethodDeclarerImpl<TMethodBuilder, TMethod> Implementation
        {
            get { return m_implementation; }
        }

        #endregion

        #region private instance fields -----------------------------------------------------------

        private readonly TypeBuilder m_builder;
        private readonly MethodAttributes m_methodAttributes;
        private readonly TMethod m_realSubjectTypeMethod;
        private readonly IMethodDeclarerImpl<TMethodBuilder, TMethod> m_implementation;

        #endregion
    }
}
