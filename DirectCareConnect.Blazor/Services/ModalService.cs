using DirectCareConnect.Blazor.Components;
using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor
{
    public class ModalService: IModalService
    {
        public event Action<string, RenderFragment> OnShow;
        public event Action<object> OnClose;
        public string Logd = String.Empty;

        [Inject] IXamarinBridge XamarinBridge { get; set; }

        public ModalService()
        {
            
        }

    
        public void Show(string title, Type contentType, object model)
        {
            if (contentType.BaseType != typeof(ComponentBase) && contentType.BaseType.BaseType != typeof(ComponentBase) && contentType.BaseType != typeof(ModelBase) && contentType.BaseType.BaseType != typeof(ModelBase))
            {
                throw new ArgumentException($"{contentType.FullName} must be a Blazor Component");
            }

            
            var content = new RenderFragment(x => { 
                x.OpenComponent(1, contentType);
                if(model!=null)
                    x.AddAttribute(2, "Model", model);
                x.CloseComponent(); 
            });
            
            OnShow?.Invoke(title, content);
        }

        public void Show(string title, object model)
        {
            Type contentType = typeof(EnterFence);
            //XamarinBridge.DisplayAlert("Hey", "You", "There2");
            
            if (XamarinBridge == null)
                title += "is null";

            if (contentType.BaseType != typeof(ComponentBase))
            {
                throw new ArgumentException($"{contentType.FullName} must be a Blazor Component");
            }

            var content = new RenderFragment(x => {
                x.OpenComponent(1, contentType);
                if (model != null)
                    x.AddAttribute(2, "Title", model);
                x.CloseComponent();
            });

            OnShow?.Invoke(title, content);
        }

        public void Close(object model)
        {
            OnClose?.Invoke(model);
        }
    }
}
