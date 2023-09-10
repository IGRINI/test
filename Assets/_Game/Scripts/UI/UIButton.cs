using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public RectTransform RectTransform => _rectTransform;
        
        private Button _button;
        [SerializeField] private RectTransform _rectTransform;
        
        public UnityEvent OnClick => _button.onClick;
        
        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
        }
    }
}