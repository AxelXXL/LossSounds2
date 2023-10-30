using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LossSounds.Models;
namespace LossSounds.Controllers
{
    public class HomeController : Controller
    {
        private BD_LOSS_SOUNDSEntities db = new BD_LOSS_SOUNDSEntities();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult LogPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tb_Usuario userr)
        {
            // Aquí debes realizar la validación del usuario y contraseña.
            // Puedes consultar tu base de datos u otro sistema de autenticación.
            // Si las credenciales son válidas, inicia sesión y redirige a la página principal.
            try
            {
                var validacion = db.tb_Usuario
                    .Where(a => a.Nombre_Usuario == userr.Nombre_Usuario && a.Contrasena == userr.Contrasena);
                var idUser = db.tb_Usuario
                     .Where(a => a.Nombre_Usuario == userr.Nombre_Usuario && a.Contrasena == userr.Contrasena)
                     .Select(c => new
                     {
                         ID = c.ID_USUARIO
                     })
                     .FirstOrDefault();

                if (validacion != null)
                {
                    tb_Usuario oUser = validacion.First();
                    Session["User"] = oUser;
                    Session["Name"] = userr.Nombre_Usuario.ToString();
                    Session["IdUser"] = idUser.ID;

                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    return Content("Usuario invalido");
                }


            }
            catch (Exception ex)
            {
                return Content("Ocurrió un error " + ex.Message);
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult HomePage()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public JsonResult GetNovedades()
        {
            var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                                       .Take(50) // Tomar las primeras 50 filas
                                       .Select(c => new
                                       {
                                           IdCancion = c.ID_CANCION,
                                           NombreCancion = c.Nombre_Cancion,
                                           NombreArtista = c.tb_Artista.Nombre_Artista,
                                           NombreAlbum = c.tb_Album.Nombre_album,
                                           Caratula = c.Caratula_Cancion
                                       })
                                           .ToList();



            return Json(datos, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetRecomendaciones(int idUser)
        {
            var random = new Random();

            var datalikes = db.tb_LikeMusic.Include(t => t.tb_Cancion).Include(t => t.tb_Usuario)
                                      .Where(a => a.ID_USUARIO == idUser)
                                      .Select(c => new
                                      {
                                          Gen = c.tb_Cancion.tb_Album.Genero
                                      })

                                      .FirstOrDefault();

            if (datalikes != null)
            {
                //// Mezcla la lista
                //datalikes = datalikes.OrderBy(x => random.Next()).ToList();

                //var randomLike = datalikes.FirstOrDefault();
                var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                                      .Where(c => c.tb_Album.Genero.Contains(datalikes.Gen))
                                      .Take(50) // Tomar las primeras 50 filas
                                      .Select(c => new
                                      {
                                          IdCancion = c.ID_CANCION,
                                          NombreCancion = c.Nombre_Cancion,
                                          NombreArtista = c.tb_Artista.Nombre_Artista,
                                          NombreAlbum = c.tb_Album.Nombre_album,
                                          Caratula = c.Caratula_Cancion
                                      })
                                          .ToList();
                return Json(datos, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                                      .Take(50) // Tomar las primeras 50 filas
                                      .Select(c => new
                                      {
                                          IdCancion = c.ID_CANCION,
                                          NombreCancion = c.Nombre_Cancion,
                                          NombreArtista = c.tb_Artista.Nombre_Artista,
                                          NombreAlbum = c.tb_Album.Nombre_album,
                                          Caratula = c.Caratula_Cancion
                                      })

                                          .ToList();
                return Json(datos, JsonRequestBehavior.AllowGet);
            }





        }
        public JsonResult GetCancionList(string txt)
        {
            var items = db.tb_Cancion
                .Where(x =>
                x.Nombre_Cancion.Contains(txt) || x.tb_Album.Nombre_album.Contains(txt) || x.tb_Album.Genero.Contains(txt)
                || x.tb_Artista.Nombre_Artista.Contains(txt))
                .Select(x => new
                {
                    Text = x.Nombre_Cancion,
                    Value = x.ID_CANCION
                })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);



        }
        [HttpGet]
        public JsonResult GetCancionesArtist(int idArtist)
        {
            var canciones = db.tb_Cancion.Include(t => t.tb_Album)
                              .Where(b => b.ID_ARTISTA == idArtist)
                              .Select(t => new
                              {
                                  t.ID_CANCION,
                                  t.Nombre_Cancion,
                                  t.tb_Album.Nombre_album,
                                  t.Caratula_Cancion,

                              }).ToList();

            return Json(canciones, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAlbumsArtist(int idArtist)
        {
            var canciones = db.tb_Album
                              .Where(b => b.ID_ARTISTA == idArtist)
                              .Select(t => new
                              {
                                  t.ID_ALBUM,
                                  t.Nombre_album,
                                  t.Genero,
                                  t.Año_Album

                              }).ToList();

            return Json(canciones, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public FileResult GetImagen(byte[] dataImg)
        {
            byte[] fileBytes = dataImg;// aqui el array de bytes
            return File(fileBytes, "image/png", "nombre.png"); //3er argumento es opcional
        }
        [HttpPost]
        public JsonResult AddLike(int idUser, int idCancion)
        {
            tb_Usuario findUser = db.tb_Usuario.Find(idUser);
            tb_Cancion findCancion = db.tb_Cancion.Find(idCancion);

            if (findCancion != null && findUser != null)
            {
                tb_LikeMusic findLike = db.tb_LikeMusic.Where(b => b.ID_USUARIO == idUser && b.ID_CANCION == idCancion)
                                               .FirstOrDefault();
                tb_DisLikeMusic findDislike = db.tb_DisLikeMusic.Where(b => b.ID_USUARIO == idUser && b.ID_CANCION == idCancion)
                                                                .FirstOrDefault();

                if (findLike == null)
                {
                    tb_LikeMusic likes = new tb_LikeMusic();
                    likes.ID_USUARIO = idUser;
                    likes.ID_CANCION = idCancion;
                    db.tb_LikeMusic.Add(likes);
                    db.SaveChanges();
                    if (findDislike != null)
                    {
                        db.tb_DisLikeMusic.Remove(findDislike);
                        db.SaveChanges();
                    }

                }
                else
                {
                    db.tb_LikeMusic.Remove(findLike);
                    db.SaveChanges();
                }

            }
            else
            {
                return Json("Error INVALID ID_CANCION ID_USER");
            }



            return Json("Done Likes", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetLikes(string id)
        {
            int idUser = int.Parse(id);

            var datos = db.tb_LikeMusic.Include(t => t.tb_Cancion)
                                        .Where(x => x.ID_USUARIO == idUser)
                                        .Select(a => new
                                        {
                                            NombreCancion = a.tb_Cancion.Nombre_Cancion,
                                            NombreArtista = a.tb_Cancion.tb_Artista.Nombre_Artista,
                                            NombreAlbum = a.tb_Cancion.tb_Album.Nombre_album,
                                            Caratula = a.tb_Cancion.Caratula_Cancion,
                                            IdCancion = a.ID_CANCION.ToString()
                                        })
                                        .ToList();

            return Json(datos);
        }
        /// <summary>
        /// /////////////////////////////////////////
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idCancion"></param>
        /// <returns></returns>
        //////// DISLIKE ///////////////
        public JsonResult AddDislike(int idUser, int idCancion)
        {
            tb_Usuario findUser = db.tb_Usuario.Find(idUser);
            tb_Cancion findCancion = db.tb_Cancion.Find(idCancion);

            if (findCancion != null && findUser != null)
            {
                tb_DisLikeMusic findDislike = db.tb_DisLikeMusic.Where(b => b.ID_USUARIO == idUser && b.ID_CANCION == idCancion)
                                               .FirstOrDefault();
                tb_LikeMusic findLike = db.tb_LikeMusic.Where(b => b.ID_USUARIO == idUser && b.ID_CANCION == idCancion)
                                                                .FirstOrDefault();

                if (findDislike == null)
                {
                    tb_DisLikeMusic Dislike = new tb_DisLikeMusic();
                    Dislike.ID_USUARIO = idUser;
                    Dislike.ID_CANCION = idCancion;
                    db.tb_DisLikeMusic.Add(Dislike);
                    db.SaveChanges();
                    if (findLike != null)
                    {
                        db.tb_LikeMusic.Remove(findLike);
                        db.SaveChanges();
                    }

                }
                else
                {
                    db.tb_DisLikeMusic.Remove(findDislike);
                    db.SaveChanges();
                }

            }
            else
            {
                return Json("Error INVALID ID_CANCION ID_USER");
            }


            return Json("Done Dislike", JsonRequestBehavior.AllowGet);
        }
    }
}