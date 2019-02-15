using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGateIDE
{
    class StatusViewModel : BaseViewModel
    {
        public bool Saved = true;

        private string savedMessage;
        public string SavedMessage
        {
            get { return savedMessage; }
            set
            {
                savedMessage = value;
                RaisePropertyChanged("SavedMessage");
            }
        }

        public StatusViewModel()
        {
            SavedMessage = "";
        }

        public void ChangeFile()
        {
            SavedMessage = "Not Saved";
        }
        public void SaveFile()
        {
            SavedMessage = "Saved";
        }
    }
}
