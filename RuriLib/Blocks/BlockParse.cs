﻿using AngleSharp.Parser.Html;
using Extreme.Net;
using Newtonsoft.Json.Linq;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace RuriLib
{
    /// <summary>
    /// The allowed parsing algorithms.
    /// </summary>
    public enum ParseType
    {
        /// <summary>Algorithm that parses text between two strings.</summary>
        LR,

        /// <summary>Algorithm that parses a given attribute from an HTML element identified by a CSS Selector.</summary>
        CSS,

        /// <summary>Algorithm that parses values inside a json object.</summary>
        JSON,

        /// <summary>Algorithm that parses a given attribute from an HTML element identified by xpath.</summary>
        XPATH,

        /// <summary>Algorithm that extracts the text inside matched regex groups.</summary>
        REGEX
    }

    /// <summary>
    /// A block that parses data from a string.
    /// </summary>
    public class BlockParse : BlockBase
    {
        private string parseTarget = "<SOURCE>";
        /// <summary>The string to parse data from.</summary>
        public string ParseTarget { get { return parseTarget; } set { parseTarget = value; OnPropertyChanged(); } }

        private string variableName = "";
        /// <summary>The name of the output variable where the parsed text will be stored.</summary>
        public string VariableName { get { return variableName; } set { variableName = value; OnPropertyChanged(); } }

        private bool isCapture = false;
        /// <summary>Whether the output variable should be marked as Capture.</summary>
        public bool IsCapture { get { return isCapture; } set { isCapture = value; OnPropertyChanged(); } }

        private string prefix = "";
        /// <summary>The string to add to the beginning of the parsed data.</summary>
        public string Prefix { get { return prefix; } set { prefix = value; OnPropertyChanged(); } }

        private string suffix = "";
        /// <summary>The string to add to the end of the parsed data.</summary>
        public string Suffix { get { return suffix; } set { suffix = value; OnPropertyChanged(); } }

        private bool recursive = false;
        /// <summary>Whether to parse multiple values that match the criteria or just the first one.</summary>
        public bool Recursive { get { return recursive; } set { recursive = value; OnPropertyChanged(); } }

        private ParseType type = ParseType.LR;
        /// <summary>The parsing algorithm being used.</summary>
        public ParseType Type { get { return type; } set { type = value; OnPropertyChanged(); } }

        #region LR
        private string leftString = "";
        /// <summary>The string to the immediate left of the text to parse. An empty string means the start of the input.</summary>
        public string LeftString { get { return leftString; } set { leftString = value; OnPropertyChanged(); } }

        private string rightString = "";
        /// <summary>The string to the immediate right of the text to parse. An empty string means the end of the input.</summary>
        public string RightString { get { return rightString; } set { rightString = value; OnPropertyChanged(); } }

        private bool useRegexLR = false;
        /// <summary>Whether to use a regex pattern to match a text between two strings instead of the standard algorithm.</summary>
        public bool UseRegexLR { get { return useRegexLR; } set { useRegexLR = value; OnPropertyChanged(); } }
        #endregion

        #region CSS
        private string cssSelector = "";
        /// <summary>The CSS selector that addresses the desired element in the HTML page.</summary>
        public string CssSelector { get { return cssSelector; } set { cssSelector = value; OnPropertyChanged(); } }

        private string attributeName = "";
        /// <summary>The name of the attribute from which to parse the value.</summary>
        public string AttributeName { get { return attributeName; } set { attributeName = value; OnPropertyChanged(); } }

        private int cssElementIndex = 0;
        /// <summary>The index of the desired element when the selector matches multiple elements.</summary>
        public int CssElementIndex { get { return cssElementIndex; } set { cssElementIndex = value; OnPropertyChanged(); } }
        #endregion

        #region JSON
        private string jsonField = "";
        /// <summary>The name of the json field for which we want to retrieve the value.</summary>
        public string JsonField { get { return jsonField; } set { jsonField = value; OnPropertyChanged(); } }
        #endregion

        #region REGEX
        private string regexString = "";
        /// <summary>The regex pattern that matches parts of the text inside groups.</summary>
        public string RegexString { get { return regexString; } set { regexString = value; OnPropertyChanged(); } }

        private string regexOutput = "";
        /// <summary>The way the content of the matched groups should be formatted. [0] will be replaced with the full match, [1] with the first group etc.</summary>
        public string RegexOutput { get { return regexOutput; } set { regexOutput = value; OnPropertyChanged(); } }
        #endregion

        /// <summary>
        /// Creates a Parse block.
        /// </summary>
        public BlockParse()
        {
            Label = "PARSE";
        }

        /// <inheritdoc />
        public override BlockBase FromLS(string line)
        {
            // Trim the line
            var input = line.Trim();

            // Parse the label
            if (input.StartsWith("#"))
                Label = LineParser.ParseLabel(ref input);

            ParseTarget = LineParser.ParseLiteral(ref input, "TARGET");

            Type = (ParseType)LineParser.ParseEnum(ref input, "TYPE", typeof(ParseType));

            switch (Type)
            {
                case ParseType.LR:
                    // PARSE "<SOURCE>" LR "L" "R" RECURSIVE? -> VAR/CAP "ABC"
                    LeftString = LineParser.ParseLiteral(ref input, "LEFT STRING");
                    RightString = LineParser.ParseLiteral(ref input, "RIGHT STRING");
                    while (LineParser.Lookahead(ref input) == TokenType.Boolean)
                        LineParser.SetBool(ref input, this);
                    break;

                case ParseType.CSS:
                    // PARSE "<SOURCE>" CSS "Selector" "Attribute" Index RECURSIVE? ->
                    CssSelector = LineParser.ParseLiteral(ref input, "SELECTOR");
                    AttributeName = LineParser.ParseLiteral(ref input, "ATTRIBUTE");
                    if (LineParser.Lookahead(ref input) == TokenType.Boolean)
                        LineParser.SetBool(ref input, this);
                    else if (LineParser.Lookahead(ref input) == TokenType.Integer)
                        CssElementIndex = LineParser.ParseInt(ref input, "INDEX");
                    break;

                case ParseType.JSON:
                    // PARSE "<SOURCE>" JSON "Field" ->
                    JsonField = LineParser.ParseLiteral(ref input, "FIELD");
                    if (LineParser.Lookahead(ref input) == TokenType.Boolean)
                        LineParser.SetBool(ref input, this);
                    break;

                case ParseType.REGEX:
                    // PARSE "<SOURCE>" REGEX "Pattern" "Output" RECURSIVE? -> 
                    RegexString = LineParser.ParseLiteral(ref input, "PATTERN");
                    RegexOutput = LineParser.ParseLiteral(ref input, "OUTPUT");
                    if (LineParser.Lookahead(ref input) == TokenType.Boolean)
                        LineParser.SetBool(ref input, this);
                    break;
            }

            // Parse the arrow
            LineParser.ParseToken(ref input, TokenType.Arrow, true);

            // Parse the VAR / CAP
            try
            {
                var varType = LineParser.ParseToken(ref input, TokenType.Parameter, true);
                if (varType.ToUpper() == "VAR" || varType.ToUpper() == "CAP")
                    IsCapture = varType.ToUpper() == "CAP";
            }
            catch { throw new ArgumentException("Invalid or missing variable type"); }

            // Parse the variable/capture name
            try { VariableName = LineParser.ParseLiteral(ref input, "NAME"); }
            catch { throw new ArgumentException("Variable name not specified"); }

            // Parse the prefix and suffix
            try
            {
                Prefix = LineParser.ParseLiteral(ref input, "PREFIX");
                Suffix = LineParser.ParseLiteral(ref input, "SUFFIX");
            }
            catch { }

            return this;
        }

        /// <inheritdoc />
        public override string ToLS(bool indent = true)
        {
            var writer = new BlockWriter(GetType(), indent, Disabled);
            writer
                .Label(Label)
                .Token("PARSE")
                .Literal(ParseTarget)
                .Token(Type);

            switch (Type)
            {
                case ParseType.LR:
                    writer
                        .Literal(LeftString)
                        .Literal(RightString)
                        .Boolean(Recursive, "Recursive")
                        .Boolean(UseRegexLR, "UseRegexLR");
                    break;

                case ParseType.CSS:
                    writer
                        .Literal(CssSelector)
                        .Literal(AttributeName);
                    if (Recursive) writer.Boolean(Recursive, "Recursive");
                    else writer.Integer(CssElementIndex, "CssElementIndex");
                    break;

                case ParseType.JSON:
                    writer
                        .Literal(JsonField)
                        .Boolean(Recursive, "Recursive");
                    break;

                case ParseType.REGEX:
                    writer
                       .Literal(RegexString)
                       .Literal(RegexOutput)
                       .Boolean(Recursive, "Recursive");
                    break;
            }

            writer
                .Arrow()
                .Token(IsCapture ? "CAP" : "VAR")
                .Literal(VariableName);

            if (!writer.CheckDefault(Prefix, "Prefix") || !writer.CheckDefault(Suffix, "Suffix"))
                    writer.Literal(Prefix).Literal(Suffix);

            return writer.ToString();
        }

        /// <inheritdoc />
        public override void Process(BotData data)
        {
            base.Process(data);

            InsertVariables(data, isCapture, recursive, Parse(data), variableName, prefix, suffix);
        }

        private List<string> Parse(BotData data)
        {
            var original = ReplaceValues(parseTarget, data);
            var partial = original;
            var list = new List<string>();

            // Parse the value
            switch (Type)
            {
                case ParseType.LR:
                    var ls = ReplaceValues(leftString, data);
                    var rs = ReplaceValues(rightString, data);
                    var pFrom = 0;
                    var pTo = 0;

                    // No L and R = return full input
                    if (ls == "" && rs == "")
                    {
                        list.Add(original);
                        break;
                    }

                    // L or R not present and not empty
                    else if ( ((!partial.Contains(ls) && ls != "") || (!partial.Contains(rs) && rs != "")))
                    {
                        list.Add("");
                        break;
                    }
                    
                    // Instead of the mess below, we could simply use Extreme.NET's Substring extensions
                    // return original.Substrings(ls, rs); // Recursive
                    // return original.Substring(ls, rs); // Not recursive

                    if (recursive)
                    {
                        if (useRegexLR)
                        {
                            try
                            {
                                var pattern = BuildLRPattern(ls, rs);
                                MatchCollection mc = Regex.Matches(partial, pattern);
                                foreach (Match m in mc)
                                    list.Add(m.Value);
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                while ((partial.Contains(ls) || ls == "") && (partial.Contains(rs) || rs == ""))
                                {
                                    // Search for left delimiter and Calculate offset
                                    pFrom = ls == "" ? 0 : partial.IndexOf(ls) + ls.Length;
                                    // Move right of offset
                                    partial = partial.Substring(pFrom);
                                    // Search for right delimiter and Calculate length to parse
                                    pTo = rs == "" ? partial.Length : partial.IndexOf(rs);
                                    // Parse it
                                    var parsed = partial.Substring(0, pTo);
                                    list.Add(parsed);
                                    // Move right of parsed + right
                                    partial = partial.Substring(parsed.Length + rs.Length);
                                }
                            }
                            catch { }
                        }
                    }

                    // Non-recursive
                    else
                    {
                        if (useRegexLR)
                        {
                            var pattern = BuildLRPattern(ls, rs);
                            MatchCollection mc = Regex.Matches(partial, pattern);
                            if (mc.Count > 0) list.Add(mc[0].Value);
                        }
                        else
                        {
                            try
                            {
                                pFrom = ls == "" ? 0 : partial.IndexOf(ls) + ls.Length;
                                partial = partial.Substring(pFrom);
                                pTo = rs == "" ? (partial.Length - 1) : partial.IndexOf(rs);
                                list.Add(partial.Substring(0, pTo));
                            }
                            catch { }
                        }
                    }

                    break;

                case ParseType.CSS:

                    HtmlParser parser = new HtmlParser();
                    AngleSharp.Dom.Html.IHtmlDocument document = null;
                    try { document = parser.Parse(original); } catch { list.Add(""); }

                    try
                    {
                        if (recursive)
                        {
                            foreach(var element in document.QuerySelectorAll(ReplaceValues(cssSelector,data)))
                            {
                                switch (ReplaceValues(attributeName, data))
                                {
                                    case "innerHTML":
                                        list.Add(element.InnerHtml);
                                        break;
                                    case "outerHTML":
                                        list.Add(element.OuterHtml);
                                        break;
                                    default:
                                        foreach(var attr in element.Attributes)
                                        {
                                            if(attr.Name == ReplaceValues(attributeName,data))
                                            {
                                                list.Add(attr.Value);
                                                break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch (ReplaceValues(attributeName, data))
                            {
                                case "innerHTML":
                                    list.Add(document.QuerySelectorAll(ReplaceValues(cssSelector, data))[cssElementIndex].InnerHtml);
                                    break;

                                case "outerHTML":
                                    list.Add(document.QuerySelectorAll(ReplaceValues(cssSelector, data))[cssElementIndex].OuterHtml);
                                    break;

                                default:
                                    foreach (var attr in document.QuerySelectorAll(ReplaceValues(cssSelector, data))[cssElementIndex].Attributes)
                                    {
                                        if (attr.Name == ReplaceValues(attributeName, data))
                                        {
                                            list.Add(attr.Value);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    catch { list.Add(""); }

                    break;

                case ParseType.JSON:
                    var jsonlist = new List<KeyValuePair<string, string>>();
                    parseJSON("", original, jsonlist);
                    foreach(var j in jsonlist)
                        if (j.Key == ReplaceValues(jsonField, data))
                            list.Add(j.Value);

                    if (list.Count == 0) list.Add("");
                        break;

                case ParseType.XPATH:

                    // NOT IMPLEMENTED YET
                    break;

                case ParseType.REGEX:
                    REGEXBEGIN:
                    try
                    {
                        var matches = Regex.Matches(partial, ReplaceValues(regexString, data));
                        foreach(Match match in matches)
                        {
                            var output = ReplaceValues(regexOutput, data);
                            for (var i = 0; i < match.Groups.Count; i++) output = output.Replace("[" + i + "]", match.Groups[i].Value);
                            list.Add(output);
                            if (recursive && match.Index + match.Length <= partial.Length) { partial = partial.Substring(match.Index + match.Length); goto REGEXBEGIN; }
                        }
                    }
                    catch { }
                    break;
            }

            return list;
        }

        private string BuildLRPattern(string ls, string rs)
        {
            var left = string.IsNullOrEmpty(ls) ? "^" : Regex.Escape(ls); // Empty LEFT = start of the line
            var right = string.IsNullOrEmpty(rs) ? "$" : Regex.Escape(rs); // Empty RIGHT = end of the line
            return "(?<=" + left + ").+?(?=" + right + ")";
        }

        private static void parseJSON(string A, string B, List<KeyValuePair<string, string>> jsonlist)
        {
            jsonlist.Add(new KeyValuePair<string, string>(A, B));

            if (B.StartsWith("["))
            {
                JArray arr = null;
                try { arr = JArray.Parse(B); } catch { return; }

                foreach (var i in arr.Children())
                    parseJSON("", i.ToString(), jsonlist);
            }

            if (B.Contains("{"))
            {
                JObject obj = null;
                try { obj = JObject.Parse(B); } catch { return; }

                foreach (var o in obj)
                    parseJSON(o.Key, o.Value.ToString(), jsonlist);
            }
        }
        
        private string cleanString(string inputString)
        {
            return Regex.Replace(inputString, "<br>", "").Trim();
        }
    }
}
