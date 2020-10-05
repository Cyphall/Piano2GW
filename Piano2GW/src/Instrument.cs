using System.Collections.Generic;

namespace Piano2GW
{
	public class Instrument
	{
		public int FirstOctave { get; }
		public int LastOctave { get; }
		public int DefaultOctave { get; }
		
		private Instrument(int firstOctave, int lastOctave, int defaultOctave)
		{
			FirstOctave = firstOctave;
			LastOctave = lastOctave;
			DefaultOctave = defaultOctave;
		}
		
		public static Dictionary<string, Instrument> Instruments { get; } = new Dictionary<string, Instrument>
		{
			{"Musical Bass Guitar",    new Instrument(1, 2, 1)},
			{"Musical Harp",           new Instrument(3, 5, 4)},
			{"Musical Lute",           new Instrument(3, 5, 4)},
			{"Magnanimous Choir Bell", new Instrument(4, 5, 4)}
		};
	}
}