using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces
{
    public interface IModalService
    {
        event Action<string, RenderFragment> OnShow;
        event Action<object> OnClose;

        void Show(string title, Type contentType, object model);
        void Show(string title,object model);
        void Close(object model);
    }
}
