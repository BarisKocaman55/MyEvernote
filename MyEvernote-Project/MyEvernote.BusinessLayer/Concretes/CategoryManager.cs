using MyEvernote.BusinessLayer.Abstracts;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer.Concretes
{
    public class CategoryManager : ManagerBase<Category>
    {
        public override int Delete(Category category)
        {
            NotesManager noteManager = new NotesManager();
            LikedManager likeManager = new LikedManager();
            CommentManager commentManager = new CommentManager();

            //Kategoriler ile ilişkili notların silinmesi
            foreach(Note note in category.Notes.ToList())
            {
                //Note ile ilişkili like ların silinmesi
                foreach(Liked like in note.Likes.ToList())
                {
                    likeManager.Delete(like);
                }
                
                foreach(Comment comment in note.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }

                noteManager.Delete(note);
            }

            return base.Delete(category);
        }


        //public List<Category> GetCategories()
        //{
        //    return repo_category.List();
        //}

        //public Category GetCategoryById(int id)
        //{
        //    return repo_category.Find(x => x.Id == id);
        //}
    }
}
