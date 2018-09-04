﻿using Kermalis.EndianBinaryIO;
using System;
using System.IO;

namespace Kermalis.EndianBinaryTesting
{
#pragma warning disable CS0649
    class MyStruct
    {
        // Members that are not ignored
        public byte VersionMajor;
        public short VersionMinor;

        // Member that is ignored when reading and writing
        [BinaryIgnore(true)]
        public double DoNotReadOrWrite = Math.PI;

        // Arrays work as well
        [BinaryFixedLength(16)]
        public uint[] ArrayWith16Elements;

        // Boolean that occupies 4 bytes instead of one
        [BinaryBooleanSize(BooleanSize.U32)]
        public bool LongBool;

        // String encoded in ASCII
        // Reads chars until the stream encounters a '\0'
        // Writing will append a '\0' at the end of the string
        [BinaryStringEncoding(EncodingType.ASCII)]
        [BinaryStringNullTerminated(true)]
        public string NullTerminatedASCIIString;

        // String encoded in UTF-16 that will only read/write 10 chars
        [BinaryStringEncoding(EncodingType.UTF16)]
        [BinaryFixedLength(10)]
        public string UTF16String;
    }

    class Test
    {
        static readonly byte[] myStructBytesLittleEndian =
        {
            0x02,
            0xFF, 0x01,

            0x00, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00,
            0x05, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00,
            0x07, 0x00, 0x00, 0x00,
            0x08, 0x00, 0x00, 0x00,
            0x09, 0x00, 0x00, 0x00,
            0x0A, 0x00, 0x00, 0x00,
            0x0B, 0x00, 0x00, 0x00,
            0x0C, 0x00, 0x00, 0x00,
            0x0D, 0x00, 0x00, 0x00,
            0x0E, 0x00, 0x00, 0x00,
            0x0F, 0x00, 0x00, 0x00,

            0x00, 0x00, 0x00, 0x00,

            0x45, 0x6E, 0x64, 0x69, 0x61, 0x6E, 0x42, 0x69, 0x6E, 0x61, 0x72, 0x79, 0x49, 0x4F, 0x00,
            
            0x42, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        static void Main(string[] args)
        {
            var reader = new EndianBinaryReader(new MemoryStream(myStructBytesLittleEndian));
            var myStructObj = reader.ReadObject<MyStruct>();

            Console.WindowHeight = 30;

            Console.WriteLine("Major Version: {0}", myStructObj.VersionMajor);
            Console.WriteLine("Minor Version: {0}", myStructObj.VersionMinor);
            Console.WriteLine();
            Console.WriteLine("Ignored Member: {0}", myStructObj.DoNotReadOrWrite);
            Console.WriteLine();
            Console.WriteLine("Array Length: {0}", myStructObj.ArrayWith16Elements.Length);
            Console.WriteLine("Array elements:");
            foreach (var e in myStructObj.ArrayWith16Elements)
                Console.WriteLine("\t{0}", e);
            Console.WriteLine();
            Console.WriteLine("Long Bool: {0}", myStructObj.LongBool);
            Console.WriteLine();
            Console.WriteLine("Null-Terminated String: \"{0}\"", myStructObj.NullTerminatedASCIIString);
            Console.WriteLine();
            Console.WriteLine("UTF-16 String: \"{0}\"", myStructObj.UTF16String);

            Console.ReadKey();
        }
    }
}
