// ----------------------------------------------------------------------------
// GraphMLTransitionTestFixture.cs
//
// Contains the definition of the GraphMLTransitionTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/24/2009 11:46:09 AM
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

using Jolt.Automata.QuickGraph;
using Jolt.Functional;
using log4net.Config;
using NUnit.Framework;

namespace Jolt.Automata.Test.QuickGraph
{
    [TestFixture]
    public sealed class GraphMLTransitionTestFixture
    {
        #region public methods --------------------------------------------------------------------

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            BasicConfigurator.Configure();
        }


        /// <summary>
        /// Verifies the construction of the class using
        /// only the required construction arguments.
        /// </summary>
        [Test]
        public void Construction()
        {
            GraphMLState source = new GraphMLState();
            GraphMLState target = new GraphMLState();

            GraphMLTransition<char> graphMLTransition = new GraphMLTransition<char>(source, target);

            Assert.That(graphMLTransition.Source, Is.SameAs(source));
            Assert.That(graphMLTransition.Target, Is.SameAs(target));
            Assert.That(graphMLTransition.Description, Is.Null);
            Assert.That(graphMLTransition.TransitionPredicate, Is.Null);
        }

        /// <summary>
        /// Verifies the construction of the class using
        /// the optional and required construction arguments.
        /// </summary>
        [Test]
        public void Construction_OptionalArgs()
        {
            GraphMLState source = new GraphMLState();
            GraphMLState target = new GraphMLState();
            Predicate<char> predicate = Char.IsDigit;
            string description = "description";

            GraphMLTransition<char> graphMLTransition = new GraphMLTransition<char>(source, target, description, predicate);

            Assert.That(graphMLTransition.Source, Is.SameAs(source));
            Assert.That(graphMLTransition.Target, Is.SameAs(target));
            Assert.That(graphMLTransition.Description, Is.SameAs(description));
            Assert.That(graphMLTransition.TransitionPredicate, Is.EqualTo("IsDigit;" + typeof(char).AssemblyQualifiedName));
        }

        /// <summary>
        /// Verifies the construction of the class when the transition
        /// predicate references an instance method.
        /// </summary>
        [Test]
        public void Construction_InvalidPredicate()
        {
            GraphMLState source = new GraphMLState();
            GraphMLState target = new GraphMLState();
            Predicate<char> predicate = new FsmEnumerator<char>(null, null).NextState;
            string description = "description";

            GraphMLTransition<char> graphMLTransition = new GraphMLTransition<char>(source, target, description, predicate);

            Assert.That(graphMLTransition.Source, Is.SameAs(source));
            Assert.That(graphMLTransition.Target, Is.SameAs(target));
            Assert.That(graphMLTransition.Description, Is.SameAs(description));
            Assert.That(graphMLTransition.TransitionPredicate, Is.Null);
        }

        /// <summary>
        /// Verifies the behavior of the Description property.
        /// </summary>
        [Test]
        public void Description()
        {
            GraphMLTransition<char> transition = new GraphMLTransition<char>(null, null, "description", Char.IsDigit);

            string initialValue = transition.Description;
            string description = "new-description";
            transition.Description = description;

            Assert.That(transition.Description, Is.Not.EqualTo(initialValue));
            Assert.That(transition.Description, Is.SameAs(description));
        }

        /// <summary>
        /// Verifies the serializability of the Description property.
        /// </summary>
        [Test]
        public void Description_Serializable()
        {
            PropertyInfo property = typeof(GraphMLTransition<>).GetProperty("Description");

            Assert.That(
                property,
                Has.Attribute<XmlAttributeAttribute>().With.Property("AttributeName").EqualTo("description"));
        }

        /// <summary>
        /// Verifies the behavior of the TransitionPredicate property.
        /// </summary>
        [Test]
        public void TransitionPredicate()
        {
            GraphMLTransition<char> transition = new GraphMLTransition<char>(null, null, null, Char.IsDigit);

            string initialValue = transition.TransitionPredicate;
            string predicate = "new-predicate";
            transition.TransitionPredicate = predicate;

            Assert.That(transition.TransitionPredicate, Is.Not.EqualTo(initialValue));
            Assert.That(transition.TransitionPredicate, Is.SameAs(predicate));
        }

        /// <summary>
        /// Verifies the serializability of the TransitionPredicate property.
        /// </summary>
        [Test]
        public void TransitionPredicate_Serializable()
        {
            PropertyInfo property = typeof(GraphMLTransition<>).GetProperty("TransitionPredicate");

            Assert.That(
                property,
                Has.Attribute<XmlAttributeAttribute>().With.Property("AttributeName").EqualTo("transitionPredicate"));
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method.
        /// </summary>
        [Test]
        public void ToTransition()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true),
                "description",
                Char.IsDigit);

            AssertToTransition(graphMLtransition, Char.IsDigit);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate is generic.
        /// </summary>
        [Test]
        public void ToTransition_GenericPredicate()
        {
            Predicate<char> predicate = Functor.ToPredicate(Functor.TrueForAll<char>());
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true),
                "description",
                predicate);

            AssertToTransition(graphMLtransition, predicate);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate name is null.
        /// </summary>
        [Test]
        public void ToTransition_NullTransitionPredicateName()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";

            AssertToTransition(graphMLtransition);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the type referenced by the transition predicate
        /// is invalid.
        /// </summary>
        [Test]
        public void ToTransition_InvalidType()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";
            graphMLtransition.TransitionPredicate = "IsDigit;invalid-type-name";

            AssertToTransition(graphMLtransition);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate function name is unloadable.
        /// </summary>
        [Test]
        public void ToTransition_InvalidPredicate_Name()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";
            graphMLtransition.TransitionPredicate = "invalid-name;" + typeof(char).AssemblyQualifiedName;

            AssertToTransition(graphMLtransition);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate has more than one paramter.
        /// </summary>
        [Test]
        public void ToTransition_InvalidPredicate_NumParams()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";
            graphMLtransition.TransitionPredicate = "TryParse;" + typeof(char).AssemblyQualifiedName;

            AssertToTransition(graphMLtransition);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate function has a parameter
        /// with an invalid type.
        /// </summary>
        [Test]
        public void ToTransition_InvalidPredicate_ParameterType()
        {
            GraphMLTransition<int> graphMLtransition = new GraphMLTransition<int>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";
            graphMLtransition.TransitionPredicate = "IsDigit;" + typeof(char).AssemblyQualifiedName;

            AssertToTransition(graphMLtransition);
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method
        /// when the transition predicate function has an invalid
        /// return type.
        /// </summary>
        [Test]
        public void ToTransition_InvalidPredicate_ReturnType()
        {
            GraphMLTransition<char> graphMLtransition = new GraphMLTransition<char>(
                new GraphMLState("start", true, false),
                new GraphMLState("finish", false, true));

            graphMLtransition.Description = "description";
            graphMLtransition.TransitionPredicate = "GetNumericValue;" + typeof(char).AssemblyQualifiedName;

            AssertToTransition(graphMLtransition);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the ToTransition() method,
        /// for the case in which the method returns a valid conversion.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="graphMLTransition">
        /// The transition for which ToTransition will be called.
        /// </param>
        /// 
        /// <param name="expectedPredicate">
        /// The expected predicate created during the conversion.
        /// </param>
        private static void AssertToTransition<TAlphabet>(GraphMLTransition<TAlphabet> graphMLTransition, Predicate<TAlphabet> expectedPredicate)
        {
            Transition<TAlphabet> transition = graphMLTransition.ToTransition();

            Assert.That(transition.Source, Is.SameAs(graphMLTransition.Source.Name));
            Assert.That(transition.Target, Is.SameAs(graphMLTransition.Target.Name));
            Assert.That(transition.Description, Is.SameAs(graphMLTransition.Description));
            Assert.That(transition.TransitionPredicate, Is.EqualTo(expectedPredicate));
        }

        /// <summary>
        /// Verifies the behavior of the ToTransition() method,
        /// for the case in which the method returns an invalid conversion.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="graphMLTransition">
        /// The transition for which ToTransition will be called.
        /// </param>
        private static void AssertToTransition<TAlphabet>(GraphMLTransition<TAlphabet> graphMLTransition)
        {
            Predicate<TAlphabet> predicate = Delegate.CreateDelegate(
                typeof(Predicate<TAlphabet>),
                typeof(Functor).GetMethod("<FalseForAll>b__26", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TAlphabet)))
                as Predicate<TAlphabet>;

            AssertToTransition(graphMLTransition, predicate);
        }

        #endregion
    }
}