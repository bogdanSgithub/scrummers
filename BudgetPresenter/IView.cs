using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetPresenter
{
    public interface IView
    {
        public void Alert(string message);
    }
}
