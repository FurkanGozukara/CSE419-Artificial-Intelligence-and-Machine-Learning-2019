using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lecture_11_example_feature_vector
{
    class Program
    {

        public class perDocument
        {
            public string srDocName = "";
            public Dictionary<string, int> dicKeywordCounts = new Dictionary<string, int>();
        }
        public class perVectorDoc
        {
            public string srDocName = "";
            public Dictionary<string, int> dicKeywordCounts = new Dictionary<string, int>();
        }

        static Dictionary<string, int> dicKeywordIndex = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            string srUserSearchedKeyword = "Galaxy communicate enhance found";

            List<perDocument> myDocs = new List<perDocument>();

            foreach (var item in new List<string> { "text1", "text2", "text3" })
            {
                perDocument tempDoc = new perDocument();
                tempDoc.srDocName = item;
                processDoc(File.ReadAllText(item + ".txt"), tempDoc);
                myDocs.Add(tempDoc);
            }

            perDocument tempDoc2 = new perDocument();
            tempDoc2.srDocName = "userSearch";
            processDoc(srUserSearchedKeyword, tempDoc2);

            var vrResults = calculatePoints(myDocs, tempDoc2);

            //usually used algorithm is ID3
            //http://bilgisayarkavramlari.sadievrenseker.com/2012/10/22/tf-idf/

            List<perVectorDoc> vectorDocs = new List<perVectorDoc>();

            vectorizeDocs(myDocs, vectorDocs);
            List<string> lstText = new List<string>();
            int irIndex = 0;
            StringBuilder srIndex = new StringBuilder();
            foreach (var item in vectorDocs.First().dicKeywordCounts)
            {
                dicKeywordIndex.Add(item.Key, irIndex);
                srIndex.AppendLine(irIndex + ";" + item.Key);
                irIndex++;
            }
            lstText.Add(srIndex.ToString());

            foreach (var item in vectorDocs)
            {
                List<string> lstWords = new List<string>();

                for (int i = 0; i < item.dicKeywordCounts.Count; i++)
                {
                    lstWords.Add("");
                }

                    foreach (var vrggg in item.dicKeywordCounts)
                {
                    lstWords[dicKeywordIndex[vrggg.Key]] = vrggg.Value.ToString();
                }

                lstText.Add(item.srDocName + "\t\t" + string.Join(",", lstWords));
            }
            File.WriteAllLines("results.txt", lstText);

        }

        private static void vectorizeDocs(List<perDocument> myDocs, List<perVectorDoc> vectorDocs)
        {
            foreach (var item in myDocs)
            {
                perVectorDoc tempDoc = new perVectorDoc();
                tempDoc.srDocName = item.srDocName;
                vectorDocs.Add(tempDoc);
            }

            foreach (var vrDoc in myDocs)
            {
                foreach (var vrKey in vrDoc.dicKeywordCounts)
                {
                    foreach (var vrVectorDoc in vectorDocs)
                    {
                        if(vrVectorDoc.srDocName== vrDoc.srDocName)
                        {
                            if (vrVectorDoc.dicKeywordCounts.ContainsKey(vrKey.Key))
                            {
                                
                            }
                            else
                            {
                                vrVectorDoc.dicKeywordCounts.Add(vrKey.Key, vrKey.Value);
                            }
                        }
                   else
                        {
                            if (!vrVectorDoc.dicKeywordCounts.ContainsKey(vrKey.Key))
                            {

                                vrVectorDoc.dicKeywordCounts.Add(vrKey.Key, 0);
                            }
                        }
                    }
                }

            }
        }

        class csDocumentScores
        {
            public string srDocName = "";
            public int irDocScore = 0;

            public csDocumentScores(string srName)
            {
                srDocName = srName;
            }
        }

        private static List<csDocumentScores> calculatePoints(List<perDocument> myDocs, perDocument userSearch)
        {
            List<csDocumentScores> lstScores = new List<csDocumentScores>();

            foreach (var vrCompKey in myDocs)
            {
                lstScores.Add(new csDocumentScores(vrCompKey.srDocName));
            }

            foreach (var vrVerKey in userSearch.dicKeywordCounts)
            {
                for (int i = 0; i < myDocs.Count; i++)
                {
                    foreach (var vrCompKey in myDocs[i].dicKeywordCounts)
                    {
                        if (vrVerKey.Key == vrCompKey.Key)
                        {
                            lstScores.Where(pr => pr.srDocName == myDocs[i].srDocName).FirstOrDefault().irDocScore += vrCompKey.Value;
                        }
                    }
                }
            }

            return lstScores.OrderByDescending(pr => pr.irDocScore).ToList();

        }

        private static void processDoc(string srText, perDocument myDoc)
        {
            srText = srText.ToLowerInvariant().Replace(",", "").Replace(".", "").Replace("\n"," ").Replace("\r\n"," ");
            List<string> lstWords = srText.Split(' ').ToList().Where(pr => pr.Trim().Length>1).ToList();
            foreach (var vrWord in lstWords)
            {
                if (myDoc.dicKeywordCounts.ContainsKey(vrWord))
                {
                    myDoc.dicKeywordCounts[vrWord]++;
                }
                else
                    myDoc.dicKeywordCounts.Add(vrWord, 1);
            }
        }



    }
}
