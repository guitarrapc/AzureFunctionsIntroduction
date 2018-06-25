using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction.Notify
{
    public interface INotify
    {
        Task<HttpResponseMessage> SendAsync(string json);
    }
}
