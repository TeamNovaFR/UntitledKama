using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Untitled.ChatSystem
{
    public class Chat : MonoBehaviour
    {
        public static Chat instance;

        [Header("References")]
        [SerializeField]
        private Transform chatContent;
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private ScrollRect scrollRect;

        [Header("Prefabs")]
        [SerializeField]
        private ChatMessage messagePrefab;

        [Header("Variables")]
        [SerializeField]
        private int maxMessages = 30;
        [SerializeField]
        private float maxTimeChat = 30f;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private float currentTimeChat = 0f;

        private bool isChatOpened;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            scrollRect.normalizedPosition = Vector2.zero;

            if (currentTimeChat < Time.timeSinceLevelLoad && !isChatOpened)
            {
                group.alpha = Mathf.Lerp(group.alpha, 0f, Time.deltaTime * 2f);
            }
            else
            {
                group.alpha = Mathf.Lerp(group.alpha, 1f, Time.deltaTime * 2f);
            }
        }

        private void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                if (isChatOpened)
                {
                    if (inputField.isFocused)
                        SendTextIF();
                    else
                        OpenChat();
                }
                else
                {
                    OpenChat();
                }
            }

            if (Event.current.Equals(Event.KeyboardEvent("Escape")))
            {
                if (inputField.isFocused)
                {
                    CloseChat();
                }
            }
        }

        void SendTextIF()
        {
            if (inputField.text.Length > 0)
            {
                if (inputField.text.Length >= 2)
                {
                    Kama.player.CmdSendText(inputField.text);
                    inputField.text = "";

                    CloseChat();
                }
            }
            else
            {
                CloseChat();
            }
        }

        void OpenChat()
        {
            isChatOpened = true;

            inputField.gameObject.SetActive(true);
            inputField.Select();
            inputField.ActivateInputField();
        }

        void CloseChat()
        {
            isChatOpened = false;

            inputField.gameObject.SetActive(false);

            currentTimeChat = Time.timeSinceLevelLoad + maxTimeChat;
        }

        /// <summary>
        /// Send a message to the chat
        /// </summary>
        /// <param name="msg">Message</param>
        public void SendText(string msg)
        {
            if (messages.Count > maxMessages)
            {
                messages.RemoveAt(0);
            }

            ChatMessage chatMessage = Instantiate(messagePrefab, chatContent);

            chatMessage.SetText(msg);

            messages.Add(chatMessage);

            currentTimeChat = Time.timeSinceLevelLoad + maxTimeChat;
        }

        /// <summary>
        /// Clear chat content
        /// </summary>
        public void ClearChat()
        {
            messages.Clear();

            foreach (Transform child in chatContent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}