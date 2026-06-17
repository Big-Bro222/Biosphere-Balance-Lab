using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BioSphereLab.Editor
{
    public sealed class EcsTemplateGeneratorWindow : EditorWindow
    {
        private const string DefaultComponentFolder = "Assets/Scripts/Components";
        private const string DefaultSystemFolder = "Assets/Scripts/Systems";
        private const string DefaultComponentNamespace = "BioSphereLab.Components";
        private const string DefaultSystemNamespace = "BioSphereLab.Systems";

        private static readonly Regex m_identifierRegex = new Regex(
            @"^[_a-zA-Z][_a-zA-Z0-9]*$",
            RegexOptions.Compiled);

        private TemplateKind m_templateKind = TemplateKind.Component;
        private string m_typeName = "NewComponent";
        private string m_namespaceName = DefaultComponentNamespace;
        private string m_targetFolder = DefaultComponentFolder;
        private bool m_overwriteExisting;
        private Vector2 m_scrollPosition;

        private enum TemplateKind
        {
            Component,
            TagComponent,
            PlainStruct,
            SystemBase,
            ISystem
        }

        [MenuItem("Tools/BioSphereLab/Scripting/ECS Template Generator")]
        public static void Open()
        {
            EcsTemplateGeneratorWindow window = GetWindow<EcsTemplateGeneratorWindow>("ECS Templates");
            window.minSize = new Vector2(430f, 360f);
            window.Show();
        }

        [MenuItem("Assets/Create/Scripting/BioSphereLab/Component")]
        private static void OpenComponentPreset()
        {
            OpenWithPreset(TemplateKind.Component);
        }

        [MenuItem("Assets/Create/Scripting/BioSphereLab/Tag Component")]
        private static void OpenTagComponentPreset()
        {
            OpenWithPreset(TemplateKind.TagComponent);
        }

        [MenuItem("Assets/Create/Scripting/BioSphereLab/Plain Struct")]
        private static void OpenPlainStructPreset()
        {
            OpenWithPreset(TemplateKind.PlainStruct);
        }

        [MenuItem("Assets/Create/Scripting/BioSphereLab/SystemBase System")]
        private static void OpenSystemBasePreset()
        {
            OpenWithPreset(TemplateKind.SystemBase);
        }

        [MenuItem("Assets/Create/Scripting/BioSphereLab/ISystem System")]
        private static void OpenISystemPreset()
        {
            OpenWithPreset(TemplateKind.ISystem);
        }

        private static void OpenWithPreset(TemplateKind p_kind)
        {
            Open();

            EcsTemplateGeneratorWindow window = GetWindow<EcsTemplateGeneratorWindow>();
            window.m_templateKind = p_kind;
            window.ApplyDefaultsForKind(p_kind);

            string selectedFolder = GetSelectedProjectFolder();
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                window.m_targetFolder = selectedFolder;
            }
        }

        private void OnGUI()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            EditorGUILayout.LabelField("Generate ECS Code", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            EditorGUI.BeginChangeCheck();
            m_templateKind = (TemplateKind)EditorGUILayout.EnumPopup("Template", m_templateKind);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyDefaultsForKind(m_templateKind);
            }

            m_typeName = EditorGUILayout.TextField("Type Name", m_typeName);
            m_namespaceName = EditorGUILayout.TextField("Namespace", m_namespaceName);

            EditorGUILayout.BeginHorizontal();
            m_targetFolder = EditorGUILayout.TextField("Target Folder", m_targetFolder);
            if (GUILayout.Button("Selected", GUILayout.Width(78f)))
            {
                string selectedFolder = GetSelectedProjectFolder();
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    m_targetFolder = selectedFolder;
                }
            }

            EditorGUILayout.EndHorizontal();

            m_overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing", m_overwriteExisting);

            EditorGUILayout.Space(8f);
            DrawPreview();

            EditorGUILayout.Space(8f);
            DrawValidationAndActions();

            EditorGUILayout.EndScrollView();
        }

        private void ApplyDefaultsForKind(TemplateKind p_kind)
        {
            switch (p_kind)
            {
                case TemplateKind.SystemBase:
                    m_typeName = "NewSystem";
                    m_namespaceName = DefaultSystemNamespace;
                    m_targetFolder = DefaultSystemFolder;
                    break;
                case TemplateKind.ISystem:
                    m_typeName = "NewSystem";
                    m_namespaceName = DefaultSystemNamespace;
                    m_targetFolder = DefaultSystemFolder;
                    break;
                case TemplateKind.TagComponent:
                    m_typeName = "NewTag";
                    m_namespaceName = $"{DefaultComponentNamespace}.Tag";
                    m_targetFolder = $"{DefaultComponentFolder}/Tag";
                    break;
                case TemplateKind.PlainStruct:
                    m_typeName = "NewStruct";
                    m_namespaceName = DefaultComponentNamespace;
                    m_targetFolder = DefaultComponentFolder;
                    break;
                default:
                    m_typeName = "NewComponent";
                    m_namespaceName = DefaultComponentNamespace;
                    m_targetFolder = DefaultComponentFolder;
                    break;
            }
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextArea(
                    BuildTemplate(SanitizeTypeName(m_typeName), m_namespaceName),
                    GUILayout.MinHeight(150f));
            }
        }

        private void DrawValidationAndActions()
        {
            string validationError = GetValidationError();
            bool canCreate = string.IsNullOrEmpty(validationError);

            if (!canCreate)
            {
                EditorGUILayout.HelpBox(validationError, MessageType.Error);
            }

            using (new EditorGUI.DisabledScope(!canCreate))
            {
                if (GUILayout.Button("Create Script", GUILayout.Height(32f)))
                {
                    CreateScript();
                }
            }
        }

        private void CreateScript()
        {
            string cleanTypeName = SanitizeTypeName(m_typeName);
            string normalizedFolder = NormalizeAssetPath(m_targetFolder);
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string absoluteFolder = Path.Combine(projectRoot, normalizedFolder);
            string absolutePath = Path.Combine(absoluteFolder, $"{cleanTypeName}.cs");

            if (!Directory.Exists(absoluteFolder))
            {
                Directory.CreateDirectory(absoluteFolder);
            }

            if (File.Exists(absolutePath) && !m_overwriteExisting)
            {
                EditorUtility.DisplayDialog(
                    "Script already exists",
                    $"{normalizedFolder}/{cleanTypeName}.cs already exists. Enable overwrite to replace it.",
                    "OK");
                return;
            }

            File.WriteAllText(
                absolutePath,
                BuildTemplate(cleanTypeName, m_namespaceName),
                new UTF8Encoding(false));

            AssetDatabase.Refresh();

            string assetPath = $"{normalizedFolder}/{cleanTypeName}.cs";
            Object script = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            Selection.activeObject = script;
            EditorGUIUtility.PingObject(script);
        }

        private string GetValidationError()
        {
            string cleanTypeName = SanitizeTypeName(m_typeName);

            if (string.IsNullOrWhiteSpace(cleanTypeName))
            {
                return "Type name is required.";
            }

            if (!m_identifierRegex.IsMatch(cleanTypeName))
            {
                return "Type name must be a valid C# identifier.";
            }

            if (!IsValidNamespace(m_namespaceName))
            {
                return "Namespace must contain valid C# identifiers separated by dots.";
            }

            string normalizedFolder = NormalizeAssetPath(m_targetFolder);
            if (string.IsNullOrWhiteSpace(normalizedFolder) ||
                (normalizedFolder != "Assets" && !normalizedFolder.StartsWith("Assets/")))
            {
                return "Target folder must be inside the Unity Assets folder.";
            }

            return null;
        }

        private string BuildTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            switch (m_templateKind)
            {
                case TemplateKind.TagComponent:
                    return BuildTagComponentTemplate(p_cleanTypeName, p_cleanNamespace);
                case TemplateKind.PlainStruct:
                    return BuildPlainStructTemplate(p_cleanTypeName, p_cleanNamespace);
                case TemplateKind.SystemBase:
                    return BuildSystemBaseTemplate(p_cleanTypeName, p_cleanNamespace);
                case TemplateKind.ISystem:
                    return BuildISystemTemplate(p_cleanTypeName, p_cleanNamespace);
                default:
                    return BuildComponentTemplate(p_cleanTypeName, p_cleanNamespace);
            }
        }

        private static string BuildComponentTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {p_cleanNamespace}
{{
    public struct {p_cleanTypeName} : IComponentData
    {{
        public float Value;
    }}
}}
";
        }

        private static string BuildTagComponentTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {p_cleanNamespace}
{{
    public struct {p_cleanTypeName} : IComponentData
    {{
    }}
}}
";
        }

        private static string BuildPlainStructTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            return
$@"namespace {p_cleanNamespace}
{{
    public struct {p_cleanTypeName}
    {{
    }}
}}
";
        }

        private static string BuildSystemBaseTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {p_cleanNamespace}
{{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class {p_cleanTypeName} : SystemBase
    {{
        protected override void OnUpdate()
        {{
        }}
    }}
}}
";
        }

        private static string BuildISystemTemplate(string p_cleanTypeName, string p_cleanNamespace)
        {
            return
$@"using Unity.Burst;
using Unity.Entities;

namespace {p_cleanNamespace}
{{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct {p_cleanTypeName} : ISystem
    {{
        [BurstCompile]
        public void OnCreate(ref SystemState p_state)
        {{
        }}

        [BurstCompile]
        public void OnUpdate(ref SystemState p_state)
        {{
        }}
    }}
}}
";
        }

        private static bool IsValidNamespace(string p_value)
        {
            if (string.IsNullOrWhiteSpace(p_value))
            {
                return false;
            }

            string[] parts = p_value.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                if (!m_identifierRegex.IsMatch(parts[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string SanitizeTypeName(string p_value)
        {
            return string.IsNullOrWhiteSpace(p_value) ? string.Empty : p_value.Trim();
        }

        private static string NormalizeAssetPath(string p_path)
        {
            return string.IsNullOrWhiteSpace(p_path)
                ? string.Empty
                : p_path.Replace('\\', '/').Trim().TrimEnd('/');
        }

        private static string GetSelectedProjectFolder()
        {
            Object[] selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            for (int i = 0; i < selectedAssets.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(selectedAssets[i]);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (!AssetDatabase.IsValidFolder(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                return NormalizeAssetPath(path);
            }

            return null;
        }
    }
}
