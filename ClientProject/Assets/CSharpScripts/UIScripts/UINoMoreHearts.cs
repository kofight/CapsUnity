using UnityEngine;
using System.Collections;

public class UINoMoreHearts : UIWindow 
{
    UIWindow m_mainMenuExtend;
	
	NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;
	
	UISprite m_heartSprite;

    public bool NeedOpenStageInfoAfterClose;
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", Close);
		
		m_minNumber = GetChildComponent<NumberDrawer>("MinNumber");
        m_secNumber = GetChildComponent<NumberDrawer>("SecNumber");
		m_heartSprite = GetChildComponent<UISprite>("HeartToCull");

        //AddChildComponentMouseClick("Buy1HeartBtn", delegate()
        //{
        //    if (Unibiller.DebitBalance("gold", 25))
        //    {
        //        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
        //        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(delegate()
        //        {
        //            GlobalVars.AddHeart(1);
        //            ContinuePlay();
        //        });
        //        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        //    }
        //    else
        //    {
        //        HideWindow();
        //        UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
        //        GlobalVars.OnPurchaseFunc = delegate()
        //        {
        //            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
        //            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(delegate()
        //            {
        //                Unibiller.DebitBalance("gold", 25);
        //                GlobalVars.AddHeart(1);
        //                ContinuePlay();
        //            });
        //            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        //        };
        //        GlobalVars.OnCancelFunc = delegate()
        //        {
        //            Close();
        //        };
        //    }
        //});

        AddChildComponentMouseClick("Buy5HeartBtn", delegate()
        {
            if (Unibiller.DebitBalance("gold", 100))
            {
                UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
                UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(delegate()
                {
                    CapsApplication.Singleton.SubmitUseItemData("BuyHearts");
                    GlobalVars.AddHeart(5);
                    ContinuePlay();
                });
                UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
            }
            else
            {
                HideWindow();
                UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
                GlobalVars.OnPurchaseFunc = delegate()
                {
                    Unibiller.DebitBalance("gold", 100);
                    CapsApplication.Singleton.SubmitUseItemData("BuyHearts");
                    GlobalVars.AddHeart(5);
                    ContinuePlay();
                };
                GlobalVars.OnCancelFunc = delegate()
                {
                    Close();
                };
            }
        });
    }

    void ContinuePlay()
    {
        if (NeedOpenStageInfoAfterClose)
        {
            GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
            UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
            HideWindow();
        }
    }

    public void Close()
	{
		HideWindow();
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)       //若在游戏中
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);         //回大地图
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();        //返回地图，不需要刷新按钮
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
	}
	
	public override void OnUpdate ()
	{
		base.OnUpdate ();

        if (GlobalVars.HeartCount < 5)
        {
            int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
            int ticksToGetHeart = CapsConfig.Instance.GetHeartInterval * 1000 - ticks;
            int min = ticksToGetHeart / 1000 / 60;
            int second = ticksToGetHeart / 1000 % 60;
            m_minNumber.SetNumberRapid(min);
            m_secNumber.SetNumberRapid(second);

            m_heartSprite.fillAmount = 1.0f - (ticksToGetHeart / 1000.0f) / CapsConfig.Instance.GetHeartInterval;
        }
        else
        {
            m_heartSprite.fillAmount = 1.0f;
        }
	}
}
