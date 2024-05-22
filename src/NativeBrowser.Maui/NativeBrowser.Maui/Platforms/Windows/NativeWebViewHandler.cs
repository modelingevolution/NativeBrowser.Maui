using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;

namespace NativeBrowser.Maui
{
    
    public partial class NativeWebViewHandler : ViewHandler<NativeWebView, WebView2>, INativeViewHandler
    {
        const string _communicationBus = "window.NativeBridge = { sendMessage : function (msg) { window.chrome.webview.postMessage(msg); } };";
        public static IPropertyMapper<NativeWebView, NativeWebViewHandler> Mapper = new PropertyMapper<NativeWebView, NativeWebViewHandler>(ViewHandler.ViewMapper)
        {
            // Map properties here
            ["Source"] = OnMapSourceProperty,
            ["Script"] = OnMapScriptProperty
        };

        private static void OnMapScriptProperty(NativeWebViewHandler arg1, NativeWebView arg2)
        {
            var vm = arg1.PlatformView;
            if (arg2.Script == null) return;
            if (vm.CoreWebView2 == null) return;

            
        }

        protected override void ConnectHandler(WebView2 platformView)
        {
            this.VirtualView.Browser = platformView;
        }

        protected override void DisconnectHandler(WebView2 platformView)
        {
            this.VirtualView.Browser = null;
        }

        private static void OnMapSourceProperty(NativeWebViewHandler arg1, NativeWebView arg2)
        {
            var vm = arg1.PlatformView;
            if(arg2.Source != null)
                vm.Source = new Uri(arg2.Source);
        }


        
        public NativeWebViewHandler() : base(Mapper) {
            
        }
       
        
        private bool _isInitialized;
        private string _script;
        private void OnMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string msg = args.TryGetWebMessageAsString();
            this.VirtualView.RaiseMessage(msg);
        }
        private void OnCoreInitialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (_isInitialized) return;
            
            // It adds this for later.
            sender.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(_communicationBus);
            //sender.CoreWebView2.ExecuteScriptAsync(_communicationBus);
            _isInitialized = true;
        }
        static partial void OnRegister(IMauiHandlersCollection h)
        {
            h.AddHandler<NativeWebView, NativeWebViewHandler>();
        }
        protected override WebView2 CreatePlatformView()
        {
            
            var platformView = new WebView2()
            {
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
                Tag = this,
            };
            
            platformView.CoreWebView2Initialized += OnCoreInitialized;
            platformView.WebMessageReceived += OnMessageReceived;
            
            platformView.Tag = this;
            return platformView;

        }
       
    }
   
}