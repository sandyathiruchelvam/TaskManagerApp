using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using TaskManagerApp.Models;

namespace TaskManagerApp.Controllers
{
    public class TaskController : Controller
    {
        // Database connection string - connects to local SQL Server instance
        private readonly string connectionString = "Server=localhost\\SQLEXPRESS;Database=TaskManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Displays the task dashboard for the logged-in user
        public IActionResult TaskDashboard()
        {
            // Get logged-in username from session
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login if no user is logged in
                return RedirectToAction("Login", "Account");
            }

            // Pass username to view for display purposes
            ViewBag.Username = username;

            List<TaskModel> taskList = new List<TaskModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Query to get all tasks for the current user
                string query = "SELECT * FROM Tasks WHERE Username = @Username";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Read all tasks and add them to the list
                while (reader.Read())
                {
                    TaskModel task = new TaskModel
                    {
                        Id = (int)reader["Id"],
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        IsCompleted = Convert.ToBoolean(reader["IsCompleted"].ToString())
                    };
                    taskList.Add(task);
                }

                connection.Close();
            }

            // Return view with the task list
            return View(taskList);
        }

        // GET action for adding a new task - shows the form
        [HttpGet]
        public IActionResult AddTask()
        {
            // Check if user is logged in
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST action for adding a new task - processes the form submission
        [HttpPost]
        public IActionResult AddTask(TaskModel model)
        {
            // Check if user is logged in
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL to insert new task
                string query = "INSERT INTO Tasks (Title, Description, CreatedAt,IsCompleted , Username) VALUES (@Title, @Description, @CreatedAt, @IsCompleted, @Username)";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("IsCompleted", false); // New tasks are not completed by default
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            // Redirect back to dashboard after adding
            return RedirectToAction("TaskDashboard");
        }

        // GET action for editing a task - shows the edit form
        [HttpGet]
        public IActionResult EditTask(int id)
        {
            TaskModel task = new TaskModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Query to get the specific task by ID
                string query = "SELECT * FROM Tasks WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Read task data if found
                if (reader.Read())
                {
                    task.Id = (int)reader["Id"];
                    task.Title = reader["Title"].ToString();
                    task.Description = reader["Description"].ToString();
                    task.IsCompleted = Convert.ToBoolean(reader["IsCompleted"].ToString());
                }

                connection.Close();
            }

            return View(task);
        }

        // POST action for editing a task - processes the edit form submission
        [HttpPost]
        public IActionResult EditTask(TaskModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL to update task information
                string query = "UPDATE Tasks SET Title = @Title, Description = @Description,  IsCompleted= @IsCompleted WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@IsCompleted", model.IsCompleted);
                cmd.Parameters.AddWithValue("@Id", model.Id);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            // Redirect back to dashboard after editing
            return RedirectToAction("TaskDashboard");
        }

        // Action to mark a task as completed
        public IActionResult MarkComplete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // SQL to set IsCompleted to true (1) for the specified task
                string query = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("TaskDashboard");
        }

        // Action to delete a task
        public IActionResult DeleteTask(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL to delete the specified task
                string query = "DELETE FROM Tasks WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            return RedirectToAction("TaskDashboard");
        }
    }
}