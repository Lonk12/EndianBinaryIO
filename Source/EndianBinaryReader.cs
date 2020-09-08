﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kermalis.EndianBinaryIO
{
    public class EndianBinaryReader : IDisposable
    {
        public Stream BaseStream { get; }
        public Endianness Endianness { get; set; }
        public EncodingType Encoding { get; set; }
        public BooleanSize BooleanSize { get; set; }

        private byte[] _buffer;
        private bool _isDisposed;

        public EndianBinaryReader(Stream baseStream, Endianness endianness = Endianness.LittleEndian, EncodingType encoding = EncodingType.ASCII, BooleanSize booleanSize = BooleanSize.U8)
        {
            if (baseStream is null)
            {
                throw new ArgumentNullException(nameof(baseStream));
            }
            if (!baseStream.CanRead)
            {
                throw new ArgumentException(nameof(baseStream));
            }
            BaseStream = baseStream;
            Endianness = endianness;
            Encoding = encoding;
            BooleanSize = booleanSize;
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                BaseStream.Dispose();
                _buffer = null;
                _isDisposed = true;
            }
        }

        // Returns true if count is 0
        private static bool ValidateArraySize<T>(int count, out T[] array)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"Invalid array length ({count})");
            }
            if (count == 0)
            {
                array = Array.Empty<T>();
                return true;
            }
            array = null;
            return false;
        }
        private void ReadBytesIntoBuffer(int primitiveCount, int primitiveSize)
        {
            int byteCount = primitiveCount * primitiveSize;
            if (_buffer == null || _buffer.Length < byteCount)
            {
                _buffer = new byte[byteCount];
            }
            if (BaseStream.Read(_buffer, 0, byteCount) != byteCount)
            {
                throw new EndOfStreamException();
            }
            Utils.FlipPrimitives(_buffer, Endianness, byteCount, primitiveSize);
        }

        public byte PeekByte()
        {
            long pos = BaseStream.Position;
            byte b = ReadByte();
            BaseStream.Position = pos;
            return b;
        }
        public byte PeekByte(long offset)
        {
            BaseStream.Position = offset;
            return PeekByte();
        }
        public byte[] PeekBytes(int count)
        {
            long pos = BaseStream.Position;
            byte[] b = ReadBytes(count);
            BaseStream.Position = pos;
            return b;
        }
        public byte[] PeekBytes(int count, long offset)
        {
            BaseStream.Position = offset;
            return PeekBytes(count);
        }
        public char PeekChar()
        {
            long pos = BaseStream.Position;
            char c = ReadChar();
            BaseStream.Position = pos;
            return c;
        }
        public char PeekChar(long offset)
        {
            BaseStream.Position = offset;
            return PeekChar();
        }
        public char PeekChar(EncodingType encodingType)
        {
            long pos = BaseStream.Position;
            char c = ReadChar(encodingType);
            BaseStream.Position = pos;
            return c;
        }
        public char PeekChar(EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return PeekChar(encodingType);
        }

        public bool ReadBoolean()
        {
            return ReadBoolean(BooleanSize);
        }
        public bool ReadBoolean(long offset)
        {
            BaseStream.Position = offset;
            return ReadBoolean(BooleanSize);
        }
        public bool ReadBoolean(BooleanSize booleanSize)
        {
            switch (booleanSize)
            {
                case BooleanSize.U8:
                {
                    ReadBytesIntoBuffer(1, 1);
                    return _buffer[0] != 0;
                }
                case BooleanSize.U16:
                {
                    ReadBytesIntoBuffer(1, 2);
                    return Utils.BytesToInt16(_buffer, 0) != 0;
                }
                case BooleanSize.U32:
                {
                    ReadBytesIntoBuffer(1, 4);
                    return Utils.BytesToInt32(_buffer, 0) != 0;
                }
                default: throw new ArgumentOutOfRangeException(nameof(booleanSize));
            }
        }
        public bool ReadBoolean(BooleanSize booleanSize, long offset)
        {
            BaseStream.Position = offset;
            return ReadBoolean(booleanSize);
        }
        public bool[] ReadBooleans(int count)
        {
            return ReadBooleans(count, BooleanSize);
        }
        public bool[] ReadBooleans(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadBooleans(count, BooleanSize);
        }
        public bool[] ReadBooleans(int count, BooleanSize size)
        {
            if (!ValidateArraySize(count, out bool[] array))
            {
                array = new bool[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadBoolean(size);
                }
            }
            return array;
        }
        public bool[] ReadBooleans(int count, BooleanSize size, long offset)
        {
            BaseStream.Position = offset;
            return ReadBooleans(count, size);
        }
        public byte ReadByte()
        {
            ReadBytesIntoBuffer(1, 1);
            return _buffer[0];
        }
        public byte ReadByte(long offset)
        {
            BaseStream.Position = offset;
            return ReadByte();
        }
        public byte[] ReadBytes(int count)
        {
            if (!ValidateArraySize(count, out byte[] array))
            {
                ReadBytesIntoBuffer(count, 1);
                array = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = _buffer[i];
                }
            }
            return array;
        }
        public byte[] ReadBytes(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadBytes(count);
        }
        public sbyte ReadSByte()
        {
            ReadBytesIntoBuffer(1, 1);
            return (sbyte)_buffer[0];
        }
        public sbyte ReadSByte(long offset)
        {
            BaseStream.Position = offset;
            return ReadSByte();
        }
        public sbyte[] ReadSBytes(int count)
        {
            if (!ValidateArraySize(count, out sbyte[] array))
            {
                ReadBytesIntoBuffer(count, 1);
                array = new sbyte[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = (sbyte)_buffer[i];
                }
            }
            return array;
        }
        public sbyte[] ReadSBytes(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadSBytes(count);
        }
        public char ReadChar()
        {
            return ReadChar(Encoding);
        }
        public char ReadChar(long offset)
        {
            BaseStream.Position = offset;
            return ReadChar();
        }
        public char ReadChar(EncodingType encodingType)
        {
            Encoding encoding = Utils.EncodingFromEnum(encodingType);
            int encodingSize = Utils.EncodingSize(encoding);
            ReadBytesIntoBuffer(1, encodingSize);
            return encoding.GetChars(_buffer, 0, encodingSize)[0];
        }
        public char ReadChar(EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadChar(encodingType);
        }
        public char[] ReadChars(int count)
        {
            return ReadChars(count, Encoding);
        }
        public char[] ReadChars(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadChars(count);
        }
        public char[] ReadChars(int count, EncodingType encodingType)
        {
            if (ValidateArraySize(count, out char[] array))
            {
                return array;
            }
            Encoding encoding = Utils.EncodingFromEnum(encodingType);
            int encodingSize = Utils.EncodingSize(encoding);
            ReadBytesIntoBuffer(count, encodingSize);
            return encoding.GetChars(_buffer, 0, encodingSize * count);
        }
        public char[] ReadChars(int count, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadChars(count, encodingType);
        }
        public string ReadStringNullTerminated()
        {
            return ReadStringNullTerminated(Encoding);
        }
        public string ReadStringNullTerminated(long offset)
        {
            BaseStream.Position = offset;
            return ReadStringNullTerminated();
        }
        public string ReadStringNullTerminated(EncodingType encodingType)
        {
            string text = string.Empty;
            while (true)
            {
                char c = ReadChar(encodingType);
                if (c == '\0')
                {
                    break;
                }
                text += c;
            }
            return text;
        }
        public string ReadStringNullTerminated(EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadStringNullTerminated(encodingType);
        }
        public string ReadString(int charCount)
        {
            return ReadString(charCount, Encoding);
        }
        public string ReadString(int charCount, long offset)
        {
            BaseStream.Position = offset;
            return ReadString(charCount);
        }
        public string ReadString(int charCount, EncodingType encodingType)
        {
            return new string(ReadChars(charCount, encodingType));
        }
        public string ReadString(int charCount, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadString(charCount, encodingType);
        }
        public string[] ReadStringsNullTerminated(int count)
        {
            return ReadStringsNullTerminated(count, Encoding);
        }
        public string[] ReadStringsNullTerminated(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadStringsNullTerminated(count);
        }
        public string[] ReadStringsNullTerminated(int count, EncodingType encodingType)
        {
            if (!ValidateArraySize(count, out string[] array))
            {
                array = new string[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadStringNullTerminated(encodingType);
                }
            }
            return array;
        }
        public string[] ReadStringsNullTerminated(int count, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadStringsNullTerminated(count, encodingType);
        }
        public string[] ReadStrings(int count, int charCount)
        {
            return ReadStrings(count, charCount, Encoding);
        }
        public string[] ReadStrings(int count, int charCount, long offset)
        {
            BaseStream.Position = offset;
            return ReadStrings(count, charCount);
        }
        public string[] ReadStrings(int count, int charCount, EncodingType encodingType)
        {
            if (!ValidateArraySize(count, out string[] array))
            {
                array = new string[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadString(charCount, encodingType);
                }
            }
            return array;
        }
        public string[] ReadStrings(int count, int charCount, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            return ReadStrings(count, charCount, encodingType);
        }
        public short ReadInt16()
        {
            ReadBytesIntoBuffer(1, 2);
            return Utils.BytesToInt16(_buffer, 0);
        }
        public short ReadInt16(long offset)
        {
            BaseStream.Position = offset;
            return ReadInt16();
        }
        public short[] ReadInt16s(int count)
        {
            if (!ValidateArraySize(count, out short[] array))
            {
                ReadBytesIntoBuffer(count, 2);
                array = new short[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToInt16(_buffer, 2 * i);
                }
            }
            return array;
        }
        public short[] ReadInt16s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadInt16s(count);
        }
        public ushort ReadUInt16()
        {
            ReadBytesIntoBuffer(1, 2);
            return (ushort)Utils.BytesToInt16(_buffer, 0);
        }
        public ushort ReadUInt16(long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt16();
        }
        public ushort[] ReadUInt16s(int count)
        {
            if (!ValidateArraySize(count, out ushort[] array))
            {
                ReadBytesIntoBuffer(count, 2);
                array = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = (ushort)Utils.BytesToInt16(_buffer, 2 * i);
                }
            }
            return array;
        }
        public ushort[] ReadUInt16s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt16s(count);
        }
        public int ReadInt32()
        {
            ReadBytesIntoBuffer(1, 4);
            return Utils.BytesToInt32(_buffer, 0);
        }
        public int ReadInt32(long offset)
        {
            BaseStream.Position = offset;
            return ReadInt32();
        }
        public int[] ReadInt32s(int count)
        {
            if (!ValidateArraySize(count, out int[] array))
            {
                ReadBytesIntoBuffer(count, 4);
                array = new int[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToInt32(_buffer, 4 * i);
                }
            }
            return array;
        }
        public int[] ReadInt32s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadInt32s(count);
        }
        public uint ReadUInt32()
        {
            ReadBytesIntoBuffer(1, 4);
            return (uint)Utils.BytesToInt32(_buffer, 0);
        }
        public uint ReadUInt32(long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt32();
        }
        public uint[] ReadUInt32s(int count)
        {
            if (!ValidateArraySize(count, out uint[] array))
            {
                ReadBytesIntoBuffer(count, 4);
                array = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = (uint)Utils.BytesToInt32(_buffer, 4 * i);
                }
            }
            return array;
        }
        public uint[] ReadUInt32s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt32s(count);
        }
        public long ReadInt64()
        {
            ReadBytesIntoBuffer(1, 8);
            return Utils.BytesToInt64(_buffer, 0);
        }
        public long ReadInt64(long offset)
        {
            BaseStream.Position = offset;
            return ReadInt64();
        }
        public long[] ReadInt64s(int count)
        {
            if (!ValidateArraySize(count, out long[] array))
            {
                ReadBytesIntoBuffer(count, 8);
                array = new long[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToInt64(_buffer, 8 * i);
                }
            }
            return array;
        }
        public long[] ReadInt64s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadInt64s(count);
        }
        public ulong ReadUInt64()
        {
            ReadBytesIntoBuffer(1, 8);
            return (ulong)Utils.BytesToInt64(_buffer, 0);
        }
        public ulong ReadUInt64(long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt64();
        }
        public ulong[] ReadUInt64s(int count)
        {
            if (!ValidateArraySize(count, out ulong[] array))
            {
                ReadBytesIntoBuffer(count, 8);
                array = new ulong[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = (ulong)Utils.BytesToInt64(_buffer, 8 * i);
                }
            }
            return array;
        }
        public ulong[] ReadUInt64s(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadUInt64s(count);
        }
        public float ReadSingle()
        {
            ReadBytesIntoBuffer(1, 4);
            return Utils.BytesToSingle(_buffer, 0);
        }
        public float ReadSingle(long offset)
        {
            BaseStream.Position = offset;
            return ReadSingle();
        }
        public float[] ReadSingles(int count)
        {
            if (!ValidateArraySize(count, out float[] array))
            {
                ReadBytesIntoBuffer(count, 4);
                array = new float[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToSingle(_buffer, 4 * i);
                }
            }
            return array;
        }
        public float[] ReadSingles(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadSingles(count);
        }
        public double ReadDouble()
        {
            ReadBytesIntoBuffer(1, 8);
            return Utils.BytesToDouble(_buffer, 0);
        }
        public double ReadDouble(long offset)
        {
            BaseStream.Position = offset;
            return ReadDouble();
        }
        public double[] ReadDoubles(int count)
        {
            if (!ValidateArraySize(count, out double[] array))
            {
                ReadBytesIntoBuffer(count, 8);
                array = new double[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToDouble(_buffer, 8 * i);
                }
            }
            return array;
        }
        public double[] ReadDoubles(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadDoubles(count);
        }
        public decimal ReadDecimal()
        {
            ReadBytesIntoBuffer(1, 16);
            return Utils.BytesToDecimal(_buffer, 0);
        }
        public decimal ReadDecimal(long offset)
        {
            BaseStream.Position = offset;
            return ReadDecimal();
        }
        public decimal[] ReadDecimals(int count)
        {
            if (!ValidateArraySize(count, out decimal[] array))
            {
                ReadBytesIntoBuffer(count, 16);
                array = new decimal[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Utils.BytesToDecimal(_buffer, 16 * i);
                }
            }
            return array;
        }
        public decimal[] ReadDecimals(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadDecimals(count);
        }

        public TEnum ReadEnum<TEnum>() where TEnum : struct, Enum
        {
            // Do not allow writing abstract "Enum" because there is no way to know which underlying type to read
            // Yes "struct" restriction on reads
            Type enumType = typeof(TEnum);
            Type underlyingType = Enum.GetUnderlyingType(enumType);
            object value;
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Byte: value = ReadByte(); break;
                case TypeCode.SByte: value = ReadSByte(); break;
                case TypeCode.Int16: value = ReadInt16(); break;
                case TypeCode.UInt16: value = ReadUInt16(); break;
                case TypeCode.Int32: value = ReadInt32(); break;
                case TypeCode.UInt32: value = ReadUInt32(); break;
                case TypeCode.Int64: value = ReadInt64(); break;
                case TypeCode.UInt64: value = ReadUInt64(); break;
                default: throw new ArgumentOutOfRangeException(nameof(underlyingType));
            }
            return (TEnum)Enum.ToObject(enumType, value);
        }
        public TEnum ReadEnum<TEnum>(long offset) where TEnum : struct, Enum
        {
            BaseStream.Position = offset;
            return ReadEnum<TEnum>();
        }
        public TEnum[] ReadEnums<TEnum>(int count) where TEnum : struct, Enum
        {
            if (!ValidateArraySize(count, out TEnum[] array))
            {
                array = new TEnum[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadEnum<TEnum>();
                }
            }
            return array;
        }
        public TEnum[] ReadEnums<TEnum>(int count, long offset) where TEnum : struct, Enum
        {
            BaseStream.Position = offset;
            return ReadEnums<TEnum>(count);
        }

        public DateTime ReadDateTime()
        {
            return DateTime.FromBinary(ReadInt64());
        }
        public DateTime ReadDateTime(long offset)
        {
            BaseStream.Position = offset;
            return ReadDateTime();
        }
        public DateTime[] ReadDateTimes(int count)
        {
            if (!ValidateArraySize(count, out DateTime[] array))
            {
                array = new DateTime[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadDateTime();
                }
            }
            return array;
        }
        public DateTime[] ReadDateTimes(int count, long offset)
        {
            BaseStream.Position = offset;
            return ReadDateTimes(count);
        }

        public T ReadObject<T>() where T : new()
        {
            return (T)ReadObject(typeof(T));
        }
        public object ReadObject(Type objType)
        {
            Utils.ThrowIfCannotReadWriteType(objType);
            object obj = Activator.CreateInstance(objType);
            ReadIntoObject(obj);
            return obj;
        }
        public T ReadObject<T>(long offset) where T : new()
        {
            BaseStream.Position = offset;
            return ReadObject<T>();
        }
        public object ReadObject(Type objType, long offset)
        {
            BaseStream.Position = offset;
            return ReadObject(objType);
        }
        public void ReadIntoObject(IBinarySerializable obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            obj.Read(this);
        }
        public void ReadIntoObject(IBinarySerializable obj, long offset)
        {
            BaseStream.Position = offset;
            ReadIntoObject(obj);
        }
        public void ReadIntoObject(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (obj is IBinarySerializable bs)
            {
                bs.Read(this);
                return;
            }

            Type objType = obj.GetType();
            Utils.ThrowIfCannotReadWriteType(objType);

            // Get public non-static properties
            foreach (PropertyInfo propertyInfo in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryIgnoreAttribute), false))
                {
                    continue; // Skip properties with BinaryIgnoreAttribute
                }

                Type propertyType = propertyInfo.PropertyType;
                object value;

                if (propertyType.IsArray)
                {
                    int arrayLength = Utils.GetArrayLength(obj, objType, propertyInfo);
                    // Get array type
                    Type elementType = propertyType.GetElementType();
                    if (arrayLength == 0)
                    {
                        value = Array.CreateInstance(elementType, 0); // Create 0 length array regardless of type
                    }
                    else
                    {
                        if (elementType.IsEnum)
                        {
                            elementType = Enum.GetUnderlyingType(elementType);
                        }
                        switch (Type.GetTypeCode(elementType))
                        {
                            case TypeCode.Boolean:
                            {
                                BooleanSize booleanSize = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryBooleanSizeAttribute), BooleanSize);
                                value = ReadBooleans(arrayLength, booleanSize);
                                break;
                            }
                            case TypeCode.Byte: value = ReadBytes(arrayLength); break;
                            case TypeCode.SByte: value = ReadSBytes(arrayLength); break;
                            case TypeCode.Char:
                            {
                                EncodingType encodingType = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryEncodingAttribute), Encoding);
                                value = ReadChars(arrayLength, encodingType);
                                break;
                            }
                            case TypeCode.Int16: value = ReadInt16s(arrayLength); break;
                            case TypeCode.UInt16: value = ReadUInt16s(arrayLength); break;
                            case TypeCode.Int32: value = ReadInt32s(arrayLength); break;
                            case TypeCode.UInt32: value = ReadUInt32s(arrayLength); break;
                            case TypeCode.Int64: value = ReadInt64s(arrayLength); break;
                            case TypeCode.UInt64: value = ReadUInt64s(arrayLength); break;
                            case TypeCode.Single: value = ReadSingles(arrayLength); break;
                            case TypeCode.Double: value = ReadDoubles(arrayLength); break;
                            case TypeCode.Decimal: value = ReadDecimals(arrayLength); break;
                            case TypeCode.DateTime: value = ReadDateTimes(arrayLength); break;
                            case TypeCode.String:
                            {
                                Utils.GetStringLength(obj, objType, propertyInfo, true, out bool? nullTerminated, out int stringLength);
                                EncodingType encodingType = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryEncodingAttribute), Encoding);
                                if (nullTerminated == true)
                                {
                                    value = ReadStringsNullTerminated(arrayLength, encodingType);
                                }
                                else
                                {
                                    value = ReadStrings(arrayLength, stringLength, encodingType);
                                }
                                break;
                            }
                            case TypeCode.Object:
                            {
                                value = Array.CreateInstance(elementType, arrayLength);
                                if (typeof(IBinarySerializable).IsAssignableFrom(elementType))
                                {
                                    for (int i = 0; i < arrayLength; i++)
                                    {
                                        var serializable = (IBinarySerializable)Activator.CreateInstance(elementType);
                                        serializable.Read(this);
                                        ((Array)value).SetValue(serializable, i);
                                    }
                                }
                                else // Element's type is not supported so try to read the array's objects
                                {
                                    for (int i = 0; i < arrayLength; i++)
                                    {
                                        object elementObj = ReadObject(elementType);
                                        ((Array)value).SetValue(elementObj, i);
                                    }
                                }
                                break;
                            }
                            default: throw new ArgumentOutOfRangeException(nameof(elementType));
                        }
                    }
                }
                else
                {
                    if (propertyType.IsEnum)
                    {
                        propertyType = Enum.GetUnderlyingType(propertyType);
                    }
                    switch (Type.GetTypeCode(propertyType))
                    {
                        case TypeCode.Boolean:
                        {
                            BooleanSize booleanSize = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryBooleanSizeAttribute), BooleanSize);
                            value = ReadBoolean(booleanSize);
                            break;
                        }
                        case TypeCode.Byte: value = ReadByte(); break;
                        case TypeCode.SByte: value = ReadSByte(); break;
                        case TypeCode.Char:
                        {
                            EncodingType encodingType = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryEncodingAttribute), Encoding);
                            value = ReadChar(encodingType);
                            break;
                        }
                        case TypeCode.Int16: value = ReadInt16(); break;
                        case TypeCode.UInt16: value = ReadUInt16(); break;
                        case TypeCode.Int32: value = ReadInt32(); break;
                        case TypeCode.UInt32: value = ReadUInt32(); break;
                        case TypeCode.Int64: value = ReadInt64(); break;
                        case TypeCode.UInt64: value = ReadUInt64(); break;
                        case TypeCode.Single: value = ReadSingle(); break;
                        case TypeCode.Double: value = ReadDouble(); break;
                        case TypeCode.Decimal: value = ReadDecimal(); break;
                        case TypeCode.DateTime: value = ReadDateTime(); break;
                        case TypeCode.String:
                        {
                            Utils.GetStringLength(obj, objType, propertyInfo, true, out bool? nullTerminated, out int stringLength);
                            EncodingType encodingType = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryEncodingAttribute), Encoding);
                            if (nullTerminated == true)
                            {
                                value = ReadStringNullTerminated(encodingType);
                            }
                            else
                            {
                                value = ReadString(stringLength, encodingType);
                            }
                            break;
                        }
                        case TypeCode.Object:
                        {
                            if (typeof(IBinarySerializable).IsAssignableFrom(propertyType))
                            {
                                value = Activator.CreateInstance(propertyType);
                                ((IBinarySerializable)value).Read(this);
                            }
                            else // The property's type is not supported so try to read the object
                            {
                                value = ReadObject(propertyType);
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(propertyType));
                    }
                }

                // Set the value into the property
                propertyInfo.SetValue(obj, value);
            }
        }
        public void ReadIntoObject(object obj, long offset)
        {
            BaseStream.Position = offset;
            ReadIntoObject(obj);
        }
    }
}
