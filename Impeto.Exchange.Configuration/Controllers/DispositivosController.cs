using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Impeto.Exchange.Configuration.Contexto;
using Impeto.Exchange.Configuration.Models;
using System.Net.Http;
using System.Net.Http.Formatting;
using Impeto.Framework.Exchange.Entity;
using System.Data.Entity.Validation;

namespace Impeto.Exchange.Configuration.Controllers
{
    public class DispositivosController : Controller
    {
        private ConfiguracaoContexto db = new ConfiguracaoContexto();

        // GET: Dispositivos
        [Authorize]
        public async Task<ActionResult> Index(int? id)
        {
            try
            { 
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (usuario == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int codigoCliente = Convert.ToInt32(id);

            var dispositivoModels = db.DispositivoModels.Where(x => x.CodigoCliente == codigoCliente).ToListAsync();
            return View(await dispositivoModels);
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        public async Task<ActionResult> Todos()
        {
            try
            {
                var dispositivoModels = db.DispositivoModels.ToListAsync();
                return View("Index", await dispositivoModels);
            }
            catch (Exception ex)
            {

            }
            return View("Index");
        }

        // GET: Dispositivos/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (usuario == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivo dispositivoModel = await db.DispositivoModels.Where(x => x.CodigoCliente == usuario.Codigo && x.Codigo == id).FirstOrDefaultAsync();
            if (dispositivoModel == null)
            {
                return HttpNotFound();
            }
            return View(dispositivoModel);
        }

        // GET: Dispositivos/Create
        public ActionResult Create(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (usuario == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idCliente = ((id == null)?0:Convert.ToInt32(id));

            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems;

            var salasAssociadas = db.DispositivoModels.Where(x => x.CodigoCliente == idCliente && x.Smtp == "").Select(x => x.Smtp).ToList();

            var salasDoCliente = new Utils.SalasDeReuniaoHelper().ObterSalasDoCliente(idCliente);
            /*
            foreach (var item in salasDoCliente)
            {
                bool salaJaEstaAssociada = salasAssociadas.Where(s => s == item.Value).FirstOrDefault() == null;
                if (salaJaEstaAssociada)
                {
                    salasDoCliente.Remove(item);
                }
            }
            */
            ViewBag.SalasDoCliente = salasDoCliente;

            return View();
        }

        public async Task<ActionResult> CreateSalas(int? idCliente)
        {
            if (idCliente == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            ViewBag.CodigoCliente = new SelectList(db.ClienteModels.Where(x => x.Codigo == idCliente), "Codigo", "Nome");

            ViewBag.IdCliente = idCliente;

            var dispositivoModels = db.DispositivoModels.Where(x => x.CodigoCliente == idCliente).ToListAsync();
            if (dispositivoModels == null)
            {
                return HttpNotFound();
            }

            return View("Index", await dispositivoModels);
        }

        // POST: Dispositivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int? id, [Bind(Include = "Ativo,Cliente,CodigoCliente,Codigo,DataAtivacao,MAC,Nome,Serial,Smtp,TimeZone,Token")] Dispositivo dispositivoModel)
        {
            dispositivoModel.CodigoCliente = id;
            if (ModelState.IsValid)
            {
                var dispositivo = await db.DispositivoModels.AsNoTracking().Where(x => x.Serial == dispositivoModel.Serial).FirstOrDefaultAsync();
                if (dispositivo != null)
                {

                    dispositivo.DataAtivacao = DateTime.Now;
                    dispositivo.CodigoCliente = dispositivoModel.CodigoCliente;
                    db.Entry(dispositivo).State = System.Data.Entity.EntityState.Modified;

                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index", new { id = dispositivo.CodigoCliente });
            }

            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems;

            var salasDoCliente = new Utils.SalasDeReuniaoHelper().ObterSalasDoCliente(dispositivoModel.CodigoCliente);
            ViewBag.SalasDoCliente = salasDoCliente;


            ViewBag.CodigoCliente = new SelectList(db.ClienteModels, "Codigo", "Nome", dispositivoModel.CodigoCliente);
            return View(dispositivoModel);
        }

        // GET: Dispositivos/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (usuario == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivo dispositivoModel = await db.DispositivoModels.Where(x => x.CodigoCliente == usuario.Codigo && x.Codigo == id).FirstOrDefaultAsync();
            if (dispositivoModel == null)
            {
                return HttpNotFound();
            }

            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems;

            var salasDoCliente = new Utils.SalasDeReuniaoHelper().ObterSalasDoCliente(dispositivoModel.CodigoCliente);
            ViewBag.SalasDoCliente = salasDoCliente;

            ViewBag.CodigoCliente = new SelectList(db.ClienteModels, "Codigo", "Nome", dispositivoModel.CodigoCliente);
            return View(dispositivoModel);
        }

        // POST: Dispositivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Ativo,Cliente,CodigoCliente,Codigo,DataAtivacao,MAC,Nome,Serial,Smtp,TimeZone,Token")] Dispositivo dispositivoModel)
        {

            if (dispositivoModel != null)
            {
                var dispositivo = db.DispositivoModels.Include(t => t.Cliente)
                                                      .Where(t => t.Codigo == dispositivoModel.Codigo)
                                                      .FirstOrDefault();

                // Por prevenção de segurança somente estas propriedades podem ser alteradas
                dispositivo.Nome = dispositivoModel.Nome;
                dispositivo.Smtp = dispositivoModel.Smtp;
                dispositivo.TimeZone = dispositivoModel.TimeZone;
                dispositivo.Ativo = dispositivoModel.Ativo;

                db.Entry(dispositivo).State = System.Data.Entity.EntityState.Modified;

                await db.SaveChangesAsync();

                dispositivoModel = dispositivo;

                return RedirectToAction("Index", new { id = dispositivo.CodigoCliente });
            }

            ;
            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems; // new SelectList(selectListItems, "E. South America Standard Time");

            ViewBag.CodigoCliente = new SelectList(db.ClienteModels, "Codigo", "Nome", dispositivoModel.CodigoCliente);
            return View(dispositivoModel);
        }

        // GET: Dispositivos/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (usuario == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositivo dispositivoModel = await db.DispositivoModels.Where(x => x.CodigoCliente == usuario.Codigo && x.Codigo == id).FirstOrDefaultAsync();
            if (dispositivoModel == null)
            {
                return HttpNotFound();
            }

            return View(dispositivoModel);
        }

        // POST: Dispositivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Dispositivo dispositivoModel = await db.DispositivoModels.FindAsync(id);
            var codigoCliente = dispositivoModel.CodigoCliente;
            dispositivoModel.CodigoCliente = null;
            db.Entry(dispositivoModel).State = System.Data.Entity.EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = codigoCliente });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
