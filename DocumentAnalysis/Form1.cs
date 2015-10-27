using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using AForge.Imaging.Filters;
using AForge;
using System.Drawing.Imaging;
using AForge.Imaging;

namespace DocumentAnalysis
{    
    public partial class Form1 : Form
    {
        Boolean bHaveMouse;
        System.Drawing.Point ptOriginal = new System.Drawing.Point();
        System.Drawing.Point ptLast = new System.Drawing.Point();

        int startX, startY, width, height;

        public int redMin = 0, redMax = 0;
        public int greenMin = 0, greenMax = 0;
        public int blueMin = 0, blueMax = 0;       
                
        public Form1()
        {
            InitializeComponent();

            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);

            this.colorbar();

         //   this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);

        }

        private void colorbar()
        {
            trackBar1.Maximum = 255;
            trackBar2.Maximum = 255;
            trackBar3.Maximum = 255;
            trackBar4.Maximum = 255;
            trackBar5.Maximum = 255;
            trackBar6.Maximum = 255;

            trackBar1.Minimum = 0;
            trackBar2.Minimum = 0;
            trackBar3.Minimum = 0;
            trackBar4.Minimum = 0;
            trackBar5.Minimum = 0;
            trackBar6.Minimum = 0;

            trackBar1.TickFrequency = 3;
            trackBar2.TickFrequency = 3;
            trackBar3.TickFrequency = 3;
            trackBar4.TickFrequency = 3;
            trackBar5.TickFrequency = 3;
            trackBar6.TickFrequency = 3;
        }

        private void MyDrawReversibleRectangle(System.Drawing.Point p1, System.Drawing.Point p2)
        {
         //   Bitmap myBitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
                      
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);
            Bitmap myBitmap = new Bitmap(Global.source); // pictureBox1.InitialImage 자리에 기준처방전을 넣어야 함(기준 처방전을 스캔한후 불러오도록 할 것)

            using (Graphics g = Graphics.FromImage(myBitmap))
            {             
                g.DrawRectangle(new Pen(Brushes.Red), p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);

                startX = p1.X;
                startY = p1.Y;
                width = p2.X - p1.X;
                height = p2.Y - p1.Y;                               
            }

            this.pictureBox1.Image = myBitmap;
       }

       private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
       {
            System.Drawing.Point ptCurrent = new System.Drawing.Point(e.X, e.Y);
            if (bHaveMouse)
            {
                if (ptLast.X != -1)
                {
                    MyDrawReversibleRectangle(ptOriginal, ptLast);
                }
                ptLast = ptCurrent;
                MyDrawReversibleRectangle(ptOriginal, ptCurrent);
            }
      }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            bHaveMouse = true;
            ptOriginal.X = e.X;
            ptOriginal.Y = e.Y;
            ptLast.X = -1;
            ptLast.Y = -1;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            bHaveMouse = false;
            if (ptLast.X != -1)
            {
                System.Drawing.Point ptCurrent = new System.Drawing.Point(e.X, e.Y);
                MyDrawReversibleRectangle(ptOriginal, ptLast);
            }
            ptLast.X = -1;
            ptLast.Y = -1;
            ptOriginal.X = -1;
            ptOriginal.Y = -1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            textBox1.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("좌표1번", "X"), ini.GetIniValue("좌표1번", "Y"), ini.GetIniValue("좌표1번", "가로"), ini.GetIniValue("좌표1번", "세로"));
            textBox2.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("좌표2번", "X"), ini.GetIniValue("좌표2번", "Y"), ini.GetIniValue("좌표2번", "가로"), ini.GetIniValue("좌표번", "세로"));
            textBox3.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("환자이름", "X"), ini.GetIniValue("환자이름", "Y"), ini.GetIniValue("환자이름", "가로"), ini.GetIniValue("환자이름", "세로"));
            textBox4.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("주민번호", "X"), ini.GetIniValue("주민번호", "Y"), ini.GetIniValue("주민번호", "가로"), ini.GetIniValue("주민번호", "세로"));
            textBox5.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("교부번호", "X"), ini.GetIniValue("교부번호", "Y"), ini.GetIniValue("교부번호", "가로"), ini.GetIniValue("교부번호", "세로"));
            textBox6.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("의사면허", "X"), ini.GetIniValue("의사면허", "Y"), ini.GetIniValue("의사면허", "가로"), ini.GetIniValue("의사면허", "세로"));
            textBox7.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("발행기관", "X"), ini.GetIniValue("발행기관", "Y"), ini.GetIniValue("발행기관", "가로"), ini.GetIniValue("발행기관", "세로"));
            textBox8.Text = String.Format("{0}:{1}:{2}:{3}", ini.GetIniValue("질병기호", "X"), ini.GetIniValue("질병기호", "Y"), ini.GetIniValue("질병기호", "가로"), ini.GetIniValue("질병기호", "세로"));
            textBox14.Text = ini.GetIniValue("좌표텍스트", "좌표1번");
            textBox15.Text = ini.GetIniValue("좌표텍스트", "좌표2번");
            textBox16.Text = ini.GetIniValue("색상필터값", "RedMin");
            textBox17.Text = ini.GetIniValue("색상필터값", "GreenMin");
            textBox18.Text = ini.GetIniValue("색상필터값", "BlueMin");
            textBox19.Text = ini.GetIniValue("색상필터값", "RedMax");
            textBox20.Text = ini.GetIniValue("색상필터값", "GreenMax");
            textBox21.Text = ini.GetIniValue("색상필터값", "BlueMax");
            trackBar1.Value = int.Parse(ini.GetIniValue("색상필터값", "RedMin"));            
            trackBar2.Value = int.Parse(ini.GetIniValue("색상필터값", "RedMax"));
            trackBar3.Value = int.Parse(ini.GetIniValue("색상필터값", "GreenMin"));
            trackBar4.Value = int.Parse(ini.GetIniValue("색상필터값", "GreenMax"));
            trackBar5.Value = int.Parse(ini.GetIniValue("색상필터값", "BlueMin"));
            trackBar6.Value = int.Parse(ini.GetIniValue("색상필터값", "BlueMax"));
            trackBar1.Invalidate();
            trackBar2.Invalidate();
            trackBar3.Invalidate();
            trackBar4.Invalidate();
            trackBar5.Invalidate();
            trackBar6.Invalidate();
            textBox23.Text = ini.GetIniValue("밝기값", "밝게횟수");
            textBox22.Text = ini.GetIniValue("밝기값", "밝게횟수");
            textBox24.Text = ini.GetIniValue("선굵기값", "굵게횟수");
            textBox25.Text = ini.GetIniValue("선굵기값", "가늘게횟수");
            comboBox6.Text = ini.GetIniValue("지정한 사분면", "좌표1번사분면");
            comboBox7.Text = ini.GetIniValue("지정한 사분면", "좌표2번사분면");
            comboBox8.Text = ini.GetIniValue("실행 순서", "1단계");
            comboBox9.Text = ini.GetIniValue("실행 순서", "2단계");
            comboBox10.Text = ini.GetIniValue("실행 순서", "3단계");
            comboBox11.Text = ini.GetIniValue("실행 순서", "4단계");
            comboBox12.Text = ini.GetIniValue("실행 순서", "5단계");
            comboBox13.Text = ini.GetIniValue("실행 순서", "6단계");
            comboBox14.Text = ini.GetIniValue("실행 순서", "7단계");            

            Global.source = null;
            Global.sourcebefore = null;
            Global.sourceoriginal = null;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Global.source = (Bitmap)Bitmap.FromFile(path);
            //Global.sourceoriginal = (Bitmap)Bitmap.FromFile(path);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("좌표1번", "X", startX.ToString());
            ini.SetIniValue("좌표1번", "Y", startY.ToString());
            ini.SetIniValue("좌표1번", "가로", width.ToString());
            ini.SetIniValue("좌표1번", "세로", height.ToString());
        }       

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox2.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("좌표2번", "X", startX.ToString());
            ini.SetIniValue("좌표2번", "Y", startY.ToString());
            ini.SetIniValue("좌표2번", "가로", width.ToString());
            ini.SetIniValue("좌표2번", "세로", height.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox3.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("환자이름", "X", startX.ToString());
            ini.SetIniValue("환자이름", "Y", startY.ToString());
            ini.SetIniValue("환자이름", "가로", width.ToString());
            ini.SetIniValue("환자이름", "세로", height.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox4.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("주민번호", "X", startX.ToString());
            ini.SetIniValue("주민번호", "Y", startY.ToString());
            ini.SetIniValue("주민번호", "가로", width.ToString());
            ini.SetIniValue("주민번호", "세로", height.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
            textBox5.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("교부번호", "X", startX.ToString());
            ini.SetIniValue("교부번호", "Y", startY.ToString());
            ini.SetIniValue("교부번호", "가로", width.ToString());
            ini.SetIniValue("교부번호", "세로", height.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
            textBox6.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("의사면허", "X", startX.ToString());
            ini.SetIniValue("의사면허", "Y", startY.ToString());
            ini.SetIniValue("의사면허", "가로", width.ToString());
            ini.SetIniValue("의사면허", "세로", height.ToString());
        }       

        private void button7_Click(object sender, EventArgs e)
        {
            textBox7.Text = "";
            textBox7.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("발행기관", "X", startX.ToString());
            ini.SetIniValue("발행기관", "Y", startY.ToString());
            ini.SetIniValue("발행기관", "가로", width.ToString());
            ini.SetIniValue("발행기관", "세로", height.ToString());
        }             
      
        private void button8_Click_1(object sender, EventArgs e)
        {
            textBox8.Text = "";
            textBox8.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("질병기호", "X", startX.ToString());
            ini.SetIniValue("질병기호", "Y", startY.ToString());
            ini.SetIniValue("질병기호", "가로", width.ToString());
            ini.SetIniValue("질병기호", "세로", height.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());                   

                    ini.SetIniValue("1번 약품명", "X", startX.ToString());
                    ini.SetIniValue("1번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("1번 약품명", "가로", width.ToString());
                    ini.SetIniValue("1번 약품명", "세로", height.ToString());

                    break;
                case 1:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());                    

                    ini.SetIniValue("2번 약품명", "X", startX.ToString());
                    ini.SetIniValue("2번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("2번 약품명", "가로", width.ToString());
                    ini.SetIniValue("2번 약품명", "세로", height.ToString());

                    break;
                case 2:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString()); 
                    
                    ini.SetIniValue("3번 약품명", "X", startX.ToString());
                    ini.SetIniValue("3번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("3번 약품명", "가로", width.ToString());
                    ini.SetIniValue("3번 약품명", "세로", height.ToString());

                    break;
                case 3:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString()); 
                    
                    ini.SetIniValue("4번 약품명", "X", startX.ToString());
                    ini.SetIniValue("4번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("4번 약품명", "가로", width.ToString());
                    ini.SetIniValue("4번 약품명", "세로", height.ToString());

                    break;
                case 4:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("5번 약품명", "X", startX.ToString());
                    ini.SetIniValue("5번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("5번 약품명", "가로", width.ToString());
                    ini.SetIniValue("5번 약품명", "세로", height.ToString());

                    break;
                case 5:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("6번 약품명", "X", startX.ToString());
                    ini.SetIniValue("6번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("6번 약품명", "가로", width.ToString());
                    ini.SetIniValue("6번 약품명", "세로", height.ToString());

                    break;
                case 6:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("7번 약품명", "X", startX.ToString());
                    ini.SetIniValue("7번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("7번 약품명", "가로", width.ToString());
                    ini.SetIniValue("7번 약품명", "세로", height.ToString());

                    break;
                case 7:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("8번 약품명", "X", startX.ToString());
                    ini.SetIniValue("8번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("8번 약품명", "가로", width.ToString());
                    ini.SetIniValue("8번 약품명", "세로", height.ToString());

                    break;
                case 8:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("9번 약품명", "X", startX.ToString());
                    ini.SetIniValue("9번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("9번 약품명", "가로", width.ToString());
                    ini.SetIniValue("9번 약품명", "세로", height.ToString());

                    break;
                case 9:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("10번 약품명", "X", startX.ToString());
                    ini.SetIniValue("10번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("10번 약품명", "가로", width.ToString());
                    ini.SetIniValue("10번 약품명", "세로", height.ToString());

                    break;
                case 10:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("11번 약품명", "X", startX.ToString());
                    ini.SetIniValue("11번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("11번 약품명", "가로", width.ToString());
                    ini.SetIniValue("11번 약품명", "세로", height.ToString());

                    break;
                case 11:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("12번 약품명", "X", startX.ToString());
                    ini.SetIniValue("12번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("12번 약품명", "가로", width.ToString());
                    ini.SetIniValue("12번 약품명", "세로", height.ToString());

                    break;
                case 12:
                    textBox10.Text = "";
                    textBox10.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("13번 약품명", "X", startX.ToString());
                    ini.SetIniValue("13번 약품명", "Y", startY.ToString());
                    ini.SetIniValue("13번 약품명", "가로", width.ToString());
                    ini.SetIniValue("13번 약품명", "세로", height.ToString());

                    break;
            } 

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());
                    
                    ini.SetIniValue("1번 투약량", "X", startX.ToString());
                    ini.SetIniValue("1번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("1번 투약량", "가로", width.ToString());
                    ini.SetIniValue("1번 투약량", "세로", height.ToString());

                    break;
                case 1:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("2번 투약량", "X", startX.ToString());
                    ini.SetIniValue("2번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("2번 투약량", "가로", width.ToString());
                    ini.SetIniValue("2번 투약량", "세로", height.ToString());

                    break;
                case 2:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("3번 투약량", "X", startX.ToString());
                    ini.SetIniValue("3번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("3번 투약량", "가로", width.ToString());
                    ini.SetIniValue("3번 투약량", "세로", height.ToString());

                    break;
                case 3:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("4번 투약량", "X", startX.ToString());
                    ini.SetIniValue("4번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("4번 투약량", "가로", width.ToString());
                    ini.SetIniValue("4번 투약량", "세로", height.ToString());

                    break;
                case 4:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("5번 투약량", "X", startX.ToString());
                    ini.SetIniValue("5번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("5번 투약량", "가로", width.ToString());
                    ini.SetIniValue("5번 투약량", "세로", height.ToString());

                    break;
                case 5:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("6번 투약량", "X", startX.ToString());
                    ini.SetIniValue("6번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("6번 투약량", "가로", width.ToString());
                    ini.SetIniValue("6번 투약량", "세로", height.ToString());

                    break;
                case 6:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("7번 투약량", "X", startX.ToString());
                    ini.SetIniValue("7번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("7번 투약량", "가로", width.ToString());
                    ini.SetIniValue("7번 투약량", "세로", height.ToString());

                    break;
                case 7:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("8번 투약량", "X", startX.ToString());
                    ini.SetIniValue("8번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("8번 투약량", "가로", width.ToString());
                    ini.SetIniValue("8번 투약량", "세로", height.ToString());

                    break;
                case 8:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("9번 투약량", "X", startX.ToString());
                    ini.SetIniValue("9번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("9번 투약량", "가로", width.ToString());
                    ini.SetIniValue("9번 투약량", "세로", height.ToString());

                    break;
                case 9:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("10번 투약량", "X", startX.ToString());
                    ini.SetIniValue("10번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("10번 투약량", "가로", width.ToString());
                    ini.SetIniValue("10번 투약량", "세로", height.ToString());

                    break;
                case 10:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("11번 투약량", "X", startX.ToString());
                    ini.SetIniValue("11번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("11번 투약량", "가로", width.ToString());
                    ini.SetIniValue("11번 투약량", "세로", height.ToString());

                    break;
                case 11:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("12번 투약량", "X", startX.ToString());
                    ini.SetIniValue("12번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("12번 투약량", "가로", width.ToString());
                    ini.SetIniValue("12번 투약량", "세로", height.ToString());

                    break;
                case 12:
                    textBox11.Text = "";
                    textBox11.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("13번 투약량", "X", startX.ToString());
                    ini.SetIniValue("13번 투약량", "Y", startY.ToString());
                    ini.SetIniValue("13번 투약량", "가로", width.ToString());
                    ini.SetIniValue("13번 투약량", "세로", height.ToString());

                    break;
            } 
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("1번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("1번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("1번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("1번 투약횟수", "세로", height.ToString());

                    break;
                case 1:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("2번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("2번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("2번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("2번 투약횟수", "세로", height.ToString());

                    break;
                case 2:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("3번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("3번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("3번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("3번 투약횟수", "세로", height.ToString());

                    break;
                case 3:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("4번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("4번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("4번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("4번 투약횟수", "세로", height.ToString());

                    break;
                case 4:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("5번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("5번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("5번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("5번 투약횟수", "세로", height.ToString());

                    break;
                case 5:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("6번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("6번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("6번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("6번 투약횟수", "세로", height.ToString());

                    break;
                case 6:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("7번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("7번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("7번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("7번 투약횟수", "세로", height.ToString());

                    break;
                case 7:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("8번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("8번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("8번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("8번 투약횟수", "세로", height.ToString());

                    break;
                case 8:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("9번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("9번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("9번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("9번 투약횟수", "세로", height.ToString());

                    break;
                case 9:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("10번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("10번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("10번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("10번 투약횟수", "세로", height.ToString());

                    break;
                case 10:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("11번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("11번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("11번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("11번 투약횟수", "세로", height.ToString());

                    break;
                case 11:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("12번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("12번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("12번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("12번 투약횟수", "세로", height.ToString());

                    break;
                case 12:
                    textBox12.Text = "";
                    textBox12.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("13번 투약횟수", "X", startX.ToString());
                    ini.SetIniValue("13번 투약횟수", "Y", startY.ToString());
                    ini.SetIniValue("13번 투약횟수", "가로", width.ToString());
                    ini.SetIniValue("13번 투약횟수", "세로", height.ToString());

                    break;
            } 
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            switch (comboBox4.SelectedIndex)
            {
                case 0:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());
                    
                    ini.SetIniValue("1번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("1번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("1번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("1번 총투약일수", "세로", height.ToString());

                    break;
                case 1:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("2번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("2번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("2번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("2번 총투약일수", "세로", height.ToString());

                    break;
                case 2:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("3번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("3번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("3번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("3번 총투약일수", "세로", height.ToString());

                    break;
                case 3:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("4번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("4번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("4번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("4번 총투약일수", "세로", height.ToString());

                    break;
                case 4:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("5번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("5번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("5번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("5번 총투약일수", "세로", height.ToString());

                    break;
                case 5:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("6번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("6번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("6번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("6번 총투약일수", "세로", height.ToString());

                    break;
                case 6:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("7번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("7번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("7번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("7번 총투약일수", "세로", height.ToString());

                    break;
                case 7:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("8번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("8번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("8번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("8번 총투약일수", "세로", height.ToString());

                    break;
                case 8:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("9번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("9번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("9번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("9번 총투약일수", "세로", height.ToString());

                    break;
                case 9:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("10번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("10번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("10번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("10번 총투약일수", "세로", height.ToString());

                    break;
                case 10:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("11번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("11번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("11번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("11번 총투약일수", "세로", height.ToString());

                    break;
                case 11:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("12번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("12번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("12번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("12번 총투약일수", "세로", height.ToString());

                    break;
                case 12:
                    textBox13.Text = "";
                    textBox13.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("13번 총투약일수", "X", startX.ToString());
                    ini.SetIniValue("13번 총투약일수", "Y", startY.ToString());
                    ini.SetIniValue("13번 총투약일수", "가로", width.ToString());
                    ini.SetIniValue("13번 총투약일수", "세로", height.ToString());

                    break;
            } 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  
                        
            //pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);

            //pictureBox1.Invalidate();  // picturebox paint 이벤트 불러오기
            //pictureBox1.Dispose();                     

            MessageBox.Show("좌표 저장되었습니다.");            
        }

       // private void pictureBox1_Paint(object sender, PaintEventArgs e)  // 처방 이미지 위에, 설정한 좌표 사각형을 모두 표시한다.
       // {
       //     Bitmap myBitmap = new Bitmap(@"C:\Program Files\PLOCR\prescription.png");
       //     Graphics gs = Graphics.FromImage(myBitmap);
       ////     Rectangle rect = new Rectangle(0, 0, 50, 100);
       //     Rectangle[] multirect = { new Rectangle(0, 0, 100, 200), new Rectangle(350, 90, 400, 200) };
       //     Pen p = new Pen(new SolidBrush(Color.Red));
       //     Pen p1 = new Pen(new SolidBrush(Color.Orange));
       // //    gs.DrawRectangle(p1, rect);
       //     gs.DrawRectangles(p, multirect); //draw more two rectangles 
            
       //     //myBitmap.Dispose();
       //     //gs.Dispose();           
       // }           

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            switch (comboBox5.SelectedIndex)
            {
                case 0:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("1번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("1번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("1번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("1번 약품코드", "세로", height.ToString());

                    break;
                case 1:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("2번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("2번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("2번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("2번 약품코드", "세로", height.ToString());

                    break;
                case 2:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("3번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("3번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("3번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("3번 약품코드", "세로", height.ToString());

                    break;
                case 3:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("4번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("4번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("4번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("4번 약품코드", "세로", height.ToString());

                    break;
                case 4:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("5번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("5번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("5번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("5번 약품코드", "세로", height.ToString());

                    break;
                case 5:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("6번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("6번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("6번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("6번 약품코드", "세로", height.ToString());

                    break;
                case 6:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("7번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("7번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("7번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("7번 약품코드", "세로", height.ToString());

                    break;
                case 7:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("8번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("8번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("8번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("8번 약품코드", "세로", height.ToString());

                    break;
                case 8:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("9번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("9번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("9번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("9번 약품코드", "세로", height.ToString());

                    break;
                case 9:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("10번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("10번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("10번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("10번 약품코드", "세로", height.ToString());

                    break;
                case 10:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("11번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("11번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("11번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("11번 약품코드", "세로", height.ToString());

                    break;
                case 11:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("12번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("12번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("12번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("12번 약품코드", "세로", height.ToString());

                    break;
                case 12:
                    textBox9.Text = "";
                    textBox9.Text += String.Format("{0}:{1}:{2}:{3}", startX.ToString(), startY.ToString(), width.ToString(), height.ToString());

                    ini.SetIniValue("13번 약품코드", "X", startX.ToString());
                    ini.SetIniValue("13번 약품코드", "Y", startY.ToString());
                    ini.SetIniValue("13번 약품코드", "가로", width.ToString());
                    ini.SetIniValue("13번 약품코드", "세로", height.ToString());

                    break;
            } 
        }

        private void button10_Click(object sender, EventArgs e)             // 기준처방전 스캔
        {            
            //System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            //start.FileName = @"C:\Program Files\PLOCR\PLOCRscan.exe";         // 스캔 프로그램 시작
            //start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;   // 윈도우 속성을  windows hidden  으로 지정
            //start.CreateNoWindow = true;  // hidden 을 시키기 위해서 이 속성도 true 로 체크해야 함

            //Process process = Process.Start(start);
            //process.WaitForExit(); // 외부 프로세스가 끝날 때까지 프로그램의 진행을 멈춘다. 

            scan.scanImage();

            MessageBox.Show("스캔이 완료되었습니다.");

            string path = @"C:\Program Files\PLOCR\prescription.png";
            Bitmap source = (Bitmap)Bitmap.FromFile(path);
            Global.source = source;             // 스캔한 이미지를 전역클래스 변수에 담아두기
            Global.sourceoriginal = Global.source;
           // Global.source.Save(@"C:\Program Files\PLOCR\processedprescription.png");

            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();
        }

        private void button11_Click(object sender, EventArgs e)  // 1사분면을 스캔하여 html 포맷으로 판독내용 저장하여 보여주기
        {
            int x = (int) calculator.quadrantX(1);
            int y = 0;

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));
            
            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }            
        }

        private void button12_Click(object sender, EventArgs e)  // 2사분면을 스캔하여 html 포맷으로 판독내용 저장하여 보여주기
        {
            int x = 0;
            int y = 0;

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }            
        }

        private void button13_Click(object sender, EventArgs e) // 3사분면을 스캔하여 html 포맷으로 판독내용 저장하여 보여주기
        {
            int x = 0;
            int y = (int)calculator.quadrantY(3);

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }
        }

        private void button14_Click(object sender, EventArgs e)  // 4사분면을 스캔하여 html 포맷으로 판독내용 저장하여 보여주기
        {
            int x = (int)calculator.quadrantX(4);
            int y = (int)calculator.quadrantY(4);

           // Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }            
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("좌표텍스트", "좌표1번", textBox14.Text);            
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("좌표텍스트", "좌표2번", textBox15.Text);            
        }

        private void button15_Click(object sender, EventArgs e)    // 특정영역1을 읽고 좌표를 저장하고, 변환된 텍스트를 보여준다.
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("특정좌표영역1", "X", startX.ToString());
            ini.SetIniValue("특정좌표영역1", "Y", startY.ToString());
            ini.SetIniValue("특정좌표영역1", "가로", width.ToString());
            ini.SetIniValue("특정좌표영역1", "세로", height.ToString());

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, startX, startY, width, height);

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }
        }

        private void button16_Click(object sender, EventArgs e)      // 1사분면에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = (int)calculator.quadrantX(1);
            int y = 0;

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }

        }

        private void button17_Click(object sender, EventArgs e)   // 2사분면에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = 0;
            int y = 0;

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void button19_Click(object sender, EventArgs e)   // 3사분면에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = 0;
            int y = (int)calculator.quadrantY(3);

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void button18_Click(object sender, EventArgs e)   // 4사분면에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = (int)calculator.quadrantX(4);
            int y = (int)calculator.quadrantX(4);

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, (int)(source.Width / 2), (int)(source.Height / 2));

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {            
            radioButton2.Checked = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {           
            radioButton1.Checked = false;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            ini.SetIniValue("지정한 사분면", "좌표1번사분면", comboBox6.Text);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            ini.SetIniValue("지정한 사분면", "좌표2번사분면", comboBox7.Text);
        }

        private void button20_Click(object sender, EventArgs e)    // 특정영역1에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = int.Parse(ini.GetIniValue("특정좌표영역1", "X"));
            int y = int.Parse(ini.GetIniValue("특정좌표영역1", "Y"));
            int width = int.Parse(ini.GetIniValue("특정좌표영역1", "가로"));            
            int height = int.Parse(ini.GetIniValue("특정좌표영역1", "세로"));

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, width, height);

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void button21_Click(object sender, EventArgs e)             // 전체영역을 스캔하여 html 포맷으로 판독내용 저장하여 보여주기
        {
            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, 0, 0, (int)(source.Width), (int)(source.Height));

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }          
        }

        private void button22_Click(object sender, EventArgs e)             // 전체영역을 읽고 좌표를 저장하고, 변환된 텍스트를 보여준다.
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, 0, 0, (int)(source.Width), (int)(source.Height));

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void button23_Click(object sender, EventArgs e)             // 특정영역2를 읽고 좌표를 저장하고, 변환된 텍스트를 보여준다.
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("특정좌표영역2", "X", startX.ToString());
            ini.SetIniValue("특정좌표영역2", "Y", startY.ToString());
            ini.SetIniValue("특정좌표영역2", "가로", width.ToString());
            ini.SetIniValue("특정좌표영역2", "세로", height.ToString());

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            OcrEngine.hocr(source, startX, startY, width, height);

            try
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\PLOCR\textrecognition.html");
            }
            catch { }
        }

        private void button24_Click(object sender, EventArgs e)                 // 특정영역2에서 좌표1번, 좌표2번 텍스트 탐색하여 좌표 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            int x = int.Parse(ini.GetIniValue("특정좌표영역2", "X"));
            int y = int.Parse(ini.GetIniValue("특정좌표영역2", "Y"));
            int width = int.Parse(ini.GetIniValue("특정좌표영역2", "가로"));
            int height = int.Parse(ini.GetIniValue("특정좌표영역2", "세로"));

            //Bitmap source = (Bitmap)pictureBox1.Image;
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);

            Bitmap source = Global.source;
            string htext = OcrEngine.hocr(source, x, y, width, height);

            if (radioButton1.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표1번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표1번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표1번", "세로", standardHeight.ToString());
            }
            else if (radioButton2.Checked == true)
            {
                int standardX, standardY, standardWidth, standardHeight;
                standardXY.startXY(htext, ini.GetIniValue("좌표텍스트", "좌표2번"), out standardX, out standardY, out standardWidth, out standardHeight);
                ini.SetIniValue("스캔으로 찾은 좌표2번", "X", standardX.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "Y", standardY.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "가로", standardWidth.ToString());
                ini.SetIniValue("스캔으로 찾은 좌표2번", "세로", standardHeight.ToString());
            }
            else
            {
                MessageBox.Show("\"좌표1번 찾기\" 또는 \"좌표2번 찾기\"를 체크해주세요.");
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            redMin = trackBar1.Value;
            textBox16.Text = redMin.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            greenMin = trackBar3.Value;
            textBox17.Text = greenMin.ToString();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            blueMin = trackBar5.Value;
            textBox18.Text = blueMin.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            redMax = trackBar2.Value;
            textBox19.Text = redMax.ToString();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            greenMax = trackBar4.Value;
            textBox20.Text = greenMax.ToString();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            blueMax = trackBar6.Value;
            textBox21.Text = blueMax.ToString();
        }

        private Bitmap RGBfilter(Bitmap source)     // rgb 필터
        {
            // create filter
            ColorFiltering filter = new ColorFiltering();
            // set color ranges to keep
            filter.Red = new IntRange(redMin, redMax);
            filter.Green = new IntRange(greenMin, greenMax);
            filter.Blue = new IntRange(blueMin, blueMax);
            Bitmap processedImage = filter.Apply(source);
            return processedImage;
        }

        private void button25_Click(object sender, EventArgs e)     // 색상 필터값 적용 버튼
        {            
            Bitmap source = Global.source;
            Global.sourcebefore = Global.source;      //  색상 필터 적용 바로 전 이미지 저장해두기(취소할 경우에 쓰려고)
            Global.source = RGBfilter(source);

            pictureBox1.Image = Global.source;
            pictureBox1.Invalidate();
            
            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");        
            //pictureBox1.Image.Save(path);
        }

        private void button26_Click(object sender, EventArgs e)  // 색상 필터값 제거하고 처음 이미지 새로 불러옴
        {
            //string path = @"C:\Program Files\PLOCR\prescription.png";
            //Bitmap source = (Bitmap)Bitmap.FromFile(path);
            pictureBox1.Image = Global.sourcebefore;
            pictureBox1.Invalidate();
        }

        private void button27_Click(object sender, EventArgs e)     // 색상 필터값 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            ini.SetIniValue("색상필터값", "RedMin", textBox16.Text.ToString());
            ini.SetIniValue("색상필터값", "RedMax", textBox19.Text.ToString());
            ini.SetIniValue("색상필터값", "GreenMin", textBox17.Text.ToString());
            ini.SetIniValue("색상필터값", "GreenMax", textBox20.Text.ToString());
            ini.SetIniValue("색상필터값", "BlueMin", textBox18.Text.ToString());
            ini.SetIniValue("색상필터값", "BlueMax", textBox21.Text.ToString());

            MessageBox.Show("색상필터값 저장되었습니다.");
        }

        private void button28_Click(object sender, EventArgs e)     // 색상 반전
        {
            Bitmap source = Global.source;
            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();            

            Invert invertfilter = new Invert();
            invertfilter.ApplyInPlace(source);

            Global.source = source;
            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();

            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");
            //pictureBox1.Image.Save(path);
        }                     

        private void button29_Click_1(object sender, EventArgs e)       // 색상 반전을 통한 배경값을 저장한다.
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            if (radioButton3.Checked == true)
            {               
                ini.SetIniValue("색상 반전값", "배경", "흰배경");
                MessageBox.Show("흰배경으로 저장되었습니다.");
            }
            else if (radioButton4.Checked == true)
            {
                ini.SetIniValue("색상 반전값", "배경", "검은배경"); 
                MessageBox.Show("검은배경으로 저장되었습니다.");
            }
            else
            {
                MessageBox.Show("색상 반전 배경색을 체크해주세요.");
            }
        }

        private void button30_Click(object sender, EventArgs e)    // 실행 순서 저장
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            string path = @"C:\Program Files\PLOCR\prescription.png";
            Bitmap source = (Bitmap)Bitmap.FromFile(path); 

            source = calculator.sequence(source);
            source = calculator.originalcheck(source);
            
            Global.source.Save(@"C:\Program Files\PLOCR\processedprescription.png");
            
            ini.SetIniValue("실행 순서", "1단계", comboBox8.Text);
            ini.SetIniValue("실행 순서", "2단계", comboBox9.Text);
            ini.SetIniValue("실행 순서", "3단계", comboBox10.Text);
            ini.SetIniValue("실행 순서", "4단계", comboBox11.Text);
            ini.SetIniValue("실행 순서", "5단계", comboBox12.Text);
            ini.SetIniValue("실행 순서", "6단계", comboBox13.Text);
            ini.SetIniValue("실행 순서", "7단계", comboBox14.Text);

            MessageBox.Show("실행 순서 저장되었습니다.");
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "1단계", comboBox8.Text);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "2단계", comboBox9.Text);
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "3단계", comboBox10.Text);
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "4단계", comboBox11.Text);
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "5단계", comboBox12.Text);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "6단계", comboBox13.Text);
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("실행 순서", "7단계", comboBox14.Text);
        }

        private void button31_Click(object sender, EventArgs e)         // 밝기값 어둡게 하기, 횟수 기록
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            //Bitmap source = (Bitmap)pictureBox1.Image;       

            Bitmap source = Global.source;
                      
            // create filter
            BrightnessCorrection filter = new BrightnessCorrection(-10);
            // apply the filter
            filter.ApplyInPlace(source);

            Global.source = source;

            pictureBox1.Image = Global.source;
            pictureBox1.Invalidate();

            textBox23.Text = (int.Parse(textBox23.Text) + 1).ToString();
            ini.SetIniValue("밝기값", "어둡게횟수", textBox23.Text.ToString());            

            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");
            //pictureBox1.Image.Save(path);
        }

        private void button32_Click(object sender, EventArgs e)     // 밝기값 밝게 하기, 횟수 기록
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            //Bitmap source = (Bitmap)pictureBox1.Image;

            Bitmap source = Global.source;

            // create filter
            BrightnessCorrection filter = new BrightnessCorrection(+10);
            // apply the filter
            filter.ApplyInPlace(source);

            Global.source = source;

            pictureBox1.Image = Global.source;
            pictureBox1.Invalidate();           
                        
            textBox22.Text = (int.Parse(textBox22.Text) + 1).ToString();
            ini.SetIniValue("밝기값", "밝게횟수", textBox22.Text.ToString());            

            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");
            //pictureBox1.Image.Save(path);
        }

        private void button33_Click(object sender, EventArgs e)     // 밝기값 저장 버튼
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("밝기값", "어둡게횟수", textBox23.Text.ToString());  
            ini.SetIniValue("밝기값", "밝게횟수", textBox22.Text.ToString()); 
            MessageBox.Show("밝기값 저장되었습니다.");
        }

        private void button34_Click(object sender, EventArgs e)     // 선 굵게
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            Bitmap source = Global.source;
            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();            
                     
            Erosion filter = new Erosion();            
            filter.ApplyInPlace(source);

            Global.source = source;
            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();                       

            textBox24.Text = (int.Parse(textBox24.Text) + 1).ToString();           
            ini.SetIniValue("선굵기값", "굵게횟수", textBox24.Text.ToString());        

            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");
            //pictureBox1.Image.Save(path);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            ini.SetIniValue("선굵기값", "굵게횟수", textBox24.Text.ToString());
            ini.SetIniValue("선굵기값", "가늘게횟수", textBox25.Text.ToString());  
            MessageBox.Show("선 굵기 값 저장되었습니다.");
        }

        private void button36_Click(object sender, EventArgs e)     // 선 가늘게
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            Bitmap source = Global.source;
            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();

            Dilatation filter = new Dilatation();
            filter.ApplyInPlace(source);

            Global.source = source;
            pictureBox1.Image = Global.source;
            pictureBox1.Invalidate();        
          
            textBox25.Text = (int.Parse(textBox25.Text) + 1).ToString();
            ini.SetIniValue("선굵기값", "가늘게횟수", textBox25.Text.ToString());        

            //string path = calculator.CreateFileCheck("C:\\Program Files\\PLOCR\\prescription.png");
            //pictureBox1.Image.Save(path);
        }

        private void button37_Click(object sender, EventArgs e)     // 처음 이미지로 초기화
        {
            string path = @"C:\Program Files\PLOCR\prescription.png";
            Global.source = (Bitmap)Bitmap.FromFile(path);
            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();
            textBox22.Text = "0"; 
            textBox23.Text = "0";
            textBox24.Text = "0";
            textBox25.Text = "0";
            textBox26.Text = "0";
            textBox27.Text = "0";                       
        }

        private void button51_Click(object sender, EventArgs e)     // 기울기 보정
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            Bitmap source = Global.source;            

            // create grayscale filter (BT709)
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);       // 8비트 grayscale 로 바꾸고
            // apply the filter
            Bitmap grayImage = filter.Apply(source);

            grayImage.Save(@"C:\\Program Files\\PLOCR\\그레이스케일.png");

            // create instance of skew checker
            DocumentSkewChecker skewChecker = new DocumentSkewChecker();        // 8비트 grayscale 로 넣어줘야 함
            //    // get documents skew angle
            double angle = skewChecker.GetSkewAngle(grayImage);  // 기울어진 각도를 얻고

            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);       // 로테이션 전에 24비트로 바꿔주고
            // delete old image
            tmp.Dispose();           

            // create rotation filter
            RotateBilinear rotationFilter = new RotateBilinear(-angle);
            rotationFilter.FillColor = Color.White;
            // rotate image applying the filter
            Bitmap rotatedImage = rotationFilter.Apply(source);  // 원래 이미지를 가져다가 각도만큼 돌리고(원래 이미지는 24비트로 넣어줘야함)            

            Global.source = rotatedImage;
            pictureBox1.Image = Global.source;
            pictureBox1.Invalidate();

            ini.SetIniValue("기울어짐 바로잡기", "바로잡기 예/아니오", "예");
        }

        private void button52_Click(object sender, EventArgs e)
        {
            MessageBox.Show("기울어짐 바로잡기 값 저장되었습니다.");
        }

        private void button53_Click(object sender, EventArgs e)
        {
            try
            {
                string path = @"C:\Program Files\PLOCR\processedprescription.png";
                Bitmap source = (Bitmap)Bitmap.FromFile(path);
                pictureBox1.Image = source;
                pictureBox1.Invalidate();
            }
            catch(Exception ex)
            {
                MessageBox.Show("실행순서 검증을 먼저 해주세요.");
            }
        }

        private void button54_Click(object sender, EventArgs e)     // 실행순서 검증
        {            
            string path = @"C:\Program Files\PLOCR\prescription.png";
            Bitmap source = (Bitmap)Bitmap.FromFile(path);

            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            string exeNo1 = ini.GetIniValue("실행 순서", "1단계");
            string exeNo2 = ini.GetIniValue("실행 순서", "2단계");
            string exeNo3 = ini.GetIniValue("실행 순서", "3단계");
            string exeNo4 = ini.GetIniValue("실행 순서", "4단계");
            string exeNo5 = ini.GetIniValue("실행 순서", "5단계");
            string exeNo6 = ini.GetIniValue("실행 순서", "6단계");
            string exeNo7 = ini.GetIniValue("실행 순서", "7단계");

            if (exeNo7 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);                
                source = calculator.imgProcess(source, exeNo2);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo3);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo4);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo5);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo6);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
            }
            else if (exeNo6 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo2);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo3);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo4);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo5);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
            }
            else if (exeNo5 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
                source = calculator.imgProcess(source, exeNo2);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
                source = calculator.imgProcess(source, exeNo3);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
                source = calculator.imgProcess(source, exeNo4);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
            }
            else if (exeNo4 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo2);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo3);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
            }
            else if (exeNo3 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
                source = calculator.imgProcess(source, exeNo2);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
            }
            else if (exeNo2 == "끝")
            {
                source = calculator.imgProcess(source, exeNo1);
                pictureBox1.Image = source;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(3000);
            }
            else if (exeNo1 == "끝")
            {
            }
            
            calculator.originalcheck(source);

            Global.source = source;
            pictureBox1.Image = source;
            pictureBox1.Refresh();           

            Global.source.Save(@"C:\Program Files\PLOCR\processedprescription.png");
        }

        private void button55_Click(object sender, EventArgs e)         // closing
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            Bitmap source = Global.source;
            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();                                               
             
            // create filter
            Closing filter = new Closing();
            // apply the filter
            filter.Apply(source);

            Global.source = source;
            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();

            textBox26.Text = (int.Parse(textBox24.Text) + 1).ToString();
            ini.SetIniValue("closing", "closing횟수", textBox26.Text.ToString()); 
        }

        private void button56_Click(object sender, EventArgs e)         // opening
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            Bitmap source = Global.source;
            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();                                               

            // create filter
            Opening filter = new Opening();
            // apply the filter
            filter.Apply(source);

            Global.source = source;
            pictureBox1.Image = Global.source;
            pictureBox1.Refresh();

            textBox27.Text = (int.Parse(textBox24.Text) + 1).ToString();
            ini.SetIniValue("opening", "opening횟수", textBox27.Text.ToString()); 
        }

        private void button57_Click(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            ini.SetIniValue("closing", "closing횟수", textBox26.Text.ToString());
            ini.SetIniValue("opening", "opening횟수", textBox27.Text.ToString()); 
            MessageBox.Show("closing / opening 값 저장되었습니다.");
        }

        private void button58_Click(object sender, EventArgs e)     // 약품코드와 약품명을 포함한 영역을 지정하여 통째로 읽어내고 문자 후처리를 하기 위한 좌표 설정
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            ini.SetIniValue("약품영역지정", "X", startX.ToString());
            ini.SetIniValue("약품영역지정", "Y", startY.ToString());
            ini.SetIniValue("약품영역지정", "가로", width.ToString());
            ini.SetIniValue("약품영역지정", "세로", height.ToString());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            if (checkBox1.Checked)
            {
                ini.SetIniValue("약품영역사용", "사용여부", "1");
            }
            else
            { 
                ini.SetIniValue("약품영역지정", "사용여부", "0"); 
            }
        }

        private void button59_Click(object sender, EventArgs e)     // 데이터 베이스 편집 버튼
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            start.FileName = @"C:\Program Files\PLOCR\PLDB.exe";         // 데이터베이스 편집 프로그램 시작
            //   start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;   // 윈도우 속성을 지정
            //   start.CreateNoWindow = true;  // hidden 을 시키기 위해서 이 속성도 true 로 체크해야 함

            Process process = Process.Start(start);
            //   process.WaitForExit(); // 외부 프로세스가 끝날 때까지 프로그램의 진행을 멈춘다. 
        }       

      
      
        
    }
}
