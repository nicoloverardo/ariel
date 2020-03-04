using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unimi.Ariel
{
    /// <summary>
    /// Rappresenta un intervento in una pagina di un sito ArielSession.
    /// </summary>
    public class RecentActivity
    {
        /// <summary>
        /// Crea una nuova istanza della classe.
        /// </summary>
        /// <param name="nome">Il nome dell'insegnamento.</param>
        /// <param name="link">Il link alla pagina dell'insegnamento.</param>
        /// <param name="titolo">Il titolo del post.</param>
        /// <param name="linkTitolo">Il link alla pagina dell'argomento del post.</param>
        /// <param name="autore">L'autore del post.</param>
        /// <param name="allegati">Il numero di allegati nell'argomento.</param>
        /// <param name="data">La data e l'ora del post.</param>
        /// <param name="strumento">Il nome della sezione in cui si trova il post.</param>
        /// <param name="linkStrumento">Il link alla sezione in cui si trova il post.</param>
        /// <param name="ambiente">L'ambiente in cui si trova il post.</param>
        /// <param name="linkAmbiente">Il link all'ambiente in cui si trova il post.</param>
        public RecentActivity(string nome, string link, string titolo, string linkTitolo, string autore, int allegati, string data, string strumento, string linkStrumento, string ambiente, string linkAmbiente)
        {
            Nome = nome;
            Link = link;
            Titolo = titolo;
            LinkTitolo = linkTitolo;
            Autore = autore;
            Allegati = allegati;
            Data = data;
            Strumento = strumento;
            LinkStrumento = linkStrumento;
            Ambiente = ambiente;
            LinkAmbiente = linkAmbiente;
        }

        /// <summary>
        /// Ottieni il nome dell'insegnamento.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Ottieni il link alla pagina dell'insegnamento.
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Ottieni il titolo del post.
        /// </summary>
        public string Titolo { get; }

        /// <summary>
        /// Ottieni il link alla pagina dell'argomento del post.
        /// </summary>
        public string LinkTitolo { get; }

        /// <summary>
        /// Ottieni l'autore del post.
        /// </summary>
        public string Autore { get; }

        /// <summary>
        /// Ottieni il numero di allegati nell'argomento.
        /// </summary>
        public int Allegati { get; }

        /// <summary>
        /// Ottieni la data e l'ora del post.
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Ottieni il nome della sezione in cui si trova il post.
        /// </summary>
        public string Strumento { get; }

        /// <summary>
        /// Ottieni il link alla sezione in cui si trova il post.
        /// </summary>
        public string LinkStrumento { get; }

        /// <summary>
        /// Ottieni l'ambiente in cui si trova il post.
        /// </summary>
        public string Ambiente { get; }

        /// <summary>
        /// Ottieni il link dell'ambiente in cui si trova il post.
        /// </summary>
        public string LinkAmbiente { get; }
    }
}
