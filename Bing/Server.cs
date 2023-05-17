using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bing
{
    public class Server
    {
        private TcpListener _listener;
        private List<Client> _clients = new List<Client>();
        private List<Books> _books = new List<Books>();

        public async Task StartAsync(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Console.WriteLine("Server started.");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                HandleClientAsync(client);
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();

            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    break;
                }

                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received request: " + request);

                // Parse JSON request
                var jsonRequest = JsonConvert.DeserializeObject<Request>(request);

                string response;

                switch (jsonRequest.Method)
                {
                    case "AddClient":
                        var addClientData = JsonConvert.DeserializeObject<AddClientData>(jsonRequest.Data);
                        AddClient(addClientData.Name, addClientData.Surname, addClientData.Role);
                        response = "Client added successfully.";
                        break;
                    case "AddBook":
                        var addBookData = JsonConvert.DeserializeObject<AddBookData>(jsonRequest.Data);
                        AddBook(addBookData.Key, addBookData.Name, addBookData.Author, addBookData.DateOfPublishing, addBookData.NumberOfCopies);
                        response = "Book added successfully.";
                        break;
                    case "DeleteBook":
                        var deleteBookData = JsonConvert.DeserializeObject<DeleteBookData>(jsonRequest.Data);
                        DeleteBook(deleteBookData.Book);
                        response = "Book deleted successfully.";
                        break;
                    case "AddBookToClient":
                        var addBookToClientData = JsonConvert.DeserializeObject<AddBookToClientData>(jsonRequest.Data);
                        AddBookToClient(addBookToClientData.Login, addBookToClientData.Book);
                        response = "Book added to client successfully.";
                        break;
                    case "TakeBook":
                        var takeBookData = JsonConvert.DeserializeObject<TakeBookData>(jsonRequest.Data);
                        TakeBook(takeBookData.Login, takeBookData.Book);
                        response = "Book taken successfully.";
                        break;
                    default:
                        response = "Unknown method.";
                        break;
                }

                var responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
        }

        private void AddClient(string name, string surname, string role)
        {
            _clients.Add(new Client { Name = name, Surname = surname, Role = role });
        }

        private void AddBook(int key, string name, string author, DateTime dateOfPublishing, int numberOfCopies)
        {
            _books.Add(new Books(key, name, author, dateOfPublishing, numberOfCopies));
        }

        private void DeleteBook(Books book)
        {
            _books.Remove(book);
        }

        private void AddBookToClient(string login, Books book)
        {
            var client = _clients.Find(c => c.Name == login);
            if (client != null)
            {
                client.Books.Add(book);
            }
        }
        private void TakeBook(string login, Books book)
        {
            var client = _clients.Find(c => c.Name == login);
            if (client != null)
            {
                client.Books.Remove(book);
            }
        }
    }

    public class Request
    {
        public string Method { get; set; }
        public string Data { get; set; }
    }

    public class AddClientData
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
    }

    public class AddBookData
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime DateOfPublishing { get; set; }
        public int NumberOfCopies { get; set; }
    }

    public class DeleteBookData
    {
        public Books Book { get; set; }
    }

    public class AddBookToClientData
    {
        public string Login { get; set; }
        public Books Book { get; set; }
    }

    public class TakeBookData
    {
        public string Login { get; set; }
        public Books Book { get; set; }
    }

    public class Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public List<Books> Books { get; set; } = new List<Books>();
    }

    // Your Books class
    public class Books
    {
        private int key;
        public string name { get; }
        public string author { get; }
        public DateTime date_of_publishing { get; }

        private int number_of_copies;

        public Books(int key, string name, string author, DateTime date_of_publishing, int number_of_copies)
        {
            this.key = key;
            this.name = name;
            this.author = author;
            this.date_of_publishing = date_of_publishing;
            this.number_of_copies = number_of_copies;
        }

        public static bool Is_Book_Inf_Valide(int number_of_copies)
        {
            if (number_of_copies <= 0) return false;
            return true;
        }

        public string Name { get { return name; } }
        public int Number_of_copies
        {
            get { return number_of_copies; }
            set { number_of_copies = value; }
        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public override string ToString()
        {
            return string.Format("Номер:{0} Название:{1} Автор:{2} Дата публикации:{3}, Количество оставшихся книг {4}", key, name, author, date_of_publishing.ToShortDateString(), number_of_copies);
        }
    }
}