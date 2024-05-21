using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace NativeBrowser.Blazor
{
    public static class JsExtensions
    {
        
        public static async Task BrowserSendMessage(this IJSRuntime js, string identifier, params object[] args)
        {
            var s= args
                .Select(x => JsonSerializer.Serialize(x))
                .Prepend(JsonSerializer.Serialize(identifier));
            var json = $"[{string.Join(',', s)}]";
            await js.InvokeVoidAsync("window.NativeBridge.sendMessage", json);
        }
    }

}
