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
    /// Defines an abstract base class that generalizes the declaration of
    /// a <see cref="System.Reflection.MethodBase"/> type.
    /// </summary>
    /// 
    /// <typeparam name="TMethodBuilder">
    /// The type of the method buider used for declaring the method.
    /// </typeparam>
    /// 
    /// <typeparam name="TMethod">
    /// The type of the method created by the method builder.
    /// </typeparam>
    /// 
    /// <remarks>
    /// Used by a <see cref="ProxyTypeBuilder"/> for creating constructor, interface
    /// method and proxy method declarations.
    /// </remarks>
    internal abstract class AbstractMethodDeclarer<TMethodBuilder, TMethod>
        where TMethodBuilder: TMethod
        where TMethod : MethodBase
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="AbstractMethodDeclarer"/> class.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The <see cref="System.Reflection.Emit.TypeBuilder"/> used to create the resulting method.
        /// </param>
        /// 
        /// <param name="methodAttributes">
        /// The <see cref="System.Reflection.MethodAttributes"/> applied to the method upon declaration.
        /// </param>
        /// 
        /// <param name="realSubjectTypeMethod">
        /// The <see cref="System.Reflection.MethodBase"/> object representing the real subject type method
        /// from which the resulting method is modelled.
        /// </param>
        /// 
        /// <param name="implementation">
        /// The <see cref="IMethodDeclarerImpl"/> implementation that performs the declaration.
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

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Declares a new <typeparamref name="TMethod"/>, modelling it after <see cref="RealSubjectTypeMethod"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the <typeparamref name="TMethodBuilder"/> object containing the method declaration.
        /// </returns>
        internal abstract TMethodBuilder Declare();

        /// <summary>
        /// Declares a new <typeparamref name="TMethod"/>, modelling it after <see cref="RealSubjectTypeMethod"/>.
        /// Allows the option of overriding the new method's return type.
        /// </summary>
        /// 
        /// <param name="desiredReturnType">
        /// The desired return <see cref="System.Type"/> of the newly declared method.
        /// </param>
        /// 
        /// <returns>
        /// Returns the <typeparamref name="TMethodBuilder"/> object containing the method declaration.
        /// </returns>
        internal abstract TMethodBuilder Declare(Type desiredReturnType);

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets the associated <see cref="System.Reflection.Emit.TypeBuilder"/> object.
        /// </summary>
        protected TypeBuilder Builder
        {
            get { return m_builder; }
        }

        /// <summary>
        /// Gets the associated <see cref="System.Reflection.MethodAttributes"/>.
        /// </summary>
        protected MethodAttributes MethodAttributes
        {
            get { return m_methodAttributes; }
        }

        /// <summary>
        /// Gets the associated <typeparamref name="TMethod"/>, representing a
        /// real subject type method.
        /// </summary>
        protected TMethod RealSubjectTypeMethod
        {
            get { return m_realSubjectTypeMethod; }
        }

        /// <summary>
        /// Gets the associated <see cref="IMethodDeclarerImpl"/> implementation.
        /// </summary>
        protected IMethodDeclarerImpl<TMethodBuilder, TMethod> Implementation
        {
            get { return m_implementation; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly TypeBuilder m_builder;
        private readonly MethodAttributes m_methodAttributes;
        private readonly TMethod m_realSubjectTypeMethod;
        private readonly IMethodDeclarerImpl<TMethodBuilder, TMethod> m_implementation;

        #endregion
    }
}
