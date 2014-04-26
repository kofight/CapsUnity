using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unibill.Impl;

namespace Unibill.Impl {
    public interface IRawBillingPlatformProvider {
        IRawGooglePlayInterface getGooglePlay();

        IRawAmazonAppStoreBillingInterface getAmazon();

        IStoreKitPlugin getStorekit();
		IRawSamsungAppsBillingService getSamsung();
    }
}
