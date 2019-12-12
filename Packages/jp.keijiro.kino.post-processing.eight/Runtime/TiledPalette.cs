//
// KinoEight/TiledPalette - Color reduction effect with 8x8 tiled palettes
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing.Eight
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Kino/Tiled Palette")]
    public sealed class TiledPalette : CustomPostProcessVolumeComponent, IPostProcessComponent
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
        public ClampedFloatParameter glitch = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);

        #endregion

        #region Static class variables

        static class IDs
        {
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int Glitch = Shader.PropertyToID("_Glitch");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
            internal static readonly int OutputTexture = Shader.PropertyToID("_OutputTexture");
            internal static readonly int Palette = Shader.PropertyToID("_Palette");
        }

        static ComputeShader _compute;
        static Vector4 [] _palette = new Vector4 [8];

        #endregion

        #region Postprocess effect implementation

        public bool IsActive() => opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (_compute == null) _compute = Resources.Load<ComputeShader>("KinoEightTiledPalette");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            _palette[0] = color1.value; _palette[1] = color2.value;
            _palette[2] = color3.value; _palette[3] = color4.value;

            _palette[4] = color5.value; _palette[5] = color6.value;
            _palette[6] = color7.value; _palette[7] = color8.value;

            cmd.SetComputeVectorArrayParam(_compute, IDs.Palette, _palette);

            cmd.SetComputeFloatParam(_compute, IDs.Dithering, dithering.value);
            cmd.SetComputeFloatParam(_compute, IDs.Glitch, glitch.value);
            cmd.SetComputeFloatParam(_compute, IDs.Opacity, opacity.value);

            cmd.SetComputeTextureParam(_compute, 0, IDs.InputTexture, srcRT);
            cmd.SetComputeTextureParam(_compute, 0, IDs.OutputTexture, destRT);

            var bx = (camera.actualWidth  + 7) / 8;
            var by = (camera.actualHeight + 7) / 8;
            cmd.DispatchCompute(_compute, 0, bx, by, 1);
        }

        public override void Cleanup()
        {
        }

        #endregion
    }
}
