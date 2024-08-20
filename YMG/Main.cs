using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YMG.Attributes;
using YMG.Utils;

[assembly: AssemblyFileVersion(YMG.Main.PluginVersion)]
[assembly: AssemblyInformationalVersion(YMG.Main.PluginVersion)]
[assembly: AssemblyVersion(YMG.Main.PluginVersion)]
namespace YMG;

[BepInPlugin(PluginGuid, "YMG", PluginVersion)]
[BepInProcess("Among Us.exe")]
public class Main : BasePlugin
{

    public static readonly string ModName = "YMG"; // 咱们的模组名字
    public static readonly string ModColor = "#3DFFBB"; // 咱们的模组颜色
    public static readonly string MainMenuText = "I LOVE EVERYONE~"; // 咱们模组的首页标语
    public const string PluginGuid = "com.Yu.YMG"; //咱们模组的Guid
    public const string PluginVersion = "0.0.1.0"; //咱们模组的版本号
    public const string CanUseInAmongUsVer = "2024.8.13"; //智齿的AU版本
    public const int PluginCreation = 1;

    public static string QQUrl = "https://qm.qq.com/q/uGuWqBkYUi";
    public static string DcUrl = "https://discord.gg/42tyx9FyD7";

    public static bool HasHacker = false;
    
    public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV08 HideNSeekOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;
    
    public static ConfigEntry<bool> SwitchVanilla; // 切换为原版
    
    public static List<string> TName_Snacks_CN = new() { "冰激凌", "奶茶", "巧克力", "蛋糕", "甜甜圈", "可乐", "柠檬水", "冰糖葫芦", "果冻", "糖果", "牛奶", "抹茶", "烧仙草", "菠萝包", "布丁", "椰子冻", "曲奇", "红豆土司", "三彩团子", "艾草团子", "泡芙", "可丽饼", "桃酥", "麻薯", "鸡蛋仔", "马卡龙", "雪梅娘", "炒酸奶", "蛋挞", "松饼", "西米露", "奶冻", "奶酥", "可颂", "奶糖" ,"咔皮呆","Yu","Night瓜","慕斯Mousse","毒液","Slok7565","喜"};
    public static List<string> TName_Snacks_EN = new() { "Ice cream", "Milk tea", "Chocolate", "Cake", "Donut", "Coke", "Lemonade", "Candied haws", "Jelly", "Candy", "Milk", "Matcha", "Burning Grass Jelly", "Pineapple Bun", "Pudding", "Coconut Jelly", "Cookies", "Red Bean Toast", "Three Color Dumplings", "Wormwood Dumplings", "Puffs", "Can be Crepe", "Peach Crisp", "Mochi", "Egg Waffle", "Macaron", "Snow Plum Niang", "Fried Yogurt", "Egg Tart", "Muffin", "Sago Dew", "panna cotta", "soufflé", "croissant", "toffee" ,"KARPED1EM","Yu","Night-GUA","Mousse","Farewell","Slok7565","Xi"};
    
    public static System.Version version = System.Version.Parse(PluginVersion);
        
    public static int ModMode { get; private set; } =
#if DEBUG
0;
#elif CANARY
        1;
    #else
    2;
#endif
    public Harmony Harmony { get; } = new Harmony(PluginGuid);

    public static BepInEx.Logging.ManualLogSource Logger;
    
    public static IEnumerable<PlayerControl> AllPlayerControls => PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    public static List<PlayerControl> ClonePlayerControlsOnStart => AllPlayerControls.ToList();
    
    public static Main Instance; //设置Main实例
    
    public static bool IsChineseUser => Translator.GetUserLangByRegion() == SupportedLangs.SChinese;
    
    public static bool VisibleTasksCount = false;
    
    public static List<(string, byte, string)> MessagesToSend = new();
    public static List<PlayerControl> HackerList = new();

    public static IEnumerable<PlayerControl> AllAlivePlayerControls =>
        PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    public static ConfigEntry<string> BetaBuildURL { get; private set; }
    public override void Load()//加载 启动！
    {
        Instance = this; //Main实例
    
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.dll",
            "YMG.Resources.InDLL.Depends.YamlDotNet.dll");
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.xml",
            "YMG.Resources.InDLL.Depends.YamlDotNet.xml");
        
        PluginModuleInitializerAttribute.InitializeAll();
        
        Logger = BepInEx.Logging.Logger.CreateLogSource("YMG"); //输出前缀 设置！
        YMG.Logger.Enable();
        
        SwitchVanilla = Config.Bind("Client Options", "SwitchVanilla", false);
        BetaBuildURL = Config.Bind("Other", "BetaBuildURL", "");
        
        if (Application.version == CanUseInAmongUsVer)
            Logger.LogInfo($"AmongUs Version: {Application.version}"); //牢底居然有智齿的版本？！
        else
            Logger.LogInfo($"游戏本体版本过低或过高,AmongUs Version: {Application.version}"); //牢底你的版本也不行啊
        
        RegistryManager.Init(); // 这是优先级最高的模块初始化方法，不能使用模块初始化属性
        
        //各组件初始化
        Harmony.PatchAll();
        if (ModMode != 0) ConsoleManager.DetachConsole();
        else ConsoleManager.CreateConsole();
        
        DevManager.Init();
        //模组加载好了标语
        YMG.Logger.Msg("========= YMG loaded! =========", "YMG Plugin Load");
    }
}

public enum RoleTeam
{
    Crewmate,
    Impostor,
    Error
}

public enum RoomProtocol
{
    Normal,
    Plus25,
    Error
}

public enum GameMdode
{
    Normal,
    KickJar,
}