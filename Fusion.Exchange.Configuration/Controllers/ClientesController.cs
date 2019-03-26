using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fusion.Exchange.Configuration.Contexto;
using Fusion.Exchange.Configuration.Models;
using Fusion.Framework.Exchange.Entity;
using Fusion.Exchange.Configuration.Utils;

namespace Fusion.Exchange.Configuration.Controllers
{
    public class ClientesController : Controller
    {
        private ConfiguracaoContexto db = new ConfiguracaoContexto();


        // GET: Clientes
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var clienteModels = db.ClienteModels.ToListAsync();
            return View(await clienteModels);
        }

        [Authorize]
        // GET: Clientes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            if (clienteModel == null)
            {
                return HttpNotFound();
            }
            return View(clienteModel);
        }

        [Authorize]
        public async Task<ActionResult> Detalhes(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            if (clienteModel == null)
            {
                return HttpNotFound();
            }
            return View(clienteModel);
        }


        // GET: Clientes/Create
        public ActionResult Create()
        {
            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems; // new SelectList(selectListItems, "E. South America Standard Time");

            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Codigo,Nome,Smtp,Senha,TimeZone,Token,DataCadastro")] Cliente clienteModel)
        {
            if (ModelState.IsValid)
            {
                db.ClienteModels.Add(clienteModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clienteModel);
        }

        //------------------------------

        // GET: Clientes/Edit/5
        public async Task<ActionResult> Editar(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems;

            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            if (clienteModel == null)
            {
                return HttpNotFound();
            }

            return View(clienteModel);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar([Bind(Include = "Codigo,Nome,Smtp,Senha,TimeZone,Token,DataCadastro,FirstDevice")] Cliente clienteModel)
        {
            // Somente Nome, Smtp e Senha podem ser alterados
            var cliente = db.ClienteModels.Where(t => t.Codigo == clienteModel.Codigo).FirstOrDefault();

            if (cliente != null)
            {
                cliente.Nome = clienteModel.Nome;
                cliente.Smtp = clienteModel.Smtp;
                cliente.Senha = new Cripto().GerarHashSHA256(clienteModel.Senha);

                // Se houver retorno antecipado, retorna com as informações da busca
                clienteModel.Codigo = cliente.Codigo;
                clienteModel.DataCadastro = cliente.DataCadastro;
                clienteModel.Dispositivos = cliente.Dispositivos;
                clienteModel.FirstDevice = cliente.FirstDevice;
                clienteModel.Nome = cliente.Nome;
                clienteModel.Senha = cliente.Senha;
                clienteModel.Smtp = cliente.Smtp;
                clienteModel.Token = cliente.Token;
                clienteModel.UserIdentity = cliente.UserIdentity;

                db.Entry(cliente).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Detalhes","Clientes",new { id = cliente.Codigo });
            }

            return View(clienteModel);
        }

        //------------------------------
        // GET: Clientes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fusos = new Utils.FusoHorarioUtils().GetAllFusos();
            var selectListItems = fusos.Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = (x.ID == "E. South America Standard Time") }).ToList();
            ViewBag.TimeZoneTable = selectListItems;

            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            if (clienteModel == null)
            {
                return HttpNotFound();
            }

            return View(clienteModel);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Codigo,Nome,Smtp,Senha,TimeZone,Token,DataCadastro")] Cliente clienteModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clienteModel).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clienteModel);
        }

        // GET: Clientes/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            if (clienteModel == null)
            {
                return HttpNotFound();
            }
            return View(clienteModel);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cliente clienteModel = await db.ClienteModels.FindAsync(id);
            db.ClienteModels.Remove(clienteModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSalas(int? id)
        {
            return RedirectToAction("CreateSalas", "Dispositivos", new { idCliente = id });
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
