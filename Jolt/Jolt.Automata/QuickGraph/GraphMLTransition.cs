// ----------------------------------------------------------------------------
// GraphMLTransition.cs
//
// Contains the definition of the GraphMLTransition class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/23/2009 23:35:48
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Xml.Serialization;

using Jolt.Automata.Properties;
using Jolt.Functional;
using Jolt.Reflection;
using log4net;
using QuickGraph;

namespace Jolt.Automata.QuickGraph
{
    /// <summary>
    /// Implements a type to enable the [de]serialization of a <see cref="Transition"/>
    /// object to and from GraphML.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the
    /// transition object.
    /// </typeparam>
    internal sealed class GraphMLTransition<TAlphabet> : Edge<GraphMLState>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="GraphMLTransition"/> class,
        /// initializing the source and target states of the new object.
        /// </summary>
        /// 
        /// <param name="source">
        /// The source state.
        /// </param>
        /// 
        /// <param name="target">
        /// The target state.
        /// </param>
        /// 
        /// <remaks>
        /// Does not initialize other attributes of the new object.
        /// </remaks>
        internal GraphMLTransition(GraphMLState source, GraphMLState target)
            : base(source, target) { }

        /// <summary>
        /// Creates a new instance of the <see cref="GraphMLTransition"/> class,
        /// initializing its attributes to the given values..
        /// </summary>
        /// 
        /// <param name="source">
        /// The source state.
        /// </param>
        /// 
        /// <param name="target">
        /// The target state.
        /// </param>
        /// 
        /// <param name="description">
        /// The transition's description.
        /// </param>
        /// 
        /// <param name="predicate">
        /// The transition's state-transition predicate.
        /// </param>
        internal GraphMLTransition(GraphMLState source, GraphMLState target, string description, Predicate<TAlphabet> predicate)
            : this(source, target)
        {
            Description = description;
            if (predicate.Target == null)
            {
                TransitionPredicate = String.Concat(predicate.Method.Name, ';', predicate.Method.DeclaringType.AssemblyQualifiedName);
            }
            else
            {
                // Instance-predicates are invalid, and are discarded.
                Log.WarnFormat(Resources.Warn_TransitionSerialization_DiscardInstancePredicate, predicate.Method.Name, predicate.Method.DeclaringType.Name);
            }
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets/sets the transition description.
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the transition predicate.
        /// </summary>
        [XmlAttribute("transitionPredicate")]
        public string TransitionPredicate { get; set; }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Converts the intenral state of this object to a a new <see cref="Transition"/> object.
        /// </summary>
        /// 
        /// <returns>
        /// A newly created <see cref="Transition"/> object, corresponding to this object.
        /// </returns>
        internal Transition<TAlphabet> ToTransition()
        {
            return new Transition<TAlphabet>(Source.Name, Target.Name, DeserializeMethod(TransitionPredicate), Description);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Deserializes a given string into a transition <see cref="System.Predicate"/>.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="method">
        /// The serialized method name (methodName;assemblyQualifiedTypeName).
        /// </param>
        /// 
        /// <returns>
        /// Creates a new predicate if the string is deserialized to an existing method,
        /// or returns a default predicate otherwise.
        /// </returns>
        /// 
        /// <remarks>
        /// The default predicate returns false for all inputs.
        /// </remarks>
        private Predicate<TAlphabet> DeserializeMethod(string method)
        {
            if (method != null)
            {
                int delimiterPos = method.IndexOf(';');
                string methodName = method.Substring(0, delimiterPos);
                Type declaringType = Type.GetType(method.Substring(delimiterPos + 1));

                if (declaringType != null)
                {
                    foreach (MethodInfo predicate in declaringType.GetMethods(CompoundBindingFlags.AnyStatic))
                    {
                        if (predicate.Name == methodName &&
                            predicate.GetParameters().Length == 1 &&
                            predicate.ReturnType == typeof(bool))
                        {
                            Type paramType = predicate.GetParameters()[0].ParameterType;
                            if (paramType == typeof(TAlphabet))
                            {
                                return Delegate.CreateDelegate(typeof(Predicate<TAlphabet>), predicate) as Predicate<TAlphabet>;
                            }
                            else if (paramType.IsGenericParameter)
                            {
                                return Delegate.CreateDelegate(typeof(Predicate<TAlphabet>), predicate.MakeGenericMethod(typeof(TAlphabet))) as Predicate<TAlphabet>;
                            }
                        }
                    }
                }

                // Predicate is invalid or could not be loaded.
                Log.WarnFormat(Resources.Warn_TransitionDeserialization_InvalidPredicate, methodName, Source.Name, Target.Name);
            }
            else
            {
                // Predicate was not specified.
                Log.WarnFormat(Resources.Warn_TransitionDeserialization_PredicateNotSpecified, Source.Name, Target.Name);
            }

            return Functor.ToPredicate(Functor.FalseForAll<TAlphabet>());
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly ILog Log = LogManager.GetLogger(typeof(GraphMLTransition<>));

        #endregion
    }
}