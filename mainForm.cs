using System;
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
using ZaraCut.Core.HH;
using AngleSharp.Parser.Html;

namespace ZaraCut
{
    public partial class mainForm : Form
    {
        private WebView webView;
        private TabControlAwesomium tCA;
        User user;
        Anketa anketa;

        
        private string VersionId = "2";
        public mainForm()
        {
            InitializeComponent();
            webSessionProvider1.DataPath = System.Environment.CurrentDirectory;
            //webView.ShowCreatedWebView += NewPage;
            //WebControl webControl = new WebControl();
            tabControl1.ContextMenuStrip = contextMenuStrip1;
            tCA = new  TabControlAwesomium(tabControl1, webSessionProvider1);
            user = new User();
        }

        private void saveAnketaButton_Click(object sender, EventArgs e)
        {
            
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

            using (PageParser pageParser = new PageParser())
            {
                anketa = pageParser.ParseHH(result, result2);
            }
            if (anketa != null)
            {
                ShowResultOnForm(anketa);
            }
        }

        private void ClearForm()
        {
            lastNameTextBox.Text = "";
            middleNameTextBox.Text = "";
            nameTextBox.Text = "";
            mobPhoneTextBox.Text = "";
            homePhoneTextBox.Text = "";
            cityTextBox.Text = "";
            emailTextBox.Text = "";
            birthdayTextBox.Text = "";
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
            //switch (anketa.Gender)
            //{
            //    case 1:
            //        {
            //            genderCheckedListBox.SelectedIndex = -1;
            //            break;
            //        }
            //    case 2:
            //        {
            //            genderCheckedListBox.SelectedIndex = -1;
            //            break;
            //        }
            //    default:
            //        {
            //            genderCheckedListBox.SelectedIndex = -1;
            //            break;
            //        }
            //}
            //switch (anketa.Nationality)
            //{
            //    case 1:
            //        {
            //            nationalityCheckListBox.SelectedIndex = 0;
            //            break;
            //        }
            //    case 2:
            //        {
            //            nationalityCheckListBox.SelectedIndex = 1;
            //            break;
            //        }
            //    case 3:
            //        {
            //            nationalityCheckListBox.SelectedIndex = 2;
            //            break;
            //        }
            //    default:
            //        {
            //            nationalityCheckListBox.SelectedIndex = 3;
            //            break;
            //        }
            //}
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
                        break;
                    }
                case 5:
                    {
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
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                case 5:
                    {
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
            tCA.ClearMemory();
            if (anketa!=null)
            {
                DataAccess da = new DataAccess();
                da.SaveData(anketa, user);
            }
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            SqlConnection sqlConnection = new SqlConnection(Path.SQLPath);
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

        private void genderCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
