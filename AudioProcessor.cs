using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using System;
using System.Diagnostics;
using Sinora.Dsp;
using Sinora.Dmp;
using System.Collections.Generic;

namespace Sinora.Dsp
{
	/// <summary>
	/// This object performs audio processing for your plugin.
	/// </summary>
	internal sealed class AudioProcessor : VstPluginAudioProcessor, IVstPluginBypass, IVstMidiProcessor
	{
		/// <summary>
		/// TODO: assign the input count.
		/// </summary>
		private const int AudioInputCount = 2;
		/// <summary>
		/// TODO: assign the output count.
		/// </summary>
		private const int AudioOutputCount = 2;
		/// <summary>
		/// TODO: assign the tail size.
		/// </summary>
		private const int InitialTailSize = 0;

		// TODO: change this to your specific needs.
		private readonly VstTimeInfoFlags _defaultTimeInfoFlags = VstTimeInfoFlags.ClockValid;
		// set after the plugin is opened
		private IVstHostSequencer? _sequencer;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AudioProcessor(IVstPluginEvents pluginEvents, PluginParameters parameters)
			: base(AudioInputCount, AudioOutputCount, InitialTailSize, noSoundInStop: false)
		{
			Throw.IfArgumentIsNull(pluginEvents, nameof(pluginEvents));
			Throw.IfArgumentIsNull(parameters, nameof(parameters));

			NoteStack = new Stack<byte>(128);
			Notes = new bool[128];
			osc = new Oscillator();

			pluginEvents.Opened += Plugin_Opened;


			Note_Released += NoteReleased;

			Note_Pressed += NotePressed;
		}

		// Used to ensure that the currently playing note is the one last inputted by the user.
		public Stack<byte> NoteStack { get; set; }
		public bool[] Notes { get; set; }

		public Oscillator osc;

		/// <summary>
		/// Override the default implementation to pass it through to the delay.
		/// </summary>
		public override float SampleRate
		{
			get { return osc.SampleRate; }
			set
			{
				osc.SampleRate = value;
			}
		}

		private VstTimeInfo? _timeInfo;
		/// <summary>
		/// Gets the current time info.
		/// </summary>
		/// <remarks>The Time Info is refreshed with each call to Process.</remarks>
		internal VstTimeInfo? TimeInfo
		{
			get
			{
				if (_timeInfo == null && _sequencer != null)
				{
					_timeInfo = _sequencer.GetTime(_defaultTimeInfoFlags);
				}

				return _timeInfo;
			}
		}

		private void Plugin_Opened(object? sender, EventArgs e)
		{
			var plugin = (VstPlugin?)sender;

			// A reference to the host is only available after 
			// the plugin has been loaded and opened by the host.
			_sequencer = plugin?.Host?.GetInstance<IVstHostSequencer>();
		}

		/// <summary>
		/// Called by the host to allow the plugin to process audio samples.
		/// </summary>
		/// <param name="inChannels">Never null.</param>
		/// <param name="outChannels">Never null.</param>
		public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
		{
			// by resetting the time info each cycle, accessing the TimeInfo property will fetch new info.
			_timeInfo = null;

			if (!Bypass)
			{
				// check assumptions
				Debug.Assert(outChannels.Length == inChannels.Length);

				// TODO: Implement your audio (effect) processing here.

				for (int i = 0; i < outChannels.Length; i++)
				{
					Process(osc,
						inChannels[i], outChannels[i], i % 2 == 0);
				}
			}
			else
			{
				// calling the base class transfers input samples to the output channels unchanged (bypass).
				base.Process(inChannels, outChannels);
			}
		}

		// process a single audio channel
		private void Process(Oscillator oscillator, VstAudioBuffer input, VstAudioBuffer output, bool left)
		{
			if (oscillator.Playing)
			if (left)
			{
				for (int i = 0; i < input.SampleCount; i++)
				{
					oscillator.LeftTimer++;
					output[i] = oscillator.GetSample(oscillator.LeftTimer);
				}
			}
			else
			{
				for (int i = 0; i < input.SampleCount; i++)
				{
					oscillator.RightTimer++;
					output[i] = oscillator.GetSample(oscillator.RightTimer);
				}
			}
		}

		private void NotePressed(object? sender, EventArgs e)
		{
			MidiEventArgs midiEventArgs = (MidiEventArgs)e;

			PlayNote(MidiHelper.NoteKeyNumber(midiEventArgs.midiEvent.Data));
		}

		private void NoteReleased(object? sender, EventArgs e)
		{
			MidiEventArgs midiEventArgs = (MidiEventArgs)e;

			ReleaseNote(MidiHelper.NoteKeyNumber(midiEventArgs.midiEvent.Data));
		}

		public static EventHandler Note_Pressed;

		public static EventHandler Note_Released;

		public void Process(VstEventCollection events)
		{
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i].EventType == VstEventTypes.MidiEvent)
				{
					VstMidiEvent midiEvent = (VstMidiEvent)events[i];

					
					if (MidiHelper.IsNoteOn(midiEvent.Data))
					{
						Note_Pressed.Invoke(this, new MidiEventArgs(midiEvent));
					}

					if (MidiHelper.IsNoteOff(midiEvent.Data))
					{
						Note_Released.Invoke(this, new MidiEventArgs(midiEvent));
					}
				}
			}
		}

		public void PlayNote(byte note)
		{
			Notes[note] = true;
			NoteStack.Push(note);

			osc.PlayNote(note);
		}

		public void ReleaseNote(byte note)
		{
			Notes[note] = false;
			byte playing = NoteStack.Peek();
			
			if (playing == note)
			{
				NoteStack.Pop();
				bool b = NoteStack.TryPeek(out byte next);

				if (b)
				{
					if (Notes[next])
					{
						osc.PlayNote(next);
					}
					else
					{
						ReleaseNote(next);
					}
				}
				else
				{
					osc.Stop();
				}
			}
		}

		#region IVstPluginBypass Members

		public bool Bypass { get; set; }

		public int ChannelCount => throw new NotImplementedException();

		#endregion
	}
}
