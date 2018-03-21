using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.NearestStore.Models.Responses
{
    public class ServiceResponse<T>
    {
        public T Result { get; set; }
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; }
        public bool IsSuccess { get; set; }
        public ServiceResponse()
        {
            this.Errors = new List<string>();
        }
            

    }
}