using System;
using System.Collections;
using UnityEngine;

using GoogleMobileAds.Api;

namespace CBGames
{
    public class AdmobController : MonoBehaviour
    {
        [Header("Banner Id")]
        public string androidBannerId = "";
        public string iOSBannerId = "";
        public AdPosition bannerPosition = AdPosition.Bottom;

        [Header("Interstitial Ad Id")]
        public string androidInterstitialId = "";
        public string iOSInterstitialId = "";

        [Header("Rewarded Base Video Id")]
        public string androidRewardedBaseVideoId = "";
        public string iOSRewardedBaseVideoId = "";

        private BannerView bannerView = null;
        private InterstitialAd interstitial = null;
        private RewardBasedVideoAd rewardBasedVideo = null;
        private bool isCompletedRewardedVideo = false;

        private void Awake()
        {
            // Get singleton reward based video ad reference.
            rewardBasedVideo = RewardBasedVideoAd.Instance;
        }

        private void Start()
        {
            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        }


        /// <summary>
        /// Hide the current banner ad
        /// </summary>
        public void HideBanner()
        {
            bannerView.Hide();
        }


        /// <summary>
        /// Load and show a banner ad
        /// </summary>
        public void LoadAndShowBanner(float delay)
        {
            StartCoroutine(CRLoadAndShowBanner(delay));
        }
        private IEnumerator CRLoadAndShowBanner(float delay)
        {
            yield return new WaitForSeconds(delay);
            // Clean up banner ad before creating a new one.
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            // Create a 320x50 banner at the top of the screen.
#if UNITY_EDITOR
            bannerView = new BannerView("unused", AdSize.SmartBanner, bannerPosition);
#elif UNITY_ANDROID
            bannerView = new BannerView(androidBannerId, AdSize.SmartBanner, bannerPosition);
#elif UNITY_IOS
            bannerView = new BannerView(iOSBannerId, AdSize.SmartBanner, bannerPosition);
#endif
            // Load banner ad.
            bannerView.LoadAd(new AdRequest.Builder().Build());
        }

        /// <summary>
        /// Request interstitial ad
        /// </summary>
        public void RequestInterstitial()
        {
            // Clean up interstitial ad before creating a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            // Create an interstitial.
#if UNITY_EDITOR
            interstitial = new InterstitialAd("unused");
#elif UNITY_ANDROID
            interstitial = new InterstitialAd(androidInterstitialId);
#elif UNITY_IOS
            interstitial = new InterstitialAd(iOSInterstitialId);
#endif
            // Register for ad events.
            interstitial.OnAdClosed += HandleInterstitialClosed;

            // Load an interstitial ad.
            interstitial.LoadAd(new AdRequest.Builder().Build());
        }

        /// <summary>
        /// Request rewarded video ad
        /// </summary>
        public void RequestRewardedVideo()
        {
#if UNITY_EDITOR
            rewardBasedVideo.LoadAd(new AdRequest.Builder().Build(), "unused");
#elif UNITY_ANDROID
            rewardBasedVideo.LoadAd(new AdRequest.Builder().Build(), androidRewardedBaseVideoId);
#elif UNITY_IOS
            rewardBasedVideo.LoadAd(new AdRequest.Builder().Build(), iOSRewardedBaseVideoId);
#endif
        }

        /// <summary>
        /// Determine whether the interstitial ad is ready
        /// </summary>
        /// <returns></returns>
        public bool IsInterstitialReady()
        {
            if (interstitial.IsLoaded())
            {
                return true;
            }
            else
            {
                RequestInterstitial();
                return false;
            }
        }

        /// <summary>
        /// Show interstitial ad with given delay time
        /// </summary>
        /// <param name="delay"></param>
        public void ShowInterstitial(float delay)
        {
            StartCoroutine(CRShowInterstitial(delay));
        }
        private IEnumerator CRShowInterstitial(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }
            else
            {
                RequestInterstitial();
            }
        }


        /// <summary>
        /// Determine whether the rewarded video ad is ready
        /// </summary>
        /// <returns></returns>
        public bool IsRewardedVideoReady()
        {
            if (rewardBasedVideo.IsLoaded())
            {
                return true;
            }
            else
            {
                RequestRewardedVideo();
                return false;
            }
        }

        /// <summary>
        /// Show rewarded video ad with given delay time 
        /// </summary>
        /// <param name="delay"></param>
        public void ShowRewardedVideo(float delay)
        {
            StartCoroutine(CRShowRewardedVideoAd(delay));
        }
        IEnumerator CRShowRewardedVideoAd(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if (rewardBasedVideo.IsLoaded())
            {
                rewardBasedVideo.Show();
            }
            else
            {
                RequestRewardedVideo();
            }
        }

        //Events callback
        private void HandleInterstitialClosed(object sender, EventArgs args)
        {
            RequestInterstitial();
        }

        private void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            //User watched the whole video
            isCompletedRewardedVideo = true;
        }

        private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            //User closed the video
            ServicesManager.Instance.AdManager.OnRewardedVideoClosed(isCompletedRewardedVideo);
            isCompletedRewardedVideo = false;
            RequestRewardedVideo();
        }
    }
}