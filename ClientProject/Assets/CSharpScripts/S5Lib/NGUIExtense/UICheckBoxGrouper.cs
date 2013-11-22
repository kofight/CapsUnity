using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class UICheckBoxGrouper
{
    private List<CheckBoxItem> _checkboxs;

    public UICheckBoxGrouper(object defaultvalue)
    {
        _checkboxs = new List<CheckBoxItem>();
        _currentData = defaultvalue;
    }

    public void SetCheckBox(UIToggle checkbox, object userdata = null)
    {
		if(checkbox == null) return;
		EventDelegate.Set(checkbox.onChange, OnCheck);
        _checkboxs.Add(new CheckBoxItem { CheckBox = checkbox, UserData = userdata });
    }

    public T GetCheckData<T>()
    {
        if (_currentData == null)
            return default(T);
        return (T)_currentData;
    }

    public void SetChecked(object userdata)
    {
        if (_checkboxs == null || _checkboxs.Count < 1)
            return;
        UIToggle ckbox = _checkboxs.FirstOrDefault(p => p.UserData.ToString() == userdata.ToString()).CheckBox;
        if (ckbox == null)
            return;
        foreach (var item in _checkboxs)
        {
            if (item.CheckBox.isChecked)
                item.CheckBox.isChecked = false;
        }
        ckbox.isChecked = true;
    }

    object _currentData;

    private void OnCheck()
    {
        if (_checkboxs == null || _checkboxs.Count < 1)
            return;
        foreach (var item in _checkboxs)
        {
            if (item.CheckBox.isChecked)
                item.CheckBox.isChecked = false;
        }
       
        _currentData = _checkboxs.FirstOrDefault(p => p.CheckBox == UIToggle.current).UserData;
    }

    private class CheckBoxItem
    {
        public UIToggle CheckBox { get; set; }

        public object UserData { get; set; }
    }
}

