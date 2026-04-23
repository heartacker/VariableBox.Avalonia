using Avalonia.Metadata;

// Single URI mapping for all types in the library.
[assembly: XmlnsDefinition("https://github.com/heartacker/VariableBox.Avalonia", "VariableBox")]
[assembly: XmlnsDefinition("https://github.com/heartacker/VariableBox.Avalonia", "VariableBox.Themes")]

// Backward compatibility or simpler local access
[assembly: XmlnsDefinition("VariableBox", "VariableBox")]
[assembly: XmlnsDefinition("VariableBox", "VariableBox.Themes")]
