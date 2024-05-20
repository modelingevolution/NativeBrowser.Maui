using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using AWebView = Android.Webkit.WebView;
using Android.Webkit;
using Android.Content;
using Android.Runtime;
using Java.Net;
using System.Runtime.Versioning;
using Java.Interop;

namespace NativeBrowser.Maui
{
    public class CustomWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(AWebView view, IWebResourceRequest request)
        {
            view.LoadUrl(request.Url.ToString());
            return true;
        }
    }
    // All the code in this file is only included on Android.
    public partial class NativeWebViewHandler : ViewHandler<NativeWebView, AWebView>, INativeViewHandler
    {
        public static IPropertyMapper<NativeWebView, NativeWebViewHandler> Mapper = new PropertyMapper<NativeWebView, NativeWebViewHandler>(ViewHandler.ViewMapper)
        {
            // Map properties here
            ["Source"] = OnMapSourceProperty,
            ["Script"] = OnMapScriptProperty
        };

        public NativeWebViewHandler() : base(Mapper)
        {
            
        }
        private static void OnMapScriptProperty(NativeWebViewHandler arg1, NativeWebView arg2)
        {
            var vm = arg1.PlatformView;
            if (arg2.Script == null) return;

        }
        private static void OnMapSourceProperty(NativeWebViewHandler arg1, NativeWebView arg2)
        {
            var vm = arg1.PlatformView;
            if (arg2.Source != null)
            {
                vm?.LoadUrl(arg2.Source);
                
            }
        }
        private ILogger? _logger;
        internal ILogger Logger => _logger ??= Services!.GetService<ILogger<NativeWebViewHandler>>() ?? NullLogger<NativeWebViewHandler>.Instance;

        protected override AWebView CreatePlatformView()
        {
            
            //var _webChromeClient = new BlazorWebChromeClient();
            //this.PlatformView.SetWebChromeClient(_webChromeClient);

            var webView = new AWebView(this.Context);
            webView.SetWebViewClient(new CustomWebViewClient());
            webView.Settings.JavaScriptEnabled = true;
            webView.AddJavascriptInterface(new NativeBridge(this), "NativeBridge");

            return webView;
        }
        
      

        protected override void DisconnectHandler(AWebView platformView)
        {
            platformView.StopLoading();
            
        }

    }
    public class NativeBridge : Java.Lang.Object
    {
        NativeWebViewHandler _webViewHandler;

        public NativeBridge(NativeWebViewHandler webViewHandler)
        {
            _webViewHandler = webViewHandler;
        }

        [Export("sendMessage")]
        [JavascriptInterface]
        public void SendMessage(string message)
        {
            Console.WriteLine("Android: "+ message);
        }
    }

}
