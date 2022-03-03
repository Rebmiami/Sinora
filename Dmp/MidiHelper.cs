using System;

namespace Sinora.Dmp
{
    internal static class MidiHelper
    {
		public static bool IsNoteOn(byte[] dataBuffer)
		{
			return IsNoteOn(dataBuffer[0]);
		}

		public static bool IsNoteOn(byte data)
		{
			return ((data & 0xF0) == 0x90);
		}

		public static bool IsNoteOff(byte[] dataBuffer)
		{
			return IsNoteOff(dataBuffer[0]);
		}

		public static bool IsNoteOff(byte data)
		{
			return ((data & 0xF0) == 0x80);
		}

		public static byte NoteVelocity(byte[] dataBuffer)
		{
			return NoteVelocity(dataBuffer[2]);
		}

		public static byte NoteVelocity(byte data)
		{
			return (byte)(data & 0b01111111);
		}

		public static byte NoteKeyNumber(byte[] dataBuffer)
		{
			return NoteKeyNumber(dataBuffer[1]);
		}

		public static byte NoteKeyNumber(byte data)
		{
			return (byte)(data & 0b01111111);
		}

		public static double NotePitch(byte[] dataBuffer)
		{
			return NotePitch(NoteKeyNumber(dataBuffer));
		}

		public static double NotePitch(byte keynumber)
		{
			return 440 * Math.Pow(2, (double)(keynumber - 69) / 12);
		}
	}
}
