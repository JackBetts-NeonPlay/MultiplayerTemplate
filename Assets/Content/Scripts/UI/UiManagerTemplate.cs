using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public abstract class UiManagerTemplate<T> : SingletonBehaviour<T>
    {
        [SerializeField] private CanvasGroup backgroundGroup;
        
        public Menu CurrentMenu => _currentMenu;
        private Menu _currentMenu;
        
        private Dictionary<string, Menu> _menus = new Dictionary<string, Menu>(); 

        public override void Awake()
        {
            base.Awake();
            GetMenus();
        }

        private void GetMenus()
        {
            Menu[] menus = GetComponentsInChildren<Menu>();
            foreach (Menu menu in menus)
            {
                _menus.Add(menu.MenuId, menu);
            }
        }

        public void SetBackgroundActive(bool active)
        {
            float targetAlpha = active ? 1f : 0f;
            backgroundGroup.DOFade(targetAlpha, .5f); 
        }

        public void OpenMenu(string id)
        {
            if (_currentMenu)
            {
                _currentMenu.Close();
            }

            Menu newMenu = GetMenuById(id);
            if (newMenu)
            {
                newMenu.Open();
                _currentMenu = newMenu;
            }
        }

        public bool IsMenuOpen(string id)
        {
            return _currentMenu.MenuId == id; 
        }

        private Menu GetMenuById(string id)
        {
            if (!_menus.ContainsKey(id))
            {
                Debug.LogError($"{this} No Menu found with id {id}");
                return null; 
            }

            return _menus[id]; 
        }
    }
}
