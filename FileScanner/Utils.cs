using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileScanner
{
    class Utils
    {
        public static async void Search(string path, string pattern)
        {
            IEnumerable<string> allFilesInAllFolders = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
            Regex finder = new(@pattern);

            List<string> files = new List<string>();
            List<Task<string>> tasks = new List<Task<string>>();

            foreach (var file in allFilesInAllFolders)
            {

                files.Add(file);
                tasks.Add(File.ReadAllTextAsync(file));
            }

            string[] resultedTasks = await Task.WhenAll(tasks);

            List<Task<MatchCollection>> matchTasks = new();

            foreach (var text in resultedTasks)
            {
                matchTasks.Add(FindByRegex(finder, text));
            }

            MatchCollection[] matchResultedTasks = await Task.WhenAll(matchTasks);

            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine(files[i]);

                foreach (Match m in matchResultedTasks[i].Cast<Match>())
                {
                    Console.WriteLine("\t" + m.ToString());
                }
            }
        }

        private static async Task<MatchCollection> FindByRegex(Regex r, string text)
        {
            return await Task.Run(() => r.Matches(text));
        }
    }
}
