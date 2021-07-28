using MyEvernote.DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.MySql
{
    public class RepositoryBase
    {
        protected static DatabaseContext db;
        private static object _lockSync;

        public RepositoryBase()
        {
            db = CreateContext(); 
        }

        private static DatabaseContext CreateContext()
        {
            if(db == null)
            {
                lock(_lockSync)
                {
                    if(db == null)
                    {
                        db = new DatabaseContext();
                    }
                }
            }

            return db;
        }
    }
}
