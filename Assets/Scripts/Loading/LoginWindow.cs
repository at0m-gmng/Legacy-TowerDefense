using System;

namespace Loading.Login
{
    using System.Threading.Tasks;
    using AppInfo;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LoginWindow : MonoBehaviour
    {
        private const int NAME_MIN_LENGTH = 3;
        
        [SerializeField]
        private Text _nameField = default;
        [SerializeField]
        private Button _facebookLogin = default;
        [SerializeField]
        private Button _simpleLogin = default;

        private TaskCompletionSource<UserInfoContainer> _loginCompletionSource;

        private void Awake()
        {
            _simpleLogin.onClick.AddListener(OnSimpleLoginClicked);
            _simpleLogin.onClick.AddListener(OnFacebookLoginClicked);
            Debug.LogError(Time.timeScale);
        }

        public async Task<UserInfoContainer> ProcessLogin()
        {
            _loginCompletionSource = new TaskCompletionSource<UserInfoContainer>();
            return await _loginCompletionSource.Task;
        }

        private void OnSimpleLoginClicked()
        {
            if (_nameField.text.Length < NAME_MIN_LENGTH)
                return;
            
            _loginCompletionSource.SetResult(new UserInfoContainer()
            {
                Name = _nameField.text
            });
        }

        private void OnFacebookLoginClicked()
        {
            //TODO
        }

        private void OnDestroy()
        {
            _simpleLogin.onClick.RemoveListener(OnSimpleLoginClicked);
            _simpleLogin.onClick.RemoveListener(OnFacebookLoginClicked);
        }
    }
}