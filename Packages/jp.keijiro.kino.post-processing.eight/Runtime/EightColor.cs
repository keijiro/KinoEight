using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing.Eight
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Kino/Eight Color")]
    public sealed class EightColor : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ColorParameter color1 = new ColorParameter(new Color(0, 0, 0, 0), false, true, true);
        public ColorParameter color2 = new ColorParameter(new Color(1, 0, 0, 0), false, true, true);
        public ColorParameter color3 = new ColorParameter(new Color(0, 1, 0, 0), false, true, true);
        public ColorParameter color4 = new ColorParameter(new Color(1, 1, 0, 0), false, true, true);
        public ColorParameter color5 = new ColorParameter(new Color(0, 0, 1, 0), false, true, true);
        public ColorParameter color6 = new ColorParameter(new Color(1, 0, 1, 0), false, true, true);
        public ColorParameter color7 = new ColorParameter(new Color(0, 1, 1, 0), false, true, true);
        public ColorParameter color8 = new ColorParameter(new Color(1, 1, 1, 0), false, true, true);
        public ClampedFloatParameter dithering = new ClampedFloatParameter(0.05f, 0, 0.5f);
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);

        Material _material;

        static class ShaderIDs
        {
            internal static readonly int Color1 = Shader.PropertyToID("_Color1");
            internal static readonly int Color2 = Shader.PropertyToID("_Color2");
            internal static readonly int Color3 = Shader.PropertyToID("_Color3");
            internal static readonly int Color4 = Shader.PropertyToID("_Color4");
            internal static readonly int Color5 = Shader.PropertyToID("_Color5");
            internal static readonly int Color6 = Shader.PropertyToID("_Color6");
            internal static readonly int Color7 = Shader.PropertyToID("_Color7");
            internal static readonly int Color8 = Shader.PropertyToID("_Color8");
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
        }

        public bool IsActive() => _material != null && opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/Kino/PostProcess/EightColor");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            if (_material == null) return;

            _material.SetColor(ShaderIDs.Color1, color1.value);
            _material.SetColor(ShaderIDs.Color2, color2.value);
            _material.SetColor(ShaderIDs.Color3, color3.value);
            _material.SetColor(ShaderIDs.Color4, color4.value);
            _material.SetColor(ShaderIDs.Color5, color5.value);
            _material.SetColor(ShaderIDs.Color6, color6.value);
            _material.SetColor(ShaderIDs.Color7, color7.value);
            _material.SetColor(ShaderIDs.Color8, color8.value);
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
