//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using UnityEngine;
using Unibill.Impl;

[AddComponentMenu("")]
public class AmazonAppStoreCallbackMonoBehaviour : MonoBehaviour {

    public void Start() {
        gameObject.name = this.GetType().ToString();
        GameObject.DontDestroyOnLoad(this);
    }

    private AmazonAppStoreBillingService amazon;
    public void initialise(AmazonAppStoreBillingService amazon) {
        this.amazon = amazon;
    }

    public void onSDKAvailable(string isSandboxEnvironment) {
        amazon.onSDKAvailable(isSandboxEnvironment);
    }

    public void onGetItemDataFailed(string empty) {
        amazon.onGetItemDataFailed();
    }

    public void onProductListReceived(string productCSVString) {
        amazon.onProductListReceived(productCSVString);
    }

    public void onPurchaseFailed(string item) {
        amazon.onPurchaseFailed(item);
    }

    public void onPurchaseSucceeded (string item) {
        amazon.onPurchaseSucceeded(item);
    }

    public void onTransactionsRestored (string success) {
        amazon.onTransactionsRestored(success);
    }

    public void onPurchaseUpdateFailed(string empty) {
        amazon.onPurchaseUpdateFailed();
    }

    public void onPurchaseUpdateSuccess(string data) {
        amazon.onPurchaseUpdateSuccess(data);
    }

    public void onUserIdRetrieved(string userId) {
        amazon.onUserIdRetrieved(userId);
    }
}
