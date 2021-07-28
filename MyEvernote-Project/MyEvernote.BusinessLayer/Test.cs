using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class Test
    {
        private Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        private Repository<Category> repo_category = new Repository<Category>();
        private Repository<Comment> repo_comment = new Repository<Comment>();
        private Repository<Note> repo_note = new Repository<Note>();
        public Test()
        {
            repo_category.List();
        }

        public void InserTest()
        {
            int result = repo_user.Insert(new EvernoteUser()
            {
                Name = "aaa",
                Surname = "bbb",
                Email = "dsadsa@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "aabb",
                Password = "111",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "aabb"
            });
        }

        public void UpdateTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "aabb");
            if(user != null)
            {
                user.Username = "xxx";
                int result = repo_user.Update(user);
            }
        }

        public void DeleteTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "aabb");
            if (user != null)
            {
                int result = repo_user.Delete(user);
            }
        }


        public void InsertNote()
        {
            EvernoteUser user = repo_user.Find(x => x.Id == 1);
            Note note = new Note()
            {
                Title = "sfdsfds",
                Text = "dsadas",
                IsDraft = false,
                LikeCount = 20,
                Owner = user,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddDays(3),
                ModifiedUsername = user.Username
            };
        }


        public void CommnetTest()
        {
            EvernoteUser owner = repo_user.Find(x => x.Id == 1);
            Note note = repo_note.Find(x => x.Id == 1);

            Comment comment = new Comment()
            {
                Text = "This is a test comment 2!!!",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "bariskocaman",
                Note = note,
                Owner = owner
            };

            repo_comment.Insert(comment);
        }

    }
}
