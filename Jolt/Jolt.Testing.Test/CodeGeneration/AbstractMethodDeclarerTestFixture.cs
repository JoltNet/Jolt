// ----------------------------------------------------------------------------
// AbstractMethodDeclarerTestFixture.cs
//
// Contains the definition of the AbstractMethodDeclarerTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/25/2008 18:37:15
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using NUnit.Framework;

namespace Jolt.Testing.Test.CodeGeneration
{
    public abstract class AbstractMethodDeclarerTestFixture<TMethodBuilder, TMethod>
        where TMethodBuilder : TMethod
        where TMethod : MethodBase
    {
        #region public methods --------------------------------------------------------------------

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            m_defaultModuleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("__transientAssembly"),
                AssemblyBuilderAccess.Run).DefineDynamicModule("__transientModule");
        }
        
        [SetUp]
        public virtual void Setup()
        {
            m_defaultTypeBuilder = m_defaultModuleBuilder.DefineType("__transientType_" + Guid.NewGuid().ToString("N"));
        }

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets the TypeBuilder that is created for use within the scope
        /// of a single unit test.
        /// </summary>
        protected TypeBuilder CurrentTypeBuilder
        {
            get { return m_defaultTypeBuilder; }
        }

        #endregion

        #region protected class methods -----------------------------------------------------------

        /// <summary>
        /// Creates a delegate that stores its method builder parameter in the given
        /// list.
        /// </summary>
        /// 
        /// <param name="implementationArgs">
        /// The list to store method builder references.
        /// </param>
        /// 
        /// <remarks>
        /// Required as a TypeBuilder created from a dynamic module can not be queried prior
        /// to being persisted or having its construction finalized.
        /// </remarks>
        protected static DeclareMethodAttributesDelegate CreateDeclareMethodsAttributeDelegate(List<TMethodBuilder> implementationArgs)
        {
            return new DeclareMethodAttributesDelegate(delegate(TMethodBuilder builder, TMethod method)
            {
                // Stores method builder parameter passed to mock methods for future verification.
                // Required as a TypeBuilder created from a dynamic modules can not be queried prior
                // to being persisted or having its construction finalized.
                implementationArgs.Add(builder);
            });
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private ModuleBuilder m_defaultModuleBuilder;
        private TypeBuilder m_defaultTypeBuilder;

        #endregion

        #region delegate types supporting unit tests ----------------------------------------------

        protected delegate void DeclareMethodAttributesDelegate(TMethodBuilder builder, TMethod method);

        #endregion
    }
}
