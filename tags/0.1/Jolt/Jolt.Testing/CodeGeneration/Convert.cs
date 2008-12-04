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
            return ToParameterTypes(parameters, Type.EmptyTypes, Type.EmptyTypes);
        }

        /// <summary>
        /// Converts an array of ParameterInfo types to an array of
        /// types repesenting the type of each paramater.  Refers to
        /// the type from a generic type argument collection when a
        /// parameter is deemed to be generic.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameters to convert.
        /// </param>
        /// 
        /// <param name="genericTypeArguments">
        /// The generic arguments from the declaring type of the parameter's method.
        /// </param>
        internal static Type[] ToParameterTypes(ParameterInfo[] parameters, Type[] genericTypeArguments)
        {
            return ToParameterTypes(parameters, genericTypeArguments, Type.EmptyTypes);
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
        /// <param name="genericTypeArguments">
        /// The generic arguments from the declaring type of the parameter's method.
        /// </param>
        /// 
        /// <param name="genericMethodArguments">
        /// The generic arguments from the parameter's method.
        /// </param>
        internal static Type[] ToParameterTypes(ParameterInfo[] parameters, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return Array.ConvertAll(parameters, methodParam => ToParameterType(methodParam, genericTypeArguments, genericMethodArguments));
        }

        /// <summary>
        /// Converts a ParameterInfo to the type that repesents the
        /// type of the paramater.  Refers to the type from a generic
        /// type parameter collection when a parameter is deemed to be
        /// generic.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameter to convert.
        /// </param>
        /// 
        /// <param name="genericTypeArguments">
        /// The generic arguments from the declaring type of the parameter's method.
        /// </param>
        /// 
        /// <param name="genericMethodArguments">
        /// The generic arguments from the parameter's method.
        /// </param>
        internal static Type ToParameterType(ParameterInfo parameter, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            Type parameterType = parameter.ParameterType;
            if (parameterType.IsGenericParameter)
            {
                if (parameterType.DeclaringMethod != null && genericMethodArguments.Length > 0)
                {
                    return genericMethodArguments[parameterType.GenericParameterPosition];
                }

                if (genericTypeArguments.Length > 0)
                {
                    return genericTypeArguments[parameterType.GenericParameterPosition];
                }
            }

            return parameterType;
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
            return Array.ConvertAll(types, type => type.Name);
        }
    }
}
