using System;
using System.Collections.Generic;
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
			int key;
			
			switch (note.Name)
			{
				case NoteName.C:
					key = 1;
					break;
				case NoteName.D:
					key = 2;
					break;
				case NoteName.E:
					key = 3;
					break;
				case NoteName.F:
					key = 4;
					break;
				case NoteName.G:
					key = 5;
					break;
				case NoteName.A:
					key = 6;
					break;
				case NoteName.B:
					key = 7;
					break;
				default:
					throw new Exception();
			}

			bool firstOctaveChange = true;
			while (_currentOctave != requestedOctave)
			{
				if (!firstOctaveChange)
					Thread.Sleep(90);
				
				if (note.Name == NoteName.C && _currentOctave == requestedOctave - 1)
				{
					Thread.Sleep(10);
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

				firstOctaveChange = false;
				
				Thread.Sleep(10);
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