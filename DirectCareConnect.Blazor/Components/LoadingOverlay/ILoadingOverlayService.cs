using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.LoadingOverlay
{
    public interface ILoadingOverlayService
    {
        event Action<bool, string> ShowChanged;
        void Hide();
        void Show(string message);
    }
}
