using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Launchbeat_Console
{
    class analyzer
    {
        public void changeFile(string s)
        {
            Encoding e = Encoding.Default;
            var lines = File.ReadAllLines(s, Encoding.Default);
            for(int i = 9; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace('口', 'O').Replace('－', '-').Replace(" ", string.Empty);
            }
            File.WriteAllLines(s, lines, Encoding.Default);
        }

        public void readBar(string s)
        {
            var lines = File.ReadAllLines(s, Encoding.Default).ToArray();
            int linesLength = lines.Count();
            int currentBar = 0;

            int NowBar = 0;

            string orderData = string.Empty;
            string beatData = string.Empty;

            for (int i = 0; i < linesLength; i++)
            {
                string line = lines[i];

                if (line.Count() == 0)
                    continue;
                switch (i)
                {
                    case 0:
                        //HitObject.Title = line;
                        break;
                    case 1:
                        //HitObject.Artist = line;
                        break;
                    case 3:
                        //HitObject.Difficulty = line;
                        break;
                    case 5:
                        //HitObject.Level = int.Parse(line.Split(':')[1]);
                        break;
                    case 6:
                        HitObject.BPM = int.Parse(line.Split(':')[1]);
                        break;
                    case 7:
                        //HitObject.NoteCount = int.Parse(line.Split(':')[1]);
                        break;
                    default:
                        if (int.TryParse(line, out currentBar))
                        {
                            Console.WriteLine(orderData);
                            Console.WriteLine(beatData);
                            AnalyzeBeat(orderData.Replace(" ", string.Empty), beatData);

                            Console.WriteLine();

                            Console.WriteLine(currentBar);

                            orderData = string.Empty;
                            beatData = string.Empty;

                            NowBar++;
                        }
                        else
                        {
                            bool isCheckingBeat = false;
                            foreach (char data in line)
                            {
                                if (data == '|')
                                {
                                    if (isCheckingBeat)
                                    {
                                        isCheckingBeat = false;
                                        beatData += '/';
                                    }
                                    else
                                        isCheckingBeat = true;

                                    continue;
                                }

                                if (isCheckingBeat)
                                {
                                    beatData += data;
                                }
                                else
                                {
                                    orderData += data;
                                }
                            }
                        }
                        break;
                }
            }

            void AnalyzeBeat(string orderText, string beatText)
            {
                if (beatText.Length < 1)
                    return;

                string[] beats = beatText.Split('/');

                int order = 0;
                int beatSplit = 4;
                int nowLine = 1;
                for (int i = 0; i < 4; i++)
                {
                    int nowBeat = 1;
                    beatSplit = beats[i].Length;
                    foreach (char beat in beats[i])
                    {
                        if (beat == '-')
                        {
                            nowBeat++;
                            continue;
                        }

                        List<int> keyList = new List<int>();
                        int count = 1;
                        foreach ( char c in orderText)
                        {
                            if (c == (char)(9312 + order))
                            {
                                keyList.Add(count);
                            }
                            if (++count > 16)
                            {
                                count = 0;
                            }
                        }
                        
                        double calcBar = 60000f * 4 * (NowBar - 1) / (double)HitObject.BPM;
                        double calcLine = 60000f * (nowLine - 1) / (double)HitObject.BPM;
                        double calcBeat = 60000f * (nowBeat - 1) / (double)beatSplit / (double)HitObject.BPM;

                        System.Diagnostics.Debug.WriteLine(string.Join(", ", keyList));
                        HitObject.HitObjects.Enqueue(new HitObject(keyList.ToArray(), (calcBar + calcLine + calcBeat)));
                        order++;
                    }
                    nowLine++;
                }
            }
        }
    }
}
