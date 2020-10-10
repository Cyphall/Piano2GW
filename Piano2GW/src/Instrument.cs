using System.Collections.Generic;

namespace Piano2GW
{
	public class Instrument
	{
		public string Name { get; }
		
		public int FirstOctave { get; }
		public int LastOctave { get; }
		public int DefaultOctave { get; }
		
		private Instrument(string name, int firstOctave, int lastOctave, int defaultOctave)
		{
			Name = name;
			FirstOctave = firstOctave;
			LastOctave = lastOctave;
			DefaultOctave = defaultOctave;
		}
		
		public static List<Instrument> Instruments { get; } = new List<Instrument>
		{
			new Instrument("Musical Bass Guitar",    1, 2, 1),
			new Instrument("Musical Harp",           3, 5, 4),
			new Instrument("Musical Lute",           3, 6, 6),
			new Instrument("Magnanimous Choir Bell", 4, 5, 4)
		};
	}
}