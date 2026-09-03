using System.IO;
Console.WriteLine("File I0 Demo");


#region Create and write file
FileStream myFile = new FileStream("myFile.txt", FileMode.Create, FileAccess.Write);


StreamWriter myPen = new StreamWriter(myFile);

//lets write
myPen.WriteLine("Hello, My name is Sandeeb, welcome to my JOURNAL");

string hobbie= "";
Console.WriteLine("What is you hobbie?");
hobbie = Console.ReadLine();
myPen.WriteLine(hobbie);

myPen.Close();
myFile.Close();

#endregion



