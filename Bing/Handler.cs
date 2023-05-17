using Bing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs2
{
    class Handler
    {
        public static string CommandHandler(Request jsonRequest) {
            string response = string.Empty;
            switch (jsonRequest.Method)
            {
                case "AddClient":
                    var client = JsonConvert.DeserializeObject<Clients>(jsonRequest.Data);
                    Library.Add_Client(client);
                    response = "Client added successfully.";
                    break;
                case "AddBook":
                    var book = JsonConvert.DeserializeObject<Books>(jsonRequest.Data);
                    Library.Add_Book(book.Key, book.Name, book.author, book.date_of_publishing, book.Number_of_copies);
                    response = "Book added successfully.";
                    break;
                case "DeleteBook":
                    book = JsonConvert.DeserializeObject<Books>(jsonRequest.Data);
                    Library.Delete_Book(book.Key);
                    response = "Book deleted successfully.";
                    break;
                case "GetBooks":
                    response = JsonConvert.SerializeObject(Library.Books);
                    break;
                case "AddBookToClient":
                    var bookClient = JsonConvert.DeserializeObject<(Books,Clients)>(jsonRequest.Data);
                    Library.Get_Book(bookClient.Item2, bookClient.Item1);
                    response = "Book added to client successfully.";
                    break;
                case "PassBook":
                    var takeBookData = JsonConvert.DeserializeObject<(Books,Clients)>(jsonRequest.Data);
                    Library.Pass_Book(takeBookData.Item1, takeBookData.Item2);
                    response = "Book taken successfully.";
                    break;
                default:
                    response = "Unknown method.";
                    break;
            }
            return response;
        }
    }
}
