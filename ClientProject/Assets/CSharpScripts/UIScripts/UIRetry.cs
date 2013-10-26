using UnityEngine;
using System.Collections;

public class UIRetry : UIWindowNGUI 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("RetryBtn", OnRetryClicked);
        AddChildComponentMouseClick("NextLevelBtn", OnNextLevelClicked);
    }
    public override void OnShow()
    {
        Transform nextBtn = UIToolkits.FindChild(mUIObject.transform, "NextLevelBtn");
        if (GlobalVars.CurGameLogic.CheckStageFinish())
        {
            nextBtn.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.gameObject.SetActive(false);
        }
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnRetryClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        GlobalVars.CurGameLogic.ClearGame();
        GlobalVars.CurGameLogic.Init();
        GlobalVars.CurGameLogic.StartGame();
    }

    private void OnNextLevelClicked(object sender, UIMouseClick.ClickArgs e)
    {

    }

    private void OnCloseClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }
}
