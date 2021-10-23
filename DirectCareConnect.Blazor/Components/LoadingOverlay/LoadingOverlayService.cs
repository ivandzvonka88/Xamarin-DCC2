using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.LoadingOverlay
{
    public class LoadingOverlayService : ILoadingOverlayService
    {
        public event Action<bool, string> ShowChanged;

        public void Hide()
        {
            ShowChanged?.Invoke(false, String.Empty);
        }

        public void Show(string message)
        {
            ShowChanged?.Invoke(true, message);
        }
    }
}
