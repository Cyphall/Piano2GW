// ReSharper disable InconsistentNaming
namespace Piano2GW
{
	public struct Note
	{
		public int Octave;
		public NoteName Name;

		public Note(int value)
		{
			Octave = (value - 12) / 12;
			Name = (NoteName)(value % 12);
		}

		public bool IsSharp()
		{
			switch (Name)
			{
				case NoteName.CS:
				case NoteName.DS:
				case NoteName.FS:
				case NoteName.GS:
				case NoteName.AS:
					return true;
				default:
					return false;
			}
		}
	}

	public enum NoteName
	{
		C,
		CS,
		D,
		DS,
		E,
		F,
		FS,
		G,
		GS,
		A,
		AS,
		B,
	}
}