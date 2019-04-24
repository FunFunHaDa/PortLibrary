using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.IO;
using System.Xml.Linq;

using System.Web.Script.Serialization; //System.Web.Extensions
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library
{
    public partial class Form2 : Form
    {
        ///////////////////////////////////////////
        List<Book> Books_Naver = new List<Book>(); // xml
        IList<Book> Books_Daum = new List<Book>(); // json

        public static bool NaverBool = false;
        public static bool DaumBool = false;

        public Form2()
        {
            InitializeComponent();
            Text = "도서 관리";


            // 그림
            pictureBox1.Size = new System.Drawing.Size(100, 150);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;

            // 데이터 그리드 설정
            dataGridView1.DataSource = DataManager.Books;
            dataGridView1.CurrentCellChanged += DataGridView1_CurrentCellChanged;
            dataGridView2.CurrentCellChanged += DataGridView2_CurrentCellChanged; // 갱신 

            // 버튼 설정
            button1.Click += (sender, e) =>
            {
            // 추가 버튼
            try
                {
                    if (DataManager.Books.Exists((x) => x.Isbn == textBox1.Text)) // Exists(), 리스트에 조건에 맞는 객체가 있는지 확인, 
                {
                        MessageBox.Show("이미 존재하는 도서입니다");
                    }
                    else
                    {
                        Book book = new Book()
                        {
                            Isbn = textBox1.Text,
                            Title = textBox2.Text,
                            Publisher = textBox3.Text,
                            Page = int.Parse(textBox4.Text)
                        };
                        DataManager.Books.Add(book);

                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = DataManager.Books;
                        DataManager.Save();
                    }
                }
                catch (Exception exception)
                {

                }
            };

            button2.Click += (sender, e) =>
            {
            // 수정 버튼
            try
                {
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox1.Text);
                    book.Title = textBox2.Text;
                    book.Publisher = textBox3.Text;
                    book.Page = int.Parse(textBox4.Text);

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = DataManager.Books;
                    DataManager.Save();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("존재하지 않는 도서입니다");
                }
            };

            button3.Click += (sender, e) =>
            {
            // 삭제 버튼
            try
                {
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox1.Text);
                    DataManager.Books.Remove(book);

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = DataManager.Books;
                    DataManager.Save();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("존재하지 않는 도서입니다");
                }
            };
        }

        private void DataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                // 그리드의 셀이 선택되면 텍스트박스에 글자 지정
                Book book = dataGridView1.CurrentRow.DataBoundItem as Book;
                textBox1.Text = book.Isbn;
                textBox2.Text = book.Title;
                textBox3.Text = book.Publisher;
                textBox4.Text = book.Page.ToString();
            }
            catch (Exception exception)
            {

            }
        }
        /////////////////////////////////////////////////////////////////
        private void DataGridView2_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                // 그리드의 셀이 선택되면 텍스트박스에 글자 지정
                Book book = dataGridView2.CurrentRow.DataBoundItem as Book;
                textBox1.Text = book.Isbn;
                textBox2.Text = book.Title;
                textBox3.Text = book.Publisher;
                textBox4.Text = book.Page.ToString();
                pictureBox1.LoadAsync(book.Image);
            }
            catch (Exception exception)
            {

            }
        }

        private void Naver(string query)
        {
            // string query = "이것이"; // 검색할 문자열
            // string url = "https://openapi.naver.com/v1/search/blog?query=" + query; // 결과가 JSON 포맷
            string url = "https://openapi.naver.com/v1/search/book.xml?query=" + query;  // 결과가 XML 포맷
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", ""); // 비공개
            request.Headers.Add("X-Naver-Client-Secret", ""); // 비공개
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();

            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();

                XElement exam = XElement.Parse(text);
                Books_Naver = (from item in exam.Descendants("item")
                               select new Book()
                               {
                                   Isbn = item.Element("isbn").Value.Remove(0, 11),
                                   Title = item.Element("title").Value.Replace("<b>", "").Replace("</b>", ""),
                                   Publisher = item.Element("publisher").Value,
                                   Image = item.Element("image").Value,
                                   Author = item.Element("author").Value,
                                   Page = Int32.Parse(item.Element("price").Value) / 100
                               }).ToList<Book>();

                dataGridView2.DataSource = Books_Naver; // 데이터 소스 연결
            }
            else
            {
                Console.WriteLine("Error 발생=" + status);
            }
        }

        private void Daum(string input)
        {
            int i = 0;
            string site = "https://dapi.kakao.com/v3/search/book.json";
            string query = string.Format("{0}?query={1}", site, input);
            WebRequest request = WebRequest.Create(query);
            string rkey = "";               // 비공개
            string header = "" + rkey;      // 비공개
            request.Headers.Add("Authorization", header);

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            String json = reader.ReadToEnd();
            stream.Close();

            // Console.WriteLine(json);// 전체 출력
            JObject rss = JObject.Parse(json);
            JArray a = (JArray)rss["documents"];

            Books_Daum = (from item in a.ToObject<IList<Book>>()
                          select new Book()
                          {
                              Author = rss["documents"][i]["authors"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Replace(" ", "").Replace("\n", "").Replace(",", ""),
                              Isbn = item.Isbn.Remove(0, 11),
                              Title = item.Title,
                              Publisher = item.Publisher,
                              Image = rss["documents"][i]["thumbnail"].ToString(),
                              Page = (int)(rss["documents"][i++]["price"]) / 100
                          }).ToList<Book>();

            dataGridView2.DataSource = Books_Daum; // 데이터 소스 연결
                                                   // dataGridView2.DataSource = Books_Daum; // 데이터 소스 연결

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (NaverBool == true)
                    Naver(textBox5.Text);

                if (DaumBool == true)
                    Daum(textBox5.Text);
            }
            catch
            {
                MessageBox.Show("잘못된 접근입니다.");

            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            NaverBool = true;
            DaumBool = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            NaverBool = false;
            DaumBool = true;
        }

     
    }
}
