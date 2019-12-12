using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing.Eight
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Kino/Tiled Palette")]
    public sealed class TiledPalette : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ColorParameter color1a = new ColorParameter(new Color(0, 0, 0, 0), false, true, true);
        public ColorParameter color1b = new ColorParameter(new Color(1, 0, 0, 0), false, true, true);
        public ColorParameter color1c = new ColorParameter(new Color(0, 1, 0, 0), false, true, true);
        public ColorParameter color1d = new ColorParameter(new Color(1, 1, 0, 0), false, true, true);

        public ColorParameter color2a = new ColorParameter(new Color(0, 0, 1, 0), false, true, true);
        public ColorParameter color2b = new ColorParameter(new Color(1, 0, 1, 0), false, true, true);
        public ColorParameter color2c = new ColorParameter(new Color(0, 1, 1, 0), false, true, true);
        public ColorParameter color2d = new ColorParameter(new Color(1, 1, 1, 0), false, true, true);

        public ClampedFloatParameter dithering = new ClampedFloatParameter(0.05f, 0, 0.5f);
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);

        static class IDs
        {
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
            internal static readonly int OutputTexture = Shader.PropertyToID("_OutputTexture");
            internal static readonly int Palette = Shader.PropertyToID("_Palette");
        }

        public bool IsActive() => opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        static ComputeShader _compute;
        static Vector4 [] _palette = new Vector4 [8];

        public override void Setup()
        {
            if (_compute == null)
                _compute = Resources.Load<ComputeShader>("TiledPalette");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            _palette[0] = color1a.value; _palette[1] = color1b.value;
            _palette[2] = color1c.value; _palette[3] = color1d.value;

            _palette[4] = color2a.value; _palette[5] = color2b.value;
            _palette[6] = color2c.value; _palette[7] = color2d.value;

            cmd.SetComputeVectorArrayParam(_compute, IDs.Palette, _palette);

            cmd.SetComputeFloatParam(_compute, IDs.Dithering, dithering.value);
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
    }
}
