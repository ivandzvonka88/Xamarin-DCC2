using DirectCareConnect.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using System.Text.Json;
using BlazorMobile.Common.Helpers;
using DirectCareConnect.Blazor.Helpers;

namespace DirectCareConnect.Blazor.Interop
{
    public class Modals
    {
        [Inject] IModalService modalService{get;set;}

        private static Modals modal;
        //[Inject] IXamarinBridge XamarinBridge { get; set; }

        private static IModalService ModalService
        {
            get
            {
                
                if (modal == null || modal.modalService == null)
                {
                
                    modal = new Modals();
                }
                
                if (modal.modalService != null)
                {
                
                }
                else
                {
                
                }

                return modal.modalService;
            }
        }
        private Modals()
        {

            this.modalService = ServicesHelper.GetModalService();
        }

        [JSInvokable]
        public static void ShowModal(string title, string name, object model)
        {
            
            var typ = Type.GetType(name);
            if (typ != null)
            {
            
                if (ModalService != null)
                {
            
                }
                else
                {
            
                }
                ModalService.Show(title, typ, model);
            }
            else
            {
                
            }
            return;
        }
    }
}
