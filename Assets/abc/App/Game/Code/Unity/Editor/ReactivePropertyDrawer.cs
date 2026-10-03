using R3;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Editor
{
    class ReactiveView<T>
    {
        static readonly Color Tint = new Color(0.65f, 0.85f, 1f);

        class Box
        {
            [HideInInspector] public ReadOnlyReactiveProperty<T> Source;
            [HideInInspector] public string Label;

            [ShowInInspector, LabelText("$Label")]
            public T Value => Source.CurrentValue;
        }

        Box box;
        PropertyTree tree;

        public void Draw(ReadOnlyReactiveProperty<T> src, GUIContent label)
        {
            if (src == null)
            {
                EditorGUILayout.LabelField(label, new GUIContent("null"));
                return;
            }

            if (tree == null || box.Source != src)
            {
                tree?.Dispose();
                box = new Box { Source = src };
                tree = PropertyTree.Create(box);
            }

            box.Label = label?.text;
            GUIHelper.PushColor(Tint);
            tree.Draw(false);
            GUIHelper.PopColor();

            if (Application.isPlaying) GUIHelper.RequestRepaint();
        }
    }

    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class ReadOnlyReactivePropertyDrawer<T> : OdinValueDrawer<ReadOnlyReactiveProperty<T>>
    {
        readonly ReactiveView<T> view = new();
        protected override void DrawPropertyLayout(GUIContent label) => view.Draw(ValueEntry.SmartValue, label);
    }

    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class ReactivePropertyDrawer<T> : OdinValueDrawer<ReactiveProperty<T>>
    {
        readonly ReactiveView<T> view = new();
        protected override void DrawPropertyLayout(GUIContent label) => view.Draw(ValueEntry.SmartValue, label);
    }
}