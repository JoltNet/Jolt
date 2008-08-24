// ----------------------------------------------------------------------------
// AbstractMethodDeclarer.cs
//
// Contains the definition of the AbstractMethodDeclarer class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/19/2008 13:58:33
// ----------------------------------------------------------------------------

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
        /// <param name="realSubjectTypeMethod">
        /// The real subject type method from which the resulting method is modelled.
        /// </param>
        /// 
        /// <param name="implementation">
        /// The declarer implementation.
        /// </param>
        internal AbstractMethodDeclarer(TypeBuilder builder, TMethod realSubjectTypeMethod, IMethodDeclarerImpl<TMethodBuilder, TMethod> implementation)
        {
            m_builder = builder;
            m_realSubjectTypeMethod = realSubjectTypeMethod;
            m_implementation = implementation;
        }

        #endregion

        // TODO: consider making these properties protected, then testing via reflection.
        #region internal instance properties ------------------------------------------------------

        /// <summary>
        /// Gets the associated type builder .
        /// </summary>
        internal TypeBuilder Builder
        {
            get { return m_builder; }
        }

        /// <summary>
        /// Gets the associated real subject type method.
        /// </summary>
        internal TMethod RealSubjectTypeMethod
        {
            get { return m_realSubjectTypeMethod; }
        }

        /// <summary>
        /// Gets the implementation of the declarer.
        /// </summary>
        internal IMethodDeclarerImpl<TMethodBuilder, TMethod> Implementation
        {
            get { return m_implementation; }
        }

        #endregion

        #region internal instance methods ---------------------------------------------------------

        /// <summary>
        /// Declares a new method, modelling after the associated method
        /// from the real subject type.
        /// </summary>
        internal abstract TMethodBuilder Declare();

        #endregion

        #region private instance fields -----------------------------------------------------------

        private readonly TypeBuilder m_builder;
        private readonly TMethod m_realSubjectTypeMethod;
        private readonly IMethodDeclarerImpl<TMethodBuilder, TMethod> m_implementation;

        #endregion
    }
}
