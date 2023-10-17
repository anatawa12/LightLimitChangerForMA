using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace io.github.azukimochi
{
    [CustomEditor(typeof(LightLimitChangerPreset))]
    internal sealed class LightLimitChangerPresetEditor : Editor
    {
        private SerializedProperty _name;
        private SerializedProperty _enable;

        private SerializedProperty _parameters;


        private void OnEnable()
        {
            var obj = new SerializedObject((serializedObject.targetObject as Component).gameObject);
            _name = obj.FindProperty("m_Name");
            _enable = obj.FindProperty("m_IsActive");
            _parameters = serializedObject.FindProperty(nameof(LightLimitChangerPreset.Parameters));
        }

        public override void OnInspectorGUI()
        {
            var settings = (target as LightLimitChangerPreset).GetParent();
            if (settings == null)
            {
                // TODO: プリセットがLLCの配下にないので警告を出す
                return;
            }
            serializedObject.Update();
            _name.serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_name);
            EditorGUILayout.PropertyField(_enable);
            if (EditorGUI.EndChangeCheck())
            {
                _name.serializedObject.ApplyModifiedProperties();
            }

            int count = _parameters.arraySize;
            var targetControl = settings.Parameters.GetControlTypeFlags();
            for (int i = 0; i < count; i++)
            {
                var property = _parameters.GetArrayElementAtIndex(i);

                var type = (LightLimitControlType)property.FindPropertyRelative(nameof(LightLimitChangerPreset.Parameter.Type)).intValue;
                bool isLightingControl = LightLimitControlType.Light.HasFlag(type);

                bool disabled;
                if (type == LightLimitControlType.Light)
                    disabled = settings.Parameters.IsSeparateLightControl;
                else if (isLightingControl)
                    disabled = !settings.Parameters.IsSeparateLightControl;
                else
                    disabled = !targetControl.HasFlag(type);


                EditorGUI.BeginDisabledGroup(disabled);
                DrawProperty(property, 0, isLightingControl ? 10 : 1);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawProperty(SerializedProperty property, float min = 0, float max = 1)
        {
            var labelCache = _controlTypeLabelCache;
            if (labelCache == null)
            {
                labelCache = new Dictionary<int, GUIContent>();
                foreach(var v in Enum.GetValues(typeof(LightLimitControlType)))
                {
                    var idx = (int)v;
                    labelCache.Add(idx, new GUIContent(ObjectNames.NicifyVariableName(((LightLimitControlType)v).ToString())));
                }
                _controlTypeLabelCache = labelCache;
            }

            var rect = EditorGUILayout.GetControlRect(true);
            var label = EditorGUI.BeginProperty(rect, labelCache[property.FindPropertyRelative(nameof(LightLimitChangerPreset.Parameter.Type)).intValue], property);

            var enable = property.FindPropertyRelative(nameof(LightLimitChangerPreset.Parameter.Enable));
            var value = property.FindPropertyRelative(nameof(LightLimitChangerPreset.Parameter.Value));

            var labelRect = rect;
            var sliderRect = rect;

            labelRect.width = EditorGUIUtility.labelWidth; 
            sliderRect.width -= labelRect.width;
            sliderRect.x += labelRect.width;

            enable.boolValue = EditorGUI.ToggleLeft(labelRect, label, enable.boolValue);
            EditorGUI.BeginDisabledGroup(!enable.boolValue);
            value.floatValue = EditorGUI.Slider(sliderRect, value.floatValue, min, max);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        private static Dictionary<int, GUIContent> _controlTypeLabelCache;
    }
}
