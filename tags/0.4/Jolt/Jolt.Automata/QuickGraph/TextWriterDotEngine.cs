// ----------------------------------------------------------------------------
// TextWriterDotEngine.cs
//
// Contains the definition of the TextWriterDotEngine class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/16/2009 16:51:51
// ----------------------------------------------------------------------------

using System.IO;

using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace Jolt.Automata.QuickGraph
{
    /// <summary>
    /// Implements the IDotEngine interface and provides a GraphViz
    /// dot engine capable of writing data to a <see cref="System.IO.TextWriter"/>.
    /// </summary>
    internal sealed class TextWriterDotEngine : IDotEngine
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="TextWriterDotEngine"/> class,
        /// intializing the object with the given <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The writer that accepts the engine's GraphViz data.
        /// </param>
        internal TextWriterDotEngine(TextWriter writer)
        {
            m_writer = writer;
        }

        #endregion

        #region IDotEngine members ----------------------------------------------------------------

        /// <summary>
        /// Writes the given GraphViz data to the configured <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// 
        /// <seealso cref="IDotEngine.Run"/>
        string IDotEngine.Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            m_writer.Write(dot);
            return outputFileName;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly TextWriter m_writer;

        #endregion
    }
}