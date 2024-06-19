using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// 날짜 : 2021-01-30 PM 8:02:49
// 작성자 : Rito

namespace Rito.InputBindings
{
    public class BindingPairUI : MonoBehaviour
    {
        public int NumberOfKeys;
        public bool[] selected = { false };

        public TMPro.TMP_Text actionLabel;
        public TMPro.TMP_Text[] codeLabels;
        public Button[] codeButtons;
        public Image[] buttonImages;
        public GameObject buttonPrefab;
        public Transform _contentParent;

        public void Select(int index)
        {
            selected[index] = true;
            buttonImages[index].color = Color.green;
        }

        public void Deselect(int index)
        {
            selected[index] = false;
            buttonImages[index].color = Color.white;
        }

        public void InitLabels(string actionText, string codeText, int index)
        {
            actionLabel.text = actionText;
            codeLabels[index].text = codeText;
        }

        public void SetCodeLabel(string text, int index)
        {
            codeLabels[index].text = text;
        }

        public void SetBindingPairUI(int index)
        {
            NumberOfKeys = index;
            selected = new bool[NumberOfKeys];
            codeLabels = new TMPro.TMP_Text[NumberOfKeys];
            codeButtons = new Button[NumberOfKeys];
            buttonImages = new Image[NumberOfKeys];

            CreateUIElements();
        }

        void CreateUIElements()
        {
            for (int i = 0; i < NumberOfKeys; i++)
            {
                var button = Instantiate(buttonPrefab, _contentParent);
                selected[i] = false;
                codeLabels[i] = button.GetComponentInChildren<TMPro.TMP_Text>();
                codeButtons[i] = button.GetComponent<Button>();
                buttonImages[i] = button.GetComponent<Image>();
            }
        }

        public void AddButtonListener(UnityAction method, int index)
        {
            codeButtons[index].onClick.AddListener(method);
        }
    }
}