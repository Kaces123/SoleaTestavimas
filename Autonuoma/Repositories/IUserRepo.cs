using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using System.Collections.Generic;

namespace Org.Ktu.Isk.P175B602.Autonuoma.Repositories
{
    public interface IUserRepo
    {
        List<User> List();
        User Find(int id);
        User Find(User user);
        User Find(string email);
        User Find(string user, int n);
        void Update(User user);
        void Insert(User user);
        void Delete(int id);
    }
}
