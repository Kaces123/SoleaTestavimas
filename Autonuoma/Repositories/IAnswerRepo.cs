using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;

namespace Org.Ktu.Isk.P175B602.Autonuoma.Repositories
{
    public interface IAnswerRepo
    {
        List<AnswerListVM> List();
        List<AnswerListVM> QuestionAnswers(int id, int n);
        AnswerEditVM Find(int id);
        AnswerListVM FindForDeletion(int id);
        void Update(AnswerEditVM AnswerEvm);
        void Delete(int id);
        void Insert(AnswerEditVM AnswerEvm);
    }
}
