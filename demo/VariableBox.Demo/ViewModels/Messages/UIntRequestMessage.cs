using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VariableBox.Demo.ViewModels.Messages;

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
public class UIntRequestMessage : RequestMessage<UInt32>
{
    public double t { get; private set; }
    // 可以传递参数，但是返回值一定是 UInt32
    public UIntRequestMessage(double t)
    {
        this.t = t;
    }
}
