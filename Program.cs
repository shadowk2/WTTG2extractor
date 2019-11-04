using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.IO;

namespace WTTG2ex
{
    class Program
    {
        static void Main(string[] args)
        {
            //exract files from "browser_assets"
            using (BinaryReader file = new BinaryReader(File.Open("browser_assets", FileMode.Open)))
            {
                byte indexnamelen = file.ReadByte();
                char[] indexnamec = file.ReadChars(indexnamelen);
                string indexname = new string(indexnamec);
                int entrycnt = file.ReadInt32();
                FILEDATA[] item = new FILEDATA[entrycnt];
                for (int i = 0; i < entrycnt; i++)
                {
                    byte namelen = file.ReadByte();
                    char[] namec = file.ReadChars(namelen);
                    string names = new string(namec);
                    item[i].name = names;
                    item[i].offset = file.ReadInt64();
                    item[i].length = file.ReadInt32();
                    byte[] data = new byte[item[i].length];
                    long pos = file.BaseStream.Position;
                    file.BaseStream.Position = (int)item[i].offset;
                    file.Read(data, 0, item[i].length);
                    item[i].data = data;
                    file.BaseStream.Position = pos;
                }

                for (int i = 0; i < entrycnt; i++)
                {
                    string[] name = item[i].name.Split('/');
                    string dirname = "sites";
                    for (int j = 0; j < name.Length - 1; j++)
                    {
                        dirname += "\\" + name[j];
                    }
                    Directory.CreateDirectory(dirname);
                    using (BinaryWriter file2 = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + "/sites" + item[i].name, FileMode.Create)))
                    {
                        file2.Write(item[i].data);
                    }
                }
            }

            //find all folders and append the "notes.txt" modification to the style.css in every folder
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + "/sites");
            for(int i = 0; i < dirs.Length; i++)
            {
                using (BinaryReader file4 = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + "/notes.txt", FileMode.Open)))
                {
                    long testinf = new FileInfo(Directory.GetCurrentDirectory() + "/notes.txt").Length;
                    char[] test = file4.ReadChars((int)testinf);
                    using (BinaryWriter file3 = new BinaryWriter(File.Open(dirs[i] + "/style.css", FileMode.Append)))   //!!!doesn't modify ann-styles.css
                    {
                        file3.Write(test);
                    }
                }
                Console.Write("test" + i + '\n');
            }

            //add nexus links, replace click css with href and add styles to html that don't include a style.css
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] files = Directory.GetFiles(dirs[i]);
                for(int j = 0; j < files.Length; j++)
                {
                    if (files[j].Contains(".html"))
                    {
                        string f5string;
                        using (var file5 = new BinaryReader(File.Open(files[j], FileMode.Open)))
                        {
                            long f5len = new FileInfo(files[j]).Length;
                            char[] f5char = file5.ReadChars((int)f5len);
                            f5string = new string(f5char);
                        }
                        f5string = "<a href=\"../nexus.html\">Back</a>" + f5string + "<a href=\"../nexus.html\">Back</a>";
                        f5string = f5string.Replace("onClick=\"LinkClick(\'", "href=\"");
                        f5string = f5string.Replace("\')", "");

                        using (var file5 = new BinaryWriter(File.Open(files[j], FileMode.Create)))
                        {
                            file5.Write(f5string.ToCharArray());
                            if(f5string.IndexOf("style.css") == -1)
                            {
                                string notes;
                                using (BinaryReader file4 = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + "/notes.txt", FileMode.Open)))
                                {
                                    long testinf = new FileInfo(Directory.GetCurrentDirectory() + "/notes.txt").Length;
                                    notes = new string(file4.ReadChars((int)testinf));
                                }
                                string wstring = "<style>" + notes + "</style>";
                                file5.Write(wstring.ToCharArray());
                            }
                        }
                    }
                }
            }

            //write nexus.html
            using (var file6 = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + "/sites/nexus.html", FileMode.Create)))
            {
                file6.Write("<table style=\"width: 25 % \"> <tr> <td>\n".ToCharArray());
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirs[i] = dirs[i].Replace(Directory.GetCurrentDirectory(), "");
                    dirs[i] = dirs[i].Replace("\\", "/");
                    dirs[i] = dirs[i].Replace("/sites/", "");
                    string wstring = "<a href=\"" + dirs[i] + "/index.html\">" + dirs[i] + "<a/> <br>\n";
                    file6.Write(wstring.ToCharArray());
                    if (i == ((dirs.Length - 1)  / 2)) file6.Write(" </td> <td>\n".ToCharArray());
                }
                file6.Write("</td> </tr> </table>\n<style> body{background-color: #bbbbbb} </style>".ToCharArray());
            }

            Console.Write("Done!");
            //Console.ReadKey();
        }

        
    }

    //file informations in the "browser_assets"
    struct FILEDATA
    {
        public string name;
        public long offset;
        public int length;
        public byte[] data;
    }
}
