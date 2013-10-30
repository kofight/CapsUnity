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
        UIWindowManager.Singleton.CreateWindow<UILogin>();
        UIWindowManager.Singleton.CreateWindow<UIHowToPlay>();
        UIWindowManager.Singleton.CreateWindow<UIMainMenu>(UIWindowManager.Anchor.BottomLeft);
        UIWindowManager.Singleton.CreateWindow<UIOption>();
        UIWindowManager.Singleton.CreateWindow<UIQuitConfirm>();
        UIWindowManager.Singleton.CreateWindow<UIStageInfo>();
        UIWindowManager.Singleton.CreateWindow<UIMap>();
        //UIWindowManager.Singleton.GetUIWindow<UISplash>().ShowWindow();

        Instance = this;
    }

    public override void OnPressDown(int fingerIndex, Vector2 fingerPos)
    {
        base.OnPressDown(fingerIndex, fingerPos);
		if(UIWindowManager.Singleton.GetUIWindow<UISplash>().Visible)
        	UIWindowManager.Singleton.GetUIWindow<UISplash>().Close();
    }

    public override void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
        base.OnDragMove(fingerPos, delta);
        if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMap>().OnDragMove(fingerPos, delta);
        }
    }

    public override void Update()
    {
 	    base.Update();
    }
}
