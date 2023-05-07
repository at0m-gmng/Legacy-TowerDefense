using System;
using System.Collections.Generic;
using Loading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
   using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _quickGameBtn = default;

        private void Start()
        {
            _quickGameBtn.onClick.AddListener(OnQuickGameBtnClicked);
        }

        private void OnQuickGameBtnClicked()
        {
            var loadOperations = new Queue<ILoadingOperation>();
            loadOperations.Enqueue(new GameLoadingOperation());
            LoadingScreen.Instance.Load(loadOperations);
        }

        private void OnDestroy()
        {
            _quickGameBtn.onClick.RemoveListener(OnQuickGameBtnClicked);
        }
    }
}