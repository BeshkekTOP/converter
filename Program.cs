using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class TextFileModel
{
    public string[] Lines { get; set; }

    public TextFileModel()
    {
        Lines = new string[0];
    }
}

public class FileHandler
{
    private string filePath;

    public FileHandler(string filePath)
    {
        this.filePath = filePath;
    }

    private bool IsTxtFile()
    {
        return filePath.EndsWith(".txt");
    }

    private bool IsJsonFile()
    {
        return filePath.EndsWith(".json");
    }

    private bool IsXmlFile()
    {
        return filePath.EndsWith(".xml");
    }

    public TextFileModel LoadFile()
    {
        TextFileModel model = new TextFileModel();

        if (IsTxtFile())
        {
            model.Lines = File.ReadAllLines(filePath);
        }
        else if (IsJsonFile())
        {
            string json = File.ReadAllText(filePath);
            model = JsonConvert.DeserializeObject<TextFileModel>(json);
        }
        else if (IsXmlFile())
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TextFileModel));
                model = (TextFileModel)serializer.Deserialize(streamReader);
            }
        }

        return model;
    }

    private string GetFileExtension()
    {
        return Path.GetExtension(filePath);
    }

    public void SaveFile(TextFileModel model)
    {
        if (IsTxtFile() || IsJsonFile() || IsXmlFile())
        {
            string fileExtension = GetFileExtension();

            if (IsTxtFile())
            {
                File.WriteAllLines(filePath, model.Lines);
            }
            else if (IsJsonFile())
            {
                string json = JsonConvert.SerializeObject(model);
                File.WriteAllText(filePath, json);
            }
            else if (IsXmlFile())
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TextFileModel));
                    serializer.Serialize(streamWriter, model);
                }
            }

            Console.WriteLine($"Файл успешно сохранен {fileExtension}");
        }
        else
        {
            Console.WriteLine("Неверный формат файла. Файл не сохранен.");
        }
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Введите путь до файла:");
        string filePath = Console.ReadLine();

        FileHandler fileHandler = new FileHandler(filePath);

        TextFileModel model = fileHandler.LoadFile();
        PrintFileContents(model);

        Console.WriteLine("Нажмите F1, чтобы сохранить файл.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.F1)
            {
                fileHandler.SaveFile(model);
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    private static void PrintFileContents(TextFileModel model)
    {
        Console.WriteLine("Содержимое файла:");
        foreach (string line in model.Lines)
        {
            Console.WriteLine(line);
        }
        Console.WriteLine();
    }
}