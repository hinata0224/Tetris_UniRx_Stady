using UnityEngine;
using UniRx;
using System;

namespace Tetris_UI
{
    public class MenuUIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject Menu;

        private Subject<bool> openMenu= new();
        void Start()
        {
            Menu.SetActive(false);
        }

        public void DisplayMenu()
        {
            Menu.SetActive(true);
            openMenu.OnNext(true);
        }

        public void CloseMenu()
        {
            Menu.SetActive(false);
            openMenu.OnNext(false);
        }

        public IObservable<bool> GetOpenMenu()
        {
            return openMenu;
        }
    }
}