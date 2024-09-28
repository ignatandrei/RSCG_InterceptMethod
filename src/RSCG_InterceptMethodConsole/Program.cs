using RSCG_InterceptMethodConsole;
Person person = new ();
person.FirstName = "Andrei";
person.LastName = "Ignat";
//Console.WriteLine(await person.FullName(5000) + await person.FullName(3000));
Console.WriteLine(await person.FullName(5000));
Console.WriteLine(await Person.GetFullStaticName(person, 2000));