using ADF.Entity;
using ADF.IBusiness;
using System.Linq.Expressions;

namespace ADF.Business
{
    public class UserBusiness : BaseBusiness<User>, IUserBusiness
    {
        public User GetUser(int id)
        {
            return GetEntity(id);
        }

        public void AddData(User input)
        {
            Insert(input);
        }

        public void DeleteData(List<int> ids)
        {
            Delete(ids.First().ToString());
        }

        public void DeleteData(Expression<Func<User,bool>> expression)
        {
            Delete(expression);
        }

        public void UpdateData(User input)
        {
            Update(input);
        }
    }
}
