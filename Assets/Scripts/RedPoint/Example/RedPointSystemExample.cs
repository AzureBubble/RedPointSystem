using System;
using System.Collections.Generic;
using RedPointSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 红点系统可视化测试
/// 运行时自动创建 UGUI 界面显示红点树结构
/// </summary>
public class RedPointSystemExample : MonoBehaviour
{
    private List<NodeUIItem> m_items = new List<NodeUIItem>();
    public Transform m_nodeContent;

    private void Awake()
    {
        RedPointPaths.RegisterAll();

        for (int i = 0; i < m_nodeContent.childCount; i++)
        {
            var child = m_nodeContent.GetChild(i);
            m_items.Add(child.GetComponent<NodeUIItem>());
        }

        m_items[0].Init(RedPointPaths.Main.Id);
        m_items[1].Init(RedPointPaths.Main.Mail.Id);
        m_items[2].Init(RedPointPaths.Main.Mail.System);
        m_items[3].Init(RedPointPaths.Main.Mail.Player);
        m_items[4].Init(RedPointPaths.Main.Mail.Guild);

        m_items[5].Init(RedPointPaths.Main.Bag.Id);
        m_items[6].Init(RedPointPaths.Main.Bag.Equipment);
        m_items[7].Init(RedPointPaths.Main.Bag.Item);
        m_items[8].Init(RedPointPaths.Main.Bag.Material);

        m_items[9].Init(RedPointPaths.Social.Id);
        m_items[10].Init(RedPointPaths.Social.Friends.Id);
        m_items[11].Init(RedPointPaths.Social.Friends.Request);
        m_items[12].Init(RedPointPaths.Social.Friends.Recommend);

        m_items[13].Init(RedPointPaths.Social.Chat.Id);
        m_items[14].Init(RedPointPaths.Social.Chat.World);
        m_items[15].Init(RedPointPaths.Social.Chat.Guild);
        m_items[16].Init(RedPointPaths.Social.Chat.Private);
    }
}