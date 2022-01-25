using System.Threading.Tasks;
using Home.Model.Component;

namespace Home.Hotfix.Service;

public static class BagService
{
    public static Task Load(this BagComponent self)
    {
        return Task.CompletedTask;
    }
}