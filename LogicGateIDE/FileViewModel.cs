using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LogicGateIDE
{
    public class FileReadEventArgs : EventArgs
    {
        public string FileText { get; private set; }

        public FileReadEventArgs(string text) { FileText = text; }
    }

    class FileViewModel : BaseViewModel
    {
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                RaisePropertyChanged("FileName");
                Console.WriteLine("FileName: {0}", fileName);
            }
        }

        public FileViewModel()
        {
            fileName = null;
        }

        // Custom event for reading files and passing on the text.
        public delegate void FileReadHandler(object sender, FileReadEventArgs e);
        public event FileReadHandler FileRead;

        private void RaiseFileReadEvent(string text)
        {
            FileRead?.Invoke(this, new FileReadEventArgs(text));
        }

        // Read text from the specified file. Triggers the "FileRead" event to send the text to MainWindow.xaml.cs to update the richtextbox.
        private void ReadFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    RaiseFileReadEvent(sr.ReadToEnd());
                }
                catch (IOException) { }
            }
        }

        // Open Button. Open Windows Explorer dialog to get a file.
        // Calls "ReadFile" to open the file and read all text and sets "FileName" with the selected file name.
        public Result<bool> Open()
        {
            using (var fd = new CommonOpenFileDialog() { InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString() })
            {
                switch (fd.ShowDialog())
                {
                    case CommonFileDialogResult.Ok:
                        FileName = fd.FileName;
                        ReadFile(fd.FileName);
                        Console.WriteLine("Opening file:{0}", FileName);
                        return Result<bool>.Success(true);
                }
                return Result<bool>.Failure("No file opened");
            }
        }

        // Return path to write files to.
        private string GetSavePath()
        {
            using (var fd = new CommonSaveFileDialog() { InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString() })
            {
                switch (fd.ShowDialog())
                {
                    case CommonFileDialogResult.Ok:
                        return fd.FileName;
                }
            }
            return null;
        }

        // Save to a file. This method defaults to "Save" where the text is written to "FileName".
        // Passing "saveAs" as true sets this to "SaveAs" where the text is written to the file from a save file dialog.
        public Result<bool> Save(string text, bool saveAs=false)
        {
            string path = saveAs ? GetSavePath() : FileName; // If the operation is "save as", then the path is set to the new path.

            if (!saveAs && FileName == null) // FileName defaults to null.
            {
                return Result<bool>.Failure("You must pick a save path!");
            }
            else
            {
                if (path != null)
                {
                    FileName = path;
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            Console.WriteLine("Saving {0}to file:{1}", saveAs ? "As " : "", path);
                            sw.Write(text);
                            return Result<bool>.Success(true);
                        }
                    }
                    catch (IOException)
                    {
                        throw new ArgumentException(String.Format("{0} is not a valid file", path));
                    }
                }
                return Result<bool>.Failure("Not Saved");
            }
        }
    }
}
