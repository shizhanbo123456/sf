using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BulletParticleController))]
public class ParticleColorChangeEditor : Editor
{
    private Color testColor = Color.white;

    public override void OnInspectorGUI()
    {
        // 绘制默认 Inspector
        DrawDefaultInspector();

        BulletParticleController script = (BulletParticleController)target;

        // 增加测试颜色选择器
        GUILayout.Space(10);
        GUILayout.Label("测试颜色", EditorStyles.boldLabel);
        testColor = EditorGUILayout.ColorField("目标颜色", testColor);

        // 增加测试按钮
        if (GUILayout.Button("测试颜色切换"))
        {
            script.ChangeColor(testColor);

            // 在编辑模式下立即更新粒子系统
            foreach (var particle in script.particleSystems)
            {
                if (particle != null)
                {
                    particle.Clear();
                    particle.Play();
                }
            }

            EditorUtility.SetDirty(script);
        }
    }
}