using System.Linq;

namespace NavProps
{
    public static class StringExtensions
    {
        public static string LowerFirstLetter(this string value)
        {
            var firstLetter = value[0].ToString();
            var rest = value.Substring(1);
            return firstLetter.ToLowerInvariant() + rest;
        }

        public static string Indent(this string value, int size)
        {
            var fullIndent = Enumerable.Range(0, size).Aggregate("", (current, _) => current + Constants.Indent)!;
            return fullIndent + value.Replace(Constants.NewLine, Constants.NewLine + Constants.Indent).TrimEnd();
        }
    }
}
