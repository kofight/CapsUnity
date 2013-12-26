using UnityEngine;
using System.Collections;

public class UINoMoreHearts : UIWindow 
{
    UIWindow m_mainMenuExtend;
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", Close);
		AddChildComponentMouseClick("OKBtn", Close);

        AddChildComponentMouseClick("Buy1HeartBtn", delegate()
        {
            if (GlobalVars.Coins > 0)
            {
                --GlobalVars.Coins;
                ++GlobalVars.HeartCount;
                PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
                ContinuePlay();
            }
        });

        AddChildComponentMouseClick("Buy5HeartBtn", delegate()
        {
            if (GlobalVars.Coins >= 3)
            {
                GlobalVars.Coins -= 3;
                GlobalVars.HeartCount = 5;
                PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
                ContinuePlay();
            }
        });
    }

    void ContinuePlay()
    {
        GlobalVars.CurStageData.LoadStageData(GlobalVars.CurStageNum);
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
        HideWindow();
    }

    void Close()
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
}
