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

//        //API Settings 수정
//        private const string X_APIKEY = "mk-9c815385-53e4-435e-8aef-60ac634846aa";

//        //금일 토큰 값을 확인 후 사용 가능 시 True
//        private bool isOn = true;

//        private TouchScreenKeyboard keyboard = null;

//        //모바일 키보드 유무
//        bool isKeyboard = false;

//        protected void OnEnable()
//        {
//            chatInput.text = "";
//            ClearChatItems();

//            GameObject answerChatObj = Instantiate(answerChatItemPrefab, chatScrollViewTrans);
//            ChatbotAnswerItem answerItem = answerChatObj.GetComponent<ChatbotAnswerItem>();

//            if (isOn)
//                answerItem.SetItem("안녕하세요. Tax GPT입니다. 세무와 관련된 질문을 해보세요.");
//            else
//                answerItem.SetItem("이번 달 근무 시간은 다 채웠어요. 다음 달에 방문해주세요.");

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

//            //키보드 창에서 확인 버튼 눌렀을 때 발생하는 이벤트
//            chatInput.onSubmit.AddListener(delegate { SendChat(); });

//            //키보드창이 사라졌을 떄 발생하는 이벤트
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
//            //iOS : inputField 창 Hide Mobile Input 활성화 필요
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

//        #region 추가한 것
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

//            //API Settings 수정
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

//            //input 필드 Off
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
//                    answerItem.SetItem("TaxGPT 연결이 끊겼습니다.<br>다시 시도하세요");
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

//                                //답변을 받을 때에는 스크롤을 계속 아래로 잡을 필요 없다고 판단
//                                //StartCoroutine(RefreshLayoutCoroutine(answerItem.GetComponent<RectTransform>()));
//                                yield return new WaitForSeconds(0.05f);
//                            }
//                        }
//                    }

//                    //답변이 모두 끝나면 Refresh
//                    StartCoroutine(RefreshLayoutCoroutine(answerItem.GetComponent<RectTransform>()));
//                }

//                //input 필드 On
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

