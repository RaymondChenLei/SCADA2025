using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Interface
{
    public interface IDialogHostAware
    {
        DelegateCommand CancelCommand { get; set; }
        string DialogHostName { get; set; }

        DelegateCommand SaveCommand { get; set; }

        void OnDialogOpend(IDialogParameters parameters);
    }
}