//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;
//using System.Collections;
//using TMPro;
//using DG.Tweening;

//namespace SCM.Service.TaxSquare
//{
//    public class ChatingManager : MonoBehaviour
//    {
//        [SerializeField] private Transform chatScrollViewTrans;
//        [SerializeField] private ScrollRect chatScrollRect;
//        [SerializeField] private ContentSizeFitter scrollFitter;
//        [SerializeField] private TMP_InputField chatInput;
//        [SerializeField] private Button sendButton;
//        [SerializeField] private GameObject sendChatItemPrefab;
//        [SerializeField] private GameObject answerChatItemPrefab;
//        [SerializeField] private RectTransform viewport;
//        private RectTransform chatinputRect;

//        private Coroutine cursorCoroutine;

//        private Vector2 scrollRectOriginalPosition;
//        private Vector2 inputOriginalPosition;

//        //API Settings ����
//        private const string X_APIKEY = "mk-9c815385-53e4-435e-8aef-60ac634846aa";

//        //���� ��ū ���� Ȯ�� �� ��� ���� �� True
//        private bool isOn = true;

//        private TouchScreenKeyboard keyboard = null;

//        //����� Ű���� ����
//        bool isKeyboard = false;

//        protected void OnEnable()
//        {
//            chatInput.text = "";
//            ClearChatItems();

//            GameObject answerChatObj = Instantiate(answerChatItemPrefab, chatScrollViewTrans);
//            ChatbotAnswerItem answerItem = answerChatObj.GetComponent<ChatbotAnswerItem>();

//            if (isOn)
//                answerItem.SetItem("�ȳ��ϼ���. Tax GPT�Դϴ�. ������ ���õ� ������ �غ�����.");
//            else
//                answerItem.SetItem("�̹� �� �ٹ� �ð��� �� ä�����. ���� �޿� �湮���ּ���.");

//            chatInput.interactable = isOn;
//            sendButton.interactable = isOn;
//        }


//        protected override void OnDisable()
//        {
//            base.OnDisable();
//        }

//        private void Start()
//        {
//            chatinputRect = chatInput.transform.parent.GetComponent<RectTransform>();
//            scrollRectOriginalPosition = viewport.offsetMin;
//            inputOriginalPosition = chatinputRect.offsetMin;

//            //Ű���� â���� Ȯ�� ��ư ������ �� �߻��ϴ� �̺�Ʈ
//            chatInput.onSubmit.AddListener(delegate { SendChat(); });

//            //Ű����â�� ������� �� �߻��ϴ� �̺�Ʈ
//            //chatInput.onEndEdit.AddListener(delegate { SendChat(); });

//            if (keyboard == null || !TouchScreenKeyboard.visible)
//                openAndroidKeyboard();
//        }

//        public void openAndroidKeyboard()
//        {
//            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
//            Invoke("hideInputField", 0.1f);
//        }

//        private void hideInputField()
//        {
//            //iOS : inputField â Hide Mobile Input Ȱ��ȭ �ʿ�
//            TouchScreenKeyboard.hideInput = true;
//        }

//        private void Update()
//        {
//            if (TouchScreenKeyboard.visible)
//            {
//                isKeyboard = true;
//                ApplyScrollPosition();
//            }
//            else
//            {
//                if (isKeyboard)
//                {
//                    isKeyboard = false;
//                    //viewport.offsetMin = scrollRectOriginalPosition;
//                    //chatinputRect.offsetMin = inputOriginalPosition;
//                    float oriviewport = scrollRectOriginalPosition.y;
//                    viewport.offsetMin = new Vector2(viewport.offsetMin.x, oriviewport);

//                    float orichatinputRect = inputOriginalPosition.y;
//                    chatinputRect.transform.DOMoveY(orichatinputRect, 0.1f);
//                }
//            }
//        }

//        #region �߰��� ��
//        public void ApplyScrollPosition()
//        {
//            //float newBottomPosition = GetRelativeKeyboardHeight(viewport.transform.parent.transform.GetComponent<RectTransform>(), false);
//            float newBottomPosition = viewport.offsetMin.y + GetRelativeKeyboardHeight(chatScrollRect.transform.parent.transform.GetComponent<RectTransform>(), false);
//            viewport.offsetMin = new Vector2(viewport.offsetMin.x, newBottomPosition);
//            //viewport.offsetMax = new Vector2(viewport.offsetMax.x, newBottomPosition);

//            //float newBottomPositionChat = GetRelativeKeyboardHeight(chatinputRect.transform.parent.transform.GetComponent<RectTransform>(), false);
//            float newBottomPositionChat = chatinputRect.transform.position.y + GetRelativeKeyboardHeight(chatinputRect.transform.parent.transform.GetComponent<RectTransform>(), false);
//            chatinputRect.transform.DOMoveY(newBottomPositionChat, 0.1f);

//            StartCoroutine(ApplyScrollPositionDelayed());
//        }

//        public static int GetRelativeKeyboardHeight(RectTransform rectTransform, bool includeInput)
//        {
//            int keyboardHeight = GetKeyboardHeight(includeInput);
//            float screenToRectRatio = Screen.height / rectTransform.rect.height;
//            float keyboardHeightRelativeToRect = keyboardHeight / screenToRectRatio;

//            return (int)keyboardHeightRelativeToRect;
//        }

       

//        private IEnumerator ApplyScrollPositionDelayed()
//        {
//            yield return new WaitForEndOfFrame();

//            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatScrollRect.transform);
//            chatScrollRect.normalizedPosition = new Vector2(0, 0);
//        }
//        #endregion

//        private void ClearChatItems()
//        {
//            foreach (Transform chatItem in chatScrollViewTrans)
//                Destroy(chatItem.gameObject);

//            chatInput.interactable = true;
//            sendButton.interactable = true;
//        }

//        private void SendChat(string content = null)
//        {
//            if (string.IsNullOrEmpty(content))
//            {
//                if (string.IsNullOrEmpty(chatInput.text))
//                    return;
//                else
//                    content = chatInput.text;
//            }

//            if (cursorCoroutine != null)
//            {
//                StopCoroutine(cursorCoroutine);
//                cursorCoroutine = null;
//            }

//            PrintChat(content);

//            //API Settings ����
//            string url = $"http://61.38.144.248:7000/chats/ask?query={UnityWebRequest.EscapeURL(content)}&streaming=true&chat_id=ecac18eb-0cbd-4963-9afb-39c510944c7e";

//            chatInput.text = "";
//            StartCoroutine(GetRequest(url));
//        }

//        IEnumerator GetRequest(string url)
//        {
//            GameObject answerChatObj = Instantiate(answerChatItemPrefab, chatScrollViewTrans);
//            answerChatObj.SetActive(false);
//            yield return new WaitForSeconds(2f);
//            answerChatObj.SetActive(true);
//            ChatbotAnswerItem answerItem = answerChatObj.GetComponent<ChatbotAnswerItem>();
//            answerItem.SetItem(" ");
//            StartCoroutine(RefreshLayoutCoroutine(answerItem.GetComponent<RectTransform>()));

//            //input �ʵ� Off
//            chatInput.interactable = false;
//            sendButton.interactable = false;

//            cursorCoroutine = StartCoroutine(Cursor(answerItem));

//            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
//            {
//                webRequest.SetRequestHeader("Content-Type", "application/json");
//                webRequest.SetRequestHeader("X-API-KEY", X_APIKEY);

//                yield return webRequest.SendWebRequest();

//                StopCoroutine(cursorCoroutine);

//                if (webRequest.result != UnityWebRequest.Result.Success)
//                {
//                    answerItem.SetItem("TaxGPT ������ ������ϴ�.<br>�ٽ� �õ��ϼ���");
//                    yield return null;
//                }
//                else
//                {
//                    string responseJson = webRequest.downloadHandler.text;
//                    string[] responseDataArray = responseJson.Split("data:");

//                    foreach (string responseData in responseDataArray)
//                    {
//                        JSONObject jsonObject = new JSONObject(responseData);
//                        if (jsonObject.HasField("result") && jsonObject.GetField("result").type == JSONObject.Type.STRING)
//                        {
//                            string resultMsg = jsonObject.GetField("result").str;
//                            if (resultMsg != null)
//                            {
//                                string decodedString = System.Text.RegularExpressions.Regex.Unescape(resultMsg);
//                                if (!responseData.Equals(responseDataArray[responseDataArray.Length - 1]))
//                                    decodedString = AddRandomCharacter(decodedString);
//                                PrintAnswer(answerItem, decodedString);

//                                //�亯�� ���� ������ ��ũ���� ��� �Ʒ��� ���� �ʿ� ���ٰ� �Ǵ�
//                                //StartCoroutine(RefreshLayoutCoroutine(answerItem.GetComponent<RectTransform>()));
//                                yield return new WaitForSeconds(0.05f);
//                            }
//                        }
//                    }

//                    //�亯�� ��� ������ Refresh
//                    StartCoroutine(RefreshLayoutCoroutine(answerItem.GetComponent<RectTransform>()));
//                }

//                //input �ʵ� On
//                chatInput.interactable = true;
//                sendButton.interactable = true;
//            }
//        }

//        IEnumerator Cursor(ChatbotAnswerItem chatbotAnswerItem)
//        {
//            while (true)
//            {
//                chatbotAnswerItem.SetItem("|");
//                yield return new WaitForSeconds(0.5f);
//                chatbotAnswerItem.SetItem(" ");
//                yield return new WaitForSeconds(0.5f);
//            }
//        }

//        private string AddRandomCharacter(string isCursor)
//        {
//            char randomChar = (char)Random.Range(0, 1);

//            if (randomChar != 0)
//                isCursor += "|";
//            else isCursor += "";

//            return isCursor;
//        }

//        public void PrintAnswer(ChatbotAnswerItem answerItem, string answerInfo)
//        {
//            if (answerItem == null) return;
//            answerItem.SetItem(answerInfo);
//        }

//        public void PrintChat(string chat)
//        {
//            GameObject sendChatObj = Instantiate(sendChatItemPrefab, chatScrollViewTrans);
//            //SendChatItem sendChatItem = sendChatObj.GetComponent<SendChatItem>();

//            if (sendChatItem == null) return;
//            sendChatItem.SetItem(chat);
//            StartCoroutine(RefreshLayoutCoroutine(sendChatItem.GetComponent<RectTransform>()));
//        }

//        IEnumerator RefreshLayoutCoroutine(RectTransform targetRect)
//        {
//            yield return null;
//            LayoutRebuilder.ForceRebuildLayoutImmediate(targetRect);
//            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollFitter.GetComponent<RectTransform>());
//            StartCoroutine(ScrollDownCoroutine());
//        }

//        IEnumerator ScrollDownCoroutine()
//        {
//            yield return null;
//            chatScrollRect.normalizedPosition = new Vector2(0f, 0f);
//        }
//    }
//}

