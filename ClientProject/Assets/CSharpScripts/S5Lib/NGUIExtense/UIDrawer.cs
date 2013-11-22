using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDrawerData
{
    public bool DidDrawLastFrame = false;       //Did the Data Drawed Last Frame?
    public int xPos;
    public int yPos;
    public UIWidget uiWidget;
    public UIEffectPlayer effectPlayer;
	public UIWindowStateEnum playingState;
    public string numberStartName;
    public float curNumber = -100000;
    public Transform transform;
    public List<Transform> m_numSpriteList = new List<Transform>();

    public void Show()
    {
        effectPlayer = transform.GetComponent<UIEffectPlayer>();
        if (effectPlayer != null)
        {
            effectPlayer.ShowEffect();
            playingState = UIWindowStateEnum.PlayingShowEffect;
        }
    }

    public void Hide()
    {
        if (effectPlayer != null)
        {
            effectPlayer.HideEffect();
            playingState = UIWindowStateEnum.PlayingHideEffect;
        }
    }

    public void Update()
    {
        if (effectPlayer != null)
        {
            if (playingState == UIWindowStateEnum.PlayingShowEffect)
            {
                if (!effectPlayer.IsPlaying())
                {
                    playingState = UIWindowStateEnum.Show;
                }
            }

            if (playingState == UIWindowStateEnum.PlayingHideEffect)
            {
                if (!effectPlayer.IsPlaying())
                {
                    playingState = UIWindowStateEnum.Hide;
                }
            }
        }
    }
}

public class UIDrawer
{
    private static readonly int moveDataCapibility = 100;               //清理时每次清理的数量
    string[] DataToRemove = new string[moveDataCapibility];             //清理时的临时储存区

    public UIWindowManager.Anchor DefaultAnchor = UIWindowManager.Anchor.TopLeft;               //默认的锚点

    #region Singleton
    public static UIDrawer Singleton { get; private set; }
    public UIDrawer()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion

    Dictionary<string, UIDrawerData> mDrawDataMap = new Dictionary<string, UIDrawerData>();

    GameObject[] mPrefabs = new GameObject[10];                                                 //用来存放原型的数组
    int m_curPrefabCount = 0;                                                                   //当前的prefab号

    public int CurDepth = 0;                    //当前的深度
    public float CurZ = 0.0f;                   //当前的Z值

    public int fontDefaultPrefabID = 0;         //字体的默认Prefab
    public int spriteDefaultPrefabID = 0;       //图片的默认Prefab
    public int numDefaultPrefabID = 0;          //数字的默认Prefab

    public int AddPrefab(GameObject prefab)     //添加一个Prefab并返回ID
    {
        mPrefabs[m_curPrefabCount] = prefab;
        return m_curPrefabCount++;
    }

    UIDrawerData CreateText(string id, int x, int y, string text, int prefabID)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UILabel label = data.uiWidget as UILabel;
        label.text = text;
		data.Show();      //触发显示
        return data;
    }

    UIDrawerData CreateSprite(string id, int x, int y, string spriteName, int prefabID, int depth)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UISprite sprite = data.uiWidget as UISprite;
        sprite.spriteName = spriteName;
        sprite.depth = depth;
		sprite.MakePixelPerfect();
        data.transform.localPosition = new Vector3(x, -y, 0);
		data.Show();      //触发显示
        return data;
    }

    UIDrawerData CreateNumber(string id, int x, int y, string imageNameStart, float xInterval, int maxIntLenth = 3, int floatLenth = 0)
    {
        UIDrawerData data = CreateWidget(id, x, y, numDefaultPrefabID);
		
        Transform trans = data.transform.GetChild(0);
        int lenth = maxIntLenth + (floatLenth > 0 ? floatLenth + 1 : 0);		//计算位长
        int xPos = 0;
        data.m_numSpriteList.Add(trans);
        trans.gameObject.SetActive(false);
        trans.gameObject.LocalPositionX(xPos);
		trans.gameObject.LocalPositionY(0.0f);		
		data.numberStartName = imageNameStart;

        for (int i = 1; i < lenth; ++i )
        {
            GameObject newObj = GameObject.Instantiate(trans.gameObject) as GameObject;
            data.m_numSpriteList.Add(newObj.transform);
			newObj.transform.parent = data.transform;
			newObj.transform.localPosition = Vector3.zero;
			
            xPos += (int)xInterval;
			newObj.transform.localScale = trans.localScale;
            newObj.transform.LocalPositionX(xPos);
			newObj.transform.LocalPositionY(0.0f);
            newObj.SetActive(false);
			
        }
        return data;
    }

    UIDrawerData CreateWidget(string id, int x, int y, int prefabID)
    {
        UIDrawerData data = new UIDrawerData();
        data.xPos = x;
        data.yPos = y;

        //实例化Prefab
        GameObject prefab = mPrefabs[prefabID];
        if (prefab == null)
        {
            Debug.LogError("Use Undefine PrefabID : " + prefabID.ToString() + " When Draw " + id);
            return null;
        }
        GameObject labelObj = GameObject.Instantiate(prefab) as GameObject;

        //查找widgets////////////////////////////////////////////////////////////////////////
        data.uiWidget = labelObj.GetComponent<UIWidget>();
        data.transform = labelObj.transform;
        labelObj.transform.parent = UIWindowManager.Singleton.AnchorObject[(int)DefaultAnchor].transform;
        labelObj.transform.localPosition = new Vector3(x, -y, CurZ);
        labelObj.transform.localScale = Vector3.one;

        mDrawDataMap[id] = data;
        return data;
    }

    //绘制文字的原型版本
    public void DrawText(string id, int x, int y, string text, int labelID)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateText(id, x, y, text, labelID);
            data.transform.localPosition = new Vector3(x, -y, CurZ);
        }
        UILabel label = data.uiWidget as UILabel;
        if (label.text != text)
        {
            label.text = text;
        }
        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.transform.localPosition = new Vector3(x, -y, CurZ);
        }
        data.DidDrawLastFrame = true;
    }

    //绘制文字的简化版本，使用默认ID
    public void DrawText(string id, int x, int y, string text)
    {
        DrawText(id, x, y, text, fontDefaultPrefabID);
    }

    //绘制文字的简化版本，使用表中字段
    public void DrawText(string stringKeyName, int x, int y)
    {
        DrawText(stringKeyName, x, y, TextTable.Singleton.GetString(stringKeyName), fontDefaultPrefabID);
    }

    //绘制图片
    public UISprite DrawSprite(string id, int x, int y, string spriteName, int prefabID)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateSprite(id, x, y, spriteName, prefabID, CurDepth);
        }
        var sprite = data.uiWidget as UISprite;
        sprite.depth = CurDepth;

        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.transform.localPosition = new Vector3(x, -y, CurZ);
        }
        data.DidDrawLastFrame = true;
        return sprite;
    }

    //绘制图片的简化版本
    public UISprite DrawSprite(string id, int x, int y, string spriteName)
    {
        return DrawSprite(id, x, y, spriteName, spriteDefaultPrefabID);
    }

    public void DrawNumber(string id, int x, int y, float numberValue, string imageNameStart, float xInterval, int maxIntLenth = 3, int floatLenth = 0)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateNumber(id, x, y, imageNameStart, xInterval, maxIntLenth, floatLenth);
        }

        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.transform.localPosition = new Vector3(x, -y, CurZ);
        }
        if (numberValue != data.curNumber)
        {
            data.curNumber = numberValue;
	        int intNum = (int)numberValue;
	        int factor = 10;									//用来取某个位的数字的因子
	        string name;

	        int curNumStartIndex = maxIntLenth - 1;
	        for (int i=0; i<maxIntLenth; ++i)		//处理整数部分
	        {
		        int number = (intNum % factor) / (factor / 10);		//取出正在处理的位的数字
		        if (number != 0)
		        {
			        curNumStartIndex = maxIntLenth - 1 - i;
		        }

                name = data.numberStartName + number;
                data.m_numSpriteList[maxIntLenth - 1 - i].GetComponent<UISprite>().spriteName = name;
				data.m_numSpriteList[maxIntLenth - 1 - i].GetComponent<UISprite>().depth = CurDepth;
		        factor *= 10;
	        }

            if (floatLenth > 0)			//若有小数部分
            {
                name = data.numberStartName + "Point";        //生成名字字符串
                data.m_numSpriteList[maxIntLenth].GetComponent<UISprite>().spriteName = name;		//小数点

                factor = 10;

                for (int i = 0; i < floatLenth; ++i)
                {
                    int number = ((int)(numberValue * factor)) % 10;
                    name = data.numberStartName + number;
                    data.m_numSpriteList[maxIntLenth].GetComponent<UISprite>().spriteName = name;
                    data.m_numSpriteList[maxIntLenth].GetComponent<UISprite>().depth = CurDepth;
                    factor *= 10;
                }
            }

            for (int i = curNumStartIndex; i < data.m_numSpriteList.Count; ++i)
            {
                data.m_numSpriteList[i].gameObject.SetActive(true);
            }

        }
        data.DidDrawLastFrame = true;
    }

    public void Update()
    {
        int i = 0;
        foreach (KeyValuePair<string, UIDrawerData> pair in mDrawDataMap)
        {
            pair.Value.Update();                            //更新
            //Find All The Data Which We Want To Remove
            if (!pair.Value.DidDrawLastFrame)
            {
                if (pair.Value.effectPlayer != null)        //若有特效
                {
                    if (pair.Value.playingState == UIWindowStateEnum.Hide)       //若特效播放完毕
                    {
                        if (i < moveDataCapibility)
                        {
                            DataToRemove[i] = pair.Key;
                            ++i;
                        }
                    }
                    else if(pair.Value.playingState == UIWindowStateEnum.Show || pair.Value.playingState == UIWindowStateEnum.PlayingShowEffect)
                    {
						pair.Value.Hide();                      //隐藏
                    }
                }
                else
                {
                    if (i < moveDataCapibility)
                    {
                        DataToRemove[i] = pair.Key;
                        ++i;
                    }
                }
            }
            pair.Value.DidDrawLastFrame = false;            //Reset the flag
        }

        //Remove The Data
        for (int j = 0; j < i; j++)
        {
            DestoryObj(DataToRemove[j]);
        }
    }

    void DestoryObj(string id)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            Debug.LogWarning("Remove an not exist obj id = " + id);
        }
        GameObject.Destroy(data.transform.gameObject);
        mDrawDataMap.Remove(id);
    }
}
