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
        // TODO: merge this type with Jolt.Testing.Convert
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
            return ToXmlDocCommentMember<EventInfo>(eventInfo.DeclaringType, eventInfo.Name, EmptyParameterList);
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
            return ToXmlDocCommentMember<FieldInfo>(field.DeclaringType, field.Name, EmptyParameterList);
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
            return ToXmlDocCommentMember<PropertyInfo>(property.DeclaringType, property.Name, property.GetIndexParameters());
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
            return ToXmlDocCommentMember<ConstructorInfo>(constructor.DeclaringType, constructor.Name.Replace('.', '#'), constructor.GetParameters());
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
            return ToXmlDocCommentMember<MethodInfo>(method.DeclaringType, method.Name, method.GetParameters());
        }

        #endregion

        #region private methods -------------------------------------------------------------------

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
        private static string ToXmlDocCommentMember<TMember>(Type memberDeclaringType, string memberName, ParameterInfo[] memberParameters)
        {
            StringBuilder builder = new StringBuilder();
            AppendXDCFullTypeNameTo(builder, memberDeclaringType)
                .Insert(0, XDCMemberPrefixes[typeof(TMember)])
                .Append('.')
                .Append(memberName);

            if (memberParameters.Length > 0)
            {
                // TODO: Replace ConvertAll with method call in other Convert class.
                builder.Append('(');
                AppendXDCParameterTypesTo(builder, Array.ConvertAll(memberParameters, param => param.ParameterType))
                    .Append(')');
            }

            return builder.ToString();
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

        private static readonly IDictionary<Type, string> XDCMemberPrefixes;
        private static readonly ParameterInfo[] EmptyParameterList;

        #endregion
    }
}