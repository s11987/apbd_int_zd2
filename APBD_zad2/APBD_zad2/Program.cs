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
                            studyNew.name = daneStudent[2];
                            studyNew.mode = daneStudent[3];
                            studentNew.fname = daneStudent[0];
                            studentNew.lname = daneStudent[1];
                            studentNew.studies = studyNew;
                            studentNew.indexNumber = daneStudent[4];
                            studentNew.date = daneStudent[5];
                            studentNew.email = daneStudent[6];
                            studentNew.mothersName = daneStudent[7];
                            studentNew.fathersName = daneStudent[8];
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
                File.WriteAllText(@".\export_xyz", info);
            //usuniecie powtorzen
            List<Student> studentListDistinct = StudentList.GroupBy(car => new { car.fname, car.lname, car.indexNumber }).Select(g => g.First()).ToList();

            //teraz na XML
            Uczelnia uczelnia = new Uczelnia();
            List<ActiveStudies> activeStudiesList = new List<ActiveStudies>();
            var activeStudies = studentListDistinct.GroupBy(s => new { s.studies.name }).ToList();
            foreach(var item in activeStudies)
            {
                ActiveStudies activeStudie = new ActiveStudies();
                activeStudie.name = item.Key.name;
                activeStudie.numberOfStudents = item.Count();
                activeStudiesList.Add(activeStudie);
            }
            uczelnia.studenci = studentListDistinct;
            uczelnia.activeStudies = activeStudiesList;

            //convert
            ConvertToXml(uczelnia);



            void ConvertToXml(Uczelnia uczelniaInfo)
            {

                string xmlString = ConvertObjectToXMLString(uczelniaInfo);
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


    public class Uczelnia
    {
       public List<Student> studenci { get; set; }
       public List<ActiveStudies> activeStudies { get; set; }
    }

    public class Student
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public Study studies { get; set; }
       // public string Study { get; set; }
       // public string StudyF { get; set; }
        public string indexNumber { get; set; }
        public string date { get; set; }
        public string email { get; set; }
        public string mothersName { get; set; }
        public string fathersName { get; set; }
    }

        public class Study
        {
            public string name { get; set; }
            public string mode { get; set; }
        }

    public class ActiveStudies
    {
        public string name { get; set; }
        public int numberOfStudents { get; set; }
    }
}
