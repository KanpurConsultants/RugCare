﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Service;
using Data.Models;
using System.Configuration;
using Mailer;
using Core.Common;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Mailer.Model;
using System.Linq;

namespace EmailContents
{

    public class DailyEveningContent
    {
        DataTable Dt = new DataTable();
        List<EMailContent> EMailLst;

        public void SendEMail()
        {

            EMailLst = new List<EMailContent>();

            try
            {
                using (StreamWriter writer =
                 new StreamWriter(@"c:\temp\Cachecallback.txt", true))
                {
                    writer.WriteLine("Calling Daily Evening Event"+ DateTime.Now.ToString(), DateTime.Now);
                    writer.Close();
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }




            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString()))
            {

                SqlDataAdapter sqlDataAapter = new SqlDataAdapter("Web.spEMail_DailyEvening", sqlConnection);
                sqlDataAapter.Fill(Dt);

            }


            if (Dt.Rows.Count > 0)
            {
                for ( int i = 0; i< Dt.Rows.Count ;i++)
                {
                    if (Dt.Rows[i]["ReportName"].ToString() == "")
                    {
                        CreateFile(Dt.Rows[i]["ProcedureName"].ToString(), Dt.Rows[i]["FileName"].ToString(), Dt.Rows[i]["EmailSubject"].ToString(), Dt.Rows[i]["EmailToStr"].ToString(), Dt.Rows[i]["EmailCCStr"].ToString(), Dt.Rows[i]["EmailBCCStr"].ToString(), Dt.Rows[i]["EmailBody"].ToString(), Dt.Rows[i]["ParameterStr"].ToString());
                        //CreateFile(Dt.Rows[i]["ProcedureName"].ToString(), Dt.Rows[i]["FileName"].ToString(), Dt.Rows[i]["EmailSubject"].ToString(), "Satyam.tripathi07@gmail.com", "Satyam.tripathi07@gmail.com", "Satyam.tripathi07@gmail.com", Dt.Rows[i]["EmailBody"].ToString(), Dt.Rows[i]["ParameterStr"].ToString());
                    }
                    else
                    {
                        CreateFileFromSQLReporting(Dt.Rows[i]["ReportName"].ToString(), Dt.Rows[i]["FileName"].ToString(), Dt.Rows[i]["EmailSubject"].ToString(), Dt.Rows[i]["EmailToStr"].ToString(), Dt.Rows[i]["EmailCCStr"].ToString(), Dt.Rows[i]["EmailBCCStr"].ToString(), Dt.Rows[i]["EmailBody"].ToString(), Dt.Rows[i]["ParameterStr"].ToString(), Dt.Rows[i]["Parameter"].ToString(), Dt.Rows[i]["ParameterValue"].ToString());
                        //CreateFileFromSQLReporting(Dt.Rows[i]["ReportName"].ToString(), Dt.Rows[i]["FileName"].ToString(), Dt.Rows[i]["EmailSubject"].ToString(), "Satyam.tripathi07@gmail.com", "Satyam.tripathi07@gmail.com", "Satyam.tripathi07@gmail.com", Dt.Rows[i]["EmailBody"].ToString(), Dt.Rows[i]["ParameterStr"].ToString(), Dt.Rows[i]["Parameter"].ToString(), Dt.Rows[i]["ParameterValue"].ToString());
                    }
                                      
                   
                }
            }

            //Start For Test
            //EMailContent A = new EMailContent();
            //A.Subject = "THis is Subkect";
            //A.Body = "THis is Body";
            //A.ToAddress = "satyam.tripathi07@gmail.com";
            //A.CCAddress = "meet2arpitg@gmail.com";
            //A.BCCAddress = "singh.akash409@gmail.com";

            //EMailLst.Add(A);
            //End For Test

            var EMail = from p in EMailLst
                        group p by new
                        {
                            p.Subject ,
                            p.Body, 
                            p.ToAddress,
                            p.CCAddress,
                            p.BCCAddress,
                        } into g
                        select new 
                        {
                          Subject=  g.Key.Subject,
                          Body = g.Key.Body,
                          ToAddress = g.Key.ToAddress,
                          CCAddress =  g.Key.CCAddress,
                          BCCAddress = g.Key.BCCAddress,
                        };


            if (EMail.ToList().Count > 0 && EMailLst.ToList().Count > 0 )
            {
                foreach (var value in EMail)
                {
                    string FilenameList ="";

                    EmailMessage message = new EmailMessage();
                    message.Subject = value.Subject;
                    message.To = value.ToAddress;
                    message.CC = value.CCAddress ;
                    message.BCC  = value.BCCAddress;
                    message.Body = value.Body;

                    foreach (var Filevalue in EMailLst.Where(m => m.Subject == value.Subject && m.ToAddress == value.ToAddress && m.CCAddress == value.CCAddress && m.BCCAddress == value.BCCAddress && m.Body == value.Body).ToList())
                    {
                        if (FilenameList == "")
                        {
                            if (Filevalue.FileName.Contains(".pdf"))
                            {
                                FilenameList = Filevalue.FileName;
                            }
                        }
                        else
                        {
                            if (Filevalue.FileName.Contains(".pdf"))
                            {
                                FilenameList = FilenameList + ", " + Filevalue.FileName;
                            }
                        }

                    }

                    if ( FilenameList != "" )
                    SendEmail.SendEmailMsgWithAttachment(message, FilenameList);
                   
                }
            }




        }



        public void CreateFile(string StrProcedure, string FileName, string Subject, string ToAddress, string CCAddress, string BCCAddress, string Body, string FilterParameter)
        {

            try
            {


                string GenFileName = ReportFiles.CreateFiles(StrProcedure, FileName, FilterParameter);

                using (StreamWriter writer =
                new StreamWriter(@"c:\temp\Cachecallback.txt", true))
                {
                    writer.WriteLine(GenFileName);
                    writer.Close();
                }

                EMailLst.Add(new EMailContent() { Subject = Subject, ToAddress = ToAddress, CCAddress = CCAddress, BCCAddress = BCCAddress, Body = Body, FileName = GenFileName });

            }

            catch (Exception ex)
            {
                return;
            }

        }


        public void CreateFileFromSQLReporting(string ReportName, string FileName, string Subject, string ToAddress, string CCAddress, string BCCAddress, string Body, string FilterParameter, string Parameter, string ParameterValue)
        {

            try
            {



                string GenFileName = ReportFiles.CreateFilesFromSQLReporting(ReportName, FileName, FilterParameter,"System", Parameter, ParameterValue);

                using (StreamWriter writer =
                new StreamWriter(@"c:\temp\Cachecallback.txt", true))
                {
                    writer.WriteLine(GenFileName);
                    writer.Close();
                }

                EMailLst.Add(new EMailContent() { Subject = Subject, ToAddress = ToAddress, CCAddress = CCAddress, BCCAddress = BCCAddress, Body = Body, FileName = GenFileName });

            }

            catch (Exception ex)
            {
                return;
            }

        }


    }
}
