using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.DBML;

namespace tradelr.Repository
{
    public class EmailRepository
    {
        private readonly tradelrDataContext db;

        public EmailRepository()
        {
            db = new tradelrDataContext();
        }


    }
}
