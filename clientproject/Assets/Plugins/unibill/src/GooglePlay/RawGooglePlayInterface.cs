//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using UnityEngine;

namespace Unibill.Impl {
    public class RawGooglePlayInterface : IRawGooglePlayInterface {

#if UNITY_ANDROID
        private AndroidJavaObject plugin;
#endif

        public void initialise(GooglePlayBillingService callback, string publicKey, string[] productIds) {
#if UNITY_ANDROID
            // Setup our GameObject to listen to events from the Java plugin.
            new GameObject().AddComponent<GooglePlayCallbackMonoBehaviour>().Initialise(callback);
            using (var pluginClass = new AndroidJavaClass("com.outlinegames.unibill.UniBill")) {
                plugin = pluginClass.CallStatic<AndroidJavaObject> ("instance");
            }
            plugin.Call("initialise", publicKey);
#endif
        }

        public void restoreTransactions() {
#if UNITY_ANDROID
            plugin.Call("restoreTransactions");
#endif
        }

        public void purchase(string id) {
#if UNITY_ANDROID
            plugin.Call("purchaseProduct", id);
#endif
        }
		
		public void pollForConsumables () {
#if UNITY_ANDROID
			plugin.Call ("pollForConsumables");
#endif
		}
    }
}
