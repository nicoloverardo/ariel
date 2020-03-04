using System.Collections.Generic;

namespace Unimi.Ariel
{
    /// <summary>
    /// Rappresenta una pagina di un sito Ariel.
    /// </summary>
    public partial class Page
    {
        /// <summary>
        /// Inizializza una nuova pagina del sito Ariel.
        /// </summary>
        /// <param name="rooms">La lista degli ambienti.</param>
        /// <param name="threads">La lista dei sottoambienti.</param>
        public Page(List<RoomElement> rooms, List<ThreadElement> threads)
        {
            Rooms = rooms;
            Threads = threads;
        }

        /// <summary>
        /// La lista degli ambienti.
        /// </summary>
        public List<RoomElement> Rooms { get; }

        /// <summary>
        /// La lista dei sottoambienti.
        /// </summary>
        public List<ThreadElement> Threads { get; }
    }
}
