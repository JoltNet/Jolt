// ----------------------------------------------------------------------------
// GraphMLTransition.cs
//
// Contains the definition of the GraphMLTransition class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/23/2009 23:35:48
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

using QuickGraph;

namespace Jolt
{
    /// <summary>
    /// Represents the transition object used by the FiniteStateMachine for
    /// [de]serialization to/from GraphML.
    /// </summary>
    /// 
    /// <typeparam name="TAlphabet">
    /// The type that represents the alphabet operated upon by the
    /// finite state machine.
    /// </typeparam>
    internal sealed class GraphMLTransition<TAlphabet> : Edge<GraphMLState>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the source and target states
        /// of the class, leaving other attributes in their
        /// default state.
        /// </summary>
        /// 
        /// <param name="source">
        /// The source state.
        /// </param>
        /// 
        /// <param name="target">
        /// The target state.
        /// </param>
        internal GraphMLTransition(GraphMLState source, GraphMLState target)
            : base(source, target) { }

        /// <summary>
        /// Initializes the attributes of the class.
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
                TransitionPredicate = String.Concat(predicate.Method.Name, ';',predicate.Method.DeclaringType.AssemblyQualifiedName);
            }

            // TODO: Log discarding of predicate for instance methods.
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
        /// Creates a Transition object from the state of this object.
        /// </summary>
        internal Transition<TAlphabet> ToTransition()
        {
            return new Transition<TAlphabet>(Source.Name, Target.Name, DeserializeMethod(TransitionPredicate), Description);
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Converts a given string representing a serialized static method
        /// to a Predicate of the requested type.  Returns a default predicate
        /// when the given method name cannot be resolved.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="methodName">
        /// The serialized method name (methodName;assemblyQualifiedTypeName).
        /// </param>
        private static Predicate<TAlphabet> DeserializeMethod(string methodName)
        {
            // TODO: Log discarding of predicate for invalid methodName
            if (methodName != null)
            {
                int delimiterPos = methodName.IndexOf(';');
                Type declaringType = Type.GetType(methodName.Substring(delimiterPos + 1));
                if (declaringType != null)
                {
                    MethodInfo predicate = declaringType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .FirstOrDefault(m => m.Name == methodName.Substring(0, delimiterPos) &&
                             m.GetParameters().Length == 1 &&
                             m.GetParameters()[0].ParameterType == typeof(TAlphabet) &&
                             m.ReturnType == typeof(bool));

                    // TODO: Log discarding of predicate for instance methods
                    if (predicate != null) { return (Predicate<TAlphabet>)Delegate.CreateDelegate(typeof(Predicate<TAlphabet>), predicate); }
                }

                // TODO: Log discarding of predicate for invalid methodName
            }

            return inputSymbol => false;
        }

        #endregion
    }
}