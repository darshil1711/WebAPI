using CommonModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using NuGet.Packaging;

namespace EmployeeManagment.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClient _httpClient;
        public IConfiguration configuration;
        private readonly TokenAccess tokenAccess;


        public EmployeeController(IConfiguration configuration,TokenAccess tokenAccess, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient("API");
            this.configuration = configuration;
            this.tokenAccess = tokenAccess;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<EmployeeDetailsViewModel> response = new List<EmployeeDetailsViewModel>();

                // Get All EmployeeDetail from WebApi

                var client = httpClientFactory.CreateClient();
                var httpResponseMessage = await _httpClient.GetAsync("all");

                if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login");
                }

                httpResponseMessage.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<EmployeeDetailsViewModel>>());

                return View(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Error retrieving employee details: " + ex.Message);
            }
        }



        [HttpGet]
        public IActionResult AddUsers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddUsers(UsersViewModel user)
        {
            try
            {
                HttpResponseMessage response = _httpClient.PostAsJsonAsync("add-user", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User created successfully.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create user. Please try again.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Failed to create user. Error: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            try
            {
                HttpResponseMessage response = _httpClient.PostAsJsonAsync("login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content.ReadAsStringAsync().Result;

                    // Store the token in session
                    HttpContext.Session.SetString("JWToken", token);

                    TempData["Success"] = "Login successful.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Failed to login. Error: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DesignationList = new SelectList(Designation(), "Id", "Designation");
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeDetailsViewModel employee)
        {
            if (employee.ProfilePictures != null)
            {
                string filename = Path.GetFileNameWithoutExtension(employee.ProfilePictures.FileName) + DateTime.Now.ToString("ddMMyyyyhhmmssfff") + Path.GetExtension(employee.ProfilePictures.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    employee.ProfilePictures.CopyTo(stream);
                }
                employee.ProfilePicture = filename;
            }

            employee.ProfilePictures = null;
            HttpResponseMessage response = _httpClient.PostAsJsonAsync("add", employee).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                return View(employee);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage response = _httpClient.GetAsync($"get/{id}").Result;
            ViewBag.DesignationList = new SelectList(Designation(), "Id", "Designation");

            if (response.IsSuccessStatusCode)
            {
                var employee = response.Content.ReadAsAsync<EmployeeDetailsViewModel>().Result;
                return View(employee);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact the administrator.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, EmployeeDetailsViewModel employee)
        {
            if (employee.ProfilePictures != null)
            {
                string filename = Path.GetFileNameWithoutExtension(employee.ProfilePictures.FileName) + DateTime.Now.ToString("ddMMyyyyhhmmssfff") + Path.GetExtension(employee.ProfilePictures.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    employee.ProfilePictures.CopyTo(stream);
                }
                employee.ProfilePicture = filename;
            }

            employee.ProfilePictures = null;
            HttpResponseMessage response = _httpClient.PutAsJsonAsync($"update/{id}", employee).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact the administrator.");
                return View(employee);
            }
        }

        public IList<EmployeeDesignationViewModel> Designation()
        {
            HttpResponseMessage response = _httpClient.GetAsync("Designation").Result;
            var designationList = response.Content.ReadAsAsync<List<EmployeeDesignationViewModel>>().Result;
            return designationList;
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync($"Delete/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Employee Deleted Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Failed to delete employee. Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
