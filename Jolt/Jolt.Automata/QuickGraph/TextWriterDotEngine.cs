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
    /// dot engine capable of writing data to a TextWriter.
    /// </summary>
    internal sealed class TextWriterDotEngine : IDotEngine
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the dot engine with the given TextWriter.
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

        #region IDotEngine implementation ---------------------------------------------------------

        /// <summary>
        /// Writes the given GraphViz data to the configured TextWriter.
        /// <seealso cref="IDotEngine.Run"/>
        /// </summary>
        string IDotEngine.Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            m_writer.Write(dot);
            return outputFileName;
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly TextWriter m_writer;

        #endregion
    }
}