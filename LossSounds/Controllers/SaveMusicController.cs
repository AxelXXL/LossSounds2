using LossSounds.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using TagLib;

namespace LossSounds.Controllers
{
    public class SaveMusicController : Controller
    {
        private BD_LOSS_SOUNDSEntities db = new BD_LOSS_SOUNDSEntities();

        // GET: SaveMusic
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerMetadatosCancion(string archivo)
        {
            if (archivo != null && archivo.Length > 0)
            {
                try
                {
                    using (File archivoAudio = TagLib.File.Create(new File.LocalFileAbstraction(archivo)))
                    {
                        string[] artistas = archivoAudio.Tag.Performers;
                        string nomArtista = artistas[0];
                        string nomAlbum = archivoAudio.Tag.Album;
                        string genero = archivoAudio.Tag.FirstGenre;
                        int añoAlbum = (int)archivoAudio.Tag.Year;
                        int numeroCancion = (int)archivoAudio.Tag.Track;
                        int duracionSegundos = (int)archivoAudio.Properties.Duration.TotalSeconds;
                        string tituloCancion = archivoAudio.Tag.Title;

                        // Buscar si el artista ya existe en la bd
                        var artistaID = (from a in db.tb_Artista
                                         where a.Nombre_Artista == nomArtista
                                         select a.ID_ARTISTA).FirstOrDefault();

                        // Buscar si el álbum ya existe en la bd
                        var albumID = (from a in db.tb_Album
                                       where a.Nombre_album == nomAlbum
                                       select a.ID_ALBUM).FirstOrDefault();

                        if (artistaID != 0)
                        {
                            // El artista ya existe en la base de datos, no es necesario crearlo de nuevo
                        }
                        else
                        {
                            // El artista no existe en la base de datos, crea un nuevo registro
                            tb_Artista artista = new tb_Artista
                            {
                                Nombre_Artista = nomArtista,
                            };
                            db.tb_Artista.Add(artista);
                            db.SaveChanges(); // Guardar el artista para obtener su ID
                            artistaID = artista.ID_ARTISTA; // Obtener el ID del artista recién creado
                        }

                        if (albumID != 0)
                        {
                            // El álbum ya existe en la base de datos, no es necesario crearlo de nuevo
                        }
                        else
                        {
                            // El álbum no existe en la base de datos, crea un nuevo registro
                            tb_Album album = new tb_Album
                            {
                                Nombre_album = nomAlbum,
                                Genero = genero,
                                Año_Album = añoAlbum,
                                ID_ARTISTA = artistaID, // Asignar el ID del artista
                            };
                            db.tb_Album.Add(album);
                            db.SaveChanges(); // Guardar el álbum para obtener su ID
                            albumID = album.ID_ALBUM; // Obtener el ID del álbum recién creado
                        }

                        // Ahora puedes crear la canción utilizando artistaID y albumID
                        tb_Cancion cancion = new tb_Cancion
                        {
                            Nombre_Cancion = tituloCancion,
                            Duracion_Cancion = duracionSegundos,
                            Numero_Cancion = numeroCancion,
                            ID_ARTISTA = artistaID,
                            ID_ALBUM = albumID,
                            Ruta_Audio = archivo,
                        };
                        db.tb_Cancion.Add(cancion);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View(ex);
                }
            }
            else
            {
                return HttpNotFound("El archivo no fue encontrado");
            }
        }
    }
}