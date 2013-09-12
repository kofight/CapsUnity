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

    public event EventHandler<UIMouseClick.ClickArgs> OnCheckEvent;

    public void SetCheckBox(UICheckbox checkbox, object userdata = null)
    {
        UIToolkits.AddChildComponentMouseClick(checkbox.gameObject, OnCheck);
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
        UICheckbox ckbox = _checkboxs.FirstOrDefault(p => p.UserData.ToString() == userdata.ToString()).CheckBox;
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

    private void OnCheck(object sender, UIMouseClick.ClickArgs arg)
    {
        if (_checkboxs == null || _checkboxs.Count < 1)
            return;
        GameObject check = arg.UserState as GameObject;
        if (check == null)
            return;
        foreach (var item in _checkboxs)
        {
            if (item.CheckBox.isChecked)
                item.CheckBox.isChecked = false;
        }
       
        UICheckbox ckbox = check.GetComponent<UICheckbox>();
        ckbox.isChecked = true;
        _currentData = _checkboxs.FirstOrDefault(p => p.CheckBox.name == ckbox.name).UserData;
        if (OnCheckEvent != null)
            OnCheckEvent(sender, arg);
    }

    private class CheckBoxItem
    {
        public UICheckbox CheckBox { get; set; }

        public object UserData { get; set; }
    }
}

