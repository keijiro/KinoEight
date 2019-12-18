using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kino.PostProcessing.Eight
{
    [VolumeComponentEditor(typeof(TiledPalette))]
    public sealed class TiledPaletteEditor : VolumeComponentEditor
    {
        static class Labels
        {
            internal static readonly GUIContent color1 = new GUIContent("Color 1");
            internal static readonly GUIContent color2 = new GUIContent("Color 2");
            internal static readonly GUIContent color3 = new GUIContent("Color 3");
            internal static readonly GUIContent color4 = new GUIContent("Color 4");
        }

        SerializedDataParameter _color1;
        SerializedDataParameter _color2;
        SerializedDataParameter _color3;
        SerializedDataParameter _color4;

        SerializedDataParameter _color5;
        SerializedDataParameter _color6;
        SerializedDataParameter _color7;
        SerializedDataParameter _color8;

        SerializedDataParameter _dithering;
        SerializedDataParameter _downsampling;
        SerializedDataParameter _glitch;
        SerializedDataParameter _opacity;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<TiledPalette>(serializedObject);

            _color1 = Unpack(o.Find(x => x.color1));
            _color2 = Unpack(o.Find(x => x.color2));
            _color3 = Unpack(o.Find(x => x.color3));
            _color4 = Unpack(o.Find(x => x.color4));

            _color5 = Unpack(o.Find(x => x.color5));
            _color6 = Unpack(o.Find(x => x.color6));
            _color7 = Unpack(o.Find(x => x.color7));
            _color8 = Unpack(o.Find(x => x.color8));

            _dithering = Unpack(o.Find(x => x.dithering));
            _downsampling = Unpack(o.Find(x => x.downsampling));
            _glitch = Unpack(o.Find(x => x.glitch));
            _opacity = Unpack(o.Find(x => x.opacity));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Palette 1", EditorStyles.miniLabel);
            PropertyField(_color1);
            PropertyField(_color2);
            PropertyField(_color3);
            PropertyField(_color4);

            EditorGUILayout.LabelField("Palette 2", EditorStyles.miniLabel);
            PropertyField(_color5, Labels.color1);
            PropertyField(_color6, Labels.color2);
            PropertyField(_color7, Labels.color3);
            PropertyField(_color8, Labels.color4);

            EditorGUILayout.Space();
            PropertyField(_dithering);
            PropertyField(_downsampling);
            PropertyField(_glitch);
            PropertyField(_opacity);
        }
    }
}
