// ----------------------------------------------------------------------------
// FsmConverter.cs
//
// Contains the definition of the FsmConverter class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/26/2009 23:00:29
// ----------------------------------------------------------------------------

using Microsoft.Glee.Drawing;

using QuickGraph.Glee;

namespace Jolt.Automata.Glee
{
    /// <summary>
    /// Converts a FiniteStateMachine class to a Microsoft GLEE representation.
    /// </summary>
    public static class FsmConverter
    {
        /// <summary>
        /// Converts the given finite state machine to a Microsoft GLEE representation.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The finite state machine to convert.
        /// </param>
        public static Graph ToGleeGraph<TAlphabet>(FiniteStateMachine<TAlphabet> fsm)
        {
            GleeGraphPopulator<string, Transition<TAlphabet>> populator = fsm.AsGraph.CreateGleePopulator();
            populator.NodeAdded += delegate(object sender, GleeVertexEventArgs<string> args)
            {
                args.Node.Attr.Label = args.Vertex;
                if (fsm.IsFinalState(args.Vertex))
                {
                    args.Node.Attr.Shape = Shape.DoubleCircle;
                }
                else
                {
                    args.Node.Attr.Shape = Shape.Circle;
                }

                if (fsm.StartState == args.Vertex)
                {
                    args.Node.Attr.AddStyle(Style.Bold);
                }
            };

            populator.EdgeAdded += delegate(object sender, GleeEdgeEventArgs<string, Transition<TAlphabet>> args)
            {
                args.GEdge.Attr.Label = args.Edge.Description;
            };

            populator.Compute();
            return populator.GleeGraph;
        }
    }
}