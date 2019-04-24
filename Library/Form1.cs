using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = "도서관 관리";

            try//////////////////////////////
            {
            // 라벨 설정
            label5.Text = DataManager.Books.Count.ToString();// 전체 도서수
            label6.Text = DataManager.Users.Count.ToString(); // 사용자 수

          
            // 대출중인 도서의 수
            label7.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString(); // Where() 메서드는 리스트에서 조건에 맞는 대상을 뽑아내는 메서드, isBorrowed 속성이 true인 객체들만 리스트로 뽑아 크기를 구한다
            label8.Text = DataManager.Books.Where((x) =>
            {
                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now; // 대여 기간이 7일입니다. 빌린날짜 + 7일
            }).Count().ToString();

             
            // 데이터 그리드 설정
            dataGridView1.DataSource = DataManager.Books;
                
             dataGridView2.DataSource = DataManager.Users;
    

             /////////////////////////////
            }
            catch// 처음에 파일 없을시 파일 초기화
            {   
                label5.Text = "0";
                label6.Text = "0";
                label7.Text = "0";
                label8.Text = "0";
            }

            //////////////////////////////
            dataGridView1.CurrentCellChanged += DataGridView1_CurrentCellChanged; // 도서 현황
            dataGridView2.CurrentCellChanged += DataGridView2_CurrentCellChanged; // 사용자 현황

            // 버튼 이벤트 설정
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
        }

        private void DataGridView1_CurrentCellChanged(object sender, EventArgs e) // 도서 현황
        {
            try
            {
                Book book = dataGridView1.CurrentRow.DataBoundItem as Book;
                textBox1.Text = book.Isbn;
                textBox2.Text = book.Title;
                // textBox3.Text = book.UserId.ToString();
            }
            catch (Exception exception)
            {

            }
        }

        private void DataGridView2_CurrentCellChanged(object sender, EventArgs e) // 사용자 현황
        {
            try
            {
                // 그리드의 셀이 선택되면 텍스트박스에 글자 지정
                User book = dataGridView2.CurrentRow.DataBoundItem as User;
                textBox3.Text = book.Id.ToString();
            }
            catch (Exception exception)
            {

            }
        }

        private void Button1_Click(object sender, EventArgs e) // 대여
        {

            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요");
            }
            else if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("사용자 Id를 입력해주세요");
            }
            else
            {
                try
                {
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox1.Text);
                    if (book.isBorrowed)
                    {
                        MessageBox.Show("이미 대여 중인 도서입니다.");
                    }
                    else
                    {
                        User user = DataManager.Users.Single((x) => x.Id.ToString() == textBox3.Text); // Single() 메서드는 조건에 맞는 대상 객체 하나를 추출하는 메서드, 조건에 안맞을 때 예외 발생
                        book.UserId = user.Id;
                        book.UserName = user.Name;
                        book.isBorrowed = true;
                        book.BorrowedAt = DateTime.Now;

                        // 그리드를 새로고침, XML로 저장하는 코드
                        Refresh_Form1();
                        DataManager.Save();

                        MessageBox.Show("\"" + book.Title + "\"이/가 \"" + user.Name + "\"님께 대여되었습니다.");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("존재하지 않는 도서 또는 사용자입니다."); // 조건에 맞는 대상이 없다면
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e) // 반납
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요");
            }
            else
            {
                try
                {
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox1.Text);
                    if (book.isBorrowed)
                    {
                        User user = DataManager.Users.Single((x) => x.Id.ToString() == book.UserId.ToString());
                        book.UserId = 0;
                        book.UserName = "";
                        book.isBorrowed = false;
                        book.BorrowedAt = new DateTime();

                     
                        Refresh_Form1();
                        DataManager.Save();

                        if (book.BorrowedAt.AddDays(7) > DateTime.Now)
                        {
                            MessageBox.Show("\"" + book.Title + "\"이/가 연체 상태로 반납되었습니다.");
                        }
                        else
                        {
                            MessageBox.Show("\"" + book.Title + "\"이/가 반납되었습니다.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("대여 상태가 아닙니다.");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("존재하지 않는 도서 또는 사용자입니다.");
                }
            }
        }

        private void Refresh_Form1() // Form1 새로고침함
        {
            // 라벨 설정
            label5.Text = DataManager.Books.Count.ToString();// 전체 도서수
            label6.Text = DataManager.Users.Count.ToString(); // 사용자 수
            // 대출중인 도서의 수
            label7.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString(); // Where() 메서드는 리스트에서 조건에 맞는 대상을 뽑아내는 메서드, isBorrowed 속성이 true인 객체들만 리스트로 뽑아 크기를 구한다
            label8.Text = DataManager.Books.Where((x) =>
            {
                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now; // 대여 기간이 7일입니다. 빌린날짜 + 7일
            }).Count().ToString();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = DataManager.Books;
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = DataManager.Users;
        }

        private void 도서관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void 사용자관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
