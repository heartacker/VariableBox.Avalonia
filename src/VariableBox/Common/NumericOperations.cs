using System;
using System.Globalization;

namespace VariableBox.Common;

public interface INumericOperations<T> where T : struct, IComparable<T>
{
    T Zero { get; }
    T MaxValue { get; }
    T MinValue { get; }
    T DefaultStep { get; }
    bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out T result);
    string ToString(T? value, string? format, IFormatProvider? provider);
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Clamp(T value, T min, T max);
}

public static class NumericOperations
{
    public static readonly INumericOperations<byte> Byte = new ByteOperations();
    public static readonly INumericOperations<sbyte> SByte = new SByteOperations();
    public static readonly INumericOperations<short> Short = new ShortOperations();
    public static readonly INumericOperations<ushort> UShort = new UShortOperations();
    public static readonly INumericOperations<int> Int = new IntOperations();
    public static readonly INumericOperations<uint> UInt = new UIntOperations();
    public static readonly INumericOperations<long> Long = new LongOperations();
    public static readonly INumericOperations<ulong> ULong = new ULongOperations();
    public static readonly INumericOperations<float> Float = new FloatOperations();
    public static readonly INumericOperations<double> Double = new DoubleOperations();
    public static readonly INumericOperations<decimal> Decimal = new DecimalOperations();

    private static string TrimString(string? text, NumberStyles numberStyles)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        text = text.Trim();
        if (text.Contains("_")) text = text.Replace("_", "");

        // Handle Hex
        if ((numberStyles & NumberStyles.AllowHexSpecifier) != 0)
        {
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) return text.Substring(2);
            if (text.StartsWith("h'", StringComparison.OrdinalIgnoreCase)) return text.Substring(2);
            if (text.StartsWith("h", StringComparison.OrdinalIgnoreCase)) return text.Substring(1);
        }
        
        // Handle Binary (Custom Logic)
        if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase)) return text.Substring(2);
        if (text.StartsWith("b'", StringComparison.OrdinalIgnoreCase)) return text.Substring(2);
        if (text.StartsWith("b", StringComparison.OrdinalIgnoreCase) && !IsHex(text)) return text.Substring(1);
        
        return text;
    }

    private static bool IsHex(string s) => s.Any(c => (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));

    private static string FormatValue<T>(T? value, string? format, IFormatProvider? provider) where T : struct
    {
        if (value == null) return string.Empty;
        if (string.IsNullOrEmpty(format)) return value.Value.ToString() ?? string.Empty;

        // Special handling for Binary formatting (e.g., "b8" for 8-bit binary)
        if (format.StartsWith("b", StringComparison.OrdinalIgnoreCase) && int.TryParse(format.Substring(1), out int width))
        {
            string bin = Convert.ToString(Convert.ToInt64(value.Value), 2);
            return bin.PadLeft(width, '0');
        }
        if (format.Equals("b", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToString(Convert.ToInt64(value.Value), 2);
        }

        if (format.Contains("{0")) return string.Format(provider, format, value.Value);
        return string.Format(provider, "{0:" + format + "}", value.Value);
    }

    private static bool TryParseInternal<T>(string? s, NumberStyles style, IFormatProvider? provider, Func<string, int, T> converter, out T result) where T : struct
    {
        string trimmed = TrimString(s, style);
        try
        {
            // If it looks like binary or we forced it via custom style check
            if (s != null && (s.StartsWith("0b", StringComparison.OrdinalIgnoreCase) || s.StartsWith("b'", StringComparison.OrdinalIgnoreCase)))
            {
                result = converter(trimmed, 2);
                return true;
            }
            
            // Standard parse
            bool success = double.TryParse(trimmed, style, provider, out double d);
            result = (T)Convert.ChangeType(d, typeof(T));
            return success;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    private static T InternalClamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    private class ByteOperations : INumericOperations<byte>
    {
        public byte Zero => 0;
        public byte MaxValue => byte.MaxValue;
        public byte MinValue => byte.MinValue;
        public byte DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out byte result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToByte(t, 2); return true; } catch { } }
            return byte.TryParse(t, style, provider, out result);
        }
        public string ToString(byte? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public byte Add(byte a, byte b) => (byte)Math.Min((int)byte.MaxValue, (int)a + b);
        public byte Subtract(byte a, byte b) => (byte)Math.Max((int)byte.MinValue, (int)a - b);
        public byte Clamp(byte value, byte min, byte max) => InternalClamp(value, min, max);
    }

    private class SByteOperations : INumericOperations<sbyte>
    {
        public sbyte Zero => 0;
        public sbyte MaxValue => sbyte.MaxValue;
        public sbyte MinValue => sbyte.MinValue;
        public sbyte DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out sbyte result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToSByte(t, 2); return true; } catch { } }
            return sbyte.TryParse(t, style, provider, out result);
        }
        public string ToString(sbyte? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public sbyte Add(sbyte a, sbyte b) => (sbyte)InternalClamp(a + b, (int)sbyte.MinValue, (int)sbyte.MaxValue);
        public sbyte Subtract(sbyte a, sbyte b) => (sbyte)InternalClamp(a - b, (int)sbyte.MinValue, (int)sbyte.MaxValue);
        public sbyte Clamp(sbyte value, sbyte min, sbyte max) => InternalClamp(value, min, max);
    }

    private class ShortOperations : INumericOperations<short>
    {
        public short Zero => 0;
        public short MaxValue => short.MaxValue;
        public short MinValue => short.MinValue;
        public short DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out short result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToInt16(t, 2); return true; } catch { } }
            return short.TryParse(t, style, provider, out result);
        }
        public string ToString(short? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public short Add(short a, short b) => (short)InternalClamp(a + b, (int)short.MinValue, (int)short.MaxValue);
        public short Subtract(short a, short b) => (short)InternalClamp(a - b, (int)short.MinValue, (int)short.MaxValue);
        public short Clamp(short value, short min, short max) => InternalClamp(value, min, max);
    }

    private class UShortOperations : INumericOperations<ushort>
    {
        public ushort Zero => 0;
        public ushort MaxValue => ushort.MaxValue;
        public ushort MinValue => ushort.MinValue;
        public ushort DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ushort result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToUInt16(t, 2); return true; } catch { } }
            return ushort.TryParse(t, style, provider, out result);
        }
        public string ToString(ushort? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public ushort Add(ushort a, ushort b) => (ushort)Math.Min((int)ushort.MaxValue, (int)a + b);
        public ushort Subtract(ushort a, ushort b) => (ushort)Math.Max((int)ushort.MinValue, (int)a - b);
        public ushort Clamp(ushort value, ushort min, ushort max) => InternalClamp(value, min, max);
    }

    private class IntOperations : INumericOperations<int>
    {
        public int Zero => 0;
        public int MaxValue => int.MaxValue;
        public int MinValue => int.MinValue;
        public int DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out int result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToInt32(t, 2); return true; } catch { } }
            return int.TryParse(t, style, provider, out result);
        }
        public string ToString(int? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public int Add(int a, int b) { long res = (long)a + b; return (int)InternalClamp(res, (long)int.MinValue, (long)int.MaxValue); }
        public int Subtract(int a, int b) { long res = (long)a - b; return (int)InternalClamp(res, (long)int.MinValue, (long)int.MaxValue); }
        public int Clamp(int value, int min, int max) => InternalClamp(value, min, max);
    }

    private class UIntOperations : INumericOperations<uint>
    {
        public uint Zero => 0;
        public uint MaxValue => uint.MaxValue;
        public uint MinValue => uint.MinValue;
        public uint DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out uint result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToUInt32(t, 2); return true; } catch { } }
            return uint.TryParse(t, style, provider, out result);
        }
        public string ToString(uint? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public uint Add(uint a, uint b) { long res = (long)a + b; return (uint)InternalClamp(res, (long)uint.MinValue, (long)uint.MaxValue); }
        public uint Subtract(uint a, uint b) { long res = (long)a - b; return (uint)InternalClamp(res, (long)uint.MinValue, (long)uint.MaxValue); }
        public uint Clamp(uint value, uint min, uint max) => InternalClamp(value, min, max);
    }

    private class LongOperations : INumericOperations<long>
    {
        public long Zero => 0;
        public long MaxValue => long.MaxValue;
        public long MinValue => long.MinValue;
        public long DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out long result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToInt64(t, 2); return true; } catch { } }
            return long.TryParse(t, style, provider, out result);
        }
        public string ToString(long? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public long Add(long a, long b) { try { checked { return a + b; } } catch { return a > 0 ? long.MaxValue : long.MinValue; } }
        public long Subtract(long a, long b) { try { checked { return a - b; } } catch { return a > 0 ? long.MaxValue : long.MinValue; } }
        public long Clamp(long value, long min, long max) => InternalClamp(value, min, max);
    }

    private class ULongOperations : INumericOperations<ulong>
    {
        public ulong Zero => 0;
        public ulong MaxValue => ulong.MaxValue;
        public ulong MinValue => ulong.MinValue;
        public ulong DefaultStep => 1;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ulong result)
        {
            string t = TrimString(s, style);
            if (s != null && (s.Contains("0b") || s.Contains("b'"))) { try { result = Convert.ToUInt64(t, 2); return true; } catch { } }
            return ulong.TryParse(t, style, provider, out result);
        }
        public string ToString(ulong? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public ulong Add(ulong a, ulong b) { try { checked { return a + b; } } catch { return ulong.MaxValue; } }
        public ulong Subtract(ulong a, ulong b) { try { checked { return a - b; } } catch { return ulong.MinValue; } }
        public ulong Clamp(ulong value, ulong min, ulong max) => InternalClamp(value, min, max);
    }

    private class FloatOperations : INumericOperations<float>
    {
        public float Zero => 0;
        public float MaxValue => float.MaxValue;
        public float MinValue => float.MinValue;
        public float DefaultStep => 1.0f;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out float result) => float.TryParse(TrimString(s, style), style, provider, out result);
        public string ToString(float? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public float Add(float a, float b) => a + b;
        public float Subtract(float a, float b) => a - b;
        public float Clamp(float value, float min, float max) => InternalClamp(value, min, max);
    }

    private class DoubleOperations : INumericOperations<double>
    {
        public double Zero => 0;
        public double MaxValue => double.MaxValue;
        public double MinValue => double.MinValue;
        public double DefaultStep => 1.0;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out double result) => double.TryParse(TrimString(s, style), style, provider, out result);
        public string ToString(double? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
        public double Clamp(double value, double min, double max) => InternalClamp(value, min, max);
    }

    private class DecimalOperations : INumericOperations<decimal>
    {
        public decimal Zero => 0;
        public decimal MaxValue => decimal.MaxValue;
        public decimal MinValue => decimal.MinValue;
        public decimal DefaultStep => 1.0m;
        public bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out decimal result) => decimal.TryParse(TrimString(s, style), style, provider, out result);
        public string ToString(decimal? value, string? format, IFormatProvider? provider) => FormatValue(value, format, provider);
        public decimal Add(decimal a, decimal b) { try { return a + b; } catch { return a > 0 ? decimal.MaxValue : decimal.MinValue; } }
        public decimal Subtract(decimal a, decimal b) { try { return a - b; } catch { return a > 0 ? decimal.MaxValue : decimal.MinValue; } }
        public decimal Clamp(decimal value, decimal min, decimal max) => InternalClamp(value, min, max);
    }
}
