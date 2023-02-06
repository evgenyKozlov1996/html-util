// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

string html = System.IO.File.ReadAllText(@"D:\Additional Work\textbook\index.html");

string script = @"<script type='text/javascript'>
  var arr = [];
  var currentIndex = 0;

  var lastScrolled = 0;

  var lastFoundText;

  function findAndScrollTo(text) {
    clearSelection();

    lastFoundText = text;
    var ps = document.querySelectorAll('a, p');

    for (const p of ps) {
      if (p.textContent.includes(text)) {
        arr = arr.concat(p);
      }
    }

    // Цвет найденного слова
    if (arr.length > 0) {
      arr.forEach((p) => {
        p.innerHTML = p.innerText.replaceAll(
          text,
          `<span class='search-text' style='background-color: yellow'>${text}</span>`
        );
      });

      currentIndex = 0;
      arr[currentIndex].scrollIntoView(true);
      arr[currentIndex].innerHTML = arr[currentIndex].innerHTML.replaceAll(
        `background-color: yellow`,
        `background-color: orange`
      );
    }
  }

  function getFoundItemsCount() {
    return arr.length;
  }

  function getCurrentIndex() {
    return currentIndex;
  }

  function scrollToNext() {
    currentIndex++;
    if (currentIndex < arr.length) {
      // Clear previous selection
      arr[currentIndex - 1].innerHTML = arr[currentIndex].innerHTML.replaceAll(
        `background-color: orange`,
        `background-color: yellow`
      );
      arr[currentIndex].scrollIntoView(true);
      arr[currentIndex].innerHTML = arr[currentIndex].innerHTML.replaceAll(
        `background-color: yellow`,
        `background-color: orange`
      );
    } else {
      currentIndex--;
    }
  }

  function scrollToPrevious() {
    currentIndex--;
    if (currentIndex >= 0) {
      // Clear previous selection
      arr[currentIndex + 1].innerHTML = arr[currentIndex].innerHTML.replaceAll(
        `background-color: orange`,
        `background-color: yellow`
      );

      arr[currentIndex].scrollIntoView(true);
      // Set current selection if prev item exist
      arr[currentIndex].innerHTML = arr[currentIndex].innerHTML.replaceAll(
        `background-color: yellow`,
        `background-color: orange`
      );
    } else {
      currentIndex++;
    }
  }

  function saveScrollPosition() {
    lastScrolled = window.pageYOffset;
    return lastScrolled;
  }

  function scrollToLastPosition(position) {
    document.documentElement.scrollTop = position;
    document.body.scrollTop = position;
  }

  function clearSelection() {
    arr.forEach((p) => {
      p.innerHTML = p.innerHTML.replaceAll(
        `<span class='search-text' style='background-color: yellow'>${lastFoundText}</span>`,
        lastFoundText
      );
    });

    lastFoundText = '';
    currentIndex = 0;
  }
</script>
";

html = html.Replace("<head>", $"<head>{script}");

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
    html = new StringBuilder(html).Replace($@"font-size:{fontSizeToChange}pt", $"font-size:{oldFonrSizeToNewMap[fontSizeToChange]}pt").ToString();
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


File.WriteAllText(@"D:\Additional Work\textbook\index.html", html);