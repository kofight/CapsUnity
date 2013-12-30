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

public class UIDialog : UIWindow
{
	UILabel m_dialogText;
	UISprite m_head1Sprite;
	UISprite m_head2Sprite;
	UISprite m_itemBoard;

    int m_curDialogGroupNum;
    int m_curDialogIndex;

    Dictionary<int, List<DialogData> > m_dialogGroupMap = new Dictionary<int, List<DialogData> >();
	
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
		if(head1 != "None")
		{
			m_head1Sprite.gameObject.SetActive(true);
			m_head1Sprite.spriteName = head1;
			if(activeLeftHead)
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
			m_head2Sprite.gameObject.SetActive(false);
		}
		
		m_dialogText.text = content;
	}
	
	public void OnClick()
	{
		if(m_curDialogIndex < m_dialogGroupMap[m_curDialogGroupNum].Count)
        {
            ShowText(m_curDialogIndex);
            ++m_curDialogIndex;
		}
		else
		{
            m_curDialogGroupNum = -1;
			HideWindow();
		}
	}
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_dialogText = GetChildComponent<UILabel>("DialogText");
		m_head1Sprite = GetChildComponent<UISprite>("LeftHead");
		m_head2Sprite = GetChildComponent<UISprite>("RightHead");
		m_itemBoard = GetChildComponent<UISprite>("ItemBoard");

        string content = ResourceManager.Singleton.LoadTextFile("Dialog");
        //解析配置文件////////////////////////////////////////////////////////////////////////
        StringReader sr = new StringReader(content);
        string line = sr.ReadLine();
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
                if (num != curDialogGroupNum)                           //若数字变化了，新加一组对话
                {
                    m_dialogGroupMap.Add(curDialogGroupNum, curDialogGroup);
                    curDialogGroupNum = num;                            //更改当前在编的对话组数字
                    curDialogGroup = new List<DialogData>();            //重新创建一个对话组数据
                }

                DialogData data = new DialogData();
                data.activeLeftHead = (values[1] == "Y");
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
