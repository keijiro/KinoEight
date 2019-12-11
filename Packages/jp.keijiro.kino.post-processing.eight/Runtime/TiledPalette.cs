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
            internal static readonly int Color1a = Shader.PropertyToID("_Color1a");
            internal static readonly int Color1b = Shader.PropertyToID("_Color1b");
            internal static readonly int Color1c = Shader.PropertyToID("_Color1c");
            internal static readonly int Color1d = Shader.PropertyToID("_Color1d");
            internal static readonly int Color2a = Shader.PropertyToID("_Color2a");
            internal static readonly int Color2b = Shader.PropertyToID("_Color2b");
            internal static readonly int Color2c = Shader.PropertyToID("_Color2c");
            internal static readonly int Color2d = Shader.PropertyToID("_Color2d");
            internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
            internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
            internal static readonly int OutputTexture = Shader.PropertyToID("_OutputTexture");
        }

        public bool IsActive() => opacity.value > 0;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        static ComputeShader _compute;

        public override void Setup()
        {
            if (_compute == null)
                _compute = Resources.Load<ComputeShader>("TiledPalette");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
        {
            var bx = (camera.actualWidth  + 7) / 8;
            var by = (camera.actualHeight + 7) / 8;

            cmd.SetComputeFloatParam(_compute, IDs.Dithering, dithering.value);
            cmd.SetComputeFloatParam(_compute, IDs.Opacity, opacity.value);

            cmd.SetComputeVectorParam(_compute, IDs.Color1a, color1a.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color1b, color1b.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color1c, color1c.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color1d, color1d.value);

            cmd.SetComputeVectorParam(_compute, IDs.Color2a, color2a.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color2b, color2b.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color2c, color2c.value);
            cmd.SetComputeVectorParam(_compute, IDs.Color2d, color2d.value);

            cmd.SetComputeTextureParam(_compute, 0, IDs.InputTexture, srcRT);
            cmd.SetComputeTextureParam(_compute, 0, IDs.OutputTexture, destRT);

            cmd.DispatchCompute(_compute, 0, bx, by, 1);
        }

        public override void Cleanup()
        {
        }
    }
}
