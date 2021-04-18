using BethanyPieShop.Models;
using BethanyPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanyPieShop.Controllers
{
    public class PieController : Controller
    {
        //make your own
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;

        //we inject them so we can use them and they're registered in startup
        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            //to get access our models
            _pieRepository = pieRepository;
            _categoryRepository = categoryRepository;

            //also able to access mockpierepository and mockcategoryrepository due to having them
            //instantiate in .AddScope() in Startup file
        }

        //public ViewResult List()
        //{
        //    PiesListViewModel piesListViewModel = new PiesListViewModel();
        //    piesListViewModel.Pies = _pieRepository.AllPies;

        //    piesListViewModel.CurrentCategory = "Cheese cakes";
        //    return View(piesListViewModel);
        //}

        public IActionResult Details(int id)
        {
            var pie = _pieRepository.GetPieById(id);
            if (pie == null)
            {
                return NotFound();
            }
            return View(pie);
        }

        public ViewResult List(string category)
        {
            IEnumerable<Pie> pies;
            string currentCategory;

            if(string.IsNullOrEmpty(category))
            {
                pies = _pieRepository.AllPies.OrderBy(p => p.PieId);
                currentCategory = "All pies";
            }
            else
            {
                pies = _pieRepository.AllPies.Where(p => p.Category.CategoryName == category)
                    .OrderBy(p => p.PieId);
                currentCategory = _categoryRepository.AllCategories.FirstOrDefault(c=>c.CategoryName == category)?.CategoryName;
            }
            return View(new PiesListViewModel
            {
                Pies = pies,
                CurrentCategory = currentCategory
            }); 
        }

    }
}
