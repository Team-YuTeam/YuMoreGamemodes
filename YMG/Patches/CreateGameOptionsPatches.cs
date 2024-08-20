using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using static YMG.Translator;

namespace YMG;

internal static class CreateGameOptionsPatches
{
    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Start))]
    public static class CreateOptionsPicker_Start
    {
        public static void Postfix()
        {
            var template = GameObject.Find("ChatTypeOptionsCompact");
            var ChatTypeOptionsCompact = GameObject.Find("ChatTypeOptionsCompact");
            var Language_Selection = GameObject.Find("Language Selection");
            var WhiteDivider = GameObject.Find("WhiteDivider");
            var Cancel = GameObject.Find("Cancel");
            var RoomProtocolOptionsCompact = Object.Instantiate(template, template.transform.parent);
            WhiteDivider.transform.localScale += new Vector3(0.3f, 0, 0);
            WhiteDivider.GetComponent<SpriteRenderer>().color = new Color32(61, 255, 141,255);
            RoomProtocolOptionsCompact.transform.localPosition += new Vector3(4.2f, 0, 0);
            ChatTypeOptionsCompact.transform.localPosition -= new Vector3(0.9f, 0, 0);
            Language_Selection.transform.localPosition -= new Vector3(1.15f, 0, 0);
            Cancel.transform.localPosition += new Vector3(7f, 0, 0);
            RoomProtocolOptionsCompact.name = "RoomProtocolOptionsCompact";
            var OldBackground = RoomProtocolOptionsCompact.transform.FindChild("Background");
            var OldText_TMP = OldBackground.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
            OldText_TMP.text = GetString("NormalProtocal");
            PassiveButton passiveButton = RoomProtocolOptionsCompact.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener(new Action(() =>
            {
                Main.isModProtocol = !Main.isModProtocol;
                OldText_TMP.text = Main.isModProtocol ? GetString("ModProtocal") : GetString("NormalProtocal");
            }));
            new LateTask(() => { RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>().text = GetString("RoomProtocal"); }, 0.01f, "RoomModeTipsSet");
        }
    }

}