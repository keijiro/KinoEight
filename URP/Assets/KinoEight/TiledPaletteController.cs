using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Kino.PostProcessing.Eight {

[ExecuteInEditMode]
public sealed partial class TiledPaletteController : MonoBehaviour
{
    #region Editable properties

    [field:SerializeField, ColorUsage(false)] public Color Color1 { get; set; } = new Color(0, 0, 0, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color2 { get; set; } = new Color(1, 0, 0, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color3 { get; set; } = new Color(0, 1, 0, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color4 { get; set; } = new Color(1, 1, 0, 0);

    [field:SerializeField, ColorUsage(false)] public Color Color5 { get; set; } = new Color(0, 0, 1, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color6 { get; set; } = new Color(1, 0, 1, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color7 { get; set; } = new Color(0, 1, 1, 0);
    [field:SerializeField, ColorUsage(false)] public Color Color8 { get; set; } = new Color(1, 1, 1, 0);

    [field:SerializeField, Range(0, 1)] public float Dithering = 0.05f;
    [field:SerializeField, Range(1, 32)] public int Downsampling = 1;
    [field:SerializeField, Range(0, 1)] public float Glitch = 0;
    [field:SerializeField, Range(0, 1)] public float Opacity = 1;

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] ComputeShader _compute = null;
    [SerializeField, HideInInspector] Texture2D _font = null;

    #endregion

    #region Private members

    static class IDs
    {
        internal static readonly int Dithering = Shader.PropertyToID("Dithering");
        internal static readonly int Downsampling = Shader.PropertyToID("Downsampling");
        internal static readonly int Glitch = Shader.PropertyToID("Glitch");
        internal static readonly int InputTexture = Shader.PropertyToID("InputTexture");
        internal static readonly int LocalTime = Shader.PropertyToID("LocalTime");
        internal static readonly int Opacity = Shader.PropertyToID("Opacity");
        internal static readonly int OutputTexture = Shader.PropertyToID("OutputTexture");
        internal static readonly int Palette1 = Shader.PropertyToID("Palette1");
        internal static readonly int Palette2 = Shader.PropertyToID("Palette2");
    }

    float _time;

    #endregion

    #region Render function (exposed for RendererFeature)

    public void ExecutePass(ComputeGraphContext context,
                            TextureHandle source, TextureHandle dest,
                            in TextureDesc desc)
    {
        var palette1 = new Matrix4x4(Color1, Color2, Color3, Color4);
        var palette2 = new Matrix4x4(Color5, Color6, Color7, Color8);

        var cmd = context.cmd;
        cmd.SetComputeMatrixParam(_compute, IDs.Palette1, palette1.transpose);
        cmd.SetComputeMatrixParam(_compute, IDs.Palette2, palette2.transpose);
        cmd.SetComputeFloatParam(_compute, IDs.Dithering, Dithering);
        cmd.SetComputeIntParam(_compute, IDs.Downsampling, Downsampling);
        cmd.SetComputeFloatParam(_compute, IDs.Glitch, Glitch);
        cmd.SetComputeFloatParam(_compute, IDs.LocalTime, _time);
        cmd.SetComputeFloatParam(_compute, IDs.Opacity, Opacity);
        cmd.SetComputeTextureParam(_compute, 0, IDs.InputTexture, source);
        cmd.SetComputeTextureParam(_compute, 0, IDs.OutputTexture, dest);

        var stride = Downsampling * 8;
        var bx = (desc.width  + stride - 1) / stride;
        var by = (desc.height + stride - 1) / stride;
        cmd.DispatchCompute(_compute, 0, bx, by, 1);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => Shader.SetGlobalTexture("_KinoEightFontTexture", _font);

    void Update()
      => _time += Time.deltaTime;

    #endregion
}

} // namespace Kino.PostProcessing.Eight
