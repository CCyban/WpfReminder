using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MaterialDesignThemes.Wpf;

namespace WpfReminder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// TTS Vars

		// To Load/Save the TTS data
		private const string DATA_FILENAME = "TTS.dat";
		private Dictionary<string, TTSFileClass> ttsDictionary = new Dictionary<string, TTSFileClass>();
		private BinaryFormatter formatter = new BinaryFormatter();

		// To Play/Pause the TTS audio
		static SpeechSynthesizer ttsSynthesizer = new SpeechSynthesizer();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void TTS_TabItem_Initialized(object sender, EventArgs e)
		{
			LoadTTSFiles();
		}

		private void LoadTTSFiles()	// Loads the .dat file into the ttsDictionary
		{
			TTSFileComboBox.Items.Clear();

			if (File.Exists(DATA_FILENAME))
			{

				try
				{
					// Create a FileStream will gain read access to the 
					// data file.
					FileStream readerFileStream = new FileStream(DATA_FILENAME,
						FileMode.Open, FileAccess.Read);
					// Reconstruct information of our friends from file.
					ttsDictionary = (Dictionary<string, TTSFileClass>)
						formatter.Deserialize(readerFileStream);
					// Close the readerFileStream when we are done
					readerFileStream.Close();

					ttsDictionary.ToList().ForEach(file => TTSFileComboBox.Items.Add(file.Key));
				}
				catch (Exception)
				{
					MessageBox.Show("Unable to load TTS information");
				} // end try-catch

			} // end if
		}

		private void SaveTTSFiles()	// Saves the ttsDictionary into a .dat file
		{
			try
			{
				// Create a FileStream that will write data to file.
				FileStream writerFileStream =
					new FileStream(DATA_FILENAME, FileMode.Create, FileAccess.Write);
				// Save our dictionary of friends to file
				formatter.Serialize(writerFileStream, ttsDictionary);

				// Close the writerFileStream when we are done.
				writerFileStream.Close();
			}
			catch (Exception)
			{
				MessageBox.Show("Unable to save TSS information");
			} // end try-catch

			LoadTTSFiles();
			TTSFileComboBox.SelectedValue = TTSFilenameTextBox.Text;
		}

		private void SaveTTSFile_Button_Click(object sender, RoutedEventArgs e)
		{
			TTSFileClass TTSFile = new TTSFileClass();
			TTSFile.Volume = (int)TTSVolume_Slider.Value;
			TTSFile.Speed = (int)TTSSpeed_Slider.Value;
			TTSFile.Text = TTSText_TextBox.Text;

			if (ttsDictionary.ContainsKey(TTSFilenameTextBox.Text))
				ttsDictionary[TTSFilenameTextBox.Text] = TTSFile;       // Update an existing TTS file
			else
				ttsDictionary.Add(TTSFilenameTextBox.Text, TTSFile);    // Add TTS file

			SaveTTSFiles();
		}

		private void TTSFilenameTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
				TTSFileSaveButton.IsEnabled = false;
			else
				TTSFileSaveButton.IsEnabled = true;
		}

		private void TTSFileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (((ComboBox)sender).SelectedIndex > -1)
			{
				TTSFileClass TTSFile = ttsDictionary.ElementAt(TTSFileComboBox.SelectedIndex).Value;

				TTSVolume_Slider.Value = TTSFile.Volume;
				TTSSpeed_Slider.Value = TTSFile.Speed;
				TTSText_TextBox.Text = TTSFile.Text;

				TTSFilenameTextBox.Text = TTSFileComboBox.SelectedValue.ToString();
			}
		}

		private void TTSPlay_Button_Click(object sender, RoutedEventArgs e)
		{
			if (ttsSynthesizer.State == SynthesizerState.Speaking)
			{
				ttsSynthesizer.SpeakAsyncCancelAll();
				TTSPlay_PackIcon.Kind = PackIconKind.Play;
			}
			else
			{
				ttsSynthesizer.Volume = (int)TTSVolume_Slider.Value;
				ttsSynthesizer.Rate = (int)TTSSpeed_Slider.Value;
				ttsSynthesizer.SpeakAsync(TTSText_TextBox.Text);
				ttsSynthesizer.SpeakCompleted += ttsSynthesizer_SpeakCompleted;
				TTSPlay_PackIcon.Kind = PackIconKind.Pause;
			}

			void ttsSynthesizer_SpeakCompleted(object a, SpeakCompletedEventArgs b)
			{
				TTSPlay_PackIcon.Kind = PackIconKind.Play;
			}

		}

		private void TTSText_TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);	// Sets the cursor to the end of the textbox when clicked on, instead of the start
		}
	}
}
