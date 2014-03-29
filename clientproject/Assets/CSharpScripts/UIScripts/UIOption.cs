using UnityEngine;
using System.Collections;

public class UIOption : UIWindow 
{
	UIToggle m_englishVersion;
    public override void OnCreate()
    {
        base.OnCreate();
		m_englishVersion = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "EnglishCheck");
        m_englishVersion.value = Localization.instance.currentLanguage == "English";
        EventDelegate.Set(m_englishVersion.onChange, delegate()
        {
            if (m_englishVersion.value)
            {
                if (Localization.instance != null)
                {
                    Localization.instance.currentLanguage = "English";
                }
            }
            else
            {
                if (Localization.instance != null)
                {
                    Localization.instance.currentLanguage = "Chinese";
                }
            }
        });

        UIToggle musicCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "MusicCheck");
        musicCheck.value = GlobalVars.UseMusic;
        EventDelegate.Set(musicCheck.onChange, delegate()
        {
            GlobalVars.UseMusic = UIToggle.current.value;
            PlayerPrefs.SetInt("Music", GlobalVars.UseMusic == true ? 1 : 0);
            if (GlobalVars.UseMusic == false)       //关闭音乐
            {
                UIToolkits.StopMusic();
            }
            else                                    //播放音乐
            {
                if (!UIToolkits.IsPlayingMusic())
                {
                    UIToolkits.PlayMusic(CapsConfig.CurAudioList.MapMusic);
                }
            }
        });

        UIToggle soundCheck = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "SoundCheck");
        soundCheck.value = GlobalVars.UseSFX;
        EventDelegate.Set(soundCheck.onChange, delegate()
        {
            GlobalVars.UseSFX = UIToggle.current.value;
            PlayerPrefs.SetInt("SFX", GlobalVars.UseSFX == true ? 1 : 0);
        });


        AddChildComponentMouseClick("CloseBtn", CloseWindow);
    }

    public void CloseWindow()
    {
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }
}
