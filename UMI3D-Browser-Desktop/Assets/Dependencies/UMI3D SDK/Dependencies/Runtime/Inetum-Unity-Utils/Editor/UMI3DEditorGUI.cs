/*
Copyright 2019 - 2023 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace inetum.unityUtils.editor
{
	public static class UMI3DEditorGUI
	{
		private delegate void PropertyFieldFunction(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren);

		public static void PropertyField(Rect rect, SerializedProperty property, bool includeChildren)
		{
			PropertyField_Implementation(rect, property, includeChildren, DrawPropertyField);
		}

		public static void PropertyField_Layout(SerializedProperty property, bool includeChildren)
		{
			Rect dummyRect = new Rect();
			PropertyField_Implementation(dummyRect, property, includeChildren, DrawPropertyField_Layout);
		}

		private static void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren)
		{
			EditorGUI.PropertyField(rect, property, label, includeChildren);
		}

		private static void DrawPropertyField_Layout(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren)
		{
			EditorGUILayout.PropertyField(property, label, includeChildren);
		}

		private static void PropertyField_Implementation(Rect rect, SerializedProperty property, bool includeChildren, PropertyFieldFunction propertyFieldFunction)
		{
			UMI3DSpecialAttribute specialCaseAttribute = PropertyUtility.GetAttribute<UMI3DSpecialAttribute>(property);
			if (specialCaseAttribute != null)
			{
				specialCaseAttribute.GetDrawer()?.OnGUI(rect, property);
			}
			else
			{
				EditorGUI.BeginChangeCheck();

				propertyFieldFunction.Invoke(rect, property, new GUIContent(property.displayName), includeChildren);

				if (EditorGUI.EndChangeCheck())
				{
					// TODO : value changed callback
				}
			}
			
		}
    }
}
#endif