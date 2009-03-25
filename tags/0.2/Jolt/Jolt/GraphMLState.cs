// ----------------------------------------------------------------------------
// GraphMLState.cs
//
// Contains the definition of the GraphMLState class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/23/2009 23:20:44
// ----------------------------------------------------------------------------

using System.ComponentModel;
using System.Xml.Serialization;

namespace Jolt
{
    /// <summary>
    /// Represents the state object used by the FiniteStateMachine for
    /// [de]serialization to/from GraphML.
    /// </summary>
    internal sealed class GraphMLState
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Default constructor.  Intializes all attributes
        /// to their default values.
        /// </summary>
        internal GraphMLState() { }

        /// <summary>
        /// Initializes the attributes of the class.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name or ID of the state.
        /// </param>
        /// 
        /// <param name="isStartState">
        /// Determines if the state is the start state.
        /// </param>
        /// 
        /// <param name="isFinalState">
        /// Determines if the state is a final state.
        /// </param>
        internal GraphMLState(string name, bool isStartState, bool isFinalState)
        {
            Name = name;
            IsStartState = isStartState;
            IsFinalState = isFinalState;
        }

        #endregion 

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets/sets the state name.
        /// </summary>
        [XmlAttribute("stateName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the start-state qualifier.
        /// </summary>
        [XmlAttribute("isStartState"), DefaultValue(false)]
        public bool IsStartState { get; set; }

        /// <summary>
        /// Gets/sets the final-state qualifier.
        /// </summary>
        [XmlAttribute("isFinalState"), DefaultValue(false)]
        public bool IsFinalState { get; set; }

        #endregion
    }
}