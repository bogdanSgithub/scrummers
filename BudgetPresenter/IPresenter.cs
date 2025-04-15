using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace BudgetPresenter
{
    public interface IPresenter
    {
        List<Category> GetCategories();
    }
}
