using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.LifecycleEvents;

namespace NativeBrowser.Maui
{
    public partial class NativeWebViewHandler
    {
        internal static void Register(IMauiHandlersCollection collection)
        {
            OnRegister(collection);
        }

        static partial void OnRegister(IMauiHandlersCollection collection);

    }
    public static class NativeWebViewHandlerExtensions
    {
        public static MauiAppBuilder UseNativeBrowser(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(NativeWebViewHandler.Register);
            return builder;
        }
    }
}
