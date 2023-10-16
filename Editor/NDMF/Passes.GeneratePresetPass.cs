using System;
using System.Linq;
using System.Reflection.Emit;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace io.github.azukimochi
{
    internal static partial class Passes
    {
        internal sealed class GeneratePresetPass : LightLimitChangerBasePass<GeneratePresetPass>
        {
            protected override void Execute(BuildContext context, Session session, LightLimitChangerObjectCache cache)
            {
                if (session.Presets.Length == 0)
                    return;

                var controller = session.Controller;

                var layer = new AnimatorControllerLayer()
                {
                    name = "LightLimitChanger Presets",
                    defaultWeight = 1,
                    stateMachine = new AnimatorStateMachine().HideInHierarchy().AddTo(cache),
                };

                controller.AddParameter(new AnimatorControllerParameter() { name = ParameterName_Preset, type = AnimatorControllerParameterType.Int, defaultInt = 0 });
                
                var blank = new AnimationClip().AddTo(cache);
                var idle = layer.stateMachine.AddState("Idle");
                layer.stateMachine.defaultState = idle;
                idle.writeDefaultValues = false;
                idle.motion = blank;

                var condition = new AnimatorCondition[1];
                int idx = 1;
                foreach(var preset in session.Presets)
                {
                    var state = layer.stateMachine.AddState(preset.name);
                    state.writeDefaultValues = false;
                    state.motion = blank;

                    var dr = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                    
                    foreach(ref readonly var parameter in preset.Parameters.AsSpan())
                    {
                        var type = parameter.Type;
                        if (!parameter.Enable || !session.TargetControl.HasFlag(parameter.Type) || !(session.Controls.FirstOrDefault(x => x.ControlType == type).ParameterName is string parameterName))
                            continue;

                        dr.parameters.Add(new VRC_AvatarParameterDriver.Parameter() { type = VRC_AvatarParameterDriver.ChangeType.Set, name = parameterName, value = parameter.Value });
                    }

                    var t = idle.AddTransition(state);
                    t.duration = 0;
                    t.hasExitTime = false;
                    condition[0] = new AnimatorCondition() { parameter = ParameterName_Preset, mode = AnimatorConditionMode.Equals, threshold = idx };
                    t.conditions = condition;

                    t = state.AddTransition(idle);
                    t.duration = 0;
                    t.hasExitTime = false;
                    condition[0] = new AnimatorCondition() { parameter = ParameterName_Preset, mode = AnimatorConditionMode.NotEqual, threshold = idx };
                    t.conditions = condition;

                    idx++;
                }

                controller.AddLayer(layer);
            }
        }
    }
}
