using UnityEngine;
using UnityEngine.Rendering;

namespace Kino.PostProcessing.Eight {

[ExecuteInEditMode]
public sealed partial class EightColorController : MonoBehaviour
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
    [field:SerializeField, Range(0, 1)] public float Opacity = 1;

    #endregion

    #region Runtime public property

    public Material Material => UpdateMaterial();

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private variables

    static class IDs
    {
        internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
        internal static readonly int Downsampling = Shader.PropertyToID("_Downsampling");
        internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
        internal static readonly int Palette1 = Shader.PropertyToID("_Palette1");
        internal static readonly int Palette2 = Shader.PropertyToID("_Palette2");
    }

    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void OnDestroy()
      => CoreUtils.Destroy(_material);

    void OnDisable()
      => OnDestroy();

    void Update() {} // Just for providing the component enable switch.

    #endregion

    #region Controller implementation

    public Material UpdateMaterial()
    {
        if (_material == null)
            _material = CoreUtils.CreateEngineMaterial(_shader);

        var palette1 = new Matrix4x4(Color1, Color2, Color3, Color4);
        var palette2 = new Matrix4x4(Color5, Color6, Color7, Color8);

        _material.SetMatrix(IDs.Palette1, palette1.transpose);
        _material.SetMatrix(IDs.Palette2, palette2.transpose);
        _material.SetFloat(IDs.Dithering, Dithering);
        _material.SetFloat(IDs.Downsampling, Downsampling);
        _material.SetFloat(IDs.Opacity, Opacity);

        return _material;
    }

    #endregion
}

} // namespace Kino.PostProcessing.Eight
