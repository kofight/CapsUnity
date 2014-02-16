//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Unibill.Impl {



    /// <summary>
    /// Callback interface for <see cref="IBillingService"/>s.
    /// </summary>
    public interface IBillingServiceCallback {
        void onSetupComplete(bool successful);

        void onPurchaseSucceeded(string platformSpecificId);
        // This variant should be called when we have a receipt for the purchase.
        void onPurchaseSucceeded(string platformSpecificId, string receipt);
        void onPurchaseCancelledEvent(string item);
        void onPurchaseRefundedEvent(string item);
        void onPurchaseFailedEvent(string item);

        void onTransactionsRestoredSuccess();
        void onTransactionsRestoredFail(string error);

        void logError(UnibillError error, params object[] args);
        void logError(UnibillError error);
    }
}
