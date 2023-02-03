﻿using Kermalis.EndianBinaryIO;
using System.IO;
using System.Linq;
using Xunit;

namespace Kermalis.EndianBinaryIOTests;

public sealed class DecimalTests
{
	#region Constants

	private const decimal TEST_VAL = 12_345_678_909_876_543_210.123456789m;
	private static readonly byte[] _testValBytesLE = new byte[sizeof(decimal)]
	{
		0x15, 0x71, 0x84, 0xA0, /**/ 0x75, 0x40, 0xAD, 0xBE, /**/ 0x32, 0x1B, 0xE4, 0x27, /**/ 0x00, 0x00, 0x09, 0x00,
	};
	private static readonly byte[] _testValBytesBE = new byte[sizeof(decimal)]
	{
		0xA0, 0x84, 0x71, 0x15, /**/ 0xBE, 0xAD, 0x40, 0x75, /**/ 0x27, 0xE4, 0x1B, 0x32, /**/ 0x00, 0x09, 0x00, 0x00,
	};

	private static readonly decimal[] _testArr = new decimal[4]
	{
		-18_185_544_635_427_120_524.93305179m,
		-22_010_447_631_927_599_247.18039726m,
		41_455_770_299_484_821_081.65900781m,
		22_442_965_292_979_427_993.29821457m,
	};
	private static readonly byte[] _testArrBytesLE = new byte[4 * sizeof(decimal)]
	{
		0x5B, 0xC5, 0x4B, 0x5E, /**/ 0x65, 0xE6, 0xFF, 0x01, /**/ 0xE3, 0x45, 0xE0, 0x05, /**/ 0x00, 0x00, 0x08, 0x80,
		0xAE, 0xF2, 0x4B, 0xBB, /**/ 0xA4, 0x20, 0x17, 0xB3, /**/ 0x5B, 0xA9, 0x1C, 0x07, /**/ 0x00, 0x00, 0x08, 0x80,
		0xED, 0xC9, 0xFE, 0x4A, /**/ 0x54, 0x20, 0x93, 0x1A, /**/ 0x15, 0x24, 0x65, 0x0D, /**/ 0x00, 0x00, 0x08, 0x00,
		0x11, 0x83, 0x14, 0x50, /**/ 0x63, 0x4A, 0x69, 0xA3, /**/ 0x46, 0x70, 0x40, 0x07, /**/ 0x00, 0x00, 0x08, 0x00,
	};
	private static readonly byte[] _testArrBytesBE = new byte[4 * sizeof(decimal)]
	{
		0x5E, 0x4B, 0xC5, 0x5B, /**/ 0x01, 0xFF, 0xE6, 0x65, /**/ 0x05, 0xE0, 0x45, 0xE3, /**/ 0x80, 0x08, 0x00, 0x00,
		0xBB, 0x4B, 0xF2, 0xAE, /**/ 0xB3, 0x17, 0x20, 0xA4, /**/ 0x07, 0x1C, 0xA9, 0x5B, /**/ 0x80, 0x08, 0x00, 0x00,
		0x4A, 0xFE, 0xC9, 0xED, /**/ 0x1A, 0x93, 0x20, 0x54, /**/ 0x0D, 0x65, 0x24, 0x15, /**/ 0x00, 0x08, 0x00, 0x00,
		0x50, 0x14, 0x83, 0x11, /**/ 0xA3, 0x69, 0x4A, 0x63, /**/ 0x07, 0x40, 0x70, 0x46, /**/ 0x00, 0x08, 0x00, 0x00,
	};

	#endregion

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void ReadDecimal(bool le)
	{
		byte[] input = le ? _testValBytesLE : _testValBytesBE;
		Endianness e = le ? Endianness.LittleEndian : Endianness.BigEndian;

		decimal val;
		using (var stream = new MemoryStream(input))
		{
			val = new EndianBinaryReader(stream, endianness: e).ReadDecimal();
		}
		Assert.Equal(TEST_VAL, val);
	}
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void ReadDecimals(bool le)
	{
		byte[] input = le ? _testArrBytesLE : _testArrBytesBE;
		Endianness e = le ? Endianness.LittleEndian : Endianness.BigEndian;

		decimal[] arr = new decimal[4];
		using (var stream = new MemoryStream(input))
		{
			new EndianBinaryReader(stream, endianness: e).ReadDecimals(arr);
		}
		Assert.True(arr.SequenceEqual(_testArr));
	}
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void WriteDecimal(bool le)
	{
		byte[] input = le ? _testValBytesLE : _testValBytesBE;
		Endianness e = le ? Endianness.LittleEndian : Endianness.BigEndian;

		byte[] bytes = new byte[sizeof(decimal)];
		using (var stream = new MemoryStream(bytes))
		{
			new EndianBinaryWriter(stream, endianness: e).WriteDecimal(TEST_VAL);
		}
		Assert.True(bytes.SequenceEqual(input));
	}
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void WriteDecimals(bool le)
	{
		byte[] input = le ? _testArrBytesLE : _testArrBytesBE;
		Endianness e = le ? Endianness.LittleEndian : Endianness.BigEndian;

		byte[] bytes = new byte[4 * sizeof(decimal)];
		using (var stream = new MemoryStream(bytes))
		{
			new EndianBinaryWriter(stream, endianness: e).WriteDecimals(_testArr);
		}
		Assert.True(bytes.SequenceEqual(input));
	}
}
