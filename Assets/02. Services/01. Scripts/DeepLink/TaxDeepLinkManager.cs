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
using UnityEngine.Localization;

namespace SCM.Service.TaxSquare.ModelTaxPlayer
{
    public class TaxDeepLinkManager : MonoBehaviour
    {
        #region LocalizedString
        //private LocalizedString NOTICE_TITLE = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_001_01_MODELTAXPLAYER_NOTICE_01");

        //[StringContext(prefix = "SCM2_TX_005_01", postfix = "00", initialKo = "Ȯ��")]
        //private LocalizedString CONFIRM = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_005_01_MODELTAXPLAYER_CONFIRM_01");

        //[StringContext(prefix = "SCM2_TX_005_01", postfix = "01", initialKo = "��������ڰ� �ƴմϴ�.")]
        //private LocalizedString RESULT_FAIL = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_005_01_MODELTAXPLAYER_RESULT_FAIL_02");

        //[StringContext(prefix = "SCM2_TX_005_01", postfix = "02", initialKo = "�������� ȹ���Ͽ����ϴ�! ������ â�� Ȯ���ϼ���.")]
        //private LocalizedString REWARD = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_005_01_MODELTAXPLAYER_REWARD_03");

        //[StringContext(prefix = "SCM2_TX_005_01", postfix = "03", initialKo = "���� ȹ�濡 �����Ͽ����ϴ�. �ٽ� �õ����ּ���.")]
        //private LocalizedString REWARD_FAIL = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_005_01_MODELTAXPLAYER_REWARD_FAIL_04");

        //[StringContext(prefix = "SCM2_TX_005_01", postfix = "04", initialKo = "����")]
        //private LocalizedString ERROR = new LocalizedString(LocalizationDefines.DefaultStringTableName, "SCM2_TX_005_01_MODELTAXPLAYER_ERROR_05");

        #endregion

        private static TaxDeepLinkManager _instance;

        //#region Properties
        //public static Dictionary<string, object> DeepLinkParams { get => _instance.appsFlyerObj.DeepLinkParams; }
        //#endregion

        //private void Awake()
        //{
        //    if (_instance == null)
        //        _instance = this;
        //    else
        //        Destroy(this);
        //}



        #region Non_public
        private AppsFlyerObjectScript appsFlyerObject;
        private const string caseUrl = "tax/mobum";
        private const string DeepLinkError = "Deep link Error";
        private const string DeepLinkNotFound = "Deep link Not Found.";
        #endregion

        private void Awake()
        {
            appsFlyerObject = GetComponent<AppsFlyerObjectScript>();
        }

        private void Start()
        {
            StartCoroutine(CheckForDeepLinks());
        }

        /// <summary>
        /// QR ���� ���� Ȯ��
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckForDeepLinks()
        {
            float timeoutDuration = 5f;
            float elapsedTime = 0f;

            while (true)
            {
                if (elapsedTime >= timeoutDuration)
                    break;

                if (appsFlyerObject.DidReceivedDeepLink)
                {
                    appsFlyerObject.DidReceivedDeepLink = false;
                    //CheckForModelTaxPlayer();
                    break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

       

        /// <summary>
        /// �ƹ�Ÿ ���� ������ �� ����
        /// </summary>
        //private IEnumerator UnlockModelTaxPlayerItemPartsCoroutine()
        //{
        //    string deepLinkInformation = ShowDeepLinkParams();
        //    int itemIndex = int.Parse(deepLinkInformation.Split(':')[0]);
        //    AvatarPartCategory[] avatarPartCategoryIndex = new AvatarPartCategory[deepLinkInformation.Split(':').Length - 1];

        //    for (int i = 0; i < avatarPartCategoryIndex.Length; i++)
        //    {
        //        avatarPartCategoryIndex[i] = avatarPart_10Parts[int.Parse(deepLinkInformation.Split(':')[i + 1].Trim())];
        //    }

        //    int routineCounter = 0;
        //    bool hasError = false;

        //    foreach (var category in avatarPartCategoryIndex)
        //    {
        //        routineCounter += 1;
        //        AvatarManager.Instance.AcquirePart(itemIndex, category, (result, errorMsg) =>
        //        {
        //            routineCounter -= 1;
        //            if (result != APIResult.SUCCESS)
        //                hasError = true;
        //        });
        //    }

        //    yield return null;
        //    yield return new WaitUntil(() => routineCounter <= 0);

        //    if (hasError)
        //    {
        //        UIPopup.Instance.OpenNoticePopup(new PopupInfo()
        //        {
        //            title = "�˸�",
        //            content = "ȹ�� ���� �ٽ� �õ����ּ���",
        //            okTitle = "Ȯ��",
        //            onOkPressed = () => UIPopup.Instance.ClosePopup()
        //        });
        //    }
        //    else
        //    {
        //        UIPopup.Instance.OpenNoticePopup(new PopupInfo()
        //        {
        //            title = "�˸�",
        //            content = "�������� ȹ���߽��ϴ�. ������ â�� Ȯ���ϼ���.",
        //            okTitle = "Ȯ��",
        //            onOkPressed = () => UIPopup.Instance.ClosePopup()
        //        });
        //    }
        //}

        /// <summary>
        /// AppsFlyer ��ú��� �Էµ� �ƹ�Ÿ ���� ������ �� ��������
        /// </summary>
        /// <returns>partIndex:AvatarPartCategory Number_01:AvatarPartCategory Number_02:...</returns>
        private string ShowDeepLinkParams()
        {
            Dictionary<string, object> deepLinkParams = appsFlyerObject.DeepLinkParams;
            string text = null;

            if (deepLinkParams != null)
            {
                if (deepLinkParams.ContainsKey("deep_link_error"))
                    text = DeepLinkError;
                else if (deepLinkParams.ContainsKey("deep_link_not_found"))
                    text = DeepLinkNotFound;
                else
                {
                    foreach (KeyValuePair<string, object> entry in deepLinkParams)
                    {
                        if (entry.Value != null)
                            text += entry.Value.ToString() + ':';
                    }
                }
            }

            if (text != null && text.EndsWith(":"))
                text = text.Substring(0, text.Length - 1);

            return text;
        }
    }
}