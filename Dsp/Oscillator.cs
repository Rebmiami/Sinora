using Sinora.Dmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinora
{
	public class Oscillator
	{
		public byte NoteKey { get; set; }

		public double NoteFrequency { get; set; }

		public bool Playing { get; set; }

		public float SampleRate { get; set; }

		public void PlayNote(byte key)
		{
			NoteKey = key;
			NoteFrequency = MidiHelper.NotePitch(key);
			Playing = true;
		}

		public void Stop()
		{
			Playing = false;
		}

		public int LeftTimer { get; set; }
		public int RightTimer { get; set; }

		public float GetSample(int time)
		{
			return (float)Math.Sin(Math.PI * 4 * (NoteFrequency / SampleRate) * time);
		}
	}
}
