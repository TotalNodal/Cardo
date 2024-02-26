using System.Security.AccessControl;
using static Cardo.Web.Utility.SD;

namespace Cardo.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AcessToken { get; set; }
        
    }
}
