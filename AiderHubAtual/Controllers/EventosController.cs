using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiderHubAtual.Models;
using Microsoft.AspNetCore.Http;

namespace AiderHubAtual.Controllers
{
    public class EventosController : Controller
    {
        private readonly Context _context;

        public EventosController(Context context)
        {
            _context = context;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Eventos.ToListAsync());
        }


        public async Task<IActionResult> IndexOng()
        {
            int idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;

            // Filtrar as inscrições pelo valor de idVoluntario
            var eventos = await _context.Eventos
                .Where(e => e.IdOng == idUser)
                .ToListAsync();

            return View(eventos);
        }


        public IActionResult Inscricao(string result)
        {
            ViewBag.Mensagem = result;
            return View();
        }
        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.Id_Evento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Eventos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Eventos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Evento,data_Hora,Endereco,Carga_Horario,Descricao,Responsavel,Status,IdOng")] Evento evento)
        {
            int idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if (ModelState.IsValid)
            {
                evento.IdOng = idUser;
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexOng));
            }
            return View(evento);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        // POST: Eventos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Evento,data_Hora,Endereco,Carga_Horario,Descricao,Responsavel,Status,IdOng")] Evento evento)
        {
            int idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if (id != evento.Id_Evento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    evento.IdOng = idUser;
                    evento.Status = true;
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id_Evento))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.Id_Evento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id_Evento == id);
        }

        public async Task<IActionResult> Inscrever(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.Id_Evento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return RedirectToAction("InscreverConfirmed", new { id });
        }

        public async Task<IActionResult> InscreverConfirmed(int id)
        {
            // Use os valores recebidos para criar a inscrição
            //var idUser = (int)ViewData["IdUser"];
            int idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;

            var existingInscricao = await _context.Inscricoes
            .FirstOrDefaultAsync(i => i.idEvento == id && i.idVoluntario == idUser);

            if (existingInscricao != null)
            {
                // Já existe uma inscrição com os mesmos valores, faça o tratamento necessário
                ViewBag.Mensagem = "Você já está inscrito.";
                return View("Inscricao"); // Redireciona para a página desejada
            }

            Inscricao inscricao = new Inscricao
            {
                idEvento = id,
                idVoluntario = idUser,
                Status = true,
                Tipo = "V",
                Confirmacao = false,
                DataInscricao = DateTime.Today
            };
            var inscricaoController = new InscricoesController(_context);
            await inscricaoController.Create(inscricao);
            // Faça o processamento necessário com a inscrição

            ViewBag.Mensagem = "Inscrição Confirmada";

            return View("Inscricao");
        }

        public async Task<IActionResult> Encerrar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.Id_Evento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return RedirectToAction("EncerrarEvento", new { id });
        }
        public async Task<IActionResult> EncerrarEvento(int id)
        {

            var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id_Evento == id);

            if ((evento != null) && (evento.Status == false))
            {
                ViewBag.Mensagem = "Esse evento já está encerrado!";
                return RedirectToAction("Inscricao", "Eventos", new { result = ViewBag.Mensagem });
            }
            else
            {
                evento.Status = false;
                _context.SaveChanges();

            }

            return RedirectToAction("Index", "Eventos");
        }
        //    //string nomeArquivo = "MacroCertificado.xlsm";
        //    //string diretorioAtual = AppDomain.CurrentDomain.BaseDirectory;
        //    //string caminho = Path.Combine(diretorioAtual, "Relatorio", nomeArquivo);

        //    string caminho = "C:\\Users\\PC\\Documents\\AiderHubShippuden\\AiderHubAtual\\Relatorio\\MacroCertificado.xlsm";

        //    Application xlApp = new Application();

        //    if (xlApp == null)
        //    {
        //        ViewBag.Mensagem = "Erro ao executar a macro: aplicativo Excel não encontrado.";
        //        return View("Relatorio");
        //    }

        //    Workbook xlWorkbook = xlApp.Workbooks.Open(caminho, ReadOnly: false);

        //    try
        //    {
        //        xlApp.Visible = false;
        //        xlApp.Run("GerarCertificado");
        //    }
        //    catch (System.Exception)
        //    {
        //        ViewBag.Mensagem = "Erro ao executar a macro.";
        //        return View("Relatorio");
        //    }

        //    xlWorkbook.Close(false);
        //    xlApp.Application.Quit();
        //    xlApp.Quit();


        //    ViewBag.Mensagem = "Arquivo gerado com sucesso!";
        //    return View("Relatorio");
        //}



        //public async Task<IActionResult> EncerrarAsync(int id)
        //{
        //    //string nomeArquivo = "MacroCertificado.xlsm";
        //    //string diretorioAtual = AppDomain.CurrentDomain.BaseDirectory;
        //    //string caminho = Path.Combine(diretorioAtual, "Relatorio", nomeArquivo);

        //    var evento = await _context.Eventos
        //    .FirstOrDefaultAsync(e => e.Id_Evento == id);

        //    if ((evento != null) && (evento.Status == false))
        //    {
        //        // Já existe uma inscrição com os mesmos valores, faça o tratamento necessário
        //        ViewBag.Mensagem = "Esse evento já está encerrado!";
        //        return RedirectToAction("Index", "Eventos"); // Redireciona para a página desejada
        //    }
        //}
        //    string caminho = "C:\\Users\\PC\\Documents\\AiderHubShippuden\\AiderHubAtual\\Relatorio\\MacroCertificado.xlsm";

        //    Application xlApp = new Application();
        //    Workbook xlWorkbook = null;
        //    Worksheet ws = null;

        //    try
        //    {
        //        xlWorkbook = xlApp.Workbooks.Open(caminho);
        //        ws = (Worksheet)xlApp.ActiveSheet;

        //        List<InscricaoData> inscricoes = ObterInscricoesPorEvento(id);

        //        int startRow = 2; // Começando na linha 2 (exemplo)

        //        for (int i = 0; i < inscricoes.Count; i++)
        //        {
        //            var inscricao = inscricoes[i];

        //            ws.Cells[startRow + i, 1] = inscricao.idEvento;
        //            ws.Cells[startRow + i, 2] = inscricao.idVoluntario;
        //            ws.Cells[startRow + i, 3] = inscricao.NomeVoluntario;
        //            ws.Cells[startRow + i, 4] = inscricao.CargaHoraria;
        //            ws.Cells[startRow + i, 5] = inscricao.DataEvento;
        //            ws.Cells[startRow + i, 6] = inscricao.Ong;
        //        }

        //        xlWorkbook.Save();

        //        try
        //        {
        //            xlApp.Visible = false;
        //            xlApp.Run("GerarCertificado");
        //        }
        //        catch (Exception)
        //        {
        //            ViewBag.Mensagem = "Erro ao executar a macro.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Mensagem = "Erro ao abrir o arquivo da planilha: " + ex.Message;
        //    }
        //    finally
        //    {
        //        if (ws != null)
        //        {
        //            Marshal.ReleaseComObject(ws);
        //        }
        //        if (xlWorkbook != null)
        //        {
        //            xlWorkbook.Close();
        //            Marshal.ReleaseComObject(xlWorkbook);
        //        }
        //        if (xlApp != null)
        //        {
        //            xlApp.Quit();
        //            Marshal.ReleaseComObject(xlApp);
        //        }
        //    }
        //    return RedirectToAction("Inicial", "Home");
        //}

        //public List<InscricaoData> ObterInscricoesPorEvento(int id)
        //{
        //    List<InscricaoData> inscricoes = new List<InscricaoData>();

        //    var evento = _context.Eventos.FirstOrDefault(e => e.Id_Evento == id);
        //    if (evento != null)
        //    {
        //        var inscricoesEvento = _context.Inscricoes.Where(i => i.idEvento == id).ToList();

        //        foreach (var inscricaoEvento in inscricoesEvento)
        //        {
        //            var voluntario = _context.Voluntarios.FirstOrDefault(v => v.Id == inscricaoEvento.idVoluntario);
        //            if (voluntario != null)
        //            {
        //                var ong = _context.Ongs.FirstOrDefault(o => o.Id == evento.IdOng);
        //                if (ong != null)
        //                {
        //                    var inscricao = new InscricaoData
        //                    {
        //                        idEvento = id,
        //                        idVoluntario = inscricaoEvento.idVoluntario,
        //                        NomeVoluntario = voluntario.Nome,
        //                        CargaHoraria = evento.Carga_Horario,
        //                        DataEvento = evento.data_Hora,
        //                        Ong = ong.NomeFantasia
        //                    };

        //                    inscricoes.Add(inscricao);
        //                }
        //            }
        //        }

        //        return inscricoes;
        //    }
        //    return inscricoes;
        //}
    }
}
