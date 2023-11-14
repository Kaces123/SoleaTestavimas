using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;

namespace Org.Ktu.Isk.P175B602.Autonuoma.Repositories
{
    public interface ILikedRepo
    {
        List<Liked> List();
		Liked Find(int id, int idUser);
        Liked Find(int id, int idUser, int n);
        void Update(int QuestionId, int AnswerId, int UserId, int Id, int likedOrDisliked);
        void Insert(int QuestionId, int AnswerId, int UserId, int Id, int likedOrDisliked);
        void Delete(int id);
    }
}
