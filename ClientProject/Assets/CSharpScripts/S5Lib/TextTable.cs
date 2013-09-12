using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Webgame.Utility;
using System;
using System.IO;
using System.Text;

public class TextTable
{

    private ConfigOperator _textConfig;


    public string GetString(string key)
    {
        string value;
        if (!_textConfig.GetValue<string>(key, out value))
        {
            value = "key not found!";
        }
        return value;
    }

    public List<string[]> GetList(string fileName)
    {
        return _textConfig.ReadTextTable(ResManager.Singleton.LoadTextFile(fileName), new string[] { "\t", " " });
    }

    public void Write(string name, string value)
    {
        _textConfig.Write(name, value);
    }

    #region Singleton
    public static TextTable Singleton { get; private set; }
    public TextTable()
    {
        if (Singleton == null)
        {
            Singleton = this;
            _textConfig = new ConfigOperator();
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion

    public void AddTextMap(string fileName)                //
    {
        _textConfig.Read(ResManager.Singleton.LoadTextFile(fileName), false);
    }

    public void Clear()                                    //
    {
        _textConfig.Dispose();
    }

}
