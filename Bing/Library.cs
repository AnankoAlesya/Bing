using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static bool Add_Book(int key, string name, string author, DateTime date_of_publishing, int number_of_copies/*, int kol_vo_vsego, int kol_vo_vzyat*/)
        {
            foreach (var book in books)
            {
                if (book.Key == key) return false;

            }
            Books book1 = new Books(key, name, author, date_of_publishing, number_of_copies/*, kol_vo_vsego, kol_vo_vzyat*/);
            Books.Add(book1);
            return true;
        }
        public static void Delete_Book(int key)
        {
            for (int j = 0; j < books.Count; j++)
            {
                if (books[j].Key == key)
                {
                    if (books[j].Number_of_copies < 1)
                    {
                        Books.Remove(books[j]);
                    }
                    else
                    {
                        books[j].Number_of_copies--;
                    }

                    break;
                }
            }

        }


        public static void Add_To_Library(Dictionary<int, Clients> clients, int key, Clients client)
        {
            if (!clients.ContainsKey(key)) clients.Add(key, client);
        }


        //public static  isHandBook(Books book, Clients client)
        //{
        //    if (book.Key && client.Value.Carta.Number)
        //        return false;
        //}

        public static void Add_Client(Clients client)
        {

            var keys = clients.Keys;
            var last = keys.Last();
            clients.Add(last + 1, client);
        }


        public static Books GetBookByName(string name)
        {
            foreach (var book in Books)
            {
                if (book.name.Equals(name))
                    return book;
            }

            return null;
        }
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
                    if (book.Number_of_copies > 0) { book.Number_of_copies -= 1; }
                    if (book.Number_of_copies == 0) Library.Delete_Book(book.Key);/////////////
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

        private static bool Pass(Books book, Clients client)
        {
            bool flag = false;
            DateTime returning_book = DateTime.Today;
            foreach (var book1 in Library.books)
            {
                if (clients.ContainsKey(book.Key))
                {

                    if (book.Key == book1.Key)
                    {
                        book.Number_of_copies += 1;
                        foreach (var recording in client.Carta.Recordings)
                        {
                            if (recording.Books.Name.Equals(book.Name))
                            {
                                recording.IsAvailable = true;
                                flag = false;
                            }
                        }
                    }
                }
            }

            return flag;
        }
        public static void Pass_Book(Books books, Clients client)
        {
            if (!Pass(books, client))
            {
                Library.Add_Book(books.Key, books.name, books.author, books.date_of_publishing, books.Number_of_copies + 1);
                Library.Pass(books, client);
            }
        }

        public static bool IsTrue(string number)
        {
            foreach (var client in clients)
            {
                if (client.Value.Carta.Number.Equals(number))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SerializeResult()
        {
            Serialization.Serialisation(clients, books);
        }
    }
}
