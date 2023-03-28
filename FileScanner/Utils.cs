using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileScanner
{
    class Utils
    {
        public static async Task<DataTable> Search(string path, string pattern, IProgress<string> onProgressChanged)
        {
            IEnumerable<string> allFilesInAllFolders = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
            Regex finder = new(@pattern);

            List<string> files = new List<string>();
            List<Task<MatchCollection>> tasks = new List<Task<MatchCollection>>();

            int filesCount = allFilesInAllFolders.Count();
            int currentFilesReaded = 0;

            foreach (var file in allFilesInAllFolders)
            {
                var progress = new Progress<int>((i) => onProgressChanged.Report(++currentFilesReaded + "/" + filesCount));
                files.Add(file);
                tasks.Add(MatchFile(file, finder, progress));
            }

            DataTable table = new();
            table.Columns.Add("File", typeof(string));
            table.Columns.Add("Match", typeof(string));

            foreach (var (c, i) in (await Task.WhenAll(tasks)).Select((value, i) => (value, i)))
            {
                DataRow row = table.NewRow();
                row[0] = files[i];
                row[1] = "";
                table.Rows.Add(row);

                foreach (Match m in c)
                {
                    DataRow innerRow = table.NewRow();
                    innerRow[0] = "";
                    innerRow[1] = m.ToString();
                    table.Rows.Add(innerRow);
                }
            }

            return table;
        }
        private static async Task<MatchCollection> MatchFile(string filepath, Regex r, IProgress<int> onProgressChanged)
        {
            string text = await File.ReadAllTextAsync(filepath);
            MatchCollection matches = await FindByRegex(r, text);

            Random random = new Random();
            int mseconds = random.Next(3, 10) * 1000;
            await Task.Delay(mseconds);

            onProgressChanged.Report(1);
            return matches;
        }

        private static async Task<MatchCollection> FindByRegex(Regex r, string text)
        {
            return await Task.Run(() => r.Matches(text));
        }
    }
}
