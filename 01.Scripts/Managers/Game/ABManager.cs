using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Sirenix.OdinInspector;

public class ABManager : SerializedMonoBehaviour
{

    public static ABManager instance;

    public static Dictionary<string, string> AB_Dic = new Dictionary<string, string>();

    private void Awake()
    {
        instance = this;
    }

    public void SelectStart(string word)
    {
        if (ES3.KeyExists("NextStageEnable"))
            if (ES3.Load<bool>("NextStageEnable"))
            {
                return;
            }
            else
            {
                StartIdleFirst();

                return;
            }


        switch (word)
        {
            case "A":
                StartRunFirst();
                break;

            case "B":
                StartIdleFirst();
                break;

            default:

                break;

        }

        void StartRunFirst()
        {
            print("save");
            ES3.Save<string>("AB_Test", "A");

            IdleManager.instance.blackPanel.SetActive(false);
            RunManager.instance.blackPanel.SetActive(false);
        }

        void StartIdleFirst()
        {
            ES3.Save<string>("AB_Test", "B");

            if (!ES3.KeyExists("NextStageEnable"))
                ES3.Load<bool>("NextStageEnable", false);

            RunManager.instance.StartIdleFirst();

            this.TaskDelay(1f, () =>
            {
                IdleManager.instance.blackPanel.SetActive(false);
                RunManager.instance.blackPanel.SetActive(false);
            });

        }
    }

    public struct userAttributes { }
    public struct appAttributes { }

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    async Task Start()
    {
        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());

        // SelectStart(RemoteConfigService.Instance.appConfig.GetString("StartSelect"));

        AB_Dic.Add("ForceIdle", RemoteConfigService.Instance.appConfig.GetString("ForceIdle"));
        AB_Dic.Add("CandyStackType", RemoteConfigService.Instance.appConfig.GetString("CandyStackType"));

        SelectStart("A");

        foreach (var ab in AB_Dic)
        {
            if (ab.Key.Equals("ForceIdle"))
            {
                RunManager.instance.SetForceIdle(RemoteConfigService.Instance.appConfig.GetBool("ForceIdle"));
                EventManager.instance.CustomEvent(AnalyticsType.AB_TEST, "ForceIdle_" + ab.Value);

                print("ForceIdle_" + ab.Value);
            }

            if (ab.Key.Equals("CandyStackType"))
            {
                RunManager.instance.SetCandyArarngeType((CandyArrangeType)System.Enum.Parse(typeof(CandyArrangeType), ab.Value));
                EventManager.instance.CustomEvent(AnalyticsType.AB_TEST, "CandyStackType" + RemoteConfigService.Instance.appConfig.GetBool("CandyStackType"));

                ES3.Save<CandyArrangeType>("CandyArrangeType", (CandyArrangeType)System.Enum.Parse(typeof(CandyArrangeType), ab.Value));
            }

            print("RemoteConfigService.Instance.appConfig fetched: " + ab.Key + " - " + ab.Value);
        }
    }
}
