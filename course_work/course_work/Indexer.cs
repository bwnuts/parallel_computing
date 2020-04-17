using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.SqlServer.Server;

namespace course_work
{
    public static class Indexer
    {
        private static bool CheckFile(string file)
        {
            int lowerBound = 4500, upperBound = 4750;
            if (file.Contains("unsup"))
            {
                lowerBound = 18000;
                upperBound = 19000;
            }

            string[] splittedName = file.Split('/');
            int name = Convert.ToInt32(splittedName[splittedName.Length - 1].Split('_')[0]);
            if (name >= lowerBound && name <= upperBound)
            {
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetFiles()
        {
            return from file in Directory.EnumerateFiles("../../../../files/", "*.txt"
                    , SearchOption.AllDirectories)
                where CheckFile(file)
                select file;
        }

        private static void ReadFilesThreadFunc(ref Dictionary<string, string[]> fileText, string[] files
            , int threadId, int threadNumber)
        {
            for (int i = threadId; i < files.Length; i += threadNumber)
            {
                fileText.Add(files[i], File.ReadAllText(files[i]).Split(' '));
            }

        }

        private static Dictionary<string, string[]>[] ReadFiles(int threadNumber)
        {
            Dictionary<string, string[]>[] filesTexts = new Dictionary<string, string[]>[threadNumber];
            for (int i = 0; i < filesTexts.Length; i++)
            {
                filesTexts[i] = new Dictionary<string, string[]>();
            }

            string[] files = GetFiles().ToArray();

            Thread[] threadArray = new Thread[threadNumber];
            for (int i = 0; i < threadArray.Length; i++)
            {
                int temp = i;
                threadArray[i] = new Thread(
                    () => ReadFilesThreadFunc(ref filesTexts[temp], files, temp, threadNumber));
                threadArray[i].Start();
            }

            for (int i = 0; i < threadArray.Length; i++)
            {
                threadArray[i].Join();
            }

            return filesTexts;
        }

        private static string ParseWord(string word)
        {
            string result = "";

            for (int i = 0; i < word.Length; i++)
            {
                if (Char.IsLetter(word[i]) || word[i] == 39)
                {
                    result += word[i];
                }
            }

            return result;
        }

        private static void GetIndexThreadFunc(ref Dictionary<string, List<string>> index,
            Dictionary<string, string[]> files)
        {
            foreach (var file in files)
            {
                foreach (var word in file.Value)
                {
                    string parsedWord = ParseWord(word);
                    if (index.ContainsKey(parsedWord))
                    {
                        index[parsedWord].Add(file.Key);
                    }
                    else
                    {
                        index.Add(parsedWord, new List<string> {file.Key});
                    }
                }
            }
        }



        public static Dictionary<string, List<string>>[] GetIndex(int threadNumber)
        {
            Dictionary<string, List<string>>[] index = new Dictionary<string, List<string>>[threadNumber];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = new Dictionary<string, List<string>>();
            }

            Dictionary<string, string[]>[] files = ReadFiles(threadNumber);

            Thread[] threadArray = new Thread[threadNumber];
            for (int i = 0; i < threadArray.Length; i++)
            {
                int temp = i;
                threadArray[i] = new Thread(
                    () => GetIndexThreadFunc(ref index[temp], files[temp]));
                threadArray[i].Start();
            }

            for (int i = 0; i < threadArray.Length; i++)
            {
                threadArray[i].Join();
            }

            return index;
        }
    }
}