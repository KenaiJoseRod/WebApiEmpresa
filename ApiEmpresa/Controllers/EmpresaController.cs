using ApiEmpresa.Models;
using Microsoft.AspNetCore.Mvc;


using Newtonsoft.Json;
using System.Text;
namespace ApiEmpresa.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly HttpClient _httpClient;
        public EmpresaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://night-mango-berry.glitch.me/");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ApiEmpresa/1.0");

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/consultas/Obtener_todos_los_datos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var empresa = JsonConvert.DeserializeObject<IEnumerable<EmpresaViewsModels>>(content);
                    return View("Index", empresa);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Registra el error para depuración
                    Console.WriteLine($"Error: {response.StatusCode} - {errorContent}");
                    return View(new List<EmpresaViewsModels>());
                }
            }
            catch (Exception ex)
            {
                // Registra cualquier excepción que ocurra
                Console.WriteLine($"Exception: {ex.Message}");
                return View(new List<EmpresaViewsModels>());
            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmpresaViewsModels Empresa)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(Empresa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/consultas/insertarEmpr", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear empresa");
                }
            }
            return View(Empresa);
        }
        public async Task<IActionResult> Edit(int id)
        {

            var response = await _httpClient.GetAsync($"/api/consultas/BuscarEmp?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var empresas = JsonConvert.DeserializeObject<List<EmpresaViewsModels>>(content);

                var empresa = empresas.FirstOrDefault();

               
                return View(empresa);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/consultas/eliminarEmpresa?id={id}");
          
            if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
             else
                {
                    TempData["Error"] = "Error al eliminar el Empresa.";
                    return RedirectToAction("Index");
                }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmpresaViewsModels EmpresaViewsModels)
        {
            if (ModelState.IsValid)
            {


                var json = JsonConvert.SerializeObject(EmpresaViewsModels);
                var content = new StringContent(json, Encoding.UTF8,"application/json");

                var response = await _httpClient.PutAsync($"/api/consultas/actualizar?id={id}",content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el empresa.");
                }
            }

            return View(EmpresaViewsModels);
        }

    }
}
