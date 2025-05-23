using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http; // Session metodları için

namespace EgitimPortali.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        // Bu satırı bulun ve API'nin portunu yazın:
        private readonly string _apiUrl = "https://localhost:7155"; // veya API'nizin doğru port numarası

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiUrl = _configuration["ApiSettings:BaseUrl"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            // Kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
            if (HttpContext.Session.GetString("JWToken") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult AdminPanel()
        {
            // Admin yetkisi kontrolü
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

        [HttpPost]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                var loginData = new { email = email, password = password };
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7155/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<dynamic>(result);

                    // Role array'den ilk elemanı al
                    var userRole = tokenResponse.roles != null && tokenResponse.roles.Count > 0 ? tokenResponse.roles[0].ToString() : "";

                    // Session kullan
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "JWToken", tokenResponse.token?.ToString() ?? "");
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "UserEmail", email);
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "UserRole", userRole);

                    // Admin ise AdminPanel'e yönlendir
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
                    Console.WriteLine($"🔍 API Error: {errorContent}");
                    return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🔍 HTTP Exception: {httpEx.Message}");
                return Json(new { success = false, message = "API bağlantısı kurulamadı. Lütfen API'nin çalıştığından emin olun." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔍 General Exception: {ex.Message}");
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
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                // Token'ı al
                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"{_apiUrl}/api/Courses");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, data = JsonConvert.DeserializeObject<dynamic>(result) });
                }
                else
                {
                    return Json(new { success = false, message = "Kurslar alınamadı" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourse(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"{_apiUrl}/api/Courses/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, data = JsonConvert.DeserializeObject<dynamic>(result) });
                }
                else
                {
                    return Json(new { success = false, message = "Kurs bulunamadı" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"{_apiUrl}/api/Categories");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, data = JsonConvert.DeserializeObject<dynamic>(result) });
                }
                else
                {
                    return Json(new { success = false, message = "Kategoriler alınamadı" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
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