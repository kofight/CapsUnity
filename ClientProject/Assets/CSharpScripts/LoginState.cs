using UnityEngine;
using System.Collections;

public enum TLoginFlow
{
    LoginFlow_LoginScreen,
    LoginFlow_Map,
}

public class LoginState : State
{
    public static LoginState Instance;
    public TLoginFlow CurFlow { get; set; }

    public override void DoDeInitState()
    {
        base.DoDeInitState();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().HideWindow();
    }

    public override void DoInitState()
    {
		base.DoInitState();
		
		Instance = this;
		
        //UIWindowManager.Singleton.CreateWindow<UISplash>();
        //UIWindowManager.Singleton.CreateWindow<UIHowToPlay>();
        
        //UIWindowManager.Singleton.CreateWindow<UIOption>();

        if (UIWindowManager.Singleton.GetUIWindow<UIMap>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIMainMenu>(UIWindowManager.Anchor.BottomLeft);
            UIWindowManager.Singleton.CreateWindow<UIQuitConfirm>();
            UIWindowManager.Singleton.CreateWindow<UIStageInfo>();
            UIWindowManager.Singleton.CreateWindow<UIMap>();
            UIWindowManager.Singleton.CreateWindow<UINoMoreHearts>();
            UIWindowManager.Singleton.CreateWindow<UIHowToPlay>();
            UIWindowManager.Singleton.CreateWindow<UIWindow>("UILoading", UIWindowManager.Anchor.Center);
            UIWindowManager.Singleton.CreateWindow<UIDialog>(UIWindowManager.Anchor.Bottom);
            UIWindowManager.Singleton.CreateWindow<UIStore>();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().RefreshButtons();
        }
        
        //UIWindowManager.Singleton.GetUIWindow<UISplash>().ShowWindow();

        if (GlobalVars.UseMusic)
        {
            UIToolkits.PlayMusic(CapsConfig.CurAudioList.MapMusic);
        }

        if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
    }

    public override void OnBackKey()
    {
        base.OnBackKey();
        if (CurFlow == TLoginFlow.LoginFlow_Map)
        {
			if (UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Visible)
	        {
	            UIWindowManager.Singleton.GetUIWindow<UINoMoreHearts>().Close();
	            return;
	        }

            if (UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIHowToPlay>().OnClose();
                return;
            }
			
			if(UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().Visible)
			{
				UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().HideWindow();
				UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
				return;
			}
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
			CurFlow = TLoginFlow.LoginFlow_LoginScreen;
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
