﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lab_12_variant_1.OpenOrCreateFile
{
    class StreamOpenDataGridView
    {
        static List<ConstructRow> ListStreamOpen = new List<ConstructRow>();
        static LinkedList<Triangle> Triangles = new LinkedList<Triangle>();
        Match regularSeach;
        public enum OpenAs
        {
            txt, xml, html
        }
        public StreamOpenDataGridView(string fileFolder, OpenAs openAs)
        {
            ListStreamOpen.Clear();
            if ( openAs.Equals(OpenAs.txt))
                Triangles = TriangleTransform(CreateListText(fileFolder));

            else if (openAs.Equals(OpenAs.xml))
                Triangles = TriangleTransform(fileFolder);

            else if (openAs.Equals(OpenAs.html))
                Triangles = TriangleTransform(CreateListHtml(fileFolder));

            else throw new ArgumentException();
        }
        public StreamOpenDataGridView(string fileFolder)
        {
            ListStreamOpen.Clear();
            FileInfo fileInfo = new FileInfo(fileFolder);

            if ((fileInfo.Extension == ".txt") || (fileInfo.Extension == ""))
                Triangles = TriangleTransform(CreateListText(fileFolder));

            else if (fileInfo.Extension == ".xml")
                Triangles = TriangleTransform(fileFolder);

            else if (fileInfo.Extension == ".html")
                Triangles = TriangleTransform(CreateListHtml(fileFolder));

            else throw new ArgumentException();
        }
        private List<ConstructRow> CreateListText(string fileFolder)
        {
            StreamReader streamReader = new StreamReader(fileFolder);
            string textFromFile = streamReader.ReadToEnd();
            streamReader.Close();
            Regex regular = new Regex(@"\s*\d+\s*(Area)*(Perimeter)*\s*\d+\,*\.*\d*\r\n");
            regularSeach = regular.Match(textFromFile);
            int counter = CountRowsText(textFromFile);
            while (regularSeach.Success)
            {
                DisasembleRegularSeach(regularSeach.Value);
            }
            ConstructRow constrictRow = new ConstructRow();
            ListStreamOpen.Sort(constrictRow);
            return ListStreamOpen;
        }
        private int CountRowsText(string stringText)
        {
            Regex regularOnlyOne = new Regex(@"(\s*Number\s*Type_Calculation\s*Value\r\n){1}");
            Regex regularRN = new Regex("\r\n");
            Match textRNSeach = regularRN.Match(stringText);
            int counter = 0;
            if (regularOnlyOne.IsMatch(stringText))
            {
                while (textRNSeach.Success)
                {
                    ++counter;
                    textRNSeach = textRNSeach.NextMatch();
                }
            }
            else throw new FormatException();
            return counter;
        }
        private void DisasembleRegularSeach(string str)
        {
            Regex regularString = new Regex(@"Area|Perimeter|\d+\,*\.*\d*");
            Match regularSeachString = regularString.Match(str);
            ConstructRow constructRow = new ConstructRow();
            bool first = true;
            bool second = false;
            while (regularSeachString.Success)
            {
                if (first)
                {
                    constructRow.StrNumber = int.Parse(regularSeachString.Value);
                    first = false;
                    second = true;
                    regularSeachString = regularSeachString.NextMatch();
                }
                if (second)
                {
                    constructRow.StrTypeCalculation = regularSeachString.Value;
                    first = false;
                    second = false;
                    regularSeachString = regularSeachString.NextMatch();
                }
                if (!first && !second)
                {
                    constructRow.StrValue = double.Parse(regularSeachString.Value);
                    regularSeachString = regularSeachString.NextMatch();
                }
                ListStreamOpen.Add(constructRow);
            }
            regularSeach = regularSeach.NextMatch();
        }
        private List<List<int>> CheckTriangleTransform(List<ConstructRow> ListStreamOpen)
        {
            int length = ListStreamOpen.Count;
            List<List<int>> strNumbers = new List<List<int>>();
            int index = 0;
            strNumbers.Add(new List<int>());
            for (int i = 0; i < length; i++)
            {
                if ((i != length - 1) && (ListStreamOpen.ElementAt(i) == ListStreamOpen.ElementAt(i + 1)))
                {
                    if (strNumbers.ElementAt(strNumbers.Count - 1).Count != 0)
                        strNumbers.Add(new List<int>());
                    strNumbers[index].Add(i);
                    strNumbers[index].Add(i + 1);
                    index++;
                    i++;
                }
                else if (index == 0)
                {
                    if (!(strNumbers[index].Contains(i)))
                    {
                        if (strNumbers.ElementAt(strNumbers.Count - 1).Count != 0)
                            strNumbers.Add(new List<int>());
                        strNumbers[index].Add(i);
                        strNumbers[index].Add(i);
                        index++;
                    }
                }
                else if (!(strNumbers[index - 1].Contains(i)))
                {
                    if (strNumbers.ElementAt(strNumbers.Count - 1).Count != 0)
                        strNumbers.Add(new List<int>());
                    strNumbers[index].Add(i);
                    strNumbers[index].Add(i);
                    index++;
                }
            }
            return strNumbers;
        }
        private LinkedList<Triangle> TriangleTransform(List<ConstructRow> ListStreamOpen)
        {
            List<List<int>> strNumbers = CheckTriangleTransform(ListStreamOpen);
            int length = strNumbers.Count;
            for (int i = 0; i < length; i++)
            {
                if (strNumbers[i][0] != strNumbers[i][1])
                {
                    Triangle triangle = new Triangle();
                    triangle.Area = ListStreamOpen.ElementAt(strNumbers[i][0]).StrTypeCalculation.Equals("Area") ?
                        ListStreamOpen.ElementAt(strNumbers[i][0]).StrValue : null;
                    triangle.Perimeter = ListStreamOpen.ElementAt(strNumbers[i][1]).StrTypeCalculation.Equals("Perimeter") ?
                        ListStreamOpen.ElementAt(strNumbers[i][1]).StrValue : null;
                    if (!ListStreamOpen.ElementAt(strNumbers[i][0]).StrTypeCalculation.Equals("Area"))
                        triangle.Perimeter = ListStreamOpen.ElementAt(strNumbers[i][0]).StrTypeCalculation.Equals("Perimeter") ?
                            ListStreamOpen.ElementAt(strNumbers[i][0]).StrValue : null;
                    if (!ListStreamOpen.ElementAt(strNumbers[i][1]).StrTypeCalculation.Equals("Perimeter"))
                        triangle.Perimeter = ListStreamOpen.ElementAt(strNumbers[i][1]).StrTypeCalculation.Equals("Area") ?
                            ListStreamOpen.ElementAt(strNumbers[i][1]).StrValue : null;
                    Triangles.AddLast(triangle);
                }
                else
                {
                    Triangle triangle = new Triangle();
                    triangle.Area = ListStreamOpen.ElementAt(strNumbers[i][0]).StrTypeCalculation.Equals("Area") ?
                          ListStreamOpen.ElementAt(strNumbers[i][0]).StrValue : null;
                    triangle.Perimeter = ListStreamOpen.ElementAt(strNumbers[i][0]).StrTypeCalculation.Equals("Perimeter") ?
                        ListStreamOpen.ElementAt(strNumbers[i][0]).StrValue : null;
                    Triangles.AddLast(triangle);
                }
            }
            return Triangles;
        }
        private LinkedList<Triangle> TriangleTransform(string fileFolder)
        {
            StreamReader stream = new StreamReader(fileFolder);
            XmlReader xmlReader = new XmlTextReader(stream);
            while (xmlReader.Read())
            {
                if (xmlReader.Name == "Triangle")
                {
                    Triangle triangle = new Triangle();
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                    if (xmlReader.Name == "Area")
                    {
                        triangle.Area = double.Parse(xmlReader.ReadElementString());
                    }
                    if (xmlReader.Name == "Perimeter")
                    {
                        triangle.Perimeter = double.Parse(xmlReader.ReadElementString());
                    }
                    Triangles.AddLast(triangle);
                }
            }
            xmlReader.Close();
            return Triangles;
        }
        private List<ConstructRow> CreateListHtml(string fileFolder)
        {
            StreamReader streamReader = new StreamReader(fileFolder);
            string textFromFile = streamReader.ReadToEnd();
            streamReader.Close();
            Regex regular = new Regex(@"\s*(<td>)\s*\d+\s*(<td>Area)*(<td>Perimeter)*\s*(<td>)\d+\,*\.*\d*(</tr>)\s*");
            regularSeach = regular.Match(textFromFile);
            int counter = CountRowsHtml(textFromFile);
            while (regularSeach.Success)
            {
                DisasembleRegularSeach(regularSeach.Value);
            }
            ConstructRow constrictRow = new ConstructRow();
            ListStreamOpen.Sort(constrictRow);
            return ListStreamOpen;
        }
        private int CountRowsHtml(string stringText)
        {
            Regex regularOnlyOne = new Regex
                (@"(\s*(<tr>)\s*(<td>\s*Number\s*<td>\s*Type_Calculation\s*<td>\s*Value\s*</tr>)\s*){1}");
            Regex regularRN = new Regex(@"</tr>");
            Match textRNSeach = regularRN.Match(stringText);
            int counter = -1;
            if (regularOnlyOne.IsMatch(stringText))
            {
                while (textRNSeach.Success)
                {
                    counter++;
                    textRNSeach = textRNSeach.NextMatch();
                }
            }
            else throw new FormatException();
            return counter;
        }
        public List<ConstructRow> ReturnLinkedListStreamOpen
        {
            get
            {
                return ListStreamOpen;
            }
        }
        public LinkedList<Triangle> ReturnTriangles
        {
            get
            {
                return Triangles;
            }
        }
    }
}
