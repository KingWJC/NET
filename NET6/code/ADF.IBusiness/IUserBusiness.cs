using ADF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ADF.IBusiness
{
    public interface IUserBusiness
    {
        void AddData(User input);
        void UpdateData(User input);
        void DeleteData(List<int> ids);
        User GetUser(int id);
        void DeleteData(Expression<Func<User, bool>> expression);
    }
}
