using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RedDotSystem
{
    /// <summary>
    /// 红点管理器 - 单例模式
    /// </summary>
    public sealed class RedDotManager
    {
        #region Singleton

        private static RedDotManager s_instance;

        public static RedDotManager Instance =>  s_instance == null ? s_instance = new RedDotManager() : s_instance;

        private RedDotManager()
        {
            Initialize();
        }

        #endregion

        #region Constants

        public const char PATH_SEPARATOR = '/';
        public const string ROOT_PATH = "Root";

        #endregion

        #region Fields

        private RedDotNode m_rootNode;
        private readonly Dictionary<string, RedDotNode> m_nodeDict = new Dictionary<string, RedDotNode>(128);
        private readonly List<string> m_pathCache = new List<string>(8);
        private readonly StringBuilder m_stringBuilder = new StringBuilder(128);
        private bool m_isInitialized;
        private bool m_batchMode;

        #endregion

        #region Properties

        public RedDotNode Root => m_rootNode;
        public bool IsInitialized => m_isInitialized;
        public int NodeCount => m_nodeDict.Count;

        #endregion

        #region Initialization

        private void Initialize()
        {
            if (m_isInitialized)
                return;

            m_rootNode = new RedDotNode(ROOT_PATH);
            m_nodeDict.Add(ROOT_PATH, m_rootNode);
            m_isInitialized = true;

            Debug.Log("[RedDotManager] Initialized");
        }

        public void Reset()
        {
            m_nodeDict.Clear();
            m_rootNode = new RedDotNode(ROOT_PATH);
            m_nodeDict.Add(ROOT_PATH, m_rootNode);

            Debug.Log("[RedDotManager] Reset");
        }

        public void Dispose()
        {
            Reset();
            m_isInitialized = false;
            s_instance = null;
        }

        #endregion

        #region Node Operations

        /// <summary>
        /// 注册红点节点（自动创建路径上的所有节点）
        /// </summary>
        public RedDotNode Register(string path, RedDotType type = RedDotType.Dot,
            RedDotAggregateStrategy strategy = RedDotAggregateStrategy.Or)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[RedDotManager] Path cannot be empty");
                return null;
            }

            if (m_nodeDict.TryGetValue(path, out RedDotNode existingNode))
            {
                return existingNode;
            }

            SplitPath(path, m_pathCache);

            return RegisterInternal(path, m_pathCache, type, strategy);
        }

        /// <summary>
        /// 注册红点节点（使用预计算的路径分段，零 GC）
        /// </summary>
        public RedDotNode Register(string path, string[] segments, RedDotType type = RedDotType.Dot,
            RedDotAggregateStrategy strategy = RedDotAggregateStrategy.Or)
        {
            if (string.IsNullOrEmpty(path) || segments == null || segments.Length == 0)
            {
                Debug.LogError("[RedDotManager] Path and segments cannot be empty");
                return null;
            }

            if (m_nodeDict.TryGetValue(path, out RedDotNode existingNode))
            {
                return existingNode;
            }

            return RegisterInternal(path, segments, type, strategy);
        }

        private RedDotNode RegisterInternal(string fullPath, IList<string> segments, RedDotType type, RedDotAggregateStrategy strategy)
        {
            RedDotNode parent = m_rootNode;
            m_stringBuilder.Clear();

            for (int i = 0; i < segments.Count; i++)
            {
                string segment = segments[i];

                if (m_stringBuilder.Length > 0)
                    m_stringBuilder.Append(PATH_SEPARATOR);
                m_stringBuilder.Append(segment);

                string currentPath = m_stringBuilder.ToString();

                if (!m_nodeDict.TryGetValue(currentPath, out RedDotNode node))
                {
                    node = new RedDotNode(currentPath);
                    m_nodeDict.Add(currentPath, node);
                    parent.AddChild(node);
                }

                if (i == segments.Count - 1)
                {
                    node.Type = type;
                    node.AggregateStrategy = strategy;
                }

                parent = node;
            }

            return parent;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public RedDotNode GetNode(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            m_nodeDict.TryGetValue(path, out RedDotNode node);
            return node;
        }

        /// <summary>
        /// 检查节点是否存在
        /// </summary>
        public bool HasNode(string path)
        {
            return !string.IsNullOrEmpty(path) && m_nodeDict.ContainsKey(path);
        }

        /// <summary>
        /// 注销节点
        /// </summary>
        public void Unregister(string path)
        {
            if (!m_nodeDict.TryGetValue(path, out RedDotNode node))
                return;

            node.RemoveAllListeners();
            node.Parent?.RemoveChild(node);
            m_nodeDict.Remove(path);
        }

        #endregion

        #region Value Operations

        /// <summary>
        /// 设置红点数值
        /// </summary>
        public void SetValue(string path, int value)
        {
            RedDotNode node = GetNode(path);
            if (node == null)
            {
                Debug.LogWarning($"[RedDotManager] Node not found: {path}");
                return;
            }
            node.SetValue(value);
        }

        /// <summary>
        /// 增加红点数值
        /// </summary>
        public void AddValue(string path, int delta)
        {
            RedDotNode node = GetNode(path);
            if (node == null)
            {
                Debug.LogWarning($"[RedDotManager] Node not found: {path}");
                return;
            }
            node.AddValue(delta);
        }

        /// <summary>
        /// 获取红点数值
        /// </summary>
        public int GetValue(string path)
        {
            RedDotNode node = GetNode(path);
            return node?.Value ?? 0;
        }

        /// <summary>
        /// 检查是否显示红点
        /// </summary>
        public bool IsShow(string path)
        {
            return GetValue(path) > 0;
        }

        /// <summary>
        /// 清除红点
        /// </summary>
        public void Clear(string path)
        {
            SetValue(path, 0);
        }

        /// <summary>
        /// 清除所有红点
        /// </summary>
        public void ClearAll()
        {
            foreach (var kvp in m_nodeDict)
            {
                if (kvp.Value.IsLeaf)
                {
                    kvp.Value.SetValue(0);
                }
            }
        }

        #endregion

        #region Listener Operations

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddListener(string path, Action<RedDotNode> callback)
        {
            RedDotNode node = GetNode(path);
            node?.AddListener(callback);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveListener(string path, Action<RedDotNode> callback)
        {
            RedDotNode node = GetNode(path);
            node?.RemoveListener(callback);
        }

        #endregion

        #region Batch Update

        /// <summary>
        /// 开始批量更新模式
        /// </summary>
        public void BeginBatch()
        {
            m_batchMode = true;
        }

        /// <summary>
        /// 结束批量更新模式
        /// </summary>
        public void EndBatch()
        {
            m_batchMode = false;
            FlushDirtyNodes();
        }

        private void FlushDirtyNodes()
        {
            if (m_rootNode.IsDirty)
            {
                m_rootNode.Recalculate();
            }
        }

        public void Update()
        {
            if (!m_batchMode)
            {
                FlushDirtyNodes();
            }
        }

        #endregion

        #region Utility

        private void SplitPath(string path, List<string> result)
        {
            result.Clear();

            int start = 0;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == PATH_SEPARATOR)
                {
                    if (i > start)
                    {
                        result.Add(path.Substring(start, i - start));
                    }
                    start = i + 1;
                }
            }

            if (start < path.Length)
            {
                result.Add(path.Substring(start));
            }
        }

        /// <summary>
        /// 获取所有叶子节点
        /// </summary>
        public List<RedDotNode> GetAllLeafNodes()
        {
            List<RedDotNode> result = new List<RedDotNode>();
            foreach (var kvp in m_nodeDict)
            {
                if (kvp.Value.IsLeaf)
                {
                    result.Add(kvp.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有节点
        /// </summary>
        public IEnumerable<RedDotNode> GetAllNodes()
        {
            return m_nodeDict.Values;
        }

        /// <summary>
        /// 打印树形结构（调试用）
        /// </summary>
        public void PrintTree()
        {
            Debug.Log("[RedDotManager] RedDot Tree Structure:");
            PrintNode(m_rootNode, 0);
        }

        private void PrintNode(RedDotNode node, int depth)
        {
            string indent = new string('-', depth * 2);
            Debug.Log($"{indent}{node.Name} [Value={node.Value}, Type={node.Type}]");

            for (int i = 0; i < node.ChildCount; i++)
            {
                PrintNode(node.GetChild(i), depth + 1);
            }
        }

        #endregion
    }
}