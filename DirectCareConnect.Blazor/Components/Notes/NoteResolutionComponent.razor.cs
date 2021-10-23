using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Notes
{
    public class NoteResolutionComponentBase: NotesComponentsBase
    {
        [Parameter]
        public EventCallback<bool> OnGoBack { get; set; }

        [Parameter]
        public EventCallback<bool> OnSave { get; set; }

        [Parameter]
        public EventCallback<bool> OnSubmit { get; set; }

        public async void GoBack()
        {
            await OnGoBack.InvokeAsync(true);
        }

        public async void Save()
        {
            await OnSave.InvokeAsync(true);
            this.StateHasChanged();
        }

        public async void Submit()
        {
            await OnSubmit.InvokeAsync(true);
            this.StateHasChanged();
        }
    }
}
