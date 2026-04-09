using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedPointSystem
{
    /// <summary>
    /// 红点树节点配置
    /// </summary>
    [Serializable]
    public class RedPointNodeConfig
    {
        [Tooltip("节点名称（用于生成路径）")]
        public string name;

        [Tooltip("节点描述")]
        public string description;

        [Tooltip("红点类型")]
        public RedPointType type = RedPointType.Dot;

        [Tooltip("聚合策略")]
        public RedPointAggregateStrategy strategy = RedPointAggregateStrategy.Or;

        [Tooltip("子节点")]
        [SerializeReference]
        public List<RedPointNodeConfig> children = new List<RedPointNodeConfig>();

        [HideInInspector]
        public bool foldout = true;

        [HideInInspector]
        public string generatedPath;

        [HideInInspector]
        public int generatedId;

        public RedPointNodeConfig()
        {
            name = "NewNode";
        }

        public RedPointNodeConfig(string nodeName)
        {
            name = nodeName;
        }

        /// <summary>
        /// 递归生成路径
        /// </summary>
        public void GeneratePaths(string parentPath = "")
        {
            generatedPath = string.IsNullOrEmpty(parentPath) ? name : $"{parentPath}/{name}";

            foreach (var child in children)
            {
                child.GeneratePaths(generatedPath);
            }
        }

        /// <summary>
        /// 获取所有节点（扁平化）
        /// </summary>
        public void GetAllNodes(List<RedPointNodeConfig> result)
        {
            result.Add(this);
            foreach (var child in children)
            {
                child.GetAllNodes(result);
            }
        }
    }

    /// <summary>
    /// 红点树配置 - ScriptableObject
    /// </summary>
    [CreateAssetMenu(menuName = "RedPointSystem/TreeConfig", fileName = "RedPointTreeConfig")]
    public class RedPointTreeConfig : ScriptableObject
    {
        [Header("红点树根节点")]
        [SerializeField]
        private List<RedPointNodeConfig> m_rootNodes = new List<RedPointNodeConfig>();

        [Header("代码生成设置")]
        [Tooltip("生成的脚本路径（相对于Assets）")]
        [SerializeField]
        private string m_outputPath = "Scripts/RedPoint/Config/RedPointPathDefine.g.cs";

        [Tooltip("生成的命名空间")]
        [SerializeField]
        private string m_namespace = "RedPointSystem";

        [Tooltip("生成的类名")]
        [SerializeField]
        private string m_className = "RedPointPaths";

        /// <summary>
        /// 根节点列表
        /// </summary>
        public List<RedPointNodeConfig> RootNodes => m_rootNodes;

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath => m_outputPath;

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace => m_namespace;

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName => m_className;

        /// <summary>
        /// 刷新所有节点的路径
        /// </summary>
        public void RefreshPaths()
        {
            foreach (var root in m_rootNodes)
            {
                root.GeneratePaths();
            }
        }

        /// <summary>
        /// 获取所有节点
        /// </summary>
        public List<RedPointNodeConfig> GetAllNodes()
        {
            var result = new List<RedPointNodeConfig>();
            foreach (var root in m_rootNodes)
            {
                root.GetAllNodes(result);
            }
            return result;
        }

        /// <summary>
        /// 注册所有节点到红点管理器
        /// </summary>
        public void RegisterAll()
        {
            RefreshPaths();
            var manager = RedPointMgr.Instance;
            var allNodes = GetAllNodes();

            foreach (var node in allNodes)
            {
                if (!string.IsNullOrEmpty(node.generatedPath))
                {
                    manager.Register(node.generatedPath, node.type, node.strategy);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshPaths();
        }
#endif
    }
}