using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RpgMvc.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;



namespace RpgMvc.Controllers
{
    public class UsuariosController : Controller
    {
       public string uriBase = "http://luizsouza.somee.com/RpgApi/Usuarios/";
       //xyz tem que ser substituido pelo endereço da sua API.


       [HttpGet]

       public ActionResult Index()
       {
            return View("CadastrarUsuario");
       }

       [HttpPost]
       public async Task<ActionResult> RegistrarAsync(UsuarioViewModel u)
       {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementar = "Registrar";

            var content = new StringContent(JsonConvert.SerializeObject(u));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Mensagem"] = 
                    string.Format("Usuário {0} Registrado com sucesso! Faca o login para acessar.", u.Username);
                return View("AutenticarUsuario");    
            }
            else
            {
                throw new System.Exception(serialized);
            }


        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Index");
        }
       }


       [HttpGet]
       public ActionResult IndexLogin()
       {
            return View("AutenticarUsuario");
       }


       [HttpPost]
       public async Task<ActionResult> AutenticarAsync(UsuarioViewModel u)
       {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Autenticar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UsuarioViewModel uLogado = JsonConvert.DeserializeObject<UsuarioViewModel>(serialized);
                    HttpContext.Session.SetString("SessionTokenUsuario", uLogado.Token);
                    TempData["Mensagem"] = string.Format("Bem-vindo {0}!!!", uLogado.Username);
                    return RedirectToAction("Index", "Personagens");
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return IndexLogin();
            }
       }
    }
}