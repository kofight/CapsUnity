using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReuseableRes
{
    public float freeStateStartTime;
    public bool bFree;
    public string Name;
    public virtual void Update() { }
    public virtual bool IsFinish() { return false; }
    public virtual ReuseableRes Clone() { return null; }
    public virtual void LoadRes() { }
}

public class S5ReuseableResManager<TResTypeName> where TResTypeName : ReuseableRes, new()
{
    float m_resKeepTime = 20.0f;														//How long manager keep the res
	float m_curTime = 0.0f;															    //Current time
	readonly float m_updateInterval = 1.0f;                                                     //Interval
    float mLastUpdateTime = 0.0f;
    Dictionary<string, ReuseableRes> m_preLoadedResMap = new Dictionary<string, ReuseableRes>();
    Dictionary<string, HashSet<ReuseableRes>> m_resList = new Dictionary<string, HashSet<ReuseableRes>>();
    Dictionary<string, HashSet<ReuseableRes>> m_freeResPoolMap = new Dictionary<string, HashSet<ReuseableRes>>();

	public void SetKeepResTime(float keepTime)
	{
		m_resKeepTime = keepTime;
	}

    bool PutResToDictionary(ReuseableRes res, Dictionary<string, HashSet<ReuseableRes>> map)
    {
        HashSet<ReuseableRes> hashSet;
        if (!map.TryGetValue(res.Name, out hashSet))
        {
            hashSet = new HashSet<ReuseableRes>();

            map.Add(res.Name, hashSet);
        }
        return hashSet.Add(res);
    }
	
	public void Update()
	{        
        m_curTime = Time.realtimeSinceStartup;
        if (m_curTime > mLastUpdateTime + m_updateInterval)
        {
            mLastUpdateTime = m_curTime;
        }
        foreach (KeyValuePair<string, HashSet<ReuseableRes>> ResSet in m_resList)
        {
            foreach (ReuseableRes res in ResSet.Value)
            {
                if (res.bFree)
                {
                    continue;
                }
                res.Update();
                if (res.IsFinish())
                {
                    res.bFree = true;
                    PutResToDictionary(res, m_freeResPoolMap);
                }
            }
        }

        Dictionary<string, HashSet<ReuseableRes>>.Enumerator mapIter = m_freeResPoolMap.GetEnumerator();
        while(mapIter.MoveNext())
        {
            HashSet<ReuseableRes> hashSet = mapIter.Current.Value;
            HashSet<ReuseableRes>.Enumerator hashIter = hashSet.GetEnumerator();
            while(hashIter.MoveNext())
            {
                ReuseableRes res = hashIter.Current;
                if (res.freeStateStartTime + m_resKeepTime < m_curTime)
                {
                    hashSet.Remove(res);            //Delete elem
                }
                if (!hashIter.MoveNext())
                {
                    break;
                }
            }
        }
	}

    bool AddPreLoadedRes(string resName)
	{
        //std::map<std::string, AsResPtr>::iterator iter = m_preLoadedResMap.find(resName);
        //if (iter != m_preLoadedResMap.end())
        //{
        //    return false;
        //}
        //AsResPtr pRes = LoadRes(resName);
        //PredicateType predicator;
        //predicator.StopRes(pRes);
        //m_preLoadedResMap.insert(std::make_pair(resName, pRes));
        return true;
	}

	bool RemovePreLoadedRes(string resName)
	{
        //std::map<std::string, AsResPtr>::iterator iter = m_preLoadedResMap.find(resName);
        //if (iter == m_preLoadedResMap.end())
        //{
        //    return false;
        //}
        //m_preLoadedResMap.erase(iter);
		return true;
	}

    void AddLoadedRes(ReuseableRes pRes)
	{
		if (pRes == null)
			return;
        pRes.freeStateStartTime = m_curTime;
        pRes.bFree = true;
        PutResToDictionary(pRes, m_resList);
        PutResToDictionary(pRes, m_freeResPoolMap);
	}

    public void Clear()
    {
        m_resList.Clear();
        m_freeResPoolMap.Clear();
        m_preLoadedResMap.Clear();
    }

	TResTypeName LoadRes(string resName)
	{
        TResTypeName res = null;
        //Try to find res in freePool
        HashSet<ReuseableRes> hashSet;
        if (m_freeResPoolMap.TryGetValue(resName, out hashSet))
        {
            if (hashSet.Count > 0)
            {
                res = hashSet.GetEnumerator().Current as TResTypeName;
                res.bFree = false;              //set it not free
                hashSet.Remove(res);            //Remove from free pool
                return res;
            }
        }

        //Try to find res in resList, to clone from
        if (m_resList.TryGetValue(resName, out hashSet))
        {
            if (hashSet.Count > 0)
            {
                TResTypeName prototype = hashSet.GetEnumerator().Current as TResTypeName;
                res = prototype.Clone() as TResTypeName;            //Clone a new obj
            }
        }

        //Cant find prototype to clone, load res
        if (res == null)
        {
            res = new TResTypeName();
            res.Name = resName;
            res.LoadRes();
        }
        return res;
	}

    bool FreeRes(TResTypeName res)
    {
        res.bFree = true;
        return PutResToDictionary(res, m_freeResPoolMap);
    }
}
