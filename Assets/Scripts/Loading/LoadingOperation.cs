namespace Loading
{
    using Common;
    using Loading.Login;
    using UnityEngine;
    using System;
    using System.Threading.Tasks;
    using AppInfo;
    using UnityEngine.SceneManagement;

    public class LoadingOperation : ILoadingOperation
    {
        public string Description => "Login to server...";

        private readonly AppInfoContainer _appInfoContainer;
        private Action<float> _onProgress = delegate {  };

        public LoadingOperation(AppInfoContainer appInfoContainer) 
            => _appInfoContainer = appInfoContainer;

        public async Task Load(Action<float> onProgress)
        {
            _onProgress = onProgress;
            _onProgress.Invoke(0.3f);

            _appInfoContainer.UserInfo = await GetUserInfo(DeviceInfoProvider.GetDeviceId());

            _onProgress.Invoke(1f);
        }

        private async Task<UserInfoContainer> GetUserInfo(string deviceID)
        {
            UserInfoContainer result = null;

            #region FakeLogin

            if (PlayerPrefs.HasKey(deviceID))
            {
                result = JsonUtility.FromJson<UserInfoContainer>(PlayerPrefs.GetString(deviceID));
            }

            await Task.Delay(TimeSpan.FromSeconds(0.15f));
            _onProgress.Invoke(0.6f);
            
            #endregion

            if (result == null)
            {
                result = await ShowLoginWindow();
            }
            
            PlayerPrefs.SetString(deviceID, JsonUtility.ToJson(result));

            return result;
        }

        private async Task<UserInfoContainer> ShowLoginWindow()
        {
            var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.LOGIN, LoadSceneMode.Additive);

            while (loadOp.isDone == false)
            {
                await Task.Delay(1);
            }

            var loginScene = SceneManager.GetSceneByName(Constants.Scenes.LOGIN);
            var rootObjects = loginScene.GetRootGameObjects();

            LoginWindow loginWindow = null;

            foreach (var go in rootObjects)
            {
                if (go.TryGetComponent(out loginWindow))
                {
                    break;
                }
            }

            var result = await loginWindow.ProcessLogin();
            var unloadOp = SceneManager.UnloadSceneAsync(Constants.Scenes.LOGIN);

            while (unloadOp.isDone == false)
            {
                await Task.Delay(1);
            }

            return result;
        }
    }
}