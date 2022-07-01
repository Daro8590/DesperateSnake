using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[XmlRoot]
public class HighscoreInfo
{
    public int highscore;
    public HighscoreInfo() { }
    public HighscoreInfo(int score)
    {
        highscore = score;
    }

    public new string ToString() => highscore.ToString(); 
}

public class PlayerDataSerializer
{
    public static void SerializePlayerInfo(HighscoreInfo hs, string filename)
    {
        XmlSerializer serializer =
            new XmlSerializer(typeof(HighscoreInfo));

        Stream myStream = new FileStream(filename, FileMode.Create, FileAccess.Write);

        serializer.Serialize(myStream, hs);
        myStream.Close();
    }

    public static HighscoreInfo DeserializePlayerInfo(string filename)
    {
        // Create an instance of the XmlSerializer class;
        // specify the type of object to be deserialized.
        XmlSerializer serializer = new XmlSerializer(typeof(HighscoreInfo));
        /* If the XML document has been altered with unknown
        nodes or attributes, handle them with the
        UnknownNode and UnknownAttribute events.*/
        serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
        serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

        if (!PlayerInfoFileExists(filename))
        {
            CreateFile(filename);
        }

        // A FileStream is needed to read the XML document.
        FileStream fs = new FileStream(filename, FileMode.Open);
        /* Use the Deserialize method to restore the object's state with
        data from the XML document. */
        HighscoreInfo hs = (HighscoreInfo)serializer.Deserialize(fs);
        fs.Close();
        return hs;
    }

    private static void serializer_UnknownNode
    (object sender, XmlNodeEventArgs e)
    {
        Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
    }

    private static void serializer_UnknownAttribute
    (object sender, XmlAttributeEventArgs e)
    {
        System.Xml.XmlAttribute attr = e.Attr;
        Console.WriteLine("Unknown attribute " +
        attr.Name + "='" + attr.Value + "'");
    }

    public static bool PlayerInfoFileExists(string fileName)
    {
        return new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles(fileName).Any();
    }

    public static void CreateFile(string fileName)
    {
        SerializePlayerInfo(new HighscoreInfo(0), fileName);
    }
}