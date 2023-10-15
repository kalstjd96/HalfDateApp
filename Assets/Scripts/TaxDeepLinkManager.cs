/**
 * @author minsung.kim@naviworks.com
 * @brief
 * @version 0.1
 * @date 2023-08-25 14:31:49Z
 *
 * @copyright Copyright 2023 Naviworks, Co., LTD. All rights reserved.
 *
 */
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class TaxDeepLinkManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] private AppsFlyerObjectScript appsFlyerObj;
    public TMP_Text title;
    #endregion

    private static TaxDeepLinkManager _instance;

    #region Properties
    public static Dictionary<string, object> DeepLinkParams { get => _instance.appsFlyerObj.DeepLinkParams; }
    #endregion

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (appsFlyerObj.DidReceivedDeepLink)
        {
            appsFlyerObj.DidReceivedDeepLink = false;
            title.text = "Success : " + ShowDeepLinkParams();
        }
    }

    //private IEnumerator CheckForDeepLinks()
    //{
    //    title.text = "Start DeepLink";
    //    float timeoutDuration = 10f; // Timeout duration in seconds
    //    float elapsedTime = 0f; // Elapsed time since coroutine started

    //    while (true)
    //    {
    //        if (elapsedTime >= timeoutDuration)
    //        {
    //            // Handle timeout condition
    //            title.text = "Timeout occurred : " + DeepLinkParams.Count;
    //            break;
    //        }

    //        if (appsFlyerObj.DidReceivedDeepLink)
    //        {
    //            appsFlyerObj.DidReceivedDeepLink = false;
    //            title.text = "Success : " + ShowDeepLinkParams();
    //            break;

    //        }

    //        elapsedTime += Time.deltaTime; // Update elapsed time

    //        yield return null; // Wait for the next frame
    //    }
    //}


    /// <summary>
    /// AppsFlyer 대시보드 입력된 아바타 파츠 데이터 값 가져오기
    /// </summary>
    /// <returns>partIndex:AvatarPartCategory Number_01:AvatarPartCategory Number_02:...</returns>
    public string ShowDeepLinkParams()
    {
        Dictionary<string, object> deepLinkParams = DeepLinkParams;
        //Dictionary<string, object> deepLinkParams = appsFlyerObj.DeepLinkParams;
        string text = null;

        if (deepLinkParams != null)
        {
            if (deepLinkParams.ContainsKey("deep_link_error"))
                text = "Deep link Error";
            else if (deepLinkParams.ContainsKey("deep_link_not_found"))
                text = "Deep link Not Found.";
            else
            {
                int i = 0;
                foreach (KeyValuePair<string, object> entry in deepLinkParams)
                {
                    if (i < deepLinkParams.Count)
                    {
                        if (entry.Value != null)
                            text += entry.Value.ToString() + ':';

                        i++;
                    }
                }
            }
            if (text.Substring(text.Length - 1, 1).Equals(":"))
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        return null;
       
    }
}