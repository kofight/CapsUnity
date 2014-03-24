using UnityEngine;
using System.Collections;

public class UINoMoreHearts : UIWindow 
{
    UIWindow m_mainMenuExtend;
	
	NumberDrawer m_minNumber;
    NumberDrawer m_secNumber;
	
	UISprite m_heartSprite;
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", Close);
		AddChildComponentMouseClick("OKBtn", Close);
		
		m_minNumber = GetChildComponent<NumberDrawer>("MinNumber");
        m_secNumber = GetChildComponent<NumberDrawer>("SecNumber");
		m_heartSprite = GetChildComponent<UISprite>("HeartToCull");

        AddChildComponentMouseClick("Buy1HeartBtn", delegate()
        {

        });

        AddChildComponentMouseClick("Buy5HeartBtn", delegate()
        {
            if (Unibiller.DebitBalance("gold", 100))
            {
                GlobalVars.HeartCount = 5;
                ContinuePlay();
            }
            else
            {
                HideWindow();
                UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
                UIWindowManager.Singleton.GetUIWindow<UIStore>().OnPurchaseFunc = delegate()
                {
                    Unibiller.DebitBalance("gold", 100);
                    GlobalVars.HeartCount = 5;
                    ContinuePlay();
                };
				UIWindowManager.Singleton.GetUIWindow<UIStore>().OnCancelFunc = delegate()
                {
                    Close();
                };
            }
        });
    }

    void ContinuePlay()
    {
        GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
        HideWindow();
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

        if (GlobalVars.HeartCount == 0)
        {
            int ticks = (int)((System.DateTime.Now.Ticks - GlobalVars.GetHeartTime.Ticks) / 10000);
            int ticksToGetHeart = CapsConfig.Instance.GetHeartInterval * 1000 - ticks;
            int min = ticksToGetHeart / 1000 / 60;
            int second = ticksToGetHeart / 1000 % 60;
            m_minNumber.SetNumber(min);
            m_secNumber.SetNumber(second);

            m_heartSprite.fillAmount = 1.0f - (ticksToGetHeart / 1000.0f) / CapsConfig.Instance.GetHeartInterval;
        }
        else
        {
            m_heartSprite.fillAmount = 1.0f;
        }
	}
}
