using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unimi.HttpHandler
{
    /// <summary>
    /// Gestisce le HttpWebRequest necessarie per ottenere i sorgenti delle pagine.
    /// </summary>
    public class HttpRequestHandler
    {
        /// <summary>
        /// Il valore dell'header HTTP Content-Type.
        /// </summary>
        private const string ContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Il contenitore dei cookies della sessione.
        /// </summary>
        private CookieContainer _cookieContainer;

        /// <summary>
        /// Crea una nuova istanza della classe
        /// </summary>
        public HttpRequestHandler()
        {
            // Inizializza il contenitore dei cookies.
            _cookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Crea una nuova <see cref="HttpWebRequest"/> ed invia i dati all'url specificato in modo asincrono.
        /// </summary>
        /// <param name="postData">La <see cref="string"/> contenente il corpo della richiesta da inviare al sito.</param>
        /// <param name="url">L'indirizzo web della richiesta.</param>
        /// <param name="token">Il <see cref="CancellationToken"/> necessario alla segnalazione dell'annullamento dell'operazione.</param>
        /// <returns>Il System.Net.HttpWebResponse della richiesta.</returns>
        public async Task<HttpWebResponse> PostDataAsync(string url, string postData, CancellationToken token = default)
        {
            // Crea la HttpWebRequest
            var request = (HttpWebRequest)WebRequest.Create(url);

            // Ottieni i byte della stringa da inviare
            var data = Encoding.UTF8.GetBytes(postData);

            // Imposta le proprietà della HttpWebRequest
            request.Method = "POST";
            //request.ContentLenght = data.Length;
            request.ContentType = ContentType;
            request.CookieContainer = _cookieContainer;

            // Ottieni lo stream per inviare i dati
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                // Invia i dati in modo asincrono
                await requestStream.WriteAsync(data, 0, data.Length, token);
            }

            // Ottieni e ritorna la risposta della HttpWebRequest
            return await request.GetResponseAsync(token);
        }

        /// <summary>
        /// Ottieni in modo asincrono lo <see cref="Stream"/> della risposta dell'<paramref name="url"/> specificato.
        /// </summary>
        /// <param name="url">L'indirizzo web della richiesta.</param>
        /// <param name="token">Il <see cref="CancellationToken"/> necessario alla segnalazione dell'annullamento dell'operazione.</param>
        /// <returns>Uno System.IO.Stream contenente la risposta della HttpWebRequest.</returns>
        public async Task<Stream> GetResponseStreamAsync(string url, CancellationToken token = default)
        {
            // Crea la HttpWebRequest
            var request = (HttpWebRequest)WebRequest.Create(url);

            // Imposta le proprietà della HttpWebRequest
            request.Method = "GET";
            request.ContentType = ContentType;
            request.CookieContainer = _cookieContainer;

            // Ottieni la risposta della HttpWebRequest
            var response = await request.GetResponseAsync(token);

            // Ritorna lo Stream contenente la risposta.
            // Servirà per inizializzare un HtmlDocument da analizzare
            // per ottenere le informazioni
            return response.GetResponseStream();
        }

        /// <summary>
        /// Ottieni in modo asincrono il contenuto dello <see cref="Stream"/> di risposta. 
        /// </summary>
        /// <param name="stream">Lo <see cref="Stream"/> che contiene la risposta.</param>
        /// <returns>Uno System.IO.Stream</returns>
        public async Task<string> GetResponseAsync(Stream stream)
        {
            // Crea uno StreamReader per ottenere il contento dello Stream come stringa
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Leggi e ritorna il contenuto dello Stream
                return await reader.ReadToEndAsync();
            }
        }
    }

    /// <summary>
    /// Contiene alcune estensioni al .NET Framework
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// When overridden in a descendant class, returns a response to an Internet request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The <see cref="HttpWebRequest"/> to cancel if requested.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that handles cancellation.</param>
        /// <returns>A System.Net.HttpWebResponse containing the result of the HttpWebRequest.</returns>
        /// <remarks>Code taken from https://stackoverflow.com/a/19215782. All rights reserved</remarks>
        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken token)
        {
            // Register the abort request method to the cancellation token
            using (token.Register(request.Abort, useSynchronizationContext: false))
            {
                try
                {
                    // Get the response async
                    var response = await request.GetResponseAsync();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly
                    if (token.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, token);
                    }

                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}
