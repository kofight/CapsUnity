//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;

namespace Unibill.Impl {

    /// <summary>
    /// Raw interface for the Unibill native iOS plugin.
    /// </summary>
    public interface IStoreKitPlugin {

        void initialise(AppleAppStoreBillingService callback);

        bool storeKitPaymentsAvailable();
        void storeKitRequestProductData (string productIdentifiers, string[] productIds);
        void storeKitPurchaseProduct (string productId);
        void storeKitRestoreTransactions();
    }
}
