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

        SerializedDataParameter _color1a;
        SerializedDataParameter _color1b;
        SerializedDataParameter _color1c;
        SerializedDataParameter _color1d;

        SerializedDataParameter _color2a;
        SerializedDataParameter _color2b;
        SerializedDataParameter _color2c;
        SerializedDataParameter _color2d;

        SerializedDataParameter _dithering;
        SerializedDataParameter _opacity;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<TiledPalette>(serializedObject);

            _color1a = Unpack(o.Find(x => x.color1a));
            _color1b = Unpack(o.Find(x => x.color1b));
            _color1c = Unpack(o.Find(x => x.color1c));
            _color1d = Unpack(o.Find(x => x.color1d));

            _color2a = Unpack(o.Find(x => x.color2a));
            _color2b = Unpack(o.Find(x => x.color2b));
            _color2c = Unpack(o.Find(x => x.color2c));
            _color2d = Unpack(o.Find(x => x.color2d));

            _dithering = Unpack(o.Find(x => x.dithering));
            _opacity = Unpack(o.Find(x => x.opacity));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Palette 1", EditorStyles.miniLabel);
            PropertyField(_color1a, Labels.color1);
            PropertyField(_color1b, Labels.color2);
            PropertyField(_color1c, Labels.color3);
            PropertyField(_color1d, Labels.color4);

            EditorGUILayout.LabelField("Palette 2", EditorStyles.miniLabel);
            PropertyField(_color2a, Labels.color1);
            PropertyField(_color2b, Labels.color2);
            PropertyField(_color2c, Labels.color3);
            PropertyField(_color2d, Labels.color4);

            EditorGUILayout.Space();
            PropertyField(_dithering);
            PropertyField(_opacity);
        }
    }
}
