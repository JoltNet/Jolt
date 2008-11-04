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
            return Array.ConvertAll<ParameterInfo, Type>(parameters, delegate(ParameterInfo methodParam)
            {
                return methodParam.ParameterType;
            });
        }

        /// <summary>
        /// Converts an array of ParameterInfo types to an array of
        /// types repesenting the type of each paramater.  Refers to
        /// the type from a generic type parameter collection when a
        /// parameter is deemed to be generic.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameters to convert.
        /// </param>
        /// 
        /// <param name="genericTypeParameters">
        /// The generic parameters of the method's declaring type.
        /// </param>
        internal static Type[] ToParameterTypes(ParameterInfo[] parameters, Type[] genericTypeParameters)
        {
            return Array.ConvertAll<ParameterInfo, Type>(parameters, delegate(ParameterInfo methodParam)
            {
                Type parameterType = methodParam.ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    // alternatively: if parameterType.DeclaringMethod == null
                    return genericTypeParameters[parameterType.GenericParameterPosition];
                }
                
                return parameterType;
            });
        }

        /// <summary>
        /// Converts an array of Type types to an array of
        /// strings repesenting the names of each paramater.
        /// </summary>
        /// 
        /// <param name="types">
        /// The types to convert.
        /// </param>
        internal static string[] ToTypeNames(Type[] types)
        {
            return Array.ConvertAll<Type, string>(types,
                delegate(Type type) { return type.Name; });
        }
    }
}
