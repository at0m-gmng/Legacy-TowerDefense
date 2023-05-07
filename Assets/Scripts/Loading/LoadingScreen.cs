namespace Loading
{
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }
        
        [SerializeField]
        private Canvas _canvas = default;
        [SerializeField]
        private Slider _progressFill = default;
        [SerializeField]
        private Text _loadingInfo = default;
        [SerializeField]
        private  float _barSpeed = default;

        private float _targetProgeress = default;
        private bool _isProgress = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            DontDestroyOnLoad(this);
        }

        public async void Load(Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = true;
            StartCoroutine(UpdateProgressBar());

            foreach (var operation in loadingOperations)
            {
                ResetFill();
                _loadingInfo.text = operation.Description;

                await operation.Load(OnProgress);
                await WaitForBarFill();
            }

            _canvas.enabled = false;
        }

        private void ResetFill()
        {
            _progressFill.value = 0;
            _targetProgeress = 0;
        }
    }
}