using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class ModelUIBase
    {
        private Dictionary<string, bool> loading;

        public ModelUIBase()
        {
            this.loading = new Dictionary<string, bool>();
        }
        public void SetLoading(string identifier, bool loading)
        {
            if (!this.loading.ContainsKey(identifier))
            {
                this.loading.Add(identifier, loading);
            }
            else
            {
                this.loading[identifier] = loading;
            }
        }

        public bool IsLoading(string property)
        {
            if (!this.loading.ContainsKey(property))
            {
                return false;
            }
            else
            {
                return this.loading[property];
            }
        }
    }
}
