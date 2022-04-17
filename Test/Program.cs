// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using com.mobuquity.packer;

Console.Write("Please enter the file location\n");

    var fileLocation = Console.ReadLine();
    var output = com.mobuquity.packer.Packer.Pack(fileLocation);
    Console.Write(output);
    Console.ReadLine();


