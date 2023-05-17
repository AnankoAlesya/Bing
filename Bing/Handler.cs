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
                    if (Library.Add_Client(client)) response = "Клиент успешно добавлен";
                    else response = "Клиент с таким логином уже существует";
                    break;
                case "AddBook":
                    var book = JsonConvert.DeserializeObject<Books>(jsonRequest.Data);
                    if (Library.Add_Book(book)) response = "Книга успешно добавлена";
                    else response = "Эта книга уже существует в библиотеке";
                    break;
                case "DeleteBook":
                    book = JsonConvert.DeserializeObject<Books>(jsonRequest.Data);
                    if (Library.Delete_Book(book.Key)) response = "Книга удалена успешно";
                    else response = "Книга все ещё есть у клиентов или такой книги в библиотеке нет";
                    break;
                case "AllBooks":
                    response = JsonConvert.SerializeObject(Library.Books);
                    break;
                case "AllClients":
                    response = JsonConvert.SerializeObject(Library.Clients.Values);
                    break;
                case "GetClient":
                    var data = JsonConvert.DeserializeObject<string>(jsonRequest.Data);
                    client = Library.GetClient(data);
                    response = JsonConvert.SerializeObject(client);
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
