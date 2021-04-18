using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanyPieShop.Models
{
    public interface ICategoryRepository
    {
        //return all categories that we have
        IEnumerable<Category> AllCategories { get; }
    }
}
