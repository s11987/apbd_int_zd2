using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace APBD_zad2
{
    class Program
    {
        static void Main(string[] args)
        {
            //domyslny adres
            string addressCsv = "data.csv";
            addressCsv = GetAddress(Console.ReadLine(), "data.csv");
            string addressSave = GetAddress(Console.ReadLine(), "result.xml");



            string info = "";
            List<string> listStudentsCsv = new List<string>();
            List<Student> StudentList = new List<Student>();


            //read csv
            try
            {
                using (var reader = new StreamReader(addressCsv))
                {
                    while (!reader.EndOfStream)
                    {
                        listStudentsCsv.Add(reader.ReadLine());
                    }
                    //dzielimy dane przez , w csv
                    foreach (var student in listStudentsCsv)
                    {
                        List<string> daneStudent = new List<string>();
                        int start = 0;
                        for (int i = 0; i < student.Count(f => f == ',') + 1; i++)
                        {
                            if (i == 8)
                            {
                                int end = student.Length;
                                string dane = new String(student.ToCharArray(start, end - start));
                                daneStudent.Add(dane);
                            }
                            else
                            {
                                int end = student.IndexOf(',', start);
                                string dane = new String(student.ToCharArray(start, end - start));
                                start = end + 1;
                                daneStudent.Add(dane);
                            }

                        }
                        bool missing = false;
                        //sprawdzamy czy nie brakuje danych
                        for (int i = 0; i < daneStudent.Count; i++)
                        {
                            if (daneStudent[i] == null || daneStudent[i] == "")
                                missing = true;
                        }
                        if (missing)
                        {
                            info += "error BRAKUJE DANYCH: ";
                            info += student + "\n";
                        }
                        else
                        {
                            //dodanie studenta
                            Student studentNew = new Student();
                            Study studyNew = new Study();
                            studyNew.Name = daneStudent[2];
                            studyNew.Mode = daneStudent[3];
                            studentNew.Fname = daneStudent[0];
                            studentNew.Lname = daneStudent[1];
                            studentNew.Study = studyNew;
                            studentNew.Number = daneStudent[4];
                            studentNew.Date = daneStudent[5];
                            studentNew.Email = daneStudent[6];
                            studentNew.NameM = daneStudent[7];
                            studentNew.NameF = daneStudent[8];
                            StudentList.Add(studentNew);
                        }
                    }

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new ArgumentException("Nie ma takiego pliku");
            }


            //zapisanie tych co nie wpisano:
            if (info != "")
                File.WriteAllText(@"C:\Users\ugryz\Desktop\Log.txt", info);
            //usuniecie powtorzen
            List<Student> studentListDistinct = StudentList.GroupBy(car => new { car.Fname, car.Lname, car.Number }).Select(g => g.First()).ToList();

            //teraz na XML

            ConvertToXml();


            void ConvertToXml()
            {

                string xmlString = ConvertObjectToXMLString(StudentList);
                // Save C# class object into Xml file
                XElement xElement = XElement.Parse(xmlString);
                xElement.Save(@"C:\Users\ugryz\source\repos\NAI_3\jezyk\polski\userDetail.xml");
            }


            string ConvertObjectToXMLString(object classObject)
            {
                string xmlString = null;
                XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(memoryStream, classObject);
                    memoryStream.Position = 0;
                    xmlString = new StreamReader(memoryStream).ReadToEnd();
                }
                return xmlString;
            }


            Console.ReadLine();
        }

        public static string GetAddress(string getInfo, string addr)
        {
            try
            {
                int startAddressCsv = getInfo.IndexOf('"') + 1;
                int endAddressCsv = getInfo.IndexOf('"', startAddressCsv + 1);
                string addressCsv = new String(getInfo.ToCharArray(startAddressCsv, endAddressCsv - startAddressCsv));
                return addressCsv;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                throw new ArgumentException("Podana ścieżka jest niepoprawna");
            }
        }
    }




    public class Student
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public Study Study { get; set; }
       // public string Study { get; set; }
       // public string StudyF { get; set; }
        public string Number { get; set; }
        public string Date { get; set; }
        public string Email { get; set; }
        public string NameM { get; set; }
        public string NameF { get; set; }
    }

        public class Study
        {
            public string Name { get; set; }
            public string Mode { get; set; }
        }
}
