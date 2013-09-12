﻿using UnityEngine;
using System.Collections;
using System.IO;

enum ByPackageEnum
{
    ByPackage_UI,
    ByPackage_Icon,
    ByPackage_Model,
}

public class ResManager : ResourceManager
{
    #region Singleton
    public static ResManager Singleton { get; private set; }
    public ResManager()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion

    public GameObject GetUIPrefabByName(string name)        //装载UI资源
    {
        return LoadResource<GameObject>((int)ByPackageEnum.ByPackage_UI, name);
    }

    public Texture GetIconByName(string name)        //装载UI资源
    {
        return LoadResource<Texture>((int)ByPackageEnum.ByPackage_UI, name);
    }

    public string LoadTextFile(string filePath)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
            case RuntimePlatform.OSXWebPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    TextAsset XMLFile = (TextAsset)Resources.Load(filePath);    //Load from asset
                    return XMLFile.text;
                }

        }
        return string.Empty;
    }

    public string GetBasePath()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor
           || Application.platform == RuntimePlatform.OSXEditor)
        {
            string path = Application.dataPath;
            int index = path.LastIndexOf('/');
            path = path.Substring(0, index);
            index = path.LastIndexOf('/');
            path = path.Substring(0, index);
            return path + "/";
        }
        else
        {
            return Application.dataPath + "/";
        }
    }
}
