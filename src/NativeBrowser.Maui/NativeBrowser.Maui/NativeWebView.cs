using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NativeBrowser.Maui
{
    
    public partial class NativeWebView : View
    {
        private readonly ConcurrentDictionary<string, Subscription> _subscriptions = new();
        internal bool IsInitialized { get; private set; }

        internal void AckInitialized() => IsInitialized = true;
        public static BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(string), typeof(NativeWebView));
        public static BindableProperty ScriptProperty = BindableProperty.Create(nameof(Script), typeof(string), typeof(NativeWebView));
        public static BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(NativeWebView));
        public string? Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }
        public string? Script { get => (string)GetValue(ScriptProperty); set => SetValue(ScriptProperty, value); }

        internal void RaiseMessage(string json)
        {
            var message = JsonSerializer.Deserialize<JsonElement>(json);
            if (message.ValueKind == JsonValueKind.Array)
            {
                var array = message.EnumerateArray().ToArray();
                switch (array.Length)
                {
                    case 0:
                        return;
                    case 1:
                    {
                        string identifier = array[0].GetString();
                        if (!_subscriptions.TryGetValue(identifier, out var subscription)) return;
                        subscription.Raise();
                        return;
                    }
                    case 2:
                    {
                        string identifier = array[0].GetString();
                        if (!_subscriptions.TryGetValue(identifier, out var subscription)) return;
                        subscription.Raise(array[1].GetRawText());
                        return;
                    }
                    default:
                        throw new InvalidOperationException("Unexpected number of arguments.");
                }
            } 

            if (message.ValueKind == JsonValueKind.String)
            {
                string identifier = message.GetString();
                if (!_subscriptions.TryGetValue(identifier, out var subscription)) return;
                subscription.Raise();
            }
        }
        public Subscription<object> On(string identifier)
        {
            return (Subscription<object>)_subscriptions.GetOrAdd(identifier, x => new Subscription<object>(identifier));
        }
        public Subscription<T> On<T>(string identifier)
        {
            return (Subscription<T>)_subscriptions.GetOrAdd(identifier, x => new Subscription<T>(identifier));
        }
    }

    public abstract class Subscription(string identifier)
    {
        public string Identifier => identifier;
        internal abstract void Raise(string json);
        internal abstract void Raise();
    }
    public class Subscription<T>(string identifier) : Subscription(identifier)
    {
        public event EventHandler<ReceivedMessageEventArgs<T>> Event;
        internal override void Raise(string json)
        {
            var arg = JsonSerializer.Deserialize<T>(json);
            Event?.Invoke(this, new ReceivedMessageEventArgs<T> { Identifier = Identifier, Data = arg });
        }

        internal override void Raise() => Event?.Invoke(this, new ReceivedMessageEventArgs<T> { Identifier = Identifier, Data = default(T) });
    }
    public class ReceivedMessageEventArgs<T> : EventArgs
    {
        public required string Identifier { get; init; }
        public required T? Data { get; init; }
    }
    public partial class NativeWebViewHandler
    {

    }

   
}
