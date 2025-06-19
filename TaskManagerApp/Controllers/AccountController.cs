using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using TaskManagerApp.Models;

namespace TaskManagerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(User model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["ToastMessage"] = "Fill all the fields!";
                return View(model);
            }

            

            string connectionString = _configuration.GetConnectionString("DefaultConnection"); // ✅ Correct spelling
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if username exists
                string userQuery = "SELECT COUNT(*) FROM Users WHERE Username=@Username";
                SqlCommand userCmd = new SqlCommand(userQuery, connection);
                userCmd.Parameters.AddWithValue("@Username", model.Username);
                int userexist = (int)userCmd.ExecuteScalar();

                // Check if password exists for that username
                string passQuery = "SELECT COUNT(*) FROM Users WHERE Username=@Username AND Password=@Password";
                SqlCommand passCmd = new SqlCommand(passQuery, connection);
                passCmd.Parameters.AddWithValue("@Username", model.Username);
                passCmd.Parameters.AddWithValue("@Password", model.Password);
                int bothexist = (int)passCmd.ExecuteScalar();

                if (bothexist > 0)
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    TempData["SuccessMessage"] = "✅ Login successful!";
                    return RedirectToAction("TaskDashboard", "Task");
                }
                

                else if (userexist > 0)
                {
                    TempData["ToastMessage"] = "❌ Incorrect Password!";
                }
                else
                {
                    TempData["ToastMessage"] = "❌ Username not found!";
                }

                return RedirectToAction("Login");

            }
        }

        // GET: /Account/Signup
        public IActionResult Signup()
        {
            return View();
        }

        // POST: /Account/Signup
        [HttpPost]
        public IActionResult Signup(User model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["ToastMessage"] = "❗ Please fill in all fields!";
                return View(model);
            }


            if (model.Password.Length < 5)
            {
                TempData["ToastMessage"] = "Password must be at least 5 characters long.";
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check for duplicate username
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Username", model.Username);
                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    TempData["ToastMessage"] = "Username already exists. Try another.";
                    return View(model);
                }

                // Insert new user
                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@Username", model.Username);
                insertCmd.Parameters.AddWithValue("@Password", model.Password);

                int result = insertCmd.ExecuteNonQuery();

                if (result > 0)
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    TempData["SuccessMessage"] = "Signup successful!";
                    return RedirectToAction("TaskDashboard", "Task");
                }
                else
                {
                    TempData["ToastMessage"] = "Signup failed. Try again.";
                }
            }

            return View(model);
        }
    }
}
