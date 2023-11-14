using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;

namespace Org.Ktu.Isk.P175B602.Autonuoma.Repositories
{
    public interface IQuestionRepo
    {
        List<QuestionListVM> List(int n);
        QuestionEditVM Find(int id);
        List<QuestionListVM> FindList(string search);
        QuestionListVM FindForDeletion(int id);
        void Update(QuestionEditVM QuestionEvm);
        void Insert(QuestionEditVM QuestionEvm);
        void Delete(int id);
    }
}
