// ----------------------------------------------------------------------------
// Convert.cs
//
// Contains the definition of the Convert class.
// Copyright 2007 Steve Guidi.
//
// File created: 7/12/2007 21:19:48
// ----------------------------------------------------------------------------

using System;
using System.Reflection;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides methods to convert one type to another.
    /// </summary>
    internal static class Convert
    {
        /// <summary>
        /// Converts an array of ParameterInfo types to an array of
        /// types repesenting the type of each paramater.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameters to convert.
        /// </param>
        internal static Type[] ToParameterTypes(ParameterInfo[] parameters)
        {
            return Array.ConvertAll<ParameterInfo, Type>(parameters,
                delegate(ParameterInfo methodParam) { return methodParam.ParameterType; });
        }
    }
}
