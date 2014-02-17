using System;

/// <summary>
/// Provides write access to various properties of our
/// Inventory Database's PurchasableItems, so that we can
/// populate them when retrieved from the underlying billing system.
/// Eg localized price.
/// </summary>
partial class PurchasableItem {
    internal class Writer {
        public static void setLocalizedPrice (PurchasableItem item, decimal price) {
            item.localizedPrice = price;
            item.localizedPriceString = price.ToString ();
        }

        public static void setLocalizedPrice (PurchasableItem item, string price) {
            item.localizedPriceString = price;
        }
        
        public static void setLocalizedTitle (PurchasableItem item, string title) {
            item.localizedTitle = title;
        }
        
        public static void setLocalizedDescription (PurchasableItem item, string description) {
            item.localizedDescription = description;
        }
    }
}
