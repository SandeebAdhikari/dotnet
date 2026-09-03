int counter = 1;

if (File.Exists("counter.txt"))
{
    counter = int.Parse(File.ReadAllText("counter.txt"));
}

Console.WriteLine(counter);

counter++;

File.WriteAllText("counter.txt", counter.ToString());