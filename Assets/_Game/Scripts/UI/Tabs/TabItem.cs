using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TabItem : MonoBehaviour
{
   public TabsView.TabInfo TabInfo => _tabInfo;
   public Toggle Toggle => _tabToggle;
   
   [SerializeField] private TMP_Text _tabNameText;
   [SerializeField] private Toggle _tabToggle;

   private Color _selectedColor;
   private Color _unselectedColor;
   private Action _onTabSelect;
   private TabsView.TabInfo _tabInfo;
   
   private void Awake()
   {
      _tabToggle.OnValueChangedAsObservable().Subscribe(value =>
      {
         _tabNameText.color = value ? _selectedColor : _unselectedColor;
         
         if(value)
            _onTabSelect?.Invoke();
      });
   }

   public void Initialize(TabsView.TabInfo tabInfo, ToggleGroup toggleGroup, Color selectedColorText = default, Color unselectedColorText = default)
   {
      _tabInfo = tabInfo;
      _selectedColor = selectedColorText == default ? Color.white : selectedColorText;
      _unselectedColor = unselectedColorText == default ? Color.white : unselectedColorText;
      _tabNameText.SetText(tabInfo.TabName);
      _tabToggle.group = toggleGroup;
      _onTabSelect = tabInfo.OnTabSelect;
   }
}
