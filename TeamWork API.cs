using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TeamWork
{
    class Program
    {
		//Author : Prityalok Raman
		//Feel free to use this code. For queries write to me prityalok.raman@hotmail.com
        static HttpWebRequest connection = null;
        static string APIKey = "<Your API Key>";
        static string TeamworkURL = "http://<Your Domain>.teamworkpm.net";
        static bool ForceEntry = false;  // Here we are using this force entry key to add entry forcefully, if we want.
										 // It can be thought as Cheat Sheet !
        
		static string EntryDate { get; set; } // Date should be in format YYYYMMDD(20161204). I have also used this as Cheat Sheet !

        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                if (args[0] == "entry") ForceEntry = true;//CheetSheet One
                if (args[0] != "entry")   // CheetSheet Two for Date 
                {
                    try
                    {
                        DateTime.ParseExact(args[0], "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        EntryDate = args[0];
                        ForceEntry = true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Please refer to cheetsheet for developer.");
                        return;
                    }

                }
                else
                {
                    EntryDate = DateTime.Now.ToString("yyyyMMdd");
                }
            }


            string nameOfTheDay = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("en-GB")).ToLower();

            if ((nameOfTheDay != "sunday" && ForceEntry == true) || (nameOfTheDay != "saturday" && ForceEntry == true))
            {
                PerformEntryOperation();
            }
            else
            {
                Console.WriteLine("Sorry I don't work on weekends ! Bye....");
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(0);
            }
            
            Console.ReadLine();
        }


        private static void PerformEntryOperation()
        {
            Dictionary<Int32, string> EmployeeTiming = new Dictionary<Int32, string>();
            EmployeeTiming.Add(<Your EmpId on Teamwork>, "<Provide time for entry>"); // Example: EmployeeTiming.Add(11120, "13:00"); 
																					  //It will mark ertry for empid 11120 for 1 o'clock.
            // If you have more employees you can add more to this Dictionary.

            foreach (var item in EmployeeTiming)
            {
                TimeSheetEntry(item.Key.ToString(), item.Value.ToString());
            }
        }
        private static void TimeSheetEntry(string personId,string hoursFrom)
        {
            
            Uri url = new Uri(TeamworkURL + "/" + "projects/<Your Project ID>/time_entries.json");
            connection = (HttpWebRequest)WebRequest.Create(url);
            connection.Method = "POST";

            using (var streamWriter = new StreamWriter(connection.GetRequestStream()))
            {
				//In the below JSON I have provided 8 hours only like : "\"hours\":\"8\, you can change as per your requirements
				
                string timeJson = "{\"time-entry\":{\"description\":\"\"," +
                  "\"person-id\":\"" + personId + "\"," +
                  "\"date\":\"" + EntryDate + "\"," +
                  "\"time\":\"" + hoursFrom + "\"," +
                  "\"hours\":\"8\"," +
                  "\"minutes\":\"00\"," +
                  "\"isbillable\":\"1\"}}";

                streamWriter.Write(timeJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                string userpassword = APIKey + ":" + "";
                string encodedAuthorization = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userpassword));

                connection.Headers.Add("Authorization", "Basic " + encodedAuthorization);
                var response = connection.GetResponse();
                var responseStream = response.GetResponseStream();

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                var json = myStreamReader.ReadToEnd();

                Console.WriteLine("Enrty Done!");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
            }

            
        }

		
		//This code will get you all the projects in your TeamWork domain
        private static void GetProjects()
        {
            Uri url = new Uri(TeamworkURL + "/projects.json");
            connection = (HttpWebRequest)WebRequest.Create(url);
            connection.Method = "GET";

            string userpassword = APIKey + ":" + "";
            string encodedAuthorization = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userpassword));

            connection.Headers.Add("Authorization", "Basic " + encodedAuthorization);
            var response = connection.GetResponse();
            var responseStream = response.GetResponseStream();

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var json = myStreamReader.ReadToEnd();

            Console.WriteLine("Response :" + json);

            Console.ReadLine();
        }

    }    
}
