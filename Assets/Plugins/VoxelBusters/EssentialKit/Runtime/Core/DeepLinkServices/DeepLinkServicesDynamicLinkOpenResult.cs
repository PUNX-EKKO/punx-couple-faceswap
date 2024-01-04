using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// This class contains the information retrieved when deep link is opened.
    /// </summary>
    public class DeepLinkServicesDynamicLinkOpenResult
    {
        #region Properties

        /// <summary>
        /// The received notification.
        /// </summary>
        public Uri Url { get; private set; }

        public string RawUrlString { get; private set; }

        #endregion

        #region Constructors

        internal DeepLinkServicesDynamicLinkOpenResult(Uri url, string rawUrlString)
        {
            // Set properties
            Url             = url;
            RawUrlString    = rawUrlString;
        }

        #endregion
    }
}