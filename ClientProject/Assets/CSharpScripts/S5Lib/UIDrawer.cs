using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDrawerData
{
    public bool IsExistMode = false;            //Draw With ExistMode Or Not, Exist mode means create and manage by hand
    public bool IsLabel = true;                 //Is Label or Sprite
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
            effectPlayer.OnShow();
            playingState = UIWindowStateEnum.PlayingShowEffect;
        }
    }

    public void OnShowEffectPlayOver()
    {
        playingState = UIWindowStateEnum.Show;
    }

    public void Hide()
    {
        if (effectPlayer != null)
        {
            effectPlayer.OnHide();
            playingState = UIWindowStateEnum.PlayingHideEffect;
        }
    }

    public void OnHideEffectPlayOver()
    {
        playingState = UIWindowStateEnum.Hide;
    }

    public void Update()
    {
        if (playingState == UIWindowStateEnum.PlayingShowEffect)
        {
            if (!effectPlayer.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.ShowEffect))
            {
                OnShowEffectPlayOver();
            }
        }
        
        if (playingState == UIWindowStateEnum.PlayingHideEffect)
        {
            if (!effectPlayer.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.HideEffect))
            {
                OnHideEffectPlayOver();
            }
        }
    }
}

public class UIDrawer
{
    private static readonly int moveDataCapibility = 100;
    public UIWindowManager.Anchor DefaultAnchor = UIWindowManager.Anchor.TopLeft;
    #region Singleton
    public static UIDrawer Singleton { get; private set; }
    public UIDrawer()
    {
        if (Singleton == null)
        {
            Singleton = this;
            OnCreate();
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion

    string[] DataToRemove = new string[moveDataCapibility];
    Dictionary<string, UIDrawerData> mDrawDataMap = new Dictionary<string, UIDrawerData>();
    Dictionary<int, UIDrawerData> mDrawDataNumMap = new Dictionary<int, UIDrawerData>();        //为了优化性能，加入用数字做索引的Map
    Dictionary<int, GameObject> mPrefabs = new Dictionary<int, GameObject>();
    public int fontDefaultPrefabID = 0;
    public int spriteDefaultPrefabID = 0;
    public int numDefaultPrefabID = 0;

    public void OnCreate()
    {

    }

    public void AddPrefab(int prefabID, GameObject prefab)
    {
        if (!prefab)
        {
            Debug.LogError("Can't add null Label when labelID : " + prefabID.ToString());
        }
        mPrefabs[prefabID] = prefab;
    }

    public UIDrawerData CreateText(string id, int x, int y, string text, int prefabID)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UILabel label = data.uiWidget as UILabel;
        data.uiWidget.gameObject.transform.localScale = new Vector3(label.font.size, label.font.size, 1);
        data.IsLabel = true;
        label.text = text;
		data.Show();      //触发显示
        return data;
    }

    public UIDrawerData CreateSprite(string id, int x, int y, string spriteName, int prefabID, int depth)
    {
        UIDrawerData data = CreateWidget(id, x, y, prefabID);
        UISprite sprite = data.uiWidget as UISprite;
        data.IsLabel = false;
        sprite.spriteName = spriteName;
        sprite.depth = depth;
        data.uiWidget.gameObject.transform.localScale = new Vector3(sprite.GetAtlasSprite().paddingRight - sprite.GetAtlasSprite().paddingLeft, sprite.GetAtlasSprite().paddingTop - sprite.GetAtlasSprite().paddingBottom, 0);
        data.uiWidget.gameObject.transform.localPosition = new Vector3(x, -y, 0);
		data.Show();      //触发显示
        return data;
    }

    public UIDrawerData CreateNumber(string id, int x, int y, string imageNameStart, float xInterval, int maxIntLenth = 3, int floatLenth = 0)
    {
        UIDrawerData data = CreateWidget(id, x, y, numDefaultPrefabID);
		data.transform.localScale = new Vector3(1, 1, 1);
		
        Transform trans = data.transform.GetChild(0);
		UISprite sprite = trans.GetComponent<UISprite>();
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
            xPos += (int)xInterval;
			newObj.transform.localScale = trans.localScale;
            newObj.transform.LocalPositionX(xPos);
			newObj.transform.LocalPositionY(0.0f);
            newObj.SetActive(false);
			
        }
        return data;
    }

    public UIDrawerData CreateWidget(string id, int x, int y, int prefabID)
    {
        UIDrawerData data = new UIDrawerData();
        data.xPos = x;
        data.yPos = y;
        GameObject labelPrefab;
        if (!mPrefabs.TryGetValue(prefabID, out labelPrefab))
        {
            Debug.LogError("Use Undefine LabelID : " + prefabID.ToString() + " When Draw " + id);
            return null;
        }
        GameObject labelObj = GameObject.Instantiate(labelPrefab) as GameObject;
		UIWidget [] widgets = labelObj.GetComponents<UIWidget>();
		if(widgets.Length > 0)
        	data.uiWidget = widgets[0];
        data.transform = labelObj.transform;
        labelObj.transform.parent = UIWindowManager.Singleton.AnchorObject[(int)DefaultAnchor].transform;
        labelObj.transform.localPosition = new Vector3(x, -y, 0.0f);

        UIToolkits.SetUILayer(labelObj, 0);

        data.IsExistMode = true;
        mDrawDataMap[id] = data;
        return data;
    }

    public void DestoryObj(string id)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            Debug.LogWarning("Remove an not exist obj id = " + id);
        }
        GameObject.Destroy(data.transform.gameObject);
        mDrawDataMap.Remove(id);
    }

    public void DrawText(string id, int x, int y, string text, int labelID, float alpha)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateText(id, x, y, text, labelID);
            data.IsExistMode = false;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, -500f);
        }
        UILabel label = data.uiWidget as UILabel;
        if (label.text != text)
        {
            label.text = text;
        }
        label.Alpha(alpha);
        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, -500f);
        }
        data.DidDrawLastFrame = true;
    }

    public void DrawText(string id, int x, int y, string text)
    {
        DrawText(id, x, y, text, fontDefaultPrefabID, 1f);
    }

    public void DrawText(string stringKeyName, int x, int y)
    {
        DrawText(stringKeyName, x, y, TextTable.Singleton.GetString(stringKeyName), fontDefaultPrefabID, 1f);
    }

    public void DrawText(string id, int x, int y, string text, float alpha)
    {
        DrawText(id, x, y, text, fontDefaultPrefabID, alpha);
    }

    public UISprite DrawSprite(string id, int x, int y, string spriteName, int depth)
    {
        //Todo 每次画Sprite需要new一个Rect很慢
        return DrawSprite(id, new Rect(x, y, 0, 0), spriteName, spriteDefaultPrefabID, true, depth);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int prefabID, bool makePixedPerfect, int depth)
    {

        var x = (int)rect.x;
        var y = (int)rect.y;
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateSprite(id, (int)rect.x, (int)rect.y, spriteName, prefabID, depth);
            data.IsExistMode = false;
        }
        var sprite = data.uiWidget as UISprite;
        sprite.depth = depth;
        if (makePixedPerfect)
        {
            sprite.MakePixelPerfect();
        }
        else
        {
            data.uiWidget.gameObject.transform.localScale = new Vector3(rect.width, rect.height, 1f);       //Todo 性能问题
        }

        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, 0);			//Todo 性能问题
        }
        data.DidDrawLastFrame = true;
        return sprite;
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int prefabID)
    {
        return DrawSprite(id, rect, spriteName, prefabID, false, 0);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int depth, int prefabID)
    {
        return DrawSprite(id, rect, spriteName, prefabID, false, depth);
    }

    public UISprite DrawSprite(string id, Rect rect, string spriteName, int depth, int prefabID, bool makePixedPerfect)
    {
        return DrawSprite(id, rect, spriteName, prefabID, makePixedPerfect, depth);
    }

    public UISprite DrawSprite(string id, int x, int y, string spriteName)
    {
        return DrawSprite(id, x, y, spriteName, 0);
    }

    public void DrawPrefab(string id, int x, int y, int labelID)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateWidget(id, x, y, labelID);
            data.IsExistMode = false;
        }
        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.uiWidget.transform.localPosition = new Vector3(x, -y, 0.0f);
        }
        data.DidDrawLastFrame = true;
    }

    public void DrawNumber(string id, int x, int y, float numberValue, string imageNameStart, float xInterval, int maxIntLenth = 3, int floatLenth = 0)
    {
        UIDrawerData data;
        if (!mDrawDataMap.TryGetValue(id, out data))
        {
            data = CreateNumber(id, x, y, imageNameStart, xInterval, maxIntLenth, floatLenth);
            data.IsExistMode = false;
        }

        if (data.xPos != x || data.yPos != y)
        {
            data.xPos = x;
            data.yPos = y;
            data.transform.localPosition = new Vector3(x, -y, -100f);
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
            if (!pair.Value.IsExistMode && !pair.Value.DidDrawLastFrame)
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
}
