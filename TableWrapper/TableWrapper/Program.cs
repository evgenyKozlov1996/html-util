// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

string html = System.IO.File.ReadAllText(@"C:\AdditionalWork\textbook\index.html");

string script = @"<script type='text/javascript'>
  var foundElems = [];
      var currentIndex = 0;

      var lastScrolled = 0;

      var lastFoundText;

      function onlyUnique(value, index, array) {
        return array.indexOf(value) === index;
      }

      function findAndScrollTo(text) {
        clearSelection();

        lastFoundText = text;

        if(!text){
          return;
        }

        // get the body of the document
        var body = document.querySelector(""body"").innerHTML;

        var regEx = new RegExp(text, ""ig"");

        let result,
          indices = [];

        while ((result = regEx.exec(body))) {
          indices.push({ index: result.index, val: result[0] });
        }

        if (indices.length == 0) {
          return;
        }

        // filter needs to remove all undefined values from array
        var foundInText = indices
          .map((charIndexPair, ind) => {
            var startSearchIndex = charIndexPair.index;

            while (
              startSearchIndex >= 0 &&
              body[startSearchIndex] != "">"" &&
              body[startSearchIndex] != ""<""
            ) {
              startSearchIndex--;
            }

            if (startSearchIndex < 0) {
              return;
            }

            if (body[startSearchIndex] == ""<"") {
              return;
            }

            if (body[startSearchIndex] == "">"") {
              return charIndexPair;
            }
          })
          .filter((x) => x);

        if (foundInText.length == 0) {
          return;
        }

        // Replace found in-text elements to <span>s
        foundInText.forEach((foundSubstr, ind) => {
          let replacement = `<span class=""search-text"" style=""background-color: yellow;"">${foundSubstr.val}</span>`;

          body =
            body.slice(
              0,
              foundSubstr.index +
                (replacement.length - foundSubstr.val.length) * ind
            ) +
            replacement +
            body.slice(
              foundSubstr.index +
                foundSubstr.val.length +
                (replacement.length - foundSubstr.val.length) * ind,
              body.length
            );
        });

        document.querySelector(""body"").innerHTML = body;

        // then get all search-text values from document
        foundElems = document.querySelectorAll(""span.search-text"");

        currentIndex = 0;
        foundElems[currentIndex].scrollIntoView(true);
        foundElems[currentIndex].style.backgroundColor = ""orange"";
      }

      function getFoundItemsCount() {
        return foundElems.length;
      }

      function getCurrentIndex() {
        return currentIndex;
      }

      function scrollToNext() {
        currentIndex++;
        if (currentIndex >= foundElems.length) {
          currentIndex--;
          return;
        }

        // Clear previous selection
        foundElems[currentIndex - 1].style.backgroundColor = ""yellow"";
        foundElems[currentIndex].scrollIntoView(true);
        foundElems[currentIndex].style.backgroundColor = ""orange"";
      }

      function scrollToPrevious() {
        currentIndex--;
        if (currentIndex >= 0) {
          // Clear previous selection
          foundElems[currentIndex + 1].style.backgroundColor = ""yellow"";

          foundElems[currentIndex].scrollIntoView(true);
          // Set current selection if prev item exist
          foundElems[currentIndex].style.backgroundColor = ""orange"";
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
        if (!lastFoundText || foundElems.length == 0) {
          return;
        }

        var regEx = new RegExp(lastFoundText, ""ig"");

        let html = document.querySelector(""body"").innerHTML;

        let found = html.match(regEx).filter(onlyUnique);

        found.forEach((val, ind) => {
          var stringToReplace = `<span class=""search-text"" style=""background-color: yellow;"">${val}</span>`;

          html = html.replaceAll(stringToReplace, val);

          html = html.replaceAll(
            `<span class=""search-text"" style=""background-color: orange;"">${val}</span>`,
            val
          );
        });

        document.getElementsByTagName(""body"")[0].innerHTML = html;

        lastFoundText = """";
        currentIndex = 0;
        foundElems = [];
      }
</script>
";
html = html.Replace("<head>",
  $"<head><meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1' />");
html = html.Replace("<head>", $"<head>{script}");

// scrollable tables
html = html.Replace("<table", "<div style=\"overflow-x: auto\"><table");
html = html.Replace("</table>", "</table></div>");

// font-size
/*
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
}*/

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


File.WriteAllText(@"C:\AdditionalWork\textbook\index.html", html);