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
        UIWindowManager.Singleton.CreateWindow<UISplash>();
        UIWindowManager.Singleton.CreateWindow<UIHowToPlay>();
        UIWindowManager.Singleton.CreateWindow<UIMainMenu>(UIWindowManager.Anchor.BottomLeft);
        UIWindowManager.Singleton.CreateWindow<UIOption>();
        UIWindowManager.Singleton.CreateWindow<UIQuitConfirm>();
        UIWindowManager.Singleton.CreateWindow<UIStageInfo>();
        UIWindowManager.Singleton.CreateWindow<UIMap>();
        UIWindowManager.Singleton.CreateWindow<UIWindow>("UILoading", UIWindowManager.Anchor.Center);
        //UIWindowManager.Singleton.GetUIWindow<UISplash>().ShowWindow();

        Instance = this;
    }

    public override void Update()
    {
 	    base.Update();
    }
}
