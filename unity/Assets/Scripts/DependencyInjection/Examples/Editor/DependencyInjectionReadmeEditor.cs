using BioSphereLab.DependencyInjection.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Examples.Editor
{
    [CustomEditor(typeof(DependencyInjectionReadme))]
    public sealed class DependencyInjectionReadmeEditor : UnityEditor.Editor
    {
        private const string ReadmeAssetPath = "Assets/Scripts/DependencyInjection/Examples/DependencyInjectionReadme.asset";
        private const string MarkdownReadmePath = "Assets/Scripts/DependencyInjection/Examples/README.md";
        private const string StorageAssetPath = "Assets/Scripts/DependencyInjection/Examples/StorageInjection.asset";
        private const string InjectionScopePath = "Assets/Scripts/DependencyInjection/Infrastructure/InjectionScope.cs";
        private const string InjectionContainerPath = "Assets/Scripts/DependencyInjection/Infrastructure/InjectionContainer.cs";
        private const string EcsRegistryPath = "Assets/Scripts/DependencyInjection/Infrastructure/EcsInjectionRegistry.cs";
        private const string InstallerPath = "Assets/Scripts/DependencyInjection/Examples/ExampleBiosphereDependencyInstaller.cs";
        private const string UsageExamplePath = "Assets/Scripts/DependencyInjection/Examples/DependencyInjectionUsageExample.cs";
        private const string EcsExamplePath = "Assets/Scripts/DependencyInjection/Examples/ExampleSystem.cs";

        private GUIStyle m_titleStyle;
        private GUIStyle m_sectionStyle;
        private GUIStyle m_bodyStyle;
        private GUIStyle m_codeStyle;

        public override void OnInspectorGUI()
        {
            EnsureStyles();

            EditorGUILayout.LabelField("Dependency Injection", m_titleStyle);
            EditorGUILayout.LabelField(
                "Runtime dependency registration for scene objects and ECS systems.",
                m_bodyStyle);

            DrawSection("Setup");
            DrawStep("1", "Create or select a StorageInjection asset.");
            DrawStep("2", "Add an empty scene GameObject named Dependency Scope.");
            DrawStep("3", "Add InjectionScope and assign the StorageInjection asset.");
            DrawStep("4", "Add ExampleBiosphereDependencyInstaller to the same GameObject.");
            DrawStep("5", "Add DependencyInjectionUsageExample to any scene GameObject.");

            DrawSection("Runtime Flow");
            EditorGUILayout.LabelField(
                "InjectionScope -> IInjectionInstaller -> InjectionContainer -> EcsInjectionRegistry -> ExampleSystem",
                m_codeStyle);

            DrawSection("Namespaces");
            EditorGUILayout.LabelField("BioSphereLab.DependencyInjection.Infrastructure", m_codeStyle);
            EditorGUILayout.LabelField("BioSphereLab.DependencyInjection.Examples", m_codeStyle);

            DrawSection("Quick Actions");
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create StorageInjection"))
                {
                    CreateStorageInjectionAsset();
                }

                if (GUILayout.Button("Open Markdown"))
                {
                    SelectAsset(MarkdownReadmePath, true);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("InjectionScope"))
                {
                    SelectAsset(InjectionScopePath, true);
                }

                if (GUILayout.Button("InjectionContainer"))
                {
                    SelectAsset(InjectionContainerPath, true);
                }

                if (GUILayout.Button("ECS Registry"))
                {
                    SelectAsset(EcsRegistryPath, true);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Installer"))
                {
                    SelectAsset(InstallerPath, true);
                }

                if (GUILayout.Button("MonoBehaviour Example"))
                {
                    SelectAsset(UsageExamplePath, true);
                }

                if (GUILayout.Button("ECS Example"))
                {
                    SelectAsset(EcsExamplePath, true);
                }
            }

            DrawSection("Rule Of Thumb");
            EditorGUILayout.HelpBox(
                "Keep simulation data in ECS components. Use dependency injection for managed configuration, prefab lookups, factories, adapters, and bridge services.",
                MessageType.Info);
        }

        private static void CreateStorageInjectionAsset()
        {
            string path = AssetDatabase.GenerateUniqueAssetPath(StorageAssetPath);
            StorageInjection storageInjection = CreateInstance<StorageInjection>();
            AssetDatabase.CreateAsset(storageInjection, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = storageInjection;
            EditorGUIUtility.PingObject(storageInjection);
        }

        private static void SelectAsset(string p_assetPath, bool p_openAsset)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(p_assetPath);

            if (asset == null)
            {
                EditorUtility.DisplayDialog("Asset Not Found", p_assetPath, "OK");
                return;
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);

            if (p_openAsset)
            {
                AssetDatabase.OpenAsset(asset);
            }
        }

        private void DrawSection(string p_text)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(p_text, m_sectionStyle);
        }

        private void DrawStep(string p_number, string p_text)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(p_number + ".", GUILayout.Width(22));
                EditorGUILayout.LabelField(p_text, m_bodyStyle);
            }
        }

        private void EnsureStyles()
        {
            m_titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                wordWrap = true
            };

            m_sectionStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                wordWrap = true
            };

            m_bodyStyle ??= new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };

            m_codeStyle ??= new GUIStyle(EditorStyles.helpBox)
            {
                font = EditorStyles.miniFont,
                wordWrap = true
            };
        }
    }
}
