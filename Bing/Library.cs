using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;

namespace Kurs2 
{
    [Serializable]
    class Library
    {
        private static Dictionary<int, Clients> clients;
        private static List<Books> books;
        public Library(Dictionary<int, Clients> clients1, List<Books> books1)
        {
            clients = clients1;
            books = books1;
        }

        public static List<Books> Books => books;
        public static Dictionary<int, Clients> Clients => clients;

        public static bool Add_Book(Books item)
        {
            if (!ContainsBookInLibrary(item.Key)) return false;
            foreach (var book in books) if (book.Key == item.Key) return false;
            Books.Add(item);
            return true;
        }

        public static bool Delete_Book(int key)
        {
            for (int j = 0; j < books.Count; j++)
            {
                if (books[j].Key == key)
                {
                    if (NoHand(books[j]))
                    {
                        return false;
                    }

                    if (books[j].Number_of_copies <= 1)
                    {
                        Books.Remove(books[j]);
                    }
                    else
                    {
                        books[j].Number_of_copies--;
                    }
                }
            }

            return true;
        }

        public static bool Add_Client(Clients client)
        {
            if (GetClient(client.Carta.Number) != null)
            {
                var keys = clients.Keys;
                var last = keys.Last();
                clients.Add(last + 1, client);
                return true;
            }

            return false;
        }


        //public static Books GetBookByName(string name)
        //{
        //    foreach (var book in Books)
        //    {
        //        if (book.name.Equals(name))
        //            return book;
        //    }

        //    return null;
        //}

        public static bool Get_Book(Clients client, Books book)
        {
            DateTime taking_book = DateTime.Today;
            DateTime returning_book1;

            if (client.Role == Roles.STUDENT)
            {
                returning_book1 = taking_book.AddMonths(1);
            }
            else
            {
                returning_book1 = taking_book.AddYears(1);
            }

            foreach (var book1 in books)
            {
                if (book.Key == book1.Key)
                {
                    Recordings recording = new Recordings(returning_book1, taking_book, book);
                    client.Carta.Recordings.Add(recording);
                    if (book.Number_of_copies > 0) book.Number_of_copies -= 1; 
                    if (book.Number_of_copies == 0) Library.Delete_Book(book.Key);
                    return true;
                }
            }

            return false;
        }

        public static Clients GetClient(string login)
        {
            foreach (var client in clients)
            {
                if (client.Value.Carta.Number.Equals(login))
                {
                    return client.Value;
                }
            }

            return null;
        }

        private static void Pass(Books book, Clients client)
        {
            foreach (var book1 in Library.books)
            {
                if (book.Key == book1.Key)
                {
                    if (book.Number_of_copies >= 0)
                    {
                        book.Number_of_copies += 1;
                        for (int i = 0; i < client.Carta.Recordings.Count; i++)
                        {
                            Recordings recording = client.Carta.Recordings[i];
                            if (recording.Books.Name.Equals(book.Name))
                            {
                                client.Carta.Recordings.Remove(recording);
                                //recording.Books.Number_of_copies -= 1;
                            }
                        }
                    }
                }
            }
        }
        public static void Pass_Book(Books books, Clients client)
        {
            Pass(books, client);
        }

        public static bool NoHand(Books books)
        {
            foreach (var client in clients)
            {
                foreach (var recording in client.Value.Carta.Recordings)
                {
                    if (recording.Books.name.Equals(books.name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //public static bool IsTrue(string number)
        //{
        //    foreach (var client in clients)
        //    {
        //        if (client.Value.Carta.Number.Equals(number))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static bool ContainsBookInLibrary(int key)
        {
            foreach (var book in books)
            {
                if (book.Key == key) return true;
            }

            return false;
        }

        public static void SerializeResult()
        {
            Serialization.Serialisation(clients, books);
        }
    }
}