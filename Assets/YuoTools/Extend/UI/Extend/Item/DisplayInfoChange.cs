using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.UI;

public class DisplayInfoChange : MonoBehaviour
{
    private YuoDropDown dropDown;

    public YuoDropDown DropDown
    {
        get => dropDown.YuoGet(this);
        set => dropDown = value;
    }

    private List<(string key, Vector2Int value)> infos = new();

    private string GetSizeString(Vector2Int size) => $"{size.x}*{size.y}";

    private void Start()
    {
        DropDown.Clear();

        var resolutions = Screen.resolutions;
        
        List<Vector2Int> screenSize = new List<Vector2Int>();
        foreach (var item in resolutions)
        {
            var size = new Vector2Int(item.width, item.height);
            if (!screenSize.Contains(size))
            {
                screenSize.Add(size);
            }
        }

        var nowSize = new Vector2Int(Screen.width, Screen.height);

        if (!screenSize.Contains(nowSize))
        {
            screenSize.Add(nowSize);
        }

        //分辨率从大到小排序
        screenSize.Sort((x, y) => y.x * y.y - x.x * x.y);

        foreach (var item in screenSize)
        {
            var sizeName = GetSizeString(item);
            if (!DropDown.ContainsItem(sizeName))
            {
                DropDown.AddItem(sizeName);
                infos.Add((sizeName, item));
            }
        }

        DropDown.OnValueChanged.AddListener(x =>
        {
            Screen.SetResolution(infos[x.index].value.x, infos[x.index].value.y,
                Screen.fullScreenMode);
        });

        var now = GetSizeString(nowSize);
        if (DropDown.ContainsItem(now))
        {
            DropDown.SetItem(now);
        }
    }
}