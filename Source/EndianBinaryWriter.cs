﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kermalis.EndianBinaryIO
{
    public class EndianBinaryWriter : IDisposable
    {
        public Stream BaseStream { get; }
        public Endianness Endianness { get; set; }
        public EncodingType Encoding { get; set; }
        public BooleanSize BooleanSize { get; set; }

        private byte[] _buffer;
        private bool _isDisposed;

        public EndianBinaryWriter(Stream baseStream, Endianness endianness = Endianness.LittleEndian, EncodingType encoding = EncodingType.ASCII, BooleanSize booleanSize = BooleanSize.U8)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException(nameof(baseStream));
            }
            if (!baseStream.CanWrite)
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

        private void SetBufferSize(int size)
        {
            if (_buffer == null || _buffer.Length < size)
            {
                _buffer = new byte[size];
            }
        }

        private void WriteBytesFromBuffer(int primitiveCount, int primitiveSize)
        {
            int byteCount = primitiveCount * primitiveSize;
            Utils.FlipPrimitives(_buffer, Endianness, byteCount, primitiveSize);
            BaseStream.Write(_buffer, 0, byteCount);
        }

        private static void TruncateString(string str, int length, out char[] toArray)
        {
            toArray = new char[length];
            int numCharsToCopy = Math.Min(length, str.Length);
            for (int i = 0; i < numCharsToCopy; i++)
            {
                toArray[i] = str[i];
            }
        }

        public void Write(bool value)
        {
            Write(value, BooleanSize);
        }
        public void Write(bool value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, BooleanSize);
        }
        public void Write(bool value, BooleanSize booleanSize)
        {
            switch (booleanSize)
            {
                case BooleanSize.U8:
                {
                    SetBufferSize(1);
                    _buffer[0] = value ? (byte)1 : (byte)0;
                    WriteBytesFromBuffer(1, 1);
                    break;
                }
                case BooleanSize.U16:
                {
                    SetBufferSize(2);
                    byte[] bytes = Utils.Int16ToBytes(value ? (short)1 : (short)0);
                    for (int i = 0; i < 2; i++)
                    {
                        _buffer[i] = bytes[i];
                    }
                    WriteBytesFromBuffer(1, 2);
                    break;
                }
                case BooleanSize.U32:
                {
                    SetBufferSize(4);
                    byte[] bytes = Utils.Int32ToBytes(value ? 1 : 0);
                    for (int i = 0; i < 4; i++)
                    {
                        _buffer[i] = bytes[i];
                    }
                    WriteBytesFromBuffer(1, 4);
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(booleanSize));
            }
        }
        public void Write(bool value, BooleanSize booleanSize, long offset)
        {
            BaseStream.Position = offset;
            Write(value, booleanSize);
        }
        public void Write(bool[] value)
        {
            Write(value, 0, value.Length, BooleanSize);
        }
        public void Write(bool[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length, BooleanSize);
        }
        public void Write(bool[] value, BooleanSize booleanSize)
        {
            Write(value, 0, value.Length, booleanSize);
        }
        public void Write(bool[] value, BooleanSize booleanSize, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length, booleanSize);
        }
        public void Write(bool[] value, int index, int count)
        {
            for (int i = index; i < count; i++)
            {
                Write(value[i], BooleanSize);
            }
        }
        public void Write(bool[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count, BooleanSize);
        }
        public void Write(bool[] value, int index, int count, BooleanSize booleanSize)
        {
            for (int i = index; i < count; i++)
            {
                Write(value[i], booleanSize);
            }
        }
        public void Write(bool[] value, int index, int count, BooleanSize booleanSize, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count, booleanSize);
        }
        public void Write(byte value)
        {
            SetBufferSize(1);
            _buffer[0] = value;
            WriteBytesFromBuffer(1, 1);
        }
        public void Write(byte value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(byte[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(byte[] value, int index, int count)
        {
            SetBufferSize(count);
            for (int i = 0; i < count; i++)
            {
                _buffer[i] = value[i + index];
            }
            WriteBytesFromBuffer(count, 1);
        }
        public void Write(byte[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(sbyte value)
        {
            SetBufferSize(1);
            _buffer[0] = (byte)value;
            WriteBytesFromBuffer(1, 1);
        }
        public void Write(sbyte value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(sbyte[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(sbyte[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(sbyte[] value, int index, int count)
        {
            SetBufferSize(count);
            for (int i = 0; i < count; i++)
            {
                _buffer[i] = (byte)value[i + index];
            }
            WriteBytesFromBuffer(count, 1);
        }
        public void Write(sbyte[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(char value)
        {
            Write(value, Encoding);
        }
        public void Write(char value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, Encoding);
        }
        public void Write(char value, EncodingType encodingType)
        {
            Encoding encoding = Utils.EncodingFromEnum(encodingType);
            int encodingSize = Utils.EncodingSize(encoding);
            SetBufferSize(encodingSize);
            byte[] bytes = encoding.GetBytes(new string(value, 1));
            for (int i = 0; i < encodingSize; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, encodingSize);
        }
        public void Write(char value, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            Write(value, encodingType);
        }
        public void Write(char[] value)
        {
            Write(value, 0, value.Length, Encoding);
        }
        public void Write(char[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length, Encoding);
        }
        public void Write(char[] value, EncodingType encodingType)
        {
            Write(value, 0, value.Length, encodingType);
        }
        public void Write(char[] value, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length, encodingType);
        }
        public void Write(char[] value, int index, int count)
        {
            Write(value, index, count, Encoding);
        }
        public void Write(char[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count, Encoding);
        }
        public void Write(char[] value, int index, int count, EncodingType encodingType)
        {
            Encoding encoding = Utils.EncodingFromEnum(encodingType);
            int encodingSize = Utils.EncodingSize(encoding);
            SetBufferSize(encodingSize * count);
            byte[] bytes = encoding.GetBytes(value, index, count);
            for (int i = 0; i < count * encodingSize; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(count, encodingSize);
        }
        public void Write(char[] value, int index, int count, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count, encodingType);
        }
        public void Write(string value, bool nullTerminated)
        {
            Write(value, nullTerminated, Encoding);
        }
        public void Write(string value, bool nullTerminated, long offset)
        {
            BaseStream.Position = offset;
            Write(value, nullTerminated, Encoding);
        }
        public void Write(string value, bool nullTerminated, EncodingType encodingType)
        {
            Write(value.ToCharArray(), encodingType);
            if (nullTerminated)
            {
                Write('\0', encodingType);
            }
        }
        public void Write(string value, bool nullTerminated, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            Write(value, nullTerminated, encodingType);
        }
        public void Write(short value)
        {
            SetBufferSize(2);
            byte[] bytes = Utils.Int16ToBytes(value);
            for (int i = 0; i < 2; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 2);
        }
        public void Write(short value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(short[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(short[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(short[] value, int index, int count)
        {
            SetBufferSize(2 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int16ToBytes(value[i + index]);
                for (int j = 0; j < 2; j++)
                {
                    _buffer[(i * 2) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 2);
        }
        public void Write(short[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(ushort value)
        {
            SetBufferSize(2);
            byte[] bytes = Utils.Int16ToBytes((short)value);
            for (int i = 0; i < 2; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 2);
        }
        public void Write(ushort value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(ushort[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(ushort[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(ushort[] value, int index, int count)
        {
            SetBufferSize(2 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int16ToBytes((short)value[i + index]);
                for (int j = 0; j < 2; j++)
                {
                    _buffer[(i * 2) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 2);
        }
        public void Write(ushort[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(int value)
        {
            SetBufferSize(4);
            byte[] bytes = Utils.Int32ToBytes(value);
            for (int i = 0; i < 4; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 4);
        }
        public void Write(int value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(int[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(int[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(int[] value, int index, int count)
        {
            SetBufferSize(4 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int32ToBytes(value[i + index]);
                for (int j = 0; j < 4; j++)
                {
                    _buffer[(i * 4) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 4);
        }
        public void Write(int[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(uint value)
        {
            SetBufferSize(4);
            byte[] bytes = Utils.Int32ToBytes((int)value);
            for (int i = 0; i < 4; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 4);
        }
        public void Write(uint value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(uint[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(uint[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(uint[] value, int index, int count)
        {
            SetBufferSize(4 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int32ToBytes((int)value[i + index]);
                for (int j = 0; j < 4; j++)
                {
                    _buffer[(i * 4) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 4);
        }
        public void Write(uint[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(long value)
        {
            SetBufferSize(8);
            byte[] bytes = Utils.Int64ToBytes(value);
            for (int i = 0; i < 8; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 8);
        }
        public void Write(long value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(long[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(long[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(long[] value, int index, int count)
        {
            SetBufferSize(8 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int64ToBytes(value[i + index]);
                for (int j = 0; j < 8; j++)
                {
                    _buffer[(i * 8) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 8);
        }
        public void Write(long[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(ulong value)
        {
            SetBufferSize(8);
            byte[] bytes = Utils.Int64ToBytes((long)value);
            for (int i = 0; i < 8; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 8);
        }
        public void Write(ulong value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(ulong[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(ulong[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(ulong[] value, int index, int count)
        {
            SetBufferSize(8 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.Int64ToBytes((long)value[i + index]);
                for (int j = 0; j < 8; j++)
                {
                    _buffer[(i * 8) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 8);
        }
        public void Write(ulong[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(float value)
        {
            SetBufferSize(4);
            byte[] bytes = Utils.SingleToBytes(value);
            for (int i = 0; i < 4; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 4);
        }
        public void Write(float value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(float[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(float[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(float[] value, int index, int count)
        {
            SetBufferSize(4 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.SingleToBytes(value[i + index]);
                for (int j = 0; j < 4; j++)
                {
                    _buffer[(i * 4) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 4);
        }
        public void Write(float[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(double value)
        {
            SetBufferSize(8);
            byte[] bytes = Utils.DoubleToBytes(value);
            for (int i = 0; i < 8; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 8);
        }
        public void Write(double value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(double[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(double[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(double[] value, int index, int count)
        {
            SetBufferSize(8 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.DoubleToBytes(value[i + index]);
                for (int j = 0; j < 8; j++)
                {
                    _buffer[(i * 8) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 8);
        }
        public void Write(double[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }
        public void Write(decimal value)
        {
            SetBufferSize(16);
            byte[] bytes = Utils.DecimalToBytes(value);
            for (int i = 0; i < 16; i++)
            {
                _buffer[i] = bytes[i];
            }
            WriteBytesFromBuffer(1, 16);
        }
        public void Write(decimal value, long offset)
        {
            BaseStream.Position = offset;
            Write(value);
        }
        public void Write(decimal[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(decimal[] value, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length);
        }
        public void Write(decimal[] value, int index, int count)
        {
            SetBufferSize(16 * count);
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = Utils.DecimalToBytes(value[i + index]);
                for (int j = 0; j < 16; j++)
                {
                    _buffer[(i * 16) + j] = bytes[j];
                }
            }
            WriteBytesFromBuffer(count, 16);
        }
        public void Write(decimal[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }

        public void WriteObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            Type objType = obj.GetType();
            if (typeof(IBinarySerializable).IsAssignableFrom(objType))
            {
                ((IBinarySerializable)obj).Write(this);
            }
            else
            {
                // Get public non-static properties
                foreach (PropertyInfo propertyInfo in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryIgnoreAttribute), false))
                    {
                        continue;
                    }

                    BooleanSize booleanSize = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryBooleanSizeAttribute), BooleanSize);
                    EncodingType encodingType = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryEncodingAttribute), Encoding);
                    bool nullTerminated = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryStringNullTerminatedAttribute), true);

                    int arrayFixedLength = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryArrayFixedLengthAttribute), 0);
                    int stringFixedLength = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryStringFixedLengthAttribute), 0);
                    string arrayVariableLengthAnchor = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryArrayVariableLengthAttribute), string.Empty);
                    int arrayVariableLength = 0;
                    if (!string.IsNullOrEmpty(arrayVariableLengthAnchor))
                    {
                        PropertyInfo anchor = objType.GetProperty(arrayVariableLengthAnchor, BindingFlags.Instance | BindingFlags.Public);
                        if (anchor == null)
                        {
                            throw new MissingMemberException($"A property in \"{objType.FullName}\" has an invalid variable array length anchor.");
                        }
                        else
                        {
                            arrayVariableLength = Convert.ToInt32(anchor.GetValue(obj));
                        }
                    }
                    string stringVariableLengthAnchor = Utils.AttributeValueOrDefault(propertyInfo, typeof(BinaryStringVariableLengthAttribute), string.Empty);
                    int stringVariableLength = 0;
                    if (!string.IsNullOrEmpty(stringVariableLengthAnchor))
                    {
                        PropertyInfo anchor = objType.GetProperty(stringVariableLengthAnchor, BindingFlags.Instance | BindingFlags.Public);
                        if (anchor == null)
                        {
                            throw new MissingMemberException($"A property in \"{objType.FullName}\" has an invalid variable string length anchor.");
                        }
                        else
                        {
                            stringVariableLength = Convert.ToInt32(anchor.GetValue(obj));
                        }
                    }
                    if ((arrayFixedLength > 0 && arrayVariableLength > 0)
                        || (stringFixedLength > 0 && stringVariableLength > 0))
                    {
                        throw new ArgumentException($"A property in \"{objType.FullName}\" has two length attributes.");
                    }
                    // One will be 0 and the other will be the intended length, so it is safe to use Math.Max to get the intended length
                    int arrayLength = Math.Max(arrayFixedLength, arrayVariableLength);
                    int stringLength = Math.Max(stringFixedLength, stringVariableLength);
                    if (stringLength > 0)
                    {
                        nullTerminated = false;
                    }

                    Type propertyType = propertyInfo.PropertyType;
                    object value = propertyInfo.GetValue(obj);

                    if (propertyType.IsArray)
                    {
                        if (arrayLength < 0)
                        {
                            throw new ArgumentOutOfRangeException($"An array in \"{objType.FullName}\" attempted to be written with an invalid length.");
                        }
                        // Get array type
                        Type elementType = propertyType.GetElementType();
                        if (elementType.IsEnum)
                        {
                            elementType = elementType.GetEnumUnderlyingType();
                        }
                        switch (elementType.Name)
                        {
                            case "Boolean": Write((bool[])value, 0, arrayLength, booleanSize); break;
                            case "Byte": Write((byte[])value, 0, arrayLength); break;
                            case "SByte": Write((sbyte[])value, 0, arrayLength); break;
                            case "Char": Write((char[])value, 0, arrayLength, encodingType); break;
                            case "Int16": Write((short[])value, 0, arrayLength); break;
                            case "UInt16": Write((ushort[])value, 0, arrayLength); break;
                            case "Int32": Write((int[])value, 0, arrayLength); break;
                            case "UInt32": Write((uint[])value, 0, arrayLength); break;
                            case "Int64": Write((long[])value, 0, arrayLength); break;
                            case "UInt64": Write((ulong[])value, 0, arrayLength); break;
                            case "Single": Write((float[])value, 0, arrayLength); break;
                            case "Double": Write((double[])value, 0, arrayLength); break;
                            case "Decimal": Write((decimal[])value, 0, arrayLength); break;
                            default:
                            {
                                if (typeof(IBinarySerializable).IsAssignableFrom(elementType))
                                {
                                    for (int i = 0; i < arrayLength; i++)
                                    {
                                        var serializable = (IBinarySerializable)((Array)value).GetValue(i);
                                        serializable.Write(this);
                                    }
                                }
                                else if (elementType.Name == "String")
                                {
                                    for (int i = 0; i < arrayLength; i++)
                                    {
                                        string str = (string)((Array)value).GetValue(i);
                                        if (nullTerminated)
                                        {
                                            Write(str, true, encodingType);
                                        }
                                        else
                                        {
                                            TruncateString(str, stringLength, out char[] chars);
                                            Write(chars, encodingType);
                                        }
                                    }
                                }
                                else // Element's type is not supported so try to write the array's objects
                                {
                                    for (int i = 0; i < arrayLength; i++)
                                    {
                                        WriteObject(((Array)value).GetValue(i));
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (propertyType.IsEnum)
                        {
                            propertyType = propertyType.GetEnumUnderlyingType();
                        }
                        switch (propertyType.Name)
                        {
                            case "Boolean": Write((bool)value, booleanSize); break;
                            case "Byte": Write((byte)value); break;
                            case "SByte": Write((sbyte)value); break;
                            case "Char": Write((char)value, encodingType); break;
                            case "Int16": Write((short)value); break;
                            case "UInt16": Write((ushort)value); break;
                            case "Int32": Write((int)value); break;
                            case "UInt32": Write((uint)value); break;
                            case "Int64": Write((long)value); break;
                            case "UInt64": Write((ulong)value); break;
                            case "Single": Write((float)value); break;
                            case "Double": Write((double)value); break;
                            case "Decimal": Write((decimal)value); break;
                            case "String":
                            {
                                if (nullTerminated)
                                {
                                    Write((string)value, true, encodingType);
                                }
                                else
                                {
                                    TruncateString((string)value, stringLength, out char[] chars);
                                    Write(chars, encodingType);
                                }
                                break;
                            }
                            default:
                            {
                                if (typeof(IBinarySerializable).IsAssignableFrom(propertyType))
                                {
                                    ((IBinarySerializable)value).Write(this);
                                }
                                else // property's type is not supported so try to write the object
                                {
                                    WriteObject(value);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void WriteObject(object obj, long offset)
        {
            BaseStream.Position = offset;
            WriteObject(obj);
        }
    }
}
