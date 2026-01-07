using System;
using System.Collections.Generic;

namespace RedDotSystem
{
    /// <summary>
    /// 红点节点 - 核心数据结构
    /// </summary>
    public sealed class RedDotNode
    {
        #region Fields

        private readonly string m_path;
        private readonly string m_name;
        private RedDotType m_type;
        private RedDotAggregateStrategy m_aggregateStrategy;

        private RedDotNode m_parent;
        private readonly List<RedDotNode> m_children;

        private int m_value;
        private bool m_isDirty;
        private bool m_isLeaf;

        private Action<RedDotNode> m_onValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// 节点完整路径（如 "Main/Mail/System"）
        /// </summary>
        public string Path => m_path;

        /// <summary>
        /// 节点名称（路径最后一段）
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// 红点类型
        /// </summary>
        public RedDotType Type
        {
            get => m_type;
            set => m_type = value;
        }

        /// <summary>
        /// 聚合策略
        /// </summary>
        public RedDotAggregateStrategy AggregateStrategy
        {
            get => m_aggregateStrategy;
            set => m_aggregateStrategy = value;
        }

        /// <summary>
        /// 红点数值（0表示无红点）
        /// </summary>
        public int Value => m_value;

        /// <summary>
        /// 是否显示红点
        /// </summary>
        public bool IsShow => m_value > 0;

        /// <summary>
        /// 是否为叶子节点
        /// </summary>
        public bool IsLeaf => m_isLeaf;

        /// <summary>
        /// 父节点
        /// </summary>
        public RedDotNode Parent => m_parent;

        /// <summary>
        /// 子节点数量
        /// </summary>
        public int ChildCount => m_children.Count;

        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool IsDirty => m_isDirty;

        #endregion

        #region Constructor

        public RedDotNode(string path)
        {
            m_path = path;
            m_name = ExtractName(path);
            m_type = RedDotType.Dot;
            m_aggregateStrategy = RedDotAggregateStrategy.Or;
            m_children = new List<RedDotNode>(4);
            m_value = 0;
            m_isDirty = false;
            m_isLeaf = true;
        }

        #endregion

        #region Tree Operations

        /// <summary>
        /// 添加子节点
        /// </summary>
        public void AddChild(RedDotNode child)
        {
            if (child == null || m_children.Contains(child))
                return;

            m_children.Add(child);
            child.m_parent = this;
            m_isLeaf = false;

            MarkDirty();
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        public void RemoveChild(RedDotNode child)
        {
            if (child == null)
                return;

            if (m_children.Remove(child))
            {
                child.m_parent = null;
                m_isLeaf = m_children.Count == 0;
                MarkDirty();
            }
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        public RedDotNode GetChild(int index)
        {
            if (index < 0 || index >= m_children.Count)
                return null;
            return m_children[index];
        }

        /// <summary>
        /// 根据名称获取子节点
        /// </summary>
        public RedDotNode GetChildByName(string name)
        {
            for (int i = 0; i < m_children.Count; i++)
            {
                if (m_children[i].m_name == name)
                    return m_children[i];
            }
            return null;
        }

        #endregion

        #region Value Operations

        /// <summary>
        /// 设置红点数值（仅叶子节点可调用）
        /// </summary>
        public void SetValue(int value)
        {
            if (!m_isLeaf)
            {
                UnityEngine.Debug.LogWarning($"[RedDot] 只有叶子节点可以直接设置数值: {m_path}");
                return;
            }

            value = Math.Max(0, value);
            if (m_value != value)
            {
                m_value = value;
                NotifyValueChanged();
                PropagateToParent();
            }
        }

        /// <summary>
        /// 增加红点数值
        /// </summary>
        public void AddValue(int delta)
        {
            SetValue(m_value + delta);
        }

        /// <summary>
        /// 清除红点
        /// </summary>
        public void Clear()
        {
            SetValue(0);
        }

        #endregion

        #region Dirty Flag and Update

        /// <summary>
        /// 标记为脏
        /// </summary>
        public void MarkDirty()
        {
            if (m_isDirty)
                return;

            m_isDirty = true;
            m_parent?.MarkDirty();
        }

        /// <summary>
        /// 重新计算数值（仅非叶子节点）
        /// </summary>
        public void Recalculate()
        {
            if (!m_isDirty)
                return;

            if (m_isLeaf)
            {
                m_isDirty = false;
                return;
            }

            for (int i = 0; i < m_children.Count; i++)
            {
                m_children[i].Recalculate();
            }

            int newValue = CalculateAggregatedValue();

            if (m_value != newValue)
            {
                m_value = newValue;
                NotifyValueChanged();
            }

            m_isDirty = false;
        }

        /// <summary>
        /// 根据聚合策略计算数值
        /// </summary>
        private int CalculateAggregatedValue()
        {
            switch (m_aggregateStrategy)
            {
                case RedDotAggregateStrategy.Sum:
                    int sum = 0;
                    for (int i = 0; i < m_children.Count; i++)
                        sum += m_children[i].m_value;
                    return sum;

                case RedDotAggregateStrategy.Or:
                    for (int i = 0; i < m_children.Count; i++)
                    {
                        if (m_children[i].m_value > 0)
                            return 1;
                    }
                    return 0;

                case RedDotAggregateStrategy.Max:
                    int max = 0;
                    for (int i = 0; i < m_children.Count; i++)
                        max = Math.Max(max, m_children[i].m_value);
                    return max;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 向上冒泡更新
        /// </summary>
        private void PropagateToParent()
        {
            if (m_parent == null)
                return;

            int newParentValue = m_parent.CalculateAggregatedValue();
            if (m_parent.m_value != newParentValue)
            {
                m_parent.m_value = newParentValue;
                m_parent.NotifyValueChanged();
                m_parent.PropagateToParent();
            }
        }

        #endregion

        #region Event Listeners

        /// <summary>
        /// 添加值变化监听
        /// </summary>
        public void AddListener(Action<RedDotNode> callback)
        {
            m_onValueChanged += callback;
        }

        /// <summary>
        /// 移除值变化监听
        /// </summary>
        public void RemoveListener(Action<RedDotNode> callback)
        {
            m_onValueChanged -= callback;
        }

        /// <summary>
        /// 移除所有监听
        /// </summary>
        public void RemoveAllListeners()
        {
            m_onValueChanged = null;
        }

        /// <summary>
        /// 通知值变化
        /// </summary>
        private void NotifyValueChanged()
        {
            m_onValueChanged?.Invoke(this);
        }

        #endregion

        #region Utility

        private static string ExtractName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            int lastSeparator = path.LastIndexOf('/');
            return lastSeparator >= 0 ? path.Substring(lastSeparator + 1) : path;
        }

        public override string ToString()
        {
            return $"[RedDotNode] Path={m_path}, Value={m_value}, Type={m_type}, IsLeaf={m_isLeaf}, ChildCount={m_children.Count}";
        }

        #endregion
    }
}
