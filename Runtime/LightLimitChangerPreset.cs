using System;
using UnityEngine;

namespace io.github.azukimochi
{
    [DisallowMultipleComponent]
    public sealed class LightLimitChangerPreset : TagComponent
    {
        public Parameter[] Parameters = new[]
        {
            new Parameter(LightLimitControlType.Light, 0.5f),
            new Parameter(LightLimitControlType.LightMin, 0.5f),
            new Parameter(LightLimitControlType.LightMax, 0.5f),
            new Parameter(LightLimitControlType.Saturation, 0.5f),
            new Parameter(LightLimitControlType.ColorTemperature, 0.5f),
            new Parameter(LightLimitControlType.Unlit, 0f),
        };

        public LightLimitChangerSettings GetParent() => GetComponentInParent<LightLimitChangerSettings>();

        public void CopyLightSettingsFromParameters(in LightLimitChangerParameters parameters)
        {
            for (int  i = 0; i < Parameters.Length; i++)
            {
                if (LightLimitControlType.Light.HasFlag(Parameters[i].Type))
                {
                    Parameters[i].Value = parameters.DefaultLightValue;
                }
            }
        }

        [Serializable]
        public struct Parameter
        {
            [HideInInspector]
            public LightLimitControlType Type;
            public float Value;
            public bool Enable;

            public Parameter(LightLimitControlType type, float value, bool enable = true)
            {
                Type = type;
                Value = value;
                Enable = enable;
            }
        }
    }
}
