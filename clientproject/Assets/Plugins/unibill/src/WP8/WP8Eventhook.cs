using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unibill.Impl;

class WP8Eventhook : MonoBehaviour {

    public WP8BillingService callback = null;

    public void Start() {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public void OnApplicationPause(bool paused) {
        if (!paused && callback != null) {
            callback.enumerateLicenses();
        }
    }
}
