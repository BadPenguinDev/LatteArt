using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MadPenguin.LatteArt
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleShortCut : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        
        private Toggle _toggle;

        private void Start()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode))
                _toggle.isOn = !_toggle.isOn;
        }
    }
}