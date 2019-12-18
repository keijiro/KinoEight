//
// KinoEight/EightColor - Color reduction effect with eight color palette
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing.Eight
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Kino/Eight Color")]
    public sealed class EightColor : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        #region Exposed parameters

        public ColorParameter color1 = new ColorParameter(new Color(0, 0, 0, 0), false, true, true);
        public ColorParameter color2 = new ColorParameter(new Color(1, 0, 0, 0), false, true, true);
        public ColorParameter color3 = new ColorParameter(new Color(0, 1, 0, 0), false, true, true);
        public ColorParameter color4 = new ColorParameter(new Color(1, 1, 0, 0), false, true, true);

        public ColorParameter color5 = new ColorParameter(new Color(0, 0, 1, 0), false, true, true);
        public ColorParameter color6 = new ColorParameter(new Color(1, 0, 1, 0), false, true, true);
        public ColorParameter color7 = new ColorParameter(new Color(0, 1, 1, 0), false, true, true);
        public ColorParameter color8 = new ColorParameter(new Color(1, 1, 1, 0), false, true, true);

        public ClampedFloatParameter dithering = new ClampedFloatParameter(0.05f, 0, 0.5f);
        public ClampedIntParameter downsampling = new ClampedIntParameter(1, 1, 32);
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);

        #endregion

        #region Private variables

        static class IDs
        {
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int Downsampling = Shader.PropertyToID("_Downsampling");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
            internal static readonly int Palette = Shader.PropertyToID("_Palette");
        }

        static Vector4 [] _palette = new Vector4 [8];

        Material _material;

        #endregion

        #region Postprocess effect implementation

        public bool IsActive() => _material != null && opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/Kino/PostProcess/Eight/EightColor");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            if (_material == null) return;

            _palette[0] = color1.value; _palette[1] = color2.value;
            _palette[2] = color3.value; _palette[3] = color4.value;

            _palette[4] = color5.value; _palette[5] = color6.value;
            _palette[6] = color7.value; _palette[7] = color8.value;

            _material.SetVectorArray(IDs.Palette, _palette);

            _material.SetFloat(IDs.Dithering, dithering.value);
            _material.SetInt(IDs.Downsampling, downsampling.value);
            _material.SetFloat(IDs.Opacity, opacity.value);

            _material.SetTexture(IDs.InputTexture, srcRT);
            HDUtils.DrawFullScreen(cmd, _material, destRT);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(_material);
        }

        #endregion
    }
}
