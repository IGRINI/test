using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Utils
{
    public class SelectButton : UIButton
    {
        [SerializeField] private Settings _selectButtonSettings;

        protected override void Awake()
        {
            base.Awake();
            OnClick.AddListener(MakeSelected);
        }

        public async void MakeSelected()
        {
            await UniTask.NextFrame();
            _selectButtonSettings.ButtonText.DOColor(_selectButtonSettings.SelectedColor, _selectButtonSettings.Duration);
        }

        public void MakeDeselected()
        {
            _selectButtonSettings.ButtonText.DOColor(_selectButtonSettings.DeselectedColor, _selectButtonSettings.Duration);
        }

        [Serializable]
        private class Settings
        {
            public TextMeshProUGUI ButtonText;
            public Color SelectedColor;
            public Color DeselectedColor; 
            public float Duration; 
        }
    }
}