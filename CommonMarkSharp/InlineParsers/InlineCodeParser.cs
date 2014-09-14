﻿using CommonMarkSharp.Inlines;
using System.Linq;

namespace CommonMarkSharp.InlineParsers
{
    public class InlineCodeParser : IInlineParser<InlineCode>
    {
        public string StartsWithChars { get { return "`"; } }

        public bool CanParse(Subject subject)
        {
            return subject.Char == '`';
        }

        public InlineCode Parse(ParserContext context, Subject subject)
        {
            if (!CanParse(subject)) return null;

            var saved = subject.Save();
            var openticks = subject.TakeWhile(c => c == '`').ToArray();
            var code = "";
            var codeEnded = false;
            while (!subject.EndOfString && !codeEnded)
            {
                if (subject.Char == '`')
                {
                    var closeticks = subject.TakeWhile(c => c == '`').ToArray();
                    if (closeticks.Length == openticks.Length)
                    {
                        codeEnded = true;
                    }
                    else
                    {
                        code += new string(closeticks);
                    }
                }
                else
                {
                    code += new string(subject.TakeWhile(c => c != '`').ToArray());
                }
            }
            if (codeEnded)
            {
                return new InlineCode(RegexUtils.NormalizeWhitespace(code));
            }

            saved.Restore();
            return null;
        }
    }
}
