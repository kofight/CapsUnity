//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using Unibill;
using Unibill.Impl;
using UnityEngine;

[AddComponentMenu("")] // Hide it from the component menu
public class AppleAppStoreCallbackMonoBehaviour : MonoBehaviour {

    public void Awake() {
        gameObject.name = this.GetType().ToString();
        DontDestroyOnLoad(this);
        
    }

    private AppleAppStoreBillingService callback;
    public void initialise(AppleAppStoreBillingService callback) {
        this.callback = callback;
    }

    public void onProductListReceived(string productList) {
        callback.onProductListReceived(productList);
    }

    public void onProductPurchaseSuccess(string productId) {
        callback.onPurchaseSucceeded(productId);
    }

    public void onProductPurchaseCancelled(string productId) {
        callback.onPurchaseCancelled(productId);
    }

    public void onProductPurchaseFailed(string productId) {
        callback.onPurchaseFailed(productId);
    }

    public void onTransactionsRestoredSuccess(string empty) {
        callback.onTransactionsRestoredSuccess();
    }

    public void onTransactionsRestoredFail(string error) {
        callback.onTransactionsRestoredFail(error);
    }

    public void onFailedToRetrieveProductList(string nop) {
        callback.onFailedToRetrieveProductList();
    }
}
