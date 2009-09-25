// ----------------------------------------------------------------------------
// Convert.cs
//
// Contains the definition of the Convert class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/6/2009 6:03:32 PM
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Jolt
{
    /// <summary>
    /// Provides methods to convert one type to another.
    /// </summary>
    public static class Convert
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the class' static state.
        /// </summary>
        static Convert()
        {
            EmptyParameterList = new ParameterInfo[0];

            XDCMemberPrefixes = new Dictionary<Type, string>();
            XDCMemberPrefixes.Add(typeof(Type), "T:");
            XDCMemberPrefixes.Add(typeof(EventInfo), "E:");
            XDCMemberPrefixes.Add(typeof(PropertyInfo), "P:");
            XDCMemberPrefixes.Add(typeof(FieldInfo), "F:");

            string methodPrefix = "M:";
            XDCMemberPrefixes.Add(typeof(MethodInfo), methodPrefix);
            XDCMemberPrefixes.Add(typeof(ConstructorInfo), methodPrefix);
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Creates a string representing a given type in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(Type type)
        {
            StringBuilder builder = new StringBuilder();
            return AppendXDCFullTypeNameTo(builder, type)
                .Insert(0, XDCMemberPrefixes[typeof(Type)])
                .ToString();
        }

        /// <summary>
        /// Creates a string representing a given event in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The event from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(EventInfo eventInfo)
        {
            return ToXmlDocCommentMember(eventInfo, EmptyParameterList).ToString();
        }

        /// <summary>
        /// Creates a string representing a given field in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="field">
        /// The field from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(FieldInfo field)
        {
            return ToXmlDocCommentMember(field, EmptyParameterList).ToString();
        }

        /// <summary>
        /// Creates a string representing a given property in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(PropertyInfo property)
        {
            return ToXmlDocCommentMember(property, property.GetIndexParameters()).ToString();
        }

        /// <summary>
        /// Creates a string representing a given constructor in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(ConstructorInfo constructor)
        {
            return ToXmlDocCommentMember(constructor, constructor.GetParameters()).ToString();
        }

        /// <summary>
        /// Creates a string representing a given method in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method from which the string is created.
        /// </param>
        public static string ToXmlDocCommentMember(MethodInfo method)
        {
            return ToXmlDocCommentMember(method, method.GetParameters()).ToString();
        }

        /// <summary>
        /// Converts an array of ParameterInfo types to an array of
        /// types repesenting the type of each parameter.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameters to convert.
        /// </param>
        public static Type[] ToParameterTypes(ParameterInfo[] parameters)
        {
            return ToParameterTypes(parameters, Type.EmptyTypes, Type.EmptyTypes);
        }

        /// <summary>
        /// Converts an array of Type types to an array of
        /// strings repesenting the names of each parameter.
        /// </summary>
        /// 
        /// <param name="types">
        /// The types to convert.
        /// </param>
        public static string[] ToTypeNames(Type[] types)
        {
            return Array.ConvertAll(types, type => type.Name);
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Converts an array of ParameterInfo types to an array of
        /// types repesenting the type of each parameter.  Refers to
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
        /// types repesenting the type of each parameter.  Refers to
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
        /// Converts a given type to one that represents a defined type
        /// that participates in an external method signature.  Refers to
        /// the type from a generic type parameter collection when the
        /// given type is deemed to begeneric.
        /// </summary>
        /// 
        /// <param name="parameterType">
        /// The type to convert.
        /// </param>
        /// 
        /// <param name="genericTypeArguments">
        /// The generic arguments from the declaring type of the parameter's method.
        /// </param>
        /// 
        /// <param name="genericMethodArguments">
        /// The generic arguments from the parameter's method.
        /// </param>
        internal static Type ToMethodSignatureType(Type parameterType, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
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

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Converts a ParameterInfo to the type that repesents the
        /// type of the parameter.  Refers to the type from a generic
        /// type parameter collection when a parameter is deemed to be
        /// generic.
        /// </summary>
        /// 
        /// <param name="parameter">
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
        private static Type ToParameterType(ParameterInfo parameter, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return ToMethodSignatureType(parameter.ParameterType, genericTypeArguments, genericMethodArguments);
        }

        /// <summary>
        /// Creates a string representing a given type member in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="member">
        /// The member from which the string is created.
        /// </param>
        /// 
        /// <param name="memberParameters">
        /// The parameters to the member, if any.
        /// </param>
        private static StringBuilder ToXmlDocCommentMember<TMember>(TMember member, ParameterInfo[] memberParameters)
            where TMember : MemberInfo
        {
            int dummy;
            return ToXmlDocCommentMember(member, memberParameters, out dummy);
        }

        /// <summary>
        /// Creates a string representing a given type member in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="member">
        /// The member from which the string is created.
        /// </param>
        /// 
        /// <param name="memberParameters">
        /// The parameters to the member, if any.
        /// </param>
        /// 
        /// <param name="namePosition">
        /// The position in the resulting string that indexes that starting
        /// position of the member name.
        /// </param>
        private static StringBuilder ToXmlDocCommentMember<TMember>(TMember member, ParameterInfo[] memberParameters, out int namePosition)
            where TMember : MemberInfo
        {
            StringBuilder builder = new StringBuilder();
            AppendXDCFullTypeNameTo(builder, member.DeclaringType)
                .Insert(0, XDCMemberPrefixes[typeof(TMember)])
                .Append('.');

            namePosition = builder.Length;
            builder.Append(member.Name);

            if (memberParameters.Length > 0)
            {
                builder.Append('(');
                AppendXDCParameterTypesTo(builder, ToParameterTypes(memberParameters))
                    .Append(')');
            }

            return builder;
        }

        /// <summary>
        /// Creates a string representing a given ConstructorInfo in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor from which the string is created.
        /// </param>
        /// 
        /// <param name="memberParameters">
        /// The parameters for the constructor, if any.
        /// </param>
        private static StringBuilder ToXmlDocCommentMember(ConstructorInfo constructor, ParameterInfo[] constructorParameters)
        {
            int namePosition;
            StringBuilder builder = ToXmlDocCommentMember<ConstructorInfo>(constructor, constructorParameters, out namePosition);
            builder[namePosition] = '#';   // Replaces . with # in ctor name.

            return builder;
        }

        /// <summary>
        /// Creates a string representing a given MethodInfo in an
        /// XML doc comment member element.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor from which the string is created.
        /// </param>
        /// 
        /// <param name="memberParameters">
        /// The parameters for the constructor, if any.
        /// </param>
        private static StringBuilder ToXmlDocCommentMember(MethodInfo method, ParameterInfo[] methodParameters)
        {
            StringBuilder builder = ToXmlDocCommentMember<MethodInfo>(method, methodParameters);

            if (Array.BinarySearch(ConversionOperatorNames, method.Name) >= 0)
            {
                builder.Append('~');
                AppendXDCParameterTypesTo(builder, new[] { method.ReturnType });
            }

            return builder;
        }

        /// <summary>
        /// Creates the XML doc comment representation of a type name
        /// including a quantifier symbol for generic arguments.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The StringBuilder to which the type name is appended.
        /// </param>
        /// 
        /// <param name="type">
        /// The type whose name is created.
        /// </param>
        private static StringBuilder AppendXDCFullTypeNameTo(StringBuilder builder, Type type)
        {
            if (type.IsGenericType) { type = type.GetGenericTypeDefinition(); }
            return AppendNormalizedXDCTypeNameTo(builder, type);
        }

        /// <summary>
        /// Creates the XML doc comment representation of a type name,
        /// excluding a quantifier symbol for generic arguments.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The StringBuilder to which the type name is appended.
        /// </param>
        /// 
        /// <param name="type">
        /// The type whose name is created.
        /// </param>
        private static StringBuilder AppendXDCTypeNameTo(StringBuilder builder, Type type)
        {
            StringBuilder typeNameBuilder = new StringBuilder();
            AppendNormalizedXDCTypeNameTo(typeNameBuilder, type);

            if (type.IsGenericType)
            {
                typeNameBuilder.Length = type.FullName.IndexOf('`');
            }

            return builder.Append(typeNameBuilder.ToString());
        }

        /// <summary>
        /// Creates a string representation of the given type's
        /// full name, replacing all nested type qualifier symbols
        /// (+) with a scope resolution symbol (.).
        /// </summary>
        /// 
        /// <param name="builder">
        /// The StringBuilder to which the type name is appended.
        /// </param>
        /// 
        /// <param name="type">
        /// The type whose name is normalized.
        /// </param>
        private static StringBuilder AppendNormalizedXDCTypeNameTo(StringBuilder builder, Type type)
        {
            return type.IsNested ?
                builder.Append(type.FullName.Replace('+', '.')) :
                builder.Append(type.FullName);
        }

        /// <summary>
        /// Appends the XML doc comment representation of a list of
        /// given parameters to the given string builder.
        /// </summary>
        /// 
        /// <param name="builder">
        /// The builder to append to.
        /// </param>
        /// 
        /// <param name="parameters">
        /// The parameter types to convert and append.
        /// </param>
        private static StringBuilder AppendXDCParameterTypesTo(StringBuilder builder, Type[] parameterTypes)
        {
            for (int i = 0; i < parameterTypes.Length; ++i)
            {
                string parameterModifier = ReduceToElementType(ref parameterTypes[i]);
                if (parameterTypes[i].IsGenericType)
                {
                    AppendXDCTypeNameTo(builder, parameterTypes[i].GetGenericTypeDefinition()).Append('{');
                    AppendXDCParameterTypesTo(builder, parameterTypes[i].GetGenericArguments()).Append('}');
                }
                else if (parameterTypes[i].IsGenericParameter)
                {
                    builder.Append('`');
                    if (parameterTypes[i].DeclaringMethod != null)
                    {
                        builder.Append('`');
                    }

                    builder.Append(parameterTypes[i].GenericParameterPosition);
                }
                else
                {
                    AppendXDCTypeNameTo(builder, parameterTypes[i]);
                }

                builder.Append(parameterModifier).Append(',');
            }

            builder.Length -= 1;
            return builder;
        }

        /// <summary>
        /// Creates a string builder containing XML doc comment
        /// representation of the given type's modifier (i.e. array
        /// type, pointer, etc...), and reduces the given type to
        /// its element type, if any modifiers are detected.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type to reduce.
        /// </param>
        /// 
        /// <remarks>
        /// The element type is the type of the given type, excluding
        /// any array, pointer or by-ref modifiers.
        /// </remarks>
        private static string ReduceToElementType(ref Type type)
        {
            StringBuilder builder = new StringBuilder();
            while (type.IsByRef)
            {
                // ELEMENT_TYPE_BYREF
                builder.Append('@');
                type = type.GetElementType();
            }
            
            while (type.IsArray)
            {
                int rank = type.GetArrayRank();
                if (rank == 1)
                {
                    // ELEMENT_TYPE_SZARRAY
                    builder.Insert(0, SZArrayTypeSuffix);
                }
                else
                {
                    // ELEMENT_TYPE_ARRAY
                    builder.Insert(0, ']')
                           .Insert(0, ArrayElementTypeDimension)
                           .Insert(0, ArrayElementTypeDimension_Delimited, rank - 1)
                           .Insert(0, '[');
                }

                type = type.GetElementType();
            }

            while (type.IsPointer)
            {
                // ELEMENT_TYPE_PTR
                builder.Insert(0, '*');
                type = type.GetElementType();
            }

            return builder.ToString();
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly string SZArrayTypeSuffix = "[]";
        private static readonly string ArrayElementTypeDimension = "0:";
        private static readonly string ArrayElementTypeDimension_Delimited = ArrayElementTypeDimension + ',';
        private static readonly string[] ConversionOperatorNames = { "op_Explicit", "op_Implicit" };    // Keep this lexicographically sorted.

        private static readonly IDictionary<Type, string> XDCMemberPrefixes;
        private static readonly ParameterInfo[] EmptyParameterList;

        #endregion
    }
}