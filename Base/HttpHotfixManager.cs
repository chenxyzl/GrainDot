using System.Collections.Generic;
using System.Reflection;
using Message;

namespace Base;

public class HttpHotfixManager : Single<HttpHotfixManager>
{
    private Dictionary<string, IHttpHandler> handles = new();

    public void ReloadHanlder()
    {
        var handlesTemp = new Dictionary<string, IHttpHandler>();
        var types = HotfixManager.Instance.GetTypes<HttpHandlerAttribute>();
        foreach (var type in types)
        {
            var attr = A.NotNull(type.GetCustomAttribute<HttpHandlerAttribute>(), Code.Error,
                "HttpHandlerAttribute must exist");
            A.Ensure(!handlesTemp.ContainsKey(attr.Router), Code.Error, $"router{attr.Router} must not repeated");
            var handler = A.NotNull(Activator.CreateInstance(type) as IHttpHandler);
            handlesTemp[attr.Router] = handler;
        }

        (handles, handlesTemp) = (handlesTemp, handles);
        handlesTemp.Clear();
    }

    public IHttpHandler GetHandler(string router)
    {
        return A.NotNull(handles[router], Code.Error, $"handler:{router} not found");
    }
}