using UnityEngine;
using System.Collections;

public class UILogin : UIWindow 
{
    UIToggle m_developerMode;
    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("PlayBtn", OnPlayBtnClick);
        AddChildComponentMouseClick("ClearBtn", delegate()
        {
            PlayerPrefs.DeleteAll();            //删除进度
        });

        AddChildComponentMouseClick("RecoverHeartBtn", delegate()
        {
            PlayerPrefs.SetInt("HeartCount", 5);            //恢复爱心
            GlobalVars.HeartCount = 5;
        });

        m_developerMode = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "DeveloperCheck");
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnPlayBtnClick()
    {
        HideWindow();
		GlobalVars.DeveloperMode = m_developerMode.value;
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }
}
