using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct DialogData
{
	public string headLeft;
	public string headRight;
	public bool activeLeftHead;
	public string itemSprite;
	public int speed;
	public string content;
}

public enum DialogTriggerPos
{
    StageStart,               //关卡开始前（地图界面）
    //StageStart,                     //关卡开始时（Loading后，游戏区出现前）
    StageEnd,                       //关卡结束时(切回大地图前)
}

public struct DialogEvent
{
    public string backPic;
    public int dialogGroupNum;
    public bool need3Star;
    public DialogTriggerPos triggerPos;
}

public class UIDialog : UIWindow
{
    AocTypewriterEffect m_dialogText;
    UISprite m_dialogBoardSprite;
	UISprite m_head1Sprite;
	UISprite m_head2Sprite;
	UISprite m_itemBoard;
    UISprite m_backPic;

    int m_curDialogGroupNum;
    int m_curDialogIndex;

    WindowEffectFinished m_afterDialogFunc;

    Dictionary<int, List<DialogData> > m_dialogGroupMap = new Dictionary<int, List<DialogData> >();
    Dictionary<KeyValuePair<int, DialogTriggerPos>, DialogEvent> m_dialogEventMap = new Dictionary<KeyValuePair<int, DialogTriggerPos>, DialogEvent>();

    public bool TriggerDialog(int stageNum, DialogTriggerPos pos, WindowEffectFinished func)                //尝试触发一个对话，返回值是是否成功触发了对话(失败是对话触发过了或者此处没对话)
    {
        DialogEvent dialogEvent;
        if (!m_dialogEventMap.TryGetValue(new KeyValuePair<int,DialogTriggerPos>(stageNum, pos), out dialogEvent))
        {
            if (func != null)
                func();
            return false;
        }
		
        //若出现过的开始对话就不再出现
		if(pos == DialogTriggerPos.StageStart)
		{
			if(PlayerPrefs.GetInt("StageStartDialogFinished") >= stageNum)
			{
                if (func != null)
                    func();
				return false;
			}
			else
			{
				PlayerPrefs.SetInt("StageStartDialogFinished", stageNum);
			}
		}

        if (dialogEvent.backPic == "None")
        {
            m_backPic.gameObject.SetActive(false);
        }
        else
        {
            m_backPic.gameObject.SetActive(true);
            m_backPic.spriteName = dialogEvent.backPic;
        }

        m_backPic.gameObject.SetActive(false);
		
        OpenDialog(dialogEvent.dialogGroupNum);                                 //触发对话
        m_afterDialogFunc = func;
        return true;
    }
	
	public void OpenDialog(int number)			//开启一段对话
	{
		ShowWindow();
        m_curDialogGroupNum = number;
        m_curDialogIndex = 0;
        ShowText(m_curDialogIndex);
	}

    void ShowText(int indexNum)
    {
        DialogData data = m_dialogGroupMap[m_curDialogGroupNum][m_curDialogIndex];
        ShowText(data.headLeft, data.headRight, data.activeLeftHead, data.itemSprite, data.speed, data.content);
    }
	
	void ShowText(string head1, string head2, bool activeLeftHead, string itemSprite, int speed, string content)
	{
        if (itemSprite != "None")
        {

        }
        else
        {
            m_itemBoard.gameObject.SetActive(false);
        }
        if (activeLeftHead)
        {
            m_dialogBoardSprite.transform.LocalScaleX(1.0f);
        }
        else
        {
            m_dialogBoardSprite.transform.LocalScaleX(-1.0f);
        }
		if(head1 != "None")
		{
			m_head1Sprite.gameObject.SetActive(true);
			m_head1Sprite.spriteName = head1;
			if(!activeLeftHead)
			{
				m_head1Sprite.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}
			else
			{
				m_head1Sprite.color = Color.white;
			}
		}
		else
		{
			m_head1Sprite.gameObject.SetActive(false);
		}
		
		if(head2 != "None")
		{
			m_head2Sprite.gameObject.SetActive(true);
			m_head2Sprite.spriteName = head2;
			if(activeLeftHead)
			{
				m_head2Sprite.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}
			else
			{
				m_head2Sprite.color = Color.white;
			}
		}
		else
		{
			m_head2Sprite.gameObject.SetActive(false);
		}

        m_dialogText.Play(content, delegate()
        {

        });
	}
	
	public void OnClick()
	{
		if(m_curDialogIndex < m_dialogGroupMap[m_curDialogGroupNum].Count-1)
        {
			++m_curDialogIndex;
            ShowText(m_curDialogIndex);
		}
		else
		{
            m_curDialogGroupNum = -1;
			HideWindow(m_afterDialogFunc);
		}
	}

    public void EndDialog()
    {
        m_curDialogGroupNum = -1;
        HideWindow(m_afterDialogFunc);
    }
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_dialogText = GetChildComponent<AocTypewriterEffect>("DialogText");
		m_head1Sprite = GetChildComponent<UISprite>("LeftHead");
		m_head2Sprite = GetChildComponent<UISprite>("RightHead");
        m_dialogBoardSprite = GetChildComponent<UISprite>("DialogBoard");

        m_backPic = GetChildComponent<UISprite>("Background");
		m_itemBoard = GetChildComponent<UISprite>("ItemBoard");

        //解析DialogEvent配置文件
        string eventContent = ResourceManager.Singleton.LoadTextFile("DialogEvent");
        //解析Dialog配置文件////////////////////////////////////////////////////////////////////////
        StringReader sr = new StringReader(eventContent);
        string line = sr.ReadLine();
        while (line != null)
        {
            if (line.Contains("//"))
            {
                line = sr.ReadLine();
                continue;
            }
            if (string.IsNullOrEmpty(line))
            {
                line = sr.ReadLine();
                continue;
            }
            string[] values = line.Split(new string[] { "\t", " " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                int curStageNum = System.Convert.ToInt32(values[0]);
                DialogTriggerPos triggerPos = (DialogTriggerPos)System.Convert.ToInt32(values[1]);

                DialogEvent data = new DialogEvent();
                data.dialogGroupNum = System.Convert.ToInt32(values[2]);
                data.need3Star = (values[2] == "Y");
                data.backPic = values[3];
                data.triggerPos = triggerPos;

                m_dialogEventMap.Add(new KeyValuePair<int, DialogTriggerPos>(curStageNum, triggerPos), data);                               //添加对话数据
            }

            line = sr.ReadLine();
        }
        sr.Close();

        string content = ResourceManager.Singleton.LoadTextFile("Dialog");
        //解析Dialog配置文件////////////////////////////////////////////////////////////////////////
        sr = new StringReader(content);
        line = sr.ReadLine();
        int curDialogGroupNum = -1;
        List<DialogData> curDialogGroup = new List<DialogData>();
        while (line != null)
        {
            if (line.Contains("//"))
            {
                line = sr.ReadLine();
                continue;
            }
            if (string.IsNullOrEmpty(line))
            {
                line = sr.ReadLine();
                continue;
            }
            string[] values = line.Split(new string[] { "\t", " " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                int num = System.Convert.ToInt32(values[0]);
				if(curDialogGroupNum == -1)
				{
					curDialogGroupNum = num;
					m_dialogGroupMap.Add(curDialogGroupNum, curDialogGroup);
				}
                else if (num != curDialogGroupNum)                           //若数字变化了，新加一组对话
                {
					curDialogGroup = new List<DialogData>();            //重新创建一个对话组数据
					curDialogGroupNum = num;                            //更改当前在编的对话组数字
                    m_dialogGroupMap.Add(curDialogGroupNum, curDialogGroup);
                }

                DialogData data = new DialogData();
                data.activeLeftHead = (values[1] == "L");
                data.headLeft = values[2];
                data.headRight = values[3];
                data.itemSprite = values[4];
                data.speed = System.Convert.ToInt32(values[5]);
                data.content = values[6];
                data.content.Replace('_', ' ');                         //下划线替换成空格

                curDialogGroup.Add(data);                               //添加对话数据
            }

            line = sr.ReadLine();
        }
        sr.Close();
		
		AddChildComponentMouseClick("DialogBoard", OnClick);
        
    }
    public override void OnShow()
    {
        base.OnShow();
    }
}
