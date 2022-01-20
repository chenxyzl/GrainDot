using Message;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Base
{
    public class HttpHotfixManager : Single<HttpHotfixManager>
    {
        private Dictionary<string, IHttpHandler> handles = new Dictionary<string, IHttpHandler>();

        public void ReloadHanlder()
        {
            var handlesTemp = new Dictionary<string, IHttpHandler>();
            HashSet<Type> types = HotfixManager.Instance.GetTypes<HttpHandlerAttribute>();
            foreach (var type in types)
            {
                var attr = A.RequireNotNull(type.GetCustomAttribute<HttpHandlerAttribute>(), Code.Error, "HttpHandlerAttribute must exist");
                A.Ensure(handlesTemp[attr.Router] == null, Code.Error, $"router{attr.Router} must not repeated");
                var handler = Activator.CreateInstance(type) as IHttpHandler;
                handlesTemp[attr.Router] = handler;
            }
            (handles, handlesTemp) = (handlesTemp, handles);
            handlesTemp.Clear();
        }

        public IHttpHandler GetHandler(string router)
        {
            return A.RequireNotNull(handles[router], Code.Error, $"handler:{router} not found");
        }
    }
}
