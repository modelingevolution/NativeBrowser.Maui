using System.Text;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

namespace NativeBrowser.Maui;

public partial class NativeWebView : IWebView
{
    private WebView2 _browser;
    public WebView2 Browser
    {
        get => _browser;
        set
        {
            _browser = value;
            AckInitialized();
        }
    }
    private readonly static JsonSerializerOptions _serializationOptions = new JsonSerializerOptions
    {
        MaxDepth = 32,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters =
        {

        }
    };
   

    public async Task InvokeVoidAsync(string identifier, params object[] args)
    {
        var script = PrepareScript(identifier, args);
        await _browser.ExecuteScriptAsync(script);
    }

    public async Task<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        var script = PrepareScript(identifier, args);
        var result = await _browser.ExecuteScriptAsync(script);
        return JsonSerializer.Deserialize<T>(result, _serializationOptions);
    }

    private static string PrepareScript(string identifier, object[] args)
    {
        // Let's keep it simple. We assume that the identifier is a window function and args are json serializable. 
        StringBuilder scriptBuilder = new StringBuilder();
        List<string> argBuilder = new();
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            string argVariableName = $"arg_{i}";
            scriptBuilder.AppendLine(
                $"const {argVariableName} = {JsonSerializer.Serialize(arg, _serializationOptions)};");
        }

        scriptBuilder.Append($"window.{identifier}(");
        scriptBuilder.Append(string.Join(',', argBuilder));
        scriptBuilder.AppendLine(");");
        return scriptBuilder.ToString();
    }
}