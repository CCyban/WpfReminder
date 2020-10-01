using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfReminder
{
	[Serializable]
	class TTSFileClass
	{
		public int Volume { get; set; }
		public int Speed { get; set; }
		public string Text { get; set; }
	}
}
