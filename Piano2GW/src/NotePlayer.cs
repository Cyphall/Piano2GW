using System;
using System.Diagnostics;
using System.Threading;

namespace Piano2GW
{
	public static class NotePlayer
	{
		private static Instrument _instrument;

		private static int _currentOctave;
		
		private static Stopwatch _timeSinceLastOctaveSwitch = new Stopwatch();
		private const int MIN_TIME_BETWEEN_OCTAVE_SWITCH = 90;

		public static void ChangeInstrument(Instrument instrument)
		{
			_instrument = instrument;
					
			_currentOctave = _instrument.DefaultOctave;
			
			_timeSinceLastOctaveSwitch.Start();
		}
		
		public static void PlayNote(int value)
		{
			Note note = new Note(value);

			if (note.IsSharp())
				return;

			if (note.Octave < _instrument.FirstOctave || (note.Octave > _instrument.LastOctave && !(note.Octave == _instrument.LastOctave + 1 && note.Name == NoteName.C)))
				return;

			int key = AdjustInGameOctave(note);
			
			InputInterface.SendKey(key);
		}

		private static int AdjustInGameOctave(Note note)
		{
			int requestedOctave = note.Octave;
			int key = note.Name switch
			{
				NoteName.C => 1,
				NoteName.D => 2,
				NoteName.E => 3,
				NoteName.F => 4,
				NoteName.G => 5,
				NoteName.A => 6,
				NoteName.B => 7,
				_ => throw new Exception()
			};

			while (_currentOctave != requestedOctave)
			{
				if (_timeSinceLastOctaveSwitch.ElapsedMilliseconds < MIN_TIME_BETWEEN_OCTAVE_SWITCH)
					Thread.Sleep(Math.Max(MIN_TIME_BETWEEN_OCTAVE_SWITCH - (int)_timeSinceLastOctaveSwitch.ElapsedMilliseconds, 0));
				
				if (note.Name == NoteName.C && _currentOctave == requestedOctave - 1)
				{
					return 8;
				}

				if (_currentOctave < requestedOctave)
				{
					InputInterface.SendKey(0);
					_currentOctave++;
				}
				else
				{
					InputInterface.SendKey(9);
					_currentOctave--;
				}

				_timeSinceLastOctaveSwitch.Restart();
			}

			return key;
		}
	}
}