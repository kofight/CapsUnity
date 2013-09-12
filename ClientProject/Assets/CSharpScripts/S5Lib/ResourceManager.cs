using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum UseFilePath
{
    WebBundlePath = 0,						//Bundle Assets On Web
    LocalBundlePath = 1,					//Local Bundle Assets
    LocalPath = 2							//Local Path Without Bundle
}


public class ResourceManager
{
    public event System.Action<int, float> OnLoadProgressEvent;

    public IEnumerator LoadPackage(string url, int index)         //装载包
    {
        WWW newPackage = null;
        if (CapsConfig.Instance.useFilePath == UseFilePath.LocalBundlePath)
        {
            newPackage = WWW.LoadFromCacheOrDownload(url, 0);
        }
        else if (CapsConfig.Instance.useFilePath == UseFilePath.WebBundlePath)
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

        if (CapsConfig.Instance.useFilePath == UseFilePath.LocalPath
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
