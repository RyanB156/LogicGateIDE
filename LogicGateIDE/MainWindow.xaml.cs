using ScintillaNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using STYLE = ScintillaNET.Style;
using COLOR = System.Drawing.Color;
using FORMS = System.Windows.Forms;

namespace LogicGateIDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LogicGateSyntaxProvider lgs;

        public MainWindow()
        {
            InitializeComponent();
            
            
            // Set default colors for the text box.
            scintilla.StyleResetDefault();
            scintilla.Styles[STYLE.Default].BackColor = COLOR.FromArgb(255, 30, 30, 30);
            scintilla.Styles[STYLE.Default].Font = "Consolas";
            scintilla.Styles[STYLE.Default].Size = 12;
            scintilla.StyleClearAll();

            // Define custom style colors for the text box.
            scintilla.Styles[LogicGateSyntaxProvider.StyleDefault].ForeColor = COLOR.White;
            scintilla.Styles[LogicGateSyntaxProvider.StyleGate].ForeColor = COLOR.Blue;
            scintilla.Styles[LogicGateSyntaxProvider.StyleIdentifier].ForeColor = COLOR.Orange;
            scintilla.Styles[LogicGateSyntaxProvider.StyleOperator].ForeColor = COLOR.Red;

            scintilla.Styles[LogicGateSyntaxProvider.StyleMargin].ForeColor = COLOR.LightBlue;
            scintilla.Styles[LogicGateSyntaxProvider.StyleMargin].Size = 10;

            scintilla.CaretForeColor = COLOR.White;
            scintilla.Lexer = ScintillaNET.Lexer.Container;            
            
            

            lgs = new LogicGateSyntaxProvider();
            
            fileVM.FileRead += FileVM_FileRead;
        }

        // Text in the Scintilla textbox has been changed. Start styling.
        private void Scintilla_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            var startPos = scintilla.GetEndStyled();
            var endPos = e.Position;

            lgs.Style(scintilla, startPos, endPos);
        }

        private int maxLineNumberCharLength;
        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            statusVM.ChangeFile();

            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            scintilla.Margins[0].Width = scintilla.TextWidth(LogicGateSyntaxProvider.StyleMargin, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        // Check keys being pressed in the textbox. Intercept Ctrl+* shortcuts and call the proper method.
        private void scintilla_KeyDown(object sender, FORMS.KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case FORMS.Keys.O:
                        OpenClicked(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case FORMS.Keys.S:
                        if (e.Shift)
                        {
                            SaveAsClicked(this, new RoutedEventArgs());
                        }
                        else
                        {
                            SaveClicked(this, new RoutedEventArgs());
                        }
                        e.Handled = true;
                        break;
                    case FORMS.Keys.F5:
                        StartClicked(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                }
            }
        }

        // Stop the textbox from displaying garbage symbols.
        private void scintilla_KeyPress(object sender, FORMS.KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                e.Handled = true;
                return;
            }
        }

        // Update the "TextField" with the text from the file loaded in FileViewModel.cs.
        private void FileVM_FileRead(object sender, FileReadEventArgs e)
        {
            scintilla.Text = e.FileText;
        }

        private void OpenClicked(object sender, RoutedEventArgs e)
        {
            Result<bool> result = fileVM.Open();
            if (result.IsSuccess)
            {
                statusVM.ChangeFile();
            }
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            Result<bool> result = fileVM.Save(scintilla.Text);
            if (result.IsSuccess)
            {
                statusVM.SaveFile();
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }

        private void SaveAsClicked(object sender, RoutedEventArgs e)
        {
            Result<bool> result = fileVM.Save(scintilla.Text, true);
            if (result.IsSuccess)
            {
                statusVM.SaveFile();
            }
            else
                MessageBox.Show(result.Message);
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            ApplicationRunner ar = new ApplicationRunner(fileVM.FileName);
            Result<bool> result = ar.Run();

            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message);
            }
        }


        
    }

    // ctrl-[s/a] doesn't add text to scintilla now but the commands for saving, opening, etc. don't work when scintilla has focus now.

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Start =
            new RoutedUICommand("Start", "Start", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F5) });
    }
}
