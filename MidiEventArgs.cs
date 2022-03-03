using Jacobi.Vst.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinora
{
	class MidiEventArgs : EventArgs
	{
		public VstMidiEvent midiEvent;


		public MidiEventArgs(VstMidiEvent vstMidiEvent)
		{
			midiEvent = vstMidiEvent;
		}
	}
}
