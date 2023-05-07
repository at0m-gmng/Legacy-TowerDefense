namespace Loading
{
    using System;
    using System.Threading.Tasks;
    using AppInfo;

    public class ConfigOperation : ILoadingOperation
    {
        public string Description => "Config loading...";

        public ConfigOperation(AppInfoContainer appInfoContainer)
        {
            
        }

        public async Task Load(Action<float> onProgress)
        {
            var loadTime = UnityEngine.Random.Range(1.5f, 2.5f);
            const int steps = 4;

            for (int i = 0; i <= steps; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(loadTime / steps));
                onProgress?.Invoke(i/loadTime);
            }
            onProgress?.Invoke(1f);
        }
    }
}