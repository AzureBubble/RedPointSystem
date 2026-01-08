using System;
using RedPointSystem;
using UnityEngine;
using UnityEngine.UI;

public class NodeUIItem : MonoBehaviour
{
    public RectTransform Root;
    public Text NameText;
    public Text ValueText;
    public Image RedDotIcon;
    public Button AddButton;
    public Button SubButton;
    public Button ClearButton;
    public RedPointNode Node;
    public Text TypeText;

    private RedPointNode m_node;

    private void Awake()
    {
        // Root = transform.Find("Root").GetComponent<RectTransform>();
        NameText = transform.Find("Name")?.GetComponent<Text>();
        ValueText = transform.Find("Value")?.GetComponent<Text>();
        RedDotIcon = transform.Find("Icon")?.GetComponent<Image>();
        AddButton = transform.Find("Add")?.GetComponent<Button>();
        AddButton?.onClick.AddListener(() =>
        {
            RedPointMgr.Instance.AddValue(m_node.Id, 1);
        });
        SubButton = transform.Find("Sub")?.GetComponent<Button>();
        SubButton?.onClick.AddListener(() =>
        {
            RedPointMgr.Instance.AddValue(m_node.Id, -1);
        });
        ClearButton = transform.Find("Clear")?.GetComponent<Button>();
        ClearButton?.onClick.AddListener(() =>
        {
            RedPointMgr.Instance.SetValue(m_node.Id, 0);
        });
        transform?.Find("Type")?.TryGetComponent<Text>(out TypeText);

        ValueText.text = 0.ToString();
        ValueText.gameObject.SetActive(false);
        RedDotIcon.gameObject.SetActive(false);
    }

    public void Init(int id)
    {
        var node = RedPointMgr.Instance.GetNode(id);
        if (node == null)
        {
            return;
        }

        m_node = node;

        NameText.text = m_node.Name;
        if(TypeText!=null)
            TypeText.text = node.AggregateStrategy.ToString();

        if (node.Type == RedPointType.Number)
        {
            ValueText.text = node.Value.ToString();
        }
        else
        {
            ValueText.text = 1.ToString();
        }

        ValueText.gameObject.SetActive(node.IsShow);
        RedDotIcon.gameObject.SetActive(node.IsShow);
        node.AddListener(Refresh);
    }


    void Refresh(RedPointNode node)
    {
        if (node != null)
        {
            NameText.text = node.Name;
            if(TypeText!=null)
                TypeText.text = node.AggregateStrategy.ToString();
            if (node.Type == RedPointType.Number)
            {
                ValueText.text = node.Value.ToString();
            }
            else
            {
                ValueText.text = 1.ToString();
            }
            ValueText.gameObject.SetActive(node.IsShow);
            RedDotIcon.gameObject.SetActive(node.IsShow);
        }
    }
}