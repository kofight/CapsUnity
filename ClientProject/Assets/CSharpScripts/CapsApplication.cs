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
    BaseSpliceSpriteCommonAtlas
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
        UIDrawer.Singleton.fontDefaultPrefabID = (int)UIDrawPrefab.OutlineTextLabel;
        UIDrawer.Singleton.spriteDefaultPrefabID = (int)UIDrawPrefab.BaseSpriteCommonAtlas;
        //UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseSpliceSpriteCommonAtlas, Resources.Load("BaseSpliceSpriteCommonAtlas") as GameObject);
        //UIDrawer.Singleton.AddPrefab((int)UIDrawPrefab.BaseAvatarSpriteAtlas, Resources.Load("BaseAvatarSpriteAtlas") as GameObject);

        TextTable.Singleton.AddTextMap(@"baseText");
        TextTable.Singleton.AddTextMap(@"errorcode");

        if (CapsConfig.Instance.RapidStart)
        {
            ChangeState((int)StateEnum.Game);
        }
        else
        {
            ChangeState((int)StateEnum.Login);
			UIWindowManager.Singleton.CreateWindow<UILogin>().ShowWindow();
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
