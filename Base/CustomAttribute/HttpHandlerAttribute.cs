using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HttpHandlerAttribute : BaseAttribute
    {
        public string Router { get; private set; }
        public readonly uint HttpId;
        public HttpHandlerAttribute(string router)
        {
            Router = router;
        }
    }
}
