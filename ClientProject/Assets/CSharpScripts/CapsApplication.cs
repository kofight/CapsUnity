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

    protected override void DoInit()
    {
		Application.targetFrameRate = 60;			//
		
        new CapsConfig();
        new ResourceManager();

        UIWindowManager.Singleton.CreateWindow<UILogin>().ShowWindow(delegate()
        {
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
        });
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
