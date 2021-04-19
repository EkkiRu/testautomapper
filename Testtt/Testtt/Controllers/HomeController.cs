using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Testtt.Models;

namespace Testtt.Controllers
{
    public class HomeController : Controller
    {

        IRepository<User> repo;
        public HomeController()
        {
            repo = new UserRepository();
        }
        public ActionResult Index()
        {
            // Создание конфигурации сопоставления
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, IndexUserViewModel>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
            var users = mapper.Map<List<IndexUserViewModel>>(repo.GetAll());
            return View(users);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Настройка конфигурации AutoMapper
                var config = new MapperConfiguration(cfg => cfg.CreateMap<CreateUserViewModel, User>()
                    .ForMember("Name", opt => opt.MapFrom(c => c.FirstName + " " + c.LastName))
                    .ForMember("Email", opt => opt.MapFrom(src => src.Login)));
                var mapper = new Mapper(config);
                // Выполняем сопоставление
                User user = mapper.Map<CreateUserViewModel, User>(model);
                repo.Create(user);
                repo.Save();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            // Настройка конфигурации AutoMapper
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, EditUserViewModel>()
                    .ForMember("Login", opt => opt.MapFrom(src => src.Email)));
            var mapper = new Mapper(config);
            // Выполняем сопоставление
            EditUserViewModel user = mapper.Map<User, EditUserViewModel>(repo.Get(id.Value));
            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Настройка конфигурации AutoMapper
                var config = new MapperConfiguration(cfg => cfg.CreateMap<EditUserViewModel, User>()
                    .ForMember("Email", opt => opt.MapFrom(src => src.Login)));
                var mapper = new Mapper(config);
                // Выполняем сопоставление
                User user = mapper.Map<EditUserViewModel, User>(model);
                repo.Update(user);
                repo.Save();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
