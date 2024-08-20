using HarmonyLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace YMG;

# pragma warning disable CA1416
public static class RegistryManager
{
    public static RegistryKey SoftwareKeys => Registry.CurrentUser.OpenSubKey("Software", true);
    public static RegistryKey Keys = SoftwareKeys.OpenSubKey("AU-YMG", true);
    public static Version LastVersion;

    public static void Init()
    {
        if (Keys == null)
        {
            Logger.Info("Create YMG Registry Key", "Registry Manager");
            Keys = SoftwareKeys.CreateSubKey("AU-YMG", true);
        }
        if (Keys == null)
        {
            Logger.Error("Create Registry Failed", "Registry Manager");
            return;
        }

        if (Keys.GetValue("Last launched version") is not string regLastVersion)
            LastVersion = new Version(0, 0, 0);
        else LastVersion = Version.Parse(regLastVersion);

        Keys.SetValue("Last launched version", Main.version.ToString());
        Keys.SetValue("Path", Path.GetFullPath("./"));
    }
}