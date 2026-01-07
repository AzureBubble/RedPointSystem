using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RedDotSystem.Editor
{
    /// <summary>
    /// 红点树可视化编辑器窗口
    /// </summary>
    public class RedDotTreeEditorWindow : EditorWindow
    {
        private RedDotTreeConfig m_config;
        private Vector2 m_scrollPosition;
        private RedDotNodeConfig m_selectedNode;
        private RedDotNodeConfig m_draggedNode;
        private RedDotNodeConfig m_draggedNodeParent;
        private List<RedDotNodeConfig> m_draggedNodeList;
        private bool m_isDragging;

        // 样式
        private GUIStyle m_nodeStyle;
        private GUIStyle m_selectedNodeStyle;
        private GUIStyle m_pathLabelStyle;
        private GUIStyle m_headerStyle;

        // 颜色
        private readonly Color m_selectedColor = new Color(0.3f, 0.6f, 0.9f, 0.3f);
        private readonly Color m_dragTargetColor = new Color(0.9f, 0.6f, 0.3f, 0.3f);

        [MenuItem("Tools/RedDot/Tree Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<RedDotTreeEditorWindow>("RedDot Tree Editor");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        /// <summary>
        /// 双击 RedDotTreeConfig 资源时打开编辑器窗口
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as RedDotTreeConfig;
            if (asset == null)
                return false;

            var window = GetWindow<RedDotTreeEditorWindow>("RedDot Tree Editor");
            window.minSize = new Vector2(400, 300);
            window.SetConfig(asset);
            window.Show();
            return true;
        }

        /// <summary>
        /// 设置要编辑的配置
        /// </summary>
        public void SetConfig(RedDotTreeConfig config)
        {
            m_config = config;
            if (m_config != null)
            {
                m_config.RefreshPaths();
            }
            Repaint();
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            // 尝试加载现有配置
            string[] guids = AssetDatabase.FindAssets("t:RedDotTreeConfig");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                m_config = AssetDatabase.LoadAssetAtPath<RedDotTreeConfig>(path);
            }
        }

        private void InitStyles()
        {
            if (m_nodeStyle == null)
            {
                m_nodeStyle = new GUIStyle(EditorStyles.label)
                {
                    padding = new RectOffset(4, 4, 2, 2)
                };
            }

            if (m_selectedNodeStyle == null)
            {
                m_selectedNodeStyle = new GUIStyle(EditorStyles.label)
                {
                    padding = new RectOffset(4, 4, 2, 2),
                    fontStyle = FontStyle.Bold
                };
            }

            if (m_pathLabelStyle == null)
            {
                m_pathLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = Color.gray }
                };
            }

            if (m_headerStyle == null)
            {
                m_headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14
                };
            }
        }

        private void OnGUI()
        {
            InitStyles();

            // 工具栏
            DrawToolbar();

            if (m_config == null)
            {
                DrawNoConfigMessage();
                return;
            }

            EditorGUILayout.BeginHorizontal();

            // 左侧：树形视图
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.55f));
            DrawTreeView();
            EditorGUILayout.EndVertical();

            // 分隔线
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));

            // 右侧：节点详情
            EditorGUILayout.BeginVertical();
            DrawNodeInspector();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            // 处理拖拽
            HandleDragAndDrop();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // 配置选择
            EditorGUI.BeginChangeCheck();
            m_config = (RedDotTreeConfig)EditorGUILayout.ObjectField(m_config, typeof(RedDotTreeConfig), false, GUILayout.Width(200));
            if (EditorGUI.EndChangeCheck() && m_config != null)
            {
                m_config.RefreshPaths();
            }

            if (GUILayout.Button("New Config", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                CreateNewConfig();
            }

            GUILayout.FlexibleSpace();

            // 操作按钮
            GUI.enabled = m_config != null;

            if (GUILayout.Button("Expand All", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                SetAllFoldout(true);
            }

            if (GUILayout.Button("Collapse", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                SetAllFoldout(false);
            }

            GUILayout.Space(10);

            GUI.backgroundColor = new Color(0.6f, 0.9f, 0.6f);
            if (GUILayout.Button("Generate Code", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GenerateCode();
            }
            GUI.backgroundColor = Color.white;

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoConfigMessage()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Please select or create a RedDotTreeConfig asset.", MessageType.Info);

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Create New Config", GUILayout.Height(30)))
            {
                CreateNewConfig();
            }
        }

        private void CreateNewConfig()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create RedDot Tree Config",
                "RedDotTreeConfig",
                "asset",
                "Choose a location to save the config");

            if (!string.IsNullOrEmpty(path))
            {
                m_config = ScriptableObject.CreateInstance<RedDotTreeConfig>();
                AssetDatabase.CreateAsset(m_config, path);
                AssetDatabase.SaveAssets();
                Selection.activeObject = m_config;
            }
        }

        private void DrawTreeView()
        {
            EditorGUILayout.LabelField("Red Dot Tree", m_headerStyle);
            EditorGUILayout.Space(5);

            // 添加根节点按钮
            if (GUILayout.Button("+ Add Root Node", GUILayout.Height(24)))
            {
                AddRootNode();
            }

            EditorGUILayout.Space(5);

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            for (int i = 0; i < m_config.RootNodes.Count; i++)
            {
                DrawNode(m_config.RootNodes[i], 0, m_config.RootNodes, i);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawNode(RedDotNodeConfig node, int depth, List<RedDotNodeConfig> parentList, int index)
        {
            if (node == null) return;

            // 绘制当前节点行
            Rect rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(22));

            // 选中高亮
            if (m_selectedNode == node)
            {
                EditorGUI.DrawRect(rowRect, m_selectedColor);
            }

            // 子节点缩进（根据深度）
            if (depth > 0)
            {
                GUILayout.Space(depth * 20);
            }

            // 折叠箭头
            if (node.children.Count > 0)
            {
                string arrow = node.foldout ? "▼" : "▶";
                if (GUILayout.Button(arrow, EditorStyles.label, GUILayout.Width(16)))
                {
                    node.foldout = !node.foldout;
                }
            }
            else
            {
                GUILayout.Space(16);
            }

            // 节点图标
            GUIContent icon = EditorGUIUtility.IconContent("d_Favorite Icon");
            GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));

            // 节点名称（可点击选中）
            var style = m_selectedNode == node ? m_selectedNodeStyle : m_nodeStyle;
            if (GUILayout.Button(node.description, style, GUILayout.MinWidth(60)))
            {
                m_selectedNode = node;
            }

            // 路径预览
            GUILayout.Label($"({node.generatedPath})", m_pathLabelStyle);

            GUILayout.FlexibleSpace();

            // 操作按钮
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                AddChildNode(node);
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Delete Node",
                    $"Are you sure you want to delete '{node.name}' and all its children?", "Yes", "No"))
                {
                    parentList.RemoveAt(index);
                    if (m_selectedNode == node) m_selectedNode = null;
                    MarkDirty();
                }
            }

            // 拖拽句柄
            GUILayout.Label("≡", GUILayout.Width(18));
            Rect dragRect = GUILayoutUtility.GetLastRect();
            HandleNodeDrag(node, parentList, dragRect);

            EditorGUILayout.EndHorizontal();

            // 递归绘制子节点（展开时）
            if (node.foldout && node.children.Count > 0)
            {
                for (int i = 0; i < node.children.Count; i++)
                {
                    DrawNode(node.children[i], depth + 1, node.children, i);
                }
            }
        }

        private void HandleNodeDrag(RedDotNodeConfig node, List<RedDotNodeConfig> parentList, Rect dragRect)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && dragRect.Contains(e.mousePosition))
            {
                m_isDragging = true;
                m_draggedNode = node;
                m_draggedNodeList = parentList;
                e.Use();
            }
        }

        private void HandleDragAndDrop()
        {
            Event e = Event.current;

            if (m_isDragging && e.type == EventType.MouseUp)
            {
                m_isDragging = false;
                m_draggedNode = null;
                m_draggedNodeList = null;
                Repaint();
            }
        }

        private void DrawNodeInspector()
        {
            EditorGUILayout.LabelField("Node Properties", m_headerStyle);
            EditorGUILayout.Space(5);

            if (m_selectedNode == null)
            {
                EditorGUILayout.HelpBox("Select a node to edit its properties.", MessageType.Info);
                return;
            }

            EditorGUI.BeginChangeCheck();

            // 节点名称
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
            m_selectedNode.name = EditorGUILayout.TextField(m_selectedNode.name);

            EditorGUILayout.Space(5);

            // 生成的路径（只读）
            EditorGUILayout.LabelField("Generated Path", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(m_selectedNode.generatedPath);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);

            // 描述
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            m_selectedNode.description = EditorGUILayout.TextArea(m_selectedNode.description, GUILayout.Height(40));

            EditorGUILayout.Space(5);

            // 类型
            EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
            m_selectedNode.type = (RedDotType)EditorGUILayout.EnumPopup(m_selectedNode.type);

            EditorGUILayout.Space(5);

            // 聚合策略
            EditorGUILayout.LabelField("Aggregate Strategy", EditorStyles.boldLabel);
            m_selectedNode.strategy = (RedDotAggregateStrategy)EditorGUILayout.EnumPopup(m_selectedNode.strategy);

            EditorGUILayout.Space(10);

            // 子节点信息
            EditorGUILayout.LabelField($"Children: {m_selectedNode.children.Count}", EditorStyles.miniLabel);

            if (EditorGUI.EndChangeCheck())
            {
                m_config.RefreshPaths();
                MarkDirty();
            }

            EditorGUILayout.Space(10);

            // 快捷操作
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Child Node"))
            {
                AddChildNode(m_selectedNode);
            }

            if (GUILayout.Button("Duplicate Node"))
            {
                DuplicateNode(m_selectedNode);
            }

            EditorGUILayout.Space(10);

            // 复制路径按钮
            if (GUILayout.Button("Copy Path to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = m_selectedNode.generatedPath;
                Debug.Log($"Copied: {m_selectedNode.generatedPath}");
            }
        }

        private void AddRootNode()
        {
            var newNode = new RedDotNodeConfig("NewRoot");
            m_config.RootNodes.Add(newNode);
            m_config.RefreshPaths();
            m_selectedNode = newNode;
            MarkDirty();
        }

        private void AddChildNode(RedDotNodeConfig parent)
        {
            var newNode = new RedDotNodeConfig("NewNode");
            parent.children.Add(newNode);
            parent.foldout = true;
            m_config.RefreshPaths();
            m_selectedNode = newNode;
            MarkDirty();
        }

        private void DuplicateNode(RedDotNodeConfig node)
        {
            var copy = new RedDotNodeConfig(node.name + "_Copy")
            {
                description = node.description,
                type = node.type,
                strategy = node.strategy
            };

            // 找到父节点并添加
            foreach (var root in m_config.RootNodes)
            {
                if (TryAddAfterNode(root, node, copy, m_config.RootNodes))
                {
                    m_config.RefreshPaths();
                    m_selectedNode = copy;
                    MarkDirty();
                    return;
                }
            }

            // 如果是根节点
            int index = m_config.RootNodes.IndexOf(node);
            if (index >= 0)
            {
                m_config.RootNodes.Insert(index + 1, copy);
                m_config.RefreshPaths();
                m_selectedNode = copy;
                MarkDirty();
            }
        }

        private bool TryAddAfterNode(RedDotNodeConfig current, RedDotNodeConfig target, RedDotNodeConfig newNode, List<RedDotNodeConfig> parentList)
        {
            int index = current.children.IndexOf(target);
            if (index >= 0)
            {
                current.children.Insert(index + 1, newNode);
                return true;
            }

            foreach (var child in current.children)
            {
                if (TryAddAfterNode(child, target, newNode, current.children))
                    return true;
            }

            return false;
        }

        private void SetAllFoldout(bool foldout)
        {
            foreach (var root in m_config.RootNodes)
            {
                SetNodeFoldout(root, foldout);
            }
            Repaint();
        }

        private void SetNodeFoldout(RedDotNodeConfig node, bool foldout)
        {
            node.foldout = foldout;
            foreach (var child in node.children)
            {
                SetNodeFoldout(child, foldout);
            }
        }

        private void MarkDirty()
        {
            if (m_config != null)
            {
                EditorUtility.SetDirty(m_config);
            }
        }

        #region Code Generation

        private void GenerateCode()
        {
            if (m_config == null) return;

            m_config.RefreshPaths();

            StringBuilder sb = new StringBuilder();

            // 文件头
            sb.AppendLine("// =============================================================================");
            sb.AppendLine("// AUTO-GENERATED FILE - DO NOT EDIT MANUALLY");
            sb.AppendLine($"// Generated from: {AssetDatabase.GetAssetPath(m_config)}");
            sb.AppendLine($"// Generated at: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("// =============================================================================");
            sb.AppendLine();

            sb.AppendLine($"namespace {m_config.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {m_config.ClassName}");
            sb.AppendLine("    {");

            // 生成嵌套类结构
            foreach (var root in m_config.RootNodes)
            {
                GenerateNodeCode(sb, root, 2);
            }

            // 生成 RegisterAll 方法
            sb.AppendLine();
            GenerateRegisterAllMethod(sb);

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // 写入文件
            string fullPath = Path.Combine(Application.dataPath, m_config.OutputPath);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 如果文件已存在且为只读，先移除只读属性
            if (File.Exists(fullPath))
            {
                var attributes = File.GetAttributes(fullPath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(fullPath, attributes & ~FileAttributes.ReadOnly);
                }
            }

            File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);

            // 设置文件为只读
            File.SetAttributes(fullPath, File.GetAttributes(fullPath) | FileAttributes.ReadOnly);

            AssetDatabase.Refresh();

            Debug.Log($"[RedDot] Code generated successfully: Assets/{m_config.OutputPath}");
            EditorUtility.DisplayDialog("Success", $"Code generated successfully!\n\nPath: Assets/{m_config.OutputPath}", "OK");
        }

        private void GenerateRegisterAllMethod(StringBuilder sb)
        {
            string indent = "        ";

            sb.AppendLine($"{indent}/// <summary>");
            sb.AppendLine($"{indent}/// 注册所有红点节点到 RedDotManager（零 GC）");
            sb.AppendLine($"{indent}/// </summary>");
            sb.AppendLine($"{indent}public static void RegisterAll()");
            sb.AppendLine($"{indent}{{");
            sb.AppendLine($"{indent}    var mgr = RedDotManager.Instance;");
            sb.AppendLine();

            // 收集所有节点并生成注册代码
            foreach (var root in m_config.RootNodes)
            {
                GenerateRegisterCode(sb, root, indent + "    ", "");
            }

            sb.AppendLine($"{indent}}}");
        }

        private void GenerateRegisterCode(StringBuilder sb, RedDotNodeConfig node, string indent, string parentAccessPath)
        {
            string nodeName = SanitizeName(node.name);
            string accessPath;

            if (string.IsNullOrEmpty(parentAccessPath))
            {
                accessPath = nodeName;
            }
            else
            {
                accessPath = $"{parentAccessPath}.{nodeName}";
            }

            bool hasChildren = node.children.Count > 0;

            // 生成注册代码
            string pathExpr = hasChildren ? $"{accessPath}.Path" : accessPath;
            string segmentsExpr = hasChildren ? $"{accessPath}.Segments" : $"{accessPath}Segments";

            // 添加注释
            if (!string.IsNullOrEmpty(node.description))
            {
                sb.AppendLine($"{indent}// {node.description}");
            }

            sb.AppendLine($"{indent}mgr.Register({pathExpr}, {segmentsExpr}, RedDotType.{node.type}, RedDotAggregateStrategy.{node.strategy});");

            // 递归处理子节点
            if (hasChildren)
            {
                sb.AppendLine();
                foreach (var child in node.children)
                {
                    GenerateRegisterCode(sb, child, indent, accessPath);
                }
            }
        }

        private void GenerateNodeCode(StringBuilder sb, RedDotNodeConfig node, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);
            string nodeName = SanitizeName(node.name);
            string segmentsArray = GenerateSegmentsArray(node.generatedPath);

            if (node.children.Count > 0)
            {
                // 有子节点，生成嵌套类
                sb.AppendLine();

                if (!string.IsNullOrEmpty(node.description))
                {
                    sb.AppendLine($"{indent}/// <summary>");
                    sb.AppendLine($"{indent}/// {node.description}");
                    sb.AppendLine($"{indent}/// </summary>");
                }

                sb.AppendLine($"{indent}public static class {nodeName}");
                sb.AppendLine($"{indent}{{");

                // Path 常量
                sb.AppendLine($"{indent}    public const string Path = \"{node.generatedPath}\";");
                // Segments 数组（预计算，零 GC）
                sb.AppendLine($"{indent}    public static readonly string[] Segments = {segmentsArray};");

                // 子节点
                foreach (var child in node.children)
                {
                    GenerateNodeCode(sb, child, indentLevel + 1);
                }

                sb.AppendLine($"{indent}}}");
            }
            else
            {
                // 叶子节点，生成常量和对应的 Segments
                if (!string.IsNullOrEmpty(node.description))
                {
                    sb.AppendLine();
                    sb.AppendLine($"{indent}/// <summary>");
                    sb.AppendLine($"{indent}/// {node.description}");
                    sb.AppendLine($"{indent}/// </summary>");
                }

                sb.AppendLine($"{indent}public const string {nodeName} = \"{node.generatedPath}\";");
                sb.AppendLine($"{indent}public static readonly string[] {nodeName}Segments = {segmentsArray};");
            }
        }

        private string GenerateSegmentsArray(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "{ }";

            string[] segments = path.Split('/');
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");

            for (int i = 0; i < segments.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append($"\"{segments[i]}\"");
            }

            sb.Append(" }");
            return sb.ToString();
        }

        private string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Unknown";

            StringBuilder sb = new StringBuilder();
            bool capitalizeNext = true;

            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(capitalizeNext ? char.ToUpper(c) : c);
                    capitalizeNext = false;
                }
                else if (c == '_' || c == ' ' || c == '-')
                {
                    capitalizeNext = true;
                }
            }

            string result = sb.ToString();

            // 确保不以数字开头
            if (result.Length > 0 && char.IsDigit(result[0]))
            {
                result = "_" + result;
            }

            return result;
        }

        #endregion
    }
}