using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Utils
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        private Button _button;

        public UnityEvent OnClick => _button.onClick;
        
        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
        }
    }
}