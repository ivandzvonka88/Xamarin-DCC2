using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Notes
{
    public class NotesTitleComponentBase : NotesComponentsBase
    {
        public string FormattedDate {
            get
            {
                if(this.EndSessionModel!=null && this.EndSessionModel.NoteDate.HasValue)
                {
                    return this.EndSessionModel.NoteDate.Value.ToShortDateString();
                }
                return String.Empty;
            }
        }

    }
}
