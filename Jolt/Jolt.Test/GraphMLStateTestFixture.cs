// ----------------------------------------------------------------------------
// GraphMLStateTestFixture.cs
//
// Contains the definition of the GraphMLStateTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/24/2009 11:32:38 AM
// ----------------------------------------------------------------------------

using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class GraphMLStateTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            GraphMLState state = new GraphMLState();

            Assert.That(state.Name, Is.Null);
            Assert.That(!state.IsFinalState);
            Assert.That(!state.IsStartState);
        }

        /// <summary>
        /// Verifies the explicit construction of the class.
        /// </summary>
        [Test]
        public void Construction_Explicit()
        {
            string stateName = "state-name";
            bool isStartState = true;
            bool isFinalState = false;
            GraphMLState state = new GraphMLState(stateName, isStartState, isFinalState);

            Assert.That(state.Name, Is.SameAs(stateName));
            Assert.That(state.IsFinalState, Is.EqualTo(isFinalState));
            Assert.That(state.IsStartState, Is.EqualTo(isStartState));
        }

        /// <summary>
        /// Verifies the behavior of the Name property.
        /// </summary>
        [Test]
        public void Name()
        {
            GraphMLState state = new GraphMLState("state", false, false);

            string initialValue = state.Name;
            string name = "new-state-name";
            state.Name = name;

            Assert.That(state.Name, Is.Not.EqualTo(initialValue));
            Assert.That(state.Name, Is.SameAs(name));
        }

        /// <summary>
        /// Verifies the serializability of the Name property.
        /// </summary>
        [Test]
        public void Name_Serializable()
        {
            PropertyInfo property = typeof(GraphMLState).GetProperty("Name");
            object[] attributes = property.GetCustomAttributes(false);

            Assert.That(attributes, Has.Length(1));
            Assert.That(attributes[0], Is.InstanceOfType(typeof(XmlAttributeAttribute)));
            Assert.That((attributes[0] as XmlAttributeAttribute).AttributeName, Is.EqualTo("stateName"));
        }

        /// <summary>
        /// Verifies the behavior of the IsStartState property.
        /// </summary>
        [Test]
        public void IsStartState()
        {
            GraphMLState state = new GraphMLState();

            bool initialValue = state.IsStartState;
            state.IsStartState = !initialValue;

            Assert.That(state.IsStartState, Is.Not.EqualTo(initialValue));
            Assert.That(state.IsStartState, Is.EqualTo(!initialValue));
        }

        /// <summary>
        /// Verifies the serializability of the IsStartState property.
        /// </summary>
        [Test]
        public void IsStartState_Serializable()
        {
            PropertyInfo property = typeof(GraphMLState).GetProperty("IsStartState");
            object[] attributes = property.GetCustomAttributes(false);

            Assert.That(attributes, Has.Length(2));
            Assert.That(attributes[0], Is.InstanceOfType(typeof(DefaultValueAttribute)));
            Assert.That((attributes[0] as DefaultValueAttribute).Value, Is.False);
            Assert.That(attributes[1], Is.InstanceOfType(typeof(XmlAttributeAttribute)));
            Assert.That((attributes[1] as XmlAttributeAttribute).AttributeName, Is.EqualTo("isStartState"));
        }

        /// <summary>
        /// Verifies the behavior of the IsFinalState property.
        /// </summary>
        [Test]
        public void IsFinalState()
        {
            GraphMLState state = new GraphMLState();

            bool initialValue = state.IsFinalState;
            state.IsFinalState = !initialValue;

            Assert.That(state.IsFinalState, Is.Not.EqualTo(initialValue));
            Assert.That(state.IsFinalState, Is.EqualTo(!initialValue));
        }

        /// <summary>
        /// Verifies the serializability of the IsFinalState property.
        /// </summary>
        [Test]
        public void IsFinalState_Serializable()
        {
            PropertyInfo property = typeof(GraphMLState).GetProperty("IsFinalState");
            object[] attributes = property.GetCustomAttributes(false);

            Assert.That(attributes, Has.Length(2));
            Assert.That(attributes[0], Is.InstanceOfType(typeof(DefaultValueAttribute)));
            Assert.That((attributes[0] as DefaultValueAttribute).Value, Is.False);
            Assert.That(attributes[1], Is.InstanceOfType(typeof(XmlAttributeAttribute)));
            Assert.That((attributes[1] as XmlAttributeAttribute).AttributeName, Is.EqualTo("isFinalState"));
        }
    }
}