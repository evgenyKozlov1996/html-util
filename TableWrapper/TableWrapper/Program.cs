// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.RegularExpressions;

string html = System.IO.File.ReadAllText(@"D:\Additional Work\textbook\index.html");

// scrollable tables
html = html.Replace("<table", "<div style=\"overflow-x: auto\"><table");
html = html.Replace("</table>", "</table></div>");

// font-size
var fontSizes = Regex.Matches(html, "font-size:([0-9.]*)pt");
var differentFontSizes = fontSizes.Select(fontSize => fontSize.Groups[1].Value).Distinct().ToList();

Dictionary<string, string> oldFonrSizeToNewMap = new Dictionary<string, string>();
foreach (var fontSize in differentFontSizes)
{
    oldFonrSizeToNewMap.Add(fontSize, (double.Parse(fontSize, CultureInfo.InvariantCulture) + 24).ToString(CultureInfo.InvariantCulture));
}

foreach (var fontSizeToChange in oldFonrSizeToNewMap.Keys)
{
    html = html.Replace($@"font-size:{fontSizeToChange}pt", $"font-size:{oldFonrSizeToNewMap[fontSizeToChange]}pt");
}

// tables all width attributes)
html = Regex.Replace(html, "<td width=[0-9]*", "<td");
html = Regex.Replace(html, "style='width:[0-9.]*pt;", "style='");
html = Regex.Replace(html, @"(<table[0-9a-zA-Z'\s=]*)width=[0-9.]*", "$1");
html = Regex.Replace(html, @"(<td[0-9a-zA-Z\s\.':;=-]*)width=[0-9]*", "$1");

    /*Console.WriteLine(Regex.Match("<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 width=645",
    @"(<table[0-9a-zA-Z'\s=.]*)width=[0-9.]*").Success);

Console.WriteLine(Regex.Replace(
    "<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 width=645",
    @"(<table[0-9a-zA-Z'\s=]*)width=[0-9.]*", "$1"));*/


File.WriteAllText(@"D:\Additional Work\textbook\index.html",html);