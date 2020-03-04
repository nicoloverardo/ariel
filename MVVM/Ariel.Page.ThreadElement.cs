using System;
using System.Collections.Generic;

namespace Unimi.Ariel
{
    public partial class Page
    {
        /// <summary>
        /// Rappresenta un argomento in un ambiente Ariel.
        /// </summary>
        public class ThreadElement
        {
            /// <summary>
            /// Inizializza una nuova istanza di un sottoambiente.
            /// </summary>
            /// <param name="name">Il nome del sottoambiente.</param>
            /// <param name="description">La descrizione del sottoambiente.</param>
            /// <param name="author">L'autore del sottoambiente.</param>
            /// <param name="date">La data di creazione del sottoambiente.</param>
            /// <param name="editHistory">Lo storico delle modifiche del sottoambiente.</param>
            /// <param name="attachments">La lista degli allegati del sottoambiente.</param>
            public ThreadElement(string name, string description, string author, DateTime date, List<ThreadEdit> editHistory, List<ThreadAttachment> attachments)
            {
                Name = name;
                Description = description;
                Author = author;
                Date = date;
                EditHistory = editHistory;
                Attachments = attachments;
            }

            /// <summary>
            /// Il nome del sottoambiente
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// La descrizione del sottoambiente
            /// </summary>
            public string Description { get; }

            /// <summary>
            /// L'autore del sottoambiente
            /// </summary>
            public string Author { get; }

            /// <summary>
            /// La data di creazione del sottoambiente
            /// </summary>
            public DateTime Date { get; }

            /// <summary>
            /// Lo storico delle modifiche del sottoambiente
            /// </summary>
            public List<ThreadEdit> EditHistory { get; }

            /// <summary>
            /// La lista degli allegati del sottoambiente
            /// </summary>
            public List<ThreadAttachment> Attachments { get; }

            /// <summary>
            /// Rappresenta una modifica ad un argomento.
            /// </summary>
            public class ThreadEdit
            {
                /// <summary>
                /// Inizializza una nuova istanza della modifica di un sottoambiente.
                /// </summary>
                /// <param name="author">L'autore della modifica al sottoambiente.</param>
                /// <param name="date">La data della modifica al sottoambiente.</param>
                public ThreadEdit(string author, string date)
                {
                    Author = author;
                    Date = date;
                }

                /// <summary>
                /// L'autore della modifica al sottoambiente
                /// </summary>
                public string Author { get; }

                /// <summary>
                /// La data della modifica al sottoambiente
                /// </summary>
                public string Date { get; }

                /// <summary>
                /// Ottieni la rappresentazione dell'istanza della classe come <see cref="string"/>
                /// </summary>
                /// <returns>Una System.String</returns>
                public override string ToString()
                {
                    return Author + " - " + Date;
                }
            }

            /// <summary>
            /// Rappresenta un allegato di un argomento.
            /// </summary>
            public class ThreadAttachment
            {
                /// <summary>
                /// Inizializza una nuova istanza di un allegato del sottoambiente.
                /// </summary>
                /// <param name="name">Il nome del file allegato.</param>
                /// <param name="description">La descrizione del file allegato.</param>
                /// <param name="link">Il link al file.</param>
                public ThreadAttachment(string name, string description, string link)
                {
                    Name = name;
                    Description = description;
                    Link = link;
                }

                /// <summary>
                /// Il nome del file allegato
                /// </summary>
                public string Name { get; }

                /// <summary>
                /// La descrizione del file allegato
                /// </summary>
                public string Description { get; }

                /// <summary>
                /// Il link al file
                /// </summary>
                public string Link { get; }

                /// <summary>
                /// Ottieni la rappresentazione dell'istanza della classe come <see cref="string"/>
                /// </summary>
                /// <returns>Una System.String</returns>
                public override string ToString()
                {
                    return (string.IsNullOrWhiteSpace(Description)) ? Name + "\r\n" + Link : Name + "\r\n" + Description + "\r\n" + Link; //TODO: Remove '+ Link' when publishing
                }
            }
        }
    }
}
