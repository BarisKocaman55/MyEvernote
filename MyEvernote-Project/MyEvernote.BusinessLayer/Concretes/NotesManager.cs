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
    public class NotesManager : ManagerBase<Note>
    {
        //private Repository<Note> repo_notes = new Repository<Note>();

        //public List<Note> GetAllNotes()
        //{
        //    return repo_notes.List();
        //}

        //public IQueryable<Note> ListQueryable()
        //{
        //    return repo_notes.ListQueryable();
        //}

        //public Note FindNote(int id)
        //{
        //    return repo_notes.Find(x => x.Id == id);
        //}
    }
}
