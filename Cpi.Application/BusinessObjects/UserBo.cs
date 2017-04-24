using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using Cpi.Application.Helpers;
using System.Linq;

namespace Cpi.Application.BusinessObjects
{
    public class UserBo : BaseBo<UserDm>
    {
        public IQueryable<UserDm> SearchDropDownQuery(string searchString)
        {
            IQueryable<UserDm> query = GetListQuery();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.Nickname.StartsWith(searchString));
            }

            return query.OrderBy(a => (a.Nickname == searchString) ? 0 : 1).ThenBy(a => (a.Nickname.StartsWith(searchString)) ? 0 : 1).Take(ConstHelper.SEARCH_DROP_DOWN_ITEMS);
        }

        public UserDm GetByUsername(string username)
        {
            return GetListQuery().Where(a => a.Username == username).SingleOrDefault();
        }
    }
}
