using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannerPanelEnergySlot : MonoBehaviour
{
    private int point;
    private int color;
    private string _powertype;
    const string color_key = "TutorialMod/SH_Tiny/Args/1";   //Used to save the arguments of the skill
    const string point_key = "TutorialMod/SH_Tiny/Args/2";
    public TextMeshProUGUI pointText;
    public Image colorImage;
    private ShipData shipData;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Init(ShipData shipData)
    {
        this.shipData = shipData;
        point = -1;
        pointText = GetComponentInChildren<TextMeshProUGUI>();
        
        color = -1;
        colorImage = GetComponentInChildren<Image>();

        GetComponent<HoverTipsProperty>().des = "TutorialMod/BannerHoverTips/Tiny";

        // remove color mask
        ColorBlock cb = GetComponent<SelectableExtension>().colors;
        cb.normalColor = Color.white;
        GetComponent<SelectableExtension>().colors = cb;
    }

    void Update()
    {
        if (shipData == null)
            return;
        if (point != Define.defineValue.eventValuesGlobal.GetValueOrDefault(point_key, 1))
        {
            point = Define.defineValue.eventValuesGlobal.GetValueOrDefault(point_key, 1);
            pointText.text = point.ToString();
            UIManager.Instance().ImageEffect(colorImage);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        string powertype = "";
        if (Define.systemValue.useCatPower) powertype = "cat_";
        if (Define.systemValue.useCatPawPower) powertype = "claw_";

        if (color != Define.defineValue.eventValuesGlobal.GetValueOrDefault(color_key, 1) || powertype != _powertype)
        {
            color = Define.defineValue.eventValuesGlobal.GetValueOrDefault(color_key, 1);
            _powertype = powertype;
            colorImage.sprite = color switch
            {
                3 => ResourcesManager.Instance().Load<Sprite>(FilePath.spriteCommon + "power_" + powertype + "purple"),
                2 => ResourcesManager.Instance().Load<Sprite>(FilePath.spriteCommon + "power_" + powertype + "blue"),
                _ => ResourcesManager.Instance().Load<Sprite>(FilePath.spriteCommon + "power_" + powertype + "grey"),
            };
            UIManager.Instance().ImageEffect(colorImage);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}
