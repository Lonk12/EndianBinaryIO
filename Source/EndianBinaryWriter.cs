﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Kermalis.EndianBinaryIO
{
    public sealed class EndianBinaryWriter : EndianBinaryBase
    {
        public EndianBinaryWriter(Stream baseStream, Endianness endianness = Endianness.LittleEndian, EncodingType encoding = EncodingType.ASCII)
            : base(baseStream, endianness, encoding, false) { }

        internal override void DoNotInheritOutsideOfThisAssembly() { }

        void SetBufferSize(int size)
        {
            if (buffer == null || buffer.Length < size)
                buffer = new byte[size];
        }
        void WriteBytesFromBuffer(int count, int primitiveSize)
        {
            int byteCount = count * primitiveSize;
            Flip(byteCount, primitiveSize);
            BaseStream.Write(buffer, 0, byteCount);
        }

        public void Write(bool value, BooleanSize size)
        {
            switch (size)
            {
                case BooleanSize.U8:
                    SetBufferSize(1);
                    buffer[0] = value ? (byte)1 : (byte)0;
                    WriteBytesFromBuffer(1, 1);
                    break;
                case BooleanSize.U16:
                    SetBufferSize(2);
                    Array.Copy(BitConverter.GetBytes(value ? (short)1 : (short)0), 0, buffer, 0, 2);
                    WriteBytesFromBuffer(1, 2);
                    break;
                case BooleanSize.U32:
                    SetBufferSize(4);
                    Array.Copy(BitConverter.GetBytes(value ? 1u : 0u), 0, buffer, 0, 4);
                    WriteBytesFromBuffer(1, 4);
                    break;
                default: throw new ArgumentException("Invalid BooleanSize value.");
            }
        }
        public void Write(bool value, BooleanSize size, long offset)
        {
            BaseStream.Position = offset;
            Write(value, size);
        }
        public void Write(bool[] value, BooleanSize size)
        {
            Write(value, 0, value.Length, size);
        }
        public void Write(bool[] value, BooleanSize size, long offset)
        {
            BaseStream.Position = offset;
            Write(value, 0, value.Length, size);
        }
        public void Write(bool[] value, int index, int count, BooleanSize size)
        {
            for (int i = index; i < count; i++)
                Write(value[i], size);
        }
        public void Write(bool[] value, int index, int count, BooleanSize size, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count, size);
        }
        public void Write(byte value)
        {
            SetBufferSize(1);
            buffer[0] = value;
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
            Array.Copy(value, index, buffer, 0, count);
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
            buffer[0] = (byte)value;
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
                buffer[i] = (byte)value[i + index];
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
            Array.Copy(encoding.GetBytes(new string(value, 1)), 0, buffer, 0, encodingSize);
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
            Array.Copy(encoding.GetBytes(value, index, count), 0, buffer, 0, count * encodingSize);
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
                Write('\0', encodingType);
        }
        public void Write(string value, bool nullTerminated, EncodingType encodingType, long offset)
        {
            BaseStream.Position = offset;
            Write(value, nullTerminated, encodingType);
        }
        public void Write(short value)
        {
            SetBufferSize(2);
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 2);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 2, 2);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 2);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 2, 2);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 4);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 4, 4);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 4);
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
        public void Write(uint[] value, int offset, int count)
        {
            SetBufferSize(4 * count);
            for (int i = 0; i < count; i++)
                Array.Copy(BitConverter.GetBytes(value[i + offset]), 0, buffer, i * 4, 4);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 8);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 8, 8);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 8);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 8, 8);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 4);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 4, 4);
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
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 8);
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
                Array.Copy(BitConverter.GetBytes(value[i + index]), 0, buffer, i * 8, 8);
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
            Array.Copy(Utils.DecimalToBytes(value), 0, buffer, 0, 16);
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
                Array.Copy(Utils.DecimalToBytes(value[i + index]), 0, buffer, i * 16, 16);
            WriteBytesFromBuffer(count, 16);
        }
        public void Write(decimal[] value, int index, int count, long offset)
        {
            BaseStream.Position = offset;
            Write(value, index, count);
        }

        public void WriteObject(object obj)
        {
            // Get type
            Type objType = obj.GetType();
            // Get members
            MemberInfo[] members = objType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, null, null);
            // Check for a StructLayoutAttribute
            bool ordered = objType.StructLayoutAttribute.Value == LayoutKind.Explicit;
            // Store this object's start offset
            long objectStart = BaseStream.Position;

            foreach (var memberInfo in members)
            {
                // Members with an IgnoreAttribute get skipped
                if (Utils.AttributeValueOrDefault(memberInfo, typeof(BinaryIgnoreAttribute), false))
                    continue;

                int fixedLength = Utils.AttributeValueOrDefault(memberInfo, typeof(BinaryFixedLengthAttribute), 0);
                BooleanSize booleanSize = Utils.AttributeValueOrDefault(memberInfo, typeof(BinaryBooleanSizeAttribute), BooleanSize.U8);
                EncodingType encodingType = Utils.AttributeValueOrDefault(memberInfo, typeof(BinaryStringEncodingAttribute), Encoding);
                bool nullTerminated = Utils.AttributeValueOrDefault(memberInfo, typeof(BinaryStringNullTerminatedAttribute), false);

                // Determine member's start offset
                long memberStart = ordered ?
                    objectStart + Utils.AttributeValueOrDefault(memberInfo, typeof(FieldOffsetAttribute), -1) :
                    BaseStream.Position;

                // Get member's type
                Type memberType;
                object value = null;
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    memberType = ((PropertyInfo)memberInfo).PropertyType;
                    value = ((PropertyInfo)memberInfo).GetValue(obj, null);
                }
                else
                {
                    memberType = ((FieldInfo)memberInfo).FieldType;
                    value = ((FieldInfo)memberInfo).GetValue(obj);
                }

                if (memberType.IsArray)
                {
                    // Get array type
                    Type elementType = memberType.GetElementType();
                    if (elementType.IsEnum)
                        elementType = elementType.GetEnumUnderlyingType();
                    switch (elementType.Name)
                    {
                        case "Boolean": Write((bool[])value, 0, fixedLength, booleanSize, memberStart); break;
                        case "Byte": Write((byte[])value, 0, fixedLength, memberStart); break;
                        case "SByte": Write((sbyte[])value, 0, fixedLength, memberStart); break;
                        case "Char": Write((char[])value, 0, fixedLength, encodingType, memberStart); break;
                        case "Int16": Write((short[])value, 0, fixedLength, memberStart); break;
                        case "UInt16": Write((ushort[])value, 0, fixedLength, memberStart); break;
                        case "Int32": Write((int[])value, 0, fixedLength, memberStart); break;
                        case "UInt32": Write((uint[])value, 0, fixedLength, memberStart); break;
                        case "Int64": Write((long[])value, 0, fixedLength, memberStart); break;
                        case "UInt64": Write((ulong[])value, 0, fixedLength, memberStart); break;
                        case "Single": Write((float[])value, 0, fixedLength, memberStart); break;
                        case "Double": Write((double[])value, 0, fixedLength, memberStart); break;
                        case "Decimal": Write((decimal[])value, 0, fixedLength, memberStart); break;
                        case "String":
                            throw new ArgumentException("Cannot write an array of strings yet.");
                        default: // IBinarySerializable
                            if (!typeof(IBinarySerializable).IsAssignableFrom(elementType))
                                throw new ArgumentException("\"" + elementType.FullName + "\" cannot be written to the stream.");
                            BaseStream.Position = memberStart;
                            for (int i = 0; i < fixedLength; i++)
                                ((IBinarySerializable)((Array)value).GetValue(i)).Write(this);
                            break;
                    }
                }
                else
                {
                    if (memberType.IsEnum)
                        memberType = memberType.GetEnumUnderlyingType();
                    switch (memberType.Name)
                    {
                        case "Boolean": Write((bool)value, booleanSize, memberStart); break;
                        case "Byte": Write((byte)value, memberStart); break;
                        case "SByte": Write((sbyte)value, memberStart); break;
                        case "Char": Write((char)value, encodingType, memberStart); break;
                        case "Int16": Write((short)value, memberStart); break;
                        case "UInt16": Write((ushort)value, memberStart); break;
                        case "Int32": Write((int)value, memberStart); break;
                        case "UInt32": Write((uint)value, memberStart); break;
                        case "Int64": Write((long)value, memberStart); break;
                        case "UInt64": Write((ulong)value, memberStart); break;
                        case "Single": Write((float)value, memberStart); break;
                        case "Double": Write((double)value, memberStart); break;
                        case "Decimal": Write((decimal)value, memberStart); break;
                        case "String":
                            if (nullTerminated)
                                Write((string)value, true, encodingType, memberStart);
                            else
                            {
                                char[] chars = null;
                                Utils.TruncateOrNot((string)value, fixedLength, ref chars);
                                Write(chars, encodingType, memberStart);
                            }
                            break;
                        default: // IBinarySerializable
                            if (!typeof(IBinarySerializable).IsAssignableFrom(memberType))
                                throw new ArgumentException("\"" + memberType.FullName + "\" cannot be written to the stream.");
                            BaseStream.Position = memberStart;
                            ((IBinarySerializable)value).Write(this);
                            break;
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
