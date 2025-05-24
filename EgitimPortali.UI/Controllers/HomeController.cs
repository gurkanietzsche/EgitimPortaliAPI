using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EgitimPortali.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();

            // API URL'ini çeşitli kaynaklardan al
            _apiUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7155";

            _logger.LogInformation($"API URL: {_apiUrl}");
        }

        // VIEW METODLARI
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("JWToken") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult AdminPanel()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        public IActionResult CourseDetail(int id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        public IActionResult CourseManagement()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult CategoryManagement()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult UserManagement()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult ApiTest()
        {
            return View();
        }

        // AUTHENTICATION METODLARI
        [HttpPost]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                var loginData = new { email = email, password = password };
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var loginUrl = $"{_apiUrl}/api/auth/login";
                _logger.LogInformation($"Login URL: {loginUrl}");

                var response = await _httpClient.PostAsync(loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<dynamic>(result);

                    var userRole = tokenResponse.roles != null && tokenResponse.roles.Count > 0 ?
                                  tokenResponse.roles[0].ToString() : "";

                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "JWToken", tokenResponse.token?.ToString() ?? "");
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "UserEmail", email);
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "UserRole", userRole);

                    if (userRole == "Admin")
                    {
                        return Json(new { success = true, message = "Giriş başarılı", returnUrl = "/Home/AdminPanel" });
                    }
                    else
                    {
                        return Json(new { success = true, message = "Giriş başarılı", returnUrl = "/" });
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Login Error: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"HTTP Exception: {httpEx.Message}");
                return Json(new { success = false, message = "API bağlantısı kurulamadı. API çalışıyor mu kontrol edin." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"General Exception: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            try
            {
                if (password != confirmPassword)
                {
                    return Json(new { success = false, message = "Şifreler eşleşmiyor" });
                }

                var registerData = new { firstName, lastName, email, password, role = "Student" };
                var json = JsonConvert.SerializeObject(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}/api/Auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kayıt başarılı, lütfen giriş yapın" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Kayıt başarısız: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register Error: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true });
        }

        // DATA GET METODLARI
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var coursesUrl = $"{_apiUrl}/api/Courses";
                _logger.LogInformation($"Getting courses from: {coursesUrl}");

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(coursesUrl);

                _logger.LogInformation($"Courses API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Courses API Response: {result}");

                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Courses API Error: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = $"Kurslar alınamadı: {response.StatusCode}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"HTTP Exception in GetCourses: {httpEx.Message}");
                return Json(new { success = false, message = "API bağlantısı kurulamadı" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetCourses: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourse(int id)
        {
            try
            {
                var courseUrl = $"{_apiUrl}/api/Courses/{id}";
                _logger.LogInformation($"Getting course from: {courseUrl}");

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(courseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Get Course API Error: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = "Kurs bulunamadı" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetCourse: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categoriesUrl = $"{_apiUrl}/api/Category";
                _logger.LogInformation($"Getting categories from: {categoriesUrl}");

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(categoriesUrl);

                _logger.LogInformation($"Categories API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Categories API Response: {result}");

                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Categories API Error: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = $"Kategoriler alınamadı: {response.StatusCode}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"HTTP Exception in GetCategories: {httpEx.Message}");
                return Json(new { success = false, message = "API bağlantısı kurulamadı" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetCategories: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var categoryUrl = $"{_apiUrl}/api/Category/{id}";
                _logger.LogInformation($"Getting category from: {categoryUrl}");

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(categoryUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Get Category API Error: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = "Kategori bulunamadı" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetCategory: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // KURS YÖNETİMİ METODLARI
        [HttpPost]
        public async Task<IActionResult> AddCourse(string title, string description, decimal price, int categoryId, string imageUrl, int duration, string difficultyLevel)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var courseData = new
                {
                    title = title,
                    description = description,
                    price = price,
                    categoryId = categoryId,
                    imageUrl = imageUrl,
                    duration = duration,
                    difficultyLevel = difficultyLevel
                };

                var json = JsonConvert.SerializeObject(courseData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Adding course: {json}");

                var response = await _httpClient.PostAsync($"{_apiUrl}/api/Courses", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kurs başarıyla eklendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Add Course Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kurs eklenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in AddCourse: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourse(int id, string title, string description, decimal price, int categoryId, string imageUrl, int duration, string difficultyLevel)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var courseData = new
                {
                    id = id,
                    title = title,
                    description = description,
                    price = price,
                    categoryId = categoryId,
                    imageUrl = imageUrl,
                    duration = duration,
                    difficultyLevel = difficultyLevel
                };

                var json = JsonConvert.SerializeObject(courseData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiUrl}/api/Courses/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kurs başarıyla güncellendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Update Course Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kurs güncellenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in UpdateCourse: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.DeleteAsync($"{_apiUrl}/api/Courses/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kurs başarıyla silindi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Delete Course Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kurs silinirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in DeleteCourse: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // KATEGORİ YÖNETİMİ METODLARI
        [HttpPost]
        public async Task<IActionResult> AddCategory(string name, string description)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var categoryData = new
                {
                    name = name,
                    description = description
                };

                var json = JsonConvert.SerializeObject(categoryData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Adding category: {json}");

                var response = await _httpClient.PostAsync($"{_apiUrl}/api/Category", content);

                _logger.LogInformation($"Add Category Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kategori başarıyla eklendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Add Category Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kategori eklenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in AddCategory: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, string name, string description)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var categoryData = new
                {
                    id = id,
                    name = name,
                    description = description
                };

                var json = JsonConvert.SerializeObject(categoryData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiUrl}/api/Category/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kategori başarıyla güncellendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Update Category Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kategori güncellenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in UpdateCategory: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.DeleteAsync($"{_apiUrl}/api/Category/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kategori başarıyla silindi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Delete Category Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kategori silinirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in DeleteCategory: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // KULLANICI YÖNETİMİ METODLARI
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Oturum bulunamadı." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_apiUrl}/api/User");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = users });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Get Users Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kullanıcılar getirilemedi: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetUsers: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Oturum bulunamadı." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_apiUrl}/api/User/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<dynamic>(result);
                    return Json(new { success = true, data = user });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Get User Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kullanıcı bulunamadı: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetUser: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string firstName, string lastName, string email, string password, string role)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Oturum bulunamadı." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userData = new
                {
                    firstName = firstName,
                    lastName = lastName,
                    email = email,
                    password = password,
                    role = role
                };

                var json = JsonConvert.SerializeObject(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}/api/User", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kullanıcı başarıyla eklendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Add User Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kullanıcı eklenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in AddUser: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(int id, string firstName, string lastName, string email, string role)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Oturum bulunamadı." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userData = new
                {
                    id = id,
                    firstName = firstName,
                    lastName = lastName,
                    email = email,
                    role = role
                };

                var json = JsonConvert.SerializeObject(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiUrl}/api/User/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Update User Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kullanıcı güncellenirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in UpdateUser: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
                }

                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Oturum bulunamadı." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"{_apiUrl}/api/User/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Delete User Error: {response.StatusCode} - {error}");
                    return Json(new { success = false, message = $"Kullanıcı silinirken hata oluştu: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in DeleteUser: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // KURSA KAYIT OLMA METODU
        [HttpPost]
        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Lütfen önce giriş yapın." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var enrollmentData = new { courseId = courseId };
                var json = JsonConvert.SerializeObject(enrollmentData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}/api/Enrollments", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Kursa başarıyla kaydoldunuz!" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Kursa kayıt işlemi başarısız: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in EnrollCourse: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // API TEST METODLARI
        [HttpGet]
        public async Task<IActionResult> TestApi()
        {
            try
            {
                var results = new
                {
                    timestamp = DateTime.Now,
                    apiUrl = _apiUrl,
                    tests = new List<object>()
                };

                // API Base URL testi
                try
                {
                    var baseResponse = await _httpClient.GetAsync(_apiUrl);
                    results.tests.Add(new
                    {
                        test = "Base URL Test",
                        url = _apiUrl,
                        status = baseResponse.StatusCode.ToString(),
                        success = baseResponse.IsSuccessStatusCode,
                        reason = baseResponse.ReasonPhrase
                    });
                }
                catch (Exception ex)
                {
                    results.tests.Add(new
                    {
                        test = "Base URL Test",
                        url = _apiUrl,
                        status = "Exception",
                        success = false,
                        error = ex.Message
                    });
                }

                // Categories endpoint testi
                try
                {
                    var categoriesUrl = $"{_apiUrl}/api/Category";
                    var catResponse = await _httpClient.GetAsync(categoriesUrl);
                    var catContent = await catResponse.Content.ReadAsStringAsync();

                    results.tests.Add(new
                    {
                        test = "Categories Endpoint",
                        url = categoriesUrl,
                        status = catResponse.StatusCode.ToString(),
                        success = catResponse.IsSuccessStatusCode,
                        contentLength = catContent?.Length ?? 0,
                        content = !string.IsNullOrEmpty(catContent) ? catContent.Substring(0, Math.Min(200, catContent.Length)) : "Boş içerik"
                    });
                }
                catch (Exception ex)
                {
                    results.tests.Add(new
                    {
                        test = "Categories Endpoint",
                        url = $"{_apiUrl}/api/Category",
                        status = "Exception",
                        success = false,
                        error = ex.Message
                    });
                }

                // Courses endpoint testi
                try
                {
                    var coursesUrl = $"{_apiUrl}/api/Courses";
                    var courseResponse = await _httpClient.GetAsync(coursesUrl);
                    var courseContent = await courseResponse.Content.ReadAsStringAsync();

                    results.tests.Add(new
                    {
                        test = "Courses Endpoint",
                        url = coursesUrl,
                        status = courseResponse.StatusCode.ToString(),
                        success = courseResponse.IsSuccessStatusCode,
                        contentLength = courseContent?.Length ?? 0,
                        content = !string.IsNullOrEmpty(courseContent) ? courseContent.Substring(0, Math.Min(200, courseContent.Length)) : "Boş içerik"
                    });
                }
                catch (Exception ex)
                {
                    results.tests.Add(new
                    {
                        test = "Courses Endpoint",
                        url = $"{_apiUrl}/api/Courses",
                        status = "Exception",
                        success = false,
                        error = ex.Message
                    });
                }

                // Users endpoint testi
                try
                {
                    var token = HttpContext.Session.GetString("JWToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var usersUrl = $"{_apiUrl}/api/User";
                        var usersResponse = await _httpClient.GetAsync(usersUrl);
                        var usersContent = await usersResponse.Content.ReadAsStringAsync();

                        results.tests.Add(new
                        {
                            test = "Users Endpoint",
                            url = usersUrl,
                            status = usersResponse.StatusCode.ToString(),
                            success = usersResponse.IsSuccessStatusCode,
                            contentLength = usersContent?.Length ?? 0,
                            content = !string.IsNullOrEmpty(usersContent) ? usersContent.Substring(0, Math.Min(200, usersContent.Length)) : "Boş içerik"
                        });
                    }
                    else
                    {
                        results.tests.Add(new
                        {
                            test = "Users Endpoint",
                            url = $"{_apiUrl}/api/User",
                            status = "Skipped",
                            success = false,
                            error = "Token bulunamadı, test atlandı"
                        });
                    }
                }
                catch (Exception ex)
                {
                    results.tests.Add(new
                    {
                        test = "Users Endpoint",
                        url = $"{_apiUrl}/api/User",
                        status = "Exception",
                        success = false,
                        error = ex.Message
                    });
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Test Error: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    apiUrl = _apiUrl,
                    timestamp = DateTime.Now
                });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}