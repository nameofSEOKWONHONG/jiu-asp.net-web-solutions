using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaCrossPlatformApp;

public class Utils
{
    public static void SetTimeout(int delay, Action action)
    {
        Task.Delay(delay).ContinueWith((task) => action());
    }
}