// ----------------------------------------------------------------------------
// FsmConverter.cs
//
// Contains the definition of the FsmConverter class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/28/2009 10:04:59
// ----------------------------------------------------------------------------

using Microsoft.Msagl.Drawing;

using QuickGraph.Msagl;

namespace Jolt.Automata.Msagl
{
    /// <summary>
    /// Converts a FiniteStateMachine class to a Microsoft AGL representation.
    /// </summary>
    public static class FsmConverter
    {
        /// <summary>
        /// Converts the given finite state machine to a Microsoft AGL representation.
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
        public static Graph ToMsaglGraph<TAlphabet>(FiniteStateMachine<TAlphabet> fsm)
        {
            MsaglGraphPopulator<string, Transition<TAlphabet>> populator = fsm.AsGraph.CreateMsaglPopulator();
            populator.NodeAdded += delegate(object sender, MsaglVertexEventArgs<string> args)
            {
                args.Node.Label.Text = args.Vertex;
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

            populator.EdgeAdded += delegate(object sender, MsaglEdgeEventArgs<string, Transition<TAlphabet>> args)
            {
                args.GEdge.Label.Text = args.Edge.Description;
            };

            populator.Compute();
            return populator.MsaglGraph;
        }
    }
}