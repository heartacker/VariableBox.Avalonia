using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
namespace VariableBox.Common;


public static class AvaloniaPropertyExtension
{
    public static void SetValue<T>(this AvaloniaProperty<T> property, T value, params AvaloniaObject?[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i]?.SetValue(property, value);
        }
    }

    public static void AffectsPseudoClass<TControl>(this AvaloniaProperty<bool> property, string pseudoClass, RoutedEvent<RoutedEventArgs>? routedEvent = null) where TControl : Control
    {
        string pseudoClass2 = pseudoClass;
        RoutedEvent<RoutedEventArgs>? routedEvent2 = routedEvent;
        property.Changed.AddClassHandler(delegate (TControl control, AvaloniaPropertyChangedEventArgs<bool> args)
        {
            OnPropertyChanged(control, args, pseudoClass2, routedEvent2);
        });
    }

    private static void OnPropertyChanged<TControl, TArgs>(TControl control, AvaloniaPropertyChangedEventArgs<bool> args, string pseudoClass, RoutedEvent<TArgs>? routedEvent) where TControl : Control where TArgs : RoutedEventArgs, new()
    {
        if (args.Sender == control)
        {
            PseudolassesExtensions.Set(control.Classes, pseudoClass, args.NewValue.Value);
            if (routedEvent != null)
            {
                control.RaiseEvent(new TArgs
                {
                    RoutedEvent = routedEvent
                });
            }
        }
    }

    public static void AffectsPseudoClass<TControl, TArgs>(this AvaloniaProperty<bool> property, string pseudoClass, RoutedEvent<TArgs>? routedEvent = null) where TControl : Control where TArgs : RoutedEventArgs, new()
    {
        string pseudoClass2 = pseudoClass;
        RoutedEvent<TArgs>? routedEvent2 = routedEvent;
        property.Changed.AddClassHandler(delegate (TControl control, AvaloniaPropertyChangedEventArgs<bool> args)
        {
            OnPropertyChanged(control, args, pseudoClass2, routedEvent2);
        });
    }
}


public static class RoutedEventExtension
{
    public static void AddHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler);
        }
    }

    public static void AddHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler,
    RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler, strategies, handledEventsToo);
        }
    }

    public static void RemoveHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.RemoveHandler(routedEvent, handler);
        }
    }
#if false

        public static IDisposable AddDisposableHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
        {
            List<IDisposable> list = new List<IDisposable>(controls.Length);
            for (int i = 0; i < controls.Length; i++)
            {
                IDisposable disposable = controls[i]?.AddDisposableHandler(routedEvent, handler);
                if (disposable != null)
                {
                    list.Add(disposable);
                }
            }

            return new ReadonlyDisposableCollection(list);
        }

        public static IDisposable AddDisposableHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params Interactive?[] controls) where TArgs : RoutedEventArgs
        {
            List<IDisposable> list = new List<IDisposable>(controls.Length);
            for (int i = 0; i < controls.Length; i++)
            {
                IDisposable disposable = controls[i]?.AddDisposableHandler(routedEvent, handler, strategies, handledEventsToo);
                if (disposable != null)
                {
                    list.Add(disposable);
                }
            }

            return new ReadonlyDisposableCollection(list);
        }
#endif
}