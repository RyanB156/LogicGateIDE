using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGateIDE
{
    class LogicGateSyntaxProvider
    {
        // TODO: Rework lexing so that it uses FParsec.
        static HashSet<string> gateNames = new HashSet<string>(FCircuitParser.FParser.CircuitTypes.allGateNames);
        static List<string> operators = new List<string> { ":-" };

        public const int StyleDefault = 0;
        public const int StyleGate = 1;
        public const int StyleIdentifier = 2;
        public const int StyleOperator = 3;
        public const int StyleMargin = 4;
        
        private enum LexState { None, Identifier, Operator }

        private static string StripTrailingNums(string str)
        {
            return str.ToList().Where(c => !(c >= '0' && c <= '9')).Aggregate("", (s, c) => s + c);
        }

        public static bool IsGateName(string text)
        {
            string numberStrippedName = text.ToList().Where(c => !(c >= '0' && c <= '9')).Aggregate("", (s, c) => s + c);
            return gateNames.Contains(numberStrippedName);
        }

        public static bool IsOperator(string text)
        {
            return operators.Exists(s => s.Equals(text));
        }

        public void Style(Scintilla scintilla, int start, int end)
        {
            var line = scintilla.LineFromPosition(start);
            start = scintilla.Lines[line].Position;

            int length = 0;
            var state = LexState.None;

            scintilla.StartStyling(start);
            while (start < end)
            {
                char c = (char)scintilla.GetCharAt(start);

                REPROCESS:
                switch (state)
                {
                    case LexState.None:
                        if (Char.IsLetter(c))
                        {
                            state = LexState.Identifier;
                            goto REPROCESS;
                        }
                        else if (c == ':' && (char)scintilla.GetCharAt(start + 1) == '-')
                        {
                            int style = StyleOperator;
                            scintilla.SetStyling(2, style);
                            start++; // Next character is '-' so skip it.
                            length = 0;
                        }
                        else
                        {
                            scintilla.SetStyling(1, StyleDefault);
                        }
                        break; // Break and advance to the next char.

                    // String words.
                    case LexState.Identifier:
                        if (Char.IsLetterOrDigit(c)) // This char is an identifier character. Keep reading.
                        {
                            length++; // Advance to next char and then break.
                        }
                        else // This char is not an identifier character. Stop adding to this identifier and style it.
                        {
                            int style = StyleIdentifier;
                            string identifier = scintilla.GetTextRange(start - length, length);

                            if (gateNames.Contains(StripTrailingNums(identifier)))
                            {
                                style = StyleGate;
                            }

                            scintilla.SetStyling(length, style);
                            length = 0;
                            state = LexState.None;
                            goto REPROCESS;
                        }
                        break;
                }

                start++;
            }


        }
    }
}
