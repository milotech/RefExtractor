using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Html
{
    public class HtmlTagsParser
    {
        private string _html;
        private int _position;

        public HtmlTagsParser(string htmlContent)
        {
            _html = htmlContent;
        }

        public void Reset()
        {
            _position = 0;
        }

        public HtmlTag ReadNext(params string[] tagFilter)
        {
            // приводим фильтры в нижний регистр для дальнейших сравнений
            var lowerFilter = tagFilter == null ? null : tagFilter.Select(t => t.ToLower()).ToArray();

            // ищем тег
            while(MoveTo("<"))            
            {
                // если этот тег - комментарий, пропустим
                if (CheckPos("!--"))
                {
                    MoveTo("-->");
                    continue;
                }

                // если это - закрывающий тег, пропустим
                if(CheckPos('/'))
                {
                    MoveTo(">");
                    continue;
                }

                SkipWhiteSpace();

                // пытаемся распарсить тег
                var tag = ReadTag(lowerFilter);
                if (tag == null)
                    continue;
                                
                return tag;
            }

            return null;
        }

        private bool End()
        {
            return _position >= _html.Length;
        }


        private bool MoveTo(string text, bool skip = true)
        {
            _position = _html.IndexOf(text, _position, StringComparison.InvariantCultureIgnoreCase);
            if (_position < 0)
            {
                _position = _html.Length;
                return false;
            }

            if(skip)
                _position += text.Length;
            return true;
        }

        
        private void SkipWhiteSpace()
        {
            while (!End() && Char.IsWhiteSpace(_html, _position))
                _position++;
        }
        
        private bool CheckPos(string text)
        {
            return (_position + text.Length < _html.Length) && _html.Substring(_position, text.Length).Equals(text, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool CheckPos(char c)
        {
            return _position < _html.Length && _html[_position] == c;
        }

        private HtmlTag ReadTag(params string[] tagFilter)
        {
            string name = ReadTagNameString();
            if (name == null)
                return null;

            bool script = name.ToLower() == "script";

            // если тег не проходит фильтры, пропускаем
            if(tagFilter != null && !tagFilter.Contains(name.ToLower()))
            {
                if (script)
                    SkipScriptContent();

                return null;
            }

            HtmlTag tag = new HtmlTag { Name = name };

            SkipWhiteSpace();

            while (!CheckPos('>') && !CheckPos('/'))
            {
                var attribute = ReadAttribute();

                if (attribute != null)
                    tag.Attributes.Add(attribute);

                SkipWhiteSpace();
            }

            if (script)
                SkipScriptContent();

            return tag;
        }

        private HtmlTagAttribute ReadAttribute()
        {
            string attName = ReadAttNameString();
            if (attName == null)
                return null;

            string attVal = null;
            SkipWhiteSpace();
            if (CheckPos('='))
            {
                _position++;

                SkipWhiteSpace();
                attVal = ReadValString();
            }

            if (attName == null)
                return null;

            return new HtmlTagAttribute(attName, attVal);
        }

        private string ReadTagNameString()
        {
            int startIndex = _position;
            while (!End() && !Char.IsWhiteSpace(_html[_position]) && !CheckPos('>') && !CheckPos('/'))
                _position++;

            if (_position == startIndex)
                return null;

            return _html.Substring(startIndex, _position - startIndex);
        }

        private string ReadAttNameString()
        {
            int startIndex = _position;
            while (!End() && !Char.IsWhiteSpace(_html[_position]) && !CheckPos('>') && !CheckPos('/') && !CheckPos('='))
                _position++;

            if (_position == startIndex)
                return null;

            return _html.Substring(startIndex, _position - startIndex);
        }

        private string ReadValString()
        {
            char quote = _html[_position];
            if (quote == '\'' || quote == '"')
            {
                _position++;

                int startIndex = _position;
                MoveTo(quote.ToString());
                return _html.Substring(startIndex, _position - startIndex - 1);
            }
            else
            {
                int startIndex = _position;
                while (!End() && !Char.IsWhiteSpace(_html, _position) && !CheckPos('>') && !CheckPos('/'))
                    _position++;
                return _html.Substring(startIndex, _position - startIndex);
            }
        } 

        private void SkipScriptContent()
        {
            do
            {
                MoveTo("</");
                SkipWhiteSpace();
            } while (!End() && !CheckPos("script"));                    
        }
    }
}
