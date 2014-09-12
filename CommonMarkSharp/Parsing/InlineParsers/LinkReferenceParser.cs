﻿using CommonMarkSharp.Parsing.Blocks;
using CommonMarkSharp.Parsing.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMarkSharp.Parsing.InlineParsers
{
    public class LinkReferenceParser : IParser<LinkReference>
    {
        public LinkReferenceParser(Parsers parsers)
        {
            Parsers = parsers;
        }

        public Parsers Parsers { get; private set; }

        public string StartsWithChars { get { return "["; } }

        public LinkReference Parse(ParserContext context, Subject subject)
        {
            if (!this.CanParse(subject)) return null;

            var savedSubject = subject.Save();

            var label = Parsers.LinkLabelParser.Parse(context, subject);
            if (label != null)
            {
                subject.SkipWhiteSpace();

                LinkLabel referenceLabel;

                if (subject.IsMatch(@"\G\[\]", 0))
                {
                    // This is a collapsed reference link
                    subject.Advance(2);
                    referenceLabel = label;
                }
                else
                {
                    // This is a full reference link or a shortcut reference link
                    referenceLabel = Parsers.LinkLabelParser.Parse(context, subject) ?? label;
                }
                var link = context.Document.FindLink(referenceLabel.Literal);
                if (link != null)
                {
                    return new LinkReference(label, link);
                }
            }

            savedSubject.Restore();
            return null;
        }
    }
}
