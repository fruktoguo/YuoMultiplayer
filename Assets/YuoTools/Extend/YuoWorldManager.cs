using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuoTools.Extend;

public class YuoWorldManager : MonoBehaviour
{
    private void Awake()
    {
        WorldMono.WorldInitBeforeSceneLoad();
    }
}
