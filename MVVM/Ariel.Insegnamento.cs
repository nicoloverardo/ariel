using System.Collections.Generic;

namespace Unimi.Ariel
{
    /// <summary>
    /// Rappresenta un insegnamento della piattaforma Ariel.
    /// </summary>
    public class Insegnamento
    {

        /// <summary>
        /// Specifica la tipologia di insegnamento che contiene una istanza <see cref="Insegnamento"/>
        /// </summary>
        public enum TipologiaInsegnamento { Cronologia, Preferiti, RicercaInsegnamento, InsegnamentoCompleto }

        /// <summary>
        /// Crea una nuova istanza della classe inizializzando i parametri necessari ad contenere i dati di un insegnamento nella pagina "Cronologia Insegnamenti".
        /// </summary>
        /// <param name="nome">Il nome dell'insegnamento.</param>
        /// <param name="ultimoAccesso">La data dell'ultimo accesso.</param>
        /// <param name="scheda">Il link alla pagina della facoltà che contiene la scheda dell'insegnamento.</param>
        /// <param name="rimuoviLink">Il link necessario a rimuovere l'insegnamento dalla cronologia.</param>
        /// <param name="togglePreferito">Il link necessario ad aggiungere o rimuovere l'insegnamento alla lista dei preferiti.</param>
        /// <param name="link">Il link al sito ArielSession CTU dell'insegnamento</param>
        public Insegnamento(string nome, string ultimoAccesso, string scheda, string rimuoviLink, string togglePreferito, string link)
        {
            Nome = nome;
            Link = link;
            Scheda = scheda;
            TogglePreferito = togglePreferito;
            UltimoAccesso = ultimoAccesso;
            RimuoviLink = rimuoviLink;
            Tipologia = TipologiaInsegnamento.Cronologia;
        }

        /// <summary>
        /// Crea una nuova istanza della classe inizializzando i parametri necessari ad contenere i dati di un insegnamento nella pagina "Preferiti".
        /// </summary>
        /// <param name="nome">Il nome dell'insegnamento.</param>
        /// <param name="link">Il link al sito ArielSession CTU dell'insegnamento.</param>
        /// <param name="titolari">La lista dei titolari dell'insegnamento.</param>
        /// <param name="puoiAccedere">Il booleano che indica se l'utente può accedere al sito ArielSession CTU.</param>
        /// <param name="areaPubblica">Il booleano che indica se il sito ArielSession CTU è un'area pubblica.</param>
        /// <param name="togglePreferito">Il link necessario ad aggiungere o rimuovere l'insegnamento alla lista dei preferiti.</param>
        /// <param name="sitiCorrelati">La lista contenente i link agli insegnamenti correlati.</param>
        /// <param name="attività">Il link alla pagina contenente le attività recenti riguardanti l'insegnamento.</param>
        /// <param name="scheda">Il link alla pagina della facoltà che contiene la scheda dell'insegnamento.</param>
        public Insegnamento(string nome, string link, List<Titolare> titolari, bool puoiAccedere, bool areaPubblica, string togglePreferito, List<SitoCorrelato> sitiCorrelati, string attività, string scheda)
        {
            Nome = nome;
            Link = link;
            Scheda = scheda;
            TogglePreferito = togglePreferito;
            Titolari = titolari;
            PuoiAccedere = puoiAccedere;
            AreaPubblica = areaPubblica;
            SitiCorrelati = sitiCorrelati;
            Attività = attività;
            Tipologia = TipologiaInsegnamento.Preferiti;
        }

        /// <summary>
        /// Crea una nuova istanza della classe inizializzando i parametri necessari ad contenere i dati di un insegnamento nella pagina "Ricerca siti".
        /// </summary>
        /// <param name="preferito">Il booleano che indica se l'insegnamento è tra gli insegnamenti preferiti dell'utente.</param>
        /// <param name="nome">Il nome dell'insegnamento.</param>
        /// <param name="link">Il link al sito ArielSession CTU dell'insegnamento.</param>
        /// <param name="titolari">La lista dei titolari dell'insegnamento.</param>
        /// <param name="puoiAccedere">Il booleano che indica se l'utente può accedere al sito ArielSession CTU.</param>
        /// <param name="areaPubblica">Il booleano che indica se il sito ArielSession CTU è un'area pubblica.</param>
        /// <param name="togglePreferito">Il link necessario ad aggiungere o rimuovere l'insegnamento alla lista dei preferiti.</param>
        /// <param name="sitiCorrelati">La lista contenente i link agli insegnamenti correlati.</param>
        /// <param name="attività">Il link alla pagina contenente le attività recenti riguardanti l'insegnamento.</param>
        /// <param name="scheda">Il link alla pagina della facoltà che contiene la scheda dell'insegnamento.</param>
        public Insegnamento(string nome, string link, List<Titolare> titolari, bool puoiAccedere, bool preferito, bool areaPubblica, string togglePreferito, List<SitoCorrelato> sitiCorrelati, string attività, string scheda)
        {
            Nome = nome;
            Link = link;
            Scheda = scheda;
            TogglePreferito = togglePreferito;
            Titolari = titolari;
            PuoiAccedere = puoiAccedere;
            AreaPubblica = areaPubblica;
            SitiCorrelati = sitiCorrelati;
            Attività = attività;
            Preferito = preferito;
            Tipologia = TipologiaInsegnamento.RicercaInsegnamento;
        }

        /// <summary>
        /// Crea una nuova istanza della classe.
        /// </summary>
        /// <param name="edizione">L'edizione dell'insegnamento.</param>
        /// <param name="modulo">Il modulo dell'insegnamento.</param>
        /// <param name="preferito">Il booleano che indica se l'insegnamento è tra gli insegnamenti preferiti dell'utente.</param>
        /// <param name="nome">Il nome dell'insegnamento.</param>
        /// <param name="link">Il link al sito ArielSession CTU dell'insegnamento.</param>
        /// <param name="titolari">La lista dei titolari dell'insegnamento.</param>
        /// <param name="puoiAccedere">Il booleano che indica se l'utente può accedere al sito ArielSession CTU.</param>
        /// <param name="areaPubblica">Il booleano che indica se il sito ArielSession CTU è un'area pubblica.</param>
        /// <param name="togglePreferito">Il link necessario ad aggiungere o rimuovere l'insegnamento alla lista dei preferiti.</param>
        /// <param name="sitiCorrelati">La lista contenente i link agli insegnamenti correlati.</param>
        /// <param name="attività">Il link alla pagina contenente le attività recenti riguardanti l'insegnamento.</param>
        /// <param name="scheda">Il link alla pagina della facoltà che contiene la scheda dell'insegnamento.</param>
        public Insegnamento(string nome, string link, string edizione, string modulo, List<Titolare> titolari, bool puoiAccedere, bool preferito, bool areaPubblica, string togglePreferito, List<SitoCorrelato> sitiCorrelati, string attività, string scheda)
        {
            Nome = nome;
            Link = link;
            Scheda = scheda;
            TogglePreferito = togglePreferito;
            Titolari = titolari;
            PuoiAccedere = puoiAccedere;
            AreaPubblica = areaPubblica;
            SitiCorrelati = sitiCorrelati;
            Attività = attività;
            Edizione = edizione;
            Modulo = modulo;
            Preferito = preferito;
            Tipologia = TipologiaInsegnamento.InsegnamentoCompleto;
        }

        /// <summary>
        /// Ottieni il nome dell'insegnamento.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Ottieni il link al sito ArielSession CTU dell'insegnamento.
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Ottieni il link alla pagina della facoltà che contiene la scheda dell'insegnamento.
        /// </summary>
        public string Scheda { get; }

        /// <summary>
        /// Ottieni il link necessario ad aggiungere o rimuovere l'insegnamento alla lista dei preferiti.
        /// </summary>
        public string TogglePreferito { get; }

        /// <summary>
        /// Ottieni il link alla pagina contente le attività recenti riguardanti l'insegnamento.
        /// </summary>
        public string Attività { get; }

        /// <summary>
        /// Ottieni la data dell'ultimo accesso.
        /// </summary>
        public string UltimoAccesso { get; }

        /// <summary>
        /// Ottieni il link necessario a rimuovere l'insegnamento dalla cronologia.
        /// </summary>
        public string RimuoviLink { get; }

        /// <summary>
        /// Ottieni l'edizione dell'insegnamento.
        /// </summary>
        public string Edizione { get; }

        /// <summary>
        /// Ottieni il modulo dell'insegnamento.
        /// </summary>
        public string Modulo { get; }

        /// <summary>
        /// Ottieni il booleano che indica se l'utente può accedere al sito Ariel CTU.
        /// </summary>
        public bool PuoiAccedere { get; }

        /// <summary>
        /// Ottieni il booleano che indica se il sito ArielSession CTU è un'area pubblica.
        /// </summary>
        public bool AreaPubblica { get; }

        /// <summary>
        /// Ottieni il booleano che indica se l'insegnamento è tra gli insegnamenti preferiti dell'utente.
        /// </summary>
        public bool Preferito { get; }

        /// <summary>
        /// Ottieni la lista contenente i link agli insegnamenti correlati.
        /// </summary>
        public List<SitoCorrelato> SitiCorrelati { get; }

        /// <summary>
        /// Ottieni la lista dei titolari dell'insegnamento.
        /// </summary>
        public List<Titolare> Titolari { get; }

        /// <summary>
        /// Ottieni la tipologia dell'insegnamento rappresentato da questa istanza.
        /// </summary>
        public TipologiaInsegnamento Tipologia { get; }

        /// <summary>
        /// Rappresenta un titolare di un insegnamento.
        /// </summary>
        public class Titolare
        {
            /// <summary>
            /// Crea una nuova istanza della classe.
            /// </summary>
            /// <param name="nome">Il nome del docente titolare.</param>
            /// <param name="link">Il link al profilo del docente titolare.</param>
            public Titolare(string nome, string link)
            {
                Nome = nome;
                Link = link;
            }

            /// <summary>
            /// Ottieni il nome del docente titolare.
            /// </summary>
            public string Nome { get; }

            /// <summary>
            /// Ottieni il link al profilo del docente titolare.
            /// </summary>
            public string Link { get; }

            public override string ToString()
            {
                return Nome; //TODO: remove '+Link' when publishing
            }
        }

        /// <summary>
        /// Rappresenta un insegnamento correlato.
        /// </summary>
        public class SitoCorrelato
        {
            /// <summary>
            /// Crea una nuova istanza della classe.
            /// </summary>
            /// <param name="nome">Il nome del sito correlato.</param>
            /// <param name="link">Il link al sito.</param>
            public SitoCorrelato(string nome, string link)
            {
                Nome = nome;
                Link = link;
            }

            /// <summary>
            /// Ottieni il nome dell'insegnamento correlato.
            /// </summary>
            public string Nome { get; }

            /// <summary>
            /// Ottieni il link al sito correlato.
            /// </summary>
            public string Link { get; }

            public override string ToString()
            {
                return Nome + "\r\n" + Link; //TODO: remove '+Link' when publishing
            }
        }
    }
}
