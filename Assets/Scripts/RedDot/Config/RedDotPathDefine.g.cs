// =============================================================================
// AUTO-GENERATED FILE - DO NOT EDIT MANUALLY
// Generated from: Assets/Scripts/RedDot/Config/RedDotTreeConfig.asset
// Generated at: 2026-01-07 18:06:17
// =============================================================================

namespace RedDotSystem
{
    public static class RedDotPaths
    {

        /// <summary>
        /// 主界面
        /// </summary>
        public static class Main
        {
            public const string Path = "Main";
            public static readonly string[] Segments = { "Main" };

            /// <summary>
            /// 邮件系统
            /// </summary>
            public static class Mail
            {
                public const string Path = "Main/Mail";
                public static readonly string[] Segments = { "Main", "Mail" };

                /// <summary>
                /// 系统邮件
                /// </summary>
                public const string System = "Main/Mail/System";
                public static readonly string[] SystemSegments = { "Main", "Mail", "System" };

                /// <summary>
                /// 玩家邮件
                /// </summary>
                public const string Player = "Main/Mail/Player";
                public static readonly string[] PlayerSegments = { "Main", "Mail", "Player" };

                /// <summary>
                /// 公会邮件
                /// </summary>
                public const string Guild = "Main/Mail/Guild";
                public static readonly string[] GuildSegments = { "Main", "Mail", "Guild" };
            }

            /// <summary>
            /// 背包系统
            /// </summary>
            public static class Bag
            {
                public const string Path = "Main/Bag";
                public static readonly string[] Segments = { "Main", "Bag" };

                /// <summary>
                /// 装备
                /// </summary>
                public const string Equipment = "Main/Bag/Equipment";
                public static readonly string[] EquipmentSegments = { "Main", "Bag", "Equipment" };

                /// <summary>
                /// 道具
                /// </summary>
                public const string Item = "Main/Bag/Item";
                public static readonly string[] ItemSegments = { "Main", "Bag", "Item" };

                /// <summary>
                /// 材料
                /// </summary>
                public const string Material = "Main/Bag/Material";
                public static readonly string[] MaterialSegments = { "Main", "Bag", "Material" };
            }

            /// <summary>
            /// 任务系统
            /// </summary>
            public static class Quest
            {
                public const string Path = "Main/Quest";
                public static readonly string[] Segments = { "Main", "Quest" };

                /// <summary>
                /// 主线任务
                /// </summary>
                public const string Main = "Main/Quest/Main";
                public static readonly string[] MainSegments = { "Main", "Quest", "Main" };

                /// <summary>
                /// 日常任务
                /// </summary>
                public const string Daily = "Main/Quest/Daily";
                public static readonly string[] DailySegments = { "Main", "Quest", "Daily" };

                /// <summary>
                /// 周常任务
                /// </summary>
                public const string Weekly = "Main/Quest/Weekly";
                public static readonly string[] WeeklySegments = { "Main", "Quest", "Weekly" };
            }

            /// <summary>
            /// 活动系统
            /// </summary>
            public static class Activity
            {
                public const string Path = "Main/Activity";
                public static readonly string[] Segments = { "Main", "Activity" };

                /// <summary>
                /// 七日活动
                /// </summary>
                public static class SevenDay
                {
                    public const string Path = "Main/Activity/SevenDay";
                    public static readonly string[] Segments = { "Main", "Activity", "SevenDay" };

                    /// <summary>
                    /// 七日活动任务
                    /// </summary>
                    public const string Task = "Main/Activity/SevenDay/Task";
                    public static readonly string[] TaskSegments = { "Main", "Activity", "SevenDay", "Task" };

                    /// <summary>
                    /// 七日活动奖励
                    /// </summary>
                    public const string Reward = "Main/Activity/SevenDay/Reward";
                    public static readonly string[] RewardSegments = { "Main", "Activity", "SevenDay", "Reward" };
                }

                /// <summary>
                /// 签到活动
                /// </summary>
                public static class SignIn
                {
                    public const string Path = "Main/Activity/SignIn";
                    public static readonly string[] Segments = { "Main", "Activity", "SignIn" };

                    /// <summary>
                    /// 每日签到
                    /// </summary>
                    public const string Daily = "Main/Activity/SignIn/Daily";
                    public static readonly string[] DailySegments = { "Main", "Activity", "SignIn", "Daily" };

                    /// <summary>
                    /// 累计签到
                    /// </summary>
                    public const string Cumulative = "Main/Activity/SignIn/Cumulative";
                    public static readonly string[] CumulativeSegments = { "Main", "Activity", "SignIn", "Cumulative" };
                }
            }
        }

        /// <summary>
        /// 社交系统
        /// </summary>
        public static class Social
        {
            public const string Path = "Social";
            public static readonly string[] Segments = { "Social" };

            /// <summary>
            /// 好友
            /// </summary>
            public static class Friends
            {
                public const string Path = "Social/Friends";
                public static readonly string[] Segments = { "Social", "Friends" };

                /// <summary>
                /// 好友申请
                /// </summary>
                public const string Request = "Social/Friends/Request";
                public static readonly string[] RequestSegments = { "Social", "Friends", "Request" };

                /// <summary>
                /// 推荐好友
                /// </summary>
                public const string Recommend = "Social/Friends/Recommend";
                public static readonly string[] RecommendSegments = { "Social", "Friends", "Recommend" };
            }

            /// <summary>
            /// 聊天
            /// </summary>
            public static class Chat
            {
                public const string Path = "Social/Chat";
                public static readonly string[] Segments = { "Social", "Chat" };

                /// <summary>
                /// 世界频道
                /// </summary>
                public const string World = "Social/Chat/World";
                public static readonly string[] WorldSegments = { "Social", "Chat", "World" };

                /// <summary>
                /// 公会频道
                /// </summary>
                public const string Guild = "Social/Chat/Guild";
                public static readonly string[] GuildSegments = { "Social", "Chat", "Guild" };

                /// <summary>
                /// 私聊
                /// </summary>
                public const string Private = "Social/Chat/Private";
                public static readonly string[] PrivateSegments = { "Social", "Chat", "Private" };
            }
        }

        /// <summary>
        /// 注册所有红点节点到 RedDotManager（零 GC）
        /// </summary>
        public static void RegisterAll()
        {
            var mgr = RedDotManager.Instance;

            // 主界面
            mgr.Register(Main.Path, Main.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 邮件系统
            mgr.Register(Main.Mail.Path, Main.Mail.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 系统邮件
            mgr.Register(Main.Mail.System, Main.Mail.SystemSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 玩家邮件
            mgr.Register(Main.Mail.Player, Main.Mail.PlayerSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 公会邮件
            mgr.Register(Main.Mail.Guild, Main.Mail.GuildSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 背包系统
            mgr.Register(Main.Bag.Path, Main.Bag.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 装备
            mgr.Register(Main.Bag.Equipment, Main.Bag.EquipmentSegments, RedDotType.New, RedDotAggregateStrategy.Or);
            // 道具
            mgr.Register(Main.Bag.Item, Main.Bag.ItemSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 材料
            mgr.Register(Main.Bag.Material, Main.Bag.MaterialSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 任务系统
            mgr.Register(Main.Quest.Path, Main.Quest.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 主线任务
            mgr.Register(Main.Quest.Main, Main.Quest.MainSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 日常任务
            mgr.Register(Main.Quest.Daily, Main.Quest.DailySegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 周常任务
            mgr.Register(Main.Quest.Weekly, Main.Quest.WeeklySegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 活动系统
            mgr.Register(Main.Activity.Path, Main.Activity.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 七日活动
            mgr.Register(Main.Activity.SevenDay.Path, Main.Activity.SevenDay.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 七日活动任务
            mgr.Register(Main.Activity.SevenDay.Task, Main.Activity.SevenDay.TaskSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 七日活动奖励
            mgr.Register(Main.Activity.SevenDay.Reward, Main.Activity.SevenDay.RewardSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 签到活动
            mgr.Register(Main.Activity.SignIn.Path, Main.Activity.SignIn.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 每日签到
            mgr.Register(Main.Activity.SignIn.Daily, Main.Activity.SignIn.DailySegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 累计签到
            mgr.Register(Main.Activity.SignIn.Cumulative, Main.Activity.SignIn.CumulativeSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 社交系统
            mgr.Register(Social.Path, Social.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 好友
            mgr.Register(Social.Friends.Path, Social.Friends.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 好友申请
            mgr.Register(Social.Friends.Request, Social.Friends.RequestSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 推荐好友
            mgr.Register(Social.Friends.Recommend, Social.Friends.RecommendSegments, RedDotType.Dot, RedDotAggregateStrategy.Or);
            // 聊天
            mgr.Register(Social.Chat.Path, Social.Chat.Segments, RedDotType.Dot, RedDotAggregateStrategy.Or);

            // 世界频道
            mgr.Register(Social.Chat.World, Social.Chat.WorldSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 公会频道
            mgr.Register(Social.Chat.Guild, Social.Chat.GuildSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
            // 私聊
            mgr.Register(Social.Chat.Private, Social.Chat.PrivateSegments, RedDotType.Number, RedDotAggregateStrategy.Or);
        }
    }
}
