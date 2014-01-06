using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UseFilePath
{
    WebBundlePath = 0,						//Bundle Assets On Web
    LocalBundlePath = 1,					//Local Bundle Assets
    LocalPath = 2							//Local Path Without Bundle
}

enum ByPackageEnum
{
    ByPackage_UI,
    ByPackage_Icon,
    ByPackage_Model,
}

public class ResourceManager
{
    #region Singleton
    public static ResourceManager Singleton { get; private set; }
    public ResourceManager()
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
                    if (XMLFile == null)
                    {
                        return string.Empty;
                    }
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

    public event System.Action<int, float> OnLoadProgressEvent;
    public UseFilePath useFilePath = UseFilePath.LocalPath;

    public IEnumerator LoadPackage(string url, int index)         //装载包
    {
        WWW newPackage = null;
        if (useFilePath == UseFilePath.LocalBundlePath)
        {
            newPackage = WWW.LoadFromCacheOrDownload(url, 0);
        }
        else if (useFilePath == UseFilePath.WebBundlePath)
        {
            newPackage = new WWW(url);
        }
        else
        {
            yield return 0;
        }
        m_packageMap.Add(index, newPackage);
        while (!m_packageMap[index].isDone)                     //若装载没结束
        {
            if (OnLoadProgressEvent != null)
            {
                OnLoadProgressEvent(index, m_packageMap[index].progress);
            }
            yield return 0;
        }
        yield return 0;
    }

    public T LoadResource<T>(int index, string name) where T : UnityEngine.Object
    {
        WWW loadBundleModel = null;
        m_packageMap.TryGetValue(index, out loadBundleModel);

        if (useFilePath == UseFilePath.LocalPath
            || loadBundleModel == null)		//Load Directly
        {
            return (Resources.Load(name, typeof(T)) as T);
        }
        else //Load From Bundle Asset
        {
            if (loadBundleModel.assetBundle.Contains(name))
            {
                return loadBundleModel.assetBundle.Load(name, typeof(T)) as T;
            }
            return null;
        }
    }

    Dictionary<int, WWW> m_packageMap = new Dictionary<int, WWW>();
}
