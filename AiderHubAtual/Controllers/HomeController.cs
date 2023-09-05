using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using AiderHubAtual.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;


namespace AiderHubAtual.Controllers
{
    public class HomeController : Controller
    {
        public bool VoluntarioLogado { get; set; }
        private readonly OpenStreetMapService _openStreetMapService;
        private readonly Context _context;
        public HomeController(Context context)
        {
            _openStreetMapService = new OpenStreetMapService();
            _context = context;
        }

        public ActionResult Inicial()
        {
            int? idUser = HttpContext.Session.GetInt32("IdUser");
            string userTipo = HttpContext.Session.GetString("IdTipo");

            if (idUser.HasValue && !string.IsNullOrEmpty(userTipo))
            {
                return RedirectToAction("Index", new { id = idUser.Value, tipo = userTipo });
            }

            return RedirectToAction("LoginPage", "Usuarios");
        }
        public ActionResult Index(int id, string tipo)
        {
            Usuario usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id && u.Tipo == tipo);

            if (usuario.Tipo == "V")
            {
                bool voluntarioLogado = (usuario != null && usuario.Tipo == "V");

                ViewBag.VoluntarioLogado = voluntarioLogado;
            }
            else
            {
                bool voluntarioLogado = false;
                ViewBag.VoluntarioLogado = voluntarioLogado;
            }
            return View();
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public ActionResult Privacy()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public IActionResult GetEndereco(int idEvento)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.Id_Evento == idEvento);

            if (evento == null)
            {
                return NotFound(); // Evento não encontrado
            }

            var address = evento.Endereco;

            return Ok(address);
        }



        public ActionResult Endereco(string address, string deviceLatitude, string deviceLongitude, int idEvento)
        {
            Coordinates coordinates = _openStreetMapService.GetCoordinates(address);

            ViewBag.Address = address;

            if (coordinates != null)
            {
                ViewBag.Latitude = coordinates.Latitude;
                ViewBag.Longitude = coordinates.Longitude;

                return RedirectToAction("Resultado", new
                {
                    databaseLatitude = ViewBag.Latitude,
                    databaseLongitude = ViewBag.Longitude,
                    deviceLatitude,
                    deviceLongitude,
                    idEvento
                });
                //return View();
                // return RedirectToAction("Device", new { databaseLat = ViewBag.Latitude, databaseLone = ViewBag.Longitude });
            }
            else
            {
                // não achou / deu erro
                ViewBag.ErrorMessage = "Address not found.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(int idEvento, string address, string deviceLatitude, string deviceLongitude)
        {
            return RedirectToAction("Endereco", new { address, deviceLatitude, deviceLongitude, idEvento });
        }

        [HttpGet]
        [HttpPost]
        public async Task<ActionResult> ResultadoAsync(string databaseLatitude, string databaseLongitude, string deviceLatitude, string deviceLongitude, int idEvento)
        {
            int idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;
            double parsedDeviceLatitude = double.Parse(deviceLatitude, CultureInfo.InvariantCulture);
            double parsedDeviceLongitude = double.Parse(deviceLongitude, CultureInfo.InvariantCulture);
            //double deviceLatitude = -23.465585;
            //double deviceLongitude = -46.573850;

            if (string.IsNullOrEmpty(databaseLatitude) || string.IsNullOrEmpty(databaseLongitude))
            {
                return View("Eventos/Index");
            }

            double parsedDataBaselatitude = double.Parse(databaseLatitude, CultureInfo.InvariantCulture);
            double parsedDataBaselongitude = double.Parse(databaseLongitude, CultureInfo.InvariantCulture);

            double distanceInMeters = CalculateDistance(parsedDeviceLatitude, parsedDeviceLongitude, parsedDataBaselatitude, parsedDataBaselongitude);

            if (distanceInMeters <= 4000)
            {
                ViewBag.resultado = "DENTRO DO RAIO, CHECK-IN REALIZADO COM SUCESSO!";
                ViewBag.coordenadas = $"{parsedDeviceLatitude}, {parsedDeviceLongitude}";
                ViewBag.distancia = distanceInMeters;

                var inscricao = _context.Inscricoes.FirstOrDefault(i => i.idEvento == idEvento && i.idVoluntario == idUser);

                if (inscricao != null)
                {
                    if (inscricao.Confirmacao == true)
                    {
                        ViewBag.Mensagem = "Você já fez check-in nesse evento!";
                        return RedirectToAction("Inscricao", "Eventos", new { result = ViewBag.Mensagem });
                    }
                    else
                    {
                        inscricao.Confirmacao = true;
                        _context.SaveChanges();
                    }
                }

                var evento = _context.Eventos.FirstOrDefault(e => e.Id_Evento == idEvento);
                if (evento != null)
                {
                    var inscricoesEvento = _context.Inscricoes.Where(i => i.idEvento == idEvento).ToList();

                    var voluntario = _context.Voluntarios.FirstOrDefault(v => v.Id == idUser);
                    if (voluntario != null)
                    {
                        var ong = _context.Ongs.FirstOrDefault(o => o.Id == evento.IdOng);
                        if (ong != null)
                        {
                            Relatorio relatorio = new Relatorio
                            {
                                IdEvento = idEvento,
                                IdVoluntario = voluntario.Id,
                                NomeVoluntario = voluntario.Nome,
                                CargaHoraria = evento.Carga_Horario,
                                DataEvento = evento.data_Hora,
                                NomeONG = ong.NomeFantasia
                            };

                            //var relatorioController = new RelatoriosController(_context);
                            //await relatorioController.Create(relatorio);

                            return RedirectToAction("Validar", new { result = ViewBag.resultado, coordinate = ViewBag.coordenadas, distance = ViewBag.distancia });
                        }
                    }

                }
                return RedirectToAction("Validar", new { result = ViewBag.resultado, coordinate = ViewBag.coordenadas, distance = ViewBag.distancia });
            }
            else
            {
                ViewBag.resultado = "FORA DO RAIO, CHECK-IN INVÁLIDO!";
                ViewBag.coordenadas = $"{parsedDeviceLatitude}, {parsedDeviceLongitude}";
                ViewBag.distancia = distanceInMeters;

                return RedirectToAction("Validar", new { result = ViewBag.resultado, coordinate = ViewBag.coordenadas, distance = ViewBag.distancia });
            }
        }

        public ActionResult Validar(string result, string coordinate, double distance)
        {
            ViewBag.Result = result;
            ViewBag.Coordinate = coordinate;
            ViewBag.Distance = distance;
            return View();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371000; // in meters

            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = EarthRadius * c;

            return distance;
        }

        public ActionResult Saindo()
        {
            HttpContext.Session.Clear();

            // Redireciona para a página de login
            return RedirectToAction("LoginPage", "Usuarios");
        }

        /*public ActionResult Relatorio()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
    }
}
