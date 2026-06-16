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

        private static readonly Regex IdentifierRegex = new Regex(
            @"^[_a-zA-Z][_a-zA-Z0-9]*$",
            RegexOptions.Compiled);

        private TemplateKind templateKind = TemplateKind.Component;
        private string typeName = "NewComponent";
        private string namespaceName = DefaultComponentNamespace;
        private string targetFolder = DefaultComponentFolder;
        private bool overwriteExisting;
        private Vector2 scrollPosition;

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

        private static void OpenWithPreset(TemplateKind kind)
        {
            Open();

            EcsTemplateGeneratorWindow window = GetWindow<EcsTemplateGeneratorWindow>();
            window.templateKind = kind;
            window.ApplyDefaultsForKind(kind);

            string selectedFolder = GetSelectedProjectFolder();
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                window.targetFolder = selectedFolder;
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Generate ECS Code", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            EditorGUI.BeginChangeCheck();
            templateKind = (TemplateKind)EditorGUILayout.EnumPopup("Template", templateKind);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyDefaultsForKind(templateKind);
            }

            typeName = EditorGUILayout.TextField("Type Name", typeName);
            namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);

            EditorGUILayout.BeginHorizontal();
            targetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);
            if (GUILayout.Button("Selected", GUILayout.Width(78f)))
            {
                string selectedFolder = GetSelectedProjectFolder();
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    targetFolder = selectedFolder;
                }
            }

            EditorGUILayout.EndHorizontal();

            overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing", overwriteExisting);

            EditorGUILayout.Space(8f);
            DrawPreview();

            EditorGUILayout.Space(8f);
            DrawValidationAndActions();

            EditorGUILayout.EndScrollView();
        }

        private void ApplyDefaultsForKind(TemplateKind kind)
        {
            switch (kind)
            {
                case TemplateKind.SystemBase:
                    typeName = "NewSystem";
                    namespaceName = DefaultSystemNamespace;
                    targetFolder = DefaultSystemFolder;
                    break;
                case TemplateKind.ISystem:
                    typeName = "NewSystem";
                    namespaceName = DefaultSystemNamespace;
                    targetFolder = DefaultSystemFolder;
                    break;
                case TemplateKind.TagComponent:
                    typeName = "NewTag";
                    namespaceName = $"{DefaultComponentNamespace}.Tag";
                    targetFolder = $"{DefaultComponentFolder}/Tag";
                    break;
                case TemplateKind.PlainStruct:
                    typeName = "NewStruct";
                    namespaceName = DefaultComponentNamespace;
                    targetFolder = DefaultComponentFolder;
                    break;
                default:
                    typeName = "NewComponent";
                    namespaceName = DefaultComponentNamespace;
                    targetFolder = DefaultComponentFolder;
                    break;
            }
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextArea(
                    BuildTemplate(SanitizeTypeName(typeName), namespaceName),
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
            string cleanTypeName = SanitizeTypeName(typeName);
            string normalizedFolder = NormalizeAssetPath(targetFolder);
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string absoluteFolder = Path.Combine(projectRoot, normalizedFolder);
            string absolutePath = Path.Combine(absoluteFolder, $"{cleanTypeName}.cs");

            if (!Directory.Exists(absoluteFolder))
            {
                Directory.CreateDirectory(absoluteFolder);
            }

            if (File.Exists(absolutePath) && !overwriteExisting)
            {
                EditorUtility.DisplayDialog(
                    "Script already exists",
                    $"{normalizedFolder}/{cleanTypeName}.cs already exists. Enable overwrite to replace it.",
                    "OK");
                return;
            }

            File.WriteAllText(
                absolutePath,
                BuildTemplate(cleanTypeName, namespaceName),
                new UTF8Encoding(false));

            AssetDatabase.Refresh();

            string assetPath = $"{normalizedFolder}/{cleanTypeName}.cs";
            Object script = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            Selection.activeObject = script;
            EditorGUIUtility.PingObject(script);
        }

        private string GetValidationError()
        {
            string cleanTypeName = SanitizeTypeName(typeName);

            if (string.IsNullOrWhiteSpace(cleanTypeName))
            {
                return "Type name is required.";
            }

            if (!IdentifierRegex.IsMatch(cleanTypeName))
            {
                return "Type name must be a valid C# identifier.";
            }

            if (!IsValidNamespace(namespaceName))
            {
                return "Namespace must contain valid C# identifiers separated by dots.";
            }

            string normalizedFolder = NormalizeAssetPath(targetFolder);
            if (string.IsNullOrWhiteSpace(normalizedFolder) ||
                (normalizedFolder != "Assets" && !normalizedFolder.StartsWith("Assets/")))
            {
                return "Target folder must be inside the Unity Assets folder.";
            }

            return null;
        }

        private string BuildTemplate(string cleanTypeName, string cleanNamespace)
        {
            switch (templateKind)
            {
                case TemplateKind.TagComponent:
                    return BuildTagComponentTemplate(cleanTypeName, cleanNamespace);
                case TemplateKind.PlainStruct:
                    return BuildPlainStructTemplate(cleanTypeName, cleanNamespace);
                case TemplateKind.SystemBase:
                    return BuildSystemBaseTemplate(cleanTypeName, cleanNamespace);
                case TemplateKind.ISystem:
                    return BuildISystemTemplate(cleanTypeName, cleanNamespace);
                default:
                    return BuildComponentTemplate(cleanTypeName, cleanNamespace);
            }
        }

        private static string BuildComponentTemplate(string cleanTypeName, string cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {cleanNamespace}
{{
    public struct {cleanTypeName} : IComponentData
    {{
        public float Value;
    }}
}}
";
        }

        private static string BuildTagComponentTemplate(string cleanTypeName, string cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {cleanNamespace}
{{
    public struct {cleanTypeName} : IComponentData
    {{
    }}
}}
";
        }

        private static string BuildPlainStructTemplate(string cleanTypeName, string cleanNamespace)
        {
            return
$@"namespace {cleanNamespace}
{{
    public struct {cleanTypeName}
    {{
    }}
}}
";
        }

        private static string BuildSystemBaseTemplate(string cleanTypeName, string cleanNamespace)
        {
            return
$@"using Unity.Entities;

namespace {cleanNamespace}
{{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class {cleanTypeName} : SystemBase
    {{
        protected override void OnUpdate()
        {{
        }}
    }}
}}
";
        }

        private static string BuildISystemTemplate(string cleanTypeName, string cleanNamespace)
        {
            return
$@"using Unity.Burst;
using Unity.Entities;

namespace {cleanNamespace}
{{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct {cleanTypeName} : ISystem
    {{
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {{
        }}

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {{
        }}
    }}
}}
";
        }

        private static bool IsValidNamespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string[] parts = value.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                if (!IdentifierRegex.IsMatch(parts[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string SanitizeTypeName(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string NormalizeAssetPath(string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : path.Replace('\\', '/').Trim().TrimEnd('/');
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
