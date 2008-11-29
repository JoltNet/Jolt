// ----------------------------------------------------------------------------
// AbstractMethodDeclarerImplTestFixture.cs
//
// Contains the definition of the AbstractMethodDeclarerImplTestFixture class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/28/2008 18:53:47
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    public abstract class AbstractMethodDeclarerImplTestFixture<TMethodBuilder, TMethod>
        where TMethodBuilder : TMethod
        where TMethod : MethodBase
    {
        #region public methods --------------------------------------------------------------------

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            m_defaultModuleBuilder = AppDomain.CurrentDomain
                .DefineDynamicAssembly(new AssemblyName("__transientAssembly"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("__transientModule");
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

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Asserts the equality of the two given parameter lists:
        /// each pair of parameters from the given arrays (same indexes)
        /// share the same attributes and name.
        /// </summary>
        /// 
        /// <param name="expectedParameters">
        /// The expected parameters.
        /// </param>
        /// 
        /// <param name="actualParameters">
        /// The actual parameters.
        /// </param>
        protected void AssertMethodParametersEqual(ParameterInfo[] actualParameters, ParameterInfo[] expectedParameters)
        {
            Assert.That(actualParameters, Has.Length(expectedParameters.Length));

            for (int i = 0; i < actualParameters.Length; ++i)
            {
                Assert.That(actualParameters[i].Name, Is.EqualTo(expectedParameters[i].Name));
                Assert.That(actualParameters[i].Attributes, Is.EqualTo(expectedParameters[i].Attributes));
            }
        }

        #endregion

        #region private instance fields -----------------------------------------------------------

        private ModuleBuilder m_defaultModuleBuilder;
        private TypeBuilder m_defaultTypeBuilder;

        #endregion
    }
}
