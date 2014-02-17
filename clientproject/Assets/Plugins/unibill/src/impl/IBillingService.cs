//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;

namespace Unibill.Impl
{
    /// <summary>
    /// Represents the public interface of the underlying billing system such as Google Play,
    /// or the Apple App store.
    /// </summary>
    public interface IBillingService {

        /// <summary>
        /// Initialise the instance using the specified <see cref="IBillingServiceCallback"/>.
        /// </summary>
        void initialise(IBillingServiceCallback biller);

        void purchase(string item);
        void restoreTransactions();
    }
}

