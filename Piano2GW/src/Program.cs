using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Devices.Infos;
using RtMidi.Core.Messages;

namespace Piano2GW
{
	internal static class Program
	{
		private static IMidiInputDevice _device;
		private static Instrument _instrument;

		private static int _currentOctave;

		private static Stopwatch _timeSinceLastOctaveSwitch = new Stopwatch();
		private const int MIN_TIME_BETWEEN_OCTAVE_SWITCH = 90;

		private static void Main()
		{
			ChooseDevice();

			if (_device == null)
				return;
			
			ChooseInstrument();

			_currentOctave = _instrument.DefaultOctave;

			_device.NoteOn += (IMidiInputDevice sender, in NoteOnMessage msg) => {
				if (msg.Velocity != 0)
					PressNote((int)msg.Key);
			};
			_device.Open();
			
			_timeSinceLastOctaveSwitch.Start();
			
			Console.WriteLine("Running... Press Q to quit.");

			while (Console.ReadKey().KeyChar != 'q')
			{
				
			}
			
			_device.Dispose();
		}
		
		#region Note handling
		
		private static void PressNote(int value)
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
		
		#endregion
		
		#region Setup

		private static void ChooseDevice()
		{
			while (_device == null)
			{
				List<IMidiInputDeviceInfo> devices = new List<IMidiInputDeviceInfo>();
				foreach (IMidiInputDeviceInfo device in MidiDeviceManager.Default.InputDevices)
				{
					devices.Add(device);
				}

				switch (devices.Count)
				{
					case 0:
					{
						Console.WriteLine("No device has been found.");
						Console.Write("Retry? [y/n] ");
						string res = Console.ReadLine();

						if (string.IsNullOrEmpty(res) || res.ToLower() == "y")
						{
							continue;
						}
						return;
					}
					case 1:
					{
						_device = devices[0].CreateDevice();
						return;
					}
					default:
					{
						_device = PickChoice(devices, info => info.Name, "Available Devices:", "Choose a device: ").CreateDevice();
						return;
					}
				}
			}
		}

		private static void ChooseInstrument()
		{
			_instrument = PickChoice(Instrument.Instruments.ToList(), pair => pair.Key, "Available instruments:", "Choose an instrument: ").Value;
		}

		private static T PickChoice<T>(List<T> values, Func<T, string> getPrintableString, string header, string choiceSentence)
		{
			List<string> names = values.Select(getPrintableString).ToList();

			int i = 0;
			string list = "";
			foreach (string name in names)
			{
				list += $"[{i}] {name}\n";
				i++;
			}

			int choice = -1;
			while (choice == -1)
			{
				Console.WriteLine();
				Console.WriteLine(header);
				Console.WriteLine(list);
				
				Console.Write(choiceSentence);
				string choiceStr = Console.ReadLine();
				Console.WriteLine();
				
				if (!int.TryParse(choiceStr ?? string.Empty, out int n))
				{
					Console.WriteLine("Error: The input is not a number");
					continue;
				}

				if (n < 0 || n >= names.Count)
				{
					Console.WriteLine("Error: The number is out of range");
					continue;
				}
				
				choice = n;
			}

			return values[choice];
		}
		
		#endregion
	}
}