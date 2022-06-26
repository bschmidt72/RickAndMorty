namespace webapi.Helper
{
    public static class Sanitizer
    {
        public static string CleanString(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, @"[^a-z^0-9^ ^-^_]", "", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

    }
}