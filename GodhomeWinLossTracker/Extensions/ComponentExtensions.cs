﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using UnityEngine;

//namespace GodhomeWinLossTracker.Extensions
//{
//    /// <summary>
//    /// Extensions for working with <see cref="Component"/> objects.
//    /// </summary>
//    public static class ComponentExtensions
//    {
//        private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

//        // Somewhat worried about this messing with some internal component state that shouldn't be messed with.
//        /// <summary>
//        /// Copies all member values from the source component to this component.
//        /// </summary>
//        /// <typeparam name="T">The type of the components.</typeparam>
//        /// <param name="component">The component to copy to.</param>
//        /// <param name="source">The source component to copy values from.</param>
//        /// <param name="replaceSelfGORefs">
//        /// If <see langword="true"/>, replace references the source component has to its parent <see cref="GameObject"/>
//        /// with references to this component's <see cref="GameObject"/>.
//        /// </param>
//        /// <param name="reflFlags">The flags that specify what types of members are replaced.</param>
//        /// <returns>This component with its values set to the values from the source component.</returns>
//        internal static T CopyFrom<T>(this T component, T source, bool replaceSelfGORefs = true, BindingFlags reflFlags = AllInstance) where T : Component
//        {
//            if (!component)
//                throw new ArgumentNullException(nameof(component));

//            if (!source)
//                throw new ArgumentNullException(nameof(source));

//            Type compType = typeof(T);
//            GameObject sourceGO = source.gameObject;
//            GameObject selfGO = component.gameObject;

//            var componentMembers = compType.GetMembers(reflFlags).Where(member => member is FieldInfo or PropertyInfo);
//            foreach (var member in componentMembers)
//            {
//                bool canReplaceGO = replaceSelfGORefs && member.GetUnderlyingType() == typeof(GameObject);

//                if (member is PropertyInfo property && property.CanWrite)
//                {
//                    try
//                    {
//                        var sourceValue = property.GetValue(source, null);
//                        var finalValue = (canReplaceGO && ReferenceEquals(sourceValue, sourceGO)) ? selfGO : sourceValue;

//                        property.SetValue(component, finalValue, null);
//                    }
//                    catch 
//                    {
//                        TB.Instance.LogWarn($"Couldn't copy component value for property '{property.Name}'.");
//                    }
//                }
//                else if (member is FieldInfo field)
//                {
//                    var sourceValue = field.GetValue(source);
//                    var finalValue = (canReplaceGO && ReferenceEquals(sourceValue, sourceGO)) ? selfGO : sourceValue;

//                    field.SetValue(component, finalValue);
//                }
//            }

//            return component;
//        }
//    }
//}
