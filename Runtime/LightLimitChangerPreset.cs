using System;
using UnityEngine;

namespace io.github.azukimochi
{
    [DisallowMultipleComponent]
    public sealed class LightLimitChangerPreset : TagComponent
    {
        public Parameter Light = new Parameter(0.5f);
        public Parameter LightMin = new Parameter(0.5f);
        public Parameter LightMax = new Parameter(0.5f);
        public Parameter Saturation = new Parameter(0.5f);
        public Parameter ColorTemperature = new Parameter(0.5f);
        public Parameter Unlit = new Parameter(0f);

        [Serializable]
        public struct Parameter
        {
            public bool Enable;
            public float Value;

            public Parameter(float value, bool enable = true)
            {
                Value = value;
                Enable = enable;
            }
        }

        public LightLimitChangerSettings GetParent() => GetComponentInParent<LightLimitChangerSettings>();

        public void CopyLightSettingsFromParameters(in LightLimitChangerParameters parameters)
        {
            Light.Value = parameters.DefaultLightValue;
            LightMin.Value = parameters.DefaultLightValue;
            LightMax.Value = parameters.DefaultLightValue;
        }
    }
}
