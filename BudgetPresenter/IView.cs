using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetPresenter
{
    public interface IView
    {   
        public IPresenter presenter { get; }
        public void ShowFileSelectWindow();
        public void ShowError(string message);
        public void ShowCompletion(string message);
    }
}
