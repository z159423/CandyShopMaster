using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAdsBtn : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject btn;
    void Start()
    {
        MondayOFF.AdsManager.OnAfterInterstitial += Event;

        if (!MondayOFF.NoAds.IsNoAds && ES3.KeyExists("IS_Showend") ? ES3.Load<bool>("IS_Showend") : false)
            btn.SetActive(true);

        MondayOFF.IAPManager.OnAfterPurchase += (isSuccess) => OnPurcahseNoAds();
    }

    void Event()
    {
        if (!MondayOFF.NoAds.IsNoAds) btn.SetActive(true);
    }

    void OnPurcahseNoAds()
    {
        if (btn != null)
            btn.SetActive(false);
    }


    public void OnClickNoAdsBtn()
    {
        MondayOFF.NoAds.Purchase();
    }

    private void OnDestroy()
    {
        MondayOFF.AdsManager.OnAfterInterstitial -= Event;
        MondayOFF.IAPManager.OnAfterPurchase -= (isSuccess) => OnPurcahseNoAds();
    }
}
