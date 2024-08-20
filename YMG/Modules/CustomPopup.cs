using Epic.OnlineServices.Presence;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using Object = UnityEngine.Object;

namespace YMG;

#nullable enable
public static class CustomPopup
{
    public static GameObject? Fill;
    public static GameObject? InfoScreen;

    public static TextMeshPro? TitleTMP;
    public static TextMeshPro? InfoTMP;

    public static PassiveButton? ActionButtonPrefab;
    public static List<PassiveButton>? ActionButtons;

    private static bool busy = false;

    /// <summary>
    /// 显示一个全屏信息显示界面
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="info">内容</param>
    /// <param name="buttons">按钮（文字，点击事件）</param>
    public static void Show(string title, string info, List<(string, Action)>? buttons)
    {
        if (busy || Fill == null || InfoScreen == null || ActionButtonPrefab == null || TitleTMP == null || InfoTMP == null) Init();

        busy = true;

        TitleTMP.text = title;
        InfoTMP.text = info;
//        if (TitleTMP != null)
//            TitleTMP.transform.localPosition += Vector3.back * 100;
//        if (InfoTMP != null)
  //          InfoTMP.transform.localPosition += Vector3.back * 100;
    //          if (Fill != null)
      //            Fill.transform.localPosition += Vector3.back * 200;
 //       if (InfoScreen != null)
   //        InfoScreen.transform.localPosition += Vector3.back * 200;
     //         if (ActionButtonPrefab != null)
       //           ActionButtonPrefab.transform.localPosition += Vector3.back * 100;
        //

        ActionButtons?.Do(b => Object.Destroy(b.gameObject));
        ActionButtons = new();

        if (buttons != null)
        {
            foreach (var buttonInfo in buttons.Where(b => b.Item1?.Trim() is not null and not ""))
            {
                var (text, action) = buttonInfo;
                var button = Object.Instantiate(ActionButtonPrefab, InfoScreen.transform);
                var tmp = button.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
                tmp.text = text;
  //              if (tmp != null)
    //                tmp.transform.localPosition += Vector3.back * 150;
                button.OnClick = new();
      //          if (button != null)
        //            button.transform.localPosition += Vector3.back * 150;
                button.OnClick.AddListener((Action)(() =>
                {
                    InfoScreen.SetActive(false);
                    Fill.SetActive(false);
                }));
                if (action != null)
                {
                    button.OnClick.AddListener(action);
                }
                button.transform.SetLocalX(0);
                button.gameObject.SetActive(true);
                ActionButtons.Add(button);
            }
        }

        if (ActionButtons.Count > 1)
        {
            float widthSum = ActionButtons.Count * ActionButtonPrefab.gameObject.GetComponent<BoxCollider2D>().size.x;
            widthSum += (ActionButtons.Count - 1) * 0.1f;
            float start = -Math.Abs(widthSum / 2);
            float each = widthSum / ActionButtons.Count;
            int index = 0;
            foreach (var button in ActionButtons)
            {
                button.transform.SetLocalX(start + each * (index + 0.5f));
                index++;
            }
        }

        Fill.SetActive(true);
        InfoScreen.SetActive(true);

        busy = false;
    }
    private static (string title, string info, List<(string, Action)>? buttons)? waitToShow = null;
    public static void ShowLater(string title, string info, List<(string, Action)>? buttons) => waitToShow = (title, info, buttons);
    private static string waitToUpdateText = string.Empty;
    public static void UpdateTextLater(string info) => waitToUpdateText = info;
    public static void Update()
    {
        if (waitToShow != null)
        {
            Show(waitToShow.Value.title, waitToShow.Value.info, waitToShow.Value.buttons);
            waitToShow = null;
        }
        if (!string.IsNullOrEmpty(waitToUpdateText))
        {
            InfoTMP?.SetText(waitToUpdateText);
            waitToUpdateText = string.Empty;
        }
    }
    public static void Init()
    {
        var DOBScreen = AccountManager.Instance.transform.FindChild("DOBEnterScreen");
        if (DOBScreen != null && (Fill == null || InfoScreen == null || ActionButtons == null))
        {
            Fill = Object.Instantiate(DOBScreen.FindChild("Fill").gameObject);
            Fill.transform.SetLocalZ(-100f);
           Fill.name = "YuET Info Popup Fill";
         //   Fill.transform.localPosition += Vector3.back * 150;
           Fill.SetActive(false);

            InfoScreen = Object.Instantiate(DOBScreen.FindChild("InfoPage").gameObject);
            InfoScreen.transform.SetLocalZ(-110f);
    //        InfoScreen.transform.localPosition += Vector3.back * 150;
            InfoScreen.name = "YuET Info Popup Page";
            InfoScreen.SetActive(false);

            TitleTMP = InfoScreen.transform.FindChild("Title Text").GetComponent<TextMeshPro>();
            TitleTMP.transform.localPosition = new(0f, 2.3f, 3f);
        //    TitleTMP.transform.localPosition += Vector3.back * 150;
            TitleTMP.DestroyTranslatorL();
            TitleTMP.text = "";

            InfoTMP = InfoScreen.transform.FindChild("InfoText_TMP").GetComponent<TextMeshPro>();
            InfoTMP.GetComponent<RectTransform>().sizeDelta = new(7f, 1.3f);
            InfoTMP.transform.localScale = new(1f, 1f, 1f);
  //          InfoTMP.transform.localPosition += Vector3.back * 150;
            InfoTMP.DestroyTranslatorL();
            InfoTMP.text = "";

            ActionButtonPrefab = InfoScreen.transform.FindChild("BackButton").GetComponent<PassiveButton>();
            ActionButtonPrefab.gameObject.name = "ActionButtonPrefab";
            ActionButtonPrefab.transform.localScale = new(0.66f, 0.66f, 0.66f);
            ActionButtonPrefab.transform.localPosition = new(0f, -1.35f, 3f);
          //  ActionButtonPrefab.transform.localPosition += Vector3.back * 150;
            ActionButtonPrefab.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>().DestroyTranslatorL();
            ActionButtonPrefab.gameObject.SetActive(false);
        }
    }
}
#nullable disable