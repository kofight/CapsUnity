//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using Uniject;

/// <summary>
/// Records purchase history for each <see cref="PurchasableItem"/>.
/// </summary>
public class TransactionDatabase {
    private IStorage storage;
    private ILogger logger;

    public string UserId { get; set; }

    public TransactionDatabase(IStorage storage, ILogger logger) {
        this.storage = storage;
        this.logger = logger;
        this.UserId = "default";
    }

    public int getPurchaseHistory (PurchasableItem item) {
        return storage.GetInt(getKey(item.Id), 0);
    }

    public void onPurchase (PurchasableItem item) {
        int previousCount = getPurchaseHistory (item);
        if (item.PurchaseType != PurchaseType.Consumable && previousCount != 0) {
            logger.LogWarning("Apparently multi purchased a non consumable:{0}", item.Id);
            return;
        }

        storage.SetInt(getKey(item.Id), previousCount + 1);
    }

    public void clearPurchases(PurchasableItem item) {
        storage.SetInt (getKey (item.Id), 0);
    }

    public void onRefunded(PurchasableItem item) {
        int previousCount = getPurchaseHistory(item);
        previousCount = Math.Max(0, previousCount - 1);
        storage.SetInt(getKey(item.Id), previousCount);
    }

    private string getKey(string fragment) {
        return string.Format("{0}.{1}", UserId, fragment);
    }
}
