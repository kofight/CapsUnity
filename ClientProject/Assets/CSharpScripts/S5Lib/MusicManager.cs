using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MusicManager
{
    public static MusicManager Instance;

    private Dictionary<string, GameObject> m_musicdict;

    private string m_languagetype = "cn_";
    private System.Random _random;
    public MusicManager()
    {
        if (Instance == null)
        {
            Instance = this;
            _random = new System.Random();
            m_musicdict = new Dictionary<string, GameObject>();
            Init();
        }
        else
            throw new System.Exception("重复的实例!");
    }

    /*public void SetLanguage(LanguageType language)
    {
        PlayerPrefs.SetInt("golflanguage", (int)language);
        m_musicdict.Clear();
        switch (language)
        {
            case LanguageType.中文:
                m_languagetype = "cn_";
                break;
            case LanguageType.英文:
                m_languagetype = "en_";
                break;
        }
    }*/

    private void Init()
    {
        m_musicdict.Clear();
        //默认是0,即是中文.
        LanguageType golflanguage = (LanguageType)PlayerPrefs.GetInt("golflanguage");
        switch (golflanguage)
        {
            case LanguageType.Chinese:
                m_languagetype = "cn_";
                break;
            case LanguageType.English:
                m_languagetype = "en_";
                break;
        }
    }

    public void Dispose()
    {
        List<string> removekeys = new List<string>();
        foreach (var key in m_musicdict.Keys)
        {
            if (m_musicdict[key] == null)
                continue;
            AudioSource audio = m_musicdict[key].GetComponent<AudioSource>();
            if (audio.isPlaying == false)
                removekeys.Add(key);
        }
        foreach (var key in removekeys)
        {
            UnityEngine.Object.Destroy(m_musicdict[key]);
            m_musicdict.Remove(key);
        }
    }

    /// <summary>
    /// 播放音乐,如果random = true 则随机在已有的3个音源中选一个播放.
    /// </summary>
    /// <param name="musicname"></param>
    /// <param name="random">随机播放(必须要有3个以上音源文件才可以随机播放，否则会报错.)</param>
    public float Play(string musicname, float volume, bool random, int max = 4)
    {
        string key = string.Empty;
        if (random)
            key = m_languagetype + musicname + _random.Next(1, max);
        else
            key = m_languagetype + musicname;
        AudioClip clip = Resources.Load(key, typeof(AudioClip)) as AudioClip;
        //如果按语言区分后没有找到音源，那么不按语言区分再寻找一次.
        if (clip == null)
        {
            if (random)
                key = musicname + _random.Next(1, 4);

            else
                key = musicname;
            clip = Resources.Load(key, typeof(AudioClip)) as AudioClip;
            if (clip == null)
                return 0;
        }

        AudioSource audio = null;
        if (!m_musicdict.ContainsKey(key))
        {
            GameObject music = GameObject.Instantiate(Resources.Load("MusicSource")) as GameObject;
            music.SetActive(true);
            audio = music.GetComponent<AudioSource>();
            audio.clip = clip;
            audio.playOnAwake = false;
            audio.volume = volume;
            audio.Play();
            m_musicdict.Add(key, music);
        }
        else
        {
            if (m_musicdict[key] == null)
                return 0;
            audio = m_musicdict[key].GetComponent<AudioSource>();
            audio.volume = volume;
            audio.Stop();
            audio.clip = clip;
            audio.Play();
        }
        return audio.clip.length;
    }

    public float Play(string musicname, float volume)
    {
        if (volume < 0)
            return 0;
        return Play(musicname, volume, false);
    }

    public float Play(string musicname, bool random)
    {
        return Play(musicname, 1, random);
    }

    public float Play(string musicname)
    {
        return Play(musicname, 1, false);
    }

}

