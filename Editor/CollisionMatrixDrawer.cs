using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dythervin.Collections;
using Dythervin.Core;
using Dythervin.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomPropertyDrawer(typeof(ICollisionMatrix))]
    public class CollisionMatrixDrawer : PropertyDrawer
    {
        private static class Labels
        {
            public static readonly GUIContent Reset = new("Reset");
        }

        private const float UpperMargin = 5f;
        private const float BottomMargin = 2f;
        private static readonly Vector2 Spacing = new(1f, 5f);

        protected float labelWidth;

        private SerializedProperty _thisProperty;
        private SerializedProperty _wrappedArrayProperty;
        private ICollisionMatrix _collisionMatrix;
        private IReadOnlyList<string> _registeredNames;

        protected virtual int ElementHeight => 20;

        private bool SelfIntersect => _collisionMatrix.SelfIntersect;

        private static float LineHeight => EditorGUIUtility.singleLineHeight;

        private int Count => _collisionMatrix?.CappedKeyCount ?? 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float value = base.GetPropertyHeight(property, label);

            if (property.isExpanded)
            {
                value += UpperMargin;
                value += (Count + 1) * (ElementHeight + Spacing.y) - Spacing.y;
                value += BottomMargin;
            }

            return value;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _thisProperty = property;
            _wrappedArrayProperty = _thisProperty.FindPropertyRelative("array");
            if (_wrappedArrayProperty == null)
            {
                return;
            }

            _collisionMatrix = (ICollisionMatrix)_thisProperty.GetValue();
            if (_collisionMatrix == null)
            {
                return;
            }

            CollisionMatrixHelper.TryGetNames(_collisionMatrix.GetType(), out _registeredNames);

            position = EditorGUI.IndentedRect(position);

            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new(position) { height = LineHeight };

            labelWidth = 0;
            for (int i = 0; i < Count; i++)
            {
                labelWidth = Mathf.Max(EditorStyles.label.CalcSize(new GUIContent(GetName(i))).x + 15, labelWidth);
            }

            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, label, menuAction: HeaderContextMenu);
            EditorGUI.EndFoldoutHeaderGroup();

            // Go to next line
            position.y += LineHeight;

            if (property.isExpanded)
            {
                position.y += UpperMargin;
                OnGUIMatrix(position);
            }

            EditorGUI.EndProperty();
        }


        private string GetName(int i)
        {
            return _registeredNames != null && _registeredNames.Count > i ? _registeredNames[i] : i.ToString();
        }

        private void HeaderContextMenu(Rect position)
        {
            GenericMenu menu = new();
            menu.AddItem(Labels.Reset, false, ResetValues);
            menu.AddSeparator("");
            menu.DropDown(position);
        }

        private void ResetValues()
        {
            int count = CollisionMatrixHelper.Triangulate(_collisionMatrix.CappedKeyCount);
            _wrappedArrayProperty.ClearArray();

            for (int i = 0; i < count; i++)
            {
                _wrappedArrayProperty.InsertArrayElementAtIndex(i);
                SerializedProperty element = _wrappedArrayProperty.GetArrayElementAtIndex(i);
                element.SetDefaultAutoValue();
            }

            _thisProperty.serializedObject.ApplyModifiedProperties();
        }

        private void OnGUIMatrix(in Rect initPosition)
        {
            Rect defaultLabelRect = new()
            {
                width = labelWidth,
                height = ElementHeight
            };

            Rect position = initPosition;
            float totalElementsWidth = initPosition.width - defaultLabelRect.width - Spacing.x;
            Rect elementRect = new()
            {
                width = (totalElementsWidth - Spacing.x * (Count - 1)) / Count,
                height = ElementHeight
            };

            {
                //Display column names
                int arrayLength = _collisionMatrix.GetRowLength(0);

                for (int i = 0; i < arrayLength; i++)
                {
                    Rect labelRect = new(defaultLabelRect)
                    {
                        x = initPosition.x + defaultLabelRect.width + Spacing.x + (elementRect.width + Spacing.x) * i,
                        y = initPosition.y,
                        width = elementRect.width
                    };

                    int nameIndex = Count - i - 1;
                    if (!SelfIntersect)
                    {
                        nameIndex++;
                    }

                    EditorGUI.LabelField(labelRect, GetName(nameIndex));
                }

                position.y += ElementHeight + Spacing.y;
            }

            for (int arrayIndex = 0; arrayIndex < Count; arrayIndex++)
            {
                int arrayLength = _collisionMatrix.GetRowLength(arrayIndex);
                Vector2 localPosition = new(position.x, position.y + (ElementHeight + Spacing.y) * arrayIndex);
                Rect labelRect = new(defaultLabelRect) { position = localPosition };
                localPosition.x += labelRect.width + Spacing.x;

                EditorGUI.LabelField(labelRect, GetName(arrayIndex));
                for (int x = 0; x < arrayLength; x++)
                {
                    Rect currentElementRect = new(elementRect)
                    {
                        x = localPosition.x + (elementRect.width + Spacing.x) * x,
                        y = localPosition.y
                    };
                    int i = GetKeyFromIndex(arrayIndex, arrayLength - x - 1);
                    SerializedProperty property = _wrappedArrayProperty.GetArrayElementAtIndex(i);

                    if (property.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        Match match = Regex.Match(property.type, @"PPtr<\$(.+)>");
                        EditorGUI.ObjectField(currentElementRect, property,
                            Type.GetType($"{nameof(UnityEngine)}.{match.Groups[1]}, {nameof(UnityEngine)}"), GUIContent.none);
                    }
                    else
                    {
                        EditorGUI.PropertyField(currentElementRect, property, GUIContent.none);
                    }
                }
            }
        }

        private int GetKeyFromIndex(int arrayIndex, int elementIndex)
        {
            elementIndex += arrayIndex;
            if (!SelfIntersect)
            {
                elementIndex++;
            }

            return _collisionMatrix.GetKeyIndex(arrayIndex, elementIndex);
        }
    }
}