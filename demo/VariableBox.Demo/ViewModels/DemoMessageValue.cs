using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VariableBox.Demo.ViewModels;

// Create a message
public class UIntValueChangedMessage : ValueChangedMessage<UInt32>
{
    public UIntValueChangedMessage(UInt32 v) : base(v)
    {
    }
}
