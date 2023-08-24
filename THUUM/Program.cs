using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Recognition;
using System.Text;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace THUUM
{
    class Program
    {

        public class Magicka
        {
            public static Dictionary<string, string> spellTome { get; } = new Dictionary<string, string>();

            public static string spellToCast { get; set; } = "";



            public static void initMagic()
            {
                string[] lines = File.ReadAllLines("./spells.txt"); //schlurps from the spells.txt to get the proper spells
                foreach (string line in lines)
                {
                    try
                    {
                        spellTome.Add("Cast " + line.Split(',')[0], line.Split(',')[1]);
                        Console.WriteLine("Registered Magic: " + line.Split(',')[0]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            public static void castMagic(string magic)
            {
                try
                {
                    Console.WriteLine(magic + Magicka.spellTome[magic]);
                    Magicka.spellToCast = Magicka.spellTome[magic];
                    Thread recant = new Thread(recantMagic);
                    recant.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                }
            }

            public static void recantMagic() {

                Thread.Sleep(50);
                Magicka.spellToCast = "";
            }

        }
        private static Grammar CreateGrammarFromFile() //gets the grammar for voice recog, like "Cast Glintstone Pebble"
        {
            Grammar spellGrammar = new Grammar("./VCGrammarer.xml");
            spellGrammar.Name = "VC Grammars";
            return spellGrammar;
        }



        public static void voiceRecog() {

            using (
            SpeechRecognitionEngine recognizer =
                new SpeechRecognitionEngine(
                    new System.Globalization.CultureInfo("en-US")))
            {

                // Create and load a dictation grammar.  
                recognizer.LoadGrammar(CreateGrammarFromFile());

                // Add a handler for the speech recognized event.  
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognizedAsync);

                // Configure input to the speech recognizer.  
                recognizer.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.  
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                // Keep the console window open.  
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }
        // Handle the SpeechRecognized event.  
        static void recognizer_SpeechRecognizedAsync(object sender, SpeechRecognizedEventArgs e)
        {
            Magicka.castMagic(e.Result.Text);
        }
        static async void CSWriter()
        {
            

            while (true)
            {

                try
                {
                    Console.WriteLine("Named Pipe Server Thuuminator started...");
                    var server = new NamedPipeServerStream("thuuminator", PipeDirection.Out, 1);
                    Console.WriteLine("Waiting for connection...");

                    await server.WaitForConnectionAsync();
                    Console.WriteLine("Server Online");
                    while (true) {
                        try
                        {

                            StreamWriter writer = new StreamWriter(server); //must run constantly, else elden ring thread stops
                            await writer.WriteLineAsync(Magicka.spellToCast);
                            //Console.WriteLine("Written response: " + Globals.spellToCast);
                            await writer.FlushAsync();
                            //Console.WriteLine("Flushed async");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                            server.Close();
                            server.Dispose(); //in the case that your character reloads, the server will close and dispose
                            break;
                        }
                    }
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }



        }

        public static void Main(string[] args)
        {
            Magicka.initMagic();
            // Create an in-process speech recognizer for the en-US locale.  
            Thread listener = new Thread(voiceRecog);
            listener.Start();
            Thread writer = new Thread(CSWriter);
            writer.Start();

        }

    }
}