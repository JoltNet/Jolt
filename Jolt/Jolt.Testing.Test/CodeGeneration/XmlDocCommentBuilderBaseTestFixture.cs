// ----------------------------------------------------------------------------
// XmlDocCommentBuilderBaseTestFixture.cs
//
// Contains the definition of the XmlDocCommentBuilderBaseTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/1/2009 11:10:53
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using Jolt.Functional;
using Jolt.Testing.CodeGeneration;
using Jolt.Testing.Test.CodeGeneration.Types;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class XmlDocCommentBuilderBaseTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            VerifyBehavior(Functor.NoOperation<XmlDocCommentBuilderBase>());
        }

        /// <summary>
        /// Verifies the behavior of the AddConstructor() method.
        /// </summary>
        [Test]
        public void AddConstructor()
        {
            VerifyBehavior(builder => builder.AddConstuctor(GetType().GetConstructor(Type.EmptyTypes)));
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method.
        /// </summary>
        [Test]
        public void AddEvent()
        {
            VerifyBehavior(builder => builder.AddEvent(typeof(__EventTestType).GetEvent("InstanceEvent")));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method.
        /// </summary>
        [Test]
        public void AddMethod()
        {
            VerifyBehavior(builder => builder.AddMethod(MethodBase.GetCurrentMethod() as MethodInfo));
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method.
        /// </summary>
        [Test]
        public void AddProperty()
        {
            VerifyBehavior(builder => builder.AddProperty(typeof(__PropertyTestType).GetProperty("InstanceProperty")));
        }

        /// <summary>
        /// Verifies the behavior of the CreateReader() method.
        /// </summary>
        [Test]
        public void CreateReader()
        {
            VerifyBehavior(delegate(XmlDocCommentBuilderBase builder)
            {
                using (XmlReader reader = builder.CreateReader())
                {
                    Assert.That(!reader.Read());
                }
            });
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the XmlDocCommentBuilderBase class.
        /// </summary>
        /// 
        /// <param name="exerciseBehavior">
        /// A delegate that executes the behavior to verify.
        /// </param>
        private void VerifyBehavior(Action<XmlDocCommentBuilderBase> exerciseBehavior)
        {
            XmlDocCommentBuilderBase builder = new XmlDocCommentBuilderBase();
            exerciseBehavior(builder);

            MethodInfo getXmlDocComments = typeof(XmlDocCommentBuilderBase)
                .GetProperty("XmlDocComments", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetGetMethod(true);

            XDocument xmlDocComments = getXmlDocComments.Invoke(builder, null) as XDocument;
            Assert.That(xmlDocComments.Root, Is.Null);
        }

        #endregion
    }
}