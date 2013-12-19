using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public enum StateEnum
{
    Login,
    Game,
}

enum LanguageType
{
	English,
	Chinese,
}

public enum UIDrawPrefab
{
    DefaultLabel,
    OutlineTextLabel,
    ShadowTextLabel,
    BaseSpriteCommonAtlas,
    BaseNumber,
}

public class CapsApplication : S5Application
{
    public StateEnum CurStateEnum { get; set; }
    #region Singleton
    public static CapsApplication Singleton { get; private set; }
    public CapsApplication()
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
    public bool HasSeenSplash { get; set; }

    float m_startAppTime;                           //开始app的时间
    float m_playTime;

    protected override void DoInit()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            CapsConfig.EnableGA = true;
        }
        else
        {
            CapsConfig.EnableGA = false;
        }
        if (CapsConfig.EnableGA)
        {
            //TalkingDataPlugin.SessionStarted("8F604653A8CC694E6954B51FE6D26127", "Test");
        }
        m_startAppTime = Time.realtimeSinceStartup;
        m_playTime = PlayerPrefs.GetFloat("PlayTime");

        if (!PlayerPrefs.HasKey("Music"))        //第一次运行
        {
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("SFX", 1);
            GlobalVars.UseMusic = true;
            GlobalVars.UseSFX = true;
        }
        else
        {
            GlobalVars.UseMusic = (PlayerPrefs.GetInt("Music") == 1);
            GlobalVars.UseSFX = (PlayerPrefs.GetInt("SFX") == 1);
        }

		Application.targetFrameRate = 60;			//
		
        new CapsConfig();
        new ResourceManager();

        UIDrawer.Singleton.AddPrefab(ResourceManager.Singleton.GetUIPrefabByName("BaseTextLabel"));
        UIDrawer.Singleton.fontDefaultPrefabID = UIDrawer.Singleton.AddPrefab(ResourceManager.Singleton.GetUIPrefabByName("OutlineTextLabel"));
        UIDrawer.Singleton.AddPrefab(ResourceManager.Singleton.GetUIPrefabByName("ShadowTextLabel"));
        UIDrawer.Singleton.spriteDefaultPrefabID = UIDrawer.Singleton.AddPrefab(ResourceManager.Singleton.GetUIPrefabByName("BaseSpriteCommonAtlas"));
        UIDrawer.Singleton.numDefaultPrefabID = UIDrawer.Singleton.AddPrefab(ResourceManager.Singleton.GetUIPrefabByName("BaseNumber"));

        TextTable.Singleton.AddTextMap(@"baseText");
        TextTable.Singleton.AddTextMap(@"errorcode");

        ChangeState((int)StateEnum.Login);

        GlobalVars.TotalStageCount = CapsConfig.Instance.TotalStageCount;
        if (CapsConfig.Instance.MoveTime > 0)
        {
            GameLogic.MOVE_TIME = CapsConfig.Instance.MoveTime;
        }
        if (CapsConfig.Instance.EatTime > 0)
        {
            GameLogic.EATBLOCK_TIME = CapsConfig.Instance.EatTime;
        }
        if (CapsConfig.Instance.DropAcc > 0)
        {
            GameLogic.DROP_ACC = CapsConfig.Instance.DropAcc;
        }
        if (CapsConfig.Instance.DropSpeed > 0)
        {
            GameLogic.DROP_SPEED = CapsConfig.Instance.DropSpeed;
        }
        if (CapsConfig.Instance.DropSpeed > 0)
        {
            GameLogic.SLIDE_SPEED = CapsConfig.Instance.SlideSpeed;
        }

        //读取心数相关
        if (PlayerPrefs.HasKey("HeartCount"))
        {
            GlobalVars.HeartCount = PlayerPrefs.GetInt("HeartCount");
            string heartTimeString = PlayerPrefs.GetString("GetHeartTime");
            GlobalVars.GetHeartTime = Convert.ToDateTime(heartTimeString);
        }

        UIWindowManager.Singleton.CreateWindow<UILogin>().ShowWindow();
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void OnApplicationPause(bool bPause)
    {
        base.OnApplicationPause(bPause);
        if (bPause)
        {
            PlayerPrefs.SetFloat("PlayTime", GetPlayTime());        //暂停时保存游戏时间
            //if (CapsConfig.EnableGA)
            //    TalkingDataPlugin.SessionStoped();
        }
        else
        {
            m_playTime = PlayerPrefs.GetFloat("PlayTime");          //恢复时读取游戏时间
            m_startAppTime = Time.realtimeSinceStartup;
            //if (CapsConfig.EnableGA)
            //    TalkingDataPlugin.SessionStarted("8F604653A8CC694E6954B51FE6D26127", "Test");
        }
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Application.Quit();

        //保存心数相关
        PlayerPrefs.SetInt("HeartCount", GlobalVars.HeartCount);
        PlayerPrefs.SetString("GetHeartTime", Convert.ToString(GlobalVars.GetHeartTime));

        PlayerPrefs.SetFloat("PlayTime", GetPlayTime());

        //if (CapsConfig.EnableGA)
        //    TalkingDataPlugin.SessionStoped();
    }

    public float GetPlayTime()
    {
        float elapseTime = Time.realtimeSinceStartup - m_startAppTime;
        return m_playTime + elapseTime;
    }

    protected override State CreateState(int statEnum)
    {
        CurStateEnum = (StateEnum)statEnum;
        switch (statEnum)
        {
            case (int)StateEnum.Login:
                {
                    return new LoginState();
                }
            case (int)StateEnum.Game:
                {
                    return new GameState();
                }
        }
        return null;
    }
}
