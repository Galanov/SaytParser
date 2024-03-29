﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Awesomium.Core;
using Awesomium.Windows;
using Awesomium.Windows.Forms;
using ZaraCut.Core;
using System.Data.SqlClient;
using AngleSharp.Parser.Html;
using System.Xml.Linq;
using System.IO;

namespace ZaraCut
{
    public partial class mainForm : Form
    {
        private WebView webView;
        private TabControlAwesomium tCA;
        User user;
        Anketa anketa;
        Result result;
        List<Brand> brands;
        
        private void ClearMemory()
        {
            string folder = System.Environment.CurrentDirectory + "\\Sessions";
            float folderSize = 0.0f;
            DirectoryInfo dI = new DirectoryInfo(folder);
            FileInfo[] files = dI.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                folderSize += file.Length;
            }
            int sizeDirectory = Convert.ToInt32(folderSize) / 1000000;
            //MessageBox.Show((sizeDirectory).ToString());
            if (sizeDirectory>300)
            {
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
        }
        private string VersionId = "2";
        public mainForm()
        {
            InitializeComponent();
            webSessionProvider1.DataPath = System.Environment.CurrentDirectory+"\\Sessions";
            //webView.ShowCreatedWebView += NewPage;
            //WebControl webControl = new WebControl();
            tabControl1.ContextMenuStrip = contextMenuStrip1;
            tCA = new  TabControlAwesomium(tabControl1, webSessionProvider1);
            user = new User();
            result = new Result(infoLabel);
            brands = LoadBrandsFromXML();
            AddBrandsInComboBox(brands);
            //result.Message = "Test";
            //result.MessageColor = Color.Red;
        }

        private void AddBrandsInComboBox(List<Brand> brands)
        {
            foreach (var item in brands)
            {
                brandsComboBox.Items.Add(item.Name);
            }
            brandsComboBox.SelectedIndex = 0;
        }
        
        private List<Brand> LoadBrandsFromXML()
        {
            List<Brand> brands = new List<Brand>();
            string path = "";
            path = Environment.CurrentDirectory + "\\config.xml";
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Load(path);
                foreach (XElement phoneElement in xdoc.Element("brands").Elements("brand"))
                {
                    XElement idElement = phoneElement.Element("id");
                    XElement nameElement = phoneElement.Element("name");
                    XElement visibleElement = phoneElement.Element("visible");

                    if (idElement != null && nameElement != null && visibleElement != null)
                    {
                        Console.WriteLine("Смартфон: {0}", idElement.Value);
                        Console.WriteLine("Компания: {0}", nameElement.Value);
                        Console.WriteLine("Цена: {0}", visibleElement.Value);
                        brands.Add(new Brand() { Id = Convert.ToInt32(idElement.Value), Name = nameElement.Value, Visible = Convert.ToBoolean(visibleElement.Value) });
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("config.xml Файл не найден. Завершение приложения.");
                Close();
            }
            //var sortBrands
            var sortBrands = from u in brands where u.Visible == true orderby u.Name select u;
            //brands.OrderBy(u => u.Name).Where(u=>u.Visible==true);
            brands = null;
            brands = new List<Brand>();
            brands.Add(new Brand() { Name = "-", Visible = true, Id = 0 });
            foreach (Brand brand in sortBrands)
            {
                brands.Add(brand);
            }
            return brands;
        }

        private void saveAnketaButton_Click(object sender, EventArgs e)
        {
            
        }
        private void ParseJobMo()
        {
            WebControl webC;
            try
            {
                webC = (WebControl)tabControl1.SelectedTab.Controls[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            string result = "";
            var parser = new HtmlParser();
            var document = parser.Parse(webC.HTML);
            //var result = document.QuerySelector("[class='items']").InnerHtml;
            var element = document.All.Where(m => m.LocalName == "div" && m.ClassName == "main-box");
            //document.QuerySelectorAll("div.items");
            //.cf
            //document.All.Where(m => m.LocalName == "div" && m.ClassName == "text");
            foreach (var item in element)
            {
                result = item.OuterHtml;
            }

            if (result != "")
            {
                using (PageParser pageParser = new PageParser())
                {
                    anketa = pageParser.ParseJobMo(result);
                }
                if (anketa != null)
                {
                    anketa.Info = dopInfoTextBox.Text;
                    anketa.Brand = brandsComboBox.SelectedIndex;
                    ShowResultOnForm(anketa);
                }
            }
            else
            {
                this.result.Message(Color.Red, "HTML не найден");
            }
        }
        private void ParseSuperJob()
        {
            WebControl webC;
            try
            {
                webC = (WebControl)tabControl1.SelectedTab.Controls[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            string result = "";
            var parser = new HtmlParser();
            var document = parser.Parse(webC.HTML);
            //var result = document.QuerySelector("[class='items']").InnerHtml;
            var element = document.All.Where(m => m.LocalName == "div" && m.ClassName == "ResumeMainHRNew");
            //document.QuerySelectorAll("div.items");
            //.cf
            //document.All.Where(m => m.LocalName == "div" && m.ClassName == "text");
            foreach (var item in element)
            {
                result = item.OuterHtml;
            }

            if (result != "")
            {
                using (PageParser pageParser = new PageParser())
                {
                    anketa = pageParser.ParseSuperJob(result);
                }
                if (anketa != null)
                {
                    anketa.Info = dopInfoTextBox.Text;
                    anketa.Brand = brandsComboBox.SelectedIndex;
                    ShowResultOnForm(anketa);
                }
            }
            else
            {
                this.result.Message(Color.Red, "HTML не найден");
            }
        }

        private void ParseRetailStars()
        {
            WebControl webC;
            try
            {
                webC = (WebControl)tabControl1.SelectedTab.Controls[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            string result="";
            var parser = new HtmlParser();
            var document = parser.Parse(webC.HTML);
            //var result = document.QuerySelector("[class='items']").InnerHtml;
            var element = document.All.Where(m => m.LocalName == "div" && m.ClassName == "small_card");
            //document.QuerySelectorAll("div.items");
            //.cf
            //document.All.Where(m => m.LocalName == "div" && m.ClassName == "text");
            foreach (var item in element)
            {
                result = item.OuterHtml;
            }

            if (result != "")
            {
                using (PageParser pageParser = new PageParser())
                {
                    anketa = pageParser.ParseRetailStar(result);
                }
                if (anketa != null)
                {
                    anketa.Info = dopInfoTextBox.Text;
                    anketa.Brand = brandsComboBox.SelectedIndex;
                    ShowResultOnForm(anketa);
                }
            }
            else
            {
                this.result.Message(Color.Red, "HTML не найден");
            }
            //foreach (var item in collection)
            //{

            //}
        }

        private void ParseHH()
        {
            WebControl webC;
            try
            {
                webC = (WebControl)tabControl1.SelectedTab.Controls[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            var parser = new HtmlParser();
            var document = parser.Parse(webC.HTML);
            var element = document.All.Where(m => m.LocalName == "div" && m.ClassName == "resume-header-main");
            string result = "";
            foreach (var item in element)
            {
                result = item.OuterHtml;
            }
            string result2 = "";
            try
            {
                result2 = document.QuerySelector("[itemprop='nationality']").TextContent;
            }
            catch
            {; }
            string result3 = "";
            try
            {
                result3 = document.QuerySelector("[data-qa='resume-block-salary']").TextContent;
            }
            catch
            {; }
            string result4 = "";
            try
            {
                result4 = document.QuerySelector("[data-qa='resume-block-title-position']").TextContent;
            }
            catch
            {; }
            if (result!="")
            {
                using (PageParser pageParser = new PageParser())
                {
                    anketa = pageParser.ParseHH(result, result2, result3, result4);
                }
                if (anketa != null)
                {
                    anketa.Info = dopInfoTextBox.Text;
                    anketa.Brand = brandsComboBox.SelectedIndex;
                    ShowResultOnForm(anketa);
                }
            }
            else
            {
                this.result.Message(Color.Red, "HTML не найден");
            }
        }

        private void ClearForm()
        {
            foreach (object item in containerGroupBox. Controls)
            {
                if (item is TextBox)
                {
                    TextBox tb = (TextBox)item;
                    if (tb.Name!= "dopInfoTextBox")
                    {
                        tb.Text = "";
                    }
                }
                if (item is RadioButton)
                {
                    RadioButton rb = (RadioButton)item;
                    rb.Checked = false;
                }
            }
        }

        private void ShowResultOnForm(Anketa anketa)
        {
            lastNameTextBox.Text = anketa.LastName;
            middleNameTextBox.Text = anketa.Patronymic;
            nameTextBox.Text = anketa.FirstName;
            mobPhoneTextBox.Text = anketa.MobPhone;
            homePhoneTextBox.Text = anketa.HomePhone;
            cityTextBox.Text = anketa.City;
            emailTextBox.Text = anketa.Email;
            birthdayTextBox.Text = anketa.Birthday;
            #region Gender
            switch (anketa.Gender)
            {
                case 1:
                    {
                        genderMaleRadioButton.Checked = true;
                        break;
                    }
                case 2:
                    {
                        genderWomenRadioButton.Checked = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            #endregion
            #region Nationality
            switch (anketa.Nationality)
            {
                case 1:
                    {
                        nationalityRusRadioButton.Checked = true;
                        break;
                    }
                case 2:
                    {
                        nationalityBelRadioButton.Checked = true;
                        break;
                    }
                case 3:
                    {
                        nationalityKazRadioButton.Checked = true;
                        break;
                    }
                default:
                    {
                        nationalityOtherRadioButton.Checked = true;
                        break;
                    }
            }
            #endregion
            ageTextBox.Text = anketa.Age;
            salaryTextBox.Text = anketa.Salary;
            vacancyTextBox.Text = anketa.Vacancy;
            metroTextBox.Text = anketa.Metro;
        }
        private void createAnketaButton_Click(object sender, EventArgs e)
        {
            anketa = null;
            ClearForm();
            switch (siteComboBox.SelectedIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        ParseHH();
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        ParseJobMo();
                        break;
                    }
                case 5:
                    {
                       
                        break;
                    }
                case 11:
                    {
                        ParseRetailStars();
                        break;
                    }
                case 12:
                    {
                        ParseSuperJob();
                        break;
                    }
                default:
                    break;
            }
        }

        

        private void openSiteButton_Click(object sender, EventArgs e)
        {
            string path = "";
            switch (siteComboBox.SelectedIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        path = "http://www.hh.ru";
                        //anketa = new HHAnketa();
                        tCA.tabPagesName = "hh.ru";
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        path = "http://www.job-mo.ru";
                        tCA.tabPagesName = "Job-Mo";
                        break;
                    }
                case 5:
                    {
                        break;
                    }
                case 11:
                    {
                        path = "http://retailstars.ru";
                        tCA.tabPagesName = "retailstars";
                        break;
                    }
                case 12:
                    {
                        path = "https://www.superjob.ru";
                        tCA.tabPagesName = "superjob";
                        break;
                    }
                default:
                    break;
            }
            if (path!="")
            {
                tCA.OpenPage(path);
            }
            
        }

        private void saveAnketaButton_Click_1(object sender, EventArgs e)
        {
            //tCA.ClearMemory();
            if (anketa!=null)
            {
                using (DataAccess da = new DataAccess(result))
                {
                    da.SaveData(anketa, user);
                } 
                
            }
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tCA.ClearMemory();
            tCA.CloseSelectedTab();
        }
        private void mainForm_Load(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            DialogResult dialogResult = loginForm.ShowDialog();
            if (dialogResult == DialogResult.Cancel)
            {
                base.Close();
            }
            SqlConnection sqlConnection = new SqlConnection(DBPath.SQLPath);
            SqlCommand sqlCommand = new SqlCommand("EXECUTE StartSession 'Soft','" + loginForm.login.Replace("'", "") + "'", sqlConnection);
            try
            {
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    user.IDUser = sqlDataReader["loginID"].ToString();
                    user.IDSession = sqlDataReader["SessionID"].ToString();
                }
                sqlDataReader.Close();
                sqlCommand.CommandText = "SELECT Active FROM Version WHERE VersionId = " + this.VersionId;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    if (sqlDataReader["Active"] == null)
                    {
                        MessageBox.Show("Версия программы забокирована");
                        base.Close();
                        sqlConnection.Close();
                    }
                    else
                    {
                        if (sqlDataReader["Active"].ToString() != "True")
                        {
                            MessageBox.Show("Версия программы забокирована");
                            base.Close();
                            sqlConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Версия программы забокирована");
                    base.Close();
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                MessageBox.Show(String.Concat("Ошибка подключения к БД {0}", ex.ToString()));
                base.Close();
            }
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            ClearMemory();
        }
        protected struct ItemKV
        {
            public string key;
            public string value;
            public ItemKV(string Key, string Value)
            {
                this.key = Key;
                this.value = Value;
            }
            public override string ToString()
            {
                return this.value;
            }
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tCA.ClearMemory();
        }
    }
}
