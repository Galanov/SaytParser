using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ZaraCut.Core
{
    public class DataAccess:IDisposable
    {
        Result result;
        public DataAccess(Result result)
        {
            this.result = result;
        }
        public void SaveData(Anketa anketa, User user)
        {
            //if (Convert.ToInt32(this.LoginId) == -1)
            //{
            //    MessageBox.Show("LoginId = -1");
            //    return;
            //}
            //Task taskMessage = new Task(()=>
            //result.Message(Color.Green, "Сохранение..."));
            //taskMessage.Start();
            if (anketa.MobPhone =="" || anketa.City==""||anketa.FirstName==""||anketa.LastName=="")
            {
                MessageBox.Show("Заполнены не все обязательные поля!");
                return;
            }
            
            SqlConnection sqlConnection = new SqlConnection(Path.SQLPath);
            sqlConnection.Open();
            #region GetIdCity
            int idCity = GetIdCity(sqlConnection, anketa.City);
            if (idCity==0)
            {
                sqlConnection.Close();
                MessageBox.Show("Город не распознан");
                result.Message(Color.Red, "Ошибка, Анкета не сохранена. Обратитесь к системноиу администратору.");
                return;
            }
            #endregion
            #region GetIdMetro
            string idmetro = "";
            if (anketa.Metro!="")
            {
                idmetro = GetIdMetro(sqlConnection, anketa.Metro);
            }
            #endregion
            if (anketa.MobPhone.Replace("-", "").Trim().Length + anketa.HomePhone.Replace("-", "").Trim().Length != 0)
            {
                //SqlConnection sqlConnection = new SqlConnection(sql);
                string text = anketa.MobPhone;
                string text2 = anketa.HomePhone;
                string text3 = "";
                //this.ConvertPhone(this.FriendMobPhoneTB.Text);
                if (text.Length > 0)
                {
                    text3 = text3 + " MobPhone like '%" + text + "%'";
                }
                if (text2.Length > 0)
                {
                    string text4 = text3;
                    text3 = string.Concat(new string[]
                    {
                        text4,
                        (text3.Length > 0) ? " OR " : "",
                        " HomePhone like '%",
                        text2,
                        "%'"
                    });
                }
                SqlCommand sqlCommand = new SqlCommand("SELECT count(*) FROM Card WHERE " + text3, sqlConnection);
                try
                {
                    //sqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        if ((int)sqlDataReader[0] > 0)
                        {
                            sqlDataReader.Close();
                            sqlCommand = new SqlCommand(string.Concat(new string[]
                            {
                                "Select CreateDate From Card WHERE ",
                                text3,
                                " and CreateDate=(Select MAX(CreateDate) FROM Card where ",
                                text3,
                                ")"
                            }), sqlConnection);
                            sqlDataReader = sqlCommand.ExecuteReader();
                            DateTime t = DateTime.Today;
                            if (sqlDataReader.Read())
                            {
                                try
                                {
                                    t = (DateTime)sqlDataReader[0];
                                }
                                catch(Exception ex)
                                {
                                    result.Message(Color.Red, "Ошибка, Анкета не сохранена.");
                                }
                            }
                            sqlDataReader.Close();
                            if (t <= DateTime.Today.AddMonths(-3))
                            {
                                sqlCommand = new SqlCommand(string.Concat(new string[]
                                {
                                    "UPDATE Card SET CreateDate=getdate(),SourceId=" + anketa.Source /*SourceId*/ + " WHERE ",
                                    text3,
                                    " and CreateDate=(Select MAX(CreateDate) FROM Card where ",
                                    text3,
                                    ")"
                                }), sqlConnection);
                                sqlCommand.ExecuteNonQuery();
                                MessageBox.Show("Запись выполнена");
                                result.Message(Color.Green, "Анкета сохранена.");
                            }
                            else
                            {
                                MessageBox.Show("Такая карточка уже существует");
                                result.Message(Color.Red, "Анкета не сохранена.");
                            }
                            return;
                        }
                        sqlDataReader.Close();
                    }
                }
                catch(Exception ex)
                {
                    //sqlConnection.Close();
                    //MessageBox.Show("Ошибка №4");
                    //        taskMessage = new Task(() =>
                    //result.Message(Color.Red, "Ошибка, Анкета не сохранена."));
                    //        taskMessage.Start();
                    result.Message(Color.Red, "Ошибка, Анкета не сохранена.");
                    return;
                }
                finally
                {
                    //sqlConnection.Close();
                }
                anketa.Salary = CutString(anketa.Salary, 100);
                anketa.Vacancy = CutString(anketa.Vacancy, 255);
                anketa.Info = CutString(anketa.Info, 255);
                sqlCommand = new SqlCommand(@"EXECUTE SaveAnketa2 @LastName  ,@Name  ,@MiddleName  ,@MobPhone  
,@HomePhone  ,@Email  ,@CityId  ,@Birthday  ,@Sex,@Citizenship  ,@SourceId ,@Age ,@Salary,@Vacancy,@Info ,@StationHomeVal
,@CardId  ,@OutCardId OUTPUT  ,@loginId  ,@SessionId, @visit ,@Brand", sqlConnection);
                SqlParameterCollection parameters = sqlCommand.Parameters;
                parameters.Add("LastName", SqlDbType.VarChar, 50);
                parameters["LastName"].Value = anketa.LastName;
                parameters.Add("Name", SqlDbType.VarChar, 50);
                parameters["Name"].Value = anketa.FirstName;
                parameters.Add("MiddleName", SqlDbType.Text, 50);
                parameters["MiddleName"].Value = anketa.Patronymic;
                parameters.Add("MobPhone", SqlDbType.VarChar, 25);
                parameters["MobPhone"].Value = anketa.MobPhone;
                parameters.Add("HomePhone", SqlDbType.VarChar, 25);
                parameters["HomePhone"].Value = anketa.HomePhone;
                parameters.Add("Email", SqlDbType.VarChar, 30);
                parameters["Email"].Value = anketa.Email;
                parameters.Add("CityId", SqlDbType.Int);
                parameters["CityId"].Value = idCity;
                parameters.Add("Brand", SqlDbType.Int);
                parameters["Brand"].Value = anketa.Brand;
                //parameters.Add("CardSourceId", SqlDbType.Int);
                //parameters["CardSourceId"].Value = anketa.Source;
                parameters.Add("StationHomeVal", SqlDbType.VarChar, 50);
                parameters["StationHomeVal"].Value = idmetro;
                parameters.Add("Age", SqlDbType.VarChar, 50);
                parameters["Age"].Value = anketa.Age;
                parameters.Add("Salary", SqlDbType.VarChar, 100);
                parameters["Salary"].Value = anketa.Salary;
                parameters.Add("Vacancy", SqlDbType.VarChar, 255);
                parameters["Vacancy"].Value = anketa.Vacancy;
                parameters.Add("Info", SqlDbType.VarChar, 255);
                parameters["Info"].Value = anketa.Info;
                parameters.Add("Birthday", SqlDbType.DateTime);
                if (anketa.Birthday.Length == 0)
                {
                    parameters["Birthday"].Value = DBNull.Value;
                }
                else
                {
                    parameters["Birthday"].Value = Convert.ToDateTime(anketa.Birthday);
                }
                parameters.Add("Sex", SqlDbType.Int);
                parameters["Sex"].Value = anketa.Gender;
                    //(this.SexRB_V1.Checked ? "1" : (this.SexRB_V2.Checked ? "2" : "-1"));
                parameters.Add("Citizenship", SqlDbType.Int);
                parameters["Citizenship"].Value = anketa.Nationality;
                    //(this.CitRB_V1.Checked ? "1" : (this.CitRB_V2.Checked ? "2" : (this.CitRB_V3.Checked ? "3" : (this.CitRB_V4.Checked ? "4" : "-1"))));
                parameters.Add("SourceId", SqlDbType.Int);
                parameters["SourceId"].Value = anketa.Source;
                //parameters.Add("Study", SqlDbType.Int);
                //parameters["Study"].Value = "-1";
                //parameters.Add("StationStudy", SqlDbType.VarChar, 50);
                //parameters["StationStudy"].Value = "";
                //parameters.Add("OldJob", SqlDbType.Int);
                //parameters["OldJob"].Value = "-1";
                //parameters.Add("OldJobVal", SqlDbType.VarChar, 50);
                //parameters["OldJobVal"].Value = "";
                //parameters.Add("ReqPost", SqlDbType.VarChar, 50);
                //parameters["ReqPost"].Value = "";
                //parameters.Add("ReqTPVal", SqlDbType.VarChar, 50);
                //parameters["ReqTPVal"].Value = "";
                //parameters.Add("ReqTPStationVal", SqlDbType.VarChar, 50);
                //parameters["ReqTPStationVal"].Value = "";
                //parameters.Add("ReqTime", SqlDbType.Int);
                //parameters["ReqTime"].Value = "-1";
                //parameters.Add("Experience", SqlDbType.Int);
                //parameters["Experience"].Value = "-1";
                //parameters.Add("TargetExperience", SqlDbType.Int);
                //parameters["TargetExperience"].Value = "-1";
                //parameters.Add("ExperienceMonth", SqlDbType.Int);
                //parameters["ExperienceMonth"].Value = "-1";
                //parameters.Add("ExperienceYear", SqlDbType.Int);
                //parameters["ExperienceYear"].Value = "-1";
                //parameters.Add("Resume", SqlDbType.Text);
                //parameters["Resume"].Value = this.ResumeTB.Text;
                //parameters.Add("InvitationOffice", SqlDbType.Int);
                //parameters["InvitationOffice"].Value = "-1";
                //parameters.Add("InterviewDate", SqlDbType.DateTime);
                //parameters["InterviewDate"].Value = DBNull.Value;
                //parameters.Add("InterviewTime", SqlDbType.VarChar, 5);
                //parameters["InterviewTime"].Value = "00:00";
                parameters.Add("Visit", SqlDbType.Int);
                parameters["Visit"].Value = "-1";
                //parameters.Add("InterviewStatus", SqlDbType.Int);
                //parameters["InterviewStatus"].Value = "-1";
                //parameters.Add("InterviewerId", SqlDbType.Int);
                //parameters["InterviewerId"].Value = "-1";
                //parameters.Add("InterviewerComment", SqlDbType.VarChar, 2000);
                //parameters["InterviewerComment"].Value = "";
                //parameters.Add("PostPanelVal", SqlDbType.VarChar, 50);
                //parameters["PostPanelVal"].Value = "";
                //parameters.Add("Brend", SqlDbType.VarChar, 50);
                //parameters["Brend"].Value = "";
                //parameters.Add("TPValue", SqlDbType.VarChar, 50);
                //parameters["TPValue"].Value = "";
                //parameters.Add("Mode", SqlDbType.Char, 2);
                //parameters["Mode"].Value = "";
                //parameters.Add("Comment", SqlDbType.VarChar, 2000);
                //parameters["Comment"].Value = "";
                //parameters.Add("MakeOffer", SqlDbType.Int);
                //parameters["MakeOffer"].Value = "-1";
                //parameters.Add("Answ", SqlDbType.Int);
                //parameters["Answ"].Value = "-1";
                //parameters.Add("Cause", SqlDbType.Int);
                //parameters["Cause"].Value = "-1";
                //parameters.Add("CauseOther", SqlDbType.VarChar, 100);
                //parameters["CauseOther"].Value = "";
                //parameters.Add("CardStatusId", SqlDbType.Int);
                //parameters["CardStatusId"].Value = "-1";
                parameters.Add("CardId", SqlDbType.Int);
                parameters["CardId"].Value = "-" + user.IDSession;//this.SessionId;
                SqlParameter sqlParameter = new SqlParameter("OutCardId", SqlDbType.Int);
                sqlParameter.Direction = ParameterDirection.Output;
                parameters.Add(sqlParameter);
                parameters.Add("loginId", SqlDbType.Int);
                parameters["loginId"].Value = user.IDUser;// LoginId;
                parameters.Add("SessionId", SqlDbType.Int);
                parameters["SessionId"].Value = user.IDSession;//this.SessionId;
                //parameters.Add("TimeTableGroupId", SqlDbType.Int);
                //parameters["TimeTableGroupId"].Value = -1;
                //parameters.Add("TimeTableId", SqlDbType.Int);
                //parameters["TimeTableId"].Value = -1;
                //parameters.Add("CauseFailure", SqlDbType.VarChar, 50);
                //parameters["CauseFailure"].Value = "";
                //sqlConnection.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    MessageBox.Show("Анкета сохранена");
                    result.Message(Color.Green, "Анкета сохранена.");
                    //taskMessage = new Task(() =>
                    //    result.Message(Color.Green, "Анкета сохранена."));
                    //taskMessage.Start();
                }
                catch (Exception ex)
                {
                    //taskMessage = new Task(() =>
                    //    result.Message(Color.Red, "Ошибка. Анкета не сохранена."));
                    //taskMessage.Start();
                    result.Message(Color.Red, "Ошибка. Анкета не сохранена.");
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public int GetIdCity(SqlConnection sqlConnection, string city)
        {
            int idCity=0;
            SqlCommand sqlCommand = new SqlCommand("select isnull(max(cityid),0) Cityid from city where city ='" + city.Trim() + "'", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                while (sqlDataReader.Read())
                {
                    idCity = Convert.ToInt32(sqlDataReader["Cityid"].ToString());
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return idCity;
        }
        private string CutString(string text, int length)
        {
            if (text.Length>length)
            {
                text = text.Substring(0, length-1);
            }
            return text;
        }
        private string GetIdMetro(SqlConnection sqlConnection, string metro)
        {
            string idMetro = "";
            SqlCommand sqlCommand = new SqlCommand("select isnull(max(stationid),0) stationid from station where station ='" + metro.Trim() + "'", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                while (sqlDataReader.Read())
                {
                    idMetro = sqlDataReader["stationid"].ToString();
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            idMetro = idMetro + ",";
            return idMetro;
        }
        public void Dispose()
        {
        }
    }
}
