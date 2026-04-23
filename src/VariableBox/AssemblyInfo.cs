using VariableBox;

[assembly: GenerateVariableBox(typeof(int), "VariableBoxInt")]
[assembly: GenerateVariableBox(typeof(uint), "VariableBoxUInt")]
[assembly: GenerateVariableBox(typeof(long), "VariableBoxLong")]
[assembly: GenerateVariableBox(typeof(ulong), "VariableBoxULong")]
[assembly: GenerateVariableBox(typeof(short), "VariableBoxShort")]
[assembly: GenerateVariableBox(typeof(ushort), "VariableBoxUShort")]
[assembly: GenerateVariableBox(typeof(byte), "VariableBoxByte")]
[assembly: GenerateVariableBox(typeof(sbyte), "VariableBoxSByte")]
[assembly: GenerateVariableBox(typeof(float), "VariableBoxFloat")]
[assembly: GenerateVariableBox(typeof(double), "VariableBoxDouble")]
[assembly: GenerateVariableBox(typeof(decimal), "VariableBoxDecimal")]
