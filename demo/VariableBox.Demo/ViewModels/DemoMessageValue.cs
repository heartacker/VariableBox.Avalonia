using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VariableBox.Demo.ViewModels;

// Create a message
/// <summary>
/// 只要注册了这个消息的接收者，就会收到消息。
/// 参考：
/// <br/>
/// <see cref="CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage"/>
/// <br/>
/// <see cref="CommunityToolkit.Mvvm.Messaging.Messages.PropertyChangedMessage"/>
/// <br/>
/// <see cref="CommunityToolkit.Mvvm.Messaging.Messages.RequestMessage"/>
/// </summary>
public class UIntValueChangedMessage : ValueChangedMessage<UInt32>
{
    public UIntValueChangedMessage(UInt32 v) : base(v)
    {
    }
}
