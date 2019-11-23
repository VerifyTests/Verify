using System.IO;
using System.Text;

namespace ObjectApproval
{
    public static class LinesScrubber
    {
        public static string RemoveLinesContaining(this string input, params string[] stringToMatch)
        {
            using var reader = new StringReader(input);
            var builder = new StringBuilder();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var lineContains = line.LineContains(stringToMatch);

                if (!lineContains)
                {
                    builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }

        static bool LineContains(this string line, string[] stringToMatch)
        {
            var lineContains = false;
            foreach (var toMatch in stringToMatch)
            {
                if (line.Contains(toMatch))
                {
                    lineContains = true;
                }
            }

            return lineContains;
        }
    }
}