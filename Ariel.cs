using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unimi.HttpHandler;

namespace Unimi.Ariel
{
    /// <summary>
    /// Contiene le funzionalità necessarie ad estrapolare i dati dalla piattaforma Ariel.
    /// </summary>
    public class ArielSession
    {
        /// <summary>
        /// Necessario per mantenere i cookies della sessione.
        /// </summary>
        private HttpRequestHandler _authHandler;

        /// <summary>
        /// Crea una nuova istanza della classe.
        /// </summary>
        /// <param name="username">Lo username della matricola (tipicamente "nome.cognome") senza il dominio.</param>
        /// <param name="password">La password della matricola.</param>
        /// <param name="tipoUtente">La tipologia di utente (@studenti.unimi.it, @unimi.it ecc...).</param>
        public ArielSession(string username, string password, string tipoUtente)
        {
            Username = username;
            Password = password;
            TipoUtente = tipoUtente;

            // Inizializza la classe che gestisce le richieste Http
            _authHandler = new HttpRequestHandler();
        }

        /// <summary>
        /// Ottieni lo username dell'utente che ha effettuato l'accesso.
        /// </summary>
        private string Username { get; }

        /// <summary>
        /// Ottieni la password dell'utente che ha effettuato l'accesso.
        /// </summary>
        private string Password { get; }

        /// <summary>
        /// Ottieni la tipologia di utente che ha effettuato l'accesso (@studenti.unimi.it, @unimi.it ecc...).
        /// </summary>
        private string TipoUtente { get; }

        /// <summary>
        /// Effettua il login in maniera asincrona indicando se l'operazione è andata a buon fine.
        /// </summary>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una System.String contenente il risultato del login.</returns>
        public async Task<string> LoginAsync(CancellationToken token = default)
        {
            try
            {
                // Effettua una richiesta Http con metodo post e ottieni la risposta
                var response = await _authHandler.PostDataAsync(Urls.Login, "hdnSilent=true&tbLogin=" + Username + "&tbPassword=" + Password + "&ddlType=" + TipoUtente, token);

                // Analizza lo stato
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Ottieni i sorgenti come stringa della pagina richiesta
                    var content = await _authHandler.GetResponseAsync(response.GetResponseStream());

                    // Ritorna la string che indica se le credenziali erano corrette
                    return !content.Contains("Nome utente e/o password non sono corretti") ? "OK" : "Nome utente e/o password non corretti";
                }
                else
                {
                    // Lo stato della risposta era diverso da OK.
                    // Ritorna perciò la descrizione
                    return response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Scarica il codice sorgente della pagina indicata dall'<paramref name="url"/>.
        /// </summary>
        /// <param name="url">L'indirizzo della pagina da scaricare.</param>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Un HtmlAgilityPack.HtmlDocument contenente il sorgente della pagina web.</returns>
        private async Task<HtmlDocument> GetHtmlPageSource(string url, CancellationToken token = default)
        {
            // Inizializza il documento Html
            var document = new HtmlDocument();

            // Ottieni lo Stream della risposta della HttpWebRequest
            // e caricalo nel documento Html
            document.Load(await _authHandler.GetResponseStreamAsync(url, token), Encoding.UTF8);

            return document;
        }

        /// <summary>
        /// Ottieni il contenuto della pagina Ariel in modo asincrono.
        /// </summary>
        /// <param name="url">L'indirizzo della pagina.</param>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una Unimi.ArielSession.Page che contiene gli ambienti e i sottoambienti.</returns>
        public async Task<Page> GetPageContentAsync(string url, CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var documentSource = await GetHtmlPageSource(url, token);

            // Ottieni gli ambienti della pagina
            var rooms = GetRoomElements(documentSource);

            // Ottieni i sottoambienti della pagina
            var threads = GetThreadElements(documentSource);

            // Crea e ritorna la pagina Ariel
            return new Page(rooms, threads);
        }

        /// <summary>
        /// Ottieni gli ambienti della pagina Ariel in modo asincrono.
        /// </summary>
        /// <param name="documentSource">Il documento Html sorgente.</param>
        /// <returns>Una System.Collections.Generic.List che contiene Unimi.Ariel.ArielSession.RoomElement.</returns>
        private static List<Page.RoomElement> GetRoomElements(HtmlDocument documentSource)
        {
            // Inizializza la lista
            var roomElements = new List<Page.RoomElement>();

            // Controlla che vi sia la tabella con gli ambienti,
            // altrimenti ritorna la lista vuota.
            if (documentSource.DocumentNode.Descendants()
                .FirstOrDefault(x => 
                x.Name == "tbody" && 
                x.HasAttributes && 
                x.Attributes["class"].Value.StartsWith("arielRoomList")) == null) return roomElements;

            // Ottieni la lista dei nodi Html contenenti gli ambienti
            var lines = documentSource.DocumentNode.Descendants()
                .FirstOrDefault(x => 
                x.Name == "tbody" && 
                x.Attributes["class"].Value.StartsWith("arielRoomList"))
                ?.Descendants("tr")
                .Where(x => x.HasAttributes && x.Attributes["id"].Value.StartsWith("room"))
                .ToList();

            // Se la lista è nulla, ritorna la lista vuota
            if (lines == null) return roomElements;

            // Processa la lista
            foreach (var item in lines)
            {
                // Ottieni il link dell'ambiente
                var link = item.Descendants("a").FirstOrDefault()?.Attributes["href"].Value;

                // Ottieni il nome dell'ambiente
                var name = System.Net.WebUtility.HtmlDecode(item.Descendants("a").FirstOrDefault()?.InnerText
                    .Trim());

                // Ottieni la descrizione dell'ambiente
                var description =
                    System.Net.WebUtility.HtmlDecode(item.Descendants("span").LastOrDefault()?.InnerText.Trim());

                // Ottieni i nodi Html che contengono i dati sul numero di sottoambienti
                var subElements = item.Descendants("small").FirstOrDefault()?.InnerText
                    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                // Ottieni il numero totale di elementi.
                // Imposta 0 se non è stato possibile ottenere i nodi Html
                var totalElements = (subElements != null)
                    ? Convert.ToInt32(new string(subElements?.FirstOrDefault()?.Cast<char>().Where(char.IsNumber).ToArray()))
                    : 0;

                // Ottieni il numero di elementi non letti
                // Imposta 0 se non è stato possibile ottenere i nodi Html.
                var unreadElements = (subElements?.Length > 1)
                    ? Convert.ToInt32(new string(subElements[1].Cast<char>().Where(char.IsNumber).ToArray()))
                    : 0;

                // Aggiungi l'ambiente alla lista
                roomElements.Add(new Page.RoomElement(name, description, link, totalElements, unreadElements));
            }

            return roomElements;
        }

        /// <summary>
        /// Ottieni i thread dell'ambiente della pagina Ariel.
        /// </summary>
        /// <param name="documentSource">Il documento Html sorgente.</param>
        /// <returns>Una System.Collections.Generic.List che contiene Unimi.Ariel.ArielSession.ThreadElement.</returns>
        private static List<Page.ThreadElement> GetThreadElements(HtmlDocument documentSource)
        {
            // Inizializza la lista
            var threadElements = new List<Page.ThreadElement>();

            // Ottieni la lista dei nodi Html contenenti i sottoambienti
            var lines = documentSource.DocumentNode.Descendants()
                .FirstOrDefault(x => 
                x.Name == "tbody" && 
                x.HasAttributes && 
                x.Attributes["id"].Value.Contains("threadList"))
                ?.Descendants("tr")
                .Where(x => x.HasAttributes && x.Attributes["id"].Value.StartsWith("msg")).ToList();

            // Se la lista è nulla, ritorna la lista vuota
            if (lines == null || lines.Count == 0) return threadElements;

            // Processa la lista
            foreach (var item in lines)
            {
                // Ottieni il nome del sottoambiente
                var name = item.Descendants()
                    .FirstOrDefault(x => x.Name == "h2" && x.Attributes["class"].Value == "arielTitle")
                    ?.Descendants("span")
                    .LastOrDefault()?.InnerText;

                // Ottieni la descrizione del sottoambiente
                var description = (item.Descendants()
                    .FirstOrDefault(x => x.Name == "div" && x.HasAttributes && x.Attributes["class"].Value == "arielMessageBody") != null) 
                    ? System.Net.WebUtility.HtmlDecode(item.Descendants()
                    .FirstOrDefault(x => x.Name == "div" && x.HasAttributes && x.Attributes["class"].Value == "arielMessageBody")?.Descendants()
                    .FirstOrDefault(x => x.Name == "span" && x.Attributes["class"].Value == "postbody")?.InnerText) 
                    : string.Empty;

                // Inizializza la lista degli allegati
                var attachments = new List<Page.ThreadElement.ThreadAttachment>();

                // Ottieni il valore che indica se il nodo html che contiene tutti gli allegati esiste
                var hasAttachments = item.Descendants().FirstOrDefault(x => x.Name == "div" && x.HasAttributes && x.Attributes["class"].Value == "arielAttachmentBox");

                // // Controlla che esista il nodo html
                if (hasAttachments != null)
                {
                    // Ottieni la lista di nodi Html che contengono gli allegati
                    var attachmentsNodesList = item.Descendants().FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value == "arielAttachmentBox")
                        ?.Descendants("tr").ToList();

                    // Controlla che esistano allegati
                    if (attachmentsNodesList != null && attachmentsNodesList.Count > 0)
                    {
                        // Processa gli allegati
                        foreach (var attachment in attachmentsNodesList)
                        {
                            // Ottieni il link dell'allegato
                            var link = attachment.Descendants("a").FirstOrDefault()?.Attributes["href"].Value.Trim();

                            // Ottieni il nome dell'allegato
                            var fileName = System.Net.WebUtility.HtmlDecode(attachment.Descendants("a").FirstOrDefault()?.InnerText.Trim());

                            // Ottieni la descrizione dell'allegato
                            var fileDescription =
                                System.Net.WebUtility.HtmlDecode(attachment.Descendants("span").LastOrDefault()
                                    ?.InnerText.Trim());

                            // Aggiungi l'allegato alla lista
                            attachments.Add(new Page.ThreadElement.ThreadAttachment(fileName, link, fileDescription));
                        }
                    }
                }               

                // Ottieni i nodi Html che contengono lo storico delle modifiche
                var editNodes = item.Descendants().FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value.StartsWith("arielInfoPanel"))
                    ?.Descendants().FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value == "panel-body")
                    ?.Descendants("ol").FirstOrDefault()?.Descendants("li").ToList();

                // Inizializza la lista delle modifiche
                var editHistory = new List<Page.ThreadElement.ThreadEdit>();

                // Controlla che vi siano state modifiche nel sottoambiente
                if (editNodes != null && editNodes.Count > 0)
                {
                    // Processa la lista dei nodi contenenti le modifiche
                    foreach (var edit in editNodes)
                    {
                        // Ottieni l'array contenente data e autore della modifica
                        var editText = System.Net.WebUtility.HtmlDecode(edit.InnerText)
                            ?.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                        // Ottieni la data della modifica dall'array
                        var editDate = editText?.FirstOrDefault()?.Trim().Replace(".", ":");

                        // Ottieni l'autore della modifica dall'array
                        var editAuthor = editText?[1].Trim();

                        // Aggiungi la modifica alla lista
                        editHistory.Add(new Page.ThreadElement.ThreadEdit(editAuthor, editDate));
                    }
                }

                // Ottieni l'autore del sottoambiente
                var author = System.Net.WebUtility.HtmlDecode(item.Descendants()
                    .FirstOrDefault(
                        x => x.Name == "div" && x.Attributes["class"].Value.StartsWith("arielInfoPanel"))
                    ?.Descendants()
                    .FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value == "panel-body")
                    ?.Descendants("p").FirstOrDefault()?.Descendants("a").FirstOrDefault()?.InnerText);

                // Ottieni la data di creazione del sottoambiente
                var date = DateTime.Parse(System.Net.WebUtility.HtmlDecode(item.Descendants()
                    .FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value.StartsWith("arielInfoPanel"))
                    ?.Descendants()
                    .FirstOrDefault(x => x.Name == "div" && x.Attributes["class"].Value == "panel-body")
                    ?.Descendants("p").ElementAt(1).InnerText.Split(new string[] { "modificato" }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Trim().Replace(".", ":")));

                // Aggiungi il sottoambiente alla lista
                threadElements.Add(new Page.ThreadElement(name, description, author, date, editHistory, attachments));
            }

            return threadElements;
        }

        /// <summary>
        /// Ottieni la lista dell'attività recente riguardante gli insegnamenti preferiti dell'utente.
        /// </summary>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una System.Collections.Generic.List che contiene Unimi.Ariel.ArielSession.RecentActivity</returns>
        public async Task<List<RecentActivity>> GetRecentActivity(CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var documentSource = await GetHtmlPageSource(Urls.AttivitàRecente, token);

            // Inizializza la lista
            var activityList = new List<RecentActivity>();

            // Ottieni la lista dei nodi Html contenenti gli interventi
            var lines = documentSource.DocumentNode.Descendants()
                .FirstOrDefault(x =>
                x.Name == "table" && 
                x.HasAttributes && 
                x.Attributes["class"].Value == "table")
                ?.Descendants().Where(x => x.Name == "tr").ToList();

            // Se la lista è nulla, ritorna la lista vuota
            if (lines == null || lines.Count == 0) return activityList;

            // Processa la lista
            foreach (var item in lines)
            {
                // Si tratta del titolo che raggruppa le modifiche di uno stesso giorno,
                // quindi passa al prossimo elemento
                if (item.Descendants().Any(x => x.Name == "th" && x.HasAttributes)) continue;

                // Ottieni il nodo Html che contiene la maggior parte degli elementi
                var node = item.Descendants().FirstOrDefault(x => 
                    x.Name == "td" && 
                    x.HasAttributes && 
                    x.Attributes["class"].Value.Contains("col-md-8"));

                // Non abbiamo trovato il nodo principale,
                // passa al prossimo elemento
                if (node == null) continue;

                // Ottieni il nome del thread Ariel
                var titolo = System.Net.WebUtility.HtmlDecode(node.Descendants("span").First().InnerText.Trim());

                // Ottieni l'autore
                var autore = System.Net.WebUtility.HtmlDecode(node.Descendants("strong").First().InnerText.Trim());

                // Ottieni una serie di nodi Html che contengono i rimanenti elementi
                var nodes = node.Descendants("a").ToList();

                // Ottieni il link al thread ArielSession
                var linkTitolo = nodes[0].GetAttributeValue("href", string.Empty);

                // Ottieni il titolo della sezione della pagina Ariel
                var strumento = System.Net.WebUtility.HtmlDecode(nodes[1].InnerText.Trim());

                // Ottieni il link della sezione della pagina Ariel
                var linkStrumento = nodes[1].GetAttributeValue("href", string.Empty);

                // Ottieni il titolo dell'ambiente in cui è stata fatta la modifica
                var ambiente = System.Net.WebUtility.HtmlDecode(nodes[2].InnerText.Trim());

                // Ottieni il link dell'ambiente in cui è stata fatta la modifica
                var linkAmbiente = nodes[2].GetAttributeValue("href", string.Empty);

                // Ottieni i nodi Html che contengono la data e gli allegati
                nodes = node.Descendants().Where(x => x.Name == "span" && x.HasAttributes && x.Attributes["class"].Value == "tag bg-tag-grey").ToList();

                // Ottieni il numero di allegati
                var allegati = (nodes.Count == 2) ? Convert.ToInt32(new string(nodes[0].InnerText.Cast<char>().Where(char.IsNumber).ToArray())) : 0;

                // Ottieni la data della modifica
                var data = nodes[nodes.Count - 1].Descendants("strong").FirstOrDefault()?.InnerText.Trim();

                // Ottieni il nome del sito Ariel
                var nome = System.Net.WebUtility.HtmlDecode(item.Descendants().FirstOrDefault(x =>
                        x.Name == "td" &&
                        x.HasAttributes &&
                        x.Attributes["class"].Value.Contains("col-md-3"))
                        ?.Descendants("a").FirstOrDefault()?.InnerText);

                // Ottieni il link al sito Ariel
                var link = item.Descendants().FirstOrDefault(x => 
                    x.Name == "td" && 
                    x.HasAttributes && 
                    x.Attributes["class"].Value.Contains("col-md-3"))
                    ?.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);

                // Aggiungi l'attività alla lista
                activityList.Add(new RecentActivity(nome, link, titolo, linkTitolo, autore, allegati, data, strumento, linkStrumento, ambiente, linkAmbiente));
            }

            return activityList;
        }

        /// <summary>
        /// Scarica da ariel un file sotto forma di array di byte dall'indirizzo indicato.
        /// </summary>
        /// <param name="url">L'indirizzo del file</param>
        /// <param name="token">Il <see cref="CancellationToken"/> necessario alla segnalazione dell'annullamento dell'operazione.</param>
        /// <returns>Un array di System.Byte</returns>
        public async Task<byte[]> DownloadFileDataAsync(string url, CancellationToken token = default)
        {
            // Ottieni lo stream che contiene il file
            using (var stream = await _authHandler.GetResponseStreamAsync(url, token))
            {
                // Creiamo un MemoryStream per ottenere l'array di byte
                // contenuto nello stream originario
                using (var memoryStream = new MemoryStream())
                {
                    // Copia lo stream nel MemoryStream
                    await stream.CopyToAsync(memoryStream, 81920, token);

                    // Leggi il MemoryStream
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Ottieni la lista degli insegnamenti.
        /// </summary>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una System.Collections.Generic.List contenente gli insegnamenti.</returns>
        public async Task<List<Insegnamento>> GetInsegnamenti(CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var documentSource = await GetHtmlPageSource(Urls.IlMioAriel, token);

            // Inizializza la lista
            var insegnamentiList = new List<Insegnamento>();

            // Ottieni la lista dei nodi html contenenti gli insegnamenti
            var nodeList = documentSource.DocumentNode.Descendants().FirstOrDefault(x => 
                x.Name == "table" && 
                x.Attributes["class"].Value == "table")?.Descendants().Where(x => 
                x.Name == "tr" && 
                x.ParentNode.ParentNode.Attributes["class"].Value == "table")
                .ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodeList == null) return insegnamentiList;

            // Processa la lista
            foreach (var row in nodeList)
            {
                // Ottieni il valore che indica se è attivo un sito per l'insegnamento
                var isSiteActive = row.Descendants()
                    .FirstOrDefault(
                        x => x.Name == "td" && x.HasAttributes && x.Attributes["class"].Value == "col-md-11")?
                    .Descendants().Any(x => x.Name == "table");

                // C'è un sito attivo per l'insegamento
                if (isSiteActive.GetValueOrDefault(false))
                {
                    // Inizializza la variabili
                    var modulo = string.Empty;
                    string edizione;

                    // Ottieni il nodo Html che contiene l'insegnamento
                    var htmlNode = row.Descendants().FirstOrDefault(x => x.Name == "td" && x.Attributes["class"].Value == "col-md-11")?.Descendants("table").FirstOrDefault();

                    // Ottieni il nome dell'insegnamento
                    var nome = System.Net.WebUtility.HtmlDecode(htmlNode?.Descendants("a").FirstOrDefault()?.InnerText);

                    // Ottieni il link dell'insegnamento
                    var link = htmlNode?.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);

                    // Ottieni il valore che indica se c'è il nodo html
                    // che contiene l'edizione e il modulo dell'insegnamento
                    var hasEdizioneModulo = htmlNode?.Descendants().Any(x => x.Name == "span" && x.HasAttributes && x.Attributes["class"].Value == "tag bg-B");

                    // Verifica se c'è il nodo html
                    // che contiene l'edizione e il modulo
                    if (hasEdizioneModulo.GetValueOrDefault(false))
                    {
                        // Ottieni la lista di nodi html che contengono
                        // l'edizioni e il modulo
                        var tabellaEdizioni = htmlNode?.Descendants().Where(x =>
                            x.Name == "span" && x.HasAttributes && x.Attributes["class"].Value == "tag bg-B").ToList();

                        // Ottieni l'edizione
                        edizione = (tabellaEdizioni?.Count > 0) ? tabellaEdizioni?[0].InnerText : string.Empty;

                        // Ottieni il modulo se presente
                        modulo = (tabellaEdizioni?.Count > 1) ? tabellaEdizioni?[1].InnerText : string.Empty;
                    }
                    else
                    {
                        // Ottieni l'edizione
                        edizione = row.Descendants().FirstOrDefault(x =>
                                x.Name == "span" &&
                                x.HasAttributes &&
                                (x.Attributes["class"].Value == "tag pull-right bg-B" || 
                                 x.Attributes["class"].Value == "tag bg-B"))
                            ?.InnerText;
                    }

                    // Ottieni la lista dei titolari dell'insegnamento
                    var listaTitolari = GetTitolari(htmlNode);

                    // Ottieni due tuple contenenti i permessi
                    // e il resto dei dettagli dell'insegnamento
                    var permessi = GetPermessiInsegnamento(htmlNode);
                    var dettagli = GetDettagliInsegnamento(htmlNode);

                    // Ottieni la lista degli insegnamenti correlati
                    var sitiCorrelati = GetSitiCorrelati(row);                   

                    // Aggiungi l'insegnamento alla lista
                    insegnamentiList.Add(new Insegnamento(nome, link, edizione, modulo, listaTitolari, permessi.PuoiAccedere, permessi.Preferito, permessi.AreaPubblica, dettagli.TogglePreferito, sitiCorrelati, dettagli.Attività, dettagli.Scheda));
                }
                else // Nessun sito attivato per l'insegnamento
                {
                    // Ottieni il nome dell'insegnamento
                    var nome = System.Net.WebUtility.HtmlDecode(row.Descendants("strong").ToList()[0].InnerText);

                    // Ottieni l'edizione
                    var edizione = row.Descendants().FirstOrDefault(x =>
                            x.Name == "span" &&
                            x.HasAttributes &&
                            (x.Attributes["class"].Value == "tag pull-right bg-B" ||
                             x.Attributes["class"].Value == "tag bg-B"))
                        ?.InnerText;

                    // Aggiungi l'insegnamento alla lista
                    insegnamentiList.Add(new Insegnamento(nome, string.Empty, edizione, string.Empty, new List<Insegnamento.Titolare>(), true, false, false, string.Empty, new List<Insegnamento.SitoCorrelato>(), string.Empty, string.Empty));
                }
            }

            return insegnamentiList;
        }

        /// <summary>
        /// Ottieni la lista della cronologia degli insegnamenti visitati dall'utente.
        /// </summary>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una System.Collections.Generic.List contenente la cronologia degli insegnamenti.</returns>
        public async Task<List<Insegnamento>> GetCronologiaInsegnamenti(CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var documentSource = await GetHtmlPageSource(Urls.Cronologia, token);

            // Inizializza la lista
            var insegnamentiList = new List<Insegnamento>();

            // Ottieni la lista dei nodi html contenenti gli insegnamenti
            var nodeList = documentSource.DocumentNode.Descendants().FirstOrDefault(x => 
                x.Name == "table" && 
                x.HasAttributes && 
                x.Attributes["class"].Value == "table")
            ?.Descendants().Where(x => 
                x.Name == "tr" && 
                x.HasAttributes)
            .ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodeList == null) return insegnamentiList;

            // Processa la lista
            foreach (var row in nodeList)
            {
                var elements = row.Descendants("td").ToList();

                // Ottieni il nome dell'insegnamento
                var nome = System.Net.WebUtility.HtmlDecode(elements[1].Descendants("a").FirstOrDefault()?.InnerText);

                // Ottieni il link dell'insegnamento
                var link = elements[1].Descendants("a").First().GetAttributeValue("href", string.Empty);
                var ultimoAccesso = System.Net.WebUtility.HtmlDecode(elements[2].InnerText);
                var scheda = elements[4].Descendants("a").First().GetAttributeValue("href", string.Empty);
                var rimuoviLink = elements[4].Descendants("a").ElementAt(1).GetAttributeValue("href", string.Empty);
                var togglePreferito = elements[4].Descendants("a").LastOrDefault()?.GetAttributeValue("href", string.Empty);

                insegnamentiList.Add(new Insegnamento(nome, ultimoAccesso, scheda, rimuoviLink, togglePreferito, link));
            }

            return insegnamentiList;
        }

        /// <summary>
        /// Ottieni la lista degli insegnamenti contrassegnati come preferiti dall'utente.
        /// </summary>
        /// <param name="token">Il <see cref="CancellationToken"/> che gestisce la cancellazione.</param>
        /// <returns>Una System.Collections.Generic.List contenente gli insegnamenti preferiti.</returns>
        public async Task<List<Insegnamento>> GetInsegnamentiPreferiti(CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var documentSource = await GetHtmlPageSource(Urls.Preferiti, token);

            // Inizializza la lista
            var insegnamentiList = new List<Insegnamento>();

            // Ottieni la lista dei nodi html contenenti gli insegnamenti
            var nodeList = documentSource.DocumentNode.Descendants().FirstOrDefault(x => 
                x.Name == "table" && 
                x.HasAttributes && 
                x.Attributes["class"].Value == "table" && 
                x.Attributes["id"].Value == "favoriteTable")
            ?.Descendants().Where(x => 
                x.Name == "tr" && 
                x.HasAttributes)
            .ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodeList == null) return insegnamentiList;

            // Processa la lista
            foreach (var row in nodeList)
            {
                // Ottieni il nodo html che contiene il nome e il link
                var riquadro = row.Descendants().FirstOrDefault(x => 
                    x.Name == "td" && 
                    x.HasAttributes && 
                    x.Attributes["class"].Value == "col-md-11");

                // Ottieni il nome dell'insegnamento
                var nome = (riquadro != null) ? System.Net.WebUtility.HtmlDecode(riquadro.Descendants("a").FirstOrDefault()?.InnerText) : string.Empty;

                // Ottieni il link dell'insegnamento
                var link = (riquadro != null) ? riquadro.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty) : string.Empty;

                // Ottieni la lista dei titolari dell'insegnamento
                var listaTitolari = GetTitolari(row);

                // Ottieni due tuple contenenti i permessi
                // e il resto dei dettagli dell'insegnamento
                var permessi = GetPermessiInsegnamento(row);
                var dettagli = GetDettagliInsegnamento(row);

                // Ottieni la lista degli insegnamenti correlati
                var sitiCorrelati = GetSitiCorrelati(row);

                // Il bool 'Preferito' darà falso poichè le pagine 'Insegnamenti' e 'Preferiti' sono diverse.
                // Poichè esso è inutile in questo contesto, lo ignoriamo.
                insegnamentiList.Add(new Insegnamento(nome, link, listaTitolari, permessi.PuoiAccedere, permessi.AreaPubblica, dettagli.TogglePreferito, sitiCorrelati, dettagli.Attività, dettagli.Scheda));
            }

            return insegnamentiList;
        }

        public async Task<List<Insegnamento>> RicercaSiti(string data, CancellationToken token = default)
        {
            // Ottieni il documento Html che contiene il sorgente della pagina
            var response = await _authHandler.PostDataAsync(Urls.RicercaSiti, "keyword=" + data.Replace(" ", "+"), token);

            // Inizializza la lista
            var insegnamentiList = new List<Insegnamento>();

            // Lo stato della risposta era diverso da OK.
            // Ritorna perciò la lista vuota
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return insegnamentiList;

            // Inizializza il documento Html
            var documentSource = new HtmlDocument();
            documentSource.Load(response.GetResponseStream(), Encoding.UTF8);

            // Ottieni la lista dei nodi html contenenti gli insegnamenti
            var nodeList = documentSource.DocumentNode.Descendants()
                .FirstOrDefault(x => x.Name == "div" && x.HasAttributes && x.Attributes["class"].Value.Contains("tab-content"))
                ?.Descendants().FirstOrDefault(x =>
                    x.Name == "div" && x.HasAttributes && x.Attributes["id"].Value.Contains("sitiariel"))
                ?.Descendants().Where(x =>
                    x.Name == "div" && x.HasAttributes && x.Attributes["class"].Value.Contains("col-md-6")).ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodeList == null) return insegnamentiList;

            // Processa la lista
            foreach (var row in nodeList)
            {
                // Ottieni il link dell'insegnamento
                var link = row.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);

                // Ottieni il nome dell'insegnamento
                var name = System.Net.WebUtility.HtmlDecode(row.Descendants("a").FirstOrDefault()?.InnerText);

                // Ottieni la lista dei titolari dell'insegnamento
                var listaTitolari = GetTitolari(row);

                // Ottieni due tuple contenenti i permessi
                // e il resto dei dettagli dell'insegnamento
                var permessi = GetPermessiInsegnamento(row);
                var dettagli = GetDettagliInsegnamento(row);

                // Ottieni la lista degli insegnamenti correlati
                var sitiCorrelati = GetSitiCorrelati(row);

                // Aggiungi l'insegnamento alla lista
                insegnamentiList.Add(new Insegnamento(name, link, listaTitolari, permessi.PuoiAccedere,
                    permessi.AreaPubblica, dettagli.TogglePreferito, sitiCorrelati, dettagli.Attività,
                    dettagli.Scheda));
            }

            return insegnamentiList;
        }

        /// <summary>
        /// Ottieni la lista degli insegnamenti correlati.
        /// </summary>
        /// <param name="row">Il nodo html che contiene la lista</param>
        /// <returns>Una System.Collections.Generic.List contenente la lista di insegnamenti correlati.</returns>
        private static List<Insegnamento.SitoCorrelato> GetSitiCorrelati(HtmlNode row)
        {
            // Inizializza la lista finale
            var lista = new List<Insegnamento.SitoCorrelato>();

            // Ottieni la lista di nodi html
            var nodesList = row.Descendants().FirstOrDefault(x =>
                    x.Name == "ul" && x.HasAttributes && x.Attributes["class"].Value == "list-unstyled")
                ?.Descendants("li").ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodesList == null) return lista;

            // Processa la lista
            foreach (var node in nodesList)
            {
                // Ottieni il link all'insegnamento correlato
                var sitoLink = node.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);

                // Ottieni il nome dell'insegnamento correlato
                var sitoName = System.Net.WebUtility.HtmlDecode(node.Descendants("a").FirstOrDefault()?.InnerText);

                // Aggiungi alla lista finale
                lista.Add(new Insegnamento.SitoCorrelato(sitoName, sitoLink));
            }

            return lista;
        }

        /// <summary>
        /// Ottieni la lista dei professori titolari dell'insegnamento.
        /// </summary>
        /// <param name="table">Il nodo Html che contiene i titolari</param>
        /// <returns>Una System.Collections.Generic.List contenente i titolari.</returns>
        private static List<Insegnamento.Titolare> GetTitolari(HtmlNode table)
        {
            // Inizializza la lista
            var listaTitolari = new List<Insegnamento.Titolare>();

            // Ottieni la lista dei nodi Html che contengono i titolari
            var nodesList = table.Descendants().FirstOrDefault(x => 
                x.Name == "div" && 
                x.HasAttributes && 
                x.Attributes["class"].Value == "table-view")
                ?.Descendants().FirstOrDefault(x => 
                x.Name == "ul" && 
                x.HasAttributes && 
                x.Attributes["class"].Value == "list-inline list-user")
                ?.Descendants("li").ToList();

            // Se c'è stato un errore,
            // ritorna la lista vuota
            if (nodesList == null) return listaTitolari;

            // Processa la lista dei nodi Html
            foreach (var riga in nodesList)
            {
                // Ottieni il link al profilo ArielSession del titolare
                var linkTitolare = riga.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);

                // Ottieni il nome del titolare
                var nomeTitolare = System.Net.WebUtility.HtmlDecode(riga.Descendants("a").FirstOrDefault()?.InnerText);

                // Aggiungi il titolare alla lista
                listaTitolari.Add(new Insegnamento.Titolare(nomeTitolare, linkTitolare));
            }

            // Ritorna la lista
            return listaTitolari;
        }

        /// <summary>
        /// Ottieni una tupla contenente i boleani relativi ai permessi dell'insegnamento da un nodo Html.
        /// </summary>
        /// <param name="table">Il nodo html da cui ottenere i valori.</param>
        /// <returns>Una tupla di System.Boolean</returns>
        private static (bool PuoiAccedere, bool Preferito, bool AreaPubblica) GetPermessiInsegnamento(HtmlNode table)
        {
            var tabellaPermessi = GetListaDettagliPermessi(table, 0);
            if (tabellaPermessi == null) return (false, false, false);

            bool? puoiAccedere = null, areaPubblica = null, preferito = null;
            foreach (var riga in tabellaPermessi)
            {
                var value = riga.Descendants("span").FirstOrDefault()?.InnerText;
                if (string.IsNullOrEmpty(value)) continue;

                if (!puoiAccedere.HasValue) puoiAccedere = !value.Contains("non puoi accedere");
                if (!areaPubblica.HasValue) areaPubblica = value.Contains("area pubblica");
                if (!preferito.HasValue) preferito = value.Contains("preferito");
            }

            if (!puoiAccedere.HasValue) puoiAccedere = true;
            if (!areaPubblica.HasValue) areaPubblica = false;
            if (!preferito.HasValue) preferito = false;

            return (puoiAccedere.Value, preferito.Value, areaPubblica.Value);
        }

        /// <summary>
        /// Ottieni alcuni dettagli relativi all'insegnamento contenuto nel nodo Html.
        /// </summary>
        /// <param name="table">Il nodo html da cui ottenere i valori.</param>
        /// <returns>Una tupla di System.String</returns>
        private static (string TogglePreferito, string Attività, string Scheda) GetDettagliInsegnamento(HtmlNode table)
        {
            var tabellaLink = GetListaDettagliPermessi(table, 1);
            if (tabellaLink == null) return (string.Empty, string.Empty, string.Empty);

            var scheda = GetLinkDettaglio(tabellaLink, "Scheda");
            var togglePreferito = GetLinkDettaglio(tabellaLink, "preferito");
            var attività = GetLinkDettaglio(tabellaLink, "Attività");

            return (togglePreferito, attività, scheda);
        }

        /// <summary>
        /// Ottieni il link della funzione specificata dell'insegnamento dalla lista di nodi Html.
        /// </summary>
        /// <param name="tabellaLink">La lista di nodi html</param>
        /// <param name="text">La stringa di ricerca della funzione specifica.</param>
        /// <returns>Una System.String contenente il link richiesto.</returns>
        private static string GetLinkDettaglio(IEnumerable<HtmlNode> tabellaLink, string text)
        {
            try
            {
                return tabellaLink.FirstOrDefault(x =>
                {
                    var nome = x.Descendants("a").FirstOrDefault()?.InnerText;
                    return nome != null && System.Net.WebUtility.HtmlDecode(nome).Contains(text);
                })?.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Ottieni la lista di nodi html contenenti i dettagli e i permessi relativi ad un insegnamento.
        /// </summary>
        /// <param name="table">Il nodo html da cui estrapolare i contenuti</param>
        /// <param name="index">0 se bisogna ottenere la lista dei permessi, 1 se bisogna ottenere quella dei dettagli</param>
        /// <returns>Una System.Collections.Generic.List</returns>
        private static List<HtmlNode> GetListaDettagliPermessi(HtmlNode table, int index)
        {
            return table.Descendants().FirstOrDefault(x =>
                    x.Name == "div" &&
                    x.HasAttributes &&
                    x.Attributes["class"].Value == "project-details")
                ?.Descendants("ul").ToList()[index].Descendants("li").ToList();
        }
    }
}
