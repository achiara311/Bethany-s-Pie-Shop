using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanyPieShop.Models
{
    public class PieRepository:IPieRepository 
    {
        //make as readonly
        private readonly AppDbContext _appDbContext;
        public PieRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            //PieRepository will us the DbContext for persisting
            //and reading data  from database
        }

        public IEnumerable<Pie> AllPies
        {
            get
            {
                //include category of each pie
                return _appDbContext.Pies.Include(category=>category.Category); //DbSet Pies specifically
                //going to send off a sql query that will read out all the pies
            }
        }

        public IEnumerable<Pie> PiesOfTheWeek
        {
            get
            {
                return _appDbContext.Pies.Include(category => category.Category)
                    .Where(pie=>pie.IsPieOfTheWeek);
            }
        }

        public Pie GetPieById(int pieId)
        {
            return _appDbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
        }
    }
}
