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
    BigOutlineTextLabel,
    BaseSpriteCommonAtlas,
    BaseSpliceSpriteCommonAtlas,
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

    protected override void DoInit()
    {
        new ResManager();
        new CapsConfig();
        new MusicManager();

        UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.DefaultLabel, ResManager.Singleton.GetUIPrefabByName("BaseTextLabel"));
        UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.ShadowTextLabel, ResManager.Singleton.GetUIPrefabByName("ShadowTextLabel"));
        UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.OutlineTextLabel, ResManager.Singleton.GetUIPrefabByName("OutlineTextLabel"));
        UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseSpriteCommonAtlas, ResManager.Singleton.GetUIPrefabByName("BaseSpriteCommonAtlas"));
        UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseNumber, ResManager.Singleton.GetUIPrefabByName("BaseNumber"));
        UIDrawer.Singleton.fontDefaultPrefabID = (int)UIDrawPrefab.OutlineTextLabel;
        UIDrawer.Singleton.spriteDefaultPrefabID = (int)UIDrawPrefab.BaseSpriteCommonAtlas;
        UIDrawer.Singleton.numDefaultPrefabID = (int)UIDrawPrefab.BaseNumber;
        //UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseSpliceSpriteCommonAtlas, Resources.Load("BaseSpliceSpriteCommonAtlas") as GameObject);
        //UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseAvatarSpriteAtlas, Resources.Load("BaseAvatarSpriteAtlas") as GameObject);

        TextTable.Singleton.AddTextMap(@"baseText");
        TextTable.Singleton.AddTextMap(@"errorcode");

        ChangeState((int)StateEnum.Login);
		UIWindowManager.Singleton.CreateWindow<UILogin>().ShowWindow();

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
            GameLogic.SLIDE_SPEED = CapsConfig.Instance.DropSpeed;
        }

        //读取心数相关
		if(PlayerPrefs.HasKey("HeartCount"))
		{
        	GlobalVars.HeartCount = PlayerPrefs.GetInt("HeartCount");
        	string heartTimeString = PlayerPrefs.GetString("GetHeartTime");
        	GlobalVars.GetHeartTime = Convert.ToDateTime(heartTimeString);
		}
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Application.Quit();

        //保存心数相关
        PlayerPrefs.SetInt("HeartCount", GlobalVars.HeartCount);
        PlayerPrefs.SetString("GetHeartTime", Convert.ToString(GlobalVars.GetHeartTime));
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
