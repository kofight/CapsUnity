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
        //根据平台开关数据分析
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WP8Player)
        {
            CapsConfig.EnableGA = true;
            CapsConfig.EnableTalkingData = true;
            Resources.Load("GA_SystemTracker");
        }
        else
        {
            CapsConfig.EnableGA = false;
            CapsConfig.EnableTalkingData = false;
        }

#if UNITY_ANDROID || UNITY_IPHONE
        if (CapsConfig.EnableTalkingData)
        {
            TalkingDataPlugin.SessionStarted("8F604653A8CC694E6954B51FE6D26127", "Test");
        }
#endif
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
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.SetResolution(480, 800, false);
        }
		
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
        if (CapsConfig.Instance.SlideSpeed > 0)
        {
            GameLogic.SLIDE_SPEED = CapsConfig.Instance.SlideSpeed;
        }

        GlobalVars.ReadHeart();

        UIWindowManager.Singleton.CreateWindow<UILogin>();
        Timer.AddDelayFunc(0.3f, delegate()
        {
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow(delegate()
            {
                GameObject obj = GameObject.Find("FirstTimeBackground");           //为了让第一次进游戏的图平滑变化没有闪烁，先在场景里垫了一张图，现在用完了，把图删除
                GameObject.Destroy(obj);

            });
        });

        Localization.instance.currentLanguage = "Chinese";      //中文版

        //初始化支付插件
        if (Application.platform != RuntimePlatform.WindowsPlayer)
        {
            Unibiller.onBillerReady += OnUniBillInitialised;
            Unibiller.Initialise();
        }

        //第一次运行时，5月份之前，送点金币
        if (System.DateTime.Today.Year == 2014 && System.DateTime.Today.Month <= 5
            && !PlayerPrefs.HasKey("GiveFirstTimeMoney"))
        {
            PlayerPrefs.SetInt("GiveFirstTimeMoney", 1);
            Unibiller.CreditBalance("gold", 3000);          //给3000金币
        }
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();

        if (GlobalVars.PurchaseSuc)
        {
            UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(GlobalVars.OnPurchaseFunc);        //设置完成后执行的函数
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
            GlobalVars.PurchaseSuc = false;

            if (CapsConfig.EnableGA)
            {
                string StageString = "BuyCoinsStage" + GlobalVars.AvailabeStageCount;
                GA.API.Business.NewEvent("Buy" + GlobalVars.PurchasingItemName, "USD", GlobalVars.PurchasingItemPrice);             //记录购买金钱包
                GA.API.Design.NewEvent("Buy" + GlobalVars.PurchasingItemName);
            }

            int purchaseInTotal = PlayerPrefs.GetInt("PurchaseInTotal", 0);

            if (CapsConfig.EnableGA)
            {
                if (purchaseInTotal == 0)       //若第一次消费
                {
                    GA.API.Design.NewEvent("FirstBuyPlayTime" + ":" + GlobalVars.PurchasingItemName, GetPlayTime());                    //第一次购买时的游戏时间
                    GA.API.Design.NewEvent("FirstBuyStageCount" + ":" + GlobalVars.PurchasingItemName, GlobalVars.AvailabeStageCount);  //第一次购买时的关数
                }
            }

            PlayerPrefs.SetInt("PurchaseInTotal", purchaseInTotal + GlobalVars.PurchasingItemPrice);
        }
        if (GlobalVars.PurchaseCancel)
        {
            GlobalVars.UsingItem = PurchasedItem.None;
            Debug.Log("Purchase Cancelled");

            UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseCancelled"));
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(GlobalVars.OnCancelFunc);        //设置完成后执行的函数
            GlobalVars.PurchaseCancel = false;
        }
        if (GlobalVars.PurchaseFailed)
        {
            GlobalVars.UsingItem = PurchasedItem.None;
            //Debug.Log("Purchase Failed");
            UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseFailed"));
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(GlobalVars.OnCancelFunc);        //设置完成后执行的函数
            GlobalVars.PurchaseFailed = false;
        }
    }

    void OnPurchased(PurchaseEvent e)      //购买成功
    {
        GlobalVars.PurchaseSuc = true;
    }

    void OnPurchaseFailed(PurchasableItem item)     //购买失败
    {
        GlobalVars.PurchaseFailed = true;
    }

    void OnPurchaseCancelled(PurchasableItem item)      //主动取消购买
    {
        GlobalVars.PurchaseCancel = true;
    }

    public void SubmitUseItemData(string itemName)
    {
        if (!CapsConfig.EnableGA)
        {
            return;
        }

        string StageString = "UsingItemStage" + GlobalVars.CurStageNum;
        GA.API.Design.NewEvent(itemName + ":" + StageString);
        GA.API.Design.NewEvent(StageString + ":" + itemName);

        int useItemCount = PlayerPrefs.GetInt("UsedItemCount", 0);
        if (useItemCount == 0)      //第一次使用
        {
            GA.API.Design.NewEvent("FirstUseItemPlayTime" + ":" + itemName, GetPlayTime());                    //第一次购买时的游戏时间
            GA.API.Design.NewEvent("FirstUseItemStageCount" + ":" + itemName, GlobalVars.AvailabeStageCount);  //第一次购买时的关数
        }
        PlayerPrefs.SetInt("UsedItemCount", useItemCount + 1);      //记录使用道具次数
    }

    //初始化支付插件
    private void OnUniBillInitialised(UnibillState result)
    {
        if (result != UnibillState.SUCCESS)
        {
            Debug.LogError(result.ToString());
        }
        else
        {
            Debug.Log(result.ToString());

            Unibiller.onPurchaseCompleteEvent += OnPurchased;
            Unibiller.onPurchaseFailed += OnPurchaseFailed;
            Unibiller.onPurchaseCancelled += OnPurchaseCancelled;
        }
    }

    public override void OnApplicationPause(bool bPause)
    {
        base.OnApplicationPause(bPause);
        if (bPause)
        {
            PlayerPrefs.SetFloat("PlayTime", GetPlayTime());        //暂停时保存游戏时间
#if UNITY_ANDROID || UNITY_IPHONE
            if (CapsConfig.EnableTalkingData)
                TalkingDataPlugin.SessionStoped();
#endif
        }
        else
        {
            m_playTime = PlayerPrefs.GetFloat("PlayTime");          //恢复时读取游戏时间
            m_startAppTime = Time.realtimeSinceStartup;
#if UNITY_ANDROID || UNITY_IPHONE
            if (CapsConfig.EnableTalkingData)
                TalkingDataPlugin.SessionStarted("8F604653A8CC694E6954B51FE6D26127", "Test");
#endif
        }
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Application.Quit();

        PlayerPrefs.SetFloat("PlayTime", GetPlayTime());

#if UNITY_ANDROID || UNITY_IPHONE
        if (CapsConfig.EnableTalkingData)
            TalkingDataPlugin.SessionStoped();
#endif
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
