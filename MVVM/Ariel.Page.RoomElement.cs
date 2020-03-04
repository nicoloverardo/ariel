namespace Unimi.Ariel
{
    public partial class Page
    {
        /// <summary>
        /// Rappresenta un ambiente di una pagina Ariel.
        /// </summary>
        public class RoomElement
        {
            /// <summary>
            /// Inizializza un nuovo ambiente di una pagina Ariel.
            /// </summary>
            /// <param name="name">Il nome dell'ambiente.</param>
            /// <param name="description">La descrzione dell'ambiente.</param>
            /// <param name="link">Il link all'ambiente.</param>
            /// <param name="totalElements">Il numero totale di elementi nell'ambiente.</param>
            /// <param name="unreadElements">Il numero di elementi non letti dall'utente.</param>
            public RoomElement(string name, string description, string link, int totalElements, int unreadElements)
            {
                Name = name;
                Description = description;
                Link = link;
                TotalElements = totalElements;
                UnreadElements = unreadElements;
            }

            /// <summary>
            /// Il nome dell'ambiente Ariel.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// La descrizione dell'ambiente Ariel.
            /// </summary>
            public string Description { get; }

            /// <summary>
            /// Il link dell'ambiente Ariel.
            /// </summary>
            public string Link { get; }

            /// <summary>
            /// Il numero totale di elementi nell'ambiente.
            /// </summary>
            public int TotalElements { get; }

            /// <summary>
            /// Il numero di elementi non letti nell'ambiente.
            /// </summary>
            public int UnreadElements { get; }
        }
    }
}
