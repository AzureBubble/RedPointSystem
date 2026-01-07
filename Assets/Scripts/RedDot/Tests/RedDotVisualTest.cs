using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedDotSystem.Tests
{
    /// <summary>
    /// 红点系统可视化测试脚本
    /// 使用生成的 RedDotPaths（零 GC）
    /// </summary>
    public class RedDotVisualTest : MonoBehaviour
    {
        [Header("自动生成UI")]
        [SerializeField] private bool m_autoCreateUI = true;

        private Canvas m_canvas;
        private Dictionary<string, Text> m_valueTexts = new Dictionary<string, Text>();
        private Dictionary<string, Image> m_redDotImages = new Dictionary<string, Image>();
        private Text m_logText;
        private ScrollRect m_logScrollRect;
        private List<string> m_logs = new List<string>();
        private const int MAX_LOGS = 20;

        // 测试用的叶子节点路径（用于随机测试）
        private static readonly string[] s_LeafPaths =
        {
            RedDotPaths.Main.Mail.System,
            RedDotPaths.Main.Mail.Player,
            RedDotPaths.Main.Bag.Equipment,
            RedDotPaths.Main.Bag.Item,
            RedDotPaths.Main.Quest.Daily,
            RedDotPaths.Social.Friends.Request,
            RedDotPaths.Social.Chat.Private
        };

        private void Start()
        {
            if (m_autoCreateUI)
            {
                CreateTestUI();
            }

            InitializeRedDotSystem();
        }

        private void InitializeRedDotSystem()
        {
            var mgr = RedDotManager.Instance;
            mgr.Reset();
            RedDotPaths.RegisterAll();

            // // ==================== Main 节点 ====================
            // mgr.Register(RedDotPaths.Main.Path, RedDotPaths.Main.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            //
            // // 邮件系统 - Sum策略
            // mgr.Register(RedDotPaths.Main.Mail.Path, RedDotPaths.Main.Mail.Segments, RedDotType.Number, RedDotAggregateStrategy.Sum);
            // mgr.Register(RedDotPaths.Main.Mail.System, RedDotPaths.Main.Mail.SystemSegments, RedDotType.Number);
            // mgr.Register(RedDotPaths.Main.Mail.Player, RedDotPaths.Main.Mail.PlayerSegments, RedDotType.Number);
            //
            // // 背包系统 - Or策略
            // mgr.Register(RedDotPaths.Main.Bag.Path, RedDotPaths.Main.Bag.Segments, RedDotType.New, RedDotAggregateStrategy.Or);
            // mgr.Register(RedDotPaths.Main.Bag.Equipment, RedDotPaths.Main.Bag.EquipmentSegments, RedDotType.New);
            // mgr.Register(RedDotPaths.Main.Bag.Item, RedDotPaths.Main.Bag.ItemSegments, RedDotType.New);
            //
            // // 任务系统
            // mgr.Register(RedDotPaths.Main.Quest.Path, RedDotPaths.Main.Quest.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // mgr.Register(RedDotPaths.Main.Quest.Daily, RedDotPaths.Main.Quest.DailySegments, RedDotType.Dot);
            //
            // // ==================== Social 节点 ====================
            // mgr.Register(RedDotPaths.Social.Path, RedDotPaths.Social.Segments, RedDotType.Number, RedDotAggregateStrategy.Sum);
            //
            // // 好友系统
            // mgr.Register(RedDotPaths.Social.Friends.Path, RedDotPaths.Social.Friends.Segments, RedDotType.Number, RedDotAggregateStrategy.Sum);
            // mgr.Register(RedDotPaths.Social.Friends.Request, RedDotPaths.Social.Friends.RequestSegments, RedDotType.Number);
            //
            // // 聊天系统
            // mgr.Register(RedDotPaths.Social.Chat.Path, RedDotPaths.Social.Chat.Segments, RedDotType.Number, RedDotAggregateStrategy.Sum);
            // mgr.Register(RedDotPaths.Social.Chat.Private, RedDotPaths.Social.Chat.PrivateSegments, RedDotType.Number);

            // ==================== 添加监听 ====================
            // 叶子节点
            mgr.AddListener(RedDotPaths.Main.Mail.System, node => OnRedDotChanged(RedDotPaths.Main.Mail.System, node));
            mgr.AddListener(RedDotPaths.Main.Mail.Player, node => OnRedDotChanged(RedDotPaths.Main.Mail.Player, node));
            mgr.AddListener(RedDotPaths.Main.Bag.Equipment, node => OnRedDotChanged(RedDotPaths.Main.Bag.Equipment, node));
            mgr.AddListener(RedDotPaths.Main.Bag.Item, node => OnRedDotChanged(RedDotPaths.Main.Bag.Item, node));
            mgr.AddListener(RedDotPaths.Main.Quest.Daily, node => OnRedDotChanged(RedDotPaths.Main.Quest.Daily, node));
            mgr.AddListener(RedDotPaths.Social.Friends.Request, node => OnRedDotChanged(RedDotPaths.Social.Friends.Request, node));
            mgr.AddListener(RedDotPaths.Social.Chat.Private, node => OnRedDotChanged(RedDotPaths.Social.Chat.Private, node));

            // 父节点
            mgr.AddListener(RedDotPaths.Main.Path, node => OnRedDotChanged(RedDotPaths.Main.Path, node));
            mgr.AddListener(RedDotPaths.Main.Mail.Path, node => OnRedDotChanged(RedDotPaths.Main.Mail.Path, node));
            mgr.AddListener(RedDotPaths.Main.Bag.Path, node => OnRedDotChanged(RedDotPaths.Main.Bag.Path, node));
            mgr.AddListener(RedDotPaths.Main.Quest.Path, node => OnRedDotChanged(RedDotPaths.Main.Quest.Path, node));
            mgr.AddListener(RedDotPaths.Social.Path, node => OnRedDotChanged(RedDotPaths.Social.Path, node));
            mgr.AddListener(RedDotPaths.Social.Friends.Path, node => OnRedDotChanged(RedDotPaths.Social.Friends.Path, node));
            mgr.AddListener(RedDotPaths.Social.Chat.Path, node => OnRedDotChanged(RedDotPaths.Social.Chat.Path, node));

            AddLog("<color=green>RedDot System Initialized (Zero GC)</color>");
        }

        private void OnRedDotChanged(string path, RedDotNode node)
        {
            UpdateValueDisplay(path, node.Value);
            AddLog($"<color=yellow>[{path}]</color> Value: {node.Value}");
        }

        #region UI Creation

        private void CreateTestUI()
        {
            // 创建EventSystem（如果不存在）
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            // 创建Canvas
            GameObject canvasObj = new GameObject("TestCanvas");
            m_canvas = canvasObj.AddComponent<Canvas>();
            m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            // 创建主面板
            GameObject mainPanel = CreatePanel(canvasObj.transform, "MainPanel", new Vector2(0, 0), new Vector2(1, 1));

            // 左侧：红点树显示
            GameObject leftPanel = CreatePanel(mainPanel.transform, "LeftPanel", new Vector2(0, 0), new Vector2(0.4f, 1));
            CreateTreeDisplay(leftPanel.transform);

            // 中间：操作按钮
            GameObject centerPanel = CreatePanel(mainPanel.transform, "CenterPanel", new Vector2(0.4f, 0), new Vector2(0.7f, 1));
            CreateControlButtons(centerPanel.transform);

            // 右侧：日志显示
            GameObject rightPanel = CreatePanel(mainPanel.transform, "RightPanel", new Vector2(0.7f, 0), new Vector2(1, 1));
            CreateLogDisplay(rightPanel.transform);
        }

        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            return panel;
        }

        private void CreateTreeDisplay(Transform parent)
        {
            // 标题
            CreateLabel(parent, "TreeTitle", "Red Dot Tree", new Vector2(10, -10), new Vector2(200, 30), 18, TextAnchor.MiddleLeft);

            float yOffset = -50;
            float indent = 20;

            // Main节点
            CreateRedDotRow(parent, RedDotPaths.Main.Path, 0, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Mail.Path, 1, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Mail.System, 2, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Mail.Player, 2, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Bag.Path, 1, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Bag.Equipment, 2, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Bag.Item, 2, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Quest.Path, 1, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Main.Quest.Daily, 2, ref yOffset, indent);

            yOffset -= 20;

            // Social节点
            CreateRedDotRow(parent, RedDotPaths.Social.Path, 0, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Social.Friends.Path, 1, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Social.Friends.Request, 2, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Social.Chat.Path, 1, ref yOffset, indent);
            CreateRedDotRow(parent, RedDotPaths.Social.Chat.Private, 2, ref yOffset, indent);
        }

        private void CreateRedDotRow(Transform parent, string path, int level, ref float yOffset, float indent)
        {
            float xOffset = 10 + level * indent;
            string nodeName = path.Contains("/") ? path.Substring(path.LastIndexOf('/') + 1) : path;

            // 红点图标
            GameObject redDotObj = new GameObject($"RedDot_{path}");
            redDotObj.transform.SetParent(parent, false);
            RectTransform rdRect = redDotObj.AddComponent<RectTransform>();
            rdRect.anchorMin = new Vector2(0, 1);
            rdRect.anchorMax = new Vector2(0, 1);
            rdRect.pivot = new Vector2(0, 1);
            rdRect.anchoredPosition = new Vector2(xOffset, yOffset);
            rdRect.sizeDelta = new Vector2(16, 16);

            Image rdImage = redDotObj.AddComponent<Image>();
            rdImage.color = Color.red;
            rdImage.enabled = false;
            m_redDotImages[path] = rdImage;

            // 节点名称
            string prefix = level > 0 ? "|- " : "";
            CreateLabel(parent, $"Label_{path}", prefix + nodeName,
                new Vector2(xOffset + 20, yOffset), new Vector2(150, 20), 14, TextAnchor.MiddleLeft);

            // 数值显示
            var valueText = CreateLabel(parent, $"Value_{path}", "[0]",
                new Vector2(xOffset + 180, yOffset), new Vector2(50, 20), 14, TextAnchor.MiddleLeft);
            valueText.color = Color.cyan;
            m_valueTexts[path] = valueText;

            yOffset -= 25;
        }

        private void CreateControlButtons(Transform parent)
        {
            // 标题
            CreateLabel(parent, "ControlTitle", "Controls", new Vector2(10, -10), new Vector2(200, 30), 18, TextAnchor.MiddleLeft);

            float yOffset = -50;
            float buttonHeight = 35;
            float spacing = 5;

            // 邮件操作
            CreateSectionLabel(parent, "Mail Operations", ref yOffset);
            CreateButton(parent, "+1 System Mail", ref yOffset, buttonHeight, spacing,
                () => AddValue(RedDotPaths.Main.Mail.System, 1));
            CreateButton(parent, "+1 Player Mail", ref yOffset, buttonHeight, spacing,
                () => AddValue(RedDotPaths.Main.Mail.Player, 1));
            CreateButton(parent, "-1 System Mail", ref yOffset, buttonHeight, spacing,
                () => AddValue(RedDotPaths.Main.Mail.System, -1));
            CreateButton(parent, "Clear All Mail", ref yOffset, buttonHeight, spacing,
                () => { Clear(RedDotPaths.Main.Mail.System); Clear(RedDotPaths.Main.Mail.Player); });

            yOffset -= 15;

            // 背包操作
            CreateSectionLabel(parent, "Bag Operations", ref yOffset);
            CreateButton(parent, "New Equipment", ref yOffset, buttonHeight, spacing,
                () => SetValue(RedDotPaths.Main.Bag.Equipment, 1));
            CreateButton(parent, "New Item", ref yOffset, buttonHeight, spacing,
                () => SetValue(RedDotPaths.Main.Bag.Item, 1));
            CreateButton(parent, "Clear Equipment", ref yOffset, buttonHeight, spacing,
                () => Clear(RedDotPaths.Main.Bag.Equipment));
            CreateButton(parent, "Clear Bag", ref yOffset, buttonHeight, spacing,
                () => { Clear(RedDotPaths.Main.Bag.Equipment); Clear(RedDotPaths.Main.Bag.Item); });

            yOffset -= 15;

            // 社交操作
            CreateSectionLabel(parent, "Social Operations", ref yOffset);
            CreateButton(parent, "+3 Friend Requests", ref yOffset, buttonHeight, spacing,
                () => AddValue(RedDotPaths.Social.Friends.Request, 3));
            CreateButton(parent, "+5 Private Messages", ref yOffset, buttonHeight, spacing,
                () => AddValue(RedDotPaths.Social.Chat.Private, 5));
            CreateButton(parent, "Clear Social", ref yOffset, buttonHeight, spacing,
                () => { Clear(RedDotPaths.Social.Friends.Request); Clear(RedDotPaths.Social.Chat.Private); });

            yOffset -= 15;

            // 任务操作
            CreateSectionLabel(parent, "Quest Operations", ref yOffset);
            CreateButton(parent, "Daily Quest Ready", ref yOffset, buttonHeight, spacing,
                () => SetValue(RedDotPaths.Main.Quest.Daily, 1));
            CreateButton(parent, "Complete Quest", ref yOffset, buttonHeight, spacing,
                () => Clear(RedDotPaths.Main.Quest.Daily));

            yOffset -= 15;

            // 全局操作
            CreateSectionLabel(parent, "Global Operations", ref yOffset);
            CreateButton(parent, "CLEAR ALL", ref yOffset, buttonHeight, spacing,
                () => ClearAll(), new Color(0.8f, 0.2f, 0.2f));
            CreateButton(parent, "Random Test", ref yOffset, buttonHeight, spacing,
                () => RandomTest(), new Color(0.2f, 0.6f, 0.2f));
            CreateButton(parent, "Print Tree", ref yOffset, buttonHeight, spacing,
                () => RedDotManager.Instance.PrintTree());
        }

        private void CreateSectionLabel(Transform parent, string text, ref float yOffset)
        {
            var label = CreateLabel(parent, $"Section_{text}", text,
                new Vector2(10, yOffset), new Vector2(250, 25), 14, TextAnchor.MiddleLeft);
            label.color = Color.yellow;
            label.fontStyle = FontStyle.Bold;
            yOffset -= 30;
        }

        private void CreateButton(Transform parent, string text, ref float yOffset, float height, float spacing,
            System.Action onClick, Color? color = null)
        {
            GameObject btnObj = new GameObject($"Btn_{text}");
            btnObj.transform.SetParent(parent, false);

            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, yOffset);
            rect.offsetMin = new Vector2(10, yOffset - height);
            rect.offsetMax = new Vector2(-10, yOffset);

            Image img = btnObj.AddComponent<Image>();
            img.color = color ?? new Color(0.3f, 0.3f, 0.3f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick());

            // 按钮文字
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text btnText = textObj.AddComponent<Text>();
            btnText.text = text;
            btnText.fontSize = 14;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.white;
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            yOffset -= (height + spacing);
        }

        private void CreateLogDisplay(Transform parent)
        {
            // 标题
            CreateLabel(parent, "LogTitle", "Event Log", new Vector2(10, -10), new Vector2(200, 30), 18, TextAnchor.MiddleLeft);

            // 滚动区域
            GameObject scrollObj = new GameObject("LogScroll");
            scrollObj.transform.SetParent(parent, false);

            RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.offsetMin = new Vector2(10, 10);
            scrollRect.offsetMax = new Vector2(-10, -50);

            Image scrollBg = scrollObj.AddComponent<Image>();
            scrollBg.color = new Color(0.1f, 0.1f, 0.1f);

            m_logScrollRect = scrollObj.AddComponent<ScrollRect>();

            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollObj.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(5, 5);
            viewportRect.offsetMax = new Vector2(-5, -5);
            viewport.AddComponent<Image>().color = Color.clear;
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.spacing = 2;
            layout.padding = new RectOffset(5, 5, 5, 5);

            m_logScrollRect.viewport = viewportRect;
            m_logScrollRect.content = contentRect;
            m_logScrollRect.horizontal = false;
            m_logScrollRect.vertical = true;

            // Log Text
            GameObject logTextObj = new GameObject("LogText");
            logTextObj.transform.SetParent(content.transform, false);
            m_logText = logTextObj.AddComponent<Text>();
            m_logText.fontSize = 12;
            m_logText.color = Color.white;
            m_logText.alignment = TextAnchor.UpperLeft;
            m_logText.text = "";
            m_logText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            m_logText.supportRichText = true;

            RectTransform logTextRect = logTextObj.GetComponent<RectTransform>();
            logTextRect.anchorMin = new Vector2(0, 1);
            logTextRect.anchorMax = new Vector2(1, 1);
            logTextRect.pivot = new Vector2(0.5f, 1);
        }

        private Text CreateLabel(Transform parent, string name, string text, Vector2 position, Vector2 size, float fontSize, TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text label = obj.AddComponent<Text>();
            label.text = text;
            label.fontSize = (int)fontSize;
            label.alignment = alignment;
            label.color = Color.white;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.supportRichText = true;

            return label;
        }

        #endregion

        #region Red Dot Operations

        private void SetValue(string path, int value)
        {
            RedDotManager.Instance.SetValue(path, value);
            AddLog($"SetValue({path}, {value})");
        }

        private void AddValue(string path, int delta)
        {
            RedDotManager.Instance.AddValue(path, delta);
            AddLog($"AddValue({path}, {delta})");
        }

        private void Clear(string path)
        {
            RedDotManager.Instance.Clear(path);
            AddLog($"Clear({path})");
        }

        private void ClearAll()
        {
            RedDotManager.Instance.ClearAll();
            AddLog("<color=red>ClearAll()</color>");

            // 刷新所有显示
            foreach (var path in m_valueTexts.Keys)
            {
                UpdateValueDisplay(path, 0);
            }
        }

        private void RandomTest()
        {
            string randomPath = s_LeafPaths[Random.Range(0, s_LeafPaths.Length)];
            int randomValue = Random.Range(1, 10);

            RedDotManager.Instance.SetValue(randomPath, randomValue);
            AddLog($"<color=green>Random: {randomPath} = {randomValue}</color>");
        }

        private void UpdateValueDisplay(string path, int value)
        {
            if (m_valueTexts.TryGetValue(path, out var text))
            {
                text.text = $"[{value}]";
                text.color = value > 0 ? Color.red : Color.cyan;
            }

            if (m_redDotImages.TryGetValue(path, out var image))
            {
                image.enabled = value > 0;
            }
        }

        private void AddLog(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            m_logs.Add($"<color=#888888>{timestamp}</color> {message}");

            if (m_logs.Count > MAX_LOGS)
            {
                m_logs.RemoveAt(0);
            }

            if (m_logText != null)
            {
                m_logText.text = string.Join("\n", m_logs);
            }
        }

        #endregion
    }
}