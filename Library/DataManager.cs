using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Library
{
class DataManager
{  // 모든 클래스에서 접근할 수 있게 static으로 데이터 리스트를 만든다.
    public static List<Book> Books = new List<Book>();
    public static List<User> Users = new List<User>();

    static DataManager()
    {
            try
            {
                Load();

            }
            catch(Exception)
            {
                SaveInit(); // 파일 없을 때 초기화
            }
    }

    public static void Load()
    {
        try
        {   // 도서 XML을 읽고 파싱
            string booksOutput = File.ReadAllText(@"./Books.xml");
            XElement booksXElement = XElement.Parse(booksOutput);
            Books = (from item in booksXElement.Descendants("book")
                        select new Book()
                        {
                            Isbn = item.Element("isbn").Value,  // 도서번호
                            Title = item.Element("name").Value,  // 책이름
                            Publisher = item.Element("publisher").Value,    
                            Page = int.Parse(item.Element("page").Value),
                            BorrowedAt = DateTime.Parse(item.Element("borrowedAt").Value),
                            isBorrowed = item.Element("isBorrowed").Value != "0" ? true : false, // 삼항조건부 연산자
                            UserId = int.Parse(item.Element("userId").Value),
                            UserName = item.Element("userName").Value
                        }).ToList<Book>();
            
            // 사용자 XML을 읽고 파싱
            string usersOutput = File.ReadAllText(@"./Users.xml");
            XElement usersXElement = XElement.Parse(usersOutput);
            Users = (from item in usersXElement.Descendants("user")
                        select new User()
                        {
                            Id = int.Parse(item.Element("id").Value),
                            Name = item.Element("name").Value
                        }).ToList<User>();
        }
        catch (FileLoadException exception)
        {
            // 파일이 없으면 예외 발생: 새로운 파일 생성
            Save();
        }
    }

    public static void Save()
    {
        // 도서 XML 생성
        string booksOutput = "";
        booksOutput += "<books>\n"; // 처음
        foreach (var item in Books)
        {
            booksOutput += "<book>\n";
            booksOutput += "  <isbn>" + item.Isbn + "</isbn>\n";
            booksOutput += "  <name>" + item.Title + "</name>\n";
            booksOutput += "  <publisher>" + item.Publisher + "</publisher>\n";
            booksOutput += "  <page>" + item.Page + "</page>\n";
            booksOutput += "  <borrowedAt>" + item.BorrowedAt.ToLongDateString() + "</borrowedAt>\n";
            booksOutput += "  <isBorrowed>" + (item.isBorrowed ? 1 : 0) + "</isBorrowed>\n"; // 삼항 조건부 연산자
            booksOutput += "  <userId>" + item.UserId + "</userId>\n";
            booksOutput += "  <userName>" + item.UserName + "</userName>\n";
            booksOutput += "</book>\n";
        }
        booksOutput += "</books>"; // 끝

        // 사용자 XML 생성
        string usersOutput = "";
        usersOutput += "<users>\n";
        foreach (var item in Users)
        {
            usersOutput += "<user>\n";
            usersOutput += "  <id>" + item.Id + "</id>\n";
            usersOutput += "  <name>" + item.Name + "</name>\n";
            usersOutput += "</user>\n";
        }
        usersOutput += "</users>";

        // 저장
        File.WriteAllText(@"./Books.xml", booksOutput);
        File.WriteAllText(@"./Users.xml", usersOutput);
    }


        //////////////////////////////
        public static void SaveInit()
        {
          

            // 저장
            File.WriteAllText(@"./Books.xml", "");
            File.WriteAllText(@"./Users.xml", "");
           
        }

    }
}
