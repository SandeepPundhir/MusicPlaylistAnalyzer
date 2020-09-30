using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicPlaylistAnalyzer
{
    class Program
    {
        private static List<Music> DataList = new List<Music>();
        static void Main(string[] args)
        {
            string txtFilePath = string.Empty;
            string reportFilePath = string.Empty;

            string currentDir = Directory.GetCurrentDirectory();

            // Using Visual Studio Debugger
            if (Debugger.IsAttached)
            {
                txtFilePath = Path.Combine(currentDir, "myMusicPlaylist.txt");
                reportFilePath = Path.Combine(currentDir, "MusicPlaylistReport.txt");
            }
            // Executing from commandline
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("File paths missing.\n Please try this format : CrimeAnalyzer.exe <crime_csv_file_path> <report_file_path>");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    txtFilePath = args[0];
                    reportFilePath = args[1];

                    if (!txtFilePath.Contains("\\"))
                    {
                        txtFilePath = Path.Combine(currentDir, txtFilePath);
                    }

                    if (!reportFilePath.Contains("\\"))
                    {
                        reportFilePath = Path.Combine(currentDir, reportFilePath);
                    }
                }
            }

            if (File.Exists(txtFilePath))
            {
                if (ReadData(txtFilePath))
                {
                    try
                    {
                        var file = File.Create(reportFilePath);
                        file.Close();
                    }
                    catch (Exception fe)
                    {
                        Console.WriteLine($"Unable to create report file at : {reportFilePath}");
                    }
                    WriteReport(reportFilePath);
                }
            }
            else
            {
                Console.Write($"Crime data file does not exist at path: {txtFilePath}");
            }

            Console.ReadLine();
        }

        private static bool ReadData(string filePath)
        {
            Console.WriteLine($"Reading data from file : {filePath}");
            Console.WriteLine($"----------------------");
            try
            {
                int columns = 0;
                string[] DataLines = File.ReadAllLines(filePath);//replace local variable name DataLines with dataLines . local variables always start with small letter
                for (int index = 0; index < DataLines.Length; index++)
                {
                    string crimeDataLine = DataLines[index];
                    string[] data = crimeDataLine.Split('\t');

                    if (index == 0) // Header
                    {
                        columns = data.Length;
                    }
                    else
                    {
                        if (columns != data.Length)
                        {
                            Console.WriteLine($"Row {index} contains {data.Length} values. It should contain {columns}.");
                            return false;
                        }
                        else
                        {
                            try
                            {
                                Music mData = new Music();
                                
                                mData.Name = (data[0]);
                                mData.Artist =(data[1]);
                                mData.Album = (data[2]);
                                mData.Genre = (data[3]);
                                mData.Size = Convert.ToInt32(data[4]);
                                mData.Time = Convert.ToInt32(data[5]);
                                mData.Year = Convert.ToInt32(data[6]);
                                mData.Plays = Convert.ToInt32(data[7]);

                                DataList.Add(mData);
                            }
                            catch (InvalidCastException e)
                            {
                                Console.WriteLine($"Row {index} contains invalid value.");
                                return false;
                            }
                        }
                    }
                }
                Console.WriteLine($"Data read successfully.");
                Console.WriteLine($"----------------------");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in reading data from text file.");
                throw ex;
            }
        }
        private static void WriteReport(string filePath)
        {
            try
            {
                if (DataList != null && DataList.Any())
                {
                    Console.WriteLine($"Generating the report");
                    Console.WriteLine($"----------------------");

                    StringBuilder reportData = new StringBuilder();
                    reportData.Append(Environment.NewLine);
                    reportData.Append("Music Playlist Report");
                    reportData.Append(Environment.NewLine);


                    var songs = from musicData in DataList
                                 where musicData.Plays >= 200
                                 select musicData;

                    reportData.Append(Environment.NewLine);
                    reportData.Append("Songs that received 200 or more plays:");
                    reportData.Append(Environment.NewLine);
                    foreach (var song in songs)
                    {
                        reportData.Append($"{song}");
                        reportData.Append(Environment.NewLine);
                    }
                    reportData.Append(Environment.NewLine);


                    var alternatives = from musicData in DataList
                                 where musicData.Genre=="Alternative"
                                 select musicData;
                   
                    reportData.Append($"Number of Alternative songs: {alternatives.Count()}");
                    reportData.Append(Environment.NewLine);

                    var hps = from musicData in DataList
                              where musicData.Genre == "Hip-Hop/Rap"
                              select musicData;

                    reportData.Append(Environment.NewLine);
                    reportData.Append($"Number of Alternative songs: {hps.Count()}");
                    reportData.Append(Environment.NewLine);

                    var fishbowl_songs = from musicData in DataList
                                where musicData.Album == "Welcome to the Fishbowl"
                                select musicData;

                    reportData.Append(Environment.NewLine);
                    reportData.Append("Songs from the album Welcome to the Fishbowl:");
                    reportData.Append(Environment.NewLine);
                    foreach (var song in fishbowl_songs)
                    {
                        reportData.Append($"{song}");
                        reportData.Append(Environment.NewLine);
                    }
                    reportData.Append(Environment.NewLine);


                    var _songs_before_1970 = from musicData in DataList // local variable _songs_before_1970  rename with songs_before_1970. underscore is not good local variable name
                                         where musicData.Year<1970
                                         select musicData;

                    reportData.Append("Songs from before 1970:");
                    reportData.Append(Environment.NewLine);
                    foreach (var song in _songs_before_1970)
                    {
                        reportData.Append($"{song}");
                        reportData.Append(Environment.NewLine);
                    }
                    reportData.Append(Environment.NewLine);

                    var names = from musicData in DataList
                                             where musicData.Name.Length>85
                                             select musicData.Name;

                    reportData.Append("Song names longer than 85 characters:");
                    reportData.Append(Environment.NewLine);
                    foreach (var song in names)
                    {
                        reportData.Append($"{song}");
                        reportData.Append(Environment.NewLine);
                    }
                    reportData.Append(Environment.NewLine);


                    int longest = DataList.Max(x => x.Time);

                    var longest_song = from musicData in DataList
                                where musicData.Time== longest
                                select musicData;
                    reportData.Append("Longest song: ");
                    reportData.Append(Environment.NewLine);
                    foreach (var song in longest_song)
                    {
                        reportData.Append($"{song}");
                        reportData.Append(Environment.NewLine);
                    }
                    reportData.Append(Environment.NewLine);

                    Console.WriteLine("Writing report to : {0} ", filePath);

                    using (var stream = new StreamWriter(filePath))
                    {
                        stream.Write(reportData.ToString());
                    }
                    Console.WriteLine($"===============================================================.");
                    Console.WriteLine("Report Written Successfully");
                    Console.WriteLine($"===============================================================.");
                    Console.WriteLine(reportData);
                    Console.WriteLine($"---------------------------------------------------------------.");

                }
                else
                {
                    Console.WriteLine($"No data to write.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in writing report file.");
                throw ex;
            }
        }
    }
}
