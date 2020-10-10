using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Devices.Infos;
using RtMidi.Core.Messages;

namespace Piano2GW.View
{
	public partial class MainWindow : Window
	{
		#region Running
		public static readonly DependencyProperty RunningProperty = DependencyProperty.Register(
			"Running", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

		public bool Running
		{
			get => (bool)GetValue(RunningProperty);
			set
			{
				if (value)
				{
					NotePlayer.ChangeInstrument((Instrument)InstrumentComboBox.SelectedItem);

					_device = ((IMidiInputDeviceInfo)DevicesComboBox.SelectedItem).CreateDevice();

					_device.NoteOn += (IMidiInputDevice _, in NoteOnMessage msg) => {
						if (msg.Velocity != 0)
							NotePlayer.PlayNote((int)msg.Key);
					};
					_device.Open();

					StateText.Text = "Running";
					ToggleRunning.Content = "Stop";
				}
				else
				{
					_device?.Dispose();
					_device = null;
					StateText.Text = "Stopped";
					ToggleRunning.Content = "Start";
				}
				
				SetValue(RunningProperty, value);
			}
		}

		#endregion

		private DispatcherTimer _deviceScanTimer = new DispatcherTimer();

		private static IMidiInputDevice _device;

		private bool _listenToDeviceChanged = true;
		
		public MainWindow()
		{
			InitializeComponent();

			RescanMidiDevices();

			_deviceScanTimer.Tick += (sender, e) => RescanMidiDevices();
			_deviceScanTimer.Interval = new TimeSpan(0, 0, 1);
			_deviceScanTimer.Start();

			InstrumentComboBox.ItemsSource = Instrument.Instruments;
			InstrumentComboBox.SelectedIndex = 0;
		}

		private void RescanMidiDevices()
		{
			_listenToDeviceChanged = false;
			
			string curDeviceName = ((IMidiInputDeviceInfo)DevicesComboBox.SelectedItem)?.Name;

			DevicesComboBox.ItemsSource = MidiDeviceManager.Default.InputDevices;

			if (DevicesComboBox.Items.Count != 0)
			{
				int curDeviceIndex = 0;
				
				if (curDeviceName == null)
				{
					curDeviceIndex = 0;
				}
				else
				{
					for (int i = 0; i < DevicesComboBox.Items.Count; i++)
					{
						if (((IMidiInputDeviceInfo)DevicesComboBox.Items[i]).Name == curDeviceName)
						{
							curDeviceIndex = i;
							break;
						}
					}
				}
				
				DevicesComboBox.SelectedIndex = curDeviceIndex;
			}
			
			Update();
			_listenToDeviceChanged = true;
		}

		private void ToggleRunning_OnClick(object sender, RoutedEventArgs e)
		{
			Running = !Running;
			Update();
		}

		private void Update()
		{
			if (DevicesComboBox.Items.Count == 0)
			{
				Running = false;
				
				DevicesComboBox.IsEnabled = false;
				ToggleRunning.IsEnabled = false;
			}
			else
			{
				DevicesComboBox.IsEnabled = true;
				ToggleRunning.IsEnabled = true;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			_device?.Dispose();
		}

		private void OnDeviceChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_listenToDeviceChanged) return;
			Running = false;
			Update();
		}
		
		private void OnInstrumentChanged(object sender, SelectionChangedEventArgs e)
		{
			Running = false;
			Update();
		}
	}
}