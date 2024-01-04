using System;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit.DeepLinkServicesCore;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// Provides cross-platform interface to handle deep links.    
    /// </summary>
    /// <description>
    public static class DeepLinkServices
    {
        #region Static fields

        [ClearOnReload]
        private     static  INativeDeepLinkServicesInterface    s_nativeInterface;

        #endregion

        #region Static properties

        public static DeepLinkServicesUnitySettings UnitySettings { get; private set; }

        public static IDeepLinkServicesDelegate Delegate { get; set; }

        #endregion

        #region Static events

        /// <summary>
        /// Event that will be called when url scheme is opened.
        /// </summary>
        public static event Callback<DeepLinkServicesDynamicLinkOpenResult> OnCustomSchemeUrlOpen;

        /// <summary>
        /// Event that will be called when universal link is opened.
        /// </summary>
        public static event Callback<DeepLinkServicesDynamicLinkOpenResult> OnUniversalLinkOpen;

        #endregion

        #region Setup methods

        public static bool IsAvailable()
        {
            return (s_nativeInterface != null) && s_nativeInterface.IsAvailable;
        }

        internal static void Initialize(DeepLinkServicesUnitySettings settings)
        {
            Assert.IsArgNotNull(settings, nameof(settings));

            // Reset event properties
            OnCustomSchemeUrlOpen   = null;
            OnUniversalLinkOpen     = null;

            // Set properties
            UnitySettings           = settings;

            // Configure interface
            s_nativeInterface       = NativeFeatureActivator.CreateInterface<INativeDeepLinkServicesInterface>(ImplementationSchema.DeepLinkServices, settings.IsEnabled);
            s_nativeInterface.SetCanHandleCustomSchemeUrl(handler: CanHandleCustomSchemeUrl);
            s_nativeInterface.SetCanHandleUniversalLink(handler: CanHandleUniversalLink);
            s_nativeInterface.OnCustomSchemeUrlOpen    += HandleOnCustomSchemeUrlOpen;
            s_nativeInterface.OnUniversalLinkOpen      += HandleOnUniversalLinkOpen;
            s_nativeInterface.Init();
        }

        private static bool CanHandleCustomSchemeUrl(string url)
        {
            return (Delegate == null) || Delegate.CanHandleCustomSchemeUrl(new Uri(url));
        }

        private static bool CanHandleUniversalLink(string url)
        {
            return (Delegate == null) || Delegate.CanHandleUniversalLink(new Uri(url));
        }

        #endregion

        #region Callback methods

        private static void HandleOnCustomSchemeUrlOpen(string url)
        {
            DebugLogger.Log(EssentialKitDomain.Default, $"Detected url scheme: {url}");

            // notify listeners
            var     result      = new DeepLinkServicesDynamicLinkOpenResult(new Uri(url), url);
            CallbackDispatcher.InvokeOnMainThread(OnCustomSchemeUrlOpen, result);
        }

        private static void HandleOnUniversalLinkOpen(string url)
        {
            DebugLogger.Log(EssentialKitDomain.Default, $"Detected universal link: {url}");

            // notify listeners
            var     result      = new DeepLinkServicesDynamicLinkOpenResult(new Uri(url), url);
            CallbackDispatcher.InvokeOnMainThread(OnUniversalLinkOpen, result);
        }

        #endregion
    }
}