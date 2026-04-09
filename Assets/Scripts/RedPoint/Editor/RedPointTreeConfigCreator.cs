using UnityEditor;
using UnityEngine;

namespace RedPointSystem.Editor
{
    /// <summary>
    /// 红点树配置创建向导
    /// </summary>
    public static class RedPointTreeConfigCreator
    {
        [MenuItem("Tools/RedPoint/Create Sample Config")]
        public static void CreateSampleConfig()
        {
            // 创建配置资源
            var config = ScriptableObject.CreateInstance<RedPointTreeConfig>();

            // 添加示例树结构
            var main = new RedPointNodeConfig("Main") { description = "主界面" };
            config.RootNodes.Add(main);

            // 邮件系统
            var mail = new RedPointNodeConfig("Mail") { description = "邮件系统" };
            mail.children.Add(new RedPointNodeConfig("System") { description = "系统邮件" });
            mail.children.Add(new RedPointNodeConfig("Player") { description = "玩家邮件" });
            mail.children.Add(new RedPointNodeConfig("Guild") { description = "公会邮件" });
            main.children.Add(mail);

            // 背包系统
            var bag = new RedPointNodeConfig("Bag") { description = "背包系统" };
            bag.children.Add(new RedPointNodeConfig("Equipment") { description = "装备", type = RedPointType.New });
            bag.children.Add(new RedPointNodeConfig("Item") { description = "道具", type = RedPointType.Number });
            bag.children.Add(new RedPointNodeConfig("Material") { description = "材料" });
            main.children.Add(bag);

            // 任务系统
            var quest = new RedPointNodeConfig("Quest") { description = "任务系统" };
            quest.children.Add(new RedPointNodeConfig("Main") { description = "主线任务" });
            quest.children.Add(new RedPointNodeConfig("Daily") { description = "日常任务", type = RedPointType.Number });
            quest.children.Add(new RedPointNodeConfig("Weekly") { description = "周常任务", type = RedPointType.Number });
            main.children.Add(quest);

            // 活动系统
            var activity = new RedPointNodeConfig("Activity") { description = "活动系统" };

            var sevenDay = new RedPointNodeConfig("SevenDay") { description = "七日活动" };
            sevenDay.children.Add(new RedPointNodeConfig("Task") { description = "七日活动任务" });
            sevenDay.children.Add(new RedPointNodeConfig("Reward") { description = "七日活动奖励" });
            activity.children.Add(sevenDay);

            var signin = new RedPointNodeConfig("SignIn") { description = "签到活动" };
            signin.children.Add(new RedPointNodeConfig("Daily") { description = "每日签到" });
            signin.children.Add(new RedPointNodeConfig("Cumulative") { description = "累计签到" });
            activity.children.Add(signin);

            main.children.Add(activity);

            // 社交系统
            var social = new RedPointNodeConfig("Social") { description = "社交系统" };
            config.RootNodes.Add(social);

            var friends = new RedPointNodeConfig("Friends") { description = "好友" };
            friends.children.Add(new RedPointNodeConfig("Request") { description = "好友申请", type = RedPointType.Number });
            friends.children.Add(new RedPointNodeConfig("Recommend") { description = "推荐好友" });
            social.children.Add(friends);

            var chat = new RedPointNodeConfig("Chat") { description = "聊天" };
            chat.children.Add(new RedPointNodeConfig("World") { description = "世界频道", type = RedPointType.Number });
            chat.children.Add(new RedPointNodeConfig("Guild") { description = "公会频道", type = RedPointType.Number });
            chat.children.Add(new RedPointNodeConfig("Private") { description = "私聊", type = RedPointType.Number });
            social.children.Add(chat);

            // 刷新路径
            config.RefreshPaths();

            // 保存资源
            string path = "Assets/Scripts/RedPoint/Config/RedPointTreeConfig.asset";
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();

            // 选中并打开编辑器
            Selection.activeObject = config;
            RedPointTreeEditorWindow.ShowWindow();

            Debug.Log($"[RedPoint] Sample config created at: {path}");
        }
    }
}