using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Kino/Four Color")]
    public sealed class FourColor : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ColorParameter color1 = new ColorParameter(new Color(0, 0, 0, 0), false, true, true);
        public ColorParameter color2 = new ColorParameter(new Color(1, 0, 0, 0), false, true, true);
        public ColorParameter color3 = new ColorParameter(new Color(0, 1, 0, 0), false, true, true);
        public ColorParameter color4 = new ColorParameter(new Color(1, 1, 1, 0), false, true, true);
        public ClampedFloatParameter dithering = new ClampedFloatParameter(0.05f, 0, 1);
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);

        Material _material;

        static class ShaderIDs
        {
            internal static readonly int Color1 = Shader.PropertyToID("_Color1");
            internal static readonly int Color2 = Shader.PropertyToID("_Color2");
            internal static readonly int Color3 = Shader.PropertyToID("_Color3");
            internal static readonly int Color4 = Shader.PropertyToID("_Color4");
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
        }

        public bool IsActive() => _material != null && opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/Kino/PostProcess/FourColor");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            if (_material == null) return;

            _material.SetColor(ShaderIDs.Color1, color1.value);
            _material.SetColor(ShaderIDs.Color2, color2.value);
            _material.SetColor(ShaderIDs.Color3, color3.value);
            _material.SetColor(ShaderIDs.Color4, color4.value);
            _material.SetFloat(ShaderIDs.Dithering, dithering.value);
            _material.SetFloat(ShaderIDs.Opacity, opacity.value);
            _material.SetTexture(ShaderIDs.InputTexture, srcRT);

            HDUtils.DrawFullScreen(cmd, _material, destRT);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(_material);
        }
    }
}
