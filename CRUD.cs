using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache_Money_Questions
{
    interface  ICRUD
    {
        int GetMaxQuestionID();
        void UpdateUser(string Username);
        ICollection<UserProgress> GetALL();
        void AddNewQuestion(Question newQuestion);
        void LeaderBoard();
    }
    class UserRepository : ICRUD
    {
        CacheMoneyQuestionsEntities6 entities;

        public UserRepository()
        {
            entities = new CacheMoneyQuestionsEntities6();
        }

        public void AddNewQuestion(Question newQuestion)
        {

            entities.Questions.Add(newQuestion);
            entities.SaveChanges();
        }

        public ICollection<UserProgress> GetALL()
        {
            return (entities.UserProgresses.ToList());
        }

        public int GetMaxQuestionID()
        {
            return (entities.Questions.Max(p => p.QuestionID));
        }

        public void LeaderBoard()
        {
            var leaders = entities.UserProgresses.ToList();
            leaders.Sort();
        }

        public void UpdateUser(string Username)
        {
            try
            {
                var userToUpdate = entities.UserProgresses.FirstOrDefault(u => u.Username == Username);


                if (userToUpdate == null)
                {
                    // Create a new user if none exists
                    var userProgress = new UserProgress()
                    {

                        Username = Username,
                        TotalAsked = 1,
                    };
                    entities.UserProgresses.Add(userProgress);
                }
                else
                {
                    // Update existing user
                    userToUpdate.TotalAsked++;
                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user progress: " + ex.Message);
            }

        }

    }
}
