using System.Collections.Generic;
using UnityEngine;
using CBGames;

namespace CBGames
{
    public class AdManager : MonoBehaviour
    {
        public int interstitialAdPeriodInSecond;

        private AdmobController admobController = null;
        private bool isCalledback = false;
        private bool isRewarded = false;
        private void OnEnable()
        {
            IngameManager.GameStateChanged += GameManager_GameStateChanged;
        }

        private void OnDisable()
        {
            IngameManager.GameStateChanged -= GameManager_GameStateChanged;
        }

        // Use this for initialization
        void Start()
        {
            admobController = GetComponent<AdmobController>();
            GTil.Init(this);

            //Show banner ad
            admobController.LoadAndShowBanner(0);


            //Request interstitial ads
            admobController.RequestInterstitial();

            //Request rewarded video 
            admobController.RequestRewardedVideo();
        }

        private void Update()
        {
            if (isCalledback)
            {
                isCalledback = false;
                if (isRewarded)
                {
                    IngameManager.Instance.SetContinueGame();
                }
                else
                {
                    IngameManager.Instance.GameOver();
                }
            }
        }

        private float lastShowAd = int.MinValue;
        private void GameManager_GameStateChanged(IngameState obj)
        {
            if (obj == IngameState.Ingame_CompletedLevel || obj == IngameState.Ingame_GameOver)
            {
                if (Time.time - lastShowAd < interstitialAdPeriodInSecond) return;
                if (!admobController.IsInterstitialReady())
                {
                    admobController.RequestInterstitial();
                    return;
                }

                admobController.ShowInterstitial(0);
                lastShowAd = Time.time;
            }
        }


        /// <summary>
        /// Determines whether rewarded video ad is ready.
        /// </summary>
        /// <returns></returns>
        public bool IsRewardedVideoAdReady()
        {
            return admobController.IsRewardedVideoReady();
        }


        /// <summary>
        /// Show the rewarded video ad with delay time
        /// </summary>
        /// <param name="delay"></param>
        public void ShowRewardedVideoAd()
        {
            admobController.ShowRewardedVideo(0);
        }

        public void OnRewardedVideoClosed(bool isFinishedVideo)
        {
            isCalledback = true;
            isRewarded = isFinishedVideo;
        }
    }
}
