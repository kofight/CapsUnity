//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using Unibill.Impl;

namespace Tests {
    public class FakeBillingService : IBillingService {

        private IBillingServiceCallback biller;
        private List<string> purchasedItems = new List<string>();
        private ProductIdRemapper remapper;
        public bool reportError;
        public bool reportCriticalError;

        public FakeBillingService (ProductIdRemapper remapper) {
            this.remapper = remapper;
        }

        public void initialise (IBillingServiceCallback biller) {
            this.biller = biller;
            if (reportError) {
                biller.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_FAILED);
            }
            biller.onSetupComplete(!reportCriticalError);
        }

        public bool purchaseCalled;
        public void purchase (string item) {
            purchaseCalled = true;
            // Our billing systems should only keep track of non consumables.
            if (remapper.getPurchasableItemFromPlatformSpecificId (item).PurchaseType == PurchaseType.NonConsumable) {
                purchasedItems.Add (item);
            }
            this.biller.onPurchaseSucceeded(item);
        }

        public bool restoreCalled;
        public void restoreTransactions () {
            restoreCalled = true;
            foreach (var item in purchasedItems) {
                biller.onPurchaseSucceeded(item);
            }
            this.biller.onTransactionsRestoredSuccess();
        }
    }
}
