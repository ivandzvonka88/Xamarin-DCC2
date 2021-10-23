using DirectCareConnect.Common.Interfaces.Storage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages.Testing
{
    public class DatabaseMemoryBase: ComponentBase
    {
        [Inject] IDatabaseService DbService { get; set; }
        public List<object> Items { get; set; }
        async protected override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.Items= await DbService.GetAll<object>();
        }

        public string GetPrimaryKey(object item)
        {
            PropertyInfo[] itemProperties = item.GetType().GetProperties();

            foreach (PropertyInfo property in itemProperties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(SQLite.PrimaryKeyAttribute))
                    as SQLite.PrimaryKeyAttribute;

                if (attribute != null) // This property has a KeyAttribute
                {
                    // Do something, to read from the property:
                    return property.GetValue(item).ToString();
                }
            }

            return String.Empty;
        }
        
    }
}
